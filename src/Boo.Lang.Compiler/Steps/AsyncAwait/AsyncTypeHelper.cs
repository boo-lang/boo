using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Boo.Lang.Compiler.TypeSystem;
using Boo.Lang.Environments;

namespace Boo.Lang.Compiler.Steps.AsyncAwait
{
    internal static class AsyncTypeHelper
    {
        private static readonly IType _typeReferenceType;
        private static readonly IType _argIteratorType;
        private static readonly IType _runtimeArgumentHandleType;

        static AsyncTypeHelper()
        {
            var tss = My<TypeSystemServices>.Instance;
            _typeReferenceType = tss.Map(typeof(System.TypedReference));
            _argIteratorType = tss.Map(typeof(System.ArgIterator));
            _runtimeArgumentHandleType = tss.Map(typeof(System.RuntimeArgumentHandle));
        }

        public static bool IsRestrictedType(this IType type)
        {
            return type == _typeReferenceType || type == _argIteratorType || type == _runtimeArgumentHandleType;
        }

        internal static bool IsVerifierReference(this IType type)
        {
            return !type.IsValueType && type.EntityType != EntityType.GenericParameter;
        }
    }
}
