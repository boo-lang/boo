using System;
using System.IO;
using Boo.Ast;

namespace Boo.Ast.Visiting
{
	/// <summary>
	/// Imprime a AST em uma estrutura de rvore.
	/// </summary>
	public class TreePrinterVisitor : TextEmitter
	{
		public TreePrinterVisitor(TextWriter writer) : base(writer)
		{
		}

		public void Print(Node ast)
		{
			//ast.Accept(this);
		}
	}
}
