using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.BO;
public class Call
{
    public int IdCall { get; init; }
    public CallTypes CallType { get; set; }
    public string? CallDescription { get; set; }
    public string AddressOfCall { get; set; }
    double CallLongitude { get; set; }
    double CallLatitude { get; set; }
    DateTime OpeningTime { get; init; }
    DateTime? MaxFinishTime { get; init; }
    StatusCallType StatusCallType { get; set; }
    List<BO.CallAssignInList>? callAssignInLists { get; set; }
    public override string ToString() => this.ToStringProperty();
}

