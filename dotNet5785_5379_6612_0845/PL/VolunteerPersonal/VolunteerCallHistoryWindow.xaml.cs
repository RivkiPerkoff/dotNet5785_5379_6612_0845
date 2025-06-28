using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using BL.BO;

namespace PL.VolunteerPersonal;

public partial class VolunteerCallHistoryWindow : Window
{
    private readonly BL.BIApi.IBL _bl = BlApi.Factory.Get();
    private readonly int _volunteerId;

    public IEnumerable<ClosedCallInList> CallList { get; set; } = new List<ClosedCallInList>();
    public IEnumerable<CallTypes> CallTypeOptions { get; set; }
    public IEnumerable<ClosedCallInListFields> SortFieldOptions { get; set; }

    public CallTypes? SelectedFilterType { get; set; }
    public ClosedCallInListFields? SelectedSortField { get; set; }

    public VolunteerCallHistoryWindow(int volunteerId)
    {
        InitializeComponent();
        _volunteerId = volunteerId;

        CallTypeOptions = Enum.GetValues(typeof(CallTypes)) as CallTypes[];
        SortFieldOptions = Enum.GetValues(typeof(ClosedCallInListFields)) as ClosedCallInListFields[];

        LoadCalls();

        DataContext = this;
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

    private void Filter_Changed(object sender, SelectionChangedEventArgs e)
    {
        LoadCalls();
    }
}
