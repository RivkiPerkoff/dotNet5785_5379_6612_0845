using BL.BO;
using DalApi;
using DO;
using System.Text.Json;

namespace BL.Helpers;

static internal class Tools
{
    private static IDal s_dal = Factory.Get; //stage 4

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

        return (root.GetProperty("lat").GetDouble(), root.GetProperty("lon").GetDouble());
    }

    internal static double DistanceCalculation(string address1, string address2)
    {
        var (latitude1, longitude1) = GetCoordinatesFromAddress(address1);
        var (latitude2, longitude2) = GetCoordinatesFromAddress(address2);

        double dLat = latitude2 - latitude1;
        double dLon = longitude2 - longitude1;
        return Math.Sqrt(dLat * dLat + dLon * dLon) * 111;
    }

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