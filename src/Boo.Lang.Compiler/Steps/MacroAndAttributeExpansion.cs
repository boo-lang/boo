#region license
// Copyright (c) 2004-2009, Rodrigo B. de Oliveira (rbo@acm.org)
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

using Boo.Lang.Compiler.Ast;
using Boo.Lang.Compiler.Steps.MacroProcessing;
using Boo.Lang.Compiler.TypeSystem.Services;

namespace Boo.Lang.Compiler.Steps
{
	public class MacroAndAttributeExpansion : AbstractCompilerStep, ITypeMemberReifier, IStatementReifier, IExpressionReifier
	{
		private BindAndApplyAttributes _attributes = new BindAndApplyAttributes();
		private MacroExpander _macroExpander = new MacroExpander();

		private static int _macroAndAttributeCount = 0;

		public override void Initialize(CompilerContext context)
		{
			base.Initialize(context);
			_attributes.Initialize(context);
			_macroExpander.Initialize(context);
		}

		public override void Run()
		{	
			RunExpansionIterations();
			_macroAndAttributeCount = Attribute.ConstructionCount + MacroStatement.ConstructionCount;
		}

		private void RunExpansionIterations()
		{
			int iteration = 0;
			while (true)
			{
				bool expanded = ApplyAttributesAndExpandMacros();
				if (!expanded)
				{
					if (!BubbleResultingTypeMemberStatementsUp())
						break;
				}

				++iteration;
				if (iteration > Parameters.MaxExpansionIterations)
					throw new CompilerError("Too many expansions.");
			}
		}

		private bool BubbleResultingTypeMemberStatementsUp()
		{
			return TypeMemberStatementBubbler.BubbleTypeMemberStatementsUp(CompileUnit);
		}

		private bool ApplyAttributesAndExpandMacros()
		{
			bool attributesApplied = _attributes.BindAndApply();
			bool macrosExpanded = _macroExpander.ExpandAll();
			return attributesApplied || macrosExpanded;
		}

		private bool ShouldReify()
		{
			var result = _macroAndAttributeCount != Attribute.ConstructionCount + MacroStatement.ConstructionCount;
			if (result)
			{
				_macroAndAttributeCount = Attribute.ConstructionCount + MacroStatement.ConstructionCount;
			}
			return result;
		}

		public TypeMember Reify(TypeMember node)
		{
			if (ShouldReify())
			{
				RunExpansionIterations();
			}
			return node;
		}

		public Statement Reify(Statement node)
		{
			var result = node;
			if (ShouldReify())
			{
				if (node is MacroStatement)
				{
					// macro statements are replaced
					// so we need to wrap it in a Block
					// otherwise we would lose the result
					var parentNode = node.ParentNode;
					result = new Block(node);
					parentNode.Replace(node, result);
				}
				RunExpansionIterations();
			}

			return result;
		}

		public Expression Reify(Expression node)
		{
			if (ShouldReify())
			{
				RunExpansionIterations();
			}
			return node;
		}
	}
}
