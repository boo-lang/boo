#region license
// Copyright (c) 2003, 2004, 2005 Rodrigo B. de Oliveira (rbo@acm.org)
// All rights reserved.
// 
// Redistribution and use in source and binary forms, with or without modification,
// are permitted provided that the following conditions are met:
// 
//     * Redistributions of source code must retain the above copyright notice,
//     this list of conditions and the following disclaimer.
//     * Redistributions in binary form must reproduce the above copyright notice,
//     this list of conditions and the following disclaimer in the documentation
//     and/or other materials provided with the distribution.
//     * Neither the name of Rodrigo B. de Oliveira nor the names of its
//     contributors may be used to endorse or promote products derived from this
//     software without specific prior written permission.
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
// ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
// DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE
// FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
// DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
// SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER
// CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
// OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF
// THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
#endregion

using Boo.Lang.Compiler.Ast;
using Boo.Lang.Compiler.TypeSystem;
using Boo.Lang.Compiler.TypeSystem.Services;
using Boo.Lang.Compiler.Util;
using Boo.Lang.Runtime;

namespace Boo.Lang.Compiler.Steps
{
	public class NormalizeTypeAndMemberDefinitions : AbstractVisitorCompilerStep, ITypeMemberReifier
	{
		override public void OnModule(Module node)
		{
			Visit(node.Members);
		}
		
		void LeaveTypeDefinition(TypeDefinition node)
		{
			if (!node.IsVisibilitySet)
				node.Modifiers |= Context.Parameters.DefaultTypeVisibility;

			node.Name = NormalizeName(node.Name);
		}

		public override void LeaveExplicitMemberInfo(ExplicitMemberInfo node)
		{
			var member = (TypeMember) node.ParentNode;
			member.Modifiers |= TypeMemberModifiers.Private | TypeMemberModifiers.Virtual;
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
			if (!node.HasInstanceConstructor && !node.IsStatic)
				node.Members.Add(AstUtil.CreateDefaultConstructor(node));
		}

		override public void LeaveStructDefinition(StructDefinition node)
		{
			LeaveTypeDefinition(node);
		}

		override public void LeaveField(Field node)
		{
			if (!node.IsVisibilitySet)
			{
				node.Visibility = Context.Parameters.DefaultFieldVisibility;

				//protected field in a sealed type == private,
				//so let the compiler mark them private automatically in order to get 
				//unused members warnings for free (and to make IL analysis tools happy as a bonus)
				if (node.IsProtected && node.DeclaringType.IsFinal)
					node.Visibility = TypeMemberModifiers.Private;
			}

			LeaveMember(node);
		}

		override public void LeaveProperty(Property node)
		{
			NormalizeDefaultItemProperty(node);
			NormalizePropertyModifiers(node);

			LeaveMember(node);
		}

		private void NormalizeDefaultItemProperty(Property node)
		{
			if (!IsDefaultItemProperty(node)) return;

			node.Name = "Item";
			TypeDefinition declaringType = node.DeclaringType;
			if (declaringType != null) AddDefaultMemberAttribute(declaringType, node);
		}

		private void AddDefaultMemberAttribute(TypeDefinition type, Property node)
		{
			if (!ContainsDefaultMemberAttribute(type))
			{
				Attribute attribute = CodeBuilder.CreateAttribute(
					DefaultMemberAttributeStringConstructor(), 
					new StringLiteralExpression(node.Name));
				attribute.LexicalInfo = node.LexicalInfo;
				type.Attributes.Add(attribute);
			}
		}

		private IConstructor DefaultMemberAttributeStringConstructor()
		{
			return TypeSystemServices.Map(Methods.ConstructorOf(() => new System.Reflection.DefaultMemberAttribute(default(string))));
		}

		private static bool ContainsDefaultMemberAttribute(TypeDefinition t)
		{
			foreach (Attribute a in t.Attributes)
				if (a.Name.IndexOf("DefaultMember") >= 0)
					return true;
			return false;
		}

		private static bool IsDefaultItemProperty(Property node)
		{
			return (node.Name == "Item" || node.Name == "self")
				&& node.Parameters.Count > 0
				&& !node.IsStatic;
		}

