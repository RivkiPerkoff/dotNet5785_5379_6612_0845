namespace Dal;
using DalApi;
using DO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

internal class CallImplementation : ICall
{
    /// <summary>
    /// Converts an XML element to a Call object.
    /// </summary>
    /// <param name="c">The XML element representing a Call.</param>
    /// <returns>A Call object with properties populated from the XML element.</returns>
    static Call GetCall(XElement c)
    {
        return new DO.Call(
            IdCall: c.ToIntNullable("IdCall") ?? throw new FormatException("Can't convert IdCall"),
            CallDescription: (string?)c.Element("CallDescription") ?? "",
            CallAddress: (string?)c.Element("CallAddress") ?? "",
            CallLatitude: (double?)c.Element("CallLatitude") ?? 0,
            CallLongitude: (double?)c.Element("CallLongitude") ?? 0,
            OpeningTime: DateTime.Parse(c.Element("OpeningTime")?.Value ?? throw new FormatException("Can't parse OpeningTime")),
            MaxFinishTime: DateTime.Parse(c.Element("MaxFinishTime")?.Value ?? throw new FormatException("Can't parse MaxFinishTime")),
            TypeOfReading: (TypeOfReading)Enum.Parse(typeof(TypeOfReading), c.Element("TypeOfReading")?.Value ?? "Type1")
        );
    }

    /// <summary>
    /// Converts a Call object to an XML element.
    /// </summary>
    /// <param name="item">The Call object to convert.</param>
    /// <returns>An XML element representing the Call object.</returns>
    static XElement CreateCallElement(Call item)
    {
        return new XElement("Call",
            new XElement("IdCall", item.IdCall),
            new XElement("CallDescription", item.CallDescription),
            new XElement("CallAddress", item.CallAddress),
            new XElement("CallLatitude", item.CallLatitude),
            new XElement("CallLongitude", item.CallLongitude),
            new XElement("OpeningTime", item.OpeningTime),
            new XElement("MaxFinishTime", item.MaxFinishTime),
            new XElement("TypeOfReading", item.TypeOfReading.ToString())
        );
    }

    /// <summary>
    /// Adds a new Call to the data source.
    /// </summary>
    /// <param name="item">The Call object to add.</param>
    /// <exception cref="DalDoesNotExistException">Thrown if a Call with the same ID already exists.</exception>
    public void Create(Call item)
    {
        List<Call> calls = XMLTools.LoadListFromXMLSerializer<Call>(Config.s_calls_xml);
        if (calls.Any(c => c.IdCall == item.IdCall))
            throw new DalDoesNotExistException($"Call with ID={item.IdCall} already exists");
        calls.Add(item);
        XMLTools.SaveListToXMLSerializer(calls, Config.s_calls_xml);
    }

    /// <summary>
    /// Deletes a Call by its ID.
    /// </summary>
    /// <param name="id">The ID of the Call to delete.</param>
    /// <exception cref="DalDoesNotExistException">Thrown if the Call does not exist.</exception>
    public void Delete(int id)
    {
        List<Call> calls = XMLTools.LoadListFromXMLSerializer<Call>(Config.s_calls_xml);
        if (calls.RemoveAll(c => c.IdCall == id) == 0)
            throw new DalDoesNotExistException($"Call with ID={id} does not exist");
        XMLTools.SaveListToXMLSerializer(calls, Config.s_calls_xml);
    }

    /// <summary>
    /// Deletes all Calls from the data source.
    /// </summary>
    public void DeleteAll()
    {
        XMLTools.SaveListToXMLSerializer(new List<Call>(), Config.s_calls_xml);
    }

    /// <summary>
    /// Retrieves a Call by its ID.
    /// </summary>
    /// <param name="id">The ID of the Call to retrieve.</param>
    /// <returns>The Call object if found.</returns>
    /// <exception cref="DalDoesNotExistException">Thrown if the Call does not exist.</exception>
    public Call Read(int id)
    {
        XElement? callElem = XMLTools.LoadListFromXMLElement(Config.s_calls_xml).Elements().FirstOrDefault(c => (int?)c.Element("IdCall") == id);
        return callElem is null ? throw new DalDoesNotExistException($"Call with ID={id} does not exist") : GetCall(callElem);
    }

    /// <summary>
    /// Retrieves a Call matching the given filter.
    /// </summary>
    /// <param name="filter">The condition to match the Call.</param>
    /// <returns>The Call object if found, or null if not found.</returns>
    public Call? Read(Func<Call, bool> filter)
    {
        return XMLTools.LoadListFromXMLElement(Config.s_calls_xml).Elements().Select(c => GetCall(c)).FirstOrDefault(filter);
    }

    /// <summary>
    /// Retrieves all Calls, optionally filtered by a condition.
    /// </summary>
    /// <param name="filter">The condition to filter the Calls. If null, all Calls are returned.</param>
    /// <returns>An IEnumerable of Call objects.</returns>
    public IEnumerable<Call> ReadAll(Func<Call, bool>? filter = null)
    {
        var calls = XMLTools.LoadListFromXMLElement(Config.s_calls_xml).Elements().Select(c => GetCall(c));
        return filter is null ? calls : calls.Where(filter);
    }

    /// <summary>
    /// Updates an existing Call in the data source.
    /// </summary>
    /// <param name="item">The updated Call object.</param>
    /// <exception cref="DalDoesNotExistException">Thrown if the Call does not exist.</exception>
    public void Update(Call item)
    {
        XElement callRootElem = XMLTools.LoadListFromXMLElement(Config.s_calls_xml);
        XElement? elemToRemove = callRootElem.Elements().FirstOrDefault(c => (int?)c.Element("IdCall") == item.IdCall);
        if (elemToRemove == null)
            throw new DalDoesNotExistException($"Call with ID={item.IdCall} does not exist");

        elemToRemove.Remove();
        callRootElem.Add(CreateCallElement(item));
        XMLTools.SaveListToXMLElement(callRootElem, Config.s_calls_xml);
    }
}
