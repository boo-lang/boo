namespace Boo.Ast.Compilation.Binding
{
	using System;
	using Boo.Ast;
	
	public class AbstractInternalTypeBinding : INamespace
	{		
		protected BindingManager _bindingManager;
		
		protected TypeDefinition _typeDefinition;
		
		protected AbstractInternalTypeBinding(BindingManager bindingManager, TypeDefinition typeDefinition)
		{
			_bindingManager = bindingManager;
			_typeDefinition = typeDefinition;
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
		
		public override string ToString()
		{
			return string.Format("InternalTypeBinding<TypeDefinition={0}>", _typeDefinition);
		}
	}

}
