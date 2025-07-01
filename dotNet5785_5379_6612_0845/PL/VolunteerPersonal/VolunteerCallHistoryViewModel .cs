using BL.BO;
using System.ComponentModel;
using System.Windows;

public class VolunteerCallHistoryViewModel : INotifyPropertyChanged
{
    private readonly BL.BIApi.IBL _bl;
    private readonly int _volunteerId;

    private IEnumerable<ClosedCallInList> _callList = new List<ClosedCallInList>();
    public IEnumerable<ClosedCallInList> CallList
    {
        get => _callList;
        set
        {
            _callList = value;
            OnPropertyChanged(nameof(CallList));
        }
    }

    public IEnumerable<CallTypes> CallTypeOptions { get; set; }
    public IEnumerable<ClosedCallInListFields> SortFieldOptions { get; set; }

    private CallTypes? _selectedFilterType;
    public CallTypes? SelectedFilterType
    {
        get => _selectedFilterType;
        set
        {
            _selectedFilterType = value;
            LoadCalls();
        }
    }

    private ClosedCallInListFields? _selectedSortField;
    public ClosedCallInListFields? SelectedSortField
    {
        get => _selectedSortField;
        set
        {
            _selectedSortField = value;
            LoadCalls();
        }
    }

    public VolunteerCallHistoryViewModel(int volunteerId)
    {
        _bl = BlApi.Factory.Get();
        _volunteerId = volunteerId;

        CallTypeOptions = Enum.GetValues(typeof(CallTypes)) as CallTypes[];
        SortFieldOptions = Enum.GetValues(typeof(ClosedCallInListFields)) as ClosedCallInListFields[];

        LoadCalls();
    }

    private void LoadCalls()
    {
        try
        {
            CallList = _bl.Call.GetClosedCallsForVolunteer(
                _volunteerId,
                SelectedFilterType,
                SelectedSortField
            );
        }
        catch (Exception ex)
        {
            MessageBox.Show("Error loading call history: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected virtual void OnPropertyChanged(string propertyName) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}
