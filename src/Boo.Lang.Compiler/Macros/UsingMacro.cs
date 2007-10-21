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

namespace Boo.Lang
{
	using System;
	using Boo.Lang.Compiler;
	using Boo.Lang.Compiler.Ast;
	
	/// <summary>
	/// using file=File.OpenText(fname):
	///		print(file.ReadLine())
	/// </summary>
	public class UsingMacro : AbstractAstMacro
	{
		private const string DisposableLocalName = "__disposable__";
		
		public override Statement Expand(MacroStatement macro)
		{
			// try:
			//		assignment
			// 		<the rest>
			// ensure:
			//		...			
			TryStatement stmt = new TryStatement(macro.LexicalInfo);
			stmt.EnsureBlock = new Block(macro.LexicalInfo);
			
			foreach (Expression expression in macro.Arguments)
			{
				Expression reference;

				if (expression is ReferenceExpression)
				{
					reference = expression;
				}
				else
				{
					if (IsAssignmentToReference(expression))
					{
						reference = ((BinaryExpression)expression).Left.CloneNode();
						stmt.ProtectedBlock.Add(expression);
					}
					else
					{
						string tempName = string.Format("__using{0}__", _context.AllocIndex());
						reference = new ReferenceExpression(expression.LexicalInfo, tempName);
						stmt.ProtectedBlock.Add(new BinaryExpression(expression.LexicalInfo,
														BinaryOperatorType.Assign,
														reference.CloneNode(),
														expression));

					}
					
				}

				AugmentEnsureBlock(stmt.EnsureBlock, reference);
			}
			
			stmt.ProtectedBlock.Add(macro.Block);
			return stmt;
		}
		
		private void AugmentEnsureBlock(Block block, Expression reference)
		{
			// if __disposable = <reference> as System.IDisposable:
			IfStatement stmt = new IfStatement();			
			stmt.Condition = new BinaryExpression(
								BinaryOperatorType.Assign,
								new ReferenceExpression(DisposableLocalName),
								new TryCastExpression(reference, new SimpleTypeReference("System.IDisposable"))
								);			
			
			stmt.TrueBlock = new Block();
			
			// __disposable.Dispose()
			stmt.TrueBlock.Add(
				new MethodInvocationExpression(
					new MemberReferenceExpression(
						new ReferenceExpression(DisposableLocalName),
						"Dispose")
						)
					);
				
			// __disposable = null
			stmt.TrueBlock.Add(
				new BinaryExpression(
					BinaryOperatorType.Assign,
					new ReferenceExpression(DisposableLocalName),
					new NullLiteralExpression()
					)
				);
			
			block.Add(stmt);
		}
		
		private bool IsAssignmentToReference(Expression expression)
		{
			if (NodeType.BinaryExpression != expression.NodeType)
			{
				return false;
			}
			
			BinaryExpression binaryExpression = (BinaryExpression)expression;
			return
				BinaryOperatorType.Assign == binaryExpression.Operator &&
				binaryExpression.Left is ReferenceExpression;
		}
	}
}
