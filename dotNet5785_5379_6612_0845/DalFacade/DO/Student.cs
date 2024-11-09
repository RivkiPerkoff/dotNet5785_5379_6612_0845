namespace DO;

public record Student
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
    public Student() : this(0, "", DateTime.Now, null, false, null) { }
}