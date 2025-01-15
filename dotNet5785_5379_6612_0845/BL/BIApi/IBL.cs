using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.BIApi;

public interface IBL
{
    ICall Call { get; }
    IVolunteer Volunteer { get; }  
    IAdmin Admin { get; }
}
