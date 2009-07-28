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
	public class ExpandVarArgsMethodInvocations : AbstractVisitorCompilerStep
	{
		override public void Run()
		{
			if (0 == Errors.Count)
			{
				Visit(CompileUnit);
			}
		}

		public override void LeaveMethodInvocationExpression(Boo.Lang.Compiler.Ast.MethodInvocationExpression node)
		{
			IMethod method = TypeSystemServices.GetOptionalEntity(node.Target) as IMethod;
			if (null != method && method.AcceptVarArgs)
			{
				ExpandInvocation(node, method.GetParameters());
				return;
			}

			ICallableType callable = node.Target.ExpressionType as ICallableType;
			if (callable != null)
			{
				CallableSignature signature = callable.GetSignature();
				if (!signature.AcceptVarArgs) return;
				
				ExpandInvocation(node, signature.Parameters);
				return;
			}
		}

		protected virtual void ExpandInvocation(MethodInvocationExpression node, IParameter[] parameters)
		{
			if (node.Arguments.Count > 0 && AstUtil.IsExplodeExpression(node.Arguments[-1]))
			{
				// explode the arguments
				node.Arguments.ReplaceAt(-1, ((UnaryExpression)node.Arguments[-1]).Operand);
				return;
			}

			int lastParameterIndex = parameters.Length-1;
			IType varArgType = parameters[lastParameterIndex].Type;

			ExpressionCollection varArgs = node.Arguments.PopRange(lastParameterIndex);
			node.Arguments.Add(CodeBuilder.CreateArray(varArgType, varArgs));
		}
	}
}
