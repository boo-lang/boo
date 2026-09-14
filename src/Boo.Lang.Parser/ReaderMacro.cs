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

/// <summary>
/// How a line inside a reader macro's block is read.
/// </summary>
public enum LineSyntax
{
	/// <summary>Parsed as Boo.</summary>
	Boo,

	/// <summary>The whole line as the verbatim text of a macro with no name.</summary>
	Verbatim
}

/// <summary>
/// Decides which source after a macro's name the parser keeps as verbatim text
/// instead of parsing it as Boo. Registered by macro name in
/// <see cref="ParserSettings.ReaderMacros"/>, or found on a macro marked with
/// <see cref="Boo.Lang.Compiler.VerbatimMacroAttribute"/>; the macro expands the
/// verbatim text later.
/// </summary>
public abstract class ReaderMacro
{
	/// <summary>
	/// Keeps the rest of the macro's line, or the indented block after its colon,
	/// as verbatim text. A line that reads as Boo, as in foo = 1, stays Boo.
	/// </summary>
	public static readonly ReaderMacro Verbatim = new VerbatimReader(lineToo: true);

	/// <summary>
	/// Keeps the indented block after the macro's colon as verbatim text, and
	/// parses its line as Boo.
	/// </summary>
	public static readonly ReaderMacro VerbatimBlock = new VerbatimReader(lineToo: false);

	/// <summary>
	/// Parses the lines of its block that read as Boo and keeps each other line
	/// as the text of a macro with no name. See <see cref="CommandBlockReader"/>.
	/// </summary>
	public static readonly ReaderMacro CommandBlock = new CommandBlockReader();

	/// <summary>
	/// Parses its block as Boo, with the given reader macros applying inside it.
	/// The dictionary is read as the block is lexed, so it may still grow.
	/// </summary>
	public static ReaderMacro Nesting(IDictionary<string, ReaderMacro> nestedMacros) => new NestingReader(nestedMacros);

	/// <summary>
	/// Whether the block after the macro's colon is verbatim text. If so, its
	/// lines are not read one by one.
	/// </summary>
	public virtual bool ReadsBlock => false;

	/// <summary>
	/// Whether the text after the macro's name is verbatim text, given the whole line.
	/// Asked only when that text is not empty and does not end in a colon.
	/// </summary>
	public virtual bool ReadsLine(string line) => false;

	/// <summary>
	/// How a line anywhere in the macro's block reads, given the line without
	/// its indentation. A line led by a reader macro's name is not asked about,
	/// and inside that macro's block its reader macro is asked instead, if it
	/// overrides this. One that does not leaves its lines to this one.
	/// </summary>
	public virtual LineSyntax ReadBlockLine(string line) => LineSyntax.Boo;

	/// <summary>
	/// The reader macros that apply inside the macro's block, by name, in
	/// addition to those that apply around it.
	/// </summary>
	public virtual IDictionary<string, ReaderMacro> NestedMacros => NoNestedMacros;

	private static readonly IDictionary<string, ReaderMacro> NoNestedMacros = new Dictionary<string, ReaderMacro>();

	/// <summary>
	/// The names the macro expander resolves to a macro type: foo, Foo and
	/// FooMacro for FooMacro, but only Macro for Macro.
	/// </summary>
	internal static IEnumerable<string> MacroNames(string typeName)
	{
		yield return typeName;
		var name = typeName.EndsWith("Macro", StringComparison.Ordinal) ? typeName.Substring(0, typeName.Length - "Macro".Length) : "";
		if (name.Length > 0 && char.IsUpper(name[0]))
		{
			yield return name;
			yield return char.ToLowerInvariant(name[0]) + name.Substring(1);
		}
	}

	private bool? _readsBlockLines;

	internal bool ReadsBlockLines =>
		_readsBlockLines ??= GetType().GetMethod(nameof(ReadBlockLine), new[] { typeof(string) }).DeclaringType != typeof(ReaderMacro);

	/// <summary>
	/// Whether a line clearly reads as Boo: a comment, a statement keyword, or a
	/// name followed by a call, index, assignment, declaration or unpacking.
	/// </summary>
	protected static bool IsBooLine(string line)
	{
		var c = At(line, 0);
		if (c is '#' or '(' or '[' || (c == '/' && At(line, 1) is '/' or '*'))
			return true;

		var pos = 0;
		var word = ReadName(line, ref pos);
		if (word.Length == 0)
			return false;
		if (StatementKeywords.Contains(word))
			return true;

		while (At(line, pos) == '.' && IsNameStart(At(line, pos + 1)))
		{
			pos++;
			ReadName(line, ref pos);
		}
		if (At(line, pos) is '(' or '[')
			return true;

		pos = SkipBlanks(line, pos);
		c = At(line, pos);
		var next = At(line, pos + 1);
		if (c == ',' || (c == '=' && next != '=') || (c is '+' or '-' or '*' or '/' or '%' && next == '='))
			return true;
		return c == 'a' && next == 's' && At(line, pos + 2) is ' ' or '\t';
	}

	private static readonly HashSet<string> StatementKeywords = new()
	{
		"if", "unless", "elif", "else", "for", "while", "try", "except", "ensure",
		"raise", "return", "yield", "break", "continue", "pass", "goto", "def",
		"class", "end", "then"
	};

	internal static string ReadName(string line, ref int pos)
	{
		var start = pos;
		if (!IsNameStart(At(line, pos)))
			return "";
		while (At(line, pos) is var c && (c == '_' || char.IsLetterOrDigit(c)))
			pos++;
		return line.Substring(start, pos - start);
	}

	private static bool IsNameStart(char c) => c == '_' || char.IsLetter(c);

	private static char At(string line, int pos) => pos < line.Length ? line[pos] : '\0';

	private static int SkipBlanks(string line, int pos)
	{
		while (At(line, pos) is ' ' or '\t' or '\f')
			pos++;
		return pos;
	}

	private sealed class VerbatimReader : ReaderMacro
	{
		private readonly bool _lineToo;

		public VerbatimReader(bool lineToo) => _lineToo = lineToo;

		public override bool ReadsBlock => true;

		public override bool ReadsLine(string line) => _lineToo && !IsBooLine(line);
	}

	private sealed class NestingReader : ReaderMacro
	{
		private readonly IDictionary<string, ReaderMacro> _nestedMacros;

		public NestingReader(IDictionary<string, ReaderMacro> nestedMacros) => _nestedMacros = nestedMacros ?? throw new ArgumentNullException(nameof(nestedMacros));

		public override IDictionary<string, ReaderMacro> NestedMacros => _nestedMacros;
	}
}

/// <summary>
/// Reads a block that mixes Boo statements with lines of text, as a shell script
/// mixes commands with control flow. A line that reads as Boo, such as a comment,
/// an if or for, an assignment or a call, is parsed as Boo. Any other line is a
/// macro statement with an empty name holding the whole line as text, which the
/// macro rewrites. The macro's name followed by text on one line keeps that text
/// as its own.
/// </summary>
/// <example>
/// The ls lines are text; the rest is Boo:
/// <code>
/// foo:
///     for dir in dirs:
///         ls -l $dir
///     count = 1
/// foo ls -l
/// </code>
/// </example>
public sealed class CommandBlockReader : ReaderMacro
{
	public override bool ReadsLine(string line) => !IsBooLine(line);

	public override LineSyntax ReadBlockLine(string line) => IsBooLine(line) ? LineSyntax.Boo : LineSyntax.Verbatim;
}
