using System;
using Boo.Lang.Compiler.Ast;
using Boo.Lang.Compiler.Steps.MacroProcessing;

namespace Boo.Lang.Compiler.Steps
{
	public class MacroAndAttributeExpansion : AbstractCompilerStep
	{
		private BindAndApplyAttributes _attributes = new BindAndApplyAttributes();
		private MacroExpander _macroExpander = new MacroExpander();

		public override void Initialize(CompilerContext context)
		{
			base.Initialize(context);
			_attributes.Initialize(context);
			_macroExpander.Initialize(context);
		}

		public override void Run()
		{
			ExpandExternalMacros();
			ExpandInternalMacros();
		}

		private void ExpandInternalMacros()
		{
			_macroExpander.ExpandingInternalMacros = true;
			RunExpansionIterations();
			
		}

		private void ExpandExternalMacros()
		{
			_macroExpander.ExpandingInternalMacros = false;
			RunExpansionIterations();
		}

		private void RunExpansionIterations()
		{
			int iteration = 0;
			while (true)
			{
				bool expanded = ApplyAttributesAndExpandMacros();
				if (!expanded)
					break;
				
				BubbleResultingTypeMemberStatementsUp();

				++iteration;
				if (iteration > Parameters.MaxExpansionIterations)
					throw new CompilerError("Too many expansions.");
			}
		}

		private void BubbleResultingTypeMemberStatementsUp()
		{
			CompileUnit.Accept(new TypeMemberStatementBubbler());
		}

		class TypeMemberStatementBubbler : DepthFirstTransformer, ITypeMemberStatementVisitor
		{
			private TypeDefinition _current = null;

			protected override void OnNode(Node node)
			{
				TypeDefinition typeDefinition = node as TypeDefinition;
				if (null == typeDefinition)
				{
					base.OnNode(node);
					return;
				}

				TypeDefinition previous = _current;
				try
				{
					_current = typeDefinition;
					base.OnNode(node);
				}
				finally
				{
					_current = previous;
				}
			}

			#region Implementation of ITypeMemberStatementVisitor

			public void OnTypeMemberStatement(TypeMemberStatement node)
			{
				_current.Members.Add(node.TypeMember);
				Visit(node.TypeMember);
				RemoveCurrentNode();
			}

			#endregion
		}

		private bool ApplyAttributesAndExpandMacros()
		{
			bool attributesApplied = _attributes.BindAndApply();
			bool macrosExpanded = _macroExpander.ExpandAll();
			return attributesApplied || macrosExpanded;
		}
	}
}
