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

namespace Boo.Lang.Ast.Visitors
{
	using System;
	using System.Globalization;
	using System.IO;
	using Boo.Lang.Ast;

	/// <summary>
	/// Imprime uma AST boo em boo.
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
		
		#region overridables
		public virtual void WriteKeyword(string text)
		{
			Write(text);
		}
		
		public virtual void WriteOperator(string text)
		{
			Write(text);
		}
		#endregion
		
		#region IVisitor Members	

		public override void OnModule(Module m)
		{
			Switch(m.Namespace);

			if (m.Imports.Count > 0)
			{
				Switch(m.Imports);
				WriteLine();
			}

			foreach (TypeMember member in m.Members)
			{
				Switch(member);
				WriteLine();
			}

			// m.Globals iria causar um Indent()
			// invlido
			if (null != m.Globals)
			{
				Switch(m.Globals.Statements);
			}
		}

		public override void OnNamespaceDeclaration(NamespaceDeclaration node)
		{
			WriteKeyword("namespace");
			WriteLine(" {0}", node.Name);
			WriteLine();
		}

		public override void OnImport(Import p)
		{
			WriteKeyword("import");
			Write(" {0}", p.Namespace);
			if (null != p.AssemblyReference)
			{
				WriteKeyword(" from ");
				Write(p.AssemblyReference.Name);
			}
			if (null != p.Alias)
			{
				WriteKeyword(" as ");
				Write(p.Alias.Name);
			}
			WriteLine();
		}

		public void WriteBlock(Block b)
		{
			Indent();
			if (0 == b.Statements.Count)
			{
				WriteIndented();
				WriteKeyword("pass");
				WriteLine();
			}
			else
			{
				Switch(b.Statements);
			}
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
			WriteAttributes(f.Attributes, true);
			WriteModifiers(f);
			Write(f.Name);
			WriteTypeReference(f.Type);
			if (null != f.Initializer)
			{
				WriteOperator(" = ");
				Switch(f.Initializer);
			}
			WriteLine();
		}
		
		public override void OnProperty(Property node)
		{
			WriteAttributes(node.Attributes, true);			
			WriteModifiers(node);
			WriteIndented(node.Name);
			if (node.Parameters.Count > 0)
			{
				WriteParameterList(node.Parameters);
			}
			WriteTypeReference(node.Type);
			WriteLine(":");
			Indent();
			if (null != node.Getter)
			{
				WriteAttributes(node.Getter.Attributes, true);
				WriteModifiers(node.Getter);
				WriteKeyword("get");
				WriteLine(":");
				WriteBlock(node.Getter.Body);
			}
			if (null != node.Setter)
			{
				WriteAttributes(node.Setter.Attributes, true);
				WriteModifiers(node.Setter);
				WriteKeyword("set");
				WriteLine(":");
				WriteBlock(node.Setter.Body);
			}
			Dedent();
		}
		
		public override void OnEnumMember(EnumMember node)
		{
			WriteAttributes(node.Attributes, true);
			WriteIndented(node.Name);
			if (null != node.Initializer)
			{
				WriteOperator(" = ");
				Switch(node.Initializer);
			}
			WriteLine();
		}

		public override void OnConstructor(Constructor c)
		{
			OnMethod(c);
		}

		public override void OnMethod(Method m)
		{
			WriteAttributes(m.Attributes, true);
			WriteModifiers(m);
			WriteKeyword("def ");
			Write(m.Name);
			WriteParameterList(m.Parameters);
			WriteTypeReference(m.ReturnType);
			if (m.ReturnTypeAttributes.Count > 0)
			{
				Write(" ");
				WriteAttributes(m.ReturnTypeAttributes, false);
			}
			WriteLine(":");
			WriteBlock(m.Body);
		}
		
		void WriteTypeReference(TypeReference t)
		{
			if (null != t)
			{
				WriteKeyword(" as ");
				t.Switch(this);
			}
		}

