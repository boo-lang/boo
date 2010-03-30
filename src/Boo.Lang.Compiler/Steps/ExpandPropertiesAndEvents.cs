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

        public override void LeaveBinaryExpression(BinaryExpression node)
        {
            var eventInfo = TypeSystemServices.GetOptionalEntity(node.Left) as IEvent;
            if (null == eventInfo)
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
            var methodTarget = CodeBuilder.CreateMemberReference(node.Left.LexicalInfo,
                ((MemberReferenceExpression)node.Left).Target, method);

            var mie = new MethodInvocationExpression(methodTarget);
            mie.Arguments.Add(node.Right);
            BindExpressionType(mie, method.ReturnType);
            return mie;
        }

        private void CheckEventUnsubscribe(BinaryExpression node, IEvent eventInfo)
        {
            CallableSignature expected = ((ICallableType) eventInfo.Type).GetSignature();
            CallableSignature actual = GetCallableSignature(node.Right);
            if (expected != actual)
            {
                Warnings.Add(
                    CompilerWarningFactory.InvalidEventUnsubscribe(
                        node,
                        eventInfo.FullName,
                        expected));
            }
        }

        public override void LeaveMemberReferenceExpression(MemberReferenceExpression node)
        {
            if (null == node.Entity) return;
            if (EntityType.Property != node.Entity.EntityType) return;
            if (AstUtil.IsLhsOfAssignment(node)) return;

            var property = (IProperty) node.Entity;
            MethodInvocationExpression getter = CodeBuilder.CreatePropertyGet(node.Target, property);

            // preserve duck typing...
            if (property.IsDuckTyped)
            {
                ReplaceCurrentNode(
                    CodeBuilder.CreateCast(
                        TypeSystemServices.DuckType, getter));
            }
            else
            {
                ReplaceCurrentNode(getter);
            }
        }

        private CallableSignature GetCallableSignature(Expression node)
        {
            return ((ICallableType) GetExpressionType(node)).GetSignature();
        }
    }
}