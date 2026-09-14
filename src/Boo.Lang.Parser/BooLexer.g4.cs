#region license
// Copyright (c) 2026 the Boo contributors
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

namespace Boo.Lang.Parser;

using System;
using System.Collections.Generic;
using Antlr4.Runtime;

/// <summary>
/// The helpers BooLexer.g4's actions call.
/// </summary>
partial class BooLexer
{
	protected int _skipWhitespaceRegion = 0;

	private readonly Stack<int> _beginInterpolationType = new();
	private readonly Stack<int> _endInterpolationType = new();

	private IDictionary<string, ReaderMacro> _readerMacros = new Dictionary<string, ReaderMacro>();
	private IDictionary<string, IDictionary<string, ReaderMacro>> _readerMacrosByNamespace = new Dictionary<string, IDictionary<string, ReaderMacro>>();

	// Whether _readerMacros is a copy this lexer may add imported reader macros to.
	private bool _ownsReaderMacros;
	private bool _started;

	// The namespace an import or namespace line names, while that line is lexed.
	private System.Text.StringBuilder _importedNamespace;
	private bool _importedNamespaceEnded;
	private bool _importIsNamespace;
	private int _previousTokenType;

	private static readonly string[] GlobalNamespaces = { "", "Boo.Lang", "Boo.Lang.Extensions" };

	private string _moduleNamespace = "";

	// The tokens of a macro definition's header, after macro and before its colon.
	private List<IToken> _macroHeader;
	private IToken _macroHeaderStart;
	private int _macroHeaderDepth;

	// The names that started a macro statement so far, to tell a definition came too late.
	private readonly HashSet<string> _lineStartNames = new(StringComparer.Ordinal);

	// What the module's own macro definitions declare, by name.
	private readonly Dictionary<string, ReaderMacro> _definedReaderMacros = new(StringComparer.Ordinal);

	// The macros defined inside each macro defined in the module, by the parent's type name.
	private readonly Dictionary<string, IDictionary<string, ReaderMacro>> _definedNestedMacros = new(StringComparer.Ordinal);

	/// <summary>
	/// Told the namespace, name and reader macro of each name a macro definition
	/// in the module declares.
	/// </summary>
	internal Action<string, string, ReaderMacro> SyntaxDeclared { get; set; }

	/// <summary>
	/// Told of a macro definition whose verbatim text cannot apply where the
	/// macro was already used.
	/// </summary>
	internal Action<Boo.Lang.Compiler.Ast.LexicalInfo, string> SyntaxDeclarationWarning { get; set; }

	private int _lineIndent;
	private int _lineTokenCount;
	private IToken _lineFirstToken;
	private int _lineSecondTokenType;

	private IToken _pendingVerbatimToken;

	/// <summary>
	/// A block that changes how the lines inside it are read: that of a reader
	/// macro, or of a macro definition, whose nested definitions belong to it.
	/// </summary>
	private sealed class BlockScope
	{
		public int Indent;
		public int OpenBlocks;
		public ReaderMacro Macro;
		public string DefinedMacroType;
	}

	private readonly Stack<BlockScope> _scopes = new();

	/// <summary>
	/// Reader macros by the name of the macro they read for.
	/// </summary>
	public IDictionary<string, ReaderMacro> ReaderMacros
	{
		get => _readerMacros;
		set => _readerMacros = value ?? new Dictionary<string, ReaderMacro>();
	}

	/// <summary>
	/// Reader macros by namespace, applied once the module imports their namespace.
	/// </summary>
	public IDictionary<string, IDictionary<string, ReaderMacro>> ReaderMacrosByNamespace
	{
		get => _readerMacrosByNamespace;
		set => _readerMacrosByNamespace = value ?? new Dictionary<string, IDictionary<string, ReaderMacro>>();
	}

	/// <summary>
	/// Whether blocks close with end rather than by dedenting.
	/// </summary>
	public bool WhitespaceAgnostic { get; set; }

