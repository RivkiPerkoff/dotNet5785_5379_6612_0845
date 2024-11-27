
namespace DO;
public record Call
(
    int IdCall,
    string? CallDescription,
    string? CallAddress,
    double CallLatitude,
    double CallLongitude,
    DateTime OpeningTime,
    DateTime MaxFinishTime,
    TypeOfReading TypeOfReading
)
{
    public Call() : this(0, "", "", 0, 0, DateTime.Now, DateTime.Now, TypeOfReading.Type1) { }  // ערך ברירת מחדל עבור TypeOfReading
}

//{
//    public Call(int idCall, string? callDescription, string? callAddress, double? callLatitude, double? callLongitude, DateTime openingTime, DateTime maxFinishTime, TypeOfReading TypeOfReading)
//    {
//        IdCall = idCall;
//        CallDescription = callDescription;
//        CallAddress = callAddress;
//        CallLatitude = callLatitude;
//        CallLongitude = callLongitude;
//        OpeningTime = openingTime;
//        MaxFinishTime = maxFinishTime;
//        TypeOfReading TypeOfReading;
//    }
//}
