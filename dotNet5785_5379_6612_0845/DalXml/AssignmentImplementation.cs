namespace Dal;
using DalApi;
using DO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

internal class AssignmentImplementation : IAssignment
{
    static Assignment GetAssignment(XElement a)
    {
        return new Assignment(
            NextAssignmentId: (int?)a.Element("NextAssignmentId") ?? throw new FormatException("Can't convert NextAssignmentId"),
            IdOfRunnerCall: (int?)a.Element("IdOfRunnerCall") ?? throw new FormatException("Can't convert IdOfRunnerCall"),
            VolunteerId: (int?)a.Element("VolunteerId") ?? throw new FormatException("Can't convert VolunteerId"),
            FinishCallType: (FinishCallType)Enum.Parse(typeof(FinishCallType), a.Element("FinishCallType")?.Value ?? "TakenCareof"),
            EntryTimeForTreatment: DateTime.Parse(a.Element("EntryTimeForTreatment")?.Value ?? throw new FormatException("Can't parse EntryTimeForTreatment")),
            EndTimeForTreatment: (DateTime?)a.Element("EndTimeForTreatment")
        );
    }

    static XElement CreateAssignmentElement(Assignment item)
    {
        return new XElement("Assignment",
            new XElement("NextAssignmentId", item.NextAssignmentId),
            new XElement("IdOfRunnerCall", item.IdOfRunnerCall),
            new XElement("VolunteerId", item.VolunteerId),
            new XElement("FinishCallType", item.FinishCallType.ToString()),
            new XElement("EntryTimeForTreatment", item.EntryTimeForTreatment),
            new XElement("EndTimeForTreatment", item.EndTimeForTreatment)
        );
    }

    public void Create(Assignment item)
    {
        List<Assignment> assignments = XMLTools.LoadListFromXMLSerializer<Assignment>(Config.s_assignments_xml);
        if (assignments.Any(a => a.NextAssignmentId == item.NextAssignmentId))
            throw new DalDoesNotExistException($"Assignment with ID={item.NextAssignmentId} already exists");
        assignments.Add(item);
        XMLTools.SaveListToXMLSerializer(assignments, Config.s_assignments_xml);
    }

    public void Delete(int id)
    {
        List<Assignment> assignments = XMLTools.LoadListFromXMLSerializer<Assignment>(Config.s_assignments_xml);
        if (assignments.RemoveAll(a => a.NextAssignmentId == id) == 0)
            throw new DalDoesNotExistException($"Assignment with ID={id} does not exist");
        XMLTools.SaveListToXMLSerializer(assignments, Config.s_assignments_xml);
    }

    public void DeleteAll()
    {
        XMLTools.SaveListToXMLSerializer(new List<Assignment>(), Config.s_assignments_xml);
    }

    public Assignment? Read(int id)
    {
        XElement? assignmentElem = XMLTools.LoadListFromXMLElement(Config.s_assignments_xml).Elements().FirstOrDefault(a =>
            (int?)a.Element("NextAssignmentId") == id);
        return assignmentElem is null ? throw new DalDoesNotExistException($"Call with ID={id} does not exist") : GetAssignment(assignmentElem);
    }

    public Assignment? Read(Func<Assignment, bool> filter)
    {
        return XMLTools.LoadListFromXMLElement(Config.s_assignments_xml).Elements().Select(a => GetAssignment(a)).FirstOrDefault(filter);
    }

    public IEnumerable<Assignment> ReadAll(Func<Assignment, bool>? filter = null)
    {
        var assignments = XMLTools.LoadListFromXMLElement(Config.s_assignments_xml).Elements().Select(a => GetAssignment(a));
        return filter is null ? assignments : assignments.Where(filter);
    }

    public void Update(Assignment item)
    {
        XElement assignmentRootElem = XMLTools.LoadListFromXMLElement(Config.s_assignments_xml);
        XElement? elemToRemove = assignmentRootElem.Elements().FirstOrDefault(a => (int?)a.Element("NextAssignmentId") == item.NextAssignmentId);
        if (elemToRemove == null)
            throw new DalDoesNotExistException($"Assignment with ID={item.NextAssignmentId} does not exist");

        elemToRemove.Remove();
        assignmentRootElem.Add(CreateAssignmentElement(item));
        XMLTools.SaveListToXMLElement(assignmentRootElem, Config.s_assignments_xml);
    }
}
