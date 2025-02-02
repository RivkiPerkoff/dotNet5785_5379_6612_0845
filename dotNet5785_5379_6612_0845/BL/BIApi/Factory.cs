using BL.BIApi;
using NSubstitute.Core;
using System.Xml;

namespace BlApi;
public static class Factory
{
    public static IBl Get() => new lBlImplementation.Bl();


}

