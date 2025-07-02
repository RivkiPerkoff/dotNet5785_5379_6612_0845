using System;
using System.ComponentModel;

namespace PL.Volunteer
{
    public class CallInProgressDisplay : INotifyPropertyChanged
    {
        private double _callingDistanceFromVolunteer;
        private string? _callingAddress;
        private DateTime _entryTimeForTreatment;
        private DateTime? _maxFinishTime;

        public double CallingDistanceFromVolunteer
        {
            get => _callingDistanceFromVolunteer;
            set
            {
                if (_callingDistanceFromVolunteer != value)
                {
                    _callingDistanceFromVolunteer = value;
                    OnPropertyChanged(nameof(CallingDistanceFromVolunteer));
                }
            }
        }

        public string? CallingAddress
        {
            get => _callingAddress;
            set
            {
                if (_callingAddress != value)
                {
                    _callingAddress = value;
                    OnPropertyChanged(nameof(CallingAddress));
                }
            }
        }

        public DateTime EntryTimeForTreatment
        {
            get => _entryTimeForTreatment;
            set
            {
                if (_entryTimeForTreatment != value)
                {
                    _entryTimeForTreatment = value;
                    OnPropertyChanged(nameof(EntryTimeForTreatment));
                }
            }
        }

        public DateTime? MaxFinishTime
        {
            get => _maxFinishTime;
            set
            {
                if (_maxFinishTime != value)
                {
                    _maxFinishTime = value;
                    OnPropertyChanged(nameof(MaxFinishTime));
                    OnPropertyChanged(nameof(TimeLeft));
                }
            }
        }

        public string TimeLeft
        {
            get
            {
                if (MaxFinishTime == null)
                    return "N/A";

                TimeSpan diff = MaxFinishTime.Value - DateTime.Now;
                return diff.TotalSeconds > 0
                    ? $"{diff.Minutes}m {diff.Seconds}s"
                    : "Expired";
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        public void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
