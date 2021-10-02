using System.Collections.Generic;

namespace System.Reflection
{
    public static class AssemblyExtension
    {
        public static IReadOnlyList<Type> GetAllTypes(this Assembly assembly)
        {
            try
            {
                return assembly.GetTypes();
            }
            catch (ReflectionTypeLoadException ex)
            {
                return ex.Types;
            }
        }
    }
}