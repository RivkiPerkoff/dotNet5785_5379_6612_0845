using System.Windows;

namespace PL.VolunteerPersonal;

public partial class VolunteerCallHistoryWindow : Window
{
    public VolunteerCallHistoryWindow(int volunteerId)
    {
        InitializeComponent();
        DataContext = new VolunteerCallHistoryViewModel(volunteerId);
    }
}

//using System;
//using System.Collections.Generic;
//using System.ComponentModel;
//using System.Windows;
//using System.Windows.Controls;
//using BL.BO;

//namespace PL.VolunteerPersonal;

//public partial class VolunteerCallHistoryWindow : Window, INotifyPropertyChanged
//{
//    private readonly BL.BIApi.IBL _bl = BlApi.Factory.Get();
//    private readonly int _volunteerId;

//    private IEnumerable<ClosedCallInList> _callList = new List<ClosedCallInList>();
//    public IEnumerable<ClosedCallInList> CallList
//    {
//        get => _callList;
//        set
//        {
//            _callList = value;
//            OnPropertyChanged(nameof(CallList));
//        }
//    }
//    public IEnumerable<CallTypes> CallTypeOptions { get; set; }
//    public IEnumerable<ClosedCallInListFields> SortFieldOptions { get; set; }

//    private CallTypes? _selectedFilterType;
//    public CallTypes? SelectedFilterType
//    {
//        get => _selectedFilterType;
//        set
//        {
//            _selectedFilterType = value;
//            LoadCalls();
//        }
//    }

//    private ClosedCallInListFields? _selectedSortField;
//    public ClosedCallInListFields? SelectedSortField
//    {
//        get => _selectedSortField;
//        set
//        {
//            _selectedSortField = value;
//            LoadCalls();
//        }
//    }

//    public event PropertyChangedEventHandler? PropertyChanged;

//    public VolunteerCallHistoryWindow(int volunteerId)
//    {
//        InitializeComponent();
//        _volunteerId = volunteerId;

//        CallTypeOptions = Enum.GetValues(typeof(CallTypes)) as CallTypes[];
//        SortFieldOptions = Enum.GetValues(typeof(ClosedCallInListFields)) as ClosedCallInListFields[];

//        LoadCalls();

//        DataContext = this;
//    }

//    private void LoadCalls()
//    {
//        try
//        {
//            CallList = _bl.Call.GetClosedCallsForVolunteer(
//                _volunteerId,
//                SelectedFilterType,
//                SelectedSortField
//            );
//        }
//        catch (Exception ex)
//        {
//            MessageBox.Show("Error loading call history: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
//        }
//    }
//    private void Filter_Changed(object sender, SelectionChangedEventArgs e)
//    {
//        LoadCalls();
//    }

//    protected virtual void OnPropertyChanged(string propertyName) =>
//        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
//}
