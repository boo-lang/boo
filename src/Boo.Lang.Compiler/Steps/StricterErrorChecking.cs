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
	using System;
	using System.Collections;
	using Boo.Lang;
	using Boo.Lang.Compiler;
	using Boo.Lang.Compiler.Ast;
	using Boo.Lang.Compiler.TypeSystem;
	
	public class StricterErrorChecking : AbstractVisitorCompilerStep
	{
		Hashtable _members = new Hashtable();
		
		override public void Run()
		{
			Visit(CompileUnit);
		}
		
		override public void Dispose()
		{
			_members.Clear();
			base.Dispose();
		}
		
		override public void LeaveBinaryExpression(BinaryExpression node)
		{
			if (BinaryOperatorType.ReferenceEquality == node.Operator)
			{
				if (IsTypeReference(node.Right))
				{
					Warnings.Add(
						CompilerWarningFactory.IsInsteadOfIsa(node));
				}
			}
		}
		
		bool IsTypeReference(Expression node)
		{
			return (NodeType.TypeofExpression == node.NodeType) ||
				(
					node is ReferenceExpression &&
					node.Entity is IType);
		}
		
		override public void LeaveEnumDefinition(EnumDefinition node)
		{
			_members.Clear();
			foreach (TypeMember member in node.Members)
			{
				if (_members.ContainsKey(member.Name))
				{
					MemberNameConflict(member);
				}
				else
				{
					_members[member.Name] = member;
				}
			}
		}
		
		override public void LeaveClassDefinition(ClassDefinition node)
		{
			CheckMembers(node);
		}
		
		override public void LeaveInterfaceDefinition(InterfaceDefinition node)
		{
			CheckMembers(node);
		}
		
		void CheckMembers(TypeDefinition node)
		{
			_members.Clear();
			
			foreach (TypeMember member in node.Members)
			{
				List list = GetMemberList(member.Name);
				switch (member.NodeType)
				{
					case NodeType.Constructor:
					case NodeType.Method:
					{
						CheckMethodMember(list, (Method)member);
						break;
					}
					
					default:
					{
						CheckMember(list, member);
						break;
					}
				}
				list.Add(member);
			}
		}
		
		void CheckMember(List existing, TypeMember member)
		{
			if (existing.Count > 0)
			{
				MemberNameConflict(member);
			}
		}
		
		void CheckMethodMember(List existing, TypeMember member)
		{
			NodeType expectedNodeType = member.NodeType;
			foreach (TypeMember existingMember in existing)
			{
				if (expectedNodeType != existingMember.NodeType)
				{
					MemberNameConflict(member);
				}
				else
				{
					if (existingMember.IsStatic == member.IsStatic)
					{
						if (AreParametersTheSame(existingMember, member))
						{
							MemberConflict(member, TypeSystemServices.GetSignature((IMethod)member.Entity, false));
						}
					}
				}
			}
		}
		
		bool AreParametersTheSame(TypeMember lhs, TypeMember rhs)
		{
			IParameter[] lhsParameters = ((InternalMethod)lhs.Entity).GetParameters();
			IParameter[] rhsParameters = ((InternalMethod)rhs.Entity).GetParameters();
			if (lhsParameters.Length != rhsParameters.Length)
			{
				return false;
			}
			for (int i=0; i<lhsParameters.Length; ++i)
			{
				if (lhsParameters[i].Type != rhsParameters[i].Type)
				{
					return false;
				}
			}
			return true;
		}
		
		void MemberNameConflict(TypeMember member)
		{
			MemberConflict(member, member.Name);
		}
		
		void MemberConflict(TypeMember member, string memberName)
		{
			Error(CompilerErrorFactory.MemberNameConflict(member, member.DeclaringType.FullName, memberName));
		}
		
		List GetMemberList(string name)
		{
			List list = (List)_members[name];
			if (null == list)
			{
				list = new List();
				_members[name] = list;
			}
			return list;
		}
		
		override public void OnGotoStatement(GotoStatement node)
		{			
			LabelStatement target = ((InternalLabel)node.Label.Entity).LabelStatement; 
					
			int gotoDepth = ContextAnnotations.GetTryBlockDepth(node);
			int targetDepth = ContextAnnotations.GetTryBlockDepth(target);
			if (gotoDepth < targetDepth)
			{
				BranchError(node, target);
			}
			else if (gotoDepth == targetDepth)
			{
				Node gotoParent = AstUtil.GetParentTryExceptEnsure(node);
				Node labelParent = AstUtil.GetParentTryExceptEnsure(target);
				if (gotoParent != labelParent)
				{
					BranchError(node, target);
				}
			}
		}
		
		void BranchError(GotoStatement node, LabelStatement target)
		{
			Node parent = AstUtil.GetParentTryExceptEnsure(target);
			switch (parent.NodeType)
			{
				case NodeType.TryStatement:
				{
					Error(CompilerErrorFactory.CannotBranchIntoTry(node.Label));
					break;
				}
				
				case NodeType.ExceptionHandler:
				{
					Error(CompilerErrorFactory.CannotBranchIntoExcept(node.Label));
					break;
				}
				
				case NodeType.Block:
				{
					Error(CompilerErrorFactory.CannotBranchIntoEnsure(node.Label));
					break;
				}
			}
		}
		
		override public void LeaveMethod(Method node)
		{
			InternalMethod derived = (InternalMethod)node.Entity;
			IMethod super = derived.Override;
			if (null != super)
			{
				TypeMemberModifiers derivedAccess = TypeSystemServices.GetAccess(derived);
				TypeMemberModifiers superAccess = TypeSystemServices.GetAccess(super);
				if (derivedAccess < superAccess)
				{
					Error(CompilerErrorFactory.DerivedMethodCannotReduceAccess(
								node,
								derived.FullName,
								super.FullName,
								derivedAccess,
								superAccess));
				}
			}
			
			CheckUnusedLocals(node);
		}
		
		void CheckUnusedLocals(Method node)
		{
			foreach (Local local in node.Locals)
			{
				InternalLocal entity = (InternalLocal)local.Entity;
				if (!entity.IsPrivateScope && !entity.IsUsed)
				{
					Warnings.Add(CompilerWarningFactory.UnusedLocalVariable(local, local.Name));
				}
			}
		}
		
		override public void LeaveConstructor(Constructor node)
		{
			if (node.IsStatic)
			{
				if (!node.IsPublic)
				{
					Error(CompilerErrorFactory.StaticConstructorMustBePublic(node));
				}
				if (node.Parameters.Count != 0)
				{
					Error(CompilerErrorFactory.StaticConstructorCannotDeclareParameters(node));
				}
			}
			CheckUnusedLocals(node);
		}
		
		override public void LeaveMethodInvocationExpression(MethodInvocationExpression node)
		{
			if (IsAddressOfBuiltin(node.Target))
			{
				if (!IsSecondArgumentOfDelegateConstructor(node))
				{
					Error(CompilerErrorFactory.AddressOfOutsideDelegateConstructor(node.Target));
				}
			}
		}
		
		bool IsSecondArgumentOfDelegateConstructor(Expression node)
		{                 
			MethodInvocationExpression mie = node.ParentNode as MethodInvocationExpression;
			if (null != mie)
			{
				if (IsDelegateConstructorInvocation(mie))
				{
					return mie.Arguments[1] == node;
				}
			}
			return false;
		}
		
		bool IsDelegateConstructorInvocation(MethodInvocationExpression node)
		{
			IConstructor constructor = node.Target.Entity as IConstructor;
			if (null != constructor)
			{
				return constructor.DeclaringType is ICallableType;
			}
			return false;
		}
		
		bool IsAddressOfBuiltin(Expression node)
		{
			return BuiltinFunction.AddressOf == node.Entity;
		}
	}
}