	public override IToken NextToken()
	{
		if (!_started)
		{
			_started = true;
			foreach (var ns in GlobalNamespaces)
				Import(ns);
		}

		if (_pendingVerbatimToken != null)
		{
			var pending = _pendingVerbatimToken;
			_pendingVerbatimToken = null;
			return pending;
		}

		var verbatim = TryReadBlockLine();
		if (verbatim != null)
			return verbatim;

		var token = base.NextToken();
		TrackLine(token);
		return token;
	}

	/// <summary>
	/// At the start of a line in a reader macro's block, asks the reader macro
	/// how the line reads. This runs before the line is lexed, so verbatim text
	/// need not be made of Boo tokens.
	/// </summary>
	private IToken TryReadBlockLine()
	{
		if (_lineTokenCount > 0 || _scopes.Count == 0 || SkipWhitespace
			|| _beginInterpolationType.Count > 0 || CurrentMode != DEFAULT_MODE)
			return null;

		var input = (ICharStream)InputStream;
		var c = CharAt(input, input.Index);
		if (c is IntStreamConstants.EOF or ' ' or '\t' or '\f' || IsLineBreak(c))
			return null;

		PopScopesEndedBy(_lineIndent);
		var reader = LineReader();
		if (reader == null)
			return null;

		var line = LineText(input, input.Index);
		var pos = 0;
		if (ReaderMacroNamed(ReaderMacro.ReadName(line, ref pos)) != null || reader.ReadBlockLine(line) != LineSyntax.Verbatim)
			return null;

		_lineTokenCount = 1;
		return CaptureVerbatimLine(Line);
	}

	// The innermost reader macro that reads the lines of its block.
	private ReaderMacro LineReader()
	{
		foreach (var scope in _scopes)
			if (scope.Macro?.ReadsBlockLines == true)
				return scope.Macro;
		return null;
	}

	// The reader macro for a name, nested in an enclosing reader macro's block first.
	private ReaderMacro ReaderMacroNamed(string name)
	{
		foreach (var scope in _scopes)
			if (scope.Macro != null && scope.Macro.NestedMacros.TryGetValue(name, out var nested))
				return nested;
		return _readerMacros.TryGetValue(name, out var macro) ? macro : null;
	}

	private void PopScopesEndedBy(int indent)
	{
		if (!WhitespaceAgnostic)
			while (_scopes.Count > 0 && indent <= _scopes.Peek().Indent)
				_scopes.Pop();
	}

	private void TrackLine(IToken token)
	{
		if (token.Channel != TokenConstants.DefaultChannel)
			return;
		if (token.Type == NEWLINE)
		{
			if (_importedNamespace != null)
				EndImport();
			if (_lineTokenCount == 1)
				NoteMacroStartingLine();
			_macroHeader = null;
			_lineIndent = 0;
			_lineTokenCount = 0;
			_lineFirstToken = null;
			return;
		}
		if (token.Type == WS)
		{
			if (_lineTokenCount == 0)
				_lineIndent = token.Text.Length;
			return;
		}

		var previousTokenType = _previousTokenType;
		_previousTokenType = token.Type;

		if (++_lineTokenCount == 1)
		{
			_lineFirstToken = token;
			PopScopesEndedBy(_lineIndent);
			if (ReadsRestOfLine(token))
			{
				_pendingVerbatimToken = CaptureVerbatimLine(token.Line);
				return;
			}
		}
		else if (_lineTokenCount == 2)
		{
			_lineSecondTokenType = token.Type;
			NoteMacroStartingLine();
			if (_lineFirstToken.Type == ID && _lineFirstToken.Text == "macro" && token.Type == ID)
			{
				_macroHeader = new List<IToken>();
				_macroHeaderStart = _lineFirstToken;
				_macroHeaderDepth = 0;
			}
		}

		if (token.Type is IMPORT or NAMESPACE && (_lineTokenCount == 1 || previousTokenType == EOS))
		{
			_importedNamespace = new System.Text.StringBuilder();
			_importedNamespaceEnded = false;
			_importIsNamespace = token.Type == NAMESPACE;
			return;
		}
		if (_importedNamespace != null)
		{
			TrackImport(token);
			return;
		}
		if (_macroHeader != null && TrackMacroHeader(token))
			return;

		if (WhitespaceAgnostic && _scopes.Count > 0 && ClosesBlock(token)
			&& --_scopes.Peek().OpenBlocks < 0)
			_scopes.Pop();

		if (token.Type != COLON || SkipWhitespace || _beginInterpolationType.Count > 0)
			return;

		var input = (ICharStream)InputStream;
		var macro = MacroStartingLine();
		if (macro?.ReadsBlock == true)
			_pendingVerbatimToken = CaptureVerbatimBlock(token);
		if (_pendingVerbatimToken != null)
			return;

		if (macro is { ReadsBlock: false } && (macro.ReadsBlockLines || macro.NestedMacros.Count > 0) && EndsLine(input))
			_scopes.Push(new BlockScope { Indent = _lineIndent, Macro = macro });
		else if (WhitespaceAgnostic && _scopes.Count > 0 && OpensBlock(input))
			_scopes.Peek().OpenBlocks++;
	}

