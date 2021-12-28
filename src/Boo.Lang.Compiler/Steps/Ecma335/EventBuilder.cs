using System.Reflection;
using System.Reflection.Metadata;

using Boo.Lang.Compiler.TypeSystem;

namespace Boo.Lang.Compiler.Steps.Ecma335
{
    internal class EventBuilder : IBuilder
    {
        private readonly IEvent _entity;
        private readonly TypeSystemBridge _typeSystem;
        private MethodBuilder _add;
        private MethodBuilder _remove;
        private MethodBuilder _raise;

        public EventBuilder(IEvent entity, TypeSystemBridge typeSystem)
        {
            _entity = entity;
            _typeSystem = typeSystem;
            Handle = typeSystem.ReserveEvent(entity);
        }

        public IEntity Entity => _entity;

        public EntityHandle Handle { get; }

        internal void SetAddOnMethod(MethodBuilder builder)
        {
            _add = builder;
        }

        internal void SetOnRemoveMethod(MethodBuilder builder)
        {
            _remove = builder;
        }

        internal void SetRaiseMethod(MethodBuilder builder)
        {
            _raise = builder;
        }

        public void Build()
        {
            var asm = _typeSystem.AssemblyBuilder;
            var handle = asm.AddEvent(EventAttributes.None, asm.GetOrAddString(_entity.Name), _typeSystem.LookupType(_entity.Type));
            if (handle != Handle)
            {
                throw new EcmaBuildException($"Event handle {handle} does not match reserved handle {Handle}.");
            }
            _typeSystem.EnsureEventMapForType(_entity.DeclaringType, handle);
            if (_add != null)
            {
                asm.AddMethodSemantics(handle, MethodSemanticsAttributes.Adder, (MethodDefinitionHandle)_add.Handle);
            }
            if (_remove != null)
            {
                asm.AddMethodSemantics(handle, MethodSemanticsAttributes.Remover, (MethodDefinitionHandle)_remove.Handle);
            }
            if (_raise != null)
            {
                asm.AddMethodSemantics(handle, MethodSemanticsAttributes.Raiser, (MethodDefinitionHandle)_raise.Handle);
            }
        }

    }
}
