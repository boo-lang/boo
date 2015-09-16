namespace Boo.Lang.ParserV4
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
	using Antlr4.Runtime;
	using Antlr4.Runtime.Tree;
	using Boo.Lang.Compiler;
	using Boo.Lang.Compiler.Ast;
	using Boo.Lang.Environments;
	using DocStringFormatter = Boo.Lang.Parser.DocStringFormatter;
	using CodeFactory = Boo.Lang.Parser.CodeFactory;
	using OperatorParser = Boo.Lang.Parser.OperatorParser;
	using PrimitiveParser = Boo.Lang.Parser.PrimitiveParser;

	internal class BooParserAstBuilderListener : AbstractParseTreeVisitor<Node>, IBooParserVisitor<Node>
	{
		private readonly CompileUnit _compileUnit;
		private readonly Module _module;
		private readonly string _filename;
		private readonly StringBuilder _sbuilder = new StringBuilder();

		private readonly ParseTreeProperty<MacroStatement> _macroStatement = new ParseTreeProperty<MacroStatement>();
		private readonly ParseTreeProperty<Node> _node = new ParseTreeProperty<Node>();
		private readonly ParseTreeProperty<Import> _import = new ParseTreeProperty<Import>();

		public BooParserAstBuilderListener(CompileUnit compileUnit, string filename)
		{
			if (compileUnit == null)
				throw new ArgumentNullException("compileUnit");

			_compileUnit = compileUnit;
			_module = new Module();
			_filename = filename;
		}

		private string FileName
		{
			get
			{
				return _filename;
			}
		}

		private LexicalInfo GetLexicalInfo(IToken st)
		{
			return new LexicalInfo(FileName, st.Line, st.Column);
		}

		private LexicalInfo GetLexicalInfo(ParserRuleContext ctx)
		{
			IToken st = ctx.Start;
			return new LexicalInfo(FileName, st.Line, st.Column);
		}

		private LexicalInfo GetLexicalInfo(ITerminalNode tn)
		{
			IToken sym = tn.Symbol;
			return new LexicalInfo(FileName, sym.Line, sym.Column);
		}

		private void SetEndSourceLocation(Node node, ITerminalNode token)
		{
			node.EndSourceLocation = Boo.Lang.Parser.SourceLocationFactory.ToEndSourceLocation(token.Symbol);
		}

		private MemberReferenceExpression MemberReferenceForToken(Expression target, ITerminalNode memberName)
		{
			MemberReferenceExpression mre = new MemberReferenceExpression(GetLexicalInfo(memberName));
			mre.Target = target;
			mre.Name = memberName.GetText();
			return mre;
		}

		private bool OutsideCompilationEnvironment()
		{
			return ActiveEnvironment.Instance == null;
		}

		private void EmitWarning(CompilerWarning warning)
		{
			My<CompilerWarningCollection>.Instance.Add(warning);
		}

		protected virtual void EmitTransientKeywordDeprecationWarning(LexicalInfo location)
		{
			if (OutsideCompilationEnvironment())
				return;
			EmitWarning(
				CompilerWarningFactory.ObsoleteSyntax(
					location,
					"transient keyword",
					"[Transient] attribute"));
		}

		private void EmitError(CompilerError error)
		{
			My<CompilerErrorCollection>.Instance.Add(error);
		}

		protected virtual void EmitParamAfterVarargsError(LexicalInfo location)
		{
			if (OutsideCompilationEnvironment())
				return;
			EmitError(
				CompilerErrorFactory.GenericParserError(
					location,
					new Exception("No more args are allowed after an exploded arg")));
		}

		private string FormatPropertyWithDelimiters(Property deprecated, string leftDelimiter, string rightDelimiter)
		{
			return deprecated.Name + leftDelimiter + Builtins.join(deprecated.Parameters, ", ") + rightDelimiter;
		}

		protected virtual void EmitIndexedPropertyDeprecationWarning(Property deprecated)
		{
			if (OutsideCompilationEnvironment())
				return;
			EmitWarning(
				CompilerWarningFactory.ObsoleteSyntax(
					deprecated,
					FormatPropertyWithDelimiters(deprecated, "(", ")"),
					FormatPropertyWithDelimiters(deprecated, "[", "]")));
		}

		public Module VisitStart(BooParser.StartContext context)
		{
			_module.LexicalInfo = new LexicalInfo(FileName, 1, 1);
			_compileUnit.Modules.Add(_module);

			VisitChildren(context);

			SetEndSourceLocation(_module, context.Eof());

			return _module;
		}

		Node IBooParserVisitor<Node>.VisitStart(BooParser.StartContext context)
		{
			return VisitStart(context);
		}

		public void VisitParse_module(BooParser.Parse_moduleContext context, Module m)
		{
			CheckDocumentation(m, context.docstring());
			if (context.namespace_directive() != null)
				m.Namespace = VisitNamespace_directive(context.namespace_directive());
			if (context.import_directive() != null)
				foreach (var imp in context.import_directive())
					m.Imports.Add(VisitImport_directive(imp));
			if (context.type_member() != null)
				foreach (var tm in context.type_member())
					m.Members.Add(VisitType_member(tm));
			if (context.module_macro() != null)
				foreach (var mm in context.module_macro())
					VisitModule_macro(mm, m);
			if (context.globals() != null)
				VisitGlobals(context.globals(), m);
			if (context.assembly_attribute() != null)
				foreach (var aa in context.assembly_attribute())
					VisitAssembly_attribute(aa, m);
			if (context.module_attribute() != null)
				foreach (var ma in context.module_attribute())
					VisitModule_attribute(ma, m);
		}

		Node IBooParserVisitor<Node>.VisitParse_module(BooParser.Parse_moduleContext context)
		{
			VisitParse_module(context, _module);
			return null;
		}

		public void VisitModule_macro(BooParser.Module_macroContext context, Module m)
		{
			MacroStatement macroStatement = VisitMacro_stmt(context.macro_stmt());
			m.Globals.Add(macroStatement);
		}

		Node IBooParserVisitor<Node>.VisitModule_macro(BooParser.Module_macroContext context)
		{
			VisitModule_macro(context, _module);
			return null;
		}

		public void VisitDocstring(BooParser.DocstringContext context)
		{
			Node node = _node.Get(context);
			if (node == null)
				throw new InvalidOperationException();

			VisitChildren(context);

			var tqs = context.TRIPLE_QUOTED_STRING();
			node.Documentation = DocStringFormatter.Format(TqsUnquote(tqs.GetText()));
		}

		Node IBooParserVisitor<Node>.VisitDocstring(BooParser.DocstringContext context)
		{
			throw new NotImplementedException("Should not see this");
		}

		void CheckDocumentation(Node node, BooParser.DocstringContext context)
		{
			if (context != null && context.ChildCount > 0)
			{
				_node.Put(context, node);
				VisitDocstring(context);
			}
		}

		public void VisitEos(BooParser.EosContext context)
		{
			VisitChildren(context);
		}

		Node IBooParserVisitor<Node>.VisitEos(BooParser.EosContext context)
		{
			VisitEos(context);
			return null;
		}

		Import VisitImport_directive(BooParser.Import_directiveContext context)
		{
			Import node = null;
			if (context.import_directive_() != null)
				node = VisitImport_directive_(context.import_directive_());
			else node = VisitImport_directive_from_(context.import_directive_from_());

			return node;
		}

		Node IBooParserVisitor<Node>.VisitImport_directive(BooParser.Import_directiveContext context)
		{
			return VisitImport_directive(context);
		}

		public Expression VisitNamespace_expression(BooParser.Namespace_expressionContext context)
		{
			Expression result = VisitIdentifier_expression(context.identifier_expression());
			if (context.expression_list() != null)
			{
				var mie = new MethodInvocationExpression(result.LexicalInfo, result);
				foreach (var expr in context.expression_list().expression())
					mie.Arguments.Add((Expression)Visit(expr));
				result = mie;
			}
			return result;
		}

		Node IBooParserVisitor<Node>.VisitNamespace_expression(BooParser.Namespace_expressionContext context)
		{
			return VisitNamespace_expression(context);
		}

		Expression VisitIdentifier_expression(BooParser.Identifier_expressionContext context)
		{
			return new ReferenceExpression(GetLexicalInfo(context.identifier().Start), VisitIdentifier(context.identifier()));
		}

		Node IBooParserVisitor<Node>.VisitIdentifier_expression(BooParser.Identifier_expressionContext context)
		{
			return VisitIdentifier_expression(context);
		}

		Import VisitImport_directive_(BooParser.Import_directive_Context context)
		{
			var ns = VisitNamespace_expression(context.namespace_expression());
			ReferenceExpression Assembly = null;
			ReferenceExpression Alias = null;
			if (context.FROM() != null)
			{
				string text;
				LexicalInfo li;
				if (context.identifier() != null)
				{
					text = context.identifier().GetText();
					li = GetLexicalInfo(context.identifier());
				}
				else if (context.SINGLE_QUOTED_STRING() != null)
				{
					text = SqsUnquote(context.SINGLE_QUOTED_STRING().GetText());
					li = GetLexicalInfo(context.SINGLE_QUOTED_STRING());
				}
				else
				{
					text = DqsUnquote(context.DOUBLE_QUOTED_STRING().GetText());
					li = GetLexicalInfo(context.DOUBLE_QUOTED_STRING());
				}
				Assembly = new ReferenceExpression(li, text);
			}
			if (context.AS() != null)
				Alias = new ReferenceExpression(GetLexicalInfo(context.ID()), context.ID().GetText());
			return new Import(ns, Assembly, Alias);
		}

		Node IBooParserVisitor<Node>.VisitImport_directive_(BooParser.Import_directive_Context context)
		{
			return VisitImport_directive_(context);
		}

		Import VisitImport_directive_from_(BooParser.Import_directive_from_Context context)
		{
			Expression expr = VisitIdentifier_expression(context.identifier_expression());
			if (context.MULTIPLY() == null)
			{
				var mie = new MethodInvocationExpression(expr.LexicalInfo, expr);
				foreach (var sub in context.expression_list().expression())
					mie.Arguments.Add((Expression)Visit(sub));
				expr = mie;
			}
			return new Import(GetLexicalInfo(context.FROM()), expr);
		}

		Node IBooParserVisitor<Node>.VisitImport_directive_from_(BooParser.Import_directive_from_Context context)
		{
			return VisitImport_directive_from_(context);
		}

		NamespaceDeclaration VisitNamespace_directive(BooParser.Namespace_directiveContext context)
		{
			var li = GetLexicalInfo(context.NAMESPACE());
			var result = new NamespaceDeclaration(context.identifier().GetText()) { LexicalInfo = li };
			CheckDocumentation(result, context.docstring());
			return result;
		}

		Node IBooParserVisitor<Node>.VisitNamespace_directive(BooParser.Namespace_directiveContext context)
		{
			return VisitNamespace_directive(context);
		}

		TypeMemberModifiers GetModifiers(BooParser.ModifiersContext context)
		{
			var result = TypeMemberModifiers.None;
			if (context == null)
				return result;
			foreach (var mod in context.type_member_modifier())
				switch (mod.Start.Type)
				{
					case BooParser.STATIC:
						result |= TypeMemberModifiers.Static;
						break;
					case BooParser.PUBLIC:
						result |= TypeMemberModifiers.Public;
						break;
					case BooParser.PROTECTED:
						result |= TypeMemberModifiers.Protected;
						break;
					case BooParser.PRIVATE:
						result |= TypeMemberModifiers.Private;
						break;
					case BooParser.INTERNAL:
						result |= TypeMemberModifiers.Internal;
						break;
					case BooParser.FINAL:
						result |= TypeMemberModifiers.Final;
						break;
					case BooParser.TRANSIENT:
						result |= TypeMemberModifiers.Transient;
						EmitTransientKeywordDeprecationWarning(GetLexicalInfo(mod));
						break;
					case BooParser.OVERRIDE:
						result |= TypeMemberModifiers.Override;
						break;
					case BooParser.ABSTRACT:
						result |= TypeMemberModifiers.Abstract;
						break;
					case BooParser.VIRTUAL:
						result |= TypeMemberModifiers.Virtual;
						break;
					case BooParser.NEW:
						result |= TypeMemberModifiers.New;
						break;
					case BooParser.PARTIAL:
						result |= TypeMemberModifiers.Partial;
						break;
				}
			return result;
		}

		void AddAttributes(INodeWithAttributes value, BooParser.AttributesContext context)
		{
			if (context != null)
				foreach (var attr in context.attribute())
					value.Attributes.Add(VisitAttribute(attr));
		}

		TypeMember VisitType_member(BooParser.Type_memberContext context)
		{
			TypeMember result;
			if (context.type_definition() != null)
				result = VisitType_definition(context.type_definition());
			else result = VisitMethod(context.method());
			AddAttributes(result, context.attributes());
			result.Modifiers = GetModifiers(context.modifiers());
			return result;
		}

		Node IBooParserVisitor<Node>.VisitType_member(BooParser.Type_memberContext context)
		{
			return VisitType_member(context);
		}

		TypeMember VisitType_definition(BooParser.Type_definitionContext context)
		{
			if (context.class_definition() != null)
				return VisitClass_definition(context.class_definition());
			if (context.interface_definition() != null)
				return VisitInterface_definition(context.interface_definition());
			if (context.enum_definition() != null)
				return VisitEnum_definition(context.enum_definition());
			return VisitCallable_definition(context.callable_definition());
		}

		Node IBooParserVisitor<Node>.VisitType_definition(BooParser.Type_definitionContext context)
		{
			return VisitType_definition(context);
		}

		void AddGenericParameters(INodeWithGenericParameters node, BooParser.Generic_parameter_declaration_listContext context)
		{
			if (context != null)
				foreach (var gpd in context.generic_parameter_declaration())
					node.GenericParameters.Add(VisitGeneric_parameter_declaration(gpd));
		}

		void AddParameters(INodeWithParameters node, BooParser.Parameter_declaration_listContext context)
		{
			if (context != null)
			{
				bool va = false;
				foreach (var pd in context.parameter_declaration())
				{
					if (va)
						EmitParamAfterVarargsError(GetLexicalInfo(pd));
					node.Parameters.Add(VisitParameter_declaration(pd));
					if (pd.MULTIPLY() != null)
						va = true;
				}
				node.Parameters.HasParamArray = va;
			}
		}

		CallableDefinition VisitCallable_definition(BooParser.Callable_definitionContext context)
		{
			var result = new CallableDefinition(GetLexicalInfo(context.ID())) { Name = context.ID().GetText() };
			AddGenericParameters(result, context.generic_parameter_declaration_list());
			AddParameters(result, context.parameter_declaration_list());
			if (context.type_reference() != null)
				result.ReturnType = VisitType_reference(context.type_reference());
			CheckDocumentation(result, context.docstring());
			return result;
		}

		Node IBooParserVisitor<Node>.VisitCallable_definition(BooParser.Callable_definitionContext context)
		{
			return VisitCallable_definition(context);
		}

		EnumDefinition VisitEnum_definition(BooParser.Enum_definitionContext context)
		{
			var result = new EnumDefinition(GetLexicalInfo(context.ID())) { Name = context.ID().GetText() };
			CheckDocumentation(result, context.begin_with_doc().docstring());
			if (context.PASS() == null)
				foreach (var em in context.any_enum_member())
					result.Members.Add(VisitAny_enum_member(em));
			return result;
		}

		Node IBooParserVisitor<Node>.VisitEnum_definition(BooParser.Enum_definitionContext context)
		{
			return VisitEnum_definition(context);
		}

		TypeMember VisitAny_enum_member(BooParser.Any_enum_memberContext context)
		{
			if (context.enum_member() != null)
				return VisitEnum_member(context.enum_member());
			return VisitSplice_type_definition_body(context.splice_type_definition_body());
		}

		Node IBooParserVisitor<Node>.VisitAny_enum_member(BooParser.Any_enum_memberContext context)
		{
			return VisitAny_enum_member(context);
		}

		EnumMember VisitEnum_member(BooParser.Enum_memberContext context)
		{
			var result = new EnumMember(GetLexicalInfo(context.ID()), context.ID().GetText());
			if (context.simple_initializer() != null)
				result.Initializer = (Expression)Visit(context.simple_initializer());
			AddAttributes(result, context.attributes());
			CheckDocumentation(result, context.docstring());
			return result;
		}

		Node IBooParserVisitor<Node>.VisitEnum_member(BooParser.Enum_memberContext context)
		{
			return VisitEnum_member(context);
		}

		Node IBooParserVisitor<Node>.VisitAttributes(BooParser.AttributesContext context)
		{
			throw new NotImplementedException("Should not see this");
		}

		void ApplyArgumentList(INodeWithArguments node, BooParser.Argument_listContext context)
		{
			foreach (var arg in context.argument())
				VisitArgument(arg, node);
		}

		Node IBooParserVisitor<Node>.VisitArgument_list(BooParser.Argument_listContext context)
		{
			throw new NotImplementedException();
		}

		Boo.Lang.Compiler.Ast.Attribute VisitAttribute(BooParser.AttributeContext context)
		{
			string name;
			LexicalInfo li;
			if (context.TRANSIENT() != null)
			{
				name = context.TRANSIENT().GetText();
				li = GetLexicalInfo(context.TRANSIENT());
			} else {
				name = context.identifier().GetText();
				li = GetLexicalInfo(context.identifier());
			}
			var result = new Boo.Lang.Compiler.Ast.Attribute(li, name);
			if (context.LPAREN() != null)
				ApplyArgumentList(result, context.argument_list());
			return result;
		}

		Node IBooParserVisitor<Node>.VisitAttribute(BooParser.AttributeContext context)
		{
			return VisitAttribute(context);
		}

		void VisitModule_attribute(BooParser.Module_attributeContext context, Module m)
		{
			m.Attributes.Add(VisitAttribute(context.attribute()));
		}

		Node IBooParserVisitor<Node>.VisitModule_attribute(BooParser.Module_attributeContext context)
		{
			VisitModule_attribute(context, _module);
			return null;
		}

		void VisitAssembly_attribute(BooParser.Assembly_attributeContext context, Module m)
		{
			m.AssemblyAttributes.Add(VisitAttribute(context.attribute()));
		}

		Node IBooParserVisitor<Node>.VisitAssembly_attribute(BooParser.Assembly_attributeContext context)
		{
			VisitAssembly_attribute(context, _module);
			return null;
		}

		void AddBaseTypes(TypeDefinition node, BooParser.Base_typesContext context)
		{
			if (context != null)
				foreach (var bt in context.type_reference())
					node.BaseTypes.Add(VisitType_reference(bt));
		}

		TypeMember VisitClass_definition(BooParser.Class_definitionContext context)
		{
			TypeDefinition result;
			string name;
			LexicalInfo li;
			Expression nameSplice = null;
			if (context.ID() != null)
			{
				name = context.ID().GetText();
				li = GetLexicalInfo(context.ID());
			} else {
				nameSplice = (Expression)Visit(context.atom());
				name = context.SPLICE_BEGIN().GetText();
				li = GetLexicalInfo(context.SPLICE_BEGIN());
			}
			if (context.CLASS() != null)
				result = new ClassDefinition(li) { Name = name };
			else result = new StructDefinition(li) { Name = name };
			AddGenericParameters(result, context.generic_parameter_declaration_list());
			AddBaseTypes(result, context.base_types());
			CheckDocumentation(result, context.begin_with_doc().docstring());
			if (context.PASS() == null)
			{
				foreach (var tdm in context.any_type_definition_member())
					result.Members.Add(VisitAny_type_definition_member(tdm));
			}
			if (nameSplice != null)
				return new SpliceTypeMember(result, nameSplice);
			return result;
		}

		Node IBooParserVisitor<Node>.VisitClass_definition(BooParser.Class_definitionContext context)
		{
			return VisitClass_definition(context);
		}

		TypeMember VisitAny_type_definition_member(BooParser.Any_type_definition_memberContext context)
		{
			if (context.type_definition_member() != null)
				return VisitType_definition_member(context.type_definition_member());
			return VisitSplice_type_definition_body(context.splice_type_definition_body());
		}

		Node IBooParserVisitor<Node>.VisitAny_type_definition_member(BooParser.Any_type_definition_memberContext context)
		{
			return VisitAny_type_definition_member(context);
		}

		SpliceTypeDefinitionBody VisitSplice_type_definition_body(BooParser.Splice_type_definition_bodyContext context)
		{
			var e = VisitAtom(context.atom());
			return new SpliceTypeDefinitionBody(e) { LexicalInfo = GetLexicalInfo(context.SPLICE_BEGIN()) };
		}

		Node IBooParserVisitor<Node>.VisitSplice_type_definition_body(BooParser.Splice_type_definition_bodyContext context)
		{
			return VisitSplice_type_definition_body(context);
		}

		TypeMember VisitType_definition_member(BooParser.Type_definition_memberContext context)
		{
			TypeMember result;
			if (context.method() != null)
				result = VisitMethod(context.method());
			else if (context.event_declaration() != null)
				result = VisitEvent_declaration(context.event_declaration());
			else if (context.field_or_property() != null)
				result = VisitField_or_property(context.field_or_property());
			else result = VisitType_definition(context.type_definition());
			AddAttributes(result, context.attributes());
			result.Modifiers = GetModifiers(context.modifiers());
			return result;
		}

		Node IBooParserVisitor<Node>.VisitType_definition_member(BooParser.Type_definition_memberContext context)
		{
			return VisitType_definition_member(context);
		}

		TypeMember VisitAny_intf_type_member(BooParser.Any_intf_type_memberContext context)
		{
			TypeMember result;
			if (context.interface_method() != null)
				result = VisitInterface_method(context.interface_method());
			else if (context.event_declaration() != null)
				result = VisitEvent_declaration(context.event_declaration());
			else result = VisitInterface_property(context.interface_property());
			AddAttributes(result, context.attributes());
			return result;
		}

		Node IBooParserVisitor<Node>.VisitAny_intf_type_member(BooParser.Any_intf_type_memberContext context)
		{
			return VisitAny_intf_type_member(context);
		}

		TypeMember VisitInterface_definition(BooParser.Interface_definitionContext context)
		{
			InterfaceDefinition result;
			string name;
			LexicalInfo li;
			Expression nameSplice = null;
			if (context.ID() != null)
			{
				name = context.ID().GetText();
				li = GetLexicalInfo(context.ID());
			}
			else
			{
				nameSplice = (Expression)Visit(context.atom());
				name = context.SPLICE_BEGIN().GetText();
				li = GetLexicalInfo(context.SPLICE_BEGIN());
			}
			result = new InterfaceDefinition(li) { Name = name };
			AddGenericParameters(result, context.generic_parameter_declaration_list());
			AddBaseTypes(result, context.base_types());
			CheckDocumentation(result, context.begin_with_doc().docstring());
			if (context.PASS() == null)
			{
				foreach (var tdm in context.any_intf_type_member())
					result.Members.Add(VisitAny_intf_type_member(tdm));
			}
			if (nameSplice != null)
				return new SpliceTypeMember(result, nameSplice);
			return result;
		}

		Node IBooParserVisitor<Node>.VisitInterface_definition(BooParser.Interface_definitionContext context)
		{
			return VisitInterface_definition(context);
		}

		Node IBooParserVisitor<Node>.VisitBase_types(BooParser.Base_typesContext context)
		{
			throw new NotImplementedException();
		}

		Method VisitInterface_method(BooParser.Interface_methodContext context)
		{
			string name;
			LexicalInfo li;
			Expression nameSplice = null;
			if (context.member() != null)
			{
				name = context.member().GetText();
				li = GetLexicalInfo(context.member());
			}
			else
			{
				nameSplice = (Expression)Visit(context.atom());
				name = context.SPLICE_BEGIN().GetText();
				li = GetLexicalInfo(context.SPLICE_BEGIN());
			}
			var result = new Method(li) { Name = name };
			if (context.generic_parameter_declaration_list() != null)
				AddGenericParameters(result, context.generic_parameter_declaration_list());
			else if (context.generic_parameter_declaration() != null)
				result.GenericParameters.Add(VisitGeneric_parameter_declaration(context.generic_parameter_declaration()));
			AddParameters(result, context.parameter_declaration_list());
			if (context.AS() != null)
				result.ReturnType = VisitType_reference(context.type_reference());
			CheckDocumentation(result, context.docstring());
			return result;
		}

		Node IBooParserVisitor<Node>.VisitInterface_method(BooParser.Interface_methodContext context)
		{
			return VisitInterface_method(context);
		}

		Property VisitInterface_property(BooParser.Interface_propertyContext context)
		{
			var id = context.ID() ?? context.SELF();
			var result = new Property(GetLexicalInfo(id)) { Name = id.GetText() };
			AddParameters(result, context.parameter_declaration_list());
			if (context.AS() != null)
				result.Type = VisitType_reference(context.type_reference());
			CheckDocumentation(result, context.begin_with_doc().docstring());
			foreach (var pa in context.interface_property_accessor())
			{
				if (pa.GET() != null)
					result.Getter = VisitInterface_property_accessor(pa);
				else result.Setter = VisitInterface_property_accessor(pa);
			}
			return result;
		}

		Node IBooParserVisitor<Node>.VisitInterface_property(BooParser.Interface_propertyContext context)
		{
			return VisitInterface_property(context);
		}

		Method VisitInterface_property_accessor(BooParser.Interface_property_accessorContext context)
		{
			var token = context.GET() ?? context.SET();
			var result = new Method(GetLexicalInfo(token)) { Name = token.GetText() };
			AddAttributes(result, context.attributes());
			return result;
		}

		Node IBooParserVisitor<Node>.VisitInterface_property_accessor(BooParser.Interface_property_accessorContext context)
		{
			return VisitInterface_property_accessor(context);
		}

		Node IBooParserVisitor<Node>.VisitEmpty_block(BooParser.Empty_blockContext context)
		{
			return null;
		}

		Event VisitEvent_declaration(BooParser.Event_declarationContext context)
		{
			var id = context.ID();
			var tr = VisitType_reference(context.type_reference());
			var result = new Event(GetLexicalInfo(id), id.GetText(), tr);
			CheckDocumentation(result, context.docstring());
			return result;
		}

		Node IBooParserVisitor<Node>.VisitEvent_declaration(BooParser.Event_declarationContext context)
		{
			return VisitEvent_declaration(context);
		}

		ExplicitMemberInfo VisitExplicit_member_info(BooParser.Explicit_member_infoContext context)
		{
			var ids = context.ID();
			var id = ids[0];
			var result = new ExplicitMemberInfo(GetLexicalInfo(id));
			_sbuilder.Clear();
			_sbuilder.Append(id.GetText());
			for (int i = 1; i < ids.Length; ++i)
			{
				_sbuilder.Append('.');
				_sbuilder.Append(ids[i].GetText());
			}
			result.InterfaceType = new SimpleTypeReference(result.LexicalInfo);
			result.InterfaceType.Name = _sbuilder.ToString();
			return result;
		}

		Node IBooParserVisitor<Node>.VisitExplicit_member_info(BooParser.Explicit_member_infoContext context)
		{
			return VisitExplicit_member_info(context);
		}

		TypeMember VisitMethod(BooParser.MethodContext context)
		{
			Method result;
			Expression nameSplice = null;
			if (context.CONSTRUCTOR() != null)
				result = new Constructor(GetLexicalInfo(context.CONSTRUCTOR()));
			else if (context.DESTRUCTOR() != null)
				result = new Destructor(GetLexicalInfo(context.DESTRUCTOR()));
			else
			{
				ExplicitMemberInfo emi = null;
				string name;
				LexicalInfo li;
				if (context.explicit_member_info() != null)
					emi = VisitExplicit_member_info(context.explicit_member_info());
				if (context.member() != null)
				{
					name = context.member().GetText();
					li = GetLexicalInfo(context.member());
				}
				else
				{
					nameSplice = (Expression)Visit(context.atom());
					name = context.SPLICE_BEGIN().GetText();
					li = GetLexicalInfo(context.SPLICE_BEGIN());
				}
				if (emi != null)
					result = new Method(emi.LexicalInfo) { Name = name, ExplicitInfo = emi };
				else result = new Method(li) { Name = name };
			}
			AddGenericParameters(result, context.generic_parameter_declaration_list());
			AddParameters(result, context.parameter_declaration_list());
			AddAttributes(result, context.attributes());
			if (context.AS() != null)
				result.ReturnType = VisitType_reference(context.type_reference());
			CheckDocumentation(result, context.begin_block_with_doc().docstring());
			result.Body = VisitBlock(context.block());
			result.Body.LexicalInfo = GetLexicalInfo(context.begin_block_with_doc());
			result.EndSourceLocation = result.Body.EndSourceLocation;
			if (nameSplice != null)
				return new SpliceTypeMember(result, nameSplice);
			return result;
		}

		Node IBooParserVisitor<Node>.VisitMethod(BooParser.MethodContext context)
		{
			return VisitMethod(context);
		}

		Property VisitFPProperty(BooParser.Field_or_propertyContext context)
		{
			Property result;
			ExplicitMemberInfo emi = null;
			if (context.explicit_member_info() != null)
				emi = VisitExplicit_member_info(context.explicit_member_info());
			string name;
			LexicalInfo li;
			var token = context.ID() ?? context.SELF();
			if (token != null)
			{
				name = token.GetText();
				li = GetLexicalInfo(token);
			}
			else
			{
				name = context.SPLICE_BEGIN().GetText();
				li = GetLexicalInfo(context.atom());
			}
			result = new Property(li) { Name = name, ExplicitInfo = emi };
			if (context.LPAREN() != null)
				EmitIndexedPropertyDeprecationWarning(result);
			AddParameters(result, context.parameter_declaration_list());
			if (context.AS() != null)
				result.Type = VisitType_reference(context.type_reference());
			if (context.begin_with_doc() != null)
				CheckDocumentation(result, context.begin_with_doc().docstring());
			foreach (var pa in context.property_accessor())
			{
				if (pa.GET() != null)
					result.Getter = VisitProperty_accessor(pa);
				else result.Setter = VisitProperty_accessor(pa);
			}
			return result;
		}

		Field VisitFPField(BooParser.Field_or_propertyContext context)
		{
			string name;
			LexicalInfo li;
			if (context.ID() != null)
			{
				name = context.ID().GetText();
				li = GetLexicalInfo(context.ID());
			}
			else
			{
				name = context.SPLICE_BEGIN().GetText();
				li = GetLexicalInfo(context.SPLICE_BEGIN());
			}
			var result = new Field(li) { Name = name };
			if (context.AS() != null)
				result.Type = VisitType_reference(context.type_reference());
			if (context.ASSIGN() != null)
				result.Initializer = VisitDeclaration_initializer(context.declaration_initializer());
			CheckDocumentation(result, context.docstring());
			return result;
		}

		TypeMember VisitField_or_property(BooParser.Field_or_propertyContext context)
		{
			TypeMember result;
			if (context.property_accessor().Length > 0)
				result = VisitFPProperty(context);
			else if (context.member_macro() != null)
				return VisitMember_macro(context.member_macro());
			else result = VisitFPField(context);
			if (context.atom() != null)
			{
				var nameSplice = (Expression)Visit(context.atom());
				return new SpliceTypeMember(result, nameSplice);
			}
			return result;
		}

		Node IBooParserVisitor<Node>.VisitField_or_property(BooParser.Field_or_propertyContext context)
		{
			return VisitField_or_property(context);
		}

		StatementTypeMember VisitMember_macro(BooParser.Member_macroContext context)
		{
			return new StatementTypeMember(VisitMacro_stmt(context.macro_stmt()));
		}

		Node IBooParserVisitor<Node>.VisitMember_macro(BooParser.Member_macroContext context)
		{
			return VisitMember_macro(context);
		}

		Expression VisitDeclaration_initializer(BooParser.Declaration_initializerContext context)
		{
			Expression result;
			if (context.slicing_expression() != null)
				result = VisitMethod_invocation_block(context.method_invocation_block(), VisitSlicing_expression(context.slicing_expression()));
			else if (context.array_or_expression() != null)
				result = VisitArray_or_expression(context.array_or_expression());
			else result = VisitCallable_expression(context.callable_expression());
			return result;
		}

		Node IBooParserVisitor<Node>.VisitDeclaration_initializer(BooParser.Declaration_initializerContext context)
		{
			return VisitDeclaration_initializer(context);
		}

		Expression VisitSimple_initializer(BooParser.Simple_initializerContext context)
		{
			Expression result;
			if (context.array_or_expression() != null)
				result = VisitArray_or_expression(context.array_or_expression());
			else result = VisitCallable_expression(context.callable_expression());
			return result;
		}

		Node IBooParserVisitor<Node>.VisitSimple_initializer(BooParser.Simple_initializerContext context)
		{
			return VisitSimple_initializer(context);
		}

		Method VisitProperty_accessor(BooParser.Property_accessorContext context)
		{
			var token = context.GET() ?? context.SET();
			var result = new Method(GetLexicalInfo(token)) { Name = token.GetText() };
			AddAttributes(result, context.attributes());
			result.Modifiers = GetModifiers(context.modifiers());
			if (context.compound_stmt() != null)
				result.Body = VisitCompound_stmt(context.compound_stmt());
			return result;
		}

		Node IBooParserVisitor<Node>.VisitProperty_accessor(BooParser.Property_accessorContext context)
		{
			return VisitProperty_accessor(context);
		}

		void VisitGlobals(BooParser.GlobalsContext context, Module m)
		{
			if (context.stmt() != null)
				foreach (var statement in context.stmt())
					m.Globals.Add((Statement)Visit(statement));
		}

		Node IBooParserVisitor<Node>.VisitGlobals(BooParser.GlobalsContext context)
		{
			VisitGlobals(context, _module);
			return null;
		}

		Block VisitBlock(BooParser.BlockContext context)
		{
			var result = new Block();
			if (context.stmt() != null)
				foreach (var statement in context.stmt())
					result.Add((Statement)Visit(statement));
			return result;
		}

		Node IBooParserVisitor<Node>.VisitBlock(BooParser.BlockContext context)
		{
			return VisitBlock(context);
		}

		Node IBooParserVisitor<Node>.VisitModifiers(BooParser.ModifiersContext context)
		{
			throw new NotImplementedException();
		}

		Node IBooParserVisitor<Node>.VisitType_member_modifier(BooParser.Type_member_modifierContext context)
		{
			throw new NotImplementedException();
		}

		Node IBooParserVisitor<Node>.VisitParameter_modifier(BooParser.Parameter_modifierContext context)
		{
			throw new NotImplementedException();
		}

		Node IBooParserVisitor<Node>.VisitParameter_declaration_list(BooParser.Parameter_declaration_listContext context)
		{
			throw new NotImplementedException();
		}

		ParameterDeclaration VisitParameter_declaration(BooParser.Parameter_declarationContext context)
		{
			string name;
			LexicalInfo li;
			Expression nameSplice = null;
			if (context.ID() != null)
			{
				name = context.ID().GetText();
				li = GetLexicalInfo(context.ID());
			}
			else
			{
				nameSplice = (Expression)Visit(context.atom());
				name = context.SPLICE_BEGIN().GetText();
				li = GetLexicalInfo(context.SPLICE_BEGIN());
			}
			var result = new ParameterDeclaration(li) { Name = name };
			if (context.AS() != null)
			{
				if (context.MULTIPLY() != null)
					result.Type = VisitArray_type_reference(context.array_type_reference());
				else result.Type = VisitType_reference(context.type_reference());
			}
			if (context.parameter_modifier() != null && context.parameter_modifier().REF() != null)
				result.Modifiers = ParameterModifiers.Ref;
			AddAttributes(result, context.attributes());
			if (nameSplice != null)
				return new SpliceParameterDeclaration(result, nameSplice);
			return result;
		}

		Node IBooParserVisitor<Node>.VisitParameter_declaration(BooParser.Parameter_declarationContext context)
		{
			return VisitParameter_declaration(context);
		}

		Node IBooParserVisitor<Node>.VisitCallable_parameter_declaration_list(BooParser.Callable_parameter_declaration_listContext context)
		{
			throw new NotImplementedException();
		}

		ParameterDeclaration VisitCallable_parameter_declaration(BooParser.Callable_parameter_declarationContext context)
		{
			var tr = VisitType_reference(context.type_reference());
			var result = new ParameterDeclaration(tr.LexicalInfo) { Type = tr };
			if (context.parameter_modifier() != null && context.parameter_modifier().REF() != null)
				result.Modifiers = ParameterModifiers.Ref;
			return result;
		}

		Node IBooParserVisitor<Node>.VisitCallable_parameter_declaration(BooParser.Callable_parameter_declarationContext context)
		{
			return VisitCallable_parameter_declaration(context);
		}

		void AddCallableParameters(INodeWithParameters node, BooParser.Callable_parameter_declaration_listContext context)
		{
			if (context != null)
			{
				bool va = false;
				foreach (var cpd in context.callable_parameter_declaration())
				{
					if (va)
						EmitParamAfterVarargsError(GetLexicalInfo(cpd));
					var param = VisitCallable_parameter_declaration(cpd);
					param.Name = "arg" + node.Parameters.Count;
					node.Parameters.Add(param);
					if (cpd.MULTIPLY() != null)
						va = true;
				}
				node.Parameters.HasParamArray = va;
			}
		}

		CallableTypeReference VisitCallable_type_reference(BooParser.Callable_type_referenceContext context)
		{
			var result = new CallableTypeReference(GetLexicalInfo(context.CALLABLE()));
			AddCallableParameters(result, context.callable_parameter_declaration_list());
			if (context.AS() != null)
				result.ReturnType = VisitType_reference(context.type_reference());
			return result;
		}

		Node IBooParserVisitor<Node>.VisitCallable_type_reference(BooParser.Callable_type_referenceContext context)
		{
			return VisitCallable_type_reference(context);
		}

		Node IBooParserVisitor<Node>.VisitGeneric_parameter_declaration_list(BooParser.Generic_parameter_declaration_listContext context)
		{
			throw new NotImplementedException();
		}

		void AddGenericParameterConstraints(GenericParameterDeclaration gpd, BooParser.Generic_parameter_constraintsContext context)
		{
			while (context != null)
			{
				if (context.CLASS() != null)
					gpd.Constraints |= GenericParameterConstraints.ReferenceType;
				else if (context.STRUCT() != null)
					gpd.Constraints |= GenericParameterConstraints.ValueType;
				else if (context.CONSTRUCTOR() != null)
					gpd.Constraints |= GenericParameterConstraints.Constructable;
				else gpd.BaseTypes.Add(VisitType_reference(context.type_reference()));

				context = context.generic_parameter_constraints();
			}
		}

		GenericParameterDeclaration VisitGeneric_parameter_declaration(BooParser.Generic_parameter_declarationContext context)
		{
			var id = context.ID();
			var result = new GenericParameterDeclaration(GetLexicalInfo(id)) { Name = id.GetText() };
			AddGenericParameterConstraints(result, context.generic_parameter_constraints());
			return result;
		}

		Node IBooParserVisitor<Node>.VisitGeneric_parameter_declaration(BooParser.Generic_parameter_declarationContext context)
		{
			return VisitGeneric_parameter_declaration(context);
		}

		Node IBooParserVisitor<Node>.VisitGeneric_parameter_constraints(BooParser.Generic_parameter_constraintsContext context)
		{
			throw new NotImplementedException();
		}

		ArrayTypeReference VisitArray_type_reference(BooParser.Array_type_referenceContext context)
		{
			var tr = VisitType_reference(context.type_reference());
			var result = new ArrayTypeReference(GetLexicalInfo(context.LPAREN()), tr);
			if (context.integer_literal() != null)
				result.Rank = VisitInteger_literal(context.integer_literal());
			return result;
		}

		Node IBooParserVisitor<Node>.VisitArray_type_reference(BooParser.Array_type_referenceContext context)
		{
			return VisitArray_type_reference(context);
		}

		void AddTypeReferences(TypeReferenceCollection container, BooParser.Type_reference_listContext context)
		{
			if (context != null)
				foreach (var tr in context.type_reference())
					container.Add(VisitType_reference(tr));
		}

		Node IBooParserVisitor<Node>.VisitType_reference_list(BooParser.Type_reference_listContext context)
		{
			throw new NotImplementedException();
		}

		SpliceTypeReference VisitSplice_type_reference(BooParser.Splice_type_referenceContext context)
		{
			return new SpliceTypeReference(GetLexicalInfo(context.SPLICE_BEGIN()), VisitAtom(context.atom()));
		}

		Node IBooParserVisitor<Node>.VisitSplice_type_reference(BooParser.Splice_type_referenceContext context)
		{
			return VisitSplice_type_reference(context);
		}

		TypeReference ParseGenericTypeReference(BooParser.Type_referenceContext context, BooParser.Type_nameContext id)
		{
			TypeReference result;
			if (context.LBRACK() != null)
			{
				if (context.MULTIPLY() != null)
					result = new GenericTypeDefinitionReference(GetLexicalInfo(id)) { Name = GetName(id), GenericPlaceholders = context.MULTIPLY().Length };
				else {
					GenericTypeReference gtr = new GenericTypeReference(GetLexicalInfo(id), GetName(id));
					AddTypeReferences(gtr.GenericArguments, context.type_reference_list());
					result = gtr;
				}
			}
			else {
				if (context.MULTIPLY() != null)
					result = new GenericTypeDefinitionReference(GetLexicalInfo(id)) { Name = GetName(id), GenericPlaceholders = 1 };
				else
				{
					GenericTypeReference gtr = new GenericTypeReference(GetLexicalInfo(id), GetName(id));
					gtr.GenericArguments.Add(VisitType_reference(context.type_reference()));
					result = gtr;
				}
			}
			return result;
		}

		TypeReference ParseTypeReference(BooParser.Type_referenceContext context)
		{
			TypeReference result;
			var id = context.type_name();
			if ((context.OF() ?? context.LBRACK()) != null)
				result = ParseGenericTypeReference(context, id);
			else result = new SimpleTypeReference(GetLexicalInfo(id), GetName(id));
			if (context.NULLABLE_SUFFIX() != null)
			{
				var ntr = new GenericTypeReference(result.LexicalInfo, "System.Nullable");
				ntr.GenericArguments.Add(result);
				result = ntr;
			}
			return result;
		}

		TypeReference VisitType_reference(BooParser.Type_referenceContext context)
		{
			TypeReference result;
			if (context.splice_type_reference() != null)
				result = VisitSplice_type_reference(context.splice_type_reference());
			else if (context.array_type_reference() != null)
				result = VisitArray_type_reference(context.array_type_reference());
			else if (context.callable_type_reference() != null)
				result = VisitCallable_type_reference(context.callable_type_reference());
			else result = ParseTypeReference(context);

			var enumDegree = 0;
			if (context.MULTIPLY() != null)
				enumDegree += context.MULTIPLY().Length;
			if (context.EXPONENTIATION() != null)
				enumDegree += context.EXPONENTIATION().Length * 2;
			for (int i = 0; i < enumDegree; ++i)
				result = CodeFactory.EnumerableTypeReferenceFor(result);
			return result;
		}

		Node IBooParserVisitor<Node>.VisitType_reference(BooParser.Type_referenceContext context)
		{
			return VisitType_reference(context);
		}

		string GetName(BooParser.Type_nameContext context)
		{
			if (context.identifier() != null)
				return context.identifier().GetText();
			return (context.CALLABLE() ?? context.CHAR()).GetText();
		}

		Node IBooParserVisitor<Node>.VisitType_name(BooParser.Type_nameContext context)
		{
			throw new NotImplementedException();
		}

		Node IBooParserVisitor<Node>.VisitBegin(BooParser.BeginContext context)
		{
			return null;
		}

		Node IBooParserVisitor<Node>.VisitBegin_with_doc(BooParser.Begin_with_docContext context)
		{
			throw new NotImplementedException();
		}

		Node IBooParserVisitor<Node>.VisitBegin_block_with_doc(BooParser.Begin_block_with_docContext context)
		{
			throw new NotImplementedException();
		}

		Node IBooParserVisitor<Node>.VisitEnd(BooParser.EndContext context)
		{
			return null;
		}

		Block VisitCompound_stmt(BooParser.Compound_stmtContext context)
		{
			if (context.single_line_block() != null)
				return VisitSingle_line_block(context.single_line_block());
			var result = VisitBlock(context.block());
			result.LexicalInfo = GetLexicalInfo(context.COLON());
			return result;
		}

		Node IBooParserVisitor<Node>.VisitCompound_stmt(BooParser.Compound_stmtContext context)
		{
			return VisitCompound_stmt(context);
		}

		Block VisitSingle_line_block(BooParser.Single_line_blockContext context)
		{
			var result = new Block(GetLexicalInfo(context.COLON()));
			if (context.simple_stmt() != null)
				foreach (var stmt in context.simple_stmt())
					result.Add(VisitSimple_stmt(stmt));
			if (context.EOL() != null)
				SetEndSourceLocation(result, context.EOL().Last());
			return result;
		}

		Node IBooParserVisitor<Node>.VisitSingle_line_block(BooParser.Single_line_blockContext context)
		{
			return VisitSingle_line_block(context);
		}

		MacroStatement VisitClosure_macro_stmt(BooParser.Closure_macro_stmtContext context)
		{
			var id = context.macro_name();
			var result = new MacroStatement(GetLexicalInfo(id), id.GetText());
			GetExpressionList(result.Arguments, context.expression_list());
			return result;
		}

		Node IBooParserVisitor<Node>.VisitClosure_macro_stmt(BooParser.Closure_macro_stmtContext context)
		{
			return VisitClosure_macro_stmt(context);
		}

		void GetMacroBlock(StatementCollection container, BooParser.Macro_blockContext context)
		{
			if (context.any_macro_stmt() != null)
				foreach (var stmt in context.any_macro_stmt())
					container.Add(VisitAny_macro_stmt(stmt));
		}

		Node IBooParserVisitor<Node>.VisitMacro_block(BooParser.Macro_blockContext context)
		{
			throw new NotImplementedException();
		}

		Statement VisitAny_macro_stmt(BooParser.Any_macro_stmtContext context)
		{
			if (context.stmt() != null)
				return VisitStmt(context.stmt());
			return VisitType_member_stmt(context.type_member_stmt());
		}

		Node IBooParserVisitor<Node>.VisitAny_macro_stmt(BooParser.Any_macro_stmtContext context)
		{
			throw new NotImplementedException();
		}

		TypeMemberStatement VisitType_member_stmt(BooParser.Type_member_stmtContext context)
		{
			return new TypeMemberStatement(VisitType_definition_member(context.type_definition_member()));
		}

		Node IBooParserVisitor<Node>.VisitType_member_stmt(BooParser.Type_member_stmtContext context)
		{
			return VisitType_member_stmt(context);
		}

		Block VisitMacro_compound_stmt(BooParser.Macro_compound_stmtContext context)
		{
			if (context.single_line_block() != null)
				return VisitSingle_line_block(context.single_line_block());
			var result = new Block();
			GetMacroBlock(result.Statements, context.macro_block());
			result.LexicalInfo = GetLexicalInfo(context.COLON());
			return result;
		}

		Node IBooParserVisitor<Node>.VisitMacro_compound_stmt(BooParser.Macro_compound_stmtContext context)
		{
			return VisitMacro_compound_stmt(context);
		}

		MacroStatement VisitMacro_stmt(BooParser.Macro_stmtContext context)
		{
			var id = context.macro_name();
			var result = new MacroStatement(GetLexicalInfo(id), GetMacroName(id));
			GetExpressionList(result.Arguments, context.expression_list());
			if (context.begin_with_doc() != null)
			{
				CheckDocumentation(result, context.begin_with_doc().docstring());
				GetMacroBlock(result.Body.Statements, context.macro_block());
				result.Annotate("compound");
			}
			else if (context.macro_compound_stmt() != null)
			{
				result.Body = VisitMacro_compound_stmt(context.macro_compound_stmt());
				result.Annotate("compound");
			} else {
				if (context.stmt_modifier() != null)
					result.Modifier = VisitStmt_modifier(context.stmt_modifier());
				CheckDocumentation(result, context.docstring());
			}
			return result;
		}

		Node IBooParserVisitor<Node>.VisitMacro_stmt(BooParser.Macro_stmtContext context)
		{
			return VisitMacro_stmt(context);
		}

		string GetMacroName(BooParser.Macro_nameContext context)
		{
			return (context.ID() ?? context.THEN()).GetText();
		}

		Node IBooParserVisitor<Node>.VisitMacro_name(BooParser.Macro_nameContext context)
		{
			throw new NotImplementedException();
		}

		GotoStatement VisitGoto_stmt(BooParser.Goto_stmtContext context)
		{
			return new GotoStatement(
				GetLexicalInfo(context.GOTO()),
				new ReferenceExpression(
					GetLexicalInfo(context.ID()),
					context.ID().GetText()));
		}

		Node IBooParserVisitor<Node>.VisitGoto_stmt(BooParser.Goto_stmtContext context)
		{
			return VisitGoto_stmt(context);
		}

		LabelStatement VisitLabel_stmt(BooParser.Label_stmtContext context)
		{
			return new LabelStatement(GetLexicalInfo(context.COLON()), context.ID().GetText());
		}

		Node IBooParserVisitor<Node>.VisitLabel_stmt(BooParser.Label_stmtContext context)
		{
			return VisitLabel_stmt(context);
		}

		DeclarationStatement VisitNested_function(BooParser.Nested_functionContext context)
		{
			var id = context.ID();
			string name = id.GetText();
			var be = new BlockExpression(GetLexicalInfo(id));
			be.Body = VisitCompound_stmt(context.compound_stmt());
			AddParameters(be, context.parameter_declaration_list());
			if (context.AS() != null)
				be.ReturnType = VisitType_reference(context.type_reference());
			var result = new DeclarationStatement(
						GetLexicalInfo(context.DEF()),
						new Declaration(
							GetLexicalInfo(id),
							name),
						be);
			be[BlockExpression.ClosureNameAnnotation] = name;
			return result;
		}

		Node IBooParserVisitor<Node>.VisitNested_function(BooParser.Nested_functionContext context)
		{
			return VisitNested_function(context);
		}

		Statement VisitStmt(BooParser.StmtContext context)
		{
			if (context.nested_function() != null)
				return VisitNested_function(context.nested_function());
			if (context.for_stmt() != null)
				return VisitFor_stmt(context.for_stmt());
			if (context.while_stmt() != null)
				return VisitWhile_stmt(context.while_stmt());
			if (context.if_stmt() != null)
				return VisitIf_stmt(context.if_stmt());
			if (context.unless_stmt() != null)
				return VisitUnless_stmt(context.unless_stmt());
			if (context.try_stmt() != null)
				return VisitTry_stmt(context.try_stmt());
			if (context.macro_stmt() != null)
				return VisitMacro_stmt(context.macro_stmt());
			if (context.assignment_or_method_invocation_with_block_stmt() != null)
				return VisitAssignment_or_method_invocation_with_block_stmt(context.assignment_or_method_invocation_with_block_stmt());
			if (context.return_stmt() != null)
				return VisitReturn_stmt(context.return_stmt());
			if (context.unpack_stmt() != null)
				return VisitUnpack_stmt(context.unpack_stmt());
			if (context.declaration_stmt() != null)
				return VisitDeclaration_stmt(context.declaration_stmt());

			Statement result;
			if (context.goto_stmt() != null)
				result = VisitGoto_stmt(context.goto_stmt());
			else if (context.label_stmt() != null)
				result = VisitLabel_stmt(context.label_stmt());
			else if (context.yield_stmt() != null)
				result = VisitYield_stmt(context.yield_stmt());
			else if (context.break_stmt() != null)
				result = VisitBreak_stmt(context.break_stmt());
			else if (context.continue_stmt() != null)
				result = VisitContinue_stmt(context.continue_stmt());
			else if (context.raise_stmt() != null)
				result = VisitRaise_stmt(context.raise_stmt());
			else result = VisitExpression_stmt(context.expression_stmt());
			if (context.stmt_modifier() != null)
				result.Modifier = VisitStmt_modifier(context.stmt_modifier());
			return result;
		}

		Node IBooParserVisitor<Node>.VisitStmt(BooParser.StmtContext context)
		{
			return VisitStmt(context);
		}

		Statement VisitSimple_stmt(BooParser.Simple_stmtContext context)
		{
			if (context.closure_macro_stmt() != null)
				return VisitClosure_macro_stmt(context.closure_macro_stmt());
			if (context.assignment_or_method_invocation() != null)
				return VisitAssignment_or_method_invocation(context.assignment_or_method_invocation());
			if (context.return_expression_stmt() != null)
				return VisitReturn_expression_stmt(context.return_expression_stmt());
			if (context.unpack() != null)
				return VisitUnpack(context.unpack());
			if (context.goto_stmt() != null)
				return VisitGoto_stmt(context.goto_stmt());
			if (context.label_stmt() != null)
				return VisitLabel_stmt(context.label_stmt());
			if (context.yield_stmt() != null)
				return VisitYield_stmt(context.yield_stmt());
			if (context.break_stmt() != null)
				return VisitBreak_stmt(context.break_stmt());
			if (context.continue_stmt() != null)
				return VisitContinue_stmt(context.continue_stmt());
			if (context.raise_stmt() != null)
				return VisitRaise_stmt(context.raise_stmt());
			return VisitExpression_stmt(context.expression_stmt());
		}

		Node IBooParserVisitor<Node>.VisitSimple_stmt(BooParser.Simple_stmtContext context)
		{
			return VisitSimple_stmt(context);
		}

		StatementModifier VisitStmt_modifier(BooParser.Stmt_modifierContext context)
		{
			ITerminalNode t;
			StatementModifierType type;
			if (context.IF() != null)
			{
				t = context.IF();
				type = StatementModifierType.If;
			}
			else if (context.UNLESS() != null)
			{
				t = context.UNLESS();
				type = StatementModifierType.Unless;
			} else {
				t = context.WHILE();
				type = StatementModifierType.While;
			}
			var e = VisitBoolean_expression(context.boolean_expression());
			return new StatementModifier(type, e) { LexicalInfo = GetLexicalInfo(t) };
		}

		Node IBooParserVisitor<Node>.VisitStmt_modifier(BooParser.Stmt_modifierContext context)
		{
			return VisitStmt_modifier(context);
		}

		Expression VisitCallable_or_expression(BooParser.Callable_or_expressionContext context)
		{
			if (context.callable_expression() != null)
				return VisitCallable_expression(context.callable_expression());
			return VisitArray_or_expression(context.array_or_expression());
		}

		Node IBooParserVisitor<Node>.VisitCallable_or_expression(BooParser.Callable_or_expressionContext context)
		{
			return VisitCallable_or_expression(context);
		}

		Statement VisitInternal_closure_stmt(BooParser.Internal_closure_stmtContext context)
		{
			if (context.return_expression_stmt() != null)
				return VisitReturn_expression_stmt(context.return_expression_stmt());

			Statement result;
			if (context.unpack() != null)
				result = VisitUnpack(context.unpack());
			else if (context.closure_macro_stmt() != null)
				result = VisitClosure_macro_stmt(context.closure_macro_stmt());
			else if (context.closure_expression_stmt() != null)
				result = VisitClosure_expression_stmt(context.closure_expression_stmt());
			else if (context.raise_stmt() != null)
				result = VisitRaise_stmt(context.raise_stmt());
			else result = VisitYield_stmt(context.yield_stmt());

			if (context.stmt_modifier() != null)
				result.Modifier = VisitStmt_modifier(context.stmt_modifier());
			return result;
		}

		Node IBooParserVisitor<Node>.VisitInternal_closure_stmt(BooParser.Internal_closure_stmtContext context)
		{
			return VisitInternal_closure_stmt(context);
		}

		ExpressionStatement VisitClosure_expression_stmt(BooParser.Closure_expression_stmtContext context)
		{
			var e = VisitArray_or_expression(context.array_or_expression());
			return new ExpressionStatement(e.LexicalInfo, e);
		}

		Node IBooParserVisitor<Node>.VisitClosure_expression_stmt(BooParser.Closure_expression_stmtContext context)
		{
			return VisitClosure_expression_stmt(context);
		}

		BlockExpression VisitClosure_expression(BooParser.Closure_expressionContext context)
		{
			var result = new BlockExpression(GetLexicalInfo(context.LBRACE()));
			result.Annotate("inline");
			AddParameters(result, context.parameter_declaration_list());
			foreach (var stmt in context.internal_closure_stmt())
				result.Body.Annotate(VisitInternal_closure_stmt(stmt));
			SetEndSourceLocation(result.Body, context.RBRACE());
			return result;
		}

		Node IBooParserVisitor<Node>.VisitClosure_expression(BooParser.Closure_expressionContext context)
		{
			return VisitClosure_expression(context);
		}

		BlockExpression VisitCallable_expression(BooParser.Callable_expressionContext context)
		{
			var anchor = context.DEF() ?? context.DO();
			if (anchor == null)
			{
				var body = VisitCompound_stmt(context.compound_stmt());
				return new BlockExpression(body.LexicalInfo, body);
			}
			var result = new BlockExpression(GetLexicalInfo(anchor), VisitCompound_stmt(context.compound_stmt()));
			AddParameters(result, context.parameter_declaration_list());
			if (context.AS() != null)
				result.ReturnType = VisitType_reference(context.type_reference());
			return result;
		}

		Node IBooParserVisitor<Node>.VisitCallable_expression(BooParser.Callable_expressionContext context)
		{
			return VisitCallable_expression(context);
		}

		TryStatement VisitTry_stmt(BooParser.Try_stmtContext context)
		{
			var result = new TryStatement(GetLexicalInfo(context.TRY()));
			var blocks = context.compound_stmt();
			result.ProtectedBlock = VisitCompound_stmt(blocks[0]);
			foreach (var eh in context.exception_handler())
				result.ExceptionHandlers.Add(VisitException_handler(eh));
			if (context.FAILURE() != null)
			{
				result.FailureBlock = VisitCompound_stmt(blocks[1]);
				result.FailureBlock.LexicalInfo = GetLexicalInfo(context.FAILURE());
			}
			if (context.ENSURE() != null)
			{
				result.EnsureBlock = VisitCompound_stmt(blocks[2]);
				result.EnsureBlock.LexicalInfo = GetLexicalInfo(context.ENSURE());
			}
			return result;
		}

		Node IBooParserVisitor<Node>.VisitTry_stmt(BooParser.Try_stmtContext context)
		{
			return VisitTry_stmt(context);
		}

		ExceptionHandler VisitException_handler(BooParser.Exception_handlerContext context)
		{
			var result = new ExceptionHandler(GetLexicalInfo(context.EXCEPT()));
			var x = context.ID();
			TypeReference tr = null;
			var u = context.UNLESS();
			Expression e = null;
			if (context.AS() != null)
				tr = VisitType_reference(context.type_reference());
			if (context.boolean_expression() != null)
				e = VisitBoolean_expression(context.boolean_expression());
			result.Declaration = new Declaration();
			result.Declaration.Type = tr;

			if (x != null)
			{
				result.Declaration.LexicalInfo = GetLexicalInfo(x);
				result.Declaration.Name = x.GetText();
			}
			else
			{
				result.Declaration.Name = null;
				result.Flags |= ExceptionHandlerFlags.Anonymous;
			}
			if (tr != null)
				result.Declaration.LexicalInfo = tr.LexicalInfo;
			else if (x != null)
				result.Declaration.LexicalInfo = result.LexicalInfo;
			if (tr == null)
				result.Flags |= ExceptionHandlerFlags.Untyped;
			if (e != null)
			{
				if (u != null)
				{
					UnaryExpression not = new UnaryExpression(GetLexicalInfo(u));
					not.Operator = UnaryOperatorType.LogicalNot;
					not.Operand = e;
					e = not;
				}
				result.FilterCondition = e;
				result.Flags |= ExceptionHandlerFlags.Filter;
			}
			result.Block = VisitCompound_stmt(context.compound_stmt());
			return result;
		}

		Node IBooParserVisitor<Node>.VisitException_handler(BooParser.Exception_handlerContext context)
		{
			return VisitException_handler(context);
		}

		RaiseStatement VisitRaise_stmt(BooParser.Raise_stmtContext context)
		{
			Expression e = null;
			if (context.expression() != null)
				e = VisitExpression(context.expression());
			return new RaiseStatement(GetLexicalInfo(context.RAISE()), e);
		}

		Node IBooParserVisitor<Node>.VisitRaise_stmt(BooParser.Raise_stmtContext context)
		{
			return VisitRaise_stmt(context);
		}

		DeclarationStatement VisitDeclaration_stmt(BooParser.Declaration_stmtContext context)
		{
			var id = context.ID();
			var tr = VisitType_reference(context.type_reference());
			Expression initializer = null;
			StatementModifier m = null;
			if (context.ASSIGN() != null)
			{
				if (context.simple_initializer() != null)
					initializer = VisitSimple_initializer(context.simple_initializer());
				else initializer = VisitDeclaration_initializer(context.declaration_initializer());
			}
			else if (context.stmt_modifier() != null)
				m = VisitStmt_modifier(context.stmt_modifier());

			Declaration d = new Declaration(GetLexicalInfo(id));
			d.Name = id.GetText();
			d.Type = tr;

			var result = new DeclarationStatement(d.LexicalInfo);
			result.Declaration = d;
			result.Initializer = initializer;
			result.Modifier = m;
			return result;
		}

		Node IBooParserVisitor<Node>.VisitDeclaration_stmt(BooParser.Declaration_stmtContext context)
		{
			return VisitDeclaration_stmt(context);
		}

		ExpressionStatement VisitExpression_stmt(BooParser.Expression_stmtContext context)
		{
			var e = VisitAssignment_expression(context.assignment_expression());
			return new ExpressionStatement(e.LexicalInfo, e);
		}

		Node IBooParserVisitor<Node>.VisitExpression_stmt(BooParser.Expression_stmtContext context)
		{
			return VisitExpression_stmt(context);
		}

		ReturnStatement VisitReturn_expression_stmt(BooParser.Return_expression_stmtContext context)
		{
			Expression e = null;
			StatementModifier modifier = null;
			if (context.array_or_expression() != null)
				e = VisitArray_or_expression(context.array_or_expression());
			if (context.stmt_modifier() != null)
				modifier = VisitStmt_modifier(context.stmt_modifier());
			return new ReturnStatement(GetLexicalInfo(context.RETURN()), e, modifier);
		}

		Node IBooParserVisitor<Node>.VisitReturn_expression_stmt(BooParser.Return_expression_stmtContext context)
		{
			return VisitReturn_expression_stmt(context);
		}

		ReturnStatement VisitReturn_stmt(BooParser.Return_stmtContext context)
		{
			Expression e = null;
			StatementModifier modifier = null;
			if (context.array_or_expression() != null)
			{
				e = VisitArray_or_expression(context.array_or_expression());
				if (context.method_invocation_block() != null)
				{
					e = VisitMethod_invocation_block(context.method_invocation_block(), e);
				}
				else if (context.stmt_modifier() != null)
					modifier = VisitStmt_modifier(context.stmt_modifier());
			}
			else if (context.callable_expression() != null)
				e = VisitCallable_expression(context.callable_expression());
			else if (context.stmt_modifier() != null)
				modifier = VisitStmt_modifier(context.stmt_modifier());
			return new ReturnStatement(GetLexicalInfo(context.RETURN()), e, modifier);
		}

		Node IBooParserVisitor<Node>.VisitReturn_stmt(BooParser.Return_stmtContext context)
		{
			return VisitReturn_stmt(context);
		}

		YieldStatement VisitYield_stmt(BooParser.Yield_stmtContext context)
		{
			Expression e = null;
			if (context.array_or_expression() != null)
				e = VisitArray_or_expression(context.array_or_expression());
			return new YieldStatement(GetLexicalInfo(context.YIELD()), e);
		}

		Node IBooParserVisitor<Node>.VisitYield_stmt(BooParser.Yield_stmtContext context)
		{
			return VisitYield_stmt(context);
		}

		BreakStatement VisitBreak_stmt(BooParser.Break_stmtContext context)
		{
			return new BreakStatement(GetLexicalInfo(context.BREAK()));
		}

		Node IBooParserVisitor<Node>.VisitBreak_stmt(BooParser.Break_stmtContext context)
		{
			return VisitBreak_stmt(context);
		}

		ContinueStatement VisitContinue_stmt(BooParser.Continue_stmtContext context)
		{
			return new ContinueStatement(GetLexicalInfo(context.CONTINUE()));
		}

		Node IBooParserVisitor<Node>.VisitContinue_stmt(BooParser.Continue_stmtContext context)
		{
			return VisitContinue_stmt(context);
		}

		UnlessStatement VisitUnless_stmt(BooParser.Unless_stmtContext context)
		{
			var condition = VisitExpression(context.expression());
			var result = new UnlessStatement(GetLexicalInfo(context.UNLESS())) { Condition = condition };
			result.Block = VisitCompound_stmt(context.compound_stmt());
			return result;
		}

		Node IBooParserVisitor<Node>.VisitUnless_stmt(BooParser.Unless_stmtContext context)
		{
			return VisitUnless_stmt(context);
		}

		ForStatement VisitFor_stmt(BooParser.For_stmtContext context)
		{
			var result = new ForStatement(GetLexicalInfo(context.FOR()));
			AddDeclarations(result.Declarations, context.declaration_list());
			result.Iterator = VisitArray_or_expression(context.array_or_expression());
			var blocks = context.compound_stmt();
			result.Block = VisitCompound_stmt(blocks[0]);
			int blockCounter = 1;
			if (context.OR() != null)
			{
				result.OrBlock = VisitCompound_stmt(blocks[blockCounter]);
				++blockCounter;
			}
			if (context.THEN() != null)
				result.ThenBlock = VisitCompound_stmt(blocks[blockCounter]);
			return result;
		}

		Node IBooParserVisitor<Node>.VisitFor_stmt(BooParser.For_stmtContext context)
		{
			return VisitFor_stmt(context);
		}

		WhileStatement VisitWhile_stmt(BooParser.While_stmtContext context)
		{
			Expression e = VisitExpression(context.expression());
			var blocks = context.compound_stmt();
			var result = new WhileStatement(e, VisitCompound_stmt(blocks[0])) { LexicalInfo = GetLexicalInfo(context.WHILE()) };
			int blockCounter = 1;
			if (context.OR() != null)
			{
				result.OrBlock = VisitCompound_stmt(blocks[blockCounter]);
				++blockCounter;
			}
			if (context.THEN() != null)
				result.ThenBlock = VisitCompound_stmt(blocks[blockCounter]);
			return result;
		}

		Node IBooParserVisitor<Node>.VisitWhile_stmt(BooParser.While_stmtContext context)
		{
			return VisitWhile_stmt(context);
		}

		IfStatement VisitIf_stmt(BooParser.If_stmtContext context)
		{
			Expression e = VisitExpression(context.expression(0));
			var blocks = context.compound_stmt();
			var result = new IfStatement(GetLexicalInfo(context.IF())) { Condition = e, TrueBlock = VisitCompound_stmt(blocks[0]) };
			var s = result;
			var i = 1;
			if (context.ELIF() != null)
				foreach (var ei in context.ELIF())
				{
					s.FalseBlock = new Block();
					IfStatement elif = new IfStatement(GetLexicalInfo(ei));
					elif.TrueBlock = new Block();
					elif.Condition = e;
					s.FalseBlock.Add(elif);
					s = elif;
					s.TrueBlock = VisitCompound_stmt(blocks[i]);
					++i;
				}
			if (context.ELSE() != null)
			{
				s.FalseBlock = VisitCompound_stmt(blocks[i]);
				s.FalseBlock.LexicalInfo = GetLexicalInfo(context.ELSE());
			}
			return result;
		}

		Node IBooParserVisitor<Node>.VisitIf_stmt(BooParser.If_stmtContext context)
		{
			return VisitIf_stmt(context);
		}

		UnpackStatement VisitUnpack_stmt(BooParser.Unpack_stmtContext context)
		{
			var result = VisitUnpack(context.unpack());
			if (context.stmt_modifier() != null)
				result.Modifier = VisitStmt_modifier(context.stmt_modifier());
			return result;
		}

		Node IBooParserVisitor<Node>.VisitUnpack_stmt(BooParser.Unpack_stmtContext context)
		{
			return VisitUnpack_stmt(context);
		}

		UnpackStatement VisitUnpack(BooParser.UnpackContext context)
		{
			Expression e = VisitArray_or_expression(context.array_or_expression());
			var result = new UnpackStatement(GetLexicalInfo(context.ASSIGN())) { Expression = e };
			result.Declarations.Add(VisitDeclaration(context.declaration()));
			AddDeclarations(result.Declarations, context.declaration_list());
			return result;
		}

		Node IBooParserVisitor<Node>.VisitUnpack(BooParser.UnpackContext context)
		{
			return VisitUnpack(context);
		}

		void AddDeclarations(DeclarationCollection dc, BooParser.Declaration_listContext context)
		{
			foreach (var decl in context.declaration())
				dc.Add(VisitDeclaration(decl));
		}

		Node IBooParserVisitor<Node>.VisitDeclaration_list(BooParser.Declaration_listContext context)
		{
			throw new NotImplementedException();
		}

		Declaration VisitDeclaration(BooParser.DeclarationContext context)
		{
			var id = context.ID();
			TypeReference tr = null;
			if (context.AS() != null)
				tr = VisitType_reference(context.type_reference());
			return new Declaration(GetLexicalInfo(id), id.GetText(), tr);
		}

		Node IBooParserVisitor<Node>.VisitDeclaration(BooParser.DeclarationContext context)
		{
			return VisitDeclaration(context);
		}

		Expression VisitArray_or_expression(BooParser.Array_or_expressionContext context)
		{
			var exprs = context.expression();
			var commas = context.COMMA();
			if (exprs == null)
				return new ArrayLiteralExpression(GetLexicalInfo(commas[0]));
			Expression result = VisitExpression(exprs[0]);
			if (commas.Length > 0)
			{
				var tle = new ArrayLiteralExpression(result.LexicalInfo);
				tle.Items.Add(result);
				for (int i = 1; i < exprs.Length; ++i)
					tle.Items.Add(VisitExpression(exprs[i]));
				result = tle;
			}
			return result;
		}

		Node IBooParserVisitor<Node>.VisitArray_or_expression(BooParser.Array_or_expressionContext context)
		{
			return VisitArray_or_expression(context);
		}

		Expression VisitExpression(BooParser.ExpressionContext context)
		{
			Expression result = VisitBoolean_expression(context.boolean_expression());
			ExtendedGeneratorExpression mge = null;
			GeneratorExpression ge = null;
			var gens = context.generator_expression_body();
			if (gens.Length > 0)
			{
				ge = new GeneratorExpression(GetLexicalInfo(context.FOR(0)));
				ge.Expression = result;
				result = ge;
				GetGeneratorExpressionBody(ge, gens[0]);
				for (int i = 1; i < gens.Length; ++i)
				{
					if (mge == null)
					{
						mge = new ExtendedGeneratorExpression(GetLexicalInfo(context.FOR(0)));
						mge.Items.Add(ge);
						result = mge;
					}
					ge = new GeneratorExpression(GetLexicalInfo(context.FOR(i)));
					mge.Items.Add(ge);
					GetGeneratorExpressionBody(ge, gens[i]);
				}
			}
			return result;
		}

		Node IBooParserVisitor<Node>.VisitExpression(BooParser.ExpressionContext context)
		{
			return VisitExpression(context);
		}

		void GetGeneratorExpressionBody(GeneratorExpression ge, BooParser.Generator_expression_bodyContext context)
		{
			AddDeclarations(ge.Declarations, context.declaration_list());
			ge.Iterator = VisitBoolean_expression(context.boolean_expression());
			if (context.stmt_modifier() != null)
				ge.Filter = VisitStmt_modifier(context.stmt_modifier());
		}

		Node IBooParserVisitor<Node>.VisitGenerator_expression_body(BooParser.Generator_expression_bodyContext context)
		{
			throw new NotImplementedException();
		}

		Expression VisitBoolean_expression(BooParser.Boolean_expressionContext context)
		{
			Expression result = VisitBoolean_term(context.boolean_term(0));
			var ors = context.OR();
			if (ors != null)
				for (int i = 0; i < ors.Length; ++i)
				{
					Expression r = VisitBoolean_term(context.boolean_term(i + 1));
					BinaryExpression be = new BinaryExpression(GetLexicalInfo(ors[i]));
					be.Operator = BinaryOperatorType.Or;
					be.Left = result;
					be.Right = r;
					result = be;
				}
			return result;
		}

		Node IBooParserVisitor<Node>.VisitBoolean_expression(BooParser.Boolean_expressionContext context)
		{
			return VisitBoolean_expression(context);
		}

		Expression VisitBoolean_term(BooParser.Boolean_termContext context)
		{
			Expression result = VisitNot_expression(context.not_expression(0));
			var ands = context.AND();
			if (ands != null)
				for (int i = 0; i < ands.Length; ++i)
				{
					Expression r = VisitNot_expression(context.not_expression(i + 1));
					BinaryExpression be = new BinaryExpression(GetLexicalInfo(ands[i]));
					be.Operator = BinaryOperatorType.And;
					be.Left = result;
					be.Right = r;
					result = be;
				}
			return result;
		}

		Node IBooParserVisitor<Node>.VisitBoolean_term(BooParser.Boolean_termContext context)
		{
			return VisitBoolean_term(context);
		}

		MethodInvocationExpression VisitMethod_invocation_block(BooParser.Method_invocation_blockContext context, Expression e)
		{
			MethodInvocationExpression result = e as MethodInvocationExpression ?? new MethodInvocationExpression(e.LexicalInfo, e);
			result.Arguments.Add(VisitCallable_expression(context.callable_expression()));
			return result;
		}

		Node IBooParserVisitor<Node>.VisitMethod_invocation_block(BooParser.Method_invocation_blockContext context)
		{
			throw new NotImplementedException();
		}

		QuasiquoteExpression VisitAst_literal_expression(BooParser.Ast_literal_expressionContext context)
		{
			var result = new QuasiquoteExpression(GetLexicalInfo(context.QQ_BEGIN()));
			if (context.ast_literal_block() != null)
				VisitAst_literal_block(context.ast_literal_block(), result);
			else VisitAst_literal_closure(context.ast_literal_closure(), result);
			SetEndSourceLocation(result, context.QQ_END());
			return result;
		}

		Node IBooParserVisitor<Node>.VisitAst_literal_expression(BooParser.Ast_literal_expressionContext context)
		{
			return VisitAst_literal_expression(context);
		}

		void VisitAst_literal_module(BooParser.Ast_literal_moduleContext context, QuasiquoteExpression e)
		{
			var m = CodeFactory.NewQuasiquoteModule(e.LexicalInfo);
			e.Node = m;
			VisitParse_module(context.parse_module(), m);
		}

		Node IBooParserVisitor<Node>.VisitAst_literal_module(BooParser.Ast_literal_moduleContext context)
		{
			throw new NotImplementedException();
		}

		void VisitAst_literal_block(BooParser.Ast_literal_blockContext context, QuasiquoteExpression e)
		{
			if (context.ast_literal_module() != null)
				VisitAst_literal_module(context.ast_literal_module(), e);
			else
			{
				var members = context.type_definition_member();
				if (members.Length > 0)
				{
					TypeMemberCollection collection = new TypeMemberCollection();
					foreach (var tdm in members)
						collection.Add(VisitType_definition_member(tdm));
					if (collection.Count == 1)
						e.Node = collection[0];
					else
					{
						Module m = CodeFactory.NewQuasiquoteModule(e.LexicalInfo);
						m.Members = collection;
						e.Node = m;
					}
				} else {
					Block b = new Block();
					StatementCollection statements = b.Statements;
					foreach (var stmt in context.stmt())
						statements.Add(VisitStmt(stmt));
					e.Node = b.Statements.Count > 1 ? b : b.Statements[0];
				}
			}
		}

		Node IBooParserVisitor<Node>.VisitAst_literal_block(BooParser.Ast_literal_blockContext context)
		{
			throw new NotImplementedException();
		}

		void VisitAst_literal_closure(BooParser.Ast_literal_closureContext context, QuasiquoteExpression e)
		{
			var exprs = context.expression();
			if (exprs.Length > 0)
			{
				Node node = VisitExpression(exprs[0]);
				if (exprs.Length > 1)
					e.Node = new ExpressionPair(GetLexicalInfo(context.COLON()), (Expression)node, VisitExpression(exprs[1]));
				else e.Node = node;
			}
			else if (context.import_directive_() != null)
				e.Node = VisitImport_directive_(context.import_directive_());
			else {
				var block = new Block();
				foreach (var stmt in context.internal_closure_stmt())
					block.Add(VisitInternal_closure_stmt(stmt));
				if (block.Statements.Count == 1)
					e.Node = block.FirstStatement;
				else e.Node = block;
			}
		}

		Node IBooParserVisitor<Node>.VisitAst_literal_closure(BooParser.Ast_literal_closureContext context)
		{
			throw new NotImplementedException();
		}

		Statement VisitAssignment_or_method_invocation_with_block_stmt(BooParser.Assignment_or_method_invocation_with_block_stmtContext context)
		{
			Expression lhs = VisitSlicing_expression(context.slicing_expression());
			if (context.ASSIGN() == null)
			{
				lhs = VisitMethod_invocation_block(context.method_invocation_block(), lhs);
				return new ExpressionStatement(lhs);
			} else {
				Expression rhs;
				StatementModifier modifier = null;
				if (context.callable_expression() != null)
					rhs = VisitCallable_expression(context.callable_expression());
				else
				{
					rhs = VisitArray_or_expression(context.array_or_expression());
					if (context.method_invocation_block() != null)
						rhs = VisitMethod_invocation_block(context.method_invocation_block(), rhs);
					else if (context.stmt_modifier() != null)
						modifier = VisitStmt_modifier(context.stmt_modifier());
				}
				return new ExpressionStatement(
					lhs.LexicalInfo,
					new BinaryExpression(GetLexicalInfo(context.ASSIGN()),
						OperatorParser.ParseAssignment(context.ASSIGN().GetText()),
						lhs, rhs),
					modifier);
			}
		}

		Node IBooParserVisitor<Node>.VisitAssignment_or_method_invocation_with_block_stmt(BooParser.Assignment_or_method_invocation_with_block_stmtContext context)
		{
			return VisitAssignment_or_method_invocation_with_block_stmt(context);
		}

		Statement VisitAssignment_or_method_invocation(BooParser.Assignment_or_method_invocationContext context)
		{
			Expression lhs = VisitSlicing_expression(context.slicing_expression());
			Expression rhs = VisitArray_or_expression(context.array_or_expression());
			return new ExpressionStatement(
				lhs.LexicalInfo,
				new BinaryExpression(GetLexicalInfo(context.ASSIGN()),
					OperatorParser.ParseAssignment(context.ASSIGN().GetText()),
					lhs, rhs));
		}

		Node IBooParserVisitor<Node>.VisitAssignment_or_method_invocation(BooParser.Assignment_or_method_invocationContext context)
		{
			return VisitAssignment_or_method_invocation(context);
		}

		Expression VisitNot_expression(BooParser.Not_expressionContext context)
		{
			if (context.assignment_expression() != null)
				return VisitAssignment_expression(context.assignment_expression());
			Expression result = VisitNot_expression(context.not_expression());
			return new UnaryExpression(GetLexicalInfo(context.NOT()), UnaryOperatorType.LogicalNot, result);
		}

		Node IBooParserVisitor<Node>.VisitNot_expression(BooParser.Not_expressionContext context)
		{
			return VisitNot_expression(context);
		}

		Expression VisitAssignment_expression(BooParser.Assignment_expressionContext context)
		{
			var result = VisitConditional_expression(context.conditional_expression());
			if (context.assignment_expression() != null)
			{
				var token = context.ASSIGN() ?? context.INPLACE_BITWISE_OR() ?? context.INPLACE_EXCLUSIVE_OR() ?? context.INPLACE_BITWISE_AND() ?? context.INPLACE_SHIFT_LEFT() ?? context.INPLACE_SHIFT_RIGHT();
				BinaryOperatorType binaryOperator = OperatorParser.ParseCondAssignment(token.GetText());
				var r = VisitAssignment_expression(context.assignment_expression());
				result = new BinaryExpression(GetLexicalInfo(token), binaryOperator, result, r);
			}
			return result;
		}

		Node IBooParserVisitor<Node>.VisitAssignment_expression(BooParser.Assignment_expressionContext context)
		{
			return VisitAssignment_expression(context);
		}

		Expression VisitAny_cond_expr_value(BooParser.Any_cond_expr_valueContext context, Expression e)
		{
			Expression r = null;
			BinaryOperatorType op;
			ITerminalNode token;
			if (context.ISA() != null)
			{
				TypeReference tr = VisitType_reference(context.type_reference());
				op = BinaryOperatorType.TypeTest;
				r = new TypeofExpression(tr.LexicalInfo, tr);
				token = context.ISA();
			} else {
				if (context.CMP_OPERATOR() != null)
				{ token = context.CMP_OPERATOR(); op = OperatorParser.ParseComparison(token.GetText()); }
				else if (context.GREATER_THAN() != null)
				{ token = context.GREATER_THAN(); op = BinaryOperatorType.GreaterThan; }
				else if (context.LESS_THAN() != null)
				{ token = context.LESS_THAN(); op = BinaryOperatorType.LessThan; }
				else if (context.IS() != null)
				{
					token = context.IS();
					if (context.NOT() != null)
						op = BinaryOperatorType.ReferenceInequality;
					else op = BinaryOperatorType.ReferenceEquality;
				}
				else if (context.NOT() != null)
				{ token = context.NOT(); op = BinaryOperatorType.NotMember; }
				else { token = context.IN(); op = BinaryOperatorType.Member; }
				r = VisitSum(context.sum());
			}
			return new BinaryExpression(GetLexicalInfo(token), op, e, r);
		}

		Node IBooParserVisitor<Node>.VisitAny_cond_expr_value(BooParser.Any_cond_expr_valueContext context)
		{
			throw new NotImplementedException();
		}

		Expression VisitConditional_expression(BooParser.Conditional_expressionContext context)
		{
			Expression result = VisitSum(context.sum());
			var conds = context.any_cond_expr_value();
			if (conds != null)
			{
				foreach (var expr in conds)
					result = VisitAny_cond_expr_value(expr, result);
			}
			return result;
		}

		Node IBooParserVisitor<Node>.VisitConditional_expression(BooParser.Conditional_expressionContext context)
		{
			return VisitConditional_expression(context);
		}

		Expression VisitAny_sum_value(BooParser.Any_sum_valueContext context, Expression e)
		{
			BinaryOperatorType op;
			ITerminalNode token;
			if (context.ADD() != null)
			{ token = context.ADD(); op = BinaryOperatorType.Addition; }
			else if (context.SUBTRACT() != null)
			{ token = context.SUBTRACT(); op = BinaryOperatorType.Subtraction; }
			else if (context.BITWISE_OR() != null)
			{ token = context.BITWISE_OR(); op = BinaryOperatorType.BitwiseOr; }
			else { token = context.EXCLUSIVE_OR(); op = BinaryOperatorType.ExclusiveOr; }
			Expression r = VisitTerm(context.term());
			return new BinaryExpression(GetLexicalInfo(token), op, e, r);
		}

		Node IBooParserVisitor<Node>.VisitAny_sum_value(BooParser.Any_sum_valueContext context)
		{
			throw new NotImplementedException();
		}

		Expression VisitSum(BooParser.SumContext context)
		{
			Expression result = VisitTerm(context.term());
			var terms = context.any_sum_value();
			if (terms != null)
				foreach (var term in terms)
					result = VisitAny_sum_value(term, result);
			return result;
		}

		Node IBooParserVisitor<Node>.VisitSum(BooParser.SumContext context)
		{
			return VisitSum(context);
		}

		Expression VisitAny_term_value(BooParser.Any_term_valueContext context, Expression e)
		{
			BinaryOperatorType op;
			ITerminalNode token;
			if (context.MULTIPLY() != null)
			{ token = context.MULTIPLY(); op = BinaryOperatorType.Multiply; }
			else if (context.DIVISION() != null)
			{ token = context.DIVISION(); op = BinaryOperatorType.Division; }
			else if (context.MODULUS() != null)
			{ token = context.MODULUS(); op = BinaryOperatorType.Modulus; }
			else { token = context.BITWISE_AND(); op = BinaryOperatorType.BitwiseAnd; }
			Expression r = VisitFactor(context.factor());
			return new BinaryExpression(GetLexicalInfo(token), op, e, r);
		}

		Node IBooParserVisitor<Node>.VisitAny_term_value(BooParser.Any_term_valueContext context)
		{
			throw new NotImplementedException();
		}

		Expression VisitTerm(BooParser.TermContext context)
		{
			Expression result = VisitFactor(context.factor());
			var factors = context.any_term_value();
			if (factors != null)
				foreach (var factor in factors)
					result = VisitAny_term_value(factor, result);
			return result;
		}

		Node IBooParserVisitor<Node>.VisitTerm(BooParser.TermContext context)
		{
			return VisitTerm(context);
		}

		Expression VisitAny_factor_value(BooParser.Any_factor_valueContext context, Expression e)
		{
			BinaryOperatorType op;
			ITerminalNode token;
			if (context.SHIFT_LEFT() != null)
			{ token = context.SHIFT_LEFT(); op = BinaryOperatorType.ShiftLeft; }
			else { token = context.SHIFT_RIGHT(); op = BinaryOperatorType.ShiftRight; }
			Expression r = VisitExponentiation(context.exponentiation());
			return new BinaryExpression(GetLexicalInfo(token), op, e, r);
		}

		Node IBooParserVisitor<Node>.VisitAny_factor_value(BooParser.Any_factor_valueContext context)
		{
			throw new NotImplementedException();
		}

		Expression VisitFactor(BooParser.FactorContext context)
		{
			Expression result = VisitExponentiation(context.exponentiation());
			var exps = context.any_factor_value();
			if (exps != null)
				foreach (var exp in exps)
					result = VisitAny_factor_value(exp, result);
			return result;
		}

		Node IBooParserVisitor<Node>.VisitFactor(BooParser.FactorContext context)
		{
			return VisitFactor(context);
		}

		Expression VisitExponentiation(BooParser.ExponentiationContext context)
		{
			Expression result = VisitUnary_expression(context.unary_expression());
			TypeReference tr;
			if (context.AS() != null)
			{
				tr = VisitType_reference(context.type_reference());
				result = new TryCastExpression(GetLexicalInfo(context.AS()), result, tr);
			}
			else if (context.CAST() != null)
			{
				tr = VisitType_reference(context.type_reference());
				result = new CastExpression(GetLexicalInfo(context.CAST()), result, tr);
			}
			if (context.exponentiation() != null)
			{
				int i = -1;
				ITerminalNode token;
				Expression r;
				foreach (var exp in context.exponentiation())
				{
					++i;
					token = context.EXPONENTIATION(i);
					r = VisitExponentiation(exp);
					result = new BinaryExpression(GetLexicalInfo(token), BinaryOperatorType.Exponentiation, result, r);
				}
			}
			return result;
		}

		Node IBooParserVisitor<Node>.VisitExponentiation(BooParser.ExponentiationContext context)
		{
			return VisitExponentiation(context);
		}

		Expression VisitUnary_expression(BooParser.Unary_expressionContext context)
		{
			UnaryOperatorType op = UnaryOperatorType.None;
			ITerminalNode token = null;
			Expression result;
			if (context.integer_literal() != null)
				return VisitInteger_literal(context.integer_literal());
			if (context.unary_expression() != null)
			{
				if (context.MULTIPLY() != null)
				{ token = context.MULTIPLY(); op = UnaryOperatorType.Explode; }
				else if (context.SUBTRACT() != null)
				{ token = context.SUBTRACT(); op = UnaryOperatorType.UnaryNegation; }
				else if (context.INCREMENT() != null)
				{ token = context.INCREMENT(); op = UnaryOperatorType.Increment; }
				else if (context.DECREMENT() != null)
				{ token = context.DECREMENT(); op = UnaryOperatorType.Decrement; }
				else { token = context.ONES_COMPLEMENT(); op = UnaryOperatorType.OnesComplement; }
				result = VisitUnary_expression(context.unary_expression());
			} else {
				result = VisitSlicing_expression(context.slicing_expression());
				if (context.INCREMENT() != null)
				{ token = context.INCREMENT(); op = UnaryOperatorType.PostIncrement; }
				else if (context.DECREMENT() != null)
				{ token = context.DECREMENT(); op = UnaryOperatorType.PostDecrement; }
			}
			if (token != null)
				result = new UnaryExpression(GetLexicalInfo(token), op, result);
			return result;
		}

		Node IBooParserVisitor<Node>.VisitUnary_expression(BooParser.Unary_expressionContext context)
		{
			return VisitUnary_expression(context);
		}

		Expression VisitAtom(BooParser.AtomContext context)
		{
			if (context.literal() != null)
				return VisitLiteral(context.literal());
			if (context.char_literal() != null)
				return VisitChar_literal(context.char_literal());
			if (context.reference_expression() != null)
				return VisitReference_expression(context.reference_expression());
			if (context.paren_expression() != null)
				return VisitParen_expression(context.paren_expression());
			if (context.cast_expression() != null)
				return VisitCast_expression(context.cast_expression());
			if (context.typeof_expression() != null)
				return VisitTypeof_expression(context.typeof_expression());
			if (context.splice_expression() != null)
				return VisitSplice_expression(context.splice_expression());
			return VisitOmitted_member_expression(context.omitted_member_expression());
		}

		Node IBooParserVisitor<Node>.VisitAtom(BooParser.AtomContext context)
		{
			return VisitAtom(context);
		}

		Expression VisitOmitted_member_expression(BooParser.Omitted_member_expressionContext context)
		{
			var memberName = VisitMember(context.member());
			return MemberReferenceForToken(new OmittedExpression(GetLexicalInfo(context.DOT())), memberName);
		}

		Node IBooParserVisitor<Node>.VisitOmitted_member_expression(BooParser.Omitted_member_expressionContext context)
		{
			return VisitOmitted_member_expression(context);
		}

		SpliceExpression VisitSplice_expression(BooParser.Splice_expressionContext context)
		{
			var e = VisitAtom(context.atom());
			return new SpliceExpression(GetLexicalInfo(context.SPLICE_BEGIN()), e);
		}

		Node IBooParserVisitor<Node>.VisitSplice_expression(BooParser.Splice_expressionContext context)
		{
			return VisitSplice_expression(context);
		}

		Expression VisitChar_literal(BooParser.Char_literalContext context)
		{
			var charToken = context.CHAR();
			var t = context.SINGLE_QUOTED_STRING();
			if (t != null)
				return new CharLiteralExpression(GetLexicalInfo(t), SqsUnquote(t.GetText()));
			var i = context.INT();
			if (i != null)
				return new CharLiteralExpression(GetLexicalInfo(i), (char)Boo.Lang.Parser.PrimitiveParser.ParseInt(i));
			return new MethodInvocationExpression(
				GetLexicalInfo(charToken),
				new ReferenceExpression(GetLexicalInfo(charToken), charToken.GetText()));
		}

		Node IBooParserVisitor<Node>.VisitChar_literal(BooParser.Char_literalContext context)
		{
			return VisitChar_literal(context);
		}

		CastExpression VisitCast_expression(BooParser.Cast_expressionContext context)
		{
			var tr = VisitType_reference(context.type_reference());
			var target = VisitExpression(context.expression());
			return new CastExpression(GetLexicalInfo(context.CAST()), target, tr);
		}

		Node IBooParserVisitor<Node>.VisitCast_expression(BooParser.Cast_expressionContext context)
		{
			return VisitCast_expression(context);
		}

		TypeofExpression VisitTypeof_expression(BooParser.Typeof_expressionContext context)
		{
			var tr = VisitType_reference(context.type_reference());
			return new TypeofExpression(GetLexicalInfo(context.TYPEOF()), tr);
		}

		Node IBooParserVisitor<Node>.VisitTypeof_expression(BooParser.Typeof_expressionContext context)
		{
			return VisitTypeof_expression(context);
		}

		ReferenceExpression VisitReference_expression(BooParser.Reference_expressionContext context)
		{
			var t = context.Start;
			return new ReferenceExpression(GetLexicalInfo(t), t.Text);
		}

		Node IBooParserVisitor<Node>.VisitReference_expression(BooParser.Reference_expressionContext context)
		{
			return VisitReference_expression(context);
		}

		Expression VisitParen_expression(BooParser.Paren_expressionContext context)
		{
			if (context.typed_array() != null)
				return VisitTyped_array(context.typed_array());
			Expression result = VisitArray_or_expression(context.array_or_expression(0));
			if (context.IF() != null)
			{
				var condition = VisitBoolean_expression(context.boolean_expression());
				var falseValue = VisitArray_or_expression(context.array_or_expression(1));
				ConditionalExpression ce = new ConditionalExpression(GetLexicalInfo(context.LPAREN()));
				ce.Condition = condition;
				ce.TrueValue = result;
				ce.FalseValue = falseValue;
				result = ce;
			}
			return result;
		}

		Node IBooParserVisitor<Node>.VisitParen_expression(BooParser.Paren_expressionContext context)
		{
			return VisitParen_expression(context);
		}

		ArrayLiteralExpression VisitTyped_array(BooParser.Typed_arrayContext context)
		{
			var tr = VisitType_reference(context.type_reference());
			var result = new ArrayLiteralExpression(GetLexicalInfo(context.LPAREN())) { Type = new ArrayTypeReference(tr.LexicalInfo, tr) };
			var exprs = context.expression();
			if (exprs != null)
				foreach (var expr in exprs)
					result.Items.Add(VisitExpression(expr));
			return result;
		}

		Node IBooParserVisitor<Node>.VisitTyped_array(BooParser.Typed_arrayContext context)
		{
			return VisitTyped_array(context);
		}

		ITerminalNode VisitMember(BooParser.MemberContext context)
		{
			return context.ID() ?? context.SET() ?? context.GET() ?? context.INTERNAL() ?? context.PUBLIC() ?? context.PROTECTED() ?? context.EVENT() ?? context.REF() ?? context.YIELD();
		}

		Node IBooParserVisitor<Node>.VisitMember(BooParser.MemberContext context)
		{
			throw new NotImplementedException();
		}

		Slice VisitSlice_no_begin(BooParser.Slice_no_beginContext context)
		{
			Expression end = null;
			Expression step = null;
			Expression begin = OmittedExpression.Default;
			if (context.expression() == null)
			{
				end = OmittedExpression.Default;
				step = OmittedExpression.Default;
			}
			else if (context.COLON().Length == 1)
				end = VisitExpression(context.expression());
			else
			{
				end = OmittedExpression.Default;
				step = VisitExpression(context.expression());
			}
			return new Slice(GetLexicalInfo(context.COLON(0)), begin, end, step);
		}

		Node IBooParserVisitor<Node>.VisitSlice_no_begin(BooParser.Slice_no_beginContext context)
		{
			throw new NotImplementedException();
		}

		Slice VisitSlice_with_begin(BooParser.Slice_with_beginContext context)
		{
			var exprs = context.expression();
			Expression begin = VisitExpression(exprs[0]);
			Expression end = null;
			Expression step = null;
			if (exprs.Length > 1)
			{
				if (exprs[1] == null)
					end = OmittedExpression.Default;
				else end = VisitExpression(exprs[1]);
				if (exprs.Length == 3)
					step = VisitExpression(exprs[2]);
			}
			return new Slice(begin.LexicalInfo, begin, end, step);
		}

		Node IBooParserVisitor<Node>.VisitSlice_with_begin(BooParser.Slice_with_beginContext context)
		{
			throw new NotImplementedException();
		}

		void VisitSlice(BooParser.SliceContext context, SlicingExpression se)
		{
			if (context.slice_no_begin() != null)
				se.Indices.Add(VisitSlice_no_begin(context.slice_no_begin()));
			else se.Indices.Add(VisitSlice_with_begin(context.slice_with_begin()));
		}

		Node IBooParserVisitor<Node>.VisitSlice(BooParser.SliceContext context)
		{
			throw new NotImplementedException();
		}

		Expression VisitSafe_atom(BooParser.Safe_atomContext context)
		{
			Expression result = VisitAtom(context.atom());
			if (context.NULLABLE_SUFFIX() != null)
				return new UnaryExpression(result.LexicalInfo, UnaryOperatorType.SafeAccess, result);
			return result;
		}

		Node IBooParserVisitor<Node>.VisitSafe_atom(BooParser.Safe_atomContext context)
		{
			return VisitSafe_atom(context);
		}

		Expression ParseSliceTypeRefList(BooParser.Any_slice_expr_valueContext context, Expression e)
		{
			Expression result;
			var lbrack = context.LBRACK();
			if (context.OF() != null)
			{
				var gre = new GenericReferenceExpression(GetLexicalInfo(lbrack)) { Target = e };
				AddTypeReferences(gre.GenericArguments, context.type_reference_list());
				result = gre;
			} else {
				var se = new SlicingExpression(GetLexicalInfo(lbrack)) { Target = e };
				result = se;
				foreach (var sl in context.slice())
					VisitSlice(sl, se);
			}
			if (context.NULLABLE_SUFFIX() != null)
				result = new UnaryExpression(result.LexicalInfo, UnaryOperatorType.SafeAccess, result);
			return result;
		}

		GenericReferenceExpression ParseSliceGenericType(BooParser.Any_slice_expr_valueContext context, Expression e)
		{
			var genericArgument = VisitType_reference(context.type_reference());
			var result = new GenericReferenceExpression(GetLexicalInfo(context.OF())) { Target = e };
			result.GenericArguments.Add(genericArgument);
			return result;
		}

		Expression ParseSliceDot(BooParser.Any_slice_expr_valueContext context, Expression e)
		{
			Expression result;
			if (context.member() != null)
				result = MemberReferenceForToken(e, VisitMember(context.member()));
			else
			{
				var begin = context.SPLICE_BEGIN();
				var nameSplice = VisitAtom(context.atom());
				result = new SpliceMemberReferenceExpression(
					GetLexicalInfo(begin),
					e,
					nameSplice);
			}
			if (context.NULLABLE_SUFFIX() != null)
				result = new UnaryExpression(result.LexicalInfo, UnaryOperatorType.SafeAccess, result);
			return result;
		}

		Expression ParseSliceMethod(BooParser.Any_slice_expr_valueContext context, Expression e)
		{
			var lparen = context.LPAREN();
			Expression result = new MethodInvocationExpression(GetLexicalInfo(lparen), e);
			var args = context.argument();
			Expression initializer = null;
			if (args != null)
				foreach (var arg in args)
					VisitArgument(arg, (MethodInvocationExpression)result);
			if (context.NULLABLE_SUFFIX() != null)
				result = new UnaryExpression(result.LexicalInfo, UnaryOperatorType.SafeAccess, result);
			if (context.hash_literal() != null)
				initializer = VisitHash_literal(context.hash_literal());
			else if (context.list_initializer() != null)
				initializer = VisitList_initializer(context.list_initializer());
			if (initializer != null)
				result = new CollectionInitializationExpression(result, initializer);
			return result;
		}

		Expression VisitAny_slice_expr_value(BooParser.Any_slice_expr_valueContext context, Expression e)
		{
			if (context.LBRACK() != null)
				return ParseSliceTypeRefList(context, e);
			if (context.OF() != null)
				return ParseSliceGenericType(context, e);
			if (context.DOT() != null)
				return ParseSliceDot(context, e);
			return ParseSliceMethod(context, e);
		}

		Node IBooParserVisitor<Node>.VisitAny_slice_expr_value(BooParser.Any_slice_expr_valueContext context)
		{
			throw new NotImplementedException();
		}

		Expression VisitSlicing_expression(BooParser.Slicing_expressionContext context)
		{
			Expression result = VisitSafe_atom(context.safe_atom());
			var modifiers = context.any_slice_expr_value();
			if (modifiers != null)
				foreach (var mod in modifiers)
					result = VisitAny_slice_expr_value(mod, result);
			return result;
		}

		Node IBooParserVisitor<Node>.VisitSlicing_expression(BooParser.Slicing_expressionContext context)
		{
			return VisitSlicing_expression(context);
		}

		ListLiteralExpression VisitList_initializer(BooParser.List_initializerContext context)
		{
			var result = new ListLiteralExpression(GetLexicalInfo(context.LBRACE()));
			AddListItems(result.Items, context.list_items());
			return result;
		}

		Node IBooParserVisitor<Node>.VisitList_initializer(BooParser.List_initializerContext context)
		{
			return VisitList_initializer(context);
		}

		Expression VisitLiteral(BooParser.LiteralContext context)
		{
			return (Expression)Visit(context.GetChild(0));
		}

		Node IBooParserVisitor<Node>.VisitLiteral(BooParser.LiteralContext context)
		{
			return VisitLiteral(context);
		}

		SelfLiteralExpression VisitSelf_literal(BooParser.Self_literalContext context)
		{
			return new SelfLiteralExpression(GetLexicalInfo(context.SELF()));
		}

		Node IBooParserVisitor<Node>.VisitSelf_literal(BooParser.Self_literalContext context)
		{
			return VisitSelf_literal(context);
		}

		SuperLiteralExpression VisitSuper_literal(BooParser.Super_literalContext context)
		{
			return new SuperLiteralExpression(GetLexicalInfo(context.SUPER()));
		}

		Node IBooParserVisitor<Node>.VisitSuper_literal(BooParser.Super_literalContext context)
		{
			return VisitSuper_literal(context);
		}

		NullLiteralExpression VisitNull_literal(BooParser.Null_literalContext context)
		{
			return new NullLiteralExpression(GetLexicalInfo(context.NULL()));
		}

		Node IBooParserVisitor<Node>.VisitNull_literal(BooParser.Null_literalContext context)
		{
			return VisitNull_literal(context);
		}

		BoolLiteralExpression VisitBool_literal(BooParser.Bool_literalContext context)
		{
			if (context.TRUE() != null)
				return new BoolLiteralExpression(GetLexicalInfo(context.TRUE()), true);
			return new BoolLiteralExpression(GetLexicalInfo(context.FALSE()), false);
		}

		Node IBooParserVisitor<Node>.VisitBool_literal(BooParser.Bool_literalContext context)
		{
			return VisitBool_literal(context);
		}

		IntegerLiteralExpression VisitInteger_literal(BooParser.Integer_literalContext context)
		{
			string number = null;
			ITerminalNode sign = context.SUBTRACT();
			var i = context.INT();
			if (i != null)
			{
				number = sign != null ? sign.GetText() + i.GetText() : i.GetText();
				return PrimitiveParser.ParseIntegerLiteralExpression(GetLexicalInfo(i), number, false);
			}
			var l = context.LONG();
			{
				number = sign != null ? sign.GetText() + l.GetText() : l.GetText();
				return PrimitiveParser.ParseIntegerLiteralExpression(GetLexicalInfo(l), number, true);
			}
		}

		Node IBooParserVisitor<Node>.VisitInteger_literal(BooParser.Integer_literalContext context)
		{
			return VisitInteger_literal(context);
		}

		string SqsUnquote(string value)
		{
			if (value.StartsWith("'") && value.EndsWith("'"))
				return value.Substring(1, value.Length - 2);
			throw new FormatException(string.Format("[{0}] is not a single-quoted string.", value));
		}

		string DqsUnquote(string value)
		{
			if (value.StartsWith("\"") && value.EndsWith("\""))
				return value.Substring(1, value.Length - 2);
			throw new FormatException(string.Format("[{0}] is not a double-quoted string.", value));
		}

		string TqsUnquote(string value)
		{
			if (value.StartsWith("\"\"\"") && value.EndsWith("\"\"\""))
				return value.Substring(3, value.Length - 6);
			throw new FormatException(string.Format("[{0}] is not a triple-quoted string.", value));
		}

		string BqsUnquote(string value)
		{
			if (value.StartsWith("`") && value.EndsWith("`"))
				return value.Substring(1, value.Length - 2);
			throw new FormatException(string.Format("[{0}] is not a backtick-quoted string.", value));
		}

		Expression VisitString_literal(BooParser.String_literalContext context)
		{
			StringLiteralExpression e = null;
			if (context.expression_interpolation() != null)
				return VisitExpression_interpolation(context.expression_interpolation());
			var dqs = context.DOUBLE_QUOTED_STRING();
			if (dqs != null)
			{
				e = new StringLiteralExpression(GetLexicalInfo(dqs), DqsUnquote(dqs.GetText()));
				e.Annotate("quote", "\"");
				return e;
			}
			var sqs = context.SINGLE_QUOTED_STRING();
			if (sqs != null)
			{
				e = new StringLiteralExpression(GetLexicalInfo(sqs), SqsUnquote(sqs.GetText()));
				e.Annotate("quote", "'");
				return e;
			}
			var tqs = context.TRIPLE_QUOTED_STRING();
			if (tqs != null)
			{
				e = new StringLiteralExpression(GetLexicalInfo(tqs), TqsUnquote(tqs.GetText()));
				e.Annotate("quote", "\"\"\"");
				return e;
			}
			var bqs = context.BACKTICK_QUOTED_STRING();
			e = new StringLiteralExpression(GetLexicalInfo(bqs), BqsUnquote(bqs.GetText()));
			e.Annotate("quote", "`");
			return e;
		}

		Node IBooParserVisitor<Node>.VisitString_literal(BooParser.String_literalContext context)
		{
			return VisitString_literal(context);
		}

		ExpressionInterpolationExpression VisitAny_expr_interpolation_item(BooParser.Any_expr_interpolation_itemContext context, ExpressionInterpolationExpression e)
		{
			var startsep = context.ESEPARATOR(0);
			if (e == null)
				e = new ExpressionInterpolationExpression(GetLexicalInfo(startsep));
			var param = VisitExpression(context.expression());
			ITerminalNode formatString = null;
			ITerminalNode formatSep = null;
			if (context.ID() != null)
			{
				formatString = context.ID();
				formatSep = context.COLON();
			}
			e.Expressions.Add(param);
			if (formatString != null)
				param.Annotate("formatString", formatString.GetText());
			return e;
		}

		Node IBooParserVisitor<Node>.VisitAny_expr_interpolation_item(BooParser.Any_expr_interpolation_itemContext context)
		{
			throw new NotImplementedException();
		}

		ExpressionInterpolationExpression VisitExpression_interpolation(BooParser.Expression_interpolationContext context)
		{
			ExpressionInterpolationExpression result = null;
			foreach (var item in context.any_expr_interpolation_item())
				result = VisitAny_expr_interpolation_item(item, result);
			return result;			
		}

		Node IBooParserVisitor<Node>.VisitExpression_interpolation(BooParser.Expression_interpolationContext context)
		{
			return VisitExpression_interpolation(context);
		}

		ListLiteralExpression VisitList_literal(BooParser.List_literalContext context)
		{
			var lbrack = context.LBRACK();
			var result = new ListLiteralExpression(GetLexicalInfo(lbrack));
			AddListItems(result.Items, context.list_items());
			return result;
		}

		Node IBooParserVisitor<Node>.VisitList_literal(BooParser.List_literalContext context)
		{
			return VisitList_literal(context);
		}

		void AddListItems(ExpressionCollection items, BooParser.List_itemsContext context)
		{
			foreach (var expr in context.expression())
				items.Add(VisitExpression(expr));
		}

		Node IBooParserVisitor<Node>.VisitList_items(BooParser.List_itemsContext context)
		{
			throw new NotImplementedException();
		}

		HashLiteralExpression VisitHash_literal(BooParser.Hash_literalContext context)
		{
			var result = new HashLiteralExpression(GetLexicalInfo(context.LBRACE()));
			foreach (var pair in context.expression_pair())
				result.Items.Add(VisitExpression_pair(pair));
			return result;
		}

		Node IBooParserVisitor<Node>.VisitHash_literal(BooParser.Hash_literalContext context)
		{
			return VisitHash_literal(context);
		}

		ExpressionPair VisitExpression_pair(BooParser.Expression_pairContext context)
		{
			var key = VisitExpression(context.expression(0));
			var t = context.COLON();
			var value = VisitExpression(context.expression(1));
			return new ExpressionPair(GetLexicalInfo(t), key, value);
		}

		Node IBooParserVisitor<Node>.VisitExpression_pair(BooParser.Expression_pairContext context)
		{
			return VisitExpression_pair(context);
		}

		RELiteralExpression VisitRe_literal(BooParser.Re_literalContext context)
		{
			var value = context.RE_LITERAL();
			return new RELiteralExpression(GetLexicalInfo(value), value.GetText());
		}

		Node IBooParserVisitor<Node>.VisitRe_literal(BooParser.Re_literalContext context)
		{
			return VisitRe_literal(context);
		}

		DoubleLiteralExpression VisitDouble_literal(BooParser.Double_literalContext context)
		{
			string val;
			var neg = context.SUBTRACT();
			var value = context.DOUBLE();
			if (value != null)
			{
				val = value.GetText();
				if (neg != null) val = neg.GetText() + val;
				return new DoubleLiteralExpression(GetLexicalInfo(value), PrimitiveParser.ParseDouble(GetLexicalInfo(value), val));
			}
			var single = context.FLOAT();
			val = single.GetText();
			val = val.Substring(0, val.Length - 1);
			if (neg != null) val = neg.GetText() + val;
			return new DoubleLiteralExpression(GetLexicalInfo(single), PrimitiveParser.ParseDouble(GetLexicalInfo(single), val, true), true);
		}

		Node IBooParserVisitor<Node>.VisitDouble_literal(BooParser.Double_literalContext context)
		{
			return VisitDouble_literal(context);
		}

		TimeSpanLiteralExpression VisitTimespan_literal(BooParser.Timespan_literalContext context)
		{
			var neg = context.SUBTRACT();
			var value = context.TIMESPAN();
			string val = value.GetText();
			if (neg != null) val = neg.GetText() + val;
			return new TimeSpanLiteralExpression(GetLexicalInfo(value), PrimitiveParser.ParseTimeSpan(GetLexicalInfo(value), val));
		}

		Node IBooParserVisitor<Node>.VisitTimespan_literal(BooParser.Timespan_literalContext context)
		{
			return VisitTimespan_literal(context);
		}

		void GetExpressionList(ExpressionCollection ec, BooParser.Expression_listContext context)
		{
			if (context != null)
				foreach (var expr in context.expression())
					ec.Add(VisitExpression(expr));
		}

		Node IBooParserVisitor<Node>.VisitExpression_list(BooParser.Expression_listContext context)
		{
			throw new NotImplementedException();
		}

		void VisitArgument(BooParser.ArgumentContext context, INodeWithArguments node)
		{
			if (context.expression_pair() != null)
				node.NamedArguments.Add(VisitExpression_pair(context.expression_pair()));
			else node.Arguments.Add(VisitExpression(context.expression()));
		}

		Node IBooParserVisitor<Node>.VisitArgument(BooParser.ArgumentContext context)
		{
			throw new NotImplementedException();
		}

		string VisitIdentifier(BooParser.IdentifierContext context)
		{
			_sbuilder.Clear();
			var id1 = context.macro_name();
			_sbuilder.Append(id1.GetText());
			var members = context.member();
			if (members != null)
				foreach (var id2 in members)
				{
					_sbuilder.Append('.');
					_sbuilder.Append(id2.GetText());
				}
			return _sbuilder.ToString();
		}

		Node IBooParserVisitor<Node>.VisitIdentifier(BooParser.IdentifierContext context)
		{
			throw new NotImplementedException();
		}
	}
}
