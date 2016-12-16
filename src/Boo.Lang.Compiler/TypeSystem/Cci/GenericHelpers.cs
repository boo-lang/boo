using System.Linq;
using Microsoft.Cci;

namespace Boo.Lang.Compiler.TypeSystem.Cci
{
    //Implementations for ContainsGenericParameters are adapted from CoreCLR's code
    public static class GenericHelpers
    {
        //https://github.com/dotnet/coreclr/blob/master/src/mscorlib/src/System/Type.cs
        public static bool ContainsGenericParameters(this ITypeDefinition value)
        {
            var arrType = value as IArrayTypeReference;
            if (arrType != null)
                return arrType.ElementType.ResolvedType.ContainsGenericParameters();
            if (value is IGenericParameterReference)
                return true;
            if (!value.IsGeneric)
                return false;

            var gtr = value as IGenericTypeInstanceReference;
            if (gtr == null)
                return true;
            return gtr.GenericArguments.Select(a => a.ResolvedType).Any(t => t.ContainsGenericParameters());
        }

        //https://github.com/dotnet/coreclr/blob/master/src/mscorlib/src/System/Reflection/MethodInfo.cs
        public static bool ContainsGenericParameters(this IMethodDefinition value)
        {
            if (value.ContainingTypeDefinition.ContainsGenericParameters())
                return true;
            if (!value.IsGeneric)
                return false;
            var mir = value as IGenericMethodInstanceReference;
            if (mir == null)
                return true;
            return mir.GenericArguments.Select(a => a.ResolvedType).Any(t => t.ContainsGenericParameters());
        }

        public static bool IsGenericMethodDefinition(this IMethodDefinition value)
        {
            return value.IsGeneric && !(value is IGenericMethodInstanceReference);
        }
    }
}
