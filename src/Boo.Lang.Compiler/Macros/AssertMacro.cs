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
	using System.IO;
	using Boo.Lang.Compiler;
	using Boo.Lang.Compiler.Ast;
	using Boo.Lang.Compiler.Ast.Visitors;
	
	/// <summary>
	/// assert condition[, message]
	/// </summary>
	public class AssertMacro : AbstractCompilerComponent, IAstMacro
	{		
		private static Expression ExceptionTypeReference = 
			AstUtil.CreateReferenceExpression("Boo.AssertionFailedException");
		
		public Statement Expand(MacroStatement macro)
		{
			int argc = macro.Arguments.Count;
			if (argc != 1 && argc != 2)
			{
				// TODO: localize this message
				throw new System.ArgumentException(
					String.Format(
						"expecting 1 or 2 args to assert; got {0}", argc));
			}
			
			// if not in debug mode then expand to nothing
			// TODO: do we really want that? what about assert with side effects?
			if (!Context.Parameters.Debug)
			{
				return null;
			}
			
			// figure out the msg for the exception
			Expression condition = macro.Arguments[0];
			Expression message = (argc == 1) ?
				new StringLiteralExpression(
					condition.LexicalInfo, NodeToString(condition)) : 
				macro.Arguments[1];
				
			// unless <condition>:
			//     raise Boo.AssertionFailedException(<msg>)
			UnlessStatement stmt = new UnlessStatement(macro.LexicalInfo);
			stmt.Condition = condition;
			stmt.Block = new Block(macro.LexicalInfo);
			
			RaiseStatement raise = new RaiseStatement(macro.LexicalInfo);			
			raise.Exception = 
				AstUtil.CreateMethodInvocationExpression(ExceptionTypeReference, message);
			stmt.Block.Add(raise);
			
			return stmt;
		}

		private string NodeToString(Node node)
		{
			using (StringWriter writer = new StringWriter())
			{
				BooPrinterVisitor printer = new BooPrinterVisitor(writer);
				printer.Visit(node);
				return writer.ToString();
			}
		}	
	}	
}
