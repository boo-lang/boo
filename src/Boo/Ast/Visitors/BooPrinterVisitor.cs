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

using System;
using System.IO;
using Boo.Ast;

namespace Boo.Ast.Visitors
{
	/// <summary>
	/// Imprime uma AST boo em boo. til para visualizar a 
	/// expanso de Ast Attributes e mixins.
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

			foreach (TypeMember member in m.Members)
			{
				Switch(member);
				WriteLine();
			}

			// m.Globals iria causar um Indent()
			// invlido
			Switch(m.Globals.Statements);
		}

		public override void OnPackage(Package p)
		{
			WriteLine("package {0}", p.Name);
			WriteLine();
		}

		public override void OnUsing(Using p)
		{
			Write("using {0}", p.Namespace);
			if (null != p.AssemblyReference)
			{
				Write(" from ");
				Write(p.AssemblyReference.Name);
			}
			if (null != p.Alias)
			{
				Write(" as ");
				Write(p.Alias.Name);
			}
			WriteLine();
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
			WriteAttributes(f.Attributes, true);
			WriteIndented(f.Name);
			Switch(f.Type);
			WriteLine();
		}
		
		public override void OnProperty(Property node)
		{
			WriteAttributes(node.Attributes, true);
			WriteIndented(node.Name);
			Switch(node.Type);
			WriteLine(":");
			Indent();
			if (null != node.Getter)
			{
				WriteAttributes(node.Getter.Attributes, true);
				WriteLine("get:");
				OnBlock(node.Getter.Body);
			}
			if (null != node.Setter)
			{
				WriteAttributes(node.Setter.Attributes, true);
				WriteLine("set:");
				OnBlock(node.Setter.Body);
			}
			Dedent();
		}
		
		public override void OnEnumMember(EnumMember node)
		{
			WriteAttributes(node.Attributes, true);
			WriteIndented(node.Name);
			if (null != node.Initializer)
			{
				Write(" = ");
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
			if (m.ReturnTypeAttributes.Count > 0)
			{
				Write(" ");
				WriteAttributes(m.ReturnTypeAttributes, false);
			}
			WriteLine(":");
			OnBlock(m.Body);
		}

		public override void OnParameterDeclaration(ParameterDeclaration p)
		{
			WriteAttributes(p.Attributes, false);
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
		
		public override void OnNullLiteralExpression(NullLiteralExpression node)
		{
			Write("null");
		}
		
		public override void OnSelfLiteralExpression(SelfLiteralExpression node)
		{
			Write("self");
		}
		
		public override void OnTimeSpanLiteralExpression(TimeSpanLiteralExpression node)
		{
			Write(node.Value);
		}
		
		public override void OnBoolLiteralExpression(BoolLiteralExpression node)
		{
			if (node.Value)
			{
				Write("true");
			}
			else
			{
				Write("false");
			}
		}
		
		public override void OnUnaryExpression(UnaryExpression node)
		{
			Write("(");
			Write(GetUnaryOperator(node.Operator));
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
			Write(GetBinaryOperator(e.Operator));
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
			Write(" ? ");
			Switch(node.TrueExpression);
			Write(" : ");
			Switch(node.FalseExpression);
			Write(")");
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
			Write(" for ");
			WriteCommaSeparatedList(node.Declarations);
			Write(" in ");
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
			Write("string.Format('{0}', ", sfe.Template);
			WriteTuple(sfe.Arguments);
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
			Switch(fs.Block);
		}
		
		public override void OnRetryStatement(RetryStatement node)
		{
			WriteIndented("retry");
			WriteLine();
		}
		
		public override void OnTryStatement(TryStatement node)
		{
			WriteIndented("try:");
			WriteLine();
			Switch(node.ProtectedBlock);
			Switch(node.ExceptionHandlers);
			if (null != node.SuccessBlock)
			{
				WriteIndented("success:");
				WriteLine();
				Switch(node.SuccessBlock);
			}
			if (null != node.EnsureBlock)
			{
				WriteIndented("ensure:");
				WriteLine();
				Switch(node.EnsureBlock);
			}
		}
		
		public override void OnExceptionHandler(ExceptionHandler node)
		{
			WriteIndented("catch");
			if (null != node.Declaration)
			{
				Write(" ");
				Switch(node.Declaration);
			}			
			WriteLine(":");
			Switch(node.Block);
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
		
		string GetUnaryOperator(UnaryOperatorType op)
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
					
				case UnaryOperatorType.ArithmeticNegate:
				{
					return "-";
				}
				
				case UnaryOperatorType.Not:
				{
					return "not";
				}
			}
			throw new ArgumentException("op");
		}

		string GetBinaryOperator(BinaryOperatorType op)
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
				
				case BinaryOperatorType.Add:
				{
					return "+";
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
				
				case BinaryOperatorType.GreaterThan:
				{
					return ">";
				}
				
				case BinaryOperatorType.Modulus:
				{
					return "%";
				}
			}
			throw new NotImplementedException(op.ToString());
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
			foreach (Attribute attribute in attributes)
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

		void WriteTypeDefinition(string keyword, TypeDefinition td)
		{
			WriteAttributes(td.Attributes, true);
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
				WriteIndented("pass");
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
