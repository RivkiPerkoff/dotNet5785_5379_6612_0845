using Microsoft.VisualBasic;

namespace BL.BO;
 /// <summary>
  /// Represents a call in the call list, including details about the call type, timing, and status.
  /// </summary>
    public class CallInList
    {
        /// <summary>
        /// A unique identifier for the call entity.
        /// </summary>
        public int? Id { get; init; }

        /// <summary>
        /// The identifier for the call.
        /// </summary>
        public int CallId { get; set; }

        /// <summary>
        /// The type of the call (e.g., Emergency, Routine).
        /// </summary>
        public CallTypes CallType { get; set; }

        /// <summary>
        /// The opening time of the call.
        /// </summary>
        public DateTime StartTime { get; init; }

        /// <summary>
        /// The remaining time until the call's deadline (if applicable).
        /// </summary>
        public TimeSpan? TimeToEnd { get; set; }

        /// <summary>
        /// The name of the last volunteer who updated the call (if applicable).
        /// </summary>
        public string? LastUpdateBy { get; set; }

        /// <summary>
        /// The remaining time to complete the treatment of the call (if applicable).
        /// </summary>
        public TimeSpan? TimeTocompleteTreatment { get; set; }

        /// <summary>
        /// The current status of the call (e.g., Open, In Progress, Closed).
        /// </summary>
        public StatusCallType Status { get; set; }

        /// <summary>
        /// The total number of assignments for the call.
        /// </summary>
        public int TotalAssignment { get; set; }
    }

