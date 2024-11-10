namespace DO;

public record Volunteer
(
int Id,
string Name,
DateTime RegistrationDate,
string? Alias = null,
bool IsActive = false,
DateTime? BirthDate = null
)
{
    /// <summary>
    /// Default constructor for stage 3
    /// </summary>
    public Volunteer() : this(0, "", DateTime.Now, null, false, null) { }
}