	// Only a plain import names a namespace: import a.b, optionally from an assembly.
	private void TrackImport(IToken token)
	{
		if (token.Type == EOS)
			EndImport();
		else if (token.Type == AS || (!_importedNamespaceEnded && token.Type is not (ID or DOT or FROM)))
			_importedNamespace = null;
		else if (token.Type == FROM)
			_importedNamespaceEnded = true;
		else if (!_importedNamespaceEnded)
			_importedNamespace.Append(token.Text);
	}

	// Called once the line's second token, if any, is known.
	private void NoteMacroStartingLine()
	{
		if (_lineFirstToken?.Type == ID && (_lineTokenCount == 1 || _lineSecondTokenType is not (ASSIGN or LPAREN or LBRACK or DOT or MULTIPLY)))
			_lineStartNames.Add(_lineFirstToken.Text);
	}

	private void EndImport()
	{
		var ns = _importedNamespace.ToString();
		if (_importIsNamespace)
			_moduleNamespace = ns;
		Import(ns);
		_importedNamespace = null;
	}

	private void Import(string ns)
	{
		if (_readerMacrosByNamespace.TryGetValue(ns, out var macros))
			Activate(macros);
	}

	private void Activate(IDictionary<string, ReaderMacro> macros)
	{
		if (!_ownsReaderMacros)
		{
			_readerMacros = new Dictionary<string, ReaderMacro>(_readerMacros, StringComparer.Ordinal);
			_ownsReaderMacros = true;
		}
		foreach (var macro in macros)
			_readerMacros.TryAdd(macro.Key, macro.Value);
	}

	/// <summary>
	/// Collects a macro definition's header up to the colon that ends its line,
	/// then reads it. Whether the header has ended is returned.
	/// </summary>
	private bool TrackMacroHeader(IToken token)
	{
		if (token.Type is LPAREN or LBRACK)
			_macroHeaderDepth++;
		else if (token.Type is RPAREN or RBRACK)
			_macroHeaderDepth--;
		else if (token.Type == COLON && _macroHeaderDepth == 0)
		{
			var header = _macroHeader;
			_macroHeader = null;
			if (!EndsLine((ICharStream)InputStream))
				return false;
			DefineMacro(header);
			return true;
		}
		_macroHeader.Add(token);
		return false;
	}

	/// <summary>
	/// Reads a macro definition, as in macro foo(text as verbatim), so the
	/// module can use the macro with verbatim text before it is compiled.
	/// </summary>
	private void DefineMacro(List<IToken> header)
	{
		var pos = 0;
		var path = ReadDottedName(header, ref pos);
		var scope = new BlockScope { Indent = _lineIndent };
		var parentType = DefinedMacroParent();
		_scopes.Push(scope);
		if (path == null)
			return;

		var parameters = ReadParameters(header, ref pos);
		var segments = path.Split('.');
		var typeName = scope.DefinedMacroType = MacroTypeName(segments[segments.Length - 1]);
		if (segments.Length > 1)
			parentType = MacroTypeName(segments[segments.Length - 2]);

		if (parameters == null || parameters.Count == 0 || !IsVerbatimParameter(parameters[parameters.Count - 1]))
			return;

		var reader = parameters.Count == 1 ? ReaderMacro.Verbatim : ReaderMacro.VerbatimBlock;
		if (parentType == null)
			Define(typeName, reader, _definedReaderMacros);
		else
			Define(typeName, reader, NestedMacrosOf(parentType));
	}

