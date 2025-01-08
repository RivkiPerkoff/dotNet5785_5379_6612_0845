using System;
using System.Reflection;

namespace DalApi;
public static class Factory
{
    public static IDal Get
    {
        get
        {
            // קבלת שם ה-DAL מהקונפיגורציה
            string dalType = DalApi.DalConfig.s_dalName
                ?? throw new DalConfigException("DAL name is not extracted from the configuration");

            // קבלת הגדרה מהקונפיגורציה
            var dal = DalApi.DalConfig.s_dalPackages[dalType]
                ?? throw new DalConfigException($"Package for {dalType} is not found in packages list in dal-config.xml");

            try
            {
                // טעינת החבילה
                Assembly.Load(dal.Package
                    ?? throw new DalConfigException($"Package {dal.Package} is null"));
            }
            catch (Exception ex)
            {
                throw new DalConfigException($"Failed to load {dal.Package}.dll package", ex);
            }

            // טעינת הסוג (Type) מתוך המחלקה והחבילה
            Type type = Type.GetType($"{dal.Namespace}.{dal.Class}, {dal.Package}")
                ?? throw new DalConfigException($"Class {dal.Namespace}.{dal.Class} was not found in {dal.Package}.dll");

            // קבלת ה-Instance מתוך המחלקה
            return type.GetProperty("Instance", BindingFlags.Public | BindingFlags.Static)?.GetValue(null) as IDal
                ?? throw new DalConfigException($"Class {dal.Class} is not a singleton or wrong property name for Instance");
        }
    }
}
