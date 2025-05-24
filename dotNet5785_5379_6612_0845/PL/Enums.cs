using BL.BO;
using System;
using System.Collections;
using System.Collections.Generic;

namespace PL
{
    public class VolunteerFieldsCollection : IEnumerable<TypeSortingVolunteers>
    {
        private static readonly TypeSortingVolunteers[] s_enums =
            (TypeSortingVolunteers[])Enum.GetValues(typeof(TypeSortingVolunteers));

        public IEnumerator<TypeSortingVolunteers> GetEnumerator() => ((IEnumerable<TypeSortingVolunteers>)s_enums).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