		public override void OnParameterDeclaration(ParameterDeclaration p)
		{
			WriteAttributes(p.Attributes, false);
			Write(p.Name);
			WriteTypeReference(p.Type);
		}

		public override void OnSimpleTypeReference(SimpleTypeReference t)
		{				
			Write(t.Name);
		}
		
		public override void OnTupleTypeReference(TupleTypeReference t)
		{
			Write("(");
			Switch(t.ElementType);
			Write(")");
		}

		public override void OnMemberReferenceExpression(MemberReferenceExpression e)
		{
			Switch(e.Target);
			Write(".");
			Write(e.Name);
		}
		
		public override void OnAsExpression(AsExpression e)
		{
			Switch(e.Target);
			WriteTypeReference(e.Type);
		}
		
		public override void OnNullLiteralExpression(NullLiteralExpression node)
		{
			WriteKeyword("null");
		}
		
		public override void OnSelfLiteralExpression(SelfLiteralExpression node)
		{
			WriteKeyword("self");
		}
		
		public override void OnSuperLiteralExpression(SuperLiteralExpression node)
		{
			WriteKeyword("super");
		}
		
		public override void OnTimeSpanLiteralExpression(TimeSpanLiteralExpression node)
		{
			double days = node.Value.TotalDays;
			if (days >= 1)
			{
				Write(days.ToString(CultureInfo.InvariantCulture) + "d");
			}
			else
			{
				double hours = node.Value.TotalHours;
				if (hours >= 1)
				{
					Write(hours.ToString(CultureInfo.InvariantCulture) + "h");
				}
				else
				{
					double minutes = node.Value.TotalMinutes;
					if (minutes >= 1)
					{
						Write(minutes.ToString(CultureInfo.InvariantCulture) + "m");
					}
					else
					{
						double seconds = node.Value.TotalSeconds;
						if (seconds >= 1)
						{
							Write(seconds.ToString(CultureInfo.InvariantCulture) + "s");
						}
						else
						{
							Write(node.Value.TotalMilliseconds.ToString(CultureInfo.InvariantCulture) + "ms");
						}
					}
				}
			}
		}
		
		public override void OnBoolLiteralExpression(BoolLiteralExpression node)
		{
			if (node.Value)
			{
				WriteKeyword("true");
			}
			else
			{
				WriteKeyword("false");
			}
		}
		
		public override void OnUnaryExpression(UnaryExpression node)
		{
			Write("(");
			WriteOperator(GetUnaryOperatorText(node.Operator));
			Switch(node.Operand);
			Write(")");			
		}

		public override void OnBinaryExpression(BinaryExpression e)
		{
			bool needsParens = !(e.ParentNode is ExpressionStatement);
			if (needsParens)
			{
				Write("(");
			}
			Switch(e.Left);
			Write(" ");
			WriteOperator(GetBinaryOperatorText(e.Operator));
			Write(" ");
			Switch(e.Right);
			if (needsParens)
			{
				Write(")");
			}
		}
		
		public override void OnTernaryExpression(TernaryExpression node)
		{			
			Write("(");
			Switch(node.Condition);
			WriteOperator(" ? ");
			Switch(node.TrueExpression);
			WriteOperator(" : ");
			Switch(node.FalseExpression);
			Write(")");
		}

		public override bool EnterRaiseStatement(RaiseStatement rs)
		{
			WriteIndented();
			WriteKeyword("raise ");
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
			WriteCommaSeparatedList(e.Arguments);
			if (e.NamedArguments.Count > 0)
			{
				if (e.Arguments.Count > 0)
				{
					Write(", ");
				}
				WriteCommaSeparatedList(e.NamedArguments);
			}			
			Write(")");
		}
		
		public override void OnTupleLiteralExpression(TupleLiteralExpression node)
		{
			WriteTuple(node.Items);
		}
		
		public override void OnListLiteralExpression(ListLiteralExpression node)
		{			
			Write("[");
			WriteCommaSeparatedList(node.Items);
			Write("]");
		}
		
