using System;

namespace Boo.Ast.Visiting
{
	/// <summary>
	/// Imprime a rvore em pseudo-C#.
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
		
		public override bool EnterModule(Module g)
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

		public override bool EnterImport(Using p)
		{			
			WriteLine("using {0};", p.Namespace);
			WriteLine();
			return true;
		}

		public override bool EnterClassDefinition(ClassDefinition c)
		{		
			WriteLine("[Serializable]");
			WriteLine("public class {0}", c.Name);
			WriteLine("{");
			Indent();
			return true;
		}

		public override bool LeaveClassDefinition(ClassDefinition c)
		{			
			Dedent();
			WriteLine("}");
			WriteLine();
			return true;
		}

		public override bool EnterField(Field f)
		{
			WriteLine("protected {0} {1};", ResolveType(f.Type), f.Name);
			return true;
		}

		public override bool EnterMethod(Method m)
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

		public override bool EnterBlock(Block b)
		{
			WriteLine("{");
			Indent();
			return true;
		}

		public override bool LeaveBlock(Block b)
		{
			Dedent();
			WriteLine("}");
			return true;
		}

		public override bool EnterReturnStatement(ReturnStatement r)
		{
			WriteIndented("return ");
			return true;
		}

		public override bool LeaveReturnStatement(ReturnStatement r)
		{
			WriteLine(";");
			return true;
		}

		public override bool EnterExpressionStatement(ExpressionStatement es)
		{
			WriteIndented("");
			return true;
		}

		public override bool LeaveExpressionStatement(ExpressionStatement es)
		{
			WriteLine(";");
			return true;
		}

		public override bool EnterBinaryExpression(BinaryExpression e)
		{			
			e.Left.Accept(this);
			Write(ResolveOperator(e.Operator));
			e.Right.Accept(this);
			return false;
		}

		public override bool EnterReferenceExpression(ReferenceExpression e)
		{
			Write(e.Name);
			return true;
		}

		public override bool EnterMethodInvocationExpression(MethodInvocationExpression e)
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

		public override bool EnterIntegerLiteralExpression(IntegerLiteralExpression e)
		{
			Write(e.Value.ToString());
			return true;
		}

		public override bool EnterStringLiteralExpression(StringLiteralExpression e)
		{
			Write("\"");
			Write(e.Value);
			Write("\"");
			return true;
		}

		public override bool EnterListLiteralExpression(ListLiteralExpression lle)
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

		public override bool EnterForStatement(ForStatement fs)
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
				case BinaryOperatorType.Add:
				{
					return "+";
				}

				case BinaryOperatorType.Subtract:
				{
					return "-";
				}

				case BinaryOperatorType.Multiply:
				{
					return "*";
				}

				case BinaryOperatorType.Divide:
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