		private void NormalizePropertyModifiers(Property node)
		{
			if (IsInterfaceMember(node))
				node.Modifiers = TypeMemberModifiers.Public | TypeMemberModifiers.Abstract;
			else if (!node.IsVisibilitySet && null == node.ExplicitInfo)
				node.Modifiers |= Context.Parameters.DefaultPropertyVisibility;

			if (null != node.Getter)
			{
				SetPropertyAccessorModifiers(node, node.Getter);
				node.Getter.Name = "get_" + node.Name;
			}
			if (null != node.Setter)
			{
				SetPropertyAccessorModifiers(node, node.Setter);
				node.Setter.Name = "set_" + node.Name;
			}
		}

		private static bool IsInterfaceMember(TypeMember node)
		{
			var declaringType = node.DeclaringType;
			if (null == declaringType)
				throw CompilerErrorFactory.NotImplemented(node, string.Format("{0} '{1}' is not attached to any type. It should probably have been consumed by a macro but it hasn't.", node.GetType().Name, node.Name));

			return NodeType.InterfaceDefinition == declaringType.NodeType;
		}

		void SetPropertyAccessorModifiers(Property property, Method accessor)
		{
			if (!accessor.IsVisibilitySet)
				accessor.Modifiers |= property.Visibility;
			
			if (property.IsStatic)
				accessor.Modifiers |= TypeMemberModifiers.Static;
			
			if (property.IsVirtual)
				accessor.Modifiers |= TypeMemberModifiers.Virtual;
			
			if (property.IsAbstract)
				accessor.Modifiers |= TypeMemberModifiers.Abstract;
			else if (accessor.IsAbstract)
				// an abstract accessor makes the entire property abstract
				property.Modifiers |= TypeMemberModifiers.Abstract;
		}
		
		override public void LeaveEvent(Event node)
		{
			if (IsInterfaceMember(node))
				node.Modifiers = TypeMemberModifiers.Public | TypeMemberModifiers.Abstract;
			else if (!node.IsVisibilitySet)
				node.Modifiers |= Context.Parameters.DefaultEventVisibility;

			LeaveMember(node);
		}
		
		override public void LeaveMethod(Method node)
		{
			if (IsInterfaceMember(node))
				node.Modifiers = TypeMemberModifiers.Public | TypeMemberModifiers.Abstract;
			else if (!node.IsVisibilitySet && null == node.ExplicitInfo && node.ParentNode.NodeType != NodeType.Property)
				node.Modifiers |= Context.Parameters.DefaultMethodVisibility;
			if (node.Name != null && node.Name.StartsWith("op_"))
				node.Modifiers |= TypeMemberModifiers.Static;

			LeaveMember(node);
		}

		override public void OnDestructor(Destructor node)
		{
			Method finalizer = CodeBuilder.CreateMethod(
				"Finalize",
				TypeSystemServices.VoidType,
				TypeMemberModifiers.Protected | TypeMemberModifiers.Override);
			finalizer.LexicalInfo = node.LexicalInfo;

			MethodInvocationExpression mie = new MethodInvocationExpression(new SuperLiteralExpression());

			Block bodyNew = new Block();
			Block ensureBlock = new Block();
			ensureBlock.Add (mie);

			TryStatement tryStatement = new TryStatement();
			tryStatement.EnsureBlock = ensureBlock;
			tryStatement.ProtectedBlock = node.Body;

			bodyNew.Add(tryStatement);
			finalizer.Body = bodyNew;

			node.ParentNode.Replace(node, finalizer);
		}

		void LeaveMember(TypeMember node)
		{
			if (node.IsAbstract && !IsInterfaceMember(node))
				node.DeclaringType.Modifiers |= TypeMemberModifiers.Abstract;

			node.Name = NormalizeName(node.Name);
		}

		override public void LeaveConstructor(Constructor node)
		{
			if (node.IsVisibilitySet) return;

			if (!node.IsStatic)
				node.Modifiers |= Context.Parameters.DefaultMethodVisibility;
			else
				node.Modifiers |= TypeMemberModifiers.Private;
		}

		public TypeMember Reify(TypeMember member)
		{
			Visit(member);
			return member;
		}

		override public void LeaveMemberReferenceExpression(MemberReferenceExpression node)
		{
			node.Name = NormalizeName(node.Name);
		}

		override public void OnReferenceExpression(ReferenceExpression node)
		{
			node.Name = NormalizeName(node.Name);
		}

		protected string NormalizeName(string name)
		{
			if (name != null && name.Length > 1 && name.StartsWith("@"))
				name = name.Substring(1, name.Length-1);
			return name;
		}

	}
}
