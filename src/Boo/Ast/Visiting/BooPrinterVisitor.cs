using System;
using System.IO;
using Boo.Ast;

namespace Boo.Ast.Visiting
{
	/// <summary>
	/// Imprime uma AST boo em boo. Útil para visualizar a 
	/// expansão de Ast Attributes e mixins.
	/// </summary>
	public class BooPrinterVisitor : TextEmitter
	{		
		public BooPrinterVisitor(TextWriter writer) : base(writer)
		{
		}

		public void Print(CompileUnit ast)
		{
			OnCompileUnit(ast);
		}	
		
		#region IVisitor Members	

		public override void OnModule(Module m)
		{
			Switch(m.Package);

			if (m.Using.Count > 0)
			{
				Switch(m.Using);
				WriteLine();
			}

			Switch(m.Members);

			// m.Globals iria causar um Indent()
			// inválido
			Switch(m.Globals.Statements);
		}

		public override void OnPackage(Package p)
		{
			WriteLine("package {0}", p.Name);
			WriteLine();
		}

		public override void OnUsing(Using p)
		{
			WriteLine("using {0}", p.Namespace);
		}

		public override bool EnterBlock(Block b)
		{
			Indent();
			if (0 == b.Statements.Count)
			{
				WriteIndented("pass");
			}
			return true;
		}

		public override void LeaveBlock(Block b)
		{
			Dedent();
		}

		public override void OnClassDefinition(ClassDefinition c)
		{
			WriteTypeDefinition("class", c);
		}

		public override void OnInterfaceDefinition(InterfaceDefinition id)
		{
			WriteTypeDefinition("interface", id);
		}

		public override void OnEnumDefinition(EnumDefinition ed)
		{
			WriteTypeDefinition("enum", ed);
		}

		public override void OnField(Field f)
		{
			WriteIndented(f.Name);
			Switch(f.Type);
			WriteLine();
		}

		public override void OnConstructor(Constructor c)
		{
			OnMethod(c);
		}

		public override void OnMethod(Method m)
		{
			WriteIndented("def ");
			Write(m.Name);
			Write("(");
			for (int i=0; i<m.Parameters.Count; ++i)
			{
				if (i > 0)
				{
					Write(", ");
				}
				OnParameterDeclaration(m.Parameters[i]);
			}
			Write(")");
			Switch(m.ReturnType);
			WriteLine(":");
			OnBlock(m.Body);	
			WriteLine();
		}

		public override void OnParameterDeclaration(ParameterDeclaration p)
		{
			Write(p.Name);
			Switch(p.Type);
		}

		public override void OnTypeReference(TypeReference t)
		{
			Write(" as ");
			Write(t.Name);
		}

		public override void OnMemberReferenceExpression(MemberReferenceExpression e)
		{
			Switch(e.Target);
			Write(".");
			Write(e.Name);
		}

		public override void OnBinaryExpression(BinaryExpression e)
		{
			Switch(e.Left);
			WriteOperator(e.Operator);
			Switch(e.Right);
		}

		public override void OnProperty(Property node)
		{
			WriteIndented(node.Name);
			OnTypeReference(node.Type);
			WriteLine(":");
			Indent();
			if (null != node.Getter)
			{
				WriteLine("get:");
				OnBlock(node.Getter.Body);
			}
			if (null != node.Setter)
			{
				WriteLine("set:");
				OnBlock(node.Setter.Body);
			}
			Dedent();
			WriteLine();
		}

		public override bool EnterRaiseStatement(RaiseStatement rs)
		{
			WriteIndented("raise ");
			return true;
		}

		public override void LeaveRaiseStatement(RaiseStatement rs)
		{
			WriteLine();
		}

		public override void OnMethodInvocationExpression(MethodInvocationExpression e)
		{
			Switch(e.Target);
			Write("(");
			bool commaNeeded = false;
			for (int i=0; i<e.Arguments.Count; ++i)
			{
				if (commaNeeded)
				{
					Write(", ");
				}
				else
				{
					commaNeeded = true;
				}
				Switch(e.Arguments[i]);
			}
			for (int i=0; i<e.NamedArguments.Count; ++i)
			{
				if (commaNeeded)
				{
					Write(", ");
				}
				else
				{
					commaNeeded = true;
				}
				OnExpressionPair(e.NamedArguments[i]);
			}
			Write(")");
		}

