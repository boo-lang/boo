using System;

namespace Boo.Ast.Compilation.Binding
{
	public enum BindingType
	{
		Type,
		TypeReference,
		Method,		
		Constructor,
		Field,
		Property,
		Event,
		Local,		
		Parameter,
		Assembly,
		Namespace,
		Ambiguous,
		Error
	}
	
	public interface IBinding
	{	
		string Name
		{
			get;
		}
		BindingType BindingType
		{
			get;
		}
	}	
	
	public interface ITypedBinding : IBinding
	{
		ITypeBinding BoundType
		{
			get;			
		}
	}
	
	public interface IMemberBinding : ITypedBinding
	{
		bool IsStatic
		{
			get;
		}
	}
	
	public interface IFieldBinding : IMemberBinding
	{
		System.Reflection.FieldInfo FieldInfo
		{
			get;
		}
	}
	
	public interface IPropertyBinding : IMemberBinding
	{
		System.Reflection.PropertyInfo PropertyInfo
		{
			get;
		}
	}
	
	public interface ITypeBinding : ITypedBinding, INamespace
	{		
		System.Type Type
		{
			get;
		}
		IConstructorBinding[] GetConstructors();
	}
	
	public interface IMethodBinding : IMemberBinding
	{
		int ParameterCount
		{
			get;
		}
		
		Type GetParameterType(int parameterIndex);
		
		System.Reflection.MethodBase MethodInfo
		{
			get;
		}
		
		ITypeBinding ReturnType
		{
			get;
		}
	}
	
	public interface IConstructorBinding : IMethodBinding
	{
		System.Reflection.ConstructorInfo ConstructorInfo
		{
			get;
		}
	}	
}
