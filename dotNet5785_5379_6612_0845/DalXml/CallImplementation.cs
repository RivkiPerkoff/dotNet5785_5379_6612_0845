namespace Dal;
using DalApi;
using DO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Runtime.CompilerServices;

internal class CallImplementation : ICall
{
    static Call GetCall(XElement c)
    {
        return new DO.Call(
            IdCall: c.ToIntNullable("IdCall") ?? throw new FormatException("Can't convert IdCall"),
            CallDescription: (string?)c.Element("CallDescription") ?? "",
            CallAddress: (string?)c.Element("CallAddress") ?? "",
            CallLatitude: (double?)c.Element("CallLatitude") ?? 0,
            CallLongitude: (double?)c.Element("CallLongitude") ?? 0,
            OpeningTime: DateTime.Parse(c.Element("OpeningTime")?.Value ?? throw new FormatException("Can't parse OpeningTime")),
            MaxFinishTime: DateTime.Parse(c.Element("MaxFinishTime")?.Value ?? throw new FormatException("Can't parse MaxFinishTime"))
        );
    }

    static XElement CreateCallElement(Call item)
    {
        return new XElement("Call",
            new XElement("IdCall", item.IdCall),
            new XElement("CallDescription", item.CallDescription),
            new XElement("CallAddress", item.CallAddress),
            new XElement("CallLatitude", item.CallLatitude),
            new XElement("CallLongitude", item.CallLongitude),
            new XElement("OpeningTime", item.OpeningTime),
            new XElement("MaxFinishTime", item.MaxFinishTime)
        );
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    public void Create(Call item)
    {
        List<Call> calls = XMLTools.LoadListFromXMLSerializer<Call>(Config.s_calls_xml);
        if (calls.Any(c => c.IdCall == item.IdCall))
            throw new DalDoesNotExistException($"Call with ID={item.IdCall} already exists");
        calls.Add(item);
        XMLTools.SaveListToXMLSerializer(calls, Config.s_calls_xml);
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    public void Delete(int id)
    {
        List<Call> calls = XMLTools.LoadListFromXMLSerializer<Call>(Config.s_calls_xml);
        if (calls.RemoveAll(c => c.IdCall == id) == 0)
            throw new DalDoesNotExistException($"Call with ID={id} does not exist");
        XMLTools.SaveListToXMLSerializer(calls, Config.s_calls_xml);
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    public void DeleteAll()
    {
        XMLTools.SaveListToXMLSerializer(new List<Call>(), Config.s_calls_xml);
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    public Call? Read(int id)
    {
        XElement? callElem = XMLTools.LoadListFromXMLElement(Config.s_calls_xml).Elements().FirstOrDefault(c => (int?)c.Element("IdCall") == id);
        return callElem is null ? throw new DalDoesNotExistException($"Call with ID={id} does not exist") : GetCall(callElem);
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    public Call? Read(Func<Call, bool> filter)
    {
        return XMLTools.LoadListFromXMLElement(Config.s_calls_xml).Elements().Select(c => GetCall(c)).FirstOrDefault(filter);
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    public IEnumerable<Call> ReadAll(Func<Call, bool>? filter = null)
    {
        var calls = XMLTools.LoadListFromXMLElement(Config.s_calls_xml).Elements().Select(c => GetCall(c));
        return filter is null ? calls : calls.Where(filter);
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
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
