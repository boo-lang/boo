#region license
// Copyright (c) 2004, Rodrigo B. de Oliveira (rbo@acm.org)
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

namespace Boo.Lang.Compiler.Steps
{	
	using System.Text;
	using Boo.Lang.Compiler;
	using Boo.Lang.Compiler.Ast;
	
	public class PreErrorChecking : AbstractVisitorCompilerStep
	{
		static string[] InvalidMemberPrefixes = new string[] {
														"___",
														"get_",
														"set_",
														"add_",
														"remove_",
														"raise_" };
		
		override public void Run()
		{
			Visit(CompileUnit);
		}
		
		override public void LeaveField(Field node)
		{
			MakeStaticIfNeeded(node);
			CheckMemberName(node);
			CantBeMarkedAbstract(node);
			CantBeMarkedPartial(node);
		}
		
		override public void LeaveProperty(Property node)
		{
			MakeStaticIfNeeded(node);
			CheckMemberName(node);
			CantBeMarkedTransient(node);
			CantBeMarkedPartial(node);
			CheckExplicitImpl(node);
			CheckModifierCombination(node);
		}

		override public void LeaveConstructor(Constructor node)
		{
			MakeStaticIfNeeded(node);
			CantBeMarkedTransient(node);
			CantBeMarkedPartial(node);
			CantBeMarkedFinal(node);
			CannotReturnValue(node);
			ConstructorCannotBePolymorphic(node);
		}
		
		override public void LeaveMethod(Method node)
		{
			MakeStaticIfNeeded(node);
			CheckMemberName(node);
			CantBeMarkedTransient(node);
			CantBeMarkedPartial(node);
			CheckExplicitImpl(node);
			CheckModifierCombination(node);
		}
		
		override public void LeaveEvent(Event node)
		{
			MakeStaticIfNeeded(node);
			CheckMemberName(node);
			CantBeMarkedPartial(node);
			CheckModifierCombination(node);
		}
		
		override public void LeaveInterfaceDefinition(InterfaceDefinition node)
		{
			CheckMemberName(node);
			CantBeMarkedAbstract(node);
			CantBeMarkedTransient(node);
			CantBeMarkedPartial(node);
			CantBeMarkedFinal(node);
			CantBeMarkedStatic(node);
		}
		
		override public void LeaveCallableDefinition(CallableDefinition node)
		{
			MakeStaticIfNeeded(node);
			CheckMemberName(node);
			CantBeMarkedAbstract(node);
			CantBeMarkedTransient(node);
			CantBeMarkedPartial(node);
		}
		
		public override void LeaveStructDefinition(StructDefinition node)
		{
			CheckMemberName(node);
			CantBeMarkedAbstract(node);
			CantBeMarkedFinal(node);
			CantBeMarkedStatic(node);
			CantBeMarkedPartial(node);
		}

		public override void LeaveEnumDefinition(EnumDefinition node)
		{
			CheckMemberName(node);
			CantBeMarkedAbstract(node);
			CantBeMarkedFinal(node);
			CantBeMarkedStatic(node);
			CantBeMarkedPartial(node);
		}

		override public void LeaveClassDefinition(ClassDefinition node)
		{
			CheckModifierCombination(node);
			CheckMemberName(node);
			
			if(node.IsStatic)
			{
				node.Modifiers |= TypeMemberModifiers.Abstract | TypeMemberModifiers.Final;
			}
		}
		
		override public void LeaveTryStatement(TryStatement node)
		{
			if (node.EnsureBlock == null && node.FailureBlock == null && node.ExceptionHandlers.Count == 0)
			{
				Error(CompilerErrorFactory.InvalidTryStatement(node));
			}
		}
		
		override public void LeaveBinaryExpression(BinaryExpression node)
		{
			if (BinaryOperatorType.Assign == node.Operator
				&& (node.Right.NodeType != NodeType.TryCastExpression)
				&& (IsTopLevelOfConditional(node)))
			{
				Warnings.Add(CompilerWarningFactory.EqualsInsteadOfAssign(node));
			}
		}
		
		bool IsTopLevelOfConditional(Node child)
		{
			Node parent = child.ParentNode;
			return (parent.NodeType == NodeType.IfStatement
				|| parent.NodeType == NodeType.UnlessStatement
				|| parent.NodeType == NodeType.ConditionalExpression
				|| parent.NodeType == NodeType.StatementModifier
				|| parent.NodeType == NodeType.ReturnStatement
				|| parent.NodeType == NodeType.YieldStatement);
		}