	private void Define(string typeName, ReaderMacro reader, IDictionary<string, ReaderMacro> names)
	{
		var topLevel = names == _definedReaderMacros;
		foreach (var name in ReaderMacro.MacroNames(typeName))
		{
			if (!names.TryAdd(name, reader))
				continue;
			if (!topLevel)
				continue;
			SyntaxDeclared?.Invoke(_moduleNamespace, name, reader);
			if (_lineStartNames.Contains(name))
				SyntaxDeclarationWarning?.Invoke(
					new Boo.Lang.Compiler.Ast.LexicalInfo(SourceName, _macroHeaderStart.Line, _macroHeaderStart.Column),
					$"'{name}' is used before its definition, so its verbatim text is parsed as Boo there. Define the macro before using it.");
		}
		if (topLevel)
			Activate(names);
	}

	// The reader macros nested in a macro defined in the module, which is made to read them.
	private IDictionary<string, ReaderMacro> NestedMacrosOf(string parentType)
	{
		if (_definedNestedMacros.TryGetValue(parentType, out var nested))
			return nested;
		nested = new Dictionary<string, ReaderMacro>(StringComparer.Ordinal);
		_definedNestedMacros.Add(parentType, nested);
		Define(parentType, ReaderMacro.Nesting(nested), _definedReaderMacros);
		return nested;
	}

	private string DefinedMacroParent()
	{
		foreach (var scope in _scopes)
			if (scope.DefinedMacroType != null)
				return scope.DefinedMacroType;
		return null;
	}

	// The type the macro macro names a macro definition's class.
	private static string MacroTypeName(string name) => char.ToUpperInvariant(name[0]) + name.Substring(1) + "Macro";

	// The parameters between parentheses, each as its tokens, or null if malformed.
	private static List<List<IToken>> ReadParameters(List<IToken> header, ref int pos)
	{
		var parameters = new List<List<IToken>>();
		if (pos == header.Count)
			return parameters;
		if (!Take(header, ref pos, LPAREN))
			return null;

		var depth = 0;
		var parameter = new List<IToken>();
		for (; pos < header.Count; pos++)
		{
			var token = header[pos];
			if (depth == 0 && token.Type is COMMA or RPAREN)
			{
				if (parameter.Count > 0)
					parameters.Add(parameter);
				parameter = new List<IToken>();
				if (token.Type == RPAREN)
					return pos == header.Count - 1 ? parameters : null;
				continue;
			}
			if (token.Type is LPAREN or LBRACK)
				depth++;
			else if (token.Type is RPAREN or RBRACK)
				depth--;
			parameter.Add(token);
		}
		return null;
	}

	// text as verbatim, in any number of parentheses, which the printer adds.
	private static bool IsVerbatimParameter(List<IToken> parameter)
	{
		var wrapped = 0;
		while (wrapped < parameter.Count - wrapped && parameter[wrapped].Type == LPAREN && parameter[parameter.Count - 1 - wrapped].Type == RPAREN)
			wrapped++;
		return parameter.Count - 2 * wrapped == 3
			&& parameter[wrapped].Type == ID && parameter[wrapped + 1].Type == AS
			&& parameter[wrapped + 2].Type == ID && parameter[wrapped + 2].Text == "verbatim";
	}

	private static bool Take(List<IToken> tokens, ref int pos, int type)
	{
		if (pos >= tokens.Count || tokens[pos].Type != type)
			return false;
		pos++;
		return true;
	}

