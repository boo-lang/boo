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
			if (0 == Errors.Count)
			{
				Visit(CompileUnit);
			}
		}
		
		override public void Dispose()
		{
			_members.Clear();
			base.Dispose();
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
