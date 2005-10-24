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
			CheckMemberName(node);
			CantBeMarkedAbstract(node);
		}
		
		override public void LeaveProperty(Property node)
		{
			CheckMemberName(node);
			CantBeMarkedTransient(node);
			CheckExplicitImpl(node);
		}

		override public void LeaveConstructor(Constructor node)
		{
			CheckExtensionSemantics(node);
		}
		
		override public void LeaveMethod(Method node)
		{
			CheckMemberName(node);
			CantBeMarkedTransient(node);
			CheckExplicitImpl(node);
			CheckExtensionSemantics(node);
		}
		
		override public void LeaveEvent(Event node)
		{
			CheckMemberName(node);
		}
		
		override public void LeaveInterfaceDefinition(InterfaceDefinition node)
		{
			CheckMemberName(node);
			CantBeMarkedAbstract(node);
			CantBeMarkedTransient(node);
		}
		
		override public void LeaveCallableDefinition(CallableDefinition node)
		{
			CheckMemberName(node);
			CantBeMarkedAbstract(node);
			CantBeMarkedTransient(node);
		}
		
		override public void LeaveClassDefinition(ClassDefinition node)
		{
			CheckMemberName(node);
		}
		
		override public void LeaveGivenStatement(GivenStatement node)
		{
			NotImplemented(node, "given");
		}

		override public void LeaveTryStatement(TryStatement node)
		{
			if (node.EnsureBlock == null && node.ExceptionHandlers.Count == 0)
			{
				Error(CompilerErrorFactory.InvalidTryStatement(node));
			}
		}
		
		override public void LeaveBinaryExpression(BinaryExpression node)
		{
			if (BinaryOperatorType.Assign == node.Operator
				&& (node.Right.NodeType != NodeType.AsExpression)
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
				|| parent.NodeType == NodeType.TernaryExpression
				|| parent.NodeType == NodeType.WhenClause
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
		}

		void CheckExtensionSemantics(Method node)
		{
			if (MethodImplementationFlags.Extension != (node.ImplementationFlags & MethodImplementationFlags.Extension)) return;
			if (NodeType.Method == node.NodeType && (node.IsStatic || node.DeclaringType is Module)) return;
			Error(CompilerErrorFactory.InvalidExtensionDefinition(node));
		}
		
		void CantBeMarkedAbstract(TypeMember member)
		{
			if (member.IsAbstract)
			{
				Error(CompilerErrorFactory.CantBeMarkedAbstract(member));
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
	}
}
