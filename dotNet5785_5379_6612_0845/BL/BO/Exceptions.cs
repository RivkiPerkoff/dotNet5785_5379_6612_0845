namespace BL.BO;
[Serializable]
public class BlDoesNotExistException : Exception
{
    public BlDoesNotExistException(string? message) : base(message) { }
    public BlDoesNotExistException(string message, Exception innerException)
    : base(message, innerException) { }
}
public class BlNullPropertyException : Exception
{
    public BlNullPropertyException(string? message) : base(message) { }
    public BlNullPropertyException(string message, Exception innerException)
   : base(message, innerException) { }
}
public class BlExistException : Exception
{
    public BlExistException(string? message) : base(message) { }
    public BlExistException(string message, Exception innerException)
   : base(message, innerException) { }
}
public class BlDeletionImpossible : Exception
{
    public BlDeletionImpossible(string? message) : base(message) { }
    public BlDeletionImpossible(string message, Exception innerException)
   : base(message, innerException) { }
}
public class BlReedAllImpossible : Exception //If there is no data to read
{
    public BlReedAllImpossible(string? message) : base(message) { }
    public BlReedAllImpossible(string message, Exception innerException)
   : base(message, innerException) { }
}



