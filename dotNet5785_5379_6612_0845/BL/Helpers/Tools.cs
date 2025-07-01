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
    private static IDal s_dal = Factory.Get; 
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
        //var assignments = s_dal.Assignment
        //    .ReadAll(a => a?.IdOfRunnerCall == call.IdCall)
        //    .Where(a => a != null)
        //    .OrderByDescending(a => a!.EntryTimeForTreatment)
        //    .ToList();
        List<DO.Assignment> assignments;
        lock (AdminManager.BlMutex)
            assignments = s_dal.Assignment
                .ReadAll(a => a?.IdOfRunnerCall == call.IdCall)
                .Where(a => a != null)
                .OrderByDescending(a => a!.EntryTimeForTreatment)
                .ToList();

        var activeAssignment = assignments.FirstOrDefault(a => !a.EndTimeForTreatment.HasValue);
        var lastAssignment = assignments.FirstOrDefault();

        //if (activeAssignment != null)
        //{
        //    var duration = AdminManager.Now - activeAssignment.EntryTimeForTreatment;
        //    TimeSpan riskRange;
        //    TimeSpan? riskRangeNullable;
        //    lock (AdminManager.BlMutex)
        //        riskRangeNullable = s_dal.Config.RiskRange;

        //    if (!riskRangeNullable.HasValue)
        //        throw new BL.BO.BlGeneralDatabaseException("RiskRange is not set in the configuration.");

        //    TimeSpan riskRange = riskRangeNullable.Value;

        //    if (duration > riskRange)

        //        return StatusCallType.HandlingInRisk;
        //    else
        //        return StatusCallType.inHandling;
        //}
        if (activeAssignment != null)
        {
            var duration = AdminManager.Now - activeAssignment.EntryTimeForTreatment;

            TimeSpan? riskRangeNullable;
            lock (AdminManager.BlMutex)
                riskRangeNullable = s_dal.Config.RiskRange;

            if (!riskRangeNullable.HasValue)
                throw new BL.BO.BlGeneralDatabaseException("RiskRange is not set in the configuration.");

            TimeSpan riskRange = riskRangeNullable.Value;

            if (duration > riskRange)
                return StatusCallType.HandlingInRisk;
            else
                return StatusCallType.inHandling;
        }

        if (lastAssignment?.EndTimeForTreatment.HasValue == true &&
            lastAssignment.FinishCallType == FinishCallType.TakenCareof)
        {
            return StatusCallType.closed;
        }

        if (call.MaxFinishTime < AdminManager.Now)
            return StatusCallType.Expired;

        if ((AdminManager.Now - call.OpeningTime) > s_dal.Config.RiskRange)
            return StatusCallType.openInRisk;

        return StatusCallType.open;
    }
}
