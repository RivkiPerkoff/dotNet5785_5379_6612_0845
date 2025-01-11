namespace BL.BO
{
    internal class VolunteerInList
    {
        public int VolunteerId { get; set; }
        public string FullName { get; set; }
        public bool IsActive { get; set; }
        public int HandledCalls { get; set; }
        public int CanceledCalls { get; set; }
        public int ExpiredCalls { get; set; }
        public int? CurrentCallId { get; set; }
        public CallTypes CallType { get; set; }
    }
}
