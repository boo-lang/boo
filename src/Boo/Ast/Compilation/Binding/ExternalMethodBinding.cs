using System;
using System.Reflection;

namespace Boo.Ast.Compilation.Binding
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
		
		public BindingType BindingType
		{
			get
			{
				return BindingType.Method;
			}
		}
		
		public int ParameterCount
		{
			get
			{
				return _mi.GetParameters().Length;
			}
		}
		
		public Type GetParameterType(int parameterIndex)
		{
			return _mi.GetParameters()[parameterIndex].ParameterType;
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
