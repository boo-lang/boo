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
	
	public class UsingMacro : IAstMacro
	{
		public Statement Expand(MacroStatement macro)
		{
			if (macro.Arguments.Count != 1 ||
				NodeType.BinaryExpression != macro.Arguments[0].NodeType)
			{
				throw new NotImplementedException();
			}
			
			BinaryExpression expression = (BinaryExpression)macro.Arguments[0];
			if (BinaryOperatorType.Assign != expression.Operator ||
				NodeType.ReferenceExpression != expression.Left.NodeType)
			{
				throw new NotImplementedException();
			}
			
			TryStatement stmt = new TryStatement(macro.LexicalInfo);
			stmt.ProtectedBlock.Add(new ExpressionStatement(expression));
			stmt.ProtectedBlock.Add(macro.Block);
			stmt.EnsureBlock = CreateEnsureBlock((ReferenceExpression)expression.Left);
			
			return stmt;
		}
		
		Block CreateEnsureBlock(ReferenceExpression reference)
		{
			Block block = new Block(reference.LexicalInfo);
			
			// if __disposable = <reference> as System.IDisposable:
			IfStatement stmt = new IfStatement();
			stmt.TrueBlock = new Block();
			stmt.Expression = new BinaryExpression(
								BinaryOperatorType.Assign,
								new ReferenceExpression("__disposable"),
								new AsExpression(reference, new TypeReference("System.IDisposable"))
								);
							
			// __disposable.Dispose()
			stmt.TrueBlock.Add(
				new MethodInvocationExpression(
					new MemberReferenceExpression(
						new ReferenceExpression("__disposable"),
						"Dispose")
						)
					);
					
			// __disposable = null
			stmt.TrueBlock.Add(
				new BinaryExpression(
					BinaryOperatorType.Assign,
					new ReferenceExpression("__disposable"),
					new NullLiteralExpression()
					)
				);
			
			block.Add(stmt);
			return block;
		}
	}
}
