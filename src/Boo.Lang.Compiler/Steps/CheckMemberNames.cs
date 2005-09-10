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


using System.Collections;
using Boo.Lang.Compiler.Ast;
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
				case NodeType.Constructor:
				case NodeType.Method:
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
							&& !AreDifferentInterfaceMembers((IExplicitMember)existingMember, (IExplicitMember)member))
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
			return AreParametersTheSame(lhsParameters, rhsParameters);
		}

		private static IParameter[] GetParameters(IEntity entity)
		{
			return ((IEntityWithParameters)entity).GetParameters();
		}

		private static bool AreParametersTheSame(IParameter[] lhsParameters, IParameter[] rhsParameters)
		{
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
	}
}
