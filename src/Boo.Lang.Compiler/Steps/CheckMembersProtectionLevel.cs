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
	using Boo.Lang.Compiler.TypeSystem;

	/// <summary>
	/// </summary>
	public class CheckMembersProtectionLevel : AbstractFastVisitorCompilerStep
	{
		private IAccessibilityChecker _checker = AccessibilityChecker.Global;

		override public void OnClassDefinition(ClassDefinition node)
		{
			IAccessibilityChecker saved = _checker;
			_checker = new AccessibilityChecker(node);
			base.OnClassDefinition(node);
			_checker = saved;
		}
		
		override public void OnMemberReferenceExpression(MemberReferenceExpression node)
		{
			base.OnMemberReferenceExpression(node);

			OnExpression(node);
		}
		
		override public void OnReferenceExpression(ReferenceExpression node)
		{
			OnExpression(node);
		}

		override public void OnSelfLiteralExpression(SelfLiteralExpression node)
		{
			base.OnSelfLiteralExpression(node);
			
			if (node.IsTargetOfMethodInvocation())
			{
				OnExpression(node);
			}
		}

		override public void OnSuperLiteralExpression(SuperLiteralExpression node)
		{
			base.OnSuperLiteralExpression(node);
			
			if (node.IsTargetOfMethodInvocation())
			{
				OnExpression(node);
			}
		}

		private void OnExpression(Expression node)
		{
			var member = node.Entity as IAccessibleMember;
			if (null == member) return;

			if (!IsAccessible(member))
			{
				Error(CompilerErrorFactory.UnaccessibleMember(node, member));
				return;
			}

			//if member is a property we also want to check the accessor specifically
			var property = member as IProperty;
		    if (null == property)
                return;

		    member = node.IsTargetOfAssignment() ? property.GetSetMethod() : property.GetGetMethod();
		    if (!IsAccessible(member))
		        Error(CompilerErrorFactory.UnaccessibleMember(node, member));
		}

	    private bool IsAccessible(IAccessibleMember member)
	    {
	        return _checker.IsAccessible(member);
	    }
	}
}
