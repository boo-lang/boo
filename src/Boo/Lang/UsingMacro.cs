#region license
// boo - an extensible programming language for the CLI
// Copyright (C) 2004 Rodrigo B. de Oliveira
//
// This program is free software; you can redistribute it and/or
// modify it under the terms of the GNU General Public License
// as published by the Free Software Foundation; either version 2
// of the License, or (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.
//
// As a special exception, if you link this library with other files to
// produce an executable, this library does not by itself cause the
// resulting executable to be covered by the GNU General Public License.
// This exception does not however invalidate any other reasons why the
// executable file might be covered by the GNU General Public License.
//
// Contact Information
//
// mailto:rbo@acm.org
#endregion

namespace Boo.Lang
{
	using System;
	using Boo.Lang.Ast;
	
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
			stmt.Expression = new BinaryExpression(
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
