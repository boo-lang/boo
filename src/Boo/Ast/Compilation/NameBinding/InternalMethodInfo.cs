using System;
using System.Reflection;
using System.Reflection.Emit;

namespace Boo.Ast.Compilation.NameBinding
{
	public class InternalMethodInfo : IMethodInfo
	{
		TypeManager _manager;
		
		Boo.Ast.Method _method;
		
		MethodBuilder _builder;
		
		internal InternalMethodInfo(TypeManager manager, Boo.Ast.Method method, MethodBuilder builder)
		{
			_manager = manager;
			_method = method;
			_builder = builder;
		}
		
		public NameInfoType InfoType
		{
			get
			{
				return NameInfoType.MethodInfo;
			}
		}
		
		public int ParameterCount
		{
			get
			{
				return _method.Parameters.Count;
			}
		}
		
		public ITypeInfo ReturnType
		{
			get
			{
				return _manager.GetTypeInfo(_method.ReturnType);
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
