using DalApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Helpers;

static internal class VolunteerManager
{
    private static IDal s_dal = Factory.Get; //stage 4
    static public DateTime PeriodicVolunteersUpdates(DateTime oldClock, DateTime newClock)
    {
        return DateTime.Now;
    }

}