	private static string ReadDottedName(List<IToken> tokens, ref int pos)
	{
		if (pos >= tokens.Count || tokens[pos].Type != ID)
			return null;
		var name = new System.Text.StringBuilder(tokens[pos++].Text);
		while (pos + 1 < tokens.Count && tokens[pos].Type == DOT && tokens[pos + 1].Type == ID)
		{
			name.Append('.').Append(tokens[pos + 1].Text);
			pos += 2;
		}
		return name.ToString();
	}

	// The name must start a macro statement, not an assignment, call or member access.
	private ReaderMacro MacroStartingLine() =>
		_lineFirstToken.Type == ID
		&& _lineSecondTokenType is not (ASSIGN or LPAREN or LBRACK or DOT or MULTIPLY)
			? ReaderMacroNamed(_lineFirstToken.Text)
			: null;

	/// <summary>
	/// A reader macro's name followed by text it reads as verbatim, as in foo any
	/// text. A line ending in a colon opens a block instead.
	/// </summary>
	private bool ReadsRestOfLine(IToken token)
	{
		if (token.Type != ID || SkipWhitespace || _beginInterpolationType.Count > 0)
			return false;
		var macro = ReaderMacroNamed(token.Text);
		if (macro == null)
			return false;

		var input = (ICharStream)InputStream;
		if (CharAt(input, input.Index) is not (' ' or '\t'))
			return false;
		var pos = SkipBlanks(input, input.Index);
		var c = CharAt(input, pos);
		var next = CharAt(input, pos + 1);
		if (c == IntStreamConstants.EOF || IsLineBreak(c) || (c == '=' && next != '=') || (c is '+' or '-' or '*' or '/' or '%' && next == '='))
			return false;

		var line = LineText(input, token.StartIndex);
		return line[line.Length - 1] != ':' && macro.ReadsLine(line);
	}

	// The text from index to the end of its line, without trailing blanks.
	private static string LineText(ICharStream input, int index)
	{
		var pos = index;
		while (CharAt(input, pos) != IntStreamConstants.EOF && !IsLineBreak(CharAt(input, pos)))
			pos++;
		var text = pos > index ? input.GetText(Antlr4.Runtime.Misc.Interval.Of(index, pos - 1)) : "";
		return text.TrimEnd(' ', '\t', '\f');
	}

	private IToken CaptureVerbatimLine(int tokenLine)
	{
		var input = (ICharStream)InputStream;
		var start = SkipBlanks(input, input.Index);
		var pos = start;
		while (CharAt(input, pos) != IntStreamConstants.EOF && !IsLineBreak(CharAt(input, pos)))
			pos++;

		var text = pos > start ? input.GetText(Antlr4.Runtime.Misc.Interval.Of(start, pos - 1)) : "";
		var line = TokenFactory.Create(
			Tuple.Create<ITokenSource, ICharStream>(this, input), VERBATIM_LINE, text,
			TokenConstants.DefaultChannel, start, pos - 1, tokenLine, Column + start - input.Index);
		SkipTo(input, pos);
		return line;
	}

	// Mirrors WSATokenStreamFilter, where else and or close a block only before a colon.
	private bool ClosesBlock(IToken token)
	{
		if (token.Type is END or ELIF or EXCEPT or ENSURE or THEN)
			return true;
		var input = (ICharStream)InputStream;
		return token.Type is ELSE or OR && CharAt(input, SkipBlanks(input, input.Index)) == ':';
	}

	private static bool EndsLine(ICharStream input)
	{
		var c = CharAt(input, SkipBlanks(input, input.Index));
		return c == IntStreamConstants.EOF || IsLineBreak(c);
	}

	private static bool OpensBlock(ICharStream input) =>
		CharAt(input, input.Index) is ' ' or '\t' or '\f' or '\r' or '\n';

