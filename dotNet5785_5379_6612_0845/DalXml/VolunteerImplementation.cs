namespace Dal;
using DalApi;
using DO;
using System;
using System.Collections.Generic;
using System.Xml.Linq;

internal class VolunteerImplementation : IVolunteer
{
    static Volunteer getVolunteer(XElement v)
    {
        return new DO.Volunteer(
         VolunteerId: v.ToIntNullable("VolunteerId") ?? throw new FormatException("can't convert id"),
         Name: (string?)v.Element("Name") ?? "",
         PhoneNumber: (string?)v.Element("PhoneNumber") ?? "",
         EmailOfVolunteer: (string?)v.Element("EmailOfVolunteer") ?? "",
         PasswordVolunteer: (string?)v.Element("PasswordVolunteer") ?? "",
         AddressVolunteer: (string?)v.Element("AddressVolunteer") ?? "",
         VolunteerLatitude: (double?)v.Element("VolunteerLatitude") ?? 0,
         VolunteerLongitude: (double?)v.Element("VolunteerLongitude") ?? 0,
         IsAvailable: (bool?)v.Element("IsAvailable") ?? false,
         MaximumDistanceForReceivingCall: (double?)v.Element("MaximumDistanceForReceivingCall") ?? 0,
         Role: (Role)Enum.Parse(typeof(Role), (string?)v.Element("Role") ?? "Volunteer"),
         DistanceType: (DistanceType)Enum.Parse(typeof(DistanceType), (string?)v.Element("DistanceType") ?? "AirDistance")
     );
    }
    static XElement createVolunteerElement(Volunteer item)
    {
        return new XElement("Volunteer",
            new XElement("VolunteerId", item.VolunteerId),
            new XElement("Name", item.Name),
            new XElement("PhoneNumber", item.PhoneNumber),
            new XElement("EmailOfVolunteer", item.EmailOfVolunteer),
            new XElement("PasswordVolunteer", item.PasswordVolunteer),
            new XElement("AddressVolunteer", item.AddressVolunteer),
            new XElement("VolunteerLatitude", item.VolunteerLatitude),
            new XElement("VolunteerLongitude", item.VolunteerLongitude),
            new XElement("IsAvailable", item.IsAvailable),
            new XElement("MaximumDistanceForReceivingCall", item.MaximumDistanceForReceivingCall),
            new XElement("Role", item.Role.ToString()), // Enum to string
            new XElement("DistanceType", item.DistanceType.ToString()) // Enum to string
        );
    }
    public void Create(Volunteer item)
    {
        List<Volunteer> Volunteers = XMLTools.LoadListFromXMLSerializer<Volunteer>(Config.s_volunteers_xml);
        if (Volunteers.Any(v => v.VolunteerId == item.VolunteerId))
            throw new DalExistException($"Volunteer with ID {item.VolunteerId} already exists.");
        Volunteers.Add(item);
        XMLTools.SaveListToXMLSerializer(Volunteers, Config.s_volunteers_xml);
    }
    public void Delete(int id)
    {
        // טען את הרשימה
        List<Volunteer> Volunteers = XMLTools.LoadListFromXMLSerializer<Volunteer>(Config.s_volunteers_xml);

        // בדוק אם ה-ID קיים
        if (!Volunteers.Any(v => v.VolunteerId == id))
            throw new DalDoesNotExistException($"Volunteer with ID={id} does not exist");

        // מחק את המתנדב
        Volunteers.RemoveAll(it => it.VolunteerId == id);

        // שמור את הרשימה המעודכנת
        XMLTools.SaveListToXMLSerializer(Volunteers, Config.s_volunteers_xml);
    }

    //public void Delete(int id)
    //{
    //    List<Volunteer> Volunteers = XMLTools.LoadListFromXMLSerializer<Volunteer>(Config.s_volunteers_xml);
    //    if (Volunteers.RemoveAll(it => it.VolunteerId == id) == 0)
    //        throw new DalDoesNotExistException($"Course with ID={id} does Not exist");
    //    XMLTools.SaveListToXMLSerializer(Volunteers, Config.s_volunteers_xml);
    //}
    public void DeleteAll()
    {
        XMLTools.SaveListToXMLSerializer(new List<Volunteer>(), Config.s_volunteers_xml);
    }
    //public Volunteer? Read(int id)
    //{
    //    XElement? volunteerElem =
    //    XMLTools.LoadListFromXMLElement(Config.s_volunteers_xml).Elements().FirstOrDefault(v =>
    //    (int?)v.Element("Id") == id);
    //    return volunteerElem is null ? null : getVolunteer(volunteerElem);
    //}
    //public Volunteer? Read(int id)
    //{
    //    XElement? volunteerElem =
    //    XMLTools.LoadListFromXMLElement(Config.s_volunteers_xml).Elements().FirstOrDefault(v =>
    //    (int?)v.Element("VolunteerId") == id); // תיקון מ-"Id" ל-"VolunteerId"
    //    return volunteerElem is null ? null : getVolunteer(volunteerElem);
    //}
    public Volunteer Read(int id)
    {
        // טען את הרשימה ונסה למצוא את האלמנט המתאים
        XElement? volunteerElem = XMLTools.LoadListFromXMLElement(Config.s_volunteers_xml)
            .Elements()
            .FirstOrDefault(v => (int?)v.Element("VolunteerId") == id); // וודא שמחפש "VolunteerId"

        // אם האלמנט לא נמצא, זרוק שגיאה
        if (volunteerElem == null)
            throw new DO.DalDoesNotExistException($"Volunteer with ID={id} does not exist");

        // החזר את המתנדב שנמצא
        return getVolunteer(volunteerElem);
    }

    //public Volunteer? Read(Func<Volunteer, bool> filter)
    //{
    //    return XMLTools.LoadListFromXMLElement(Config.s_volunteers_xml).Elements().Select(v =>
    //    getVolunteer(v)).FirstOrDefault(filter);
    //}
    public Volunteer Read(Func<Volunteer, bool> filter)
    {
        // טען את כל האלמנטים כמתנדבים
        var volunteers = XMLTools.LoadListFromXMLElement(Config.s_volunteers_xml)
            .Elements()
            .Select(v => getVolunteer(v));

        // נסה למצוא מתנדב שעונה לתנאי
        Volunteer? volunteer = volunteers.FirstOrDefault(filter);

        // אם לא נמצא, זרוק שגיאה
        if (volunteer == null)
            throw new DO.DalDoesNotExistException("No volunteer matching the specified condition exists");

        // החזר את המתנדב שנמצא
        return volunteer;
    }

    public IEnumerable<Volunteer> ReadAll(Func<Volunteer, bool>? filter = null)
    {
        var volunteers = XMLTools.LoadListFromXMLElement(Config.s_volunteers_xml).Elements().Select(c => getVolunteer(c));
        return filter is null ? volunteers : volunteers.Where(filter);
    }
    public void Update(Volunteer item)
    {
        XElement VolunteerRootElem = XMLTools.LoadListFromXMLElement(Config.s_volunteers_xml);
        (VolunteerRootElem.Elements().FirstOrDefault(v => (int?)v.Element("VolunteerId") == item.VolunteerId)
        ?? throw new DO.DalDoesNotExistException($"volunteer with ID={item.VolunteerId} does Not exist"))
        .Remove();
        VolunteerRootElem.Add(createVolunteerElement(item));
        XMLTools.SaveListToXMLElement(VolunteerRootElem, Config.s_volunteers_xml);
    }
}
