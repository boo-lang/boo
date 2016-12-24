using System.Collections.Generic;
using Microsoft.Cci;
using Microsoft.Cci.MutableCodeModel;

namespace Boo.Lang.Compiler.Steps.EmitCCI
{
    public class GenericMethodSpecializer : MetadataCopier
    {
        private readonly Dictionary<int, ITypeDefinition> _refs;

        public GenericMethodSpecializer(IMetadataHost targetHost, Dictionary<int, ITypeDefinition> refs)
            : base(targetHost)
        {
            _refs = refs;
        }

        protected override ITypeReference DeepCopy(ITypeReference value)
        {
            var gen = value as IGenericParameterReference;
            if (gen == null)
                return base.DeepCopy(value);
            ITypeDefinition replacement;
            if (_refs.TryGetValue(gen.Name.UniqueKey, out replacement))
                return replacement;
            return base.DeepCopy(value);
        }

    }
}
