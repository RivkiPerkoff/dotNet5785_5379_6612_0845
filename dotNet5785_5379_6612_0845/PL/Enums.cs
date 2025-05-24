using BL.BO;
using System;
using System.Collections;
using System.Collections.Generic;

namespace PL
{
    public class VolunteerFieldsCollection : IEnumerable<VolunteerFields>
    {
        private static readonly VolunteerFields[] s_enums =
            (VolunteerFields[])Enum.GetValues(typeof(VolunteerFields));

        public IEnumerator<VolunteerFields> GetEnumerator() => ((IEnumerable<VolunteerFields>)s_enums).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
