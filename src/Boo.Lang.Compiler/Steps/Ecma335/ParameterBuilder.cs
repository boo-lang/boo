using Boo.Lang.Compiler.TypeSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace Boo.Lang.Compiler.Steps.Ecma335
{
    internal class ParameterBuilder : IBuilder
    {
        private readonly IParameter _param;
        private readonly int _index;
        private readonly ParameterAttributes _attributes;
        private readonly TypeSystemBridge _typeSystem;

        public ParameterBuilder(IParameter param, int index, ParameterAttributes attributes, TypeSystemBridge typeSystem)
        {
            _param = param;
            _index = index;
            _attributes = attributes;
            _typeSystem = typeSystem;
            Handle = typeSystem.ReserveParameter(param);
        }

        public IEntity Entity => _param;

        public EntityHandle Handle { get; }

        public void Build()
        {
            var asm = _typeSystem.AssemblyBuilder;
            var handle = asm.AddParameter(_attributes, asm.GetOrAddString(_param.Name), _index);
            if (handle != Handle)
            {
                throw new EcmaBuildException($"Parameter build handle {handle} does not match reserved handle {Handle}.");
            }
        }
    }
}
