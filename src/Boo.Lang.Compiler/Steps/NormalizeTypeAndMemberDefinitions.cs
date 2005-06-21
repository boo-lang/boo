namespace Boo.Lang.Compiler.Steps
{
	using Boo.Lang.Compiler.Ast;

	public class NormalizeTypeAndMemberDefinitions : AbstractVisitorCompilerStep
	{
		override public void Run()
		{
			Visit(CompileUnit.Modules);
		}

		override public void OnModule(Module node)
		{
			Visit(node.Members);
		}
		
		void LeaveTypeDefinition(TypeDefinition node)
		{
			if (!node.IsVisibilitySet)
			{
				node.Modifiers |= TypeMemberModifiers.Public;
			}
		}
		
		override public void LeaveEnumDefinition(EnumDefinition node)
		{
			LeaveTypeDefinition(node);
		}
		
		override public void LeaveInterfaceDefinition(InterfaceDefinition node)
		{
			LeaveTypeDefinition(node);
		}
		
		override public void LeaveClassDefinition(ClassDefinition node)
		{
			LeaveTypeDefinition(node);
			if (!node.HasInstanceConstructor)
			{
				node.Members.Add(AstUtil.CreateConstructor(node, TypeMemberModifiers.Public));
			}
		}
		
		override public void LeaveField(Field node)
		{
			if (!node.IsVisibilitySet)
			{
				node.Modifiers |= TypeMemberModifiers.Protected;
			}
		}
		
		override public void LeaveProperty(Property node)
		{
			if (!node.IsVisibilitySet)
			{
				node.Modifiers |= TypeMemberModifiers.Public;
			}
			if (IsInterface(node.DeclaringType))
			{
				node.Modifiers |= TypeMemberModifiers.Abstract;
			}

			SetPropertyAccessorModifiers(node, node.Getter);
			SetPropertyAccessorModifiers(node, node.Setter);
		}

		void SetPropertyAccessorModifiers(Property property, Method accessor)
		{
			if (null == accessor)
			{
				return;
			}

			if (property.IsStatic)
			{
				accessor.Modifiers |= TypeMemberModifiers.Static;
			}
			
			if (property.IsVirtual)
			{
				accessor.Modifiers |= TypeMemberModifiers.Virtual;
			}
			
			/*
			if (property.IsOverride)
			{
				accessor.Modifiers |= TypeMemberModifiers.Override;
			}
			*/
			
			if (property.IsAbstract)
			{
				accessor.Modifiers |= TypeMemberModifiers.Abstract;
			}
			else if (accessor.IsAbstract)
			{
				// an abstract accessor makes the entire property abstract
				property.Modifiers |= TypeMemberModifiers.Abstract;
			}
		}
		
		override public void LeaveEvent(Event node)
		{
			if (!node.IsVisibilitySet)
			{
				node.Modifiers |= TypeMemberModifiers.Public;
			}
			if (IsInterface(node.DeclaringType))
			{
				node.Modifiers |= TypeMemberModifiers.Abstract;
			}
		}
		
		override public void LeaveMethod(Method node)
		{
			if (!node.IsVisibilitySet)
			{
				node.Modifiers |= TypeMemberModifiers.Public;
			}
			if (IsInterface(node.DeclaringType))
			{
				node.Modifiers |= TypeMemberModifiers.Abstract;
			}
		}

		bool IsInterface(TypeDefinition node)
		{
			return NodeType.InterfaceDefinition == node.NodeType;
		}
		
		override public void LeaveConstructor(Constructor node)
		{
			if (!node.IsVisibilitySet)
			{
				node.Modifiers |= TypeMemberModifiers.Public;
			}
		}

	}
}
