#region license
// Copyright (c) 2004, Rodrigo B. de Oliveira (rbo@acm.org)
// All rights reserved.
// 
// Redistribution and use in source and binary forms, with or without modification,
// are permitted provided that the following conditions are met:
// 
//     * Redistributions of source code must retain the above copyright notice,
//     this list of conditions and the following disclaimer.
//     * Redistributions in binary form must reproduce the above copyright notice,
//     this list of conditions and the following disclaimer in the documentation
//     and/or other materials provided with the distribution.
//     * Neither the name of Rodrigo B. de Oliveira nor the names of its
//     contributors may be used to endorse or promote products derived from this
//     software without specific prior written permission.
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
// ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
// DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE
// FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
// DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
// SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER
// CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
// OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF
// THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
#endregion

namespace Boo.Lang.Compiler.Ast.Visitors
{
	using System;
	using System.Text.RegularExpressions;
	using System.Globalization;
	using System.IO;
	using Boo.Lang.Compiler.Ast;

	/// <summary>
	/// Imprime uma AST boo em boo.
	/// </summary>
	public class BooPrinterVisitor : TextEmitter
	{		
		static Regex _identifierRE = new Regex("^[a-zA-Z.]+$");
		
		static Regex _extendedRE = new Regex(@"\s");
		
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

		override public void OnModule(Module m)
		{
			Visit(m.Namespace);

			if (m.Imports.Count > 0)
			{
				Visit(m.Imports);
				WriteLine();
			}

			foreach (TypeMember member in m.Members)
			{
				Visit(member);
				WriteLine();
			}

			// m.Globals iria causar um Indent()
			// invlido
			if (null != m.Globals)
			{
				Visit(m.Globals.Statements);
			}
		}

		override public void OnNamespaceDeclaration(NamespaceDeclaration node)
		{
			WriteKeyword("namespace");
			WriteLine(" {0}", node.Name);
			WriteLine();
		}
		
		bool IsExtendedRE(string s)
		{
			return _extendedRE.IsMatch(s);
		}
		
		bool IsSimpleIdentifier(string s)
		{
			return _identifierRE.IsMatch(s);
		}

		override public void OnImport(Import p)
		{
			WriteKeyword("import");
			Write(" {0}", p.Namespace);
			if (null != p.AssemblyReference)
			{
				WriteKeyword(" from ");

				string assemblyName = p.AssemblyReference.Name;
				if (IsSimpleIdentifier(assemblyName))
				{
					Write(assemblyName);
				}
				else
				{
					WriteStringLiteral(assemblyName);
				}
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
				Visit(b.Statements);
			}
			Dedent();
		}
		
		override public void OnClassDefinition(ClassDefinition c)
		{
			WriteTypeDefinition("class", c);
		}

		override public void OnInterfaceDefinition(InterfaceDefinition id)
		{
			WriteTypeDefinition("interface", id);
		}

		override public void OnEnumDefinition(EnumDefinition ed)
		{
			WriteTypeDefinition("enum", ed);
		}
		
		override public void OnEvent(Event node)
		{
			WriteAttributes(node.Attributes, true);
			WriteModifiers(node);
			WriteKeyword("event ");
			Write(node.Name);
			WriteTypeReference(node.Type);
			WriteLine();
		}

		override public void OnField(Field f)
		{
			WriteAttributes(f.Attributes, true);
			WriteModifiers(f);
			Write(f.Name);
			WriteTypeReference(f.Type);
			if (null != f.Initializer)
			{
				WriteOperator(" = ");
				Visit(f.Initializer);
			}
			WriteLine();
		}
		
