using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Cci;
using Microsoft.Cci.MutableCodeModel;

namespace Boo.Lang.Compiler.Steps.EmitCCI
{
    public class GenericMethodSpecializer : MetadataCopier
    {
        private readonly IGenericParameterReference[] _refs;

        public GenericMethodSpecializer(IMetadataHost targetHost, IEnumerable<IGenericParameterReference> refs)
            : base(targetHost)
        {
            _refs = refs.ToArray();
        }

        protected override ITypeReference DeepCopy(ITypeReference value)
        {
            var gen = value as IGenericParameterReference;
            if (gen == null)
                return base.DeepCopy(value);
            var replacement = _refs.SingleOrDefault(r => r.Name.UniqueKey == gen.Name.UniqueKey);
            return replacement ?? base.DeepCopy(value);
        }

    }
}
