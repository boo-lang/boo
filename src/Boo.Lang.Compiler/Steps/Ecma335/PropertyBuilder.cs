using Boo.Lang.Compiler.TypeSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace Boo.Lang.Compiler.Steps.Ecma335
{
    internal class PropertyBuilder : IBuilder
    {
        private readonly IProperty _entity;
        private readonly PropertyAttributes _attrs;
        private readonly TypeSystemBridge _typeSystem;
        private MethodBuilder _getter;
        private MethodBuilder _setter;

        public PropertyBuilder(IProperty entity, PropertyAttributes attrs, TypeSystemBridge typeSystem)
        {
            _entity = entity;
            _attrs = attrs;
            _typeSystem = typeSystem;
            Handle = typeSystem.ReserveProperty(entity);
        }

        public IEntity Entity => _entity;

        public EntityHandle Handle { get; }

        internal void SetGetMethod(MethodBuilder builder)
        {
            _getter = builder;
        }

        internal void SetSetMethod(MethodBuilder builder)
        {
            _setter = builder;
        }

        public void Build()
        {
            var asm = _typeSystem.AssemblyBuilder;
            var enc = new BlobEncoder(new BlobBuilder());
            var sig = enc.PropertySignature(!_entity.IsStatic);
            var parameters = _entity.GetParameters();
            sig.Parameters(parameters.Length, out var retEnc, out var paramEnc);
            foreach (var param in parameters)
            {
                _typeSystem.EncodeType(paramEnc.AddParameter().Type(), param.Type);
            }
            _typeSystem.EncodeType(retEnc.Type(), _entity.Type);
            var handle = asm.AddProperty(_attrs, asm.GetOrAddString(_entity.Name), asm.GetOrAddBlob(sig.Builder));
            if (handle != Handle)
            {
                throw new EcmaBuildException($"Event handle {handle} does not match reserved handle {Handle}.");
            }
            _typeSystem.EnsurePropertyMapForType(_entity.DeclaringType, handle);
            if (_getter!= null)
            {
                asm.AddMethodSemantics(handle, MethodSemanticsAttributes.Getter, (MethodDefinitionHandle)_getter.Handle);
            }
            if (_setter != null)
            {
                asm.AddMethodSemantics(handle, MethodSemanticsAttributes.Setter, (MethodDefinitionHandle)_setter.Handle);
            }
        }
    }
}
