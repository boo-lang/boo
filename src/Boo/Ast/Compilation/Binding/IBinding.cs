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
	
	public interface ITypeBinding : ITypedBinding, INameSpace
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
	
	public class TypeReferenceBinding : ITypedBinding, INameSpace
	{
		ITypeBinding _type;
		
		public TypeReferenceBinding(ITypeBinding type)
		{
			_type = type;
		}
		
		public string Name
		{
			get
			{
				return _type.Name;
			}
		}
		
		public BindingType BindingType
		{
			get
			{
				return BindingType.TypeReference;
			}
		}
		
		public ITypeBinding BoundType
		{
			get
			{
				return _type;
			}
		}
		
		public IBinding Resolve(string name)
		{
			return  _type.Resolve(name);
		}
		
		public override string ToString()
		{
			return _type.ToString();
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
		
		public string Name
		{
			get
			{
				return _bindings[0].Name;
			}
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
	
	public class ExternalEventBinding : IMemberBinding, ITypedBinding
	{
		BindingManager _bindingManager;
		
		System.Reflection.EventInfo _event;
		
		public ExternalEventBinding(BindingManager bindingManager, System.Reflection.EventInfo event_)
		{
			_bindingManager = bindingManager;
			_event = event_;
		}
		
		public System.Reflection.EventInfo EventInfo
		{
			get
			{
				return _event;
			}
		}
		
		public string Name
		{
			get
			{
				return _event.Name;
			}
		}
		
		public BindingType BindingType
		{
			get
			{
				return BindingType.Event;
			}
		}
		
		public ITypeBinding BoundType
		{
			get
			{
				return _bindingManager.ToTypeBinding(_event.EventHandlerType);
			}
		}
		
		public bool IsStatic
		{
			get
			{
				return false;
			}
		}
	}
	
	public class ExternalFieldBinding : IFieldBinding
	{
		BindingManager _bindingManager;
		
		System.Reflection.FieldInfo _field;
		
		public ExternalFieldBinding(BindingManager bindingManager, System.Reflection.FieldInfo field)
		{
			_bindingManager = bindingManager;
			_field = field;
		}
		
		public string Name
		{
			get
			{
				return _field.Name;
			}
		}
		
		public bool IsStatic
		{
			get
			{
				return _field.IsStatic;
			}
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
		
		public System.Reflection.FieldInfo FieldInfo
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
		
		public bool IsStatic
		{
			get
			{
				System.Reflection.MethodInfo mi = _property.GetGetMethod();
				if (null != mi)
				{
					return mi.IsStatic;
				}
				return _property.GetSetMethod().IsStatic;
			}
		}
		
		public string Name
		{
			get
			{
				return _property.Name;
			}
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
		
		public string Name
		{
			get
			{
				return _assembly.FullName;
			}
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
		
		public string Name
		{
			get
			{
				return _local.Name;
			}
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
		
		public string Name
		{
			get
			{
				return _parameter.Name;
			}
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
		
		public string Name
		{
			get
			{
				return "Error";
			}
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
				return Types.Void;
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