		override public void LeaveDestructor(Destructor node)
		{
			if (node.Modifiers != TypeMemberModifiers.None)
			{
				Error(CompilerErrorFactory.InvalidDestructorModifier(node));
			}

			if (node.Parameters.Count != 0)
			{
				Error(CompilerErrorFactory.CantHaveDestructorParameters(node));
			}

			CannotReturnValue(node);
		}
		
		void ConstructorCannotBePolymorphic(Constructor node)
		{
			if(node.IsAbstract || node.IsOverride || node.IsVirtual)
			{
				Error(CompilerErrorFactory.ConstructorCantBePolymorphic(node, node.FullName));
			}
		}

		private void CannotReturnValue(Method node)
		{
			if (node.ReturnType != null)
			{
				Error(CompilerErrorFactory.CannotReturnValue(node));
			}
		}

		void CantBeMarkedAbstract(TypeMember member)
		{
			if (member.IsAbstract)
			{
				Error(CompilerErrorFactory.CantBeMarkedAbstract(member));
			}
		}
		
		void CantBeMarkedFinal(TypeMember member)
		{
			if (member.IsFinal)
			{
				Error(CompilerErrorFactory.CantBeMarkedFinal(member));
			}
		}

		void CantBeMarkedTransient(TypeMember member)
		{
			if (member.IsTransient)
			{
				Error(CompilerErrorFactory.CantBeMarkedTransient(member));
			}
		}
		
		void CheckMemberName(TypeMember node)
		{
			foreach (string prefix in InvalidMemberPrefixes)
			{
				if (node.Name.StartsWith(prefix))
				{
					Error(CompilerErrorFactory.ReservedPrefix(node, prefix));
					break;
				}
			}
		}
		
		void MakeStaticIfNeeded(TypeMember node)
		{
			if(node.DeclaringType.IsStatic)
			{
				if(node.IsStatic)
				{
					Warnings.Add(CompilerWarningFactory.StaticClassMemberRedundantlyMarkedStatic(node, node.DeclaringType.Name, node.Name));
				}
				
				node.Modifiers |= TypeMemberModifiers.Static;
			}
		}
				
		void CheckExplicitImpl(IExplicitMember member)
		{
			ExplicitMemberInfo ei = member.ExplicitInfo;
			if (null == ei)
			{
				return;
			}
			
			TypeMember node = (TypeMember)member;
			if (TypeMemberModifiers.None != node.Modifiers)
			{
				Error(
					CompilerErrorFactory.ExplicitImplMustNotHaveModifiers(
						node,
						ei.InterfaceType.Name,
						node.Name));
			}
		}
		
		void CheckModifierCombination(TypeMember member)
		{
			InvalidCombination(member, TypeMemberModifiers.Static, TypeMemberModifiers.Abstract);
			InvalidCombination(member, TypeMemberModifiers.Static, TypeMemberModifiers.Virtual);
			InvalidCombination(member, TypeMemberModifiers.Static, TypeMemberModifiers.Override);
			InvalidCombination(member, TypeMemberModifiers.Abstract, TypeMemberModifiers.Final);
			
			if (member.NodeType != NodeType.Field)
			{
				InvalidCombination(member, TypeMemberModifiers.Static, TypeMemberModifiers.Final);
			}
		}
		
		void InvalidCombination(TypeMember member, TypeMemberModifiers mod1, TypeMemberModifiers mod2)
		{
			if (!member.IsModifierSet(mod1) || !member.IsModifierSet(mod2)) return;
			Error(
				CompilerErrorFactory.InvalidCombinationOfModifiers(
					member,
					member.FullName,
					string.Format("{0}, {1}", mod1.ToString().ToLower(), mod2.ToString().ToLower())));
		}
		
		
		void CantBeMarkedPartial(TypeMember member)
		{
			if (member.IsPartial)
			{
				Error(CompilerErrorFactory.CantBeMarkedPartial(member));
			}
		}
		
		void CantBeMarkedStatic(TypeMember member)
		{
			if (member.IsStatic)
			{
				Error(CompilerErrorFactory.CantBeMarkedStatic(member));
			}
		}
	}
}
