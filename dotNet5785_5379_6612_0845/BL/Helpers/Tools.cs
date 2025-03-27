using BL.BO;
using DalApi;
using DO;
using System.Net.Mail;
using System.Net;
using System.Text.Json;

namespace BL.Helpers;

static internal class Tools
{
    private static IDal s_dal = Factory.Get; //stage 4

    //public static (double, double) GetCoordinatesFromAddress(string address)
    //{
    //    string apiKey = "PK.83B935C225DF7E2F9B1ee90A6B46AD86";
    //    using var client = new HttpClient();
    //    string url = $"https://us1.locationiq.com/v1/search.php?key={apiKey}&q={Uri.EscapeDataString(address)}&format=json";

    //    var response = client.GetAsync(url).GetAwaiter().GetResult();
    //    if (!response.IsSuccessStatusCode)
    //        throw new Exception("Invalid address or API error.");

    //    var json = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
    //    using var doc = JsonDocument.Parse(json);

    //    if (doc.RootElement.ValueKind != JsonValueKind.Array || doc.RootElement.GetArrayLength() == 0)
    //        throw new Exception("Address not found.");

    //    var root = doc.RootElement[0];

    //    return (root.GetProperty("lat").GetDouble(), root.GetProperty("lon").GetDouble());
    //}
    public static (double, double) GetCoordinatesFromAddress(string address)
    {
        string apiKey = "PK.83B935C225DF7E2F9B1ee90A6B46AD86";
        using var client = new HttpClient();
        string url = $"https://us1.locationiq.com/v1/search.php?key={apiKey}&q={Uri.EscapeDataString(address)}&format=json";

        var response = client.GetAsync(url).GetAwaiter().GetResult();
        if (!response.IsSuccessStatusCode)
            throw new Exception("Invalid address or API error.");

        var json = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
        //Console.WriteLine("Response JSON: " + json); // ✅ הדפסת התגובה לבדיקה

        using var doc = JsonDocument.Parse(json);

        if (doc.RootElement.ValueKind != JsonValueKind.Array || doc.RootElement.GetArrayLength() == 0)
            throw new Exception("Address not found.");

        var root = doc.RootElement[0]; // לוקח את התוצאה הראשונה (הכי רלוונטית)

        // ✅ בדיקה שהשדות lat ו- lon קיימים ושהם מחרוזות
        if (!root.TryGetProperty("lat", out var latProperty) || !root.TryGetProperty("lon", out var lonProperty))
            throw new Exception("Missing latitude or longitude in API response.");

        // ✅ המרה ממחרוזת למספר
        if (!double.TryParse(latProperty.GetString(), out double latitude) ||
            !double.TryParse(lonProperty.GetString(), out double longitude))
            throw new Exception("Invalid latitude or longitude format.");

        return (latitude, longitude);
    }


    internal static double DistanceCalculation(string address1, string address2)
    {
        var (latitude1, longitude1) = GetCoordinatesFromAddress(address1);
        var (latitude2, longitude2) = GetCoordinatesFromAddress(address2);

        double dLat = latitude2 - latitude1;
        double dLon = longitude2 - longitude1;
        return Math.Sqrt(dLat * dLat + dLon * dLon) * 111;
    }

    //internal static void SendEmailToVolunteer(DO.Volunteer volunteer, DO.Assignment assignment)
    //{
    //    var call = s_dal.Call.Read(assignment.IdOfRunnerCall)!;
    //    string subject = "הקצאה בוטלה";
    //    string body = $"שלום {volunteer.Name},\n\n" +
    //                  $"ההקצאה שלך לטיפול בקריאה {assignment.NextAssignmentId} בוטלה על ידי המנהל.\n" +
    //                  $"פרטי הקריאה:\n" +
    //                  $"קריאה: {assignment.IdOfRunnerCall}\n" +
    //                  $"סוג הקריאה: {call.CallTypes}\n" +
    //                  $"כתובת הקריאה: {call.CallAddress}\n" +
    //                  $"זמן פתיחה: {call.OpeningTime}\n" +
    //                  $"תאור מילולי: {call.CallDescription}\n" +
    //                  $"זמן כניסה טיפול : {assignment.EntryTimeForTreatment}\n\n" +
    //                  $"בברכה,\nמערכת ניהול קריאות";

    //    Tools.SendEmail(volunteer.EmailOfVolunteer, subject, body);
    //}
    internal static void SendEmailWhenCallOpened(BO.Call call)
    {
        var volunteer = s_dal.Volunteer.ReadAll();
        foreach (var item in volunteer)
        {
            //if (item.MaximumDistanceForReceivingCall == null)
            //{ break; }
            //else if (item.MaximumDistanceForReceivingCall >= Tools.DistanceCalculation(item.AddressVolunteer!, call.AddressOfCall))
            //{
                string subject = "Openning call";
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
      -call Status:{call.StatusCallType}

      If you wish to handle this call, please log into the system.

      Best regards,  
     Call Management System Of TrampIst";

                Tools.SendEmail(item.EmailOfVolunteer, subject, body);
            
        }
    }
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

    //internal static int[] GetCallStatus(int callId)
    //{
    //    try
    //    {
    //        var calls = s_dal.Call.ReadAll();
    //        var assignments = s_dal.Assignment.ReadAll();

    //        var statusGroups =
    //            from call in calls
    //            let currentStatus = CallManager.CalculateCallStatus(call.IdCall)
    //            group call by currentStatus into statusGroup
    //            orderby statusGroup.Key
    //            select new
    //            {
    //                Status = statusGroup.Key,
    //                Count = statusGroup.Count()
    //            };

    //        // יצירת מערך בגודל מספר הערכים באינום CallStatus
    //        int statusCount = Enum.GetValues<BO.StatusCallType>().Length;
    //        int[] quantities = new int[statusCount];

    //        statusGroups.ToList()
    //                   .ForEach(group => quantities[(int)group.Status] = group.Count);

    //        return quantities;
    //    }

    //    catch (DO.DalDoesNotExistException ex)
    //    {
    //        throw new BO.BlGeolocationNotFoundException("Required data was not found in the database", ex);
    //    }
    //    catch (Exception ex)
    //    {
    //        throw new BO.BlGeneralDatabaseException("An unexpected error occurred while calculating call quantities", ex);
    //    }
    //}

    internal static StatusCallType GetCallStatus(int callId)
    {

        var call = s_dal.Call.Read(callId) ?? throw new ArgumentException($"Call with ID {callId} not found.");
        DateTime currentTime = ClockManager.Now;
        var assignments = s_dal.Assignment.ReadAll(a => a.IdOfRunnerCall == callId).ToList();
        Assignment? activeAssignment = assignments.Find(a => a.EndTimeForTreatment == null);
        Assignment? handledAssignments = assignments.Find(a => a.EndTimeForTreatment != null && a.FinishCallType == DO.FinishCallType.TakenCareof);

        if (activeAssignment != null)
        {
            if (call.MaxFinishTime.HasValue && currentTime > call.MaxFinishTime.Value - s_dal.Config.RiskRange)
                return StatusCallType.HandlingInRisk;

            return StatusCallType.inHandling;
        }

        if (handledAssignments != null)
            return StatusCallType.closed;

        if (call.MaxFinishTime.HasValue && currentTime > call.MaxFinishTime.Value)
            return StatusCallType.Expired;

        if (call.MaxFinishTime.HasValue && currentTime > call.MaxFinishTime.Value - s_dal.Config.RiskRange)
            return StatusCallType.openInRisk;

        return StatusCallType.open;
    }
}