using System;

namespace Boo.Ast.Compilation.Binding
{
	public enum BindingType
	{
		Type,
		Method,		
		Local,		
		Parameter,
		Ambiguous
	}
	
	public interface IBinding
	{		
		BindingType BindingType
		{
			get;
		}
	}	
	
	public interface ITypeBinding : IBinding
	{
		System.Type Type
		{
			get;
		}
	}
	
	public interface IMethodBinding : IBinding
	{
		int ParameterCount
		{
			get;
		}
		
		Type GetParameterType(int parameterIndex);
		
		System.Reflection.MethodInfo MethodInfo
		{
			get;
		}
		
		ITypeBinding ReturnType
		{
			get;
		}
	}
	
	public class LocalBinding : ITypeBinding
	{		
		Local _local;
		
		ITypeBinding _typeInfo;
		
		System.Reflection.Emit.LocalBuilder _builder;
		
		public LocalBinding(Local local, ITypeBinding typeInfo)
		{			
			_local = local;
			_typeInfo = typeInfo;
		}
		
		public BindingType BindingType
		{
			get
			{
				return BindingType.Local;
			}
		}
		
		public Local Local
		{
			get
			{
				return _local;
			}
		}
		
		public ITypeBinding BoundType
		{
			get
			{
				return _typeInfo;
			}
		}
		
		public Type Type
		{
			get
			{
				return _typeInfo.Type;
			}
		}
		
		public System.Reflection.Emit.LocalBuilder LocalBuilder
		{
			get
			{
				return _builder;
			}
			
			set
			{
				_builder = value;
			}
		}
	}
	
	public class ParameterBinding : ITypeBinding
	{
		ParameterDeclaration _parameter;
		
		ITypeBinding _type;
		
		int _index;
		
		public ParameterBinding(ParameterDeclaration parameter, ITypeBinding type, int index)
		{
			_parameter = parameter;
			_type = type;
			_index = index;
		}
		
		public BindingType BindingType
		{
			get
			{
				return BindingType.Parameter;
			}
		}
		
		public ParameterDeclaration Parameter
		{
			get
			{
				return _parameter;
			}
		}
		
		public ITypeBinding BoundType
		{
			get
			{
				return _type;
			}
		}
		
		public Type Type
		{
			get
			{
				return _type.Type;
			}
		}
		
		public int Index
		{
			get
			{
				return _index;
			}
		}
	}
}
