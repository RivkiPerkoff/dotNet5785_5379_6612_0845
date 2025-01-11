using System;

namespace BL.BO;

/// <summary>
/// Represents a call assignment in a list, including details about the volunteer and treatment times.
/// </summary>
public class CallAssignInList
{
    /// <summary>
    /// Gets or sets the volunteer's ID. This value can be null if the volunteer is not assigned.
    /// </summary>
    public int? VolunteerId { get; set; }

    /// <summary>
    /// Gets or sets the volunteer's name. This value can be null if the name is not provided.
    /// </summary>
    public string? VolunteerName { get; set; }

    /// <summary>
    /// Gets or sets the entry time for the treatment. This value is required and cannot be null.
    /// </summary>
    public DateTime EntryTimeForTreatment { get; set; }

    /// <summary>
    /// Gets or sets the end time for the treatment. This value can be null if the treatment is ongoing or incomplete.
    /// </summary>
    public DateTime? EndTimeForTreatment { get; set; }

    /// <summary>
    /// Gets or sets the type of treatment completion. This value can be null if the type is not determined.
    /// </summary>
    public TreatmentEndType? TreatmentEndType { get; set; }
}
