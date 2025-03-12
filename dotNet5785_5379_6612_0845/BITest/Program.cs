using System;
using System.Reflection.Metadata;
using BL.BIApi;
using BL.BO;
using BlApi;
using BO;
using DO;
///לבדוק מה עם כל הזריקות שיש פה
namespace BlTest
{
    class Program
    {
        static readonly IBL s_bl = Factory.Get();

        static void Main()
        {
            try
            {
                Console.WriteLine("Please log in.");
                Console.Write("Username: ");
                string username = Console.ReadLine()!;

                Console.Write("Enter Password (must be at least 8 characters, contain upper and lower case letters, a digit, and a special character): ");
                string password = Console.ReadLine()!;

                //BL.BO.Role userRole = s_bl.Volunteer.Login(username,password);
                string userRole = s_bl.Volunteer.Login(username, password);
                Console.WriteLine($"Login successful! Your role is: {userRole}");

                //בדיקה אם התפקיד הוא Manager
                if (userRole == "Manager")
                {
                    //הכניסה ללולאת התפריט רק אם התפקיד הוא Manager
                    ShowMenu();
                }
                else
                {
                    Console.WriteLine("UpDate Volunteer");
                    UpDateVolunteer();
                }
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
                //Console.WriteLine("2. Get Filter/Sort volunteer");
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
                                Console.WriteLine(volunteer);
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
                    //case 2:
                    //    try
                    //    {
                    //        bool? IsAvailable;
                    //        BL.BO.VolunteerSortField? sortBy;
                    //        GetVolunteerFilterAndSortCriteria(out IsAvailable, out sortBy);
                    //        var volunteersList = s_bl.Volunteer.GetVolunteers(IsAvailable, sortBy);
                    //        if (volunteersList != null)
                    //            foreach (var volunteer in volunteersList)
                    //                Console.WriteLine(volunteer);
                    //        else
                    //            Console.WriteLine("No volunteers found matching the criteria.");
                    //    }
                    //    catch (BL.BO.BlDoesNotExistException ex)
                    //    {
                    //        Console.WriteLine($"Error: {ex.Message}");
                    //    }
                    //    catch (BL.BO.BlGeneralDatabaseException ex)
                    //    {
                    //        Console.WriteLine($"System Error: {ex.Message}");
                    //    }
                    //    break;
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
                        catch (BL.BO.BlApiRequestException ex)
                        {
                            Console.WriteLine($"Error BlApiRequestException: {ex.Message}");
                        }
                        catch (BO.BlGeolocationNotFoundException ex)
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
        //public static void GetVolunteerFilterAndSortCriteria(out bool? isActive, out BL.BO.VolunteerSortField? sortBy)
        //{
        //    isActive = null;
        //    sortBy = null;

        //    try
        //    {

        //        Console.WriteLine("Is the volunteer active? (yes/no or leave blank for null): ");
        //        string activeInput = Console.ReadLine();

        //        if (!string.IsNullOrEmpty(activeInput))
        //        {
        //            if (activeInput.Equals("yes", StringComparison.OrdinalIgnoreCase))
        //                isActive = true;
        //            else if (activeInput.Equals("no", StringComparison.OrdinalIgnoreCase))
        //                isActive = false;
        //            else
        //                Console.WriteLine("Invalid input for active status. Defaulting to null.");
        //        }

        //        Console.WriteLine("Choose how to sort the volunteers by: ");
        //        Console.WriteLine("1. ID");
        //        Console.WriteLine("2. Name");
        //        Console.WriteLine("3. Total Responses Handled");
        //        Console.WriteLine("4. Total Responses Cancelled");
        //        Console.WriteLine("5. Total Expired Responses");
        //        Console.WriteLine("6. Sum of Calls");
        //        Console.WriteLine("7. Sum of Cancellations");
        //        Console.WriteLine("8. Sum of Expired Calls");
        //        Console.WriteLine("Select sorting option by number: ");
        //        string sortInput = Console.ReadLine();

        //        if (int.TryParse(sortInput, out int sortOption))
        //        {
        //            switch (sortOption)
        //            {
        //                case 1:
        //                    sortBy = BO.VolunteerSortField.Id;
        //                    break;
        //                case 2:
        //                    sortBy = BO.VolunteerSortField.Name;
        //                    break;
        //                case 3:
        //                    sortBy = BO.VolunteerSortField.TotalResponsesHandled;
        //                    break;
        //                case 4:
        //                    sortBy = BO.VolunteerSortField.TotalResponsesCancelled;
        //                    break;
        //                case 5:
        //                    sortBy = BO.VolunteerSortField.TotalExpiredResponses;
        //                    break;
        //                case 6:
        //                    sortBy = BO.VolunteerSortField.SumOfCalls;
        //                    break;
        //                case 7:
        //                    sortBy = BO.VolunteerSortField.SumOfCancellation;
        //                    break;
        //                case 8:
        //                    sortBy = BO.VolunteerSortField.SumOfExpiredCalls;
        //                    break;
        //                default:
        //                    Console.WriteLine("Invalid selection. Defaulting to sorting by ID.");
        //                    break;
        //            }
        //        }
        //        else
        //        {
        //            throw new FormatException("Invalid input for sorting option. Defaulting to sorting by ID.");
        //        }
        //    }
        //    catch (BL.BO.BlGeneralDatabaseException ex)
        //    {
        //        Console.WriteLine($"Exception: {ex.GetType().Name}");
        //        Console.WriteLine($"Message: {ex.Message}");
        //    }
        //}
        //מה לעשות עם כל הTRY ועם הזריקות
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

            Console.WriteLine("Please enter Role: 'Manager' or 'Volunteer'.");
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

            Console.Write("Distance Type (Air, Drive or Walk): ");
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
        //לדעת לטפל בזריקות עם כל הTRY
        static void UpDateVolunteer()
        {

            //מה עושים עם כל אלה בעדכון?
            //TotalCallsHandled = 0,
            //     TotalCallsCancelled = 0,
            //     TotalExpiredCallsChosen = 0,
            //צריך פשוט לא לעדכן אותם
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
            catch (BO.BlUnauthorizedAccessException ex)
            {
                Console.WriteLine(ex);
            }
            catch (BO.BlInvalidFormatException ex)
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
                    Console.WriteLine("3. Show All Callsl");
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

                                foreach (BO.Status status in Enum.GetValues(typeof(BO.Status)))
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
                                    Console.WriteLine("Enter Call Type filter (or press Enter to skip):");
                                    string? callTypeInput = Console.ReadLine();
                                    BL.BO.CallTypes? callTypeFilter = Enum.TryParse(callTypeInput, out BL.BO.CallTypes parsedCallType) ? parsedCallType : null;

                                    Console.WriteLine("Enter Sort Field (or press Enter to skip):");
                                    string? sortFieldInput = Console.ReadLine();
                                    BL.BO.ClosedCallInListFields? sortField = Enum.TryParse(sortFieldInput, out BL.BO.ClosedCallInListFields parsedSortField) ? parsedSortField : null;

                                    var closedCalls = s_bl.Call.GetClosedCallsForVolunteer(volunteerId, callTypeFilter, sortField);

                                    Console.WriteLine("\nClosed Calls Handled By Volunteer:");
                                    foreach (var call in closedCalls)
                                    {
                                        Console.WriteLine(call);
                                    }
                                }
                                else
                                {
                                    throw new BO.BlInvalidFormatException("Invalid input. Volunteer ID must be a number.");
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
                        case 3:
                            try
                            {
                                Console.WriteLine("Enter sort field (CallId, CallType, OpenTime, TimeRemainingToCall, LastVolunteer, CompletionTime, MyStatus, TotalAllocations) or press Enter to skip:");
                                string? filterFieldInput = Console.ReadLine();
                                BL.BO.CallInListFields? filterField = Enum.TryParse(filterFieldInput, out BL.BO.CallInListFields parsedFilterField) ? parsedFilterField : null;

                                object? filterValue = null;
                                if (filterField.HasValue)
                                {
                                    Console.WriteLine("Enter filter value:");
                                    filterValue = Console.ReadLine();
                                }

                                Console.WriteLine("Enter sort field (CallId, CallType, OpenTime, TimeRemainingToCall, LastVolunteer, CompletionTime, MyStatus, TotalAllocations) or press Enter to skip:");
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
                            catch (BO.BlInvalidFormatException ex)
                            {
                                Console.WriteLine($"Exception: {ex.GetType().Name}, Message: {ex.Message}");
                            }
                            catch (BO.BlAlreadyExistsException ex)
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
                            ;
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
                            catch (BlDeletionException ex)
                            {
                                Console.WriteLine($"Error: {ex.Message}");
                            }
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
                                    BO.CallType? callTypeFilter = Enum.TryParse(callTypeInput, out BO.CallType parsedCallType) ? parsedCallType : null;

                                    Console.WriteLine("Enter Sort Field (or press Enter to skip):");
                                    string? sortFieldInput = Console.ReadLine();
                                    BL.BO.OpenCallInListFields? sortField = Enum.TryParse(sortFieldInput, out BL.BO.OpenCallInListFields parsedSortField) ? parsedSortField : null;

                                    var openCalls = s_bl.Call.GetOpenCallsForVolunteer(volunteerId, callTypeFilter, sortField);

                                    Console.WriteLine("\nOpen Calls Available for Volunteer:");
                                    foreach (var call in openCalls)
                                    {
                                        Console.WriteLine(call);
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

                                s_bl.Call.UpdateCallCancellation(volunteerId, assignmentId);
                                Console.WriteLine("The call was successfully canceled.");
                            }
                            catch (BO.BlUnauthorizedAccessException ex)
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

                                s_bl.Call.UpdateCallCompletion(volunteerId, assignmentId);

                                Console.WriteLine("Call completion updated successfully!");
                            }
                            catch (BL.BO.BlDoesNotExistException ex)
                            {
                                Console.WriteLine($"Error: {ex.Message}");
                            }
                            catch (BO.BlUnauthorizedAccessException ex)
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

                                s_bl.Call.SelectCallForTreatment(volunteerId, callId);
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
            Console.WriteLine("Enter the call type (0 for None, 1 for MusicPerformance, 2 for MusicTherapy, 3 for SingingAndEmotionalSupport, 4 for GroupActivities, 5 for PersonalizedMusicCare):");
            if (!Enum.TryParse(Console.ReadLine(), out BO.CallType callType))
            {
                throw new FormatException("Invalid call type.");
            }

            Console.WriteLine("Enter the verbal description:");
            string verbalDescription = Console.ReadLine();

            Console.WriteLine("Enter the address:");
            string address = Console.ReadLine();

            //Console.WriteLine("Enter the latitude:");
            //if (!double.TryParse(Console.ReadLine(), out double latitude))
            //{
            //    throw new FormatException("Invalid latitude value.");
            //}

            //Console.WriteLine("Enter the longitude:");
            //if (!double.TryParse(Console.ReadLine(), out double longitude))
            //{
            //    throw new FormatException("Invalid longitude value.");
            //}

            Console.WriteLine("Enter the max finish time (yyyy-mm-dd) or leave empty:");
            string maxFinishTimeInput = Console.ReadLine();
            DateTime? maxFinishTime = string.IsNullOrEmpty(maxFinishTimeInput) ? null : DateTime.Parse(maxFinishTimeInput);

            Console.WriteLine("Enter the status (0 for InProgress, 1 for AtRisk, 2 for InProgressAtRisk, 3 for Opened, 4 for Closed, 5 for Expired):");
            if (!Enum.TryParse(Console.ReadLine(), out Status status))
            {
                throw new FormatException("Invalid status.");
            }

            return new BO.Call
            {
                Id = id,
                MyCallType = callType,
                VerbalDescription = verbalDescription,
                Address = address,
                Latitude = 0,
                Longitude = 0,
                //האם זה הזמן הנוכחי?
                OpenTime = DateTime.Now,
            };


        }
        //אני באמצע הפונ
        //למה היא לא מקבלת ID?
        static void UpDateCall()
        {
            Console.Write("Enter Call ID: ");
            int.TryParse(Console.ReadLine(), out int callId);
            Console.Write("Enter New Description (optional) : ");
            string description = Console.ReadLine();
            Console.Write("Enter New Full Address (optional) : ");
            string address = Console.ReadLine();
            Console.Write("Enter Call Type (optional) : ");
            BO.CallType? callType = Enum.TryParse(Console.ReadLine(), out BO.CallType parsedType) ? parsedType : (BO.CallType?)null;
            Console.Write("Enter Max Finish Time (hh:mm , (optional)): ");
            TimeSpan? maxFinishTime = TimeSpan.TryParse(Console.ReadLine(), out TimeSpan parsedTime) ? parsedTime : (TimeSpan?)null;
            try
            {
                var callToUpdate = s_bl.Call.GetCallDetails(callId);
                if (callToUpdate == null)
                    throw new BL.BO.BlDoesNotExistException($"Call with ID{callId} does not exist!");
                var newUpdatedCall = new BO.Call
                {
                    Id = callId,
                    VerbalDescription = !string.IsNullOrWhiteSpace(description) ? description : callToUpdate.VerbalDescription,
                    Address = !string.IsNullOrWhiteSpace(address) ? address : /*callToUpdate. FullAddress*/"No Address",
                    OpenTime = callToUpdate.OpenTime,
                    MaxFinishTime = (maxFinishTime.HasValue ? DateTime.Now.Date + maxFinishTime.Value : callToUpdate.MaxFinishTime),
                    MyCallType = callType ?? callToUpdate.MyCallType
                };
                s_bl.Call.UpdateCallDetails(newUpdatedCall);
                Console.WriteLine("Call updated successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.GetType().Name}, Message: {ex.Message}");
            }
            //try
            //{
            //    Console.Write("Enter requester ID: ");
            //    if (int.TryParse(Console.ReadLine(), out int requesterId))
            //    {
            //        //BO.Call boCall = CreateCall(requesterId);
            //        //s_bl.Call.UpdateCallDetails(requesterId, boCall);
            //        Console.WriteLine("Volunteer updated successfully.");
            //    }
            //    else
            //        throw new FormatException("Invalid input. Volunteer ID must be a number.");
            //}
            //catch { }
            //}
        }
    }
}

