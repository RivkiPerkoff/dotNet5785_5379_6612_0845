using System.Text.Json;

namespace BL.Helpers;

static internal class Tools
{
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
}
