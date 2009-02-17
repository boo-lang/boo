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

using System;
using System.Collections;
using System.Collections.Generic;
using Boo.Lang.Compiler.Ast;
using Boo.Lang.Compiler.TypeSystem.Generics;
using Boo.Lang.Compiler.Util;
using Boo.Lang.Compiler.TypeSystem;

namespace Boo.Lang.Compiler.Steps
{
	/// <summary>
	/// </summary>
	public class CheckMemberNames : AbstractVisitorCompilerStep
	{
		protected Hashtable _members = new Hashtable();
		
		public override void Run()
		{
			Visit(this.CompileUnit);
		}

		override public void Dispose()
		{
			_members.Clear();
			base.Dispose();
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
				if (member.NodeType == NodeType.StatementTypeMember)
					continue;

				List list = GetMemberList(member.Name);
				CheckMember(list, member);
				list.Add(member);
			}
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

		protected void CheckMember(List list, TypeMember member)
		{
			switch (member.NodeType)
			{
				case NodeType.StatementTypeMember:
					break;
				case NodeType.Constructor:
				case NodeType.Method:
				{
					CheckOverloadableMember(list, member);
					CheckLikelyTypoInTypeMemberName(member);
					break;
				}

				case NodeType.Property:
				{
					CheckOverloadableMember(list, member);
					break;
				}

				default:
				{
					CheckNonOverloadableMember(list, member);
					break;
				}
			}
		}


		protected void CheckNonOverloadableMember(List existing, TypeMember member)
		{
			if (existing.Count > 0)
			{
				MemberNameConflict(member);
			}
		}

		protected void CheckOverloadableMember(List existing, TypeMember member)
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
						if (AreParametersTheSame(existingMember, member)
							&& !AreDifferentInterfaceMembers((IExplicitMember)existingMember, (IExplicitMember)member)
							&& !AreDifferentConversionOperators(existingMember, member)
							&& IsGenericityTheSame(existingMember, member))
						{
							MemberConflict(member, TypeSystemServices.GetSignature((IEntityWithParameters)member.Entity, false));
						}
					}
				}
			}
		}
		
		bool AreParametersTheSame(TypeMember lhs, TypeMember rhs)
		{
			IParameter[] lhsParameters = GetParameters(lhs.Entity);
			IParameter[] rhsParameters = GetParameters(rhs.Entity);
			return CallableSignature.AreSameParameters(lhsParameters, rhsParameters);
		}

		private static IParameter[] GetParameters(IEntity entity)
		{
			return ((IEntityWithParameters)entity).GetParameters();
		}

		bool IsGenericityTheSame(TypeMember lhs, TypeMember rhs)
		{
			IGenericParameter[] lgp = GenericsServices.GetGenericParameters(lhs.Entity);
			IGenericParameter[] rgp = GenericsServices.GetGenericParameters(rhs.Entity);
			return (lgp == rgp || (null != lgp && null != rgp && lgp.Length == rgp.Length));
		}

		bool AreDifferentInterfaceMembers(IExplicitMember lhs, IExplicitMember rhs)
		{
			if (lhs.ExplicitInfo == null && rhs.ExplicitInfo == null)
			{
				return false;
			}
			
			if (
				lhs.ExplicitInfo != null &&
				rhs.ExplicitInfo != null &&
				lhs.ExplicitInfo.InterfaceType.Entity == rhs.ExplicitInfo.InterfaceType.Entity
				)
			{
				return false;
			}

			return true;
		}

		bool AreDifferentConversionOperators(TypeMember existing, TypeMember actual)
		{
			if ((existing.Name == "op_Implicit" || existing.Name == "op_Explicit")
				&& existing.Name == actual.Name
				&& existing.NodeType == NodeType.Method
				&& existing.IsStatic && actual.IsStatic)
			{
				IMethod one = existing.Entity as IMethod;
				IMethod two = actual.Entity as IMethod;
				if (one != null && two != null && one.ReturnType != two.ReturnType)
				{
					return true;
				}
			}
			return false;
 		}

		protected void MemberNameConflict(TypeMember member)
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

		protected void CheckLikelyTypoInTypeMemberName(TypeMember member)
		{
			foreach (string name in GetLikelyTypoNames(member))
			{
				if (name == member.Name)
					return;
				if (Math.Abs(name.Length - member.Name.Length) > 1)
					continue; //>1 distance, skip
				if (1 == StringUtilities.GetDistance(name, member.Name))
				{
					Warnings.Add(
						CompilerWarningFactory.LikelyTypoInTypeMemberName(member, name));
					break;
				}
			}
		}

		protected virtual IEnumerable<string> GetLikelyTypoNames(TypeMember member)
		{
			char first = member.Name[0];
			if (first == 'c' || first == 'C')
				yield return "constructor";
			else if (first == 'd' || first == 'D')
				yield return "destructor";
			if (member.IsStatic && member.Name.StartsWith("op_"))
			{
				yield return "op_Implicit";
				yield return "op_Addition";
				yield return "op_Subtraction";
				yield return "op_Multiply";
				yield return "op_Division";
				yield return "op_Modulus";
				yield return "op_Exponentiation";
				yield return "op_Equality";
				yield return "op_LessThan";
				yield return "op_LessThanOrEqual";
				yield return "op_GreaterThan";
				yield return "op_GreaterThanOrEqual";
				yield return "op_Match";
				yield return "op_NotMatch";
				yield return "op_Member";
				yield return "op_NotMember";
				yield return "op_BitwiseOr";
				yield return "op_BitwiseAnd";
				yield return "op_UnaryNegation";
			}
		}

	}
}
