#region license
// boo - an extensible programming language for the CLI
// Copyright (C) 2004 Rodrigo Barreto de Oliveira
//
// This library is free software; you can redistribute it and/or
// modify it under the terms of the GNU Lesser General Public
// License as published by the Free Software Foundation; either
// version 2.1 of the License, or (at your option) any later version.
// 
// This library is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
// Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public
// License along with this library; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA 02111-1307 USA
// 
// Contact Information
//
// mailto:rbo@acm.org
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
	public class UsingMacro : AbstractCompilerComponent, IAstMacro
	{
		public const string DisposableLocalName = "__disposable__";
		
		public Statement Expand(MacroStatement macro)
		{	
			if (macro.Arguments.Count != 1)
			{
				throw new NotImplementedException();
			}			
			
			Expression expression = macro.Arguments[0];
			Expression reference = null;
			
			if (IsAssignmentToReference(expression))
			{
				reference = ((BinaryExpression)expression).Left;
			}
			else
			{
				reference = new ReferenceExpression(expression.LexicalInfo,
								   string.Format("__using{0}__", _context.AllocIndex()));
				expression = new BinaryExpression(expression.LexicalInfo,
									BinaryOperatorType.Assign,
									reference,
									expression);
			}

			// try:
			//		assignment
			// 		<the rest>
			// ensure:
			//		...			
			TryStatement stmt = new TryStatement(macro.LexicalInfo);
			stmt.ProtectedBlock.Add(expression);
			stmt.ProtectedBlock.Add(macro.Block);
			stmt.EnsureBlock = CreateEnsureBlock(reference);
			
			return stmt;
		}
		
		Block CreateEnsureBlock(Expression reference)
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
			block.Add(new BinaryExpression(
						BinaryOperatorType.Assign,
						reference.CloneNode(),
						new NullLiteralExpression()
						)
					);
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
