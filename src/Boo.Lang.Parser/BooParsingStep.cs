#region license
// Copyright (c) 2004-2026, Rodrigo B. de Oliveira (rbo@acm.org) and the Boo contributors
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Boo.Lang.Compiler.Ast;
using Boo.Lang.Compiler;
using Boo.Lang.Compiler.TypeSystem.Reflection;
using Boo.Lang.Environments;
using Boo.Lang.Parser.Util;
using Antlr4.Runtime;
using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;

namespace Boo.Lang.Parser;

/// <summary>
/// Step 1. Parses any input fed to the compiler.
/// 
/// Parsing behaviour can be customized by providing a specific <see cref="ParserSettings"/> instance through
/// <see cref="CompilerParameters.Environment" />.
/// </summary>
public class BooParsingStep : ICompilerStep
{
	CompilerContext _context;
	
	protected CompilerContext Context => _context;
	
	public void Initialize(CompilerContext context)
	{
		_context = context;
	}
	
	public void Dispose()
	{
		_context = null;
	}

	protected int TabSize => My<Boo.Lang.Parser.ParserSettings>.Instance.TabSize;

	protected Boo.Lang.Parser.ParserSettings Settings => My<Boo.Lang.Parser.ParserSettings>.Instance;

	public void Run()
	{
		// Parser errors are reported through the ambient settings, so the handler
		// that was there before this step ran has to come back afterwards.
		var settings = My<Boo.Lang.Parser.ParserSettings>.Instance;
		var previousHandler = settings.ErrorHandler;
		var previousReaderMacros = settings.ReaderMacrosByNamespace;
		var previousSyntaxDeclared = settings.SyntaxDeclared;
		var previousSyntaxDeclarationWarning = settings.SyntaxDeclarationWarning;
		var readerMacros = WithDeclaredReaderMacros(previousReaderMacros);
		settings.ErrorHandler = OnParserError;
		settings.ReaderMacrosByNamespace = readerMacros;
		// A module's own verbatim macros carry over to the modules parsed after it.
		settings.SyntaxDeclared = (ns, name, macro) =>
		{
			if (!readerMacros.TryGetValue(ns, out var names))
				readerMacros.Add(ns, names = new Dictionary<string, ReaderMacro>(StringComparer.Ordinal));
			names.TryAdd(name, macro);
		};
		// Both parsing stages lex the module, so each warning comes twice.
		var reported = new HashSet<string>();
		settings.SyntaxDeclarationWarning = (location, message) =>
		{
			if (reported.Add($"{location}: {message}"))
				_context.Warnings.Add(CompilerWarningFactory.CustomWarning(location, message));
		};

		try
		{
			ParseInputs();
		}
		finally
		{
			settings.ErrorHandler = previousHandler;
			settings.ReaderMacrosByNamespace = previousReaderMacros;
			settings.SyntaxDeclared = previousSyntaxDeclared;
			settings.SyntaxDeclarationWarning = previousSyntaxDeclarationWarning;
		}
	}

