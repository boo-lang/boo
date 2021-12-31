using System.Reflection;
using System.Reflection.Metadata;

using Boo.Lang.Compiler.TypeSystem;

namespace Boo.Lang.Compiler.Steps.Ecma335
{
    internal class GenericTypeParameterBuilder
    {
        private readonly IGenericParameter _entity;
        private readonly IBuilder _parent;
        private readonly TypeSystemBridge _typeSystem;
        private GenericParameterAttributes _attrs;
        private readonly List<IType> _baseTypes = new();

        public GenericTypeParameterBuilder(IGenericParameter entity, IBuilder parent, TypeSystemBridge typeSystem)
        {
            _entity = entity;
            _parent = parent;
            _typeSystem = typeSystem;
            typeSystem.RegisterGenericParameter(parent.Handle, this);
        }

        public IEntity Entity => _entity;

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
            foreach (var type in _baseTypes)
            {
                asm.AddGenericParameterConstraint(handle, _typeSystem.LookupType(type));
            }
        }
    }
}