		public override void OnIteratorExpression(IteratorExpression node)
		{			
			Write("(");
			Switch(node.Expression);
			WriteKeyword(" for ");
			WriteCommaSeparatedList(node.Declarations);
			WriteKeyword(" in ");
			Switch(node.Iterator);
			Switch(node.Filter);
			Write(")");
		}

		public override void OnSlicingExpression(SlicingExpression node)
		{
			Switch(node.Target);
			Write("[");
			Switch(node.Begin);
			if (null != node.End || WasOmitted(node.Begin))
			{
				Write(":");
			}			
			Switch(node.End);			
			if (null != node.Step)
			{
				Write(":");
				Switch(node.Step);
			}			
			Write("]");
		}
		
		public override void OnHashLiteralExpression(HashLiteralExpression node)
		{			
			Write("{");
			if (node.Items.Count > 0)
			{
				Write(" ");
				WriteCommaSeparatedList(node.Items);
				Write(" ");
			}
			Write("}");
		}

		public override void OnExpressionPair(ExpressionPair pair)
		{
			Switch(pair.First);
			Write(": ");
			Switch(pair.Second);
		}

		public override void OnStringLiteralExpression(StringLiteralExpression e)
		{			
			WriteStringLiteral(e.Value);			
		}

		public override void OnIntegerLiteralExpression(IntegerLiteralExpression e)
		{
			Write(e.Value.ToString());
			if (e.IsLong)
			{
				Write("L");
			}
		}
		
		public override void OnDoubleLiteralExpression(DoubleLiteralExpression e)
		{
			Write(e.Value.ToString("########0.0##########", CultureInfo.InvariantCulture));
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
			Write("string.Format(");
			WriteStringLiteral(sfe.Template);
			Write(", ");
			WriteTuple(sfe.Arguments);
			Write(")");
		}

		public override void OnStatementModifier(StatementModifier sm)
		{
			Write(" ");
			WriteKeyword(sm.Type.ToString().ToLower());
			Write(" ");
			Switch(sm.Condition);
		}
		
		public override void OnMacroStatement(MacroStatement node)
		{
			WriteIndented(node.Name);
			Write(" ");
			WriteCommaSeparatedList(node.Arguments);
			WriteLine(":");
			WriteBlock(node.Block);
		}
		
		public override void OnForStatement(ForStatement fs)
		{
			WriteIndented();
			WriteKeyword("for ");
			for (int i=0; i<fs.Declarations.Count; ++i)
			{
				if (i > 0) { Write(", "); }
				Switch(fs.Declarations[i]);
			}
			WriteKeyword(" in ");
			Switch(fs.Iterator);
			WriteLine(":");
			WriteBlock(fs.Block);
		}
		
		public override void OnRetryStatement(RetryStatement node)
		{
			WriteIndented();
			WriteKeyword("retry");
			WriteLine();
		}
		
		public override void OnTryStatement(TryStatement node)
		{
			WriteIndented();
			WriteKeyword("try:");
			WriteLine();
			WriteBlock(node.ProtectedBlock);
			Switch(node.ExceptionHandlers);
			if (null != node.SuccessBlock)
			{
				WriteIndented();
				WriteKeyword("success:");
				WriteLine();
				WriteBlock(node.SuccessBlock);
			}
			if (null != node.EnsureBlock)
			{
				WriteIndented();
				WriteKeyword("ensure:");
				WriteLine();
				WriteBlock(node.EnsureBlock);
			}
		}
		
		public override void OnExceptionHandler(ExceptionHandler node)
		{
			WriteIndented();
			WriteKeyword("except");
			if (null != node.Declaration)
			{
				Write(" ");
				Switch(node.Declaration);
			}			
			WriteLine(":");
			WriteBlock(node.Block);
		}
		
		public override void OnUnlessStatement(UnlessStatement node)
		{
			WriteConditionalBlock("unless", node.Condition, node.Block);
		}
		
		public override void OnWhileStatement(WhileStatement node)
		{
			WriteConditionalBlock("while", node.Condition, node.Block);
		}

