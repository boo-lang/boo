using Boo.Lang.Compiler.TypeSystem;
using Boo.Lang.Environments;
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
    internal class FieldBuilder : IBuilder
    {
        private readonly IField _field;
        private readonly IType _type;
        private readonly FieldAttributes _attributes;
        private readonly TypeSystemBridge _typeSystem;
        private readonly object _value;

        public FieldBuilder(IField field, IType type, FieldAttributes attributes, TypeSystemBridge typeSystem, object value)
        {
            _field = field;
            _type = type;
            _attributes = attributes;
            _typeSystem = typeSystem;
            _value = value;
            Handle = typeSystem.ReserveField(field);
        }

        public IEntity Entity => _field;

        public EntityHandle Handle { get; }

        private static IType IsVolatileType = My<TypeSystemServices>.Instance.Map(typeof(System.Runtime.CompilerServices.IsVolatile));

        public void Build()
        {
            var asm = _typeSystem.AssemblyBuilder;
            var type = _field.DeclaringType.ConstructedInfo != null
                ? _field.DeclaringType.ConstructedInfo.GenericDefinition.GetMembers().OfType<IField>().First(f => f.Name == _field.Name).Type
                : _type;
            var enc = new BlobEncoder(new BlobBuilder());
            if (_field.IsVolatile)
            {
                _typeSystem.EncodeTypeModified(enc.FieldSignature(), type, IsVolatileType, true);
            }
            else
            {
                _typeSystem.EncodeType(enc.FieldSignature(), type);
            }
            var handle = asm.AddFieldDefinition(_attributes, asm.GetOrAddString(_field.Name), asm.GetOrAddBlob(enc.Builder));
            if (handle != Handle)
            {
                throw new EcmaBuildException($"Field handle {handle} does not match reserved handle {Handle}");
            }
            if (_value != null)
            {
                asm.AddConstant(handle, _value);
            }
        }
    }
}
