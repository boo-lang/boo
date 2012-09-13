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

namespace Boo.Lang.Compiler.Steps
{
    public class ExpandPropertiesAndEvents : AbstractTransformerCompilerStep
    {
        public override void Run()
        {
            if (Errors.Count > 0)
                return;

            Visit(CompileUnit);
        }

    	public override void LeaveMemberReferenceExpression(MemberReferenceExpression node)
    	{
    		var property = node.Entity as IProperty;
    		if (property == null || node.IsTargetOfAssignment())
    			return;

    		var getter = CodeBuilder.CreatePropertyGet(node.Target, property);

    		// preserve duck typing...
    		if (property.IsDuckTyped)
    			ReplaceCurrentNode(
    				CodeBuilder.CreateCast(
    					TypeSystemServices.DuckType, getter));
    		else
    			ReplaceCurrentNode(getter);
    	}

    	public override void LeaveBinaryExpression(BinaryExpression node)
        {
            var eventInfo = node.Left.Entity as IEvent;
            if (eventInfo == null)
                return;

            IMethod method;
            if (node.Operator == BinaryOperatorType.InPlaceAddition)
                method = eventInfo.GetAddMethod();
            else
            {
                method = eventInfo.GetRemoveMethod();
                CheckEventUnsubscribe(node, eventInfo);
            }

            ReplaceCurrentNode(
                MethodInvocationForEventSubscription(node, method));
        }

    	private MethodInvocationExpression MethodInvocationForEventSubscription(BinaryExpression node, IMethod method)
        {
        	var target = ((MemberReferenceExpression) node.Left).Target;
        	return CodeBuilder.CreateMethodInvocation(node.LexicalInfo, target, method, node.Right);
        }

    	private void CheckEventUnsubscribe(BinaryExpression node, IEvent eventInfo)
        {
            var expected = ((ICallableType) eventInfo.Type).GetSignature();
            var actual = GetCallableSignature(node.Right);
        	if (expected != actual)
        		Warnings.Add(
        			CompilerWarningFactory.InvalidEventUnsubscribe(node, eventInfo, expected));
        }

    	private CallableSignature GetCallableSignature(Expression node)
        {
            return ((ICallableType) GetExpressionType(node)).GetSignature();
        }
    }
}