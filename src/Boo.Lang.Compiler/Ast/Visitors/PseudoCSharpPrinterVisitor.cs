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

using System;

namespace Boo.Lang.Compiler.Ast.Visitors
{
	/// <summary>
	/// Prints a boo ast in pseudo C#.
	/// </summary>
	public class PseudoCSharpPrinterVisitor : TextEmitter
	{
		public PseudoCSharpPrinterVisitor(System.IO.TextWriter writer) : base(writer)
		{
		}

		public void Print(Node node)
		{
		//	node.Accept(this);
		}

		/*
		#region IVisitor Members
		
		override public bool EnterModule(Module g)
		{
			WriteLine("public static final class Module");
			WriteLine("{");
			Indent();

			g.Members.Accept(this);
			if (g.Globals.Statements.Count > 0)
			{
				WriteLine("public static void Main(string[] args)");
				WriteLine("{");
				Indent();
				g.Globals.Statements.Accept(this);
				Dedent();
				WriteLine("}");
			}

			Dedent();
			WriteLine("}");
			return false;
		}

		override public bool EnterImport(Import p)
		{			
			WriteLine("using {0};", p.Namespace);
			WriteLine();
			return true;
		}

		override public bool EnterClassDefinition(ClassDefinition c)
		{		
			WriteLine("[Serializable]");
			WriteLine("public class {0}", c.Name);
			WriteLine("{");
			Indent();
			return true;
		}

		override public bool LeaveClassDefinition(ClassDefinition c)
		{			
			Dedent();
			WriteLine("}");
			WriteLine();
			return true;
		}

		override public bool EnterField(Field f)
		{
			WriteLine("protected {0} {1};", ResolveType(f.Type), f.Name);
			return true;
		}

		override public bool EnterMethod(Method m)
		{
			WriteIndented("public {0} {1}(", ResolveType(m.ReturnType), m.Name);
			for (int i=0; i<m.Parameters.Count; ++i)
			{
				if (i > 0)
				{
					Write(", ");
				}
				ParameterDeclaration pd = m.Parameters[i];
				Write("{0} {1}", ResolveType(pd.Type), pd.Name);
			}
			WriteLine(")");
			return true;
		}

		override public bool EnterBlock(Block b)
		{
			WriteLine("{");
			Indent();
			return true;
		}

		override public bool LeaveBlock(Block b)
		{
			Dedent();
			WriteLine("}");
			return true;
		}

		override public bool EnterReturnStatement(ReturnStatement r)
		{
			WriteIndented("return ");
			return true;
		}

		override public bool LeaveReturnStatement(ReturnStatement r)
		{
			WriteLine(";");
			return true;
		}

		override public bool EnterExpressionStatement(ExpressionStatement es)
		{
			WriteIndented("");
			return true;
		}

		override public bool LeaveExpressionStatement(ExpressionStatement es)
		{
			WriteLine(";");
			return true;
		}

		override public bool EnterBinaryExpression(BinaryExpression e)
		{			
			e.Left.Accept(this);
			Write(ResolveOperator(e.Operator));
			e.Right.Accept(this);
			return false;
		}

		override public bool EnterReferenceExpression(ReferenceExpression e)
		{
			Write(e.Name);
			return true;
		}

		override public bool EnterMethodInvocationExpression(MethodInvocationExpression e)
		{
			e.Target.Accept(this);
			Write("(");
			for (int i=0; i<e.Parameters.Count; ++i)
			{
				if (i>0)
				{
					Write(", ");
				}
				e.Parameters[i].Accept(this);
			}
			Write(")");
			return false;
		}

		override public bool EnterIntegerLiteralExpression(IntegerLiteralExpression e)
		{
			Write(e.Value.ToString());
			return true;
		}

		override public bool EnterStringLiteralExpression(StringLiteralExpression e)
		{
			Write("\"");
			Write(e.Value);
			Write("\"");
			return true;
		}

		override public bool EnterListLiteralExpression(ListLiteralExpression lle)
		{
			Write("new ArrayList(new object[] { ");
			for (int i=0; i<lle.Items.Count; ++i)
			{
				if (i>0)
				{
					Write(", ");
				}
				lle.Items[i].Accept(this);
			}
			Write(" })");
			return false;
		}

		override public bool EnterForStatement(ForStatement fs)
		{
			WriteIndented("foreach (");
			for (int i=0; i<fs.Declarations.Count; ++i)
			{
				if (i>0)
				{
					Write(", ");
				}
				Write(ResolveType(fs.Declarations[i].Type));
				Write(" {0}", fs.Declarations[i].Name);
			}
			Write(" in ");
			fs.Iterator.Accept(this);
			WriteLine(")");
			WriteLine("{");
			Indent();

			fs.Statements.Accept(this);

			Dedent();
			WriteLine("}");
		
			return false;
		}

		#endregion

		string ResolveType(TypeReference t)
		{
			if (null == t)
			{
				return "object";
			}
			return t.Name;
		}

		string ResolveOperator(BinaryOperatorType o)
		{
			switch (o)
			{
				case BinaryOperatorType.Addition:
				{
					return "+";
				}

				case BinaryOperatorType.Subtraction:
				{
					return "-";
				}

				case BinaryOperatorType.Multiply:
				{
					return "*";
				}

				case BinaryOperatorType.Division:
				{
					return "/";
				}

				case BinaryOperatorType.LessThan:
				{
					return "<";
				}
			}
			return "?";
		}
		*/
	}
}