		public override void OnIfStatement(IfStatement ifs)
		{
			WriteIndented();
			WriteKeyword("if ");
			Switch(ifs.Condition);
			WriteLine(":");
			WriteBlock(ifs.TrueBlock);
			if (null != ifs.FalseBlock)
			{				
				WriteKeyword("else:");
				WriteLine();
				WriteBlock(ifs.FalseBlock);
			}
		}
		
		public override bool EnterDeclarationStatement(DeclarationStatement node)
		{
			WriteIndented();
			return true;
		}
		
		public override void LeaveDeclarationStatement(DeclarationStatement node)
		{
			WriteLine();
		}

		public override void OnDeclaration(Declaration d)
		{
			Write(d.Name);
			WriteTypeReference(d.Type);
		}

		public override bool EnterReturnStatement(ReturnStatement r)
		{
			WriteIndented();
			WriteKeyword("return ");
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
			WriteOperator(" = ");
			Switch(us.Expression);
			WriteLine();
		}

		#endregion
		
		public static string GetUnaryOperatorText(UnaryOperatorType op)
		{
			switch (op)
			{
				case UnaryOperatorType.Increment:
				{
					return "++";
				}
					
				case UnaryOperatorType.Decrement:
				{
					return "--";
				}
					
				case UnaryOperatorType.UnaryNegation:
				{
					return "-";
				}
				
				case UnaryOperatorType.LogicalNot:
				{
					return "not ";
				}
			}
			throw new ArgumentException("op");
		}

