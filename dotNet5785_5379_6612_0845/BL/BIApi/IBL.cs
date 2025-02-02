
using BL.BlImplementation;

namespace BL.BIApi;

public interface IBL
{
    ICall Call { get; } 
    IVolunteer Volunteer { get; }  
    IAdmin Admin { get; }
}
