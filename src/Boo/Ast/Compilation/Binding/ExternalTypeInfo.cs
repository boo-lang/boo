using System;
using System.Reflection;

namespace Boo.Ast.Compilation.Binding
{
	public class ExternalTypeBinding : ITypeBinding
	{
		BindingManager _manager;
		
		Type _type;
		
		internal ExternalTypeBinding(BindingManager manager, Type type)
		{
			_manager = manager;
			_type = type;
		}
		
		public BindingType BindingType
		{
			get
			{
				return BindingType.Type;
			}
		}
		
		public Type Type
		{
			get
			{
				return _type;
			}
		}
	}
}