		override public void OnProperty(Property node)
		{
			WriteAttributes(node.Attributes, true);			
			WriteModifiers(node);
			WriteIndented(node.Name);
			if (node.Parameters.Count > 0)
			{
				WriteParameterList(node.Parameters, false);
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
		
		override public void OnEnumMember(EnumMember node)
		{
			WriteAttributes(node.Attributes, true);
			WriteIndented(node.Name);
			if (null != node.Initializer)
			{
				WriteOperator(" = ");
				Visit(node.Initializer);
			}
			WriteLine();
		}

		override public void OnConstructor(Constructor c)
		{
			OnMethod(c);
		}
		
		bool IsSimpleClosure(CallableBlockExpression node)
		{
			if (1 == node.Body.Statements.Count)
			{
				switch (node.Body.Statements[0].NodeType)
				{
					case NodeType.IfStatement:
					{
						return false;
					}
					
					case NodeType.WhileStatement:
					{
						return false;
					}
					
					case NodeType.ForStatement:
					{
						return false;
					}
				}
				return true;
			}
			return false;
		}
		
		override public void OnCallableBlockExpression(CallableBlockExpression node)
		{
			if (IsSimpleClosure(node))
			{
				DisableNewLine();
				Write("{ ");
				if (node.Parameters.Count > 0)
				{
					WriteCommaSeparatedList(node.Parameters);
					Write(" | ");
				}				
				Visit(node.Body.Statements);
				Write(" }");
				EnableNewLine();
			}			
			else
			{
				WriteKeyword("def ");
				WriteParameterList(node.Parameters, false);
				WriteTypeReference(node.ReturnType);
				WriteLine(":");
				WriteBlock(node.Body);
			}
		}
		
		void WriteCallableDefinitionHeader(string keyword, CallableDefinition node)
		{
			WriteAttributes(node.Attributes, true);
			WriteModifiers(node);
			WriteKeyword(keyword);
			Write(node.Name);
			WriteParameterList(node.Parameters, node.VariableArguments);
			WriteTypeReference(node.ReturnType);
			if (node.ReturnTypeAttributes.Count > 0)
			{
				Write(" ");
				WriteAttributes(node.ReturnTypeAttributes, false);
			}
		}
		
		override public void OnCallableDefinition(CallableDefinition node)
		{
			WriteCallableDefinitionHeader("callable ", node);
		}

		override public void OnMethod(Method m)
		{
			WriteCallableDefinitionHeader("def ", m);
			WriteLine(":");
			WriteBlock(m.Body);
		}
		
		void WriteTypeReference(TypeReference t)
		{
			if (null != t)
			{
				WriteKeyword(" as ");
				Visit(t);
			}
		}

		override public void OnParameterDeclaration(ParameterDeclaration p)
		{
			WriteAttributes(p.Attributes, false);
			Write(p.Name);
			WriteTypeReference(p.Type);
		}

		override public void OnSimpleTypeReference(SimpleTypeReference t)
		{				
			Write(t.Name);
		}
		
		override public void OnArrayTypeReference(ArrayTypeReference t)
		{
			Write("(");
			Visit(t.ElementType);
			Write(")");
		}
		
		override public void OnCallableTypeReference(CallableTypeReference node)
		{
			Write("callable(");
			WriteCommaSeparatedList(node.Parameters);
			Write(")");
			WriteTypeReference(node.ReturnType);
		}

		override public void OnMemberReferenceExpression(MemberReferenceExpression e)
		{
			Visit(e.Target);
			Write(".");
			Write(e.Name);
		}
		
		override public void OnAsExpression(AsExpression e)
		{
			Write("(");
			Visit(e.Target);
			WriteTypeReference(e.Type);
			Write(")");
		}
		
		override public void OnCastExpression(CastExpression node)
		{
			WriteKeyword("cast");
			Write("(");
			Visit(node.Type);
			Write(", ");
			Visit(node.Target);
			Write(")");
		}
		
		override public void OnNullLiteralExpression(NullLiteralExpression node)
		{
			WriteKeyword("null");
		}
		
		override public void OnSelfLiteralExpression(SelfLiteralExpression node)
		{
			WriteKeyword("self");
		}
		
		override public void OnSuperLiteralExpression(SuperLiteralExpression node)
		{
			WriteKeyword("super");
		}
		
		override public void OnTimeSpanLiteralExpression(TimeSpanLiteralExpression node)
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
		
		override public void OnBoolLiteralExpression(BoolLiteralExpression node)
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
		
		override public void OnUnaryExpression(UnaryExpression node)
		{
			Write("(");
			WriteOperator(GetUnaryOperatorText(node.Operator));
			Visit(node.Operand);
			Write(")");			
		}

		override public void OnBinaryExpression(BinaryExpression e)
		{
			bool needsParens = !(e.ParentNode is ExpressionStatement);
			if (needsParens)
			{
				Write("(");
			}
			Visit(e.Left);
			Write(" ");
			WriteOperator(GetBinaryOperatorText(e.Operator));
			Write(" ");
			Visit(e.Right);
			if (needsParens)
			{
				Write(")");
			}
		}

		override public void OnRaiseStatement(RaiseStatement rs)
		{
			WriteIndented();
			WriteKeyword("raise ");
			Visit(rs.Exception);
			Visit(rs.Modifier);
			WriteLine();
		}

		override public void OnMethodInvocationExpression(MethodInvocationExpression e)
		{
			Visit(e.Target);
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
		
		override public void OnArrayLiteralExpression(ArrayLiteralExpression node)
		{
			WriteArray(node.Items);
		}
		
		override public void OnListLiteralExpression(ListLiteralExpression node)
		{			
			Write("[");
			WriteCommaSeparatedList(node.Items);
			Write("]");
		}
		
		override public void OnGeneratorExpression(GeneratorExpression node)
		{			
			Write("(");
			Visit(node.Expression);
			WriteKeyword(" for ");
			WriteCommaSeparatedList(node.Declarations);
			WriteKeyword(" in ");
			Visit(node.Iterator);
			Visit(node.Filter);
			Write(")");
		}

		override public void OnSlicingExpression(SlicingExpression node)
		{
			Visit(node.Target);
			Write("[");
			Visit(node.Begin);
			if (null != node.End || WasOmitted(node.Begin))
			{
				Write(":");
			}			
			Visit(node.End);			
			if (null != node.Step)
			{
				Write(":");
				Visit(node.Step);
			}			
			Write("]");
		}
		
		override public void OnHashLiteralExpression(HashLiteralExpression node)
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

		override public void OnExpressionPair(ExpressionPair pair)
		{
			Visit(pair.First);
			Write(": ");
			Visit(pair.Second);
		}
		
		override public void OnRELiteralExpression(RELiteralExpression e)
		{			
			if (IsExtendedRE(e.Value))
			{
				Write("@");
			}
			Write(e.Value);
		}

		override public void OnStringLiteralExpression(StringLiteralExpression e)
		{			
			WriteStringLiteral(e.Value);			
		}

		override public void OnIntegerLiteralExpression(IntegerLiteralExpression e)
		{
			Write(e.Value.ToString());
			if (e.IsLong)
			{
				Write("L");
			}
		}
		
		override public void OnDoubleLiteralExpression(DoubleLiteralExpression e)
		{
			Write(e.Value.ToString("########0.0##########", CultureInfo.InvariantCulture));
		}

		override public void OnReferenceExpression(ReferenceExpression node)
		{
			Write(node.Name);
		}

		override public void OnExpressionStatement(ExpressionStatement node)
		{
			WriteIndented();
			Visit(node.Expression);
			Visit(node.Modifier);
			WriteLine();
		}

		override public void OnExpressionInterpolationExpression(ExpressionInterpolationExpression node)
		{
			Write("\"");
			foreach (Expression arg in node.Expressions)
			{	
				StringLiteralExpression s = arg as StringLiteralExpression;
				if (null == s)
				{
					Write("${");
					Visit(arg);
					Write("}");
				}
				else
				{
					WriteStringLiteralContents(s.Value, _writer);
				}
			}
			Write("\"");
		}

		override public void OnStatementModifier(StatementModifier sm)
		{
			Write(" ");
			WriteKeyword(sm.Type.ToString().ToLower());
			Write(" ");
			Visit(sm.Condition);
		}
		
		override public void OnLabelStatement(LabelStatement node)
		{
			Write(":");
			WriteLine(node.Name);
		}
		
		override public void OnGotoStatement(GotoStatement node)
		{
			Write("goto ");
			Visit(node.Label);
			Visit(node.Modifier);
			WriteLine();
		}
		
		override public void OnMacroStatement(MacroStatement node)
		{
			WriteIndented(node.Name);
			Write(" ");
			WriteCommaSeparatedList(node.Arguments);
			if (node.Block.Statements.Count > 0)
			{
				WriteLine(":");
				WriteBlock(node.Block);
			}
			else
			{
				WriteLine();
			}
		}
		
		override public void OnForStatement(ForStatement fs)
		{
			WriteIndented();
			WriteKeyword("for ");
			for (int i=0; i<fs.Declarations.Count; ++i)
			{
				if (i > 0) { Write(", "); }
				Visit(fs.Declarations[i]);
			}
			WriteKeyword(" in ");
			Visit(fs.Iterator);
			WriteLine(":");
			WriteBlock(fs.Block);
		}
		
		override public void OnRetryStatement(RetryStatement node)
		{
			WriteIndented();
			WriteKeyword("retry");
			WriteLine();
		}
		
		override public void OnTryStatement(TryStatement node)
		{
			WriteIndented();
			WriteKeyword("try:");
			WriteLine();
			WriteBlock(node.ProtectedBlock);
			Visit(node.ExceptionHandlers);
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
		
		override public void OnExceptionHandler(ExceptionHandler node)
		{
			WriteIndented();
			WriteKeyword("except");
			if (null != node.Declaration)
			{
				Write(" ");
				Visit(node.Declaration);
			}			
			WriteLine(":");
			WriteBlock(node.Block);
		}
		
		override public void OnUnlessStatement(UnlessStatement node)
		{
			WriteConditionalBlock("unless", node.Condition, node.Block);
		}
		
		override public void OnBreakStatement(BreakStatement node)
		{
			WriteIndented();
			WriteKeyword("break ");
			Visit(node.Modifier);
			WriteLine();
		}
		
		override public void OnYieldStatement(YieldStatement node)
		{
			WriteIndented();
			WriteKeyword("yield ");
			Visit(node.Expression);
			Visit(node.Modifier);
			WriteLine();
		}
		
		override public void OnWhileStatement(WhileStatement node)
		{
			WriteConditionalBlock("while", node.Condition, node.Block);
		}

		override public void OnIfStatement(IfStatement ifs)
		{
			WriteIndented();
			WriteKeyword("if ");
			Visit(ifs.Condition);
			WriteLine(":");
			WriteBlock(ifs.TrueBlock);
			if (null != ifs.FalseBlock)
			{			
				WriteIndented();
				WriteKeyword("else:");
				WriteLine();
				WriteBlock(ifs.FalseBlock);
			}
		}
		
		override public bool EnterDeclarationStatement(DeclarationStatement node)
		{
			WriteIndented();
			return true;
		}
		
		override public void LeaveDeclarationStatement(DeclarationStatement node)
		{
			WriteLine();
		}

		override public void OnDeclaration(Declaration d)
		{
			Write(d.Name);
			WriteTypeReference(d.Type);
		}

		override public void OnReturnStatement(ReturnStatement r)
		{
			WriteIndented();
			WriteKeyword("return ");
			Visit(r.Expression);
			Visit(r.Modifier);
			WriteLine();
		}

		override public void OnUnpackStatement(UnpackStatement us)
		{
			WriteIndented();
			for (int i=0; i<us.Declarations.Count; ++i)
			{
				if (i > 0)
				{
					Write(", ");
				}
				Visit(us.Declarations[i]);
			}
			WriteOperator(" = ");
			Visit(us.Expression);
			Visit(us.Modifier);
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
			WriteStringLiteralContents(text, writer);
			writer.Write("'");
		}
		
		public static void WriteStringLiteralContents(string text, TextWriter writer)
		{
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
		}
		
		void WriteConditionalBlock(string keyword, Expression condition, Block block)
		{
			WriteIndented();
			WriteKeyword(keyword + " ");
			Visit(condition);
			WriteLine(":");
			WriteBlock(block);
		}
		
		void WriteParameterList(ParameterDeclarationCollection items, bool variableArguments)
		{
			Write("(");
			int last = items.Count-1;
			for (int i=0; i<items.Count; ++i)
			{
				if (i > 0)
				{
					Write(", ");					
				}
				if (variableArguments && i==last)
				{
					Write("*");
				}
				Visit(items.GetNodeAt(i));
			}
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
				Visit(items.GetNodeAt(i));
			}
		}
		
		void WriteArray(ExpressionCollection items)
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
					Visit(items[i]);
				}
			}
			else
			{
				if (items.Count > 0)
				{
					Visit(items[0]);
				}
				Write(",");
			}
			Write(")");
		}
		
		void WriteAttributes(AttributeCollection attributes, bool addNewLines)
		{
			foreach (Boo.Lang.Compiler.Ast.Attribute attribute in attributes)
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
			else if (member.IsModifierSet(TypeMemberModifiers.Virtual))
			{
				WriteKeyword("virtual ");
			}
			else if (member.IsModifierSet(TypeMemberModifiers.Abstract))
			{
				WriteKeyword("abstract ");
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
					Visit(member);
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
