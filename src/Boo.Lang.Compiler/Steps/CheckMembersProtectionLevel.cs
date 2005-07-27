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
	using System.Collections;
	using Boo.Lang.Compiler;
	using Boo.Lang.Compiler.Ast;
	using Boo.Lang.Compiler.TypeSystem;

	/// <summary>
	/// </summary>
	public class CheckMembersProtectionLevel : AbstractVisitorCompilerStep
	{
		IType _currentType;

		Stack _classStack = new Stack();

		override public void Run()
		{
			Visit(CompileUnit);
		}

		override public void OnClassDefinition(ClassDefinition node)
		{	
			_classStack.Push(node);

			IType saved = _currentType;
			_currentType = (IType)node.Entity;
			base.OnClassDefinition(node);
			_currentType = saved;

			_classStack.Pop();
		}

		override public void LeaveMemberReferenceExpression(MemberReferenceExpression node)
		{
			IAccessibleMember member = node.Entity as IAccessibleMember;
			if (null == member) return;
			if (member.IsPublic) return;

			IType declaringType = member.DeclaringType;
			if (declaringType == _currentType) return;
			if (member.IsInternal && member is IInternalEntity) return;
			if (member.IsProtected)
			{
				if (_currentType.IsSubclassOf(declaringType)) return;
			}
			
			if (IsDeclaredInside(declaringType)) return;
			Error(CompilerErrorFactory.UnaccessibleMember(node, member.FullName));
		}

		bool IsDeclaredInside(IType candidate)
		{
			IInternalEntity entity = candidate as IInternalEntity;
			if (null == entity) return false;

			return _classStack.Contains(entity.Node);
		}
	}
}
