using System;
using System.Reflection;

namespace Boo.Ast.Compilation.NameBinding
{
	public class ExternalMethodInfo : IMethodInfo
	{
		TypeManager _manager;
		
		MethodInfo _mi;
		
		internal ExternalMethodInfo(TypeManager manager, MethodInfo mi)
		{
			_manager = manager;
			_mi = mi;
		}
		
		public NameInfoType InfoType
		{
			get
			{
				return NameInfoType.Method;
			}
		}
		
		public int ParameterCount
		{
			get
			{
				return _mi.GetParameters().Length;
			}
		}
		
		public ITypeInfo ReturnType
		{
			get
			{
				return _manager.ToTypeInfo(_mi.ReturnType);
			}
		}
		
		public MethodInfo MethodInfo
		{
			get
			{
				return _mi;
			}
		}
	}
}