	/// <summary>
	/// Takes the lines after a colon that ends its line, and leaves the lexer
	/// just after them. The block ends at the first line indented no deeper than
	/// the macro's own, or in whitespace agnostic mode at a line holding only end,
	/// which is consumed with it.
	/// </summary>
	private IToken CaptureVerbatimBlock(IToken colon)
	{
		var input = (ICharStream)InputStream;
		var start = input.Index;
		var pos = SkipBlanks(input, start);
		if (!IsLineBreak(CharAt(input, pos)))
			return null;

		var stop = -1;
		var end = -1;
		while (IsLineBreak(CharAt(input, pos)))
		{
			pos = SkipLineBreak(input, pos);
			var lineStart = pos;
			pos = SkipBlanks(input, pos);
			var c = CharAt(input, pos);
			if (IsLineBreak(c))
				continue;
			if (c == IntStreamConstants.EOF)
				break;
			if (WhitespaceAgnostic && IsEndLine(input, pos))
			{
				end = pos + 2;
				break;
			}
			if (!WhitespaceAgnostic && pos - lineStart <= _lineIndent)
				break;
			while (CharAt(input, pos) != IntStreamConstants.EOF && !IsLineBreak(CharAt(input, pos)))
				pos++;
			stop = pos - 1;
		}
		if (stop < 0 || (WhitespaceAgnostic && end < 0))
			return null;

		var block = TokenFactory.Create(
			Tuple.Create<ITokenSource, ICharStream>(this, input), VERBATIM_BLOCK,
			input.GetText(Antlr4.Runtime.Misc.Interval.Of(start, stop)),
			TokenConstants.DefaultChannel, start, Math.Max(stop, end), colon.Line, Column);
		SkipTo(input, Math.Max(stop, end) + 1);
		return block;
	}

	private static bool IsEndLine(ICharStream input, int pos)
	{
		if (CharAt(input, pos) != 'e' || CharAt(input, pos + 1) != 'n' || CharAt(input, pos + 2) != 'd')
			return false;
		var next = CharAt(input, SkipBlanks(input, pos + 3));
		return next == IntStreamConstants.EOF || IsLineBreak(next);
	}

	// Counts lines the way NEWLINE does, a bare \r included.
	private void SkipTo(ICharStream input, int index)
	{
		var line = Line;
		var column = Column;
		for (var i = input.Index; i < index; i++)
		{
			var c = CharAt(input, i);
			if (c == '\n' || (c == '\r' && CharAt(input, i + 1) != '\n'))
			{
				line++;
				column = 0;
			}
			else if (c != '\r')
			{
				column++;
			}
		}
		input.Seek(index);
		Line = line;
		Column = column;
	}

	private static int CharAt(ICharStream input, int index) => input.LA(index - input.Index + 1);

	private static bool IsLineBreak(int c) => c == '\n' || c == '\r';

	private static int SkipBlanks(ICharStream input, int pos)
	{
		for (var c = CharAt(input, pos); c == ' ' || c == '\t' || c == '\f'; c = CharAt(input, ++pos)) { }
		return pos;
	}

	private static int SkipLineBreak(ICharStream input, int pos) =>
		CharAt(input, pos) == '\r' && CharAt(input, pos + 1) == '\n' ? pos + 2 : pos + 1;

	private bool SkipWhitespace => _skipWhitespaceRegion > 0;

	private static bool IsDigit(int ch) => ch >= '0' && ch <= '9';

	private void EnterSkipWhitespaceRegion() => _skipWhitespaceRegion++;

	// A closer with no opener stops at zero. Left to go negative, the next
	// opener would only bring the count back to zero, and line continuation
	// would stay off for the rest of the file.
	private void LeaveSkipWhitespaceRegion()
	{
		if (_skipWhitespaceRegion > 0)
			_skipWhitespaceRegion--;
	}

	private void HandleInterpolatedExpression(int beginInterpolationType, int endTokenType)
	{
		_beginInterpolationType.Push(beginInterpolationType);
		_endInterpolationType.Push(endTokenType);
		PushMode(DEFAULT_MODE);
	}

	private void HandleInterpolationToken(int type)
	{
		if (_beginInterpolationType.Count == 0)
			return;

		if (_beginInterpolationType.Peek() == type)
		{
			PushMode(DEFAULT_MODE);
		}
		else if (_endInterpolationType.Peek() == type)
		{
			PopMode();
			if (CurrentMode != DEFAULT_MODE)
			{
				_beginInterpolationType.Pop();
				_endInterpolationType.Pop();
			}
		}
	}
}
