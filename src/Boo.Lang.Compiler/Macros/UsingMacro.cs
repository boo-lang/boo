#region license
// boo - an extensible programming language for the CLI
// Copyright (C) 2004 Rodrigo B. de Oliveira
//
// Permission is hereby granted, free of charge, to any person 
// obtaining a copy of this software and associated documentation 
// files (the "Software"), to deal in the Software without restriction, 
// including without limitation the rights to use, copy, modify, merge, 
// publish, distribute, sublicense, and/or sell copies of the Software, 
// and to permit persons to whom the Software is furnished to do so, 
// subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included 
// in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, 
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF 
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. 
// IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY 
// CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, 
// TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE 
// OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// 
// Contact Information
//
// mailto:rbo@acm.org
#endregion

namespace Boo.Lang
{
	using System;
	using Boo.Lang.Compiler.Ast;
	
	/// <summary>
	/// using file=File.OpenText(fname):
	///		print(file.ReadLine())
	/// </summary>
	public class UsingMacro : Boo.Lang.Compiler.IAstMacro
	{
		public const string DisposableLocalName = "__disposable__";
		
		public void Initialize(Boo.Lang.Compiler.CompilerContext context)
		{			
		}
		
		public void Dispose()
		{			
		}
		
		public Statement Expand(MacroStatement macro)
		{			
			// only single assignments are supported
			// right now
			if (macro.Arguments.Count != 1 ||
				!IsAssignmentToReference(macro.Arguments[0]))
			{
				throw new NotImplementedException();
			}
			
			BinaryExpression expression = (BinaryExpression)macro.Arguments[0];

			// try:
			//		assignment
			// 		<the rest>
			// ensure:
			//		...			
			TryStatement stmt = new TryStatement(macro.LexicalInfo);
			stmt.ProtectedBlock.Add(expression);
			stmt.ProtectedBlock.Add(macro.Block);
			stmt.EnsureBlock = CreateEnsureBlock((ReferenceExpression)expression.Left);
			
			return stmt;
		}
		
		Block CreateEnsureBlock(ReferenceExpression reference)
		{
			Block block = new Block(reference.LexicalInfo);
			
			// if __disposable = <reference> as System.IDisposable:
			IfStatement stmt = new IfStatement();			
			stmt.Condition = new BinaryExpression(
								BinaryOperatorType.Assign,
								new ReferenceExpression(DisposableLocalName),
								new AsExpression(reference, new SimpleTypeReference("System.IDisposable"))
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
			return block;
		}
		
		bool IsAssignmentToReference(Expression expression)
		{
			if (NodeType.BinaryExpression != expression.NodeType)
			{
				return false;
			}
			
			BinaryExpression binaryExpression = (BinaryExpression)expression;
			return
				BinaryOperatorType.Assign == binaryExpression.Operator &&
				NodeType.ReferenceExpression == binaryExpression.Left.NodeType;
		}
	}
}
