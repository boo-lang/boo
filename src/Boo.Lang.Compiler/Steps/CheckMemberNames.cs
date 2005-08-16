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
