namespace Dal;
using DalApi;
using DO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

internal class AssignmentImplementation : IAssignment
{
    /// <summary>
    /// Converts an XML element to an Assignment object.
    /// </summary>
    /// <param name="a">The XML element representing an Assignment.</param>
    /// <returns>An Assignment object with properties populated from the XML element.</returns>
    static Assignment GetAssignment(XElement a)
    {
        return new Assignment(
            NextAssignmentId: (int?)a.Element("NextAssignmentId") ?? throw new FormatException("Can't convert NextAssignmentId"),
            IdOfRunnerCall: (int?)a.Element("IdOfRunnerCall") ?? throw new FormatException("Can't convert IdOfRunnerCall"),
            VolunteerId: (int?)a.Element("VolunteerId") ?? throw new FormatException("Can't convert VolunteerId"),
            TypeOfEndTime: (TypeOfEndTime)Enum.Parse(typeof(TypeOfEndTime), a.Element("TypeOfEndTime")?.Value ?? "treated"),
            EntryTimeForTreatment: DateTime.Parse(a.Element("EntryTimeForTreatment")?.Value ?? throw new FormatException("Can't parse EntryTimeForTreatment")),
            EndTimeForTreatment: (DateTime?)a.Element("EndTimeForTreatment")
        );
    }

    /// <summary>
    /// Converts an Assignment object to an XML element.
    /// </summary>
    /// <param name="item">The Assignment object to convert.</param>
    /// <returns>An XML element representing the Assignment object.</returns>
    static XElement CreateAssignmentElement(Assignment item)
    {
        return new XElement("Assignment",
            new XElement("NextAssignmentId", item.NextAssignmentId),
            new XElement("IdOfRunnerCall", item.IdOfRunnerCall),
            new XElement("VolunteerId", item.VolunteerId),
            new XElement("TypeOfEndTime", item.TypeOfEndTime.ToString()),
            new XElement("EntryTimeForTreatment", item.EntryTimeForTreatment),
            new XElement("EndTimeForTreatment", item.EndTimeForTreatment)
        );
    }

    /// <summary>
    /// Adds a new Assignment to the data source.
    /// </summary>
    /// <param name="item">The Assignment object to add.</param>
    /// <exception cref="DalDoesNotExistException">Thrown if an Assignment with the same ID already exists.</exception>
    public void Create(Assignment item)
    {
        List<Assignment> assignments = XMLTools.LoadListFromXMLSerializer<Assignment>(Config.s_assignments_xml);
        if (assignments.Any(a => a.NextAssignmentId == item.NextAssignmentId))
            throw new DalDoesNotExistException($"Assignment with ID={item.NextAssignmentId} already exists");
        assignments.Add(item);
        XMLTools.SaveListToXMLSerializer(assignments, Config.s_assignments_xml);
    }

    /// <summary>
    /// Deletes an Assignment by its ID.
    /// </summary>
    /// <param name="id">The ID of the Assignment to delete.</param>
    /// <exception cref="DalDoesNotExistException">Thrown if the Assignment does not exist.</exception>
    public void Delete(int id)
    {
        List<Assignment> assignments = XMLTools.LoadListFromXMLSerializer<Assignment>(Config.s_assignments_xml);
        if (assignments.RemoveAll(a => a.NextAssignmentId == id) == 0)
            throw new DalDoesNotExistException($"Assignment with ID={id} does not exist");
        XMLTools.SaveListToXMLSerializer(assignments, Config.s_assignments_xml);
    }

    /// <summary>
    /// Deletes all Assignments from the data source.
    /// </summary>
    public void DeleteAll()
    {
        XMLTools.SaveListToXMLSerializer(new List<Assignment>(), Config.s_assignments_xml);
    }

    /// <summary>
    /// Retrieves an Assignment by its ID.
    /// </summary>
    /// <param name="id">The ID of the Assignment to retrieve.</param>
    /// <returns>The Assignment object if found.</returns>
    /// <exception cref="DalDoesNotExistException">Thrown if the Assignment does not exist.</exception>
    public Assignment? Read(int id)
    {
        XElement? assignmentElem = XMLTools.LoadListFromXMLElement(Config.s_assignments_xml).Elements().FirstOrDefault(a =>
            (int?)a.Element("NextAssignmentId") == id);
        return assignmentElem is null ? throw new DalDoesNotExistException($"Assignment with ID={id} does not exist") : GetAssignment(assignmentElem);
    }

    /// <summary>
    /// Retrieves an Assignment matching the given filter.
    /// </summary>
    /// <param name="filter">The condition to match the Assignment.</param>
    /// <returns>The Assignment object if found, or null if not found.</returns>
    public Assignment? Read(Func<Assignment, bool> filter)
    {
        return XMLTools.LoadListFromXMLElement(Config.s_assignments_xml).Elements().Select(a => GetAssignment(a)).FirstOrDefault(filter);
    }

    /// <summary>
    /// Retrieves all Assignments, optionally filtered by a condition.
    /// </summary>
    /// <param name="filter">The condition to filter the Assignments. If null, all Assignments are returned.</param>
    /// <returns>An IEnumerable of Assignment objects.</returns>
    public IEnumerable<Assignment> ReadAll(Func<Assignment, bool>? filter = null)
    {
        var assignments = XMLTools.LoadListFromXMLElement(Config.s_assignments_xml).Elements().Select(a => GetAssignment(a));
        return filter is null ? assignments : assignments.Where(filter);
    }

    /// <summary>
    /// Updates an existing Assignment in the data source.
    /// </summary>
    /// <param name="item">The updated Assignment object.</param>
    /// <exception cref="DalDoesNotExistException">Thrown if the Assignment does not exist.</exception>
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
