using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BL.BO;
public class Call
{
    public int IdCall { get; init; }
    public CallTypes CallType { get; set; }
    public string? CallDescription { get; set; }
    public string AddressOfCall { get; set; }
    public double CallLongitude { get; set; }
    public double CallLatitude { get; set; }
    public DateTime OpeningTime { get; set; }
    public DateTime? MaxFinishTime { get; set; }
    public StatusCallType StatusCallType { get; set; }
    public List<BO.CallAssignInList>? CallAssignInLists { get; set; }
    //public override string ToString() => this.ToStringProperty();
    public override string ToString()
    {
        var sb = new StringBuilder();
        sb.AppendLine($"ID: {IdCall}");
        sb.AppendLine($"Call Type: {CallType}");
        sb.AppendLine($"Description: {CallDescription ?? "N/A"}");
        sb.AppendLine($"Address: {AddressOfCall}");
        sb.AppendLine($"Location: ({CallLatitude}, {CallLongitude})");
        sb.AppendLine($"Opening Time: {OpeningTime}");
        sb.AppendLine($"Max Finish Time: {MaxFinishTime?.ToString() ?? "N/A"}");
        sb.AppendLine($"Status: {StatusCallType}");

        if (CallAssignInLists != null && CallAssignInLists.Count != 0)
        {
            sb.AppendLine("Assigned Calls:");
            foreach (var assign in CallAssignInLists)
            {
                sb.AppendLine($"  - {assign}");
            }
        }
        else
        {
            sb.AppendLine("No Assigned Calls.");
        }

        return sb.ToString();
    }
}

