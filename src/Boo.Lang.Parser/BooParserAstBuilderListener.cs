namespace Boo.Lang.ParserV4
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Antlr4.Runtime;
    using Antlr4.Runtime.Tree;
    using Boo.Lang.Compiler.Ast;
    using DocStringFormatter = Boo.Lang.Parser.DocStringFormatter;

    internal class BooParserAstBuilderListener : AbstractParseTreeVisitor<object>, IBooParserVisitor<object>
    {
        private readonly CompileUnit _compileUnit;
        private readonly Module _module;

        private readonly ParseTreeProperty<MacroStatement> _macroStatement = new ParseTreeProperty<MacroStatement>();
        private readonly ParseTreeProperty<Node> _node = new ParseTreeProperty<Node>();
        private readonly ParseTreeProperty<Import> _import = new ParseTreeProperty<Import>();

        public BooParserAstBuilderListener(CompileUnit compileUnit)
        {
            if (compileUnit == null)
                throw new ArgumentNullException("compileUnit");

            _compileUnit = compileUnit;
            _module = new Module();
        }

        private string FileName
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public Module VisitStart(BooParser.StartContext context)
        {
            _module.LexicalInfo = new LexicalInfo(FileName, 1, 1);
            _compileUnit.Modules.Add(_module);

            VisitChildren(context);

            SetEndSourceLocation(_module, context.Eof());

            return _module;
        }

        object IBooParserVisitor<object>.VisitStart(BooParser.StartContext context)
        {
            return VisitStart(context);
        }

        public void VisitParse_module(BooParser.Parse_moduleContext context)
        {
            VisitChildren(context);
        }

        object IBooParserVisitor<object>.VisitParse_module(BooParser.Parse_moduleContext context)
        {
            VisitParse_module(context);
            return null;
        }

        public void VisitModule_macro(BooParser.Module_macroContext context)
        {
            MacroStatement macroStatement = VisitMacro_stmt(context.macro_stmt());
            _module.Globals.Add(macroStatement);
        }

        object IBooParserVisitor<object>.VisitModule_macro(BooParser.Module_macroContext context)
        {
            VisitModule_macro(context);
            return null;
        }

        public void VisitDocstring(BooParser.DocstringContext context)
        {
            Node node = _node.Get(context);
            if (node == null)
                throw new InvalidOperationException();

            VisitChildren(context);

            node.Documentation = DocStringFormatter.Format(context.TRIPLE_QUOTED_STRING().GetText());
        }

        object IBooParserVisitor<object>.VisitDocstring(BooParser.DocstringContext context)
        {
            VisitDocstring(context);
            return null;
        }

        public void VisitEos(BooParser.EosContext context)
        {
            VisitChildren(context);
        }

        object IBooParserVisitor<object>.VisitEos(BooParser.EosContext context)
        {
            VisitEos(context);
            return null;
        }

        public void VisitImport_directive(BooParser.Import_directiveContext context)
        {
            VisitChildren(context);

            Import node = null;
            if (context.import_directive_() != null)
                node = _import.Get(context.import_directive_());
            else if (context.import_directive_from_() != null)
                node = _import.Get(context.import_directive_from_());

            if (node != null)
                _module.Imports.Add(node);
        }

        object IBooParserVisitor<object>.VisitImport_directive(BooParser.Import_directiveContext context)
        {
            VisitImport_directive(context);
            return null;
        }

        public ReferenceExpression VisitIdentifier_expression(BooParser.Identifier_expressionContext context)
        {
            throw new NotImplementedException();
        }

        object IBooParserVisitor<object>.VisitIdentifier_expression(BooParser.Identifier_expressionContext context)
        {
            return VisitIdentifier_expression(context);
        }

        private void SetEndSourceLocation(Node node, ITerminalNode terminalNode)
        {
            throw new NotImplementedException();
        }
    }
}