	/// <summary>
	/// The given reader macros plus those for the macros referenced assemblies
	/// mark with <see cref="VerbatimMacroAttribute"/>, by the namespace of each macro.
	/// </summary>
	IDictionary<string, IDictionary<string, ReaderMacro>> WithDeclaredReaderMacros(IDictionary<string, IDictionary<string, ReaderMacro>> given)
	{
		var all = new Dictionary<string, IDictionary<string, ReaderMacro>>(StringComparer.Ordinal);
		foreach (var ns in given ?? Enumerable.Empty<KeyValuePair<string, IDictionary<string, ReaderMacro>>>())
			all.Add(ns.Key, new Dictionary<string, ReaderMacro>(ns.Value, StringComparer.Ordinal));

		// The assembly each verbatim macro came from, to report a second one.
		var declaredBy = new Dictionary<string, string>(StringComparer.Ordinal);
		// The macros nested in each parent macro type, which is made to read them.
		var nestedByParent = new Dictionary<string, IDictionary<string, ReaderMacro>>(StringComparer.Ordinal);
		foreach (var reference in _context.Parameters.References.OfType<IAssemblyReference>())
		{
			var assembly = reference.Assembly.GetName().Name;
			foreach (var (macro, attribute) in VerbatimMacroTypes(reference.Assembly))
			{
				var reader = ReaderMacroFor(macro, attribute, assembly);
				if (reader == null)
					continue;

				if (declaredBy.TryGetValue(macro.FullName, out var first))
				{
					_context.Warnings.Add(CompilerWarningFactory.CustomWarning($"The verbatim macro '{macro.FullName}' is defined by both '{first}' and '{assembly}'; the one from '{first}' is used."));
					continue;
				}
				declaredBy.Add(macro.FullName, assembly);

				var names = macro.DeclaringType == null
					? NamesIn(all, macro.Namespace)
					: NestedMacrosOf(macro.DeclaringType, all, nestedByParent);
				foreach (var name in ReaderMacro.MacroNames(macro.Name))
					names.TryAdd(name, reader);
			}
		}
		return all;
	}

	static IDictionary<string, ReaderMacro> NamesIn(Dictionary<string, IDictionary<string, ReaderMacro>> all, string ns)
	{
		if (!all.TryGetValue(ns ?? "", out var names))
			all.Add(ns ?? "", names = new Dictionary<string, ReaderMacro>(StringComparer.Ordinal));
		return names;
	}

	static IDictionary<string, ReaderMacro> NestedMacrosOf(Type parent, Dictionary<string, IDictionary<string, ReaderMacro>> all, Dictionary<string, IDictionary<string, ReaderMacro>> nestedByParent)
	{
		if (nestedByParent.TryGetValue(parent.FullName, out var nested))
			return nested;
		nested = new Dictionary<string, ReaderMacro>(StringComparer.Ordinal);
		nestedByParent.Add(parent.FullName, nested);
		var reader = ReaderMacro.Nesting(nested);
		var names = NamesIn(all, parent.Namespace);
		foreach (var name in ReaderMacro.MacroNames(parent.Name))
			names.TryAdd(name, reader);
		return nested;
	}

	static IEnumerable<(Type, CustomAttributeData)> VerbatimMacroTypes(Assembly assembly)
	{
		// Only an assembly built against the compiler can mark a macro.
		if (assembly.IsDynamic || !assembly.GetReferencedAssemblies().Any(name => name.Name == typeof(VerbatimMacroAttribute).Assembly.GetName().Name))
			yield break;

		Type[] types;
		try
		{
			types = assembly.GetTypes();
		}
		catch (ReflectionTypeLoadException e)
		{
			types = e.Types.Where(type => type != null).ToArray();
		}
		foreach (var type in types)
			foreach (var data in type.GetCustomAttributesData())
				if (data.AttributeType.FullName == typeof(VerbatimMacroAttribute).FullName)
					yield return (type, data);
	}

	ReaderMacro ReaderMacroFor(Type macro, CustomAttributeData attribute, string assembly)
	{
		if (attribute.ConstructorArguments.Count == 1)
			return attribute.ConstructorArguments[0].Value is Type readerType
				? CreateReaderMacro(readerType, assembly)
				: DeclarationError(assembly, $"The verbatim macro '{macro}' names no reader macro type.");

		var blockOnly = attribute.NamedArguments.Any(argument => argument.MemberName == nameof(VerbatimMacroAttribute.BlockOnly) && argument.TypedValue.Value is true);
		return blockOnly ? ReaderMacro.VerbatimBlock : ReaderMacro.Verbatim;
	}

