using System;
using System.Reflection;
using System.Reflection.Emit;

namespace Boo.Ast.Compilation.Binding
{
	public class InternalMethodBinding : IMethodBinding, INameSpace
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
				return _method.Parameters.Count;
			}
		}
		
		public Type GetParameterType(int parameterIndex)
		{
			return _manager.GetBoundType(_method.Parameters[parameterIndex].Type);
		}
		
		public ITypeBinding ReturnType
		{
			get
			{
				return _manager.GetTypeBinding(_method.ReturnType);
			}
		}
		
		public MethodBase MethodInfo
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
		
		public IBinding Resolve(string name)
		{
			foreach (Local local in _method.Locals)
			{
				if (name == local.Name)
				{
					return _manager.GetBinding(local);
				}
			}
			
			foreach (ParameterDeclaration parameter in _method.Parameters)
			{
				if (name == parameter.Name)
				{
					return _manager.GetBinding(parameter);
				}
			}
			return null;
		}

	}
}
