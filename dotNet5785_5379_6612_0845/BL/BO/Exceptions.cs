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
public class BlReedAllImpossible : Exception 
{
    public BlReedAllImpossible(string? message) : base(message) { }
    public BlReedAllImpossible(string message, Exception innerException)
   : base(message, innerException) { }
}
public class BlInvalidOperationException : Exception 
{
    public BlInvalidOperationException(string? message) : base(message) { }
    public BlInvalidOperationException(string message, Exception innerException)
   : base(message, innerException) { }
}
public class BlGeneralDatabaseException : Exception 
{
    public BlGeneralDatabaseException(string? message) : base(message) { }
    public BlGeneralDatabaseException(string message, Exception innerException)
   : base(message, innerException) { }
}

public class BlPermissionException : Exception 
{
    public BlPermissionException(string? message) : base(message) { }
    public BlPermissionException(string message, Exception innerException)
   : base(message, innerException) { }
}
public class BlAlreadyExistsException : Exception
{
    public BlAlreadyExistsException(string? message) : base(message) { }
    public BlAlreadyExistsException(string message, Exception innerException)
   : base(message, innerException) { }
}
public class BlGeolocationNotFoundException : Exception
{
    public BlGeolocationNotFoundException(string? message) : base(message) { }
    public BlGeolocationNotFoundException(string message, Exception innerException)
   : base(message, innerException) { }
}

public class BlUnauthorizedAccessException : Exception
{
    public BlUnauthorizedAccessException(string? message) : base(message) { }
    public BlUnauthorizedAccessException(string message, Exception innerException)
   : base(message, innerException) { }
}

public class BlInvalidFormatException : Exception
{
    public BlInvalidFormatException(string? message) : base(message) { }
    public BlInvalidFormatException(string message, Exception innerException)
   : base(message, innerException) { }
}
public class BlValidationException : Exception
{
    public BlValidationException(string? message) : base(message) { }
    public BlValidationException(string message, Exception innerException)
   : base(message, innerException) { }
}

public class BLTemporaryNotAvailableException : Exception
{
    public BLTemporaryNotAvailableException(string? message) : base(message) { }
    public BLTemporaryNotAvailableException(string message, Exception innerException)
   : base(message, innerException) { }
}
