using System;
using System.Reflection;

namespace Boo.Ast.Compilation.NameBinding
{
	public class ExternalMethodBinding : IMethodBinding
	{
		BindingManager _manager;
		
		MethodInfo _mi;
		
		internal ExternalMethodBinding(BindingManager manager, MethodInfo mi)
		{
			_manager = manager;
			_mi = mi;
		}
		
		public NameBindingType BindingType
		{
			get
			{
				return NameBindingType.Method;
			}
		}
		
		public int ParameterCount
		{
			get
			{
				return _mi.GetParameters().Length;
			}
		}
		
		public ITypeBinding ReturnType
		{
			get
			{
				return _manager.ToTypeBinding(_mi.ReturnType);
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
