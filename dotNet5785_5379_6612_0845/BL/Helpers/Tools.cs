using BL.BO;
using DalApi;
using DO;
using System.Net.Mail;
using System.Net;
using System.Text.Json;
using Helpers;

namespace BL.Helpers;

static internal class Tools
{
    private static IDal s_dal = Factory.Get; //stage 4

    /// <summary>
    /// Retrieves the geographical coordinates (latitude and longitude) for a given address using LocationIQ API.
    /// </summary>
    /// <param name="address">The address to get coordinates for.</param>
    /// <returns>A tuple containing latitude and longitude.</returns>
    /// <exception cref="Exception">Thrown when the API response is invalid or the address is not found.</exception>
    public static (double, double) GetCoordinatesFromAddress(string address)
    {
        string apiKey = "PK.83B935C225DF7E2F9B1ee90A6B46AD86";
        using var client = new HttpClient();
        string url = $"https://us1.locationiq.com/v1/search.php?key={apiKey}&q={Uri.EscapeDataString(address)}&format=json";

        var response = client.GetAsync(url).GetAwaiter().GetResult();
        if (!response.IsSuccessStatusCode)
            throw new Exception("Invalid address or API error.");

        var json = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
        using var doc = JsonDocument.Parse(json);

        if (doc.RootElement.ValueKind != JsonValueKind.Array || doc.RootElement.GetArrayLength() == 0)
            throw new Exception("Address not found.");

        var root = doc.RootElement[0];

        if (!root.TryGetProperty("lat", out var latProperty) || !root.TryGetProperty("lon", out var lonProperty))
            throw new Exception("Missing latitude or longitude in API response.");

        if (!double.TryParse(latProperty.GetString(), out double latitude) ||
            !double.TryParse(lonProperty.GetString(), out double longitude))
            throw new Exception("Invalid latitude or longitude format.");

        return (latitude, longitude);
    }

    /// <summary>
    /// Calculates the approximate distance (in km) between two addresses using their coordinates.
    /// </summary>
    /// <param name="address1">First address.</param>
    /// <param name="address2">Second address.</param>
    /// <returns>Estimated distance in kilometers.</returns>
    internal static double DistanceCalculation(string address1, string address2)
    {
        var (latitude1, longitude1) = GetCoordinatesFromAddress(address1);
        var (latitude2, longitude2) = GetCoordinatesFromAddress(address2);

        double dLat = latitude2 - latitude1;
        double dLon = longitude2 - longitude1;
        return Math.Sqrt(dLat * dLat + dLon * dLon) * 111;
    }

    /// <summary>
    /// Sends an email notification to volunteers when a new call is opened.
    /// </summary>
    /// <param name="call">The call details.</param>
    internal static void SendEmailWhenCallOpened(BO.Call call)
    {
        var volunteers = s_dal.Volunteer.ReadAll();
        foreach (var item in volunteers)
        {
            string subject = "Opening call";
            string body = $@"
      Hello {item.Name},

     A new call has been opened in your area.
      Call Details:
      - Call ID: {call.IdCall}
      - Call Type: {call.CallType}
      - Call Address: {call.AddressOfCall}
      - Opening Time: {call.OpeningTime}
      - Description: {call.CallDescription}
      - Entry Time for Treatment: {call.MaxFinishTime}
      - Call Status: {call.StatusCallType}

      If you wish to handle this call, please log into the system.

      Best regards,  
     Call Management System Of TrampIst";

            Tools.SendEmail(item.EmailOfVolunteer, subject, body);
        }
    }

    /// <summary>
    /// Sends an email with the specified subject and body.
    /// </summary>
    /// <param name="toEmail">Recipient email address.</param>
    /// <param name="subject">Email subject.</param>
    /// <param name="body">Email body.</param>
    public static void SendEmail(string toEmail, string subject, string body)
    {
        var fromAddress = new MailAddress("trampist.noreply@gmail.com", "TrampIst");
        var toAddress = new MailAddress(toEmail);

        var smtpClient = new SmtpClient("smtp.gmail.com")
        {
            Port = 587,
            Credentials = new NetworkCredential("trampist.noreply@gmail.com", "jqnd csyy kbty rapl"),
            EnableSsl = true,
        };

        using (var message = new MailMessage(fromAddress, toAddress)
        {
            Subject = subject,
            Body = body,
        })
        {
            smtpClient.Send(message);
        }
    }

    /// <summary>
    /// Determines the status of a call based on assignments and elapsed time.
    /// </summary>
    /// <param name="call">The call to evaluate.</param>
    /// <returns>The status of the call.</returns>
    public static StatusCallType GetCallStatus(this DO.Call call)
    {
        var assignments = s_dal.Assignment
            .ReadAll(a => a?.IdOfRunnerCall == call.IdCall)
            .Where(a => a != null)
            .OrderByDescending(a => a!.EntryTimeForTreatment)
            .ToList();

        var lastAssignment = assignments.FirstOrDefault();

        // 1. אם יש הקצאה שבוטלה ע"י מתנדב או מנהל
        if (lastAssignment != null &&
            lastAssignment.EndTimeForTreatment.HasValue &&
            (lastAssignment.FinishCallType == FinishCallType.CanceledByVolunteer ||
             lastAssignment.FinishCallType == FinishCallType.CanceledByManager))
        {
            return StatusCallType.open; // ← נחשב עדיין כקריאה פתוחה
        }

        // 2. אם הקריאה הושלמה
        if (lastAssignment != null &&
            lastAssignment.EndTimeForTreatment.HasValue &&
            lastAssignment.FinishCallType == FinishCallType.TakenCareof)
        {
            return StatusCallType.closed;
        }

        // 3. פג תוקף
        if (call.MaxFinishTime < AdminManager.Now)
            return StatusCallType.Expired;

        // 4. פתוחה בסיכון
        if ((AdminManager.Now - call.OpeningTime) > s_dal.Config.RiskRange && lastAssignment == null)
            return StatusCallType.openInRisk;

        // 5. בטיפול רגיל או בסיכון
        if (lastAssignment != null)
        {
            if ((AdminManager.Now - lastAssignment.EntryTimeForTreatment) > s_dal.Config.RiskRange)
                return StatusCallType.HandlingInRisk;
            else
                return StatusCallType.inHandling;
        }

        // 6. פתוחה רגילה
        return StatusCallType.open;
    }

}
