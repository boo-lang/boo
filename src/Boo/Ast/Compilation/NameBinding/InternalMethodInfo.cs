using System;
using System.Reflection;
using System.Reflection.Emit;

namespace Boo.Ast.Compilation.NameBinding
{
	public class InternalMethodBinding : IMethodBinding
	{
		BindingManager _manager;
		
		Boo.Ast.Method _method;
		
		MethodBuilder _builder;
		
		internal InternalMethodBinding(BindingManager manager, Boo.Ast.Method method, MethodBuilder builder)
		{
			_manager = manager;
			_method = method;
			_builder = builder;
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
				return _method.Parameters.Count;
			}
		}
		
		public ITypeBinding ReturnType
		{
			get
			{
				return _manager.GetTypeBinding(_method.ReturnType);
			}
		}
		
		public MethodInfo MethodInfo
		{
			get
			{
				return _builder;
			}
		}
		
		public MethodBuilder MethodBuilder
		{
			get
			{
				return _builder;
			}
		}
	}
}
