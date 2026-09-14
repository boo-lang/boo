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

namespace Boo.Lang.Compiler;

using System;

/// <summary>
/// Marks a macro that takes verbatim text: the parser keeps the text as
/// written instead of parsing it as Boo, and hands it to the macro in
/// <see cref="Ast.MacroStatement.VerbatimBody"/>. The use decides the form, the
/// rest of the macro's line or the indented block after its colon. A macro
/// nested in another macro's class takes verbatim text only inside that
/// macro's block. It applies in any module that imports the macro's namespace.
/// </summary>
/// <remarks>
/// The macro macro adds this attribute to a macro with a verbatim parameter,
/// so a macro written in Boo does not need it:
/// <code>
/// macro foo(text as verbatim):
///     ...
///
/// foo any text at all
/// foo:
///     any lines at all
/// </code>
/// </remarks>
[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public sealed class VerbatimMacroAttribute : Attribute
{
	public VerbatimMacroAttribute()
	{
	}

	/// <summary>
	/// A macro whose source a Boo.Lang.Parser.ReaderMacro subclass reads instead,
	/// deciding line by line what is text, as Boo.Lang.Parser.CommandBlockReader
	/// does. It needs a public parameterless constructor.
	/// </summary>
	public VerbatimMacroAttribute(Type readerMacro)
	{
		ReaderMacro = readerMacro;
	}

	/// <summary>The Boo.Lang.Parser.ReaderMacro subclass that reads the macro, if any.</summary>
	public Type ReaderMacro { get; }

	/// <summary>
	/// Whether only the block form takes verbatim text, as for a macro that also
	/// takes Boo arguments before its colon. Its line is then parsed as Boo.
	/// </summary>
	public bool BlockOnly { get; set; }
}