		public override void OnSlicingExpression(SlicingExpression node)
		{
			Switch(node.Target);
			Write("[");
			Switch(node.Begin);
			if (null != node.End)
			{
				Write(":");
				Switch(node.End);
			}
			if (null != node.Step)
			{
				Write(":");
				Switch(node.Step);
			}
			Write("]");
		}

		public override void OnExpressionPair(ExpressionPair pair)
		{
			Switch(pair.First);
			Write(": ");
			Switch(pair.Second);
		}

		public override void OnStringLiteralExpression(StringLiteralExpression e)
		{
			Write("'{0}'", e.Value);
		}

		public override void OnIntegerLiteralExpression(IntegerLiteralExpression e)
		{
			Write(e.Value.ToString());
		}

		public override void OnReferenceExpression(ReferenceExpression node)
		{
			Write(node.Name);
		}

		public override bool EnterExpressionStatement(ExpressionStatement es)
		{
			WriteIndented();
			return true;
		}

		public override void LeaveExpressionStatement(ExpressionStatement es)
		{
			WriteLine();
		}

		public override void OnStringFormattingExpression(StringFormattingExpression sfe)
		{
			Write("string.Format('{0}'", sfe.Template);
			foreach (Expression e in sfe.Arguments)
			{
				Write(", ");
				Switch(e);
			}
			Write(")");
		}

		public override void OnStatementModifier(StatementModifier sm)
		{
			Write(" ");
			Write(sm.Type.ToString().ToLower());
			Write(" ");
			Switch(sm.Condition);
		}

		public override void OnForStatement(ForStatement fs)
		{
			WriteIndented("for ");
			for (int i=0; i<fs.Declarations.Count; ++i)
			{
				if (i > 0) { Write(", "); }
				Switch(fs.Declarations[i]);
			}
			Write(" in ");
			Switch(fs.Iterator);
			WriteLine(":");
			Indent();
			Switch(fs.Statements);
			Dedent();
		}

		public override void OnIfStatement(IfStatement ifs)
		{
			WriteIndented("if ");
			Switch(ifs.Expression);
			WriteLine(":");
			OnBlock(ifs.TrueBlock);
			if (null != ifs.FalseBlock)
			{
				WriteLine("else:");
				OnBlock(ifs.FalseBlock);
			}
		}

		public override void OnDeclaration(Declaration d)
		{
			Write(d.Name);
			Switch(d.Type);
		}

		public override bool EnterReturnStatement(ReturnStatement r)
		{
			WriteIndented("return ");
			return true;
		}

		public override void LeaveReturnStatement(ReturnStatement r)
		{
			WriteLine();
		}

		public override void OnUnpackStatement(UnpackStatement us)
		{
			WriteIndented();
			for (int i=0; i<us.Declarations.Count; ++i)
			{
				if (i > 0)
				{
					Write(", ");
				}
				Switch(us.Declarations[i]);
			}
			Write(" = ");
			Switch(us.Expression);
			WriteLine();
		}

		#endregion

		void WriteOperator(BinaryOperatorType op)
		{
			switch (op)
			{
				case BinaryOperatorType.Assign:
				{					
					Write(" = ");
					break;
				}

				case BinaryOperatorType.Match:
				{
					Write(" =~ ");
					break;
				}
			}
		}

		void WriteTypeDefinition(string keyword, TypeDefinition td)
		{
			WriteIndented(keyword);
			Write(" ");
			Write(td.Name);
			if (td.BaseTypes.Count > 0)
			{
				Write("(");
				for (int i=0; i<td.BaseTypes.Count; ++i)
				{
					if (i > 0) { Write(", "); }
					Write(td.BaseTypes[i].Name);
				}
				Write(")");
			}
			WriteLine(":");
			Indent();
			Switch(td.Members);
			Dedent();
		}
	}
}