		public static string GetBinaryOperatorText(BinaryOperatorType op)
		{
			switch (op)
			{
				case BinaryOperatorType.Assign:
				{					
					return "=";
				}

				case BinaryOperatorType.Match:
				{
					return "=~";
				}
				
				case BinaryOperatorType.Equality:
				{
					return "==";
				}
				
				case BinaryOperatorType.Inequality:
				{
					return "!=";
				}
				
				case BinaryOperatorType.Addition:
				{
					return "+";
				}
				
				case BinaryOperatorType.Exponentiation:
				{
					return "**";
				}
				
				case BinaryOperatorType.InPlaceAdd:
				{
					return "+=";
				}
				
				case BinaryOperatorType.InPlaceSubtract:
				{
					return "-=";
				}
				
				case BinaryOperatorType.InPlaceMultiply:
				{
					return "*=";
				}
				
				case BinaryOperatorType.InPlaceDivide:
				{
					return "/=";
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
				
				case BinaryOperatorType.GreaterThan:
				{
					return ">";
				}
				
				case BinaryOperatorType.GreaterThanOrEqual:
				{
					return ">=";
				}
				
				case BinaryOperatorType.LessThan:
				{
					return "<";
				}
				
				case BinaryOperatorType.LessThanOrEqual:
				{
					return "<=";
				}
				
				case BinaryOperatorType.Modulus:
				{
					return "%";
				}
				
				case BinaryOperatorType.Member:
				{
					return "in";
				}
				
				case BinaryOperatorType.NotMember:
				{
					return "not in";
				}
				
				case BinaryOperatorType.ReferenceEquality:
				{
					return "is";
				}
				
				case BinaryOperatorType.ReferenceInequality:
				{
					return "is not";
				}
				
				case BinaryOperatorType.TypeTest:
				{
					return "isa";
				}
				
				case BinaryOperatorType.Or:
				{
					return "or";
				}
				
				case BinaryOperatorType.And:
				{
					return "and";
				}
				
				case BinaryOperatorType.BitwiseOr:
				{
					return "|";
				}
			}
			throw new NotImplementedException(op.ToString());
		}
		
		public virtual void WriteStringLiteral(string text)
		{
			WriteStringLiteral(text, _writer);
		}
		
		public static void WriteStringLiteral(string text, TextWriter writer)
		{
			writer.Write("'");
			foreach (char ch in text)
			{
				switch (ch)
				{
					case '\r':
					{
						writer.Write("\\r");						
						break;
					}
					
					case '\n':
					{
						writer.Write("\\n");
						break;
					}
					
					case '\t':
					{
						writer.Write("\\t");
						break;
					}
					
					case '\\':
					{
						writer.Write("\\\\");
						break;
					}
					
					default:
					{
						writer.Write(ch);
						break;
					}
				}				
			}
			writer.Write("'");
		}
		
		void WriteConditionalBlock(string keyword, Expression condition, Block block)
		{
			WriteIndented();
			WriteKeyword(keyword + " ");
			Switch(condition);
			WriteLine(":");
			WriteBlock(block);
		}
		
		void WriteParameterList(ParameterDeclarationCollection items)
		{
			Write("(");
			WriteCommaSeparatedList(items);
			Write(")");
		}
		
		void WriteCommaSeparatedList(NodeCollection items)
		{			
			for (int i=0; i<items.Count; ++i)
			{
				if (i > 0)
				{
					Write(", ");
				}
				Switch(items.GetNodeAt(i));
			}
		}
		
		void WriteTuple(ExpressionCollection items)
		{
			Write("(");
			if (items.Count > 1)
			{
				for (int i=0; i<items.Count; ++i)
				{
					if (i>0)
					{
						Write(", ");
					}
					Switch(items[i]);
				}
			}
			else
			{
				if (items.Count > 0)
				{
					Switch(items[0]);
				}
				Write(",");
			}
			Write(")");
		}
		
		void WriteAttributes(AttributeCollection attributes, bool addNewLines)
		{
			foreach (Boo.Lang.Ast.Attribute attribute in attributes)
			{
				WriteIndented("[");
				Write(attribute.Name);
				if (attribute.Arguments.Count > 0 ||
				    attribute.NamedArguments.Count > 0)
				{
					Write("(");
					WriteCommaSeparatedList(attribute.Arguments);
					if (attribute.NamedArguments.Count > 0)
					{
						if (attribute.Arguments.Count > 0)
						{
							Write(", ");
						}
						WriteCommaSeparatedList(attribute.NamedArguments);
					}
					Write(")");
				}
				Write("]");
				if (addNewLines)
				{
					WriteLine();
				}
				else
				{
					Write(" ");
				}
			}			
		}
		
		void WriteModifiers(TypeMember member)
		{
			WriteIndented();
			if (member.IsPublic)
			{
				WriteKeyword("public ");
			}
			else if (member.IsProtected)
			{
				WriteKeyword("protected ");
			}
			else if (member.IsPrivate)
			{
				WriteKeyword("private ");
			}
			else if (member.IsInternal)
			{
				WriteKeyword("internal ");
			}
			if (member.IsStatic)
			{
				WriteKeyword("static ");
			}
			else if (member.IsModifierSet(TypeMemberModifiers.Override))
			{
				WriteKeyword("override ");
			}
			if (member.IsFinal)
			{
				WriteKeyword("final ");
			}
			if (member.IsTransient)
			{
				WriteKeyword("transient ");
			}
		}

		void WriteTypeDefinition(string keyword, TypeDefinition td)
		{
			WriteAttributes(td.Attributes, true);
			WriteModifiers(td);
			WriteIndented();
			WriteKeyword(keyword);
			Write(" ");
			Write(td.Name);
			if (td.BaseTypes.Count > 0)
			{
				Write("(");
				for (int i=0; i<td.BaseTypes.Count; ++i)
				{
					if (i > 0) { Write(", "); }
					Write(((SimpleTypeReference)td.BaseTypes[i]).Name);
				}
				Write(")");
			}
			WriteLine(":");
			Indent();
			if (td.Members.Count > 0)
			{				
				foreach (TypeMember member in td.Members)
				{
					WriteLine();
					Switch(member);
				}
			}
			else
			{
				WriteIndented();
				WriteKeyword("pass");
				WriteLine();
			}
			Dedent();
		}
		
		bool WasOmitted(Expression node)
		{
			return null != node &&
				NodeType.OmittedExpression == node.NodeType;
		}
	}
}
