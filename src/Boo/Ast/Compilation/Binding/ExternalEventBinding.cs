using System;

namespace Boo.Ast.Compilation.Binding
{
	public class ExternalEventBinding : IMemberBinding, ITypedBinding
	{
		BindingManager _bindingManager;
		
		System.Reflection.EventInfo _event;
		
		public ExternalEventBinding(BindingManager bindingManager, System.Reflection.EventInfo event_)
		{
			_bindingManager = bindingManager;
			_event = event_;
		}
		
		public System.Reflection.EventInfo EventInfo
		{
			get
			{
				return _event;
			}
		}
		
		public string Name
		{
			get
			{
				return _event.Name;
			}
		}
		
		public BindingType BindingType
		{
			get
			{
				return BindingType.Event;
			}
		}
		
		public ITypeBinding BoundType
		{
			get
			{
				return _bindingManager.ToTypeBinding(_event.EventHandlerType);
			}
		}
		
		public bool IsStatic
		{
			get
			{
				return false;
			}
		}
	}
}
