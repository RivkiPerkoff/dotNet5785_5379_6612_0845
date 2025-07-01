namespace Dal;
using DalApi;
using DO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Xml.Linq;

internal class VolunteerImplementation : IVolunteer
{
    [MethodImpl(MethodImplOptions.Synchronized)]

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
            new XElement("Role", item.Role.ToString()),
            new XElement("DistanceType", item.DistanceType.ToString())
        );
    }
    [MethodImpl(MethodImplOptions.Synchronized)]

    [MethodImpl(MethodImplOptions.Synchronized)]
    public void Create(Volunteer item)
    {
        List<Volunteer> Volunteers = XMLTools.LoadListFromXMLSerializer<Volunteer>(Config.s_volunteers_xml);
        if (Volunteers.Any(v => v.VolunteerId == item.VolunteerId))
            throw new DalExistException($"Volunteer with ID {item.VolunteerId} already exists.");
        Volunteers.Add(item);
        XMLTools.SaveListToXMLSerializer(Volunteers, Config.s_volunteers_xml);
    }
    [MethodImpl(MethodImplOptions.Synchronized)]

    [MethodImpl(MethodImplOptions.Synchronized)]
    public void Delete(int id)
    {
        XElement volunteersRoot = XMLTools.LoadListFromXMLElement(Config.s_volunteers_xml);
        (volunteersRoot.Elements().FirstOrDefault(v => (int?)v.Element("VolunteerId") == id)
            ?? throw new DalDoesNotExistException($"Volunteer with ID={id} does Not exist")).Remove();
        XMLTools.SaveListToXMLElement(volunteersRoot, Config.s_volunteers_xml);
    }
    [MethodImpl(MethodImplOptions.Synchronized)]

    [MethodImpl(MethodImplOptions.Synchronized)]
    public void DeleteAll()
    {
        XMLTools.SaveListToXMLElement(new XElement("ArrayOfVolunteer", []), Config.s_volunteers_xml);
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    public Volunteer? Read(int id)
    {
        XElement? volunteersElemements = XMLTools.LoadListFromXMLElement(Config.s_volunteers_xml).Elements().FirstOrDefault(volunteer => (int?)volunteer.Element("VolunteerId") == id);
        return volunteersElemements is null ? null : getVolunteer(volunteersElemements);
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    public Volunteer? Read(Func<Volunteer, bool> filter)
    => XMLTools.LoadListFromXMLElement(Config.s_volunteers_xml).Elements().Where(v => v != null).Select(v => getVolunteer(v)).FirstOrDefault(filter);

    [MethodImpl(MethodImplOptions.Synchronized)]
    public IEnumerable<Volunteer> ReadAll(Func<Volunteer, bool>? filter = null)
    => filter == null
    ? XMLTools.LoadListFromXMLElement(Config.s_volunteers_xml).Elements().Where(v => v != null).Select(v => getVolunteer(v))
    : XMLTools.LoadListFromXMLElement(Config.s_volunteers_xml).Elements().Where(v => v != null).Select(v => getVolunteer(v)).Where(filter);

    [MethodImpl(MethodImplOptions.Synchronized)]
    public void Update(Volunteer updatedVolunteer)
    {
        XElement volunteersRootElemement = XMLTools.LoadListFromXMLElement(Config.s_volunteers_xml);
        (volunteersRootElemement.Elements().FirstOrDefault(v => (int?)v.Element("VolunteerId") == updatedVolunteer.VolunteerId)
            ?? throw new DalDoesNotExistException($"Volunteer with ID={updatedVolunteer.VolunteerId} does Not exist")).Remove();
        volunteersRootElemement.Add(createVolunteerElement(updatedVolunteer));
        XMLTools.SaveListToXMLElement(volunteersRootElemement, Config.s_volunteers_xml);
    }
}
