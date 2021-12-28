using System;
using System.Reflection;
using System.Reflection.Metadata;

using Boo.Lang.Compiler.TypeSystem;

namespace Boo.Lang.Compiler.Steps.Ecma335
{
    internal class GenericTypeParameterBuilder : IBuilder
    {
        private readonly IGenericParameter _entity;
        private readonly IBuilder _parent;
        private readonly TypeSystemBridge _typeSystem;
        private GenericParameterAttributes _attrs;
        private List<IType> _baseTypes = new();

        public GenericTypeParameterBuilder(IGenericParameter entity, IBuilder parent, TypeSystemBridge typeSystem)
        {
            _entity = entity;
            _parent = parent;
            _typeSystem = typeSystem;
            Handle = typeSystem.ReserveGenericParameter(entity);
        }

        public IEntity Entity => _entity;
        public EntityHandle Handle { get; }

        internal void SetBaseTypeConstraint(IType baseType)
        {
            _baseTypes.Add(baseType);
        }

        internal void SetInterfaceConstraints(IType[] interfaceTypes)
        {
            _baseTypes.AddRange(interfaceTypes);
        }

        internal void SetGenericParameterAttributes(GenericParameterAttributes attributes)
        {
            _attrs = attributes;
        }

        public void Build()
        {
            var asm = _typeSystem.AssemblyBuilder;
            var handle = asm.AddGenericParameter(_parent.Handle, _attrs, asm.GetOrAddString(_entity.Name), _entity.GenericParameterPosition);
            if (handle != Handle)
            {
                throw new EcmaBuildException($"Generic parameter build handle {handle} does not match reserved handle {Handle}.");
            }
            foreach (var type in _baseTypes)
            {
                asm.AddGenericParameterConstraint(handle, _typeSystem.LookupType(type));
            }
        }
    }
}
