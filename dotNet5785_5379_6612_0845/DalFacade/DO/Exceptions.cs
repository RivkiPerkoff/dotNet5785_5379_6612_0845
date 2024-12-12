namespace DO;

[Serializable]
public class DalDoesNotExistException : Exception
{
    public DalDoesNotExistException(string? message) : base(message) { }
}

[Serializable]
public class DalExistException : Exception
{
    public DalExistException(string? message) : base(message) { }
}
public class DalDeletionImpossible : Exception
{
    public DalDeletionImpossible(string? message) : base(message) { }
}
public class DalReedAllImpossible : Exception //If there is no data to read
{
    public DalReedAllImpossible(string? message) : base(message) { }
}
public class DalXMLFileLoadCreateException : Exception
{
    public DalXMLFileLoadCreateException(string? message) : base(message) { }
}