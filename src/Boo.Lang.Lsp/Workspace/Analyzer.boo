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

namespace Boo.Lang.Lsp.Workspace

import System
import System.Collections.Generic
import Boo.Lang.Compiler
import Boo.Lang.Compiler.Ast
import Boo.Lang.Compiler.IO
import Boo.Lang.Compiler.Pipelines

class Analyzer:
"""
Runs a document through the compiler and reports what it complains about.

Two tiers, because they cost very differently. Parsing a large file takes
about twenty milliseconds and can run on every keystroke; binding costs tens
of milliseconds for one file and grows with the size of the project, so it
belongs behind a longer debounce.

Binding reports errors only. Warnings such as an unused import come out of the
full Compile pipeline, which emits IL; whether the server pays that to report
them is a question for later.

The compiler is not reentrant, so one analyzer serves one caller at a time.
"""

	def Parse(document as TextDocument) as List[of object]:
		return Run(document, Pipelines.Parse(BreakOnErrors: false), false)

	def Bind(document as TextDocument) as List[of object]:
		return Run(document, Pipelines.ResolveExpressions(BreakOnErrors: false), true)

	def ParseTree(document as TextDocument) as Module:
	"""
	The module as parsed, or null if the compiler could not produce one.

	The text is balanced first: half typed brackets cost the parser the whole
	structure below them, and an outline is wanted most while the file is
	still being written. Only the parse sees the repaired text.
	"""
		context = CompileText(document.Uri, BracketRepair.Repair(document.Text), Pipelines.Parse(BreakOnErrors: false), false)
		return null if context is null
		return null if context.CompileUnit.Modules.Count == 0
		return context.CompileUnit.Modules[0]

	def Bound(document as TextDocument) as CompilerContext:
	"""
	The document bound far enough to carry entities, or null.

	Every caller pays a full bind. Caching one per document is what M8 is for.
	"""
		return Compile(document, Pipelines.ResolveExpressions(BreakOnErrors: false), true)

	private def Run(document as TextDocument, pipeline as CompilerPipeline, withProject as bool) as List[of object]:
		context = Compile(document, pipeline, withProject)
		return List[of object]() if context is null
		return Report(document, context)

	private def Compile(document as TextDocument, pipeline as CompilerPipeline, withProject as bool) as CompilerContext:
		return CompileText(document.Uri, document.Text, pipeline, withProject)

	private def CompileText(uri as string, text as string, pipeline as CompilerPipeline, withProject as bool) as CompilerContext:
		try:
			lock CompilerLock.Gate:
				compiler = BooCompiler()
				compiler.Parameters.Pipeline = pipeline
				# Nothing is emitted, and a project of loose scripts would
				# otherwise be told it has more than one entry point.
				compiler.Parameters.OutputType = CompilerOutputType.Library
				AddProject(compiler, uri, text) if withProject
				compiler.Parameters.Input.Add(StringInput(uri, text))
				return compiler.Run()
		except e as Exception:
			# A compiler that fell over is a bug, but a server that stops
			# answering because of one is worse.
			Console.Error.WriteLine("boo-ls: analyzing ${uri} failed: ${e.Message}")
			return null

	static def AddProject(compiler as BooCompiler, uri as string, text as string):
	"""
	Give the compiler the project the document belongs to.

	Shared with completion, which needs the same references and the same
	files beside the document: a name only completes against what the
	compilation can see.

	On its own a document sees the default assemblies and nothing else, so
	every type it takes from a reference or from a file beside it reads as
	undefined. Only the binding tier pays for this: parsing runs on every
	keystroke and wants the one file.

	A document in no project falls back to the files its own imports name,
	which is as much as can be known about a loose script without guessing.

	Both the reference set and the file list are read from disk each time,
	which M8 will want to cache along with the bind they belong to.
	"""
		project = Project.FindForDocument(uri)
		if project is null:
			for sibling in Project.LooseSiblings(Project.PathOf(uri), text):
				compiler.Parameters.Input.Add(FileInput(sibling))
			return

		for reference in Project.References(project):
			# Loading it only reads it; the collection is what the compiler sees.
			loaded = compiler.Parameters.LoadAssembly(reference, false)
			compiler.Parameters.References.Add(loaded) if loaded is not null

		# The open document is added from the client's text, not from disk:
		# what was saved is behind what is being typed, and compiling both
		# reports every type in the file as defined twice.
		open = Project.PathOf(uri)
		for source in Project.SourceFiles(project):
			compiler.Parameters.Input.Add(FileInput(source)) unless source == open

	private def Report(document as TextDocument, context as CompilerContext) as List[of object]:
		diagnostics = List[of object]()
		for error in context.Errors:
			diagnostics.Add(Diagnostic.FromError(document, error)) if Belongs(document, error.LexicalInfo)
		for warning in context.Warnings:
			diagnostics.Add(Diagnostic.FromWarning(document, warning)) if Belongs(document, warning.LexicalInfo)
		return diagnostics

	private def Belongs(document as TextDocument, location as LexicalInfo) as bool:
	"""
	Whether a report is about this document.

	The project's other files are compiled alongside it, and what they get
	wrong belongs on them, not here. A report with no location at all is kept:
	it is about the compilation, and this document is what asked for it.
	"""
		return true if location is null or string.IsNullOrEmpty(location.FileName)
		return location.FileName == document.Uri
