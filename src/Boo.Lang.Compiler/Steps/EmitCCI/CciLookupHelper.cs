using System.Collections.Generic;
using System.Linq;
using Microsoft.Cci;

namespace Boo.Lang.Compiler.Steps.EmitCCI
{
    public static class CciLookupHelper
    {
        public static IMethodDefinition GetMethod(ITypeDefinition declaringType, IName methodName,
            ITypeReference[] parameterTypes)
        {
            var candidates = declaringType.GetMembersNamed(methodName, false)
                .OfType<IMethodDefinition>()
                .Where(m => m.ParameterCount == parameterTypes.Length)
                .ToArray();
            return candidates.Single(c => TypesMatch(c.Parameters, parameterTypes));
        }

        private static bool TypesMatch(IEnumerable<IParameterDefinition> parameters, ITypeReference[] argTypes)
        {
            var i = 0;
            foreach (var param in parameters)
            {
                var arg = argTypes[i];
                if (!TypeHelper.TypesAreEquivalentAssumingGenericMethodParametersAreEquivalentIfTheirIndicesMatch(param.Type, arg, true))
                    return false;
                ++i;
            }
            return true;
        }
    }
}
