namespace Boo.Lang.Compiler.Bindings
{
	using System;
	
	public class TupleTypeBinding : ITypeBinding, INamespace
	{	
		BindingManager _bindingManager;
		
		ITypeBinding _elementType;
		
		ITypeBinding _array;
		
		public TupleTypeBinding(BindingManager bindingManager, ITypeBinding elementType)
		{
			_bindingManager = bindingManager;
			_array = bindingManager.ArrayTypeBinding;
			_elementType = elementType;
		}
		
		public string Name
		{
			get
			{
				return string.Format("({0})", _elementType.FullName);
			}
		}
		
		public BindingType BindingType
		{
			get			
			{
				return BindingType.Tuple;
			}
		}
		
		public string FullName
		{
			get
			{
				return Name;
			}
		}
		
		public virtual ITypeBinding BoundType
		{
			get
			{
				return this;
			}
		}
		
		public virtual bool IsClass
		{
			get
			{
				return false;
			}
		}
		
		public bool IsValueType
		{
			get
			{
				return false;
			}
		}
		
		public bool IsArray
		{
			get
			{
				return true;
			}
		}
		
		public ITypeBinding GetElementType()
		{
			return _elementType;
		}
		
		public ITypeBinding BaseType
		{
			get
			{
				return _array;
			}
		}
		
		public virtual bool IsSubclassOf(ITypeBinding other)
		{
			return other == _array;
		}
		
		public virtual bool IsAssignableFrom(ITypeBinding other)
		{			
			if (other == this)
			{
				return true;
			}
			
			if (other.IsArray)
			{
				ITypeBinding otherElementType = other.GetElementType();
				if (_elementType.IsValueType || otherElementType.IsValueType)
				{
					return _elementType == otherElementType;
				}
				return _elementType.IsAssignableFrom(otherElementType);
			}
			return false;
		}
		
		public IConstructorBinding[] GetConstructors()
		{
			return new IConstructorBinding[0];
		}
		
		public IBinding Resolve(string name)
		{
			return _array.Resolve(name);
		}
		
		public override string ToString()
		{
			return Name;
		}
	}
}
