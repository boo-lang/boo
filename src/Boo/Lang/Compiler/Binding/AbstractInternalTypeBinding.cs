namespace Boo.Ast.Compilation.Binding
{
	using System;
	using Boo.Ast;
	
	public abstract class AbstractInternalTypeBinding : ITypeBinding, INamespace
	{		
		protected BindingManager _bindingManager;
		
		protected TypeDefinition _typeDefinition;
		
		protected AbstractInternalTypeBinding(BindingManager bindingManager, TypeDefinition typeDefinition)
		{
			_bindingManager = bindingManager;
			_typeDefinition = typeDefinition;
		}
		
		public string FullName
		{
			get
			{
				return _typeDefinition.FullName;
			}
		}
		
		public string Name
		{
			get
			{
				return _typeDefinition.Name;
			}
		}
		
		public virtual IBinding Resolve(string name)
		{			
			foreach (TypeMember member in _typeDefinition.Members)
			{
				if (name == member.Name)
				{					
					IBinding binding = _bindingManager.GetOptionalBinding(member);
					if (null == binding)
					{						
						binding = CreateCorrectBinding(member);
						_bindingManager.Bind(member, binding);
					}	
					
					if (BindingType.Type == binding.BindingType)
					{
						binding = _bindingManager.ToTypeReference((ITypeBinding)binding);
					}
					return binding;
				}
			}
			
			foreach (TypeReference baseType in _typeDefinition.BaseTypes)
			{
				IBinding binding = _bindingManager.GetBoundType(baseType).Resolve(name);
				if (null != binding)
				{
					return binding;
				}
			}
			return null;
		}
		
		IBinding CreateCorrectBinding(TypeMember member)
		{
			switch (member.NodeType)
			{
				case NodeType.Method:
				{
					return new InternalMethodBinding(_bindingManager, (Method)member);
				}
				
				case NodeType.Field:
				{
					return new InternalFieldBinding(_bindingManager, (Field)member);
				}
			}
			throw new NotImplementedException();
		}
		
		public virtual ITypeBinding BaseType
		{
			get
			{
				return null;
			}
		}
		
		public TypeDefinition TypeDefinition
		{
			get
			{
				return _typeDefinition;
			}
		}
		
		public ITypeBinding BoundType
		{
			get
			{
				return this;
			}
		}
		
		public bool IsClass
		{
			get
			{
				return NodeType.ClassDefinition == _typeDefinition.NodeType;
			}
		}
		
		public bool IsValueType
		{
			get
			{
				return false;
			}
		}
		
		public virtual BindingType BindingType
		{
			get
			{
				return BindingType.Type;
			}
		}
		
		public virtual bool IsSubclassOf(ITypeBinding other)
		{
			return false;
		}
		
		public virtual bool IsAssignableFrom(ITypeBinding other)
		{
			return false;
		}
		
		public virtual IConstructorBinding[] GetConstructors()
		{
			return new IConstructorBinding[0];
		}
	}

}
