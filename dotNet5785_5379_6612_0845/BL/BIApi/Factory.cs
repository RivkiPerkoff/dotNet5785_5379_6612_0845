using BL.BIApi;
using NSubstitute.Core;
using System.Xml;

namespace BlApi;
public static class Factory
{
    public static IBL Get() => new BL.BlImplementation.Bl();


}

