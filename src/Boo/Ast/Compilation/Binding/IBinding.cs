using System;

namespace Boo.Ast.Compilation.Binding
{
	public enum BindingType
	{
		Type,
		Method,		
		Constructor,
		Field,
		Property,
		Local,		
		Parameter,
		Assembly,
		Namespace,
		Ambiguous,
		Error
	}
	
	public interface IBinding
	{				
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
		
		System.Type Type
		{
			get;
		}
	}
	
	public interface IPropertyBinding : ITypedBinding
	{
		System.Reflection.PropertyInfo PropertyInfo
		{
			get;
		}
	}
	
	public interface ITypeBinding : ITypedBinding, INameSpace
	{		
		IConstructorBinding[] GetConstructors();
	}
	
	public interface IMethodBinding : IBinding
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
	
	public class AmbiguousBinding : IBinding
	{
		IBinding[] _bindings;
		
		public AmbiguousBinding(IBinding[] bindings)
		{
			if (null == bindings)
			{
				throw new ArgumentNullException("bindings");
			}
			if (0 == bindings.Length)
			{
				throw new ArgumentException("bindings");
			}
			_bindings = bindings;
		}
		
		public BindingType BindingType
		{
			get
			{
				return BindingType.Ambiguous;
			}
		}
		
		public IBinding[] Bindings
		{
			get
			{
				return _bindings;
			}
		}
		
		public override string ToString()
		{
			return "";
		}
	}
	
	public class ExternalFieldBinding : ITypedBinding
	{
		BindingManager _bindingManager;
		
		System.Reflection.FieldInfo _field;
		
		public ExternalFieldBinding(BindingManager bindingManager, System.Reflection.FieldInfo field)
		{
			_bindingManager = bindingManager;
			_field = field;
		}
		
		public BindingType BindingType
		{
			get
			{
				return BindingType.Field;
			}
		}
		
		public ITypeBinding BoundType
		{
			get
			{
				return _bindingManager.ToTypeBinding(_field.FieldType);
			}
		}
		
		public System.Type Type
		{
			get
			{
				return _field.FieldType;
			}
		}
		
		System.Reflection.FieldInfo FieldInfo
		{
			get
			{
				return _field;
			}
		}
	}
	
	public class ExternalPropertyBinding : IPropertyBinding
	{
		BindingManager _bindingManager;
		
		System.Reflection.PropertyInfo _property;
		
		public ExternalPropertyBinding(BindingManager bindingManager, System.Reflection.PropertyInfo property)
		{
			_bindingManager = bindingManager;
			_property = property;
		}
		
		public BindingType BindingType
		{
			get
			{
				return BindingType.Property;
			}
		}
		
		public ITypeBinding BoundType
		{
			get
			{
				return _bindingManager.ToTypeBinding(_property.PropertyType);
			}
		}
		
		public System.Type Type
		{
			get
			{
				return _property.PropertyType;
			}
		}
		
		public System.Reflection.PropertyInfo PropertyInfo
		{
			get
			{
				return _property;
			}
		}
	}
	
	public class NamespaceBinding : IBinding, INameSpace
	{
		public class AssemblyInfo
		{
			public System.Reflection.Assembly Assembly;
			
			public Type[] Types;
			
			public AssemblyInfo(System.Reflection.Assembly assembly, Type[] types)
			{
				Assembly = assembly;
				Types = types;
			}
			
			public override int GetHashCode()
			{
				return Assembly.GetHashCode();
			}
			
			public override bool Equals(object other)
			{
				return ((AssemblyInfo)other).Assembly == Assembly;
			}
		}
		
		BindingManager _bindingManager;
		
		Using _using;
		
		AssemblyInfo[] _assemblies;		
		
		public NamespaceBinding(BindingManager bindingManager, Using node, AssemblyInfo[] assemblies)
		{			
			_bindingManager = bindingManager;
			_using = node;
			_assemblies = assemblies;
		}
		
		public BindingType BindingType
		{
			get
			{
				return BindingType.Namespace;
			}
		}
		
		public INameSpace Parent
		{
			get
			{
				return null;
			}
			set
			{
				throw new NotImplementedException();
			}
		}
		
		public IBinding Resolve(string name)
		{
			foreach (AssemblyInfo info in _assemblies)
			{
				foreach (Type type in info.Types)
				{
					if (name == type.Name)
					{
						return _bindingManager.ToTypeBinding(type);
					}
				}
			}
			return null;
		}
	}
	
	public class AssemblyBinding : IBinding
	{
		System.Reflection.Assembly _assembly;
		
		public AssemblyBinding(System.Reflection.Assembly assembly)
		{
			if (null == assembly)
			{
				throw new ArgumentNullException("assembly");
			}
			_assembly = assembly;
		}
		
		public BindingType BindingType
		{
			get
			{
				return BindingType.Assembly;
			}
		}
		
		public System.Reflection.Assembly Assembly
		{
			get
			{
				return _assembly;
			}
		}
	}
	
	public class LocalBinding : ITypedBinding
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
	
	public class ParameterBinding : ITypedBinding
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
	
	public class ErrorBinding : ITypeBinding, INameSpace
	{
		public static ErrorBinding Default = new ErrorBinding();
		
		private ErrorBinding()
		{			
		}
		
		public BindingType BindingType
		{
			get
			{
				return BindingType.Error;
			}
		}
		
		public ITypeBinding BoundType
		{
			get
			{
				return this;
			}
		}
		
		public Type Type
		{
			get
			{
				return BindingManager.VoidType;
			}
		}
		
		public IConstructorBinding[] GetConstructors()
		{
			return new IConstructorBinding[0];
		}
		
		public IBinding Resolve(string name)
		{
			return null;
		}
	}
}