	ReaderMacro CreateReaderMacro(Type type, string assembly)
	{
		object created;
		try
		{
			created = Activator.CreateInstance(type);
		}
		catch (Exception e)
		{
			var cause = e is TargetInvocationException { InnerException: not null } ? e.InnerException : e;
			return DeclarationError(assembly, $"The reader macro '{type}' could not be created: {cause.Message}");
		}
		return created as ReaderMacro
			?? DeclarationError(assembly, $"'{type}' is not a {typeof(ReaderMacro)}.");
	}

	ReaderMacro DeclarationError(string assembly, string message)
	{
		_context.Errors.Add(CompilerErrorFactory.CustomError(LexicalInfo.Empty, $"{message} (in '{assembly}')"));
		return null;
	}

	private void ParseInputs()
	{
		foreach (var input in _context.Parameters.Input)
		{
			// Each input is its own run of errors. WSABooParsingStep overrides
			// ParseModule, so the reset belongs here rather than in it.
			_lastErrorLine = -1;

			try
			{
				using (var reader = input.Open())
					ParseModule(input.Name, reader);
			}				
			catch (CompilerError error)
			{
				_context.Errors.Add(error);
			}
			catch (Exception x)
			{
				_context.Errors.Add(CompilerErrorFactory.InputError(input.Name, x));
			}
		}
	}

	protected virtual void ParseModule(string inputName, System.IO.TextReader reader)
	{
		var settings = My<Boo.Lang.Parser.ParserSettings>.Instance;
		var stream = new AntlrInputStream(reader);
		// Without this a token reports <unknown> and its error lands on no file.
		stream.name = inputName;
		BooParser.StartContext tree;

		// SLL first, as BooParser.ParseModule does. It reports nothing, because it
		// still calls the listener before bailing and the LL retry repeats it.
		try
		{
			tree = BooParser.CreateParser(settings.TabSize, inputName, stream, true, null, settings).start();
		}
		catch (ParseCanceledException)
		{
			stream.Seek(0);
			tree = BooParser.CreateParser(settings.TabSize, inputName, stream, false, OnParserError, settings).start();
		}

		var visitor = new BooParserAstBuilderVisitor(_context.CompileUnit, inputName);
		visitor.VisitStart(tree);
	}

	private int _lastErrorLine = -1;

	/// <summary>
	/// ANTLR 4 reports a missing or extraneous token with no exception at all,
	/// so an error is not conditional on there being one.
	/// </summary>
	protected void OnParserError(IRecognizer recognizer, IToken offendingSymbol, string filename, int line, int charPositionInLine, string msg, RecognitionException e)
	{
		// Errors close behind another are ANTLR 4 resynchronising, not separate
		// problems. boo.g never emitted the cascade, so it needed no such check.
		if (_lastErrorLine != -1 && line - _lastErrorLine < 3)
		{
			_lastErrorLine = line;
			return;
		}
		_lastErrorLine = line;

		// An indentation token sits past the end of the line it closes, so
		// point at the start of the line instead.
		if (offendingSymbol != null
			&& (offendingSymbol.Type == BooLexer.INDENT || offendingSymbol.Type == BooLexer.DEDENT))
			charPositionInLine = 1;

		var location = new LexicalInfo(filename, line, charPositionInLine);
		var friendly = BooErrorPatterns.Match(recognizer, offendingSymbol, e);
		if (friendly != null)
		{
			_context.Errors.Add(CompilerErrorFactory.GenericParserError(location, new Exception(friendly)));
			return;
		}

		// Name the token, as boo.g did. ANTLR's own text describes its recovery
		// rather than the problem, so it is the last resort.
		if (offendingSymbol != null)
			_context.Errors.Add(CompilerErrorFactory.UnexpectedToken(location, e, offendingSymbol.Text));
		else
			_context.Errors.Add(CompilerErrorFactory.GenericParserError(location, new Exception(msg)));
	}


	void ParserError(LexicalInfo data, NoViableAltException error, IToken offendingSymbol)
	{
		_context.Errors.Add(CompilerErrorFactory.UnexpectedToken(data, error, offendingSymbol.Text));
	}
}
