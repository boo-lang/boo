using System;
using System.Reflection;

namespace Boo.Ast.Compilation.NameBinding
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
		
		public NameBindingType BindingType
		{
			get
			{
				return NameBindingType.Type;
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
