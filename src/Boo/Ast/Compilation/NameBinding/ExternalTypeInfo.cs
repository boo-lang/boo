using System;
using System.Reflection;

namespace Boo.Ast.Compilation.NameBinding
{
	public class ExternalTypeInfo : ITypeInfo
	{
		TypeManager _manager;
		
		Type _type;
		
		internal ExternalTypeInfo(TypeManager manager, Type type)
		{
			_manager = manager;
			_type = type;
		}
		
		public NameInfoType InfoType
		{
			get
			{
				return NameInfoType.TypeInfo;
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
