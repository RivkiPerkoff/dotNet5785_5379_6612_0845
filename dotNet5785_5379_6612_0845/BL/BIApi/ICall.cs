using BL.BO;
using DO;

namespace BL.BIApi;

public interface ICall
{
    /// <summary>
    /// Retrieves an array of call amounts grouped by their status.
    /// </summary>
    /// <returns>An array where each index represents a call status, and the value is the count of calls with that status.</returns>
    int[] GetCallAmounts();

    BO.Call GetCallDetails(int CallId);

    /// <summary>
    /// Retrieves a filtered and sorted list of calls based on the provided parameters.
    /// </summary>
    /// <param name="filterField">The enum field of the call entity to filter by (nullable).</param>
    /// <param name="filterValue">The value to filter by (nullable).</param>
    /// <param name="sortField">The enum field of the call entity to sort by (nullable).</param>
    /// <returns>A sorted and filtered collection of call entities.</returns>
    IEnumerable<BO.CallInList> GetFilteredAndSortedCallList(BO.CallInListFields? filterBy = null, object? filterValue = null,BO.CallInListFields? sortBy = null);

    /// <summary>
    /// Updates the details of an existing call.
    /// </summary>
    /// <param name="callObject">The call object containing updated details.</param>
    void UpdateCallDetails(BO.Call callObject);

    /// <summary>
    /// Deletes a call based on its identifier.
    /// </summary>
    /// <param name="callId">The unique identifier of the call to delete.</param>
    void DeleteCall(int callId);

    /// <summary>
    /// Adds a new call to the system.
    /// </summary>
    /// <param name="callObject">The call object to add.</param>
    void AddCall(BO.Call callObject);

    /// <summary>
    /// Retrieves a list of closed calls handled by a specific volunteer.
    /// </summary>
    /// <param name="volunteerId">The ID of the volunteer.</param>
    /// <param name="callType">The type of call to filter by (nullable).</param>
    /// <param name="sortField">The field to sort the results by (nullable).</param>
    /// <returns>A list of closed call entities handled by the specified volunteer.</returns>
    IEnumerable<BO.ClosedCallInList> GetClosedCallsForVolunteer(int volunteerId, BL.BO.CallTypes? callType, ClosedCallInListFields? sortField);

    /// <summary>
    /// Retrieves a list of open calls available for selection by a volunteer.
    /// </summary>
    /// <param name="volunteerId">The ID of the volunteer.</param>
    /// <param name="filterField">The field to filter the calls by (nullable).</param>
    /// <param name="sortField">The field to sort the results by (nullable).</param>
    /// <returns>A list of open call entities available for the specified volunteer.</returns>
    public IEnumerable<OpenCallInList> GetOpenCallsForVolunteerSelection(int volunteerId, BL.BO.CallTypes? filterField, OpenCallInListFields? sortField);

    /// <summary>
    /// Marks a call treatment as completed.
    /// </summary>
    /// <param name="volunteerId">The ID of the volunteer completing the treatment.</param>
    /// <param name="assignmentId">The ID of the assignment being completed.</param>
    void CompleteCallTreatment(int volunteerId, int assignmentId);

    /// <summary>
    /// Cancels a call treatment.
    /// </summary>
    /// <param name="requesterId">The ID of the person requesting the cancellation.</param>
    /// <param name="assignmentId">The ID of the assignment being canceled.</param>
    void CancelCallTreatment(int requesterId, int assignmentId);

    /// <summary>
    /// Assigns a volunteer to a call for treatment.
    /// </summary>
    /// <param name="volunteerId">The ID of the volunteer taking the assignment.</param>
    /// <param name="callId">The ID of the call to assign.</param>
    void ChoosingCallForTreatment(int volunteerId, int callId);
}
