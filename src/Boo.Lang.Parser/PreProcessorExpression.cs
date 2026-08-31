#region license
// Copyright (c) the Boo contributors
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

namespace Boo.Lang.Parser.PreProcessor;

using System.Collections;
using Antlr4.Runtime;
using Antlr4.Runtime.Misc;

/// <summary>
/// Decides whether a #if condition in Boo.Lang.Useful's preprocessor holds.
///
/// The condition language is symbols combined with &amp;&amp;, || and !, defined
/// by PreProcessorExpressions.g4. A symbol is true when the table names it.
/// </summary>
public static class PreProcessorExpression
{
	/// <summary>
	/// Throws <see cref="ParseCanceledException"/> if the condition is malformed,
	/// so a typo in a #if is reported rather than silently read as false.
	/// </summary>
	public static bool Evaluate(string expression, IDictionary definedSymbols)
	{
		var lexer = new PreProcessorExpressionsLexer(new AntlrInputStream(expression));
		lexer.RemoveErrorListeners();

		var parser = new PreProcessorExpressionsParser(new CommonTokenStream(lexer));
		parser.RemoveErrorListeners();
		parser.ErrorHandler = new BailErrorStrategy();

		return new Evaluator(definedSymbols).Visit(parser.start_());
	}

	private sealed class Evaluator : PreProcessorExpressionsBaseVisitor<bool>
	{
		private readonly IDictionary _definedSymbols;

		internal Evaluator(IDictionary definedSymbols)
		{
			_definedSymbols = definedSymbols;
		}

		public override bool VisitStart_(PreProcessorExpressionsParser.Start_Context context) =>
			Visit(context.expression());

		public override bool VisitExpression(PreProcessorExpressionsParser.ExpressionContext context)
		{
			foreach (var conjunction in context.conjunction())
				if (Visit(conjunction))
					return true;
			return false;
		}

		public override bool VisitConjunction(PreProcessorExpressionsParser.ConjunctionContext context)
		{
			foreach (var atom in context.atom())
				if (!Visit(atom))
					return false;
			return true;
		}

		public override bool VisitSymbol(PreProcessorExpressionsParser.SymbolContext context) =>
			IsDefined(context.ID().GetText());

		public override bool VisitNegatedSymbol(PreProcessorExpressionsParser.NegatedSymbolContext context) =>
			!IsDefined(context.ID().GetText());

		public override bool VisitGroup(PreProcessorExpressionsParser.GroupContext context) =>
			Visit(context.expression());

		private bool IsDefined(string symbol) => _definedSymbols.Contains(symbol);
	}
}
