using BL.BIApi;

namespace BlApi;
public static class Factory
{
    public static IBL Get() => new BL.BlImplementation.Bl();
}

