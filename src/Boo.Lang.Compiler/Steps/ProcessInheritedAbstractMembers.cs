namespace Boo.Lang.Compiler.Steps
{
	using System.Diagnostics;
	using Boo.Lang.Compiler.Ast;
	using Boo.Lang.Compiler.TypeSystem;

	public class ProcessInheritedAbstractMembers : AbstractVisitorCompilerStep
	{
		private Boo.Lang.List _newAbstractClasses;

		public ProcessInheritedAbstractMembers()
		{
		}

		override public void Run()
		{	
			_newAbstractClasses = new List();
			Visit(CompileUnit.Modules);
			ProcessNewAbstractClasses();
		}

		override public void Dispose()
		{
			_newAbstractClasses = null;
			base.Dispose();
		}

		override public void OnProperty(Property node)
		{
			if (node.IsAbstract)
			{
				if (null == node.Type)
				{
					node.Type = CodeBuilder.CreateTypeReference(TypeSystemServices.ObjectType);
				}
			}
		}

		override public void OnMethod(Method node)
		{
			if (node.IsAbstract)
			{
				if (null == node.ReturnType)
				{
					node.ReturnType = CodeBuilder.CreateTypeReference(TypeSystemServices.VoidType);
				}
			}
		}

		override public void LeaveClassDefinition(ClassDefinition node)
		{
			MarkVisited(node);
			foreach (TypeReference baseTypeRef in node.BaseTypes)
			{	
				IType baseType = GetType(baseTypeRef);
				EnsureRelatedNodeWasVisited(baseType);
				if (baseType.IsInterface)
				{
					ResolveInterfaceMembers(node, baseTypeRef, baseType);
				}
				else
				{
					if (IsAbstract(baseType))
					{
						ResolveAbstractMembers(node, baseTypeRef, baseType);
					}
				}
			}
		}

		private void MarkVisited(ClassDefinition node)
		{
			node[this] = true;
		}

		private void EnsureRelatedNodeWasVisited(IType type)
		{
			AbstractInternalType internalType = type as AbstractInternalType;
			if (null != internalType)
			{
				TypeDefinition node = internalType.TypeDefinition;
				if (!node.ContainsAnnotation(this))
				{
					Visit(node);
				}
			}
		}

		bool IsAbstract(IType type)
		{
			if (type.IsAbstract)
			{
				return true;
			}
			
			AbstractInternalType internalType = type as AbstractInternalType;
			if (null != internalType)
			{
				return _newAbstractClasses.Contains(internalType.TypeDefinition);
			}
			return false;
		}

		IProperty GetPropertyEntity(TypeMember member)
		{
			return (IProperty)GetEntity(member);
		}
		
		void ResolveClassAbstractProperty(ClassDefinition node,
			TypeReference baseTypeRef,
			IProperty entity)
		{
			TypeMember member = node.Members[entity.Name];
			if (null != member && NodeType.Property == member.NodeType &&
				TypeSystemServices.CheckOverrideSignature(entity.GetParameters(), GetPropertyEntity(member).GetParameters()))
			{
				Property p = (Property)member;
				if (null != p.Getter)
				{
					p.Getter.Modifiers |= TypeMemberModifiers.Virtual;
				}
				if (null != p.Setter)
				{
					p.Setter.Modifiers |= TypeMemberModifiers.Virtual;
				}
			}
			else
			{
				if (null == member)
				{
					node.Members.Add(CreateAbstractProperty(baseTypeRef, entity));
					AbstractMemberNotImplemented(node, baseTypeRef, entity);
				}
				else
				{
					NotImplemented(baseTypeRef, "member name conflict: " + entity);
				}
			}
		}
		
		Property CreateAbstractProperty(TypeReference reference, IProperty property)
		{
			Debug.Assert(0 == property.GetParameters().Length);
			Property p = CodeBuilder.CreateProperty(property.Name, property.Type);
			p.Modifiers |= TypeMemberModifiers.Abstract;
			
			IMethod getter = property.GetGetMethod();
			if (getter != null)
			{
				p.Getter = CodeBuilder.CreateAbstractMethod(reference.LexicalInfo, getter);
			}
			
			IMethod setter = property.GetSetMethod();
			if (setter != null)
			{
				p.Setter = CodeBuilder.CreateAbstractMethod(reference.LexicalInfo, setter);
			}
			return p;
		}
		
		void ResolveAbstractMethod(ClassDefinition node,
			TypeReference baseTypeRef,
			IMethod entity)
		{
			if (entity.IsSpecialName)
			{
				return;
			}
			
			foreach (TypeMember member in node.Members)
			{
				if (
					entity.Name == member.Name &&
					NodeType.Method == member.NodeType &&
					(
					((Method)member).ExplicitInfo == null ||
					GetType(baseTypeRef) == GetType(((Method)member).ExplicitInfo.InterfaceType) ||
					GetType(baseTypeRef).IsSubclassOf(GetType(((Method)member).ExplicitInfo.InterfaceType))
					)
					)
				{
					Method method = (Method)member;
					if (TypeSystemServices.CheckOverrideSignature((IMethod)GetEntity(method), entity))
					{
						// TODO: check return type here
						if (!method.IsOverride && !method.IsVirtual)
						{
							method.Modifiers |= TypeMemberModifiers.Virtual;
						}
						
						_context.TraceInfo("{0}: Method {1} implements {2}", method.LexicalInfo, method, entity);
						return;
					}
				}
			}
			
			node.Members.Add(CodeBuilder.CreateAbstractMethod(baseTypeRef.LexicalInfo, entity));
			AbstractMemberNotImplemented(node, baseTypeRef, entity);
		}
		
		void AbstractMemberNotImplemented(ClassDefinition node, TypeReference baseTypeRef, IMember member)
		{
			if (!node.IsAbstract)
			{
				Warnings.Add(
					CompilerWarningFactory.AbstractMemberNotImplemented(baseTypeRef,
					node.FullName, member.FullName));
				_newAbstractClasses.AddUnique(node);
			}
		}
		
		void ResolveInterfaceMembers(ClassDefinition node,
			TypeReference baseTypeRef,
			IType baseType)
		{
			foreach (IType tag in baseType.GetInterfaces())
			{
				ResolveInterfaceMembers(node, baseTypeRef, tag);
			}
			
			foreach (IMember tag in baseType.GetMembers())
			{
				ResolveAbstractMember(node, baseTypeRef, tag);
			}
		}
		
		void ResolveAbstractMembers(ClassDefinition node,
			TypeReference baseTypeRef,
			IType baseType)
		{
			foreach (IEntity member in baseType.GetMembers())
			{
				switch (member.EntityType)
				{
					case EntityType.Method:
					{
						IMethod method = (IMethod)member;
						if (method.IsAbstract)
						{
							ResolveAbstractMethod(node, baseTypeRef, method);
						}
						break;
					}
					
					case EntityType.Property:
					{
						IProperty property = (IProperty)member;
						if (IsAbstractAccessor(property.GetGetMethod()) ||
							IsAbstractAccessor(property.GetSetMethod()))
						{
							ResolveClassAbstractProperty(node, baseTypeRef, property);
						}
						break;
					}
				}
			}
		}
		
		bool IsAbstractAccessor(IMethod accessor)
		{
			if (null != accessor)
			{
				return accessor.IsAbstract;
			}
			return false;
		}
		
		void ResolveAbstractMember(ClassDefinition node,
			TypeReference baseTypeRef,
			IMember member)
		{
			switch (member.EntityType)
			{
				case EntityType.Method:
				{
					ResolveAbstractMethod(node, baseTypeRef, (IMethod)member);
					break;
				}
				
				case EntityType.Property:
				{
					ResolveClassAbstractProperty(node, baseTypeRef, (IProperty)member);
					break;
				}
				
				default:
				{
					NotImplemented(baseTypeRef, "abstract member: " + member);
					break;
				}
			}
		}
		
		void ProcessNewAbstractClasses()
		{
			foreach (ClassDefinition node in _newAbstractClasses)
			{
				node.Modifiers |= TypeMemberModifiers.Abstract;
			}
		}
	}
}
