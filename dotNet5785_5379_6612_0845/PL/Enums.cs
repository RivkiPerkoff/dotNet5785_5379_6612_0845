using BL.BO;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PL;
public class VolunteerFieldsCollection : IEnumerable
{
    static readonly IEnumerable<BL.BO.VolunteerFields> s_enums =
    (Enum.GetValues(typeof(BL.BO.VolunteerFields)) as IEnumerable<BL.BO.VolunteerFields>)!;
    public IEnumerator GetEnumerator() => s_enums.GetEnumerator();
}
