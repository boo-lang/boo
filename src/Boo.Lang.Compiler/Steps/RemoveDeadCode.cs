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


namespace Boo.Lang.Compiler.Steps
{
	using Boo.Lang.Compiler.Ast;
	using System.Collections.Generic;

	public class RemoveDeadCode : AbstractTransformerCompilerStep
	{
		override public void Run()
		{
			Visit(CompileUnit);
		}

		override public bool EnterRaiseStatement(RaiseStatement node)
		{
			RemoveUnreachableCode(node);
			return false;
		}

		public override void OnReturnStatement(ReturnStatement node)
		{
			RemoveUnreachableCode(node);
		}

		override public bool EnterBreakStatement(BreakStatement node)
		{
			RemoveUnreachableCode(node);
			return false;
		}

		override public bool EnterContinueStatement(ContinueStatement node)
		{
			RemoveUnreachableCode(node);
			return false;
		}

		override public bool EnterMethodInvocationExpression(Boo.Lang.Compiler.Ast.MethodInvocationExpression node)
		{
			return false;
		}

		private void RemoveUnreachableCode(Statement node)
		{
			Block block = node.ParentNode as Block;			
			if (null == block) return;

			int from = DetectUnreachableCode(block, node);
			if (-1 != from) RemoveStatements(block, from);
		}
		
		//this method returns -1 if it doesn't detect unreachable code
		//else it returns the index of the first unreachable in block.Statements 
		private int DetectUnreachableCode(Block block, Statement limit)
		{
			bool unreachable = false;
			int idx = 0;
			foreach (Statement stmt in block.Statements)
			{
				//HACK: __switch__ builtin function is hard to detect/handle
				//		within this context, let's ignore whatever is after __switch__
				ExpressionStatement est = stmt as ExpressionStatement;
				if (null != est)
				{
					MethodInvocationExpression mie = est.Expression as MethodInvocationExpression;
					if (null != mie && TypeSystem.BuiltinFunction.Switch == mie.Target.Entity)
						return -1;//ignore followings
				}

				if (unreachable && stmt is LabelStatement)
					return -1;

				if (stmt == limit)
				{
					unreachable = true;
				}
				else if (unreachable)
				{
					Warnings.Add(
						CompilerWarningFactory.UnreachableCodeDetected(stmt) );
					return idx;
				}
				idx++;
			}
			return -1;
		}

		private static void RemoveStatements(Block block, int fromIndex)
		{
			for (int idx = block.Statements.Count-1; idx >= fromIndex; idx--)
				block.Statements.RemoveAt(idx);
		}

		override public void OnTryStatement(TryStatement node)
		{
			if (node.ProtectedBlock.IsEmpty)
			{
				if (null != node.EnsureBlock && !node.EnsureBlock.IsEmpty)
				{
					ReplaceCurrentNode(node.EnsureBlock);
				}
				else
				{
					RemoveCurrentNode();
				}
			}
			else
			{
				base.OnTryStatement(node);
			}
		}
	}
}
