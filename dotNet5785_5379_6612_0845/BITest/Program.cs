using System;
using System.Reflection.Metadata;
using BL.BIApi;
using BL.BO;
using BlApi;
using DO;
namespace BlTest
{
    class Program
    {
        static readonly IBL s_bl = Factory.Get();

        static void Main()
        {
            try
            {
                //Console.WriteLine("Please log in.");
                //Console.Write("Username: ");
                //string username = Console.ReadLine()!;

                //Console.Write("Enter Password (must be at least 8 characters, contain upper and lower case letters, a digit, and a special character): ");
                //string password = Console.ReadLine()!;

                //string userRole = s_bl.Volunteer.Login(username, password);
                //Console.WriteLine($"Login successful! Your role is: {userRole}");

                ////בדיקה אם התפקיד הוא Manager
                //if (userRole == "Manager")
                ShowMenu();
                //else
                //{
                //    Console.WriteLine("UpDate Volunteer");
                //    UpDateVolunteer();
                //}
            }
            catch (BL.BO.BlDoesNotExistException ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
            catch (BL.BO.BlInvalidFormatException ex)
            {
                Console.WriteLine("The sub menu choice is not valid.", ex);
            }
            catch (BL.BO.BlGeneralDatabaseException ex)
            {
                Console.WriteLine($"System Error: {ex.Message}");
            }

        }

        static void ShowMenu()
        {
            try
            {
                while (true)
                {
                    Console.WriteLine("\n--- BL Test System ---");
                    Console.WriteLine("1. Administration");
                    Console.WriteLine("2. Volunteers");
                    Console.WriteLine("3. Calls");
                    Console.WriteLine("0. Exit");
                    Console.Write("Choose an option: ");

                    if (int.TryParse(Console.ReadLine(), out int choice))
                    {
                        switch (choice)
                        {
                            case 1:
                                AdminMenu();
                                break;
                            case 2:
                                VolunteerMenu();
                                break;
                            case 3:
                                CallMenu();
                                break;
                            case 0:
                                return;
                            default:
                                Console.WriteLine("Invalid choice. Try again.");
                                break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred while displaying the menu: " + ex.Message);
            }
        }


        static void AdminMenu()
        {
            while (true)
            {
                Console.WriteLine("\n--- Administration ---");
                Console.WriteLine("1. Reset Database");
                Console.WriteLine("2. Initialize Database");
                Console.WriteLine("3. Advance Clock");
                Console.WriteLine("4. Show Clock");
                Console.WriteLine("5. Get Risk Time Range");
                Console.WriteLine("6. Set Risk Time Range");
                Console.WriteLine("0. Back");
                Console.Write("Choose an option: ");

                if (!int.TryParse(Console.ReadLine(), out int choice))
                {
                    Console.WriteLine("Invalid input. Please enter a number.");
                    continue;
                }

                try
                {

                    switch (choice)
                    {
                        case 1:
                            s_bl.Admin.ResetDatabase();
                            Console.WriteLine("Database reset successfully");
                            break;
                        case 2:
                            s_bl.Admin.InitializeDatabase();
                            Console.WriteLine("Database initialized successfully");
                            break;
                        case 3:
                            Console.Write("Enter time unit (Minute, Hour, Day, Month, Year): ");
                            if (Enum.TryParse(Console.ReadLine(), true, out BL.BO.TimeUnit timeUnit))
                            {
                                s_bl.Admin.AdvanceClock(timeUnit);
                                Console.WriteLine("System clock advanced.");
                            }
                            else
                            {
                                throw new FormatException("Invalid time unit. Please enter: Minute, Hour, Day, Month, Year.");
                            }
                            break;
                        case 4:
                            Console.WriteLine($"Current System Clock: {s_bl.Admin.GetClock()}");
                            break;
                        case 5:
                            Console.WriteLine($"Current Risk Time Range: {s_bl.Admin.GetRiskTimeRange()}");
                            break;
                        case 6:
                            Console.Write("Enter new risk time range (hh:mm:ss): ");
                            if (TimeSpan.TryParse(Console.ReadLine(), out TimeSpan timeRange))
                            {
                                s_bl.Admin.SetRiskTimeRange(timeRange);
                                Console.WriteLine("Risk time range updated.");
                            }
                            else
                            {
                                throw new FormatException("Invalid time format. Please use hh:mm:ss.");
                            }
                            break;
                        case 0:
                            return;
                        default:
                            Console.WriteLine("Invalid choice. Try again.");
                            break;
                    }
                }
                catch (BL.BO.BlInvalidFormatException)
                {
                    Console.WriteLine("Invalid time format.");
                }
                catch (BL.BO.BlGeneralDatabaseException ex)
                {
                    Console.WriteLine($"A database error occurred: {ex.Message}");
                }
            }
        }

        static void VolunteerMenu()
        {
            while (true)
            {
                Console.WriteLine("\n--- Volunteer Management ---");
                Console.WriteLine("1. List Volunteers");
                Console.WriteLine("2. Get Filter/Sort volunteer");
                Console.WriteLine("3. Read Volunteer by ID");
                Console.WriteLine("4. Add Volunteer");
                Console.WriteLine("5. Remove Volunteer");
                Console.WriteLine("6. UpDate Volunteer");
                Console.WriteLine("0. Back");
                Console.Write("Choose an option: ");

                if (!int.TryParse(Console.ReadLine(), out int choice))
                    //האם אפשר לעשות פה כזאת זריקה?
                    throw new FormatException("The volunteer menu choice is not valid.");


                switch (choice)
                {
                    case 1:
                        try
                        {
                            foreach (var volunteer in s_bl.Volunteer.GetVolunteers())
                                Console.WriteLine(volunteer.ToString());
                        }
                        catch (BL.BO.BlDoesNotExistException ex)
                        {
                            Console.WriteLine($"Error: {ex.Message}");
                        }
                        catch (BL.BO.BlGeneralDatabaseException ex)
                        {
                            Console.WriteLine($"System Error: {ex.Message}");
                        }
                        break;
                    case 2:
                        try
                        {
                            Console.Write("IsActive? (true/false): ");
                            if (!bool.TryParse(Console.ReadLine(), out bool active))
                                throw new FormatException("Invalid input for IsActive.");

                            Console.Write("enter type sorting: ");
                            if (!TypeSortingVolunteers.TryParse(Console.ReadLine(), out TypeSortingVolunteers type))
                                throw new FormatException("Invalid input for type.");

                            foreach (var volunteer in s_bl.Volunteer.GetVolunteers(active, type))
                                Console.WriteLine(volunteer.ToString());
                        }
                        catch (BL.BO.BlDoesNotExistException ex)
                        {
                            Console.WriteLine($"Error: {ex.Message}");
                        }
                        catch (BL.BO.BlGeneralDatabaseException ex)
                        {
                            Console.WriteLine($"System Error: {ex.Message}");
                        }
                        break;
                    case 3:
                        try
                        {
                            Console.Write("Enter Volunteer ID: ");
                            if (int.TryParse(Console.ReadLine(), out int volunteerId))

                            {
                                var volunteer = s_bl.Volunteer.GetVolunteerDetails(volunteerId);
                                Console.WriteLine(volunteer);
                            }
                            else
                                throw new FormatException("Invalid input. Volunteer ID must be a number.");
                        }
                        catch (BL.BO.BlDoesNotExistException ex)
                        {
                            Console.WriteLine($"Error: {ex.Message}");
                        }
                        break;
                    case 4:
                        try
                        {
                            Console.WriteLine("Enter Volunteer details:");
                            Console.Write("ID: ");
                            if (int.TryParse(Console.ReadLine(), out int id))
                            {
                                BL.BO.Volunteer volunteer = CreateVolunteer(id);
                                s_bl.Volunteer.AddVolunteer(volunteer);
                                Console.WriteLine("Volunteer created successfully!");
                            }
                            else
                                throw new FormatException("Invalid input. Volunteer ID must be a number.");
                        }
                        catch (BL.BO.BlAlreadyExistsException ex)
                        {
                            Console.WriteLine($"Error BlAlreadyExistsException: {ex.Message}");
                        }
                        catch (BL.BO.BlInvalidFormatException ex)
                        {
                            Console.WriteLine($"Input Error: {ex.Message}");
                        }
                        catch (BL.BO.BlPermissionException ex)
                        {
                            Console.WriteLine($"Error BlPermissionException: {ex.Message}");
                        }
                        catch (BL.BO.BlGeolocationNotFoundException ex)
                        {
                            Console.WriteLine($"Error BlGeolocationNotFoundException: {ex.Message}");
                        }
                        catch (BL.BO.BlGeneralDatabaseException ex)
                        {
                            Console.WriteLine($"Error BlGeneralDatabaseException: {ex.Message}");
                        }
                        break;
                    case 5:
                        try
                        {
                            Console.Write("Enter Volunteer ID: ");
                            if (int.TryParse(Console.ReadLine(), out int vId))
                            {
                                s_bl.Volunteer.DeleteVolunteer(vId);
                                Console.WriteLine("Volunteer removed.");
                            }
                            else
                            {
                                throw new FormatException("Invalid input. Volunteer ID must be a number.");
                            }
                        }
                        catch (BL.BO.BlDoesNotExistException ex)
                        {
                            Console.WriteLine($"Error: {ex.Message}");
                        }
                        catch (BL.BO.BlGeneralDatabaseException ex)
                        {
                            Console.WriteLine($"Error: {ex.Message}");
                        }
                        break;
                    case 6:
                        UpDateVolunteer();
                        break;
                    case 0:
                        return;
                    default:
                        //כנל לבדיקה האם לזרוק פה
                        Console.WriteLine("Invalid choice. Try again.");
                        break;
                }
            }
        }

        static BL.BO.Volunteer CreateVolunteer(int requesterId)
        {

            Console.Write("Name: ");
            string? name = Console.ReadLine();

            Console.Write("Phone Number: ");
            string? phoneNumber = Console.ReadLine();

            Console.Write("Email: ");
            string? email = Console.ReadLine();

            Console.Write("IsActive? (true/false): ");
            if (!bool.TryParse(Console.ReadLine(), out bool active))
                throw new FormatException("Invalid input for IsActive.");

            Console.WriteLine("Please enter Role:('Manager' or 'Volunteer').");
            if (!Enum.TryParse(Console.ReadLine(), out BL.BO.Role role))
                throw new FormatException("Invalid role.");

            Console.Write("Password: ");
            string? password = Console.ReadLine();

            Console.Write("Address: ");
            string? address = Console.ReadLine();

            Console.WriteLine("Enter location details:");
            Console.Write("Latitude: ");
            if (!double.TryParse(Console.ReadLine(), out double latitude))
                throw new FormatException("Invalid latitude format.");

            Console.Write("Longitude: ");
            if (!double.TryParse(Console.ReadLine(), out double longitude))
                throw new FormatException("Invalid longitude format.");

            Console.Write("Largest Distance: ");
            if (!double.TryParse(Console.ReadLine(), out double largestDistance))
                throw new FormatException("Invalid largest distance format.");

            Console.Write("Distance Type (AirDistance, RoadDistance, WalkingDistance): ");
            if (!Enum.TryParse(Console.ReadLine(), true, out BL.BO.DistanceType myDistanceType))
                throw new FormatException("Invalid distance type.");

            return new BL.BO.Volunteer
            {
                VolunteerId = requesterId,
                Name = name,
                PhoneNumber = phoneNumber,
                EmailOfVolunteer = email,
                PasswordVolunteer = password,
                AddressVolunteer = address,
                VolunteerLatitude = latitude,
                VolunteerLongitude = longitude,
                IsAvailable = active,
                Role = role,
                DistanceType = myDistanceType,
                MaximumDistanceForReceivingCall = largestDistance,
                TotalCallsHandled = 0,
                TotalCallsCanceled = 0,
                SelectedAndExpiredCalls = 0,
                CallInProgress = null
            };
        }
        static void UpDateVolunteer()
        {
            try
            {
                Console.Write("Enter requester ID: ");
                if (int.TryParse(Console.ReadLine(), out int requesterId))
                {
                    BL.BO.Volunteer boVolunteer = CreateVolunteer(requesterId);
                    s_bl.Volunteer.UpdateVolunteer(requesterId, boVolunteer);
                    Console.WriteLine("Volunteer updated successfully.");
                }
                else
                    throw new FormatException("Invalid input. Volunteer ID must be a number.");
            }
            catch (BL.BO.BlDoesNotExistException ex)
            {
                Console.WriteLine(ex);
            }
            catch (BL.BO.BlUnauthorizedAccessException ex)
            {
                Console.WriteLine(ex);
            }
            catch (BL.BO.BlInvalidOperationException ex)
            {
                Console.WriteLine(ex);
            }
            catch (BL.BO.BlGeneralDatabaseException ex)
            {
                Console.WriteLine(ex);
            }
            catch (Exception ex)
            {
                Console.WriteLine("An unexpected error occurred: " + ex.Message);
            }
        }
        static void CallMenu()
        {
            try
            {
                while (true)
                {
                    Console.WriteLine("\n--- Call Management ---");
                    Console.WriteLine("1. Get call quantities by status");
                    Console.WriteLine("2. Get Closed Calls Handled By Volunteer");
                    Console.WriteLine("3. Show All Calls");
                    Console.WriteLine("4. Read Call by ID");
                    Console.WriteLine("5. Add Call");
                    Console.WriteLine("6. Remove Call");
                    Console.WriteLine("7. Update Call");
                    Console.WriteLine("8. Get Open Calls For Volunteer");
                    Console.WriteLine("9. Mark Call As Canceled");
                    Console.WriteLine("10. Mark Call As Completed");
                    Console.WriteLine("11. Select Call For Treatment");
                    Console.WriteLine("0. Back");
                    Console.Write("Choose an option: ");

                    if (!int.TryParse(Console.ReadLine(), out int choice))
                        throw new FormatException("The call menu choice is not valid.");

                    switch (choice)
                    {
                        case 1:
                            try
                            {
                                int[] callQuantities = s_bl.Call.GetCallAmounts();
                                Console.WriteLine("Call quantities by status:");
                                foreach (BL.BO.StatusCallType status in Enum.GetValues(typeof(BL.BO.StatusCallType)))
                                {
                                    Console.WriteLine($"{status}: {callQuantities[(int)status]}");
                                }
                            }
                            catch (BL.BO.BlGeneralDatabaseException ex)
                            {
                                Console.WriteLine($"Error: {ex.Message}");
                                if (ex.InnerException != null)
                                {
                                    Console.WriteLine($"Internal error: {ex.InnerException.Message}");
                                }
                            }
                            break;
                        case 2:
                            try
                            {
                                Console.Write("Enter Volunteer ID: ");
                                if (int.TryParse(Console.ReadLine(), out int volunteerId))
                                {
                                    Console.WriteLine("Enter Call Type filter: None, ManDriver, WomanDriver (or press Enter to skip):");
                                    string? callTypeInput = Console.ReadLine();
                                    BL.BO.CallTypes? callTypeFilter = Enum.TryParse(callTypeInput, out BL.BO.CallTypes parsedCallType) ? parsedCallType : null;

                                    Console.WriteLine("Enter Sort Field:    Id,\r\n    CallTypes,\r\n    Address,\r\n    OpeningTime,\r\n    EntryTimeForTreatment,\r\n    EndTimeForTreatment,\r\n    FinishCallType (or press Enter to skip):");
                                    string? sortFieldInput = Console.ReadLine();
                                    BL.BO.ClosedCallInListFields? sortField = Enum.TryParse(sortFieldInput, out BL.BO.ClosedCallInListFields parsedSortField) ? parsedSortField : null;

                                    var closedCalls = s_bl.Call.GetClosedCallsForVolunteer(volunteerId, callTypeFilter, sortField);

                                    Console.WriteLine("\nClosed Calls Handled By Volunteer:");
                                    if (!closedCalls.Any())
                                        Console.WriteLine("No closed calls to display.");
                                    else
                                    {
                                        foreach (var call in closedCalls)
                                        {
                                            Console.WriteLine(call);
                                            Console.WriteLine("-------------------");
                                        }
                                    }
                                }
                                else
                                    throw new BL.BO.BlInvalidOperationException("Invalid input. Volunteer ID must be a number.");
                            }
                            catch (BL.BO.BlGeneralDatabaseException ex)
                            {
                                Console.WriteLine($"Error: {ex.Message}");
                                if (ex.InnerException != null)
                                {
                                    Console.WriteLine($"Internal error: {ex.InnerException.Message}");
                                }
                            }
                            break;
                        case 3:
                            try
                            {

                                Console.WriteLine("Enter filter field (StartTime, TimeToCompleteTreatment, LastUpdateBy, Status) or press Enter to skip:");
                                string? filterFieldInput = Console.ReadLine();
                                BL.BO.CallInListFields? filterField = Enum.TryParse(filterFieldInput, out BL.BO.CallInListFields parsedFilterField) ? parsedFilterField : null;

                                object? filterValue = null;
                                if (filterField.HasValue)
                                {
                                    Console.WriteLine("Enter filter value:");
                                    filterValue = Console.ReadLine();
                                }

                                Console.WriteLine("Enter sort field (StartTime, TimeToCompleteTreatment, LastUpdateBy, Status) or press Enter to skip:");
                                string? sortFieldInput = Console.ReadLine();
                                BL.BO.CallInListFields? sortField = Enum.TryParse(sortFieldInput, out BL.BO.CallInListFields parsedSortField) ? parsedSortField : null;

                                var callList = s_bl.Call.GetFilteredAndSortedCallList(filterField, filterValue, sortField);

                                foreach (var call in callList)
                                    Console.WriteLine(call);
                            }
                            catch (BL.BO.BlGeneralDatabaseException ex)
                            {
                                Console.WriteLine($"Exception: {ex.GetType().Name} - {ex.Message}");
                                if (ex.InnerException != null)
                                    Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
                            }
                            break;
                        case 4:
                            try
                            {
                                Console.Write("Enter Call ID: ");
                                if (int.TryParse(Console.ReadLine(), out int callId))
                                {
                                    var call = s_bl.Call.GetCallDetails(callId);
                                    Console.WriteLine(call);
                                }
                                else
                                {
                                    throw new FormatException("Invalid input. Volunteer ID must be a number.");
                                }
                            }
                            catch (BL.BO.BlDoesNotExistException ex)
                            {
                                Console.WriteLine($"Error: {ex.Message}");
                            }
                            catch (BL.BO.BlGeneralDatabaseException ex)
                            {
                                Console.WriteLine($"Error: {ex.Message}");
                            }
                            break;
                        case 5:
                            try
                            {
                                Console.WriteLine("Enter Call details:");
                                Console.Write("ID: ");
                                if (int.TryParse(Console.ReadLine(), out int id))
                                {
                                    BL.BO.Call call = CreateCall(id);
                                    s_bl.Call.AddCall(call);
                                    Console.WriteLine("Call created successfully!");
                                }
                                else
                                    throw new FormatException("Invalid input. Cll ID must be a number.");
                            }
                            catch (BL.BO.BlDoesNotExistException ex)
                            {
                                Console.WriteLine($"Exception: {ex.GetType().Name}, Message: {ex.Message}");
                            }
                            catch (BL.BO.BlInvalidOperationException ex)
                            {
                                Console.WriteLine($"Exception: {ex.GetType().Name}, Message: {ex.Message}");
                            }
                            catch (BL.BO.BlInvalidFormatException ex)
                            {
                                Console.WriteLine($"Exception: {ex.GetType().Name}, Message: {ex.Message}");
                            }
                            catch (BL.BO.BlAlreadyExistsException ex)
                            {
                                Console.WriteLine($"Exception: {ex.GetType().Name}, Message: {ex.Message}");
                            }
                            catch (BL.BO.BlGeneralDatabaseException ex)
                            {
                                Console.WriteLine($"Exception: {ex.GetType().Name}, Message: {ex.Message}");
                                if (ex.InnerException != null)
                                {
                                    Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
                                }
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"An unexpected error occurred: {ex.Message}");
                            }
                            break;
                        case 6:
                            try
                            {
                                Console.Write("Enter Call ID: ");
                                if (int.TryParse(Console.ReadLine(), out int cId))
                                {
                                    s_bl.Call.DeleteCall(cId);
                                    Console.WriteLine("Call removed.");
                                }
                                else
                                {
                                    throw new FormatException("Invalid input. Volunteer ID must be a number.");
                                }
                            }
                            catch (BlDoesNotExistException ex)
                            {
                                Console.WriteLine($"Error: {ex.Message}");
                            }
                            //catch (BlDeletionException ex)
                            //{
                            //    Console.WriteLine($"Error: {ex.Message}");
                            //}
                            catch (BlGeneralDatabaseException ex)
                            {
                                Console.WriteLine($"Error: {ex.Message}");
                            }
                            break;
                        case 7:
                            UpDateCall();
                            break;
                        case 8:
                            try
                            {
                                Console.Write("Enter Volunteer ID: ");
                                if (int.TryParse(Console.ReadLine(), out int volunteerId))
                                {
                                    Console.WriteLine("Enter Call Type filter (or press Enter to skip):");
                                    string? callTypeInput = Console.ReadLine();
                                    BL.BO.CallTypes? callTypeFilter = Enum.TryParse(callTypeInput, out BL.BO.CallTypes parsedCallType) ? parsedCallType : null;

                                    Console.WriteLine("Enter Sort Field (or press Enter to skip):");
                                    string? sortFieldInput = Console.ReadLine();
                                    BL.BO.OpenCallInListFields? sortField = Enum.TryParse(sortFieldInput, out BL.BO.OpenCallInListFields parsedSortField) ? parsedSortField : null;

                                    var openCalls = s_bl.Call.GetOpenCallsForVolunteerSelection(volunteerId, callTypeFilter, sortField);

                                    Console.WriteLine("\nOpen Calls Available for Volunteer:");
                                    if (openCalls == null || !openCalls.Any())
                                        Console.WriteLine("No calls available.");
                                    else
                                    {
                                        foreach (var call in openCalls)
                                        {
                                            Console.WriteLine(call);
                                        }
                                    }
                                }
                                else
                                {
                                    throw new BlInvalidFormatException("Invalid input. Volunteer ID must be a number.");
                                }
                            }
                            catch (BL.BO.BlDoesNotExistException ex)
                            {
                                Console.WriteLine($"Error: {ex.Message}");
                            }
                            catch (BL.BO.BlGeneralDatabaseException ex)
                            {
                                Console.WriteLine($"Error: {ex.Message}");
                                if (ex.InnerException != null)
                                {
                                    Console.WriteLine($"Internal error: {ex.InnerException.Message}");
                                }
                            }
                            break;
                        case 9:
                            try
                            {
                                Console.Write("Enter Volunteer ID: ");
                                if (!int.TryParse(Console.ReadLine(), out int volunteerId))
                                    throw new BlInvalidFormatException("Invalid input. Volunteer ID must be a number.");

                                Console.Write("Enter call ID: ");
                                if (!int.TryParse(Console.ReadLine(), out int assignmentId))
                                    throw new BlInvalidFormatException("Invalid input. call ID must be a number.");

                                s_bl.Call.CancelCallTreatment(volunteerId, assignmentId);
                                Console.WriteLine("The call was successfully canceled.");
                            }
                            catch (BL.BO.BlUnauthorizedAccessException ex)
                            {
                                Console.WriteLine($"Authorization Error: {ex.Message}");
                            }
                            catch (BL.BO.BlInvalidOperationException ex)
                            {
                                Console.WriteLine($"Operation Error: {ex.Message}");
                            }
                            catch (BL.BO.BlGeneralDatabaseException ex)
                            {
                                Console.WriteLine($"Database Error: {ex.Message}");
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"Unexpected error: {ex.Message}");
                            }
                            break;
                        case 10:
                            try
                            {
                                Console.Write("Enter Volunteer ID: ");
                                string? volunteerInput = Console.ReadLine();
                                if (!int.TryParse(volunteerInput, out int volunteerId))
                                {
                                    throw new FormatException("Invalid input. Volunteer ID must be a number.");
                                }

                                Console.Write("Enter Assignment ID: ");
                                string? assignmentInput = Console.ReadLine();
                                if (!int.TryParse(assignmentInput, out int assignmentId))
                                {
                                    throw new FormatException("Invalid input. Assignment ID must be a number.");
                                }

                                s_bl.Call.CompleteCallTreatment(volunteerId, assignmentId);

                                Console.WriteLine("Call completion updated successfully!");
                            }
                            catch (BL.BO.BlDoesNotExistException ex)
                            {
                                Console.WriteLine($"Error: {ex.Message}");
                            }
                            catch (BL.BO.BlUnauthorizedAccessException ex)
                            {
                                Console.WriteLine($"Error: {ex.Message}");
                            }
                            catch (BL.BO.BlInvalidOperationException ex)
                            {
                                Console.WriteLine($"Error: {ex.Message}");
                            }
                            catch (BL.BO.BlGeneralDatabaseException ex)
                            {
                                Console.WriteLine($"Error: {ex.Message}");
                            }
                            break;
                        case 11:
                            try
                            {
                                Console.Write("Enter Volunteer ID: ");
                                if (!int.TryParse(Console.ReadLine(), out int volunteerId))
                                    throw new FormatException("Invalid input. Volunteer ID must be a number.");

                                Console.Write("Enter Call ID: ");
                                if (!int.TryParse(Console.ReadLine(), out int callId))
                                    throw new FormatException("Invalid input. Call ID must be a number.");

                                s_bl.Call.ChoosingCallForTreatment(volunteerId, callId);
                                Console.WriteLine("The call has been successfully assigned to the volunteer.");
                            }
                            catch (FormatException ex)
                            {
                                Console.WriteLine($"Input Error: {ex.Message}");
                            }
                            catch (BL.BO.BlInvalidOperationException ex)
                            {
                                Console.WriteLine($"Operation Error: {ex.Message}");
                            }
                            catch (BL.BO.BlGeneralDatabaseException ex)
                            {
                                Console.WriteLine($"Database Error: {ex.Message}");
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"Unexpected error: {ex.Message}");
                            }
                            break;
                        case 0:
                            return;
                        default:
                            Console.WriteLine("Invalid choice. Try again.");
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
        static BL.BO.Call CreateCall(int id)
        {
            Console.WriteLine("Enter the call type (0 for None, 1 for ManDriver, 2 for WomanDriver):");
            if (!Enum.TryParse(Console.ReadLine(), out BL.BO.CallTypes callType))
                throw new FormatException("Invalid call type.");
            Console.WriteLine("Enter the verbal description:");
            string verbalDescription = Console.ReadLine();

            Console.WriteLine("Enter the address:");
            string address = Console.ReadLine();


            Console.WriteLine("Enter the max finish time (yyyy-mm-dd) or leave empty:");

            string maxFinishTimeInput = Console.ReadLine();
            DateTime? maxFinishTime = string.IsNullOrEmpty(maxFinishTimeInput) ? null : DateTime.Parse(maxFinishTimeInput);


            return new BL.BO.Call
            {
                IdCall = id,
                CallType = callType,
                CallDescription = verbalDescription,
                AddressOfCall = address,
                CallLatitude = 0,
                CallLongitude = 0,
                OpeningTime = DateTime.Now,
                MaxFinishTime = maxFinishTime,
            };
        }
        static void UpDateCall()
        {
            Console.Write("Enter Call ID: ");
            int.TryParse(Console.ReadLine(), out int callId);
            Console.Write("Enter New Description (optional) : ");
            string description = Console.ReadLine();
            Console.Write("Enter New Full Address (optional) : ");
            string address = Console.ReadLine();
            Console.Write("Enter Call Type (optional) : ");
            BL.BO.CallTypes? callType = Enum.TryParse(Console.ReadLine(), out BL.BO.CallTypes parsedType) ? parsedType : (BL.BO.CallTypes?)null;
            Console.Write("Enter Max Finish Time (hh:mm , (optional)): ");
            TimeSpan? maxFinishTime = TimeSpan.TryParse(Console.ReadLine(), out TimeSpan parsedTime) ? parsedTime : (TimeSpan?)null;
            try
            {
                var callToUpdate = s_bl.Call.GetCallDetails(callId);
                if (callToUpdate == null)
                    throw new BL.BO.BlDoesNotExistException($"Call with ID{callId} does not exist!");
                var newUpdatedCall = new BL.BO.Call
                {
                    IdCall = callId,
                    CallDescription = !string.IsNullOrWhiteSpace(description) ? description : callToUpdate.CallDescription,
                    AddressOfCall = !string.IsNullOrWhiteSpace(address) ? address : /*callToUpdate. FullAddress*/"No Address",
                    OpeningTime = callToUpdate.OpeningTime,
                    MaxFinishTime = (maxFinishTime.HasValue ? DateTime.Now.Date + maxFinishTime.Value : callToUpdate.MaxFinishTime),
                    CallType = callType ?? callToUpdate.CallType,

                };
                s_bl.Call.UpdateCallDetails(newUpdatedCall);
                Console.WriteLine("Call updated successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.GetType().Name}, Message: {ex.Message}");
            }
        }
    }
}

