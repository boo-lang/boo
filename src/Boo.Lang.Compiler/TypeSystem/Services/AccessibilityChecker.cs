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
using Boo.Lang.Compiler.TypeSystem.Generics;


namespace Boo.Lang.Compiler.Steps
{
	public class AccessibilityChecker : IAccessibilityChecker
	{
		public static readonly IAccessibilityChecker Global = new GlobalAccessibilityChecker();
		
		private readonly TypeDefinition _scope;

		public AccessibilityChecker(TypeDefinition scope)
		{
			_scope = scope;
		}

		public bool IsAccessible(IAccessibleMember member)
		{
			if (member.IsPublic)
				return true;

			IInternalEntity internalEntity = GetInternalEntity(member);
			if (null != internalEntity)
			{
				internalEntity.Node.RemoveAnnotation("PrivateMemberNeverUsed");
				if (member.IsInternal)
					return true;
			}

			IType declaringType = member.DeclaringType;
			if (declaringType == CurrentType())
				return true;

			if (member.IsProtected && CurrentType().IsSubclassOf(declaringType))
				return true;

			return IsDeclaredInside(declaringType);
		}

		static IInternalEntity GetInternalEntity(IAccessibleMember member)
		{
			if (member is IInternalEntity)
				return (IInternalEntity) member;

			IGenericMappedMember gmp = member as IGenericMappedMember;
			if (null != gmp && gmp.SourceMember is IInternalEntity)
				return (IInternalEntity) gmp.SourceMember;

			return null;
		}

		private IType CurrentType()
		{
			return (IType)_scope.Entity;
		}

		private bool IsDeclaredInside(IType candidate)
		{
			IInternalEntity entity = candidate as IInternalEntity;
			if (null == entity) return false;

			TypeDefinition type = _scope.DeclaringType;
			while (type != null)
			{
				if (type == entity.Node) return true;
				type = type.DeclaringType;
			}
			return false;
		}
		
		public class GlobalAccessibilityChecker : IAccessibilityChecker
		{	
			public bool IsAccessible(IAccessibleMember member)
			{
				if (member.IsPublic) return true;
				return member.IsInternal && member is IInternalEntity;
			}
		}
	}
}
