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
import Boo.Lang.Compiler
import Boo.Lang.Compiler.Ast
import Boo.Lang.Compiler.IO
import Boo.Lang.Compiler.Steps as Steps
import System.Text.Json.Nodes

class Analyzer:
"""
Runs a document through the compiler and reports what it complains about.

Two tiers, because they cost very differently. Parsing a large file takes
about twenty milliseconds and can run on every keystroke; binding costs tens
of milliseconds for one file and grows with the size of the project, so it
belongs behind a longer debounce.

Binding also reports what nothing uses, which costs a walk of the imports
rather than the full Compile pipeline and the IL it emits.

The compiler is not reentrant, so one analyzer serves one caller at a time.
"""

	def Parse(document as TextDocument) as JsonArray:
		return Run(document, Pipelines.Parse(BreakOnErrors: false), false)

	# The last bind, kept for whoever asks about the same document next.
	static _boundUri as string
	static _boundText as string
	static _boundInputs as string
	static _bound as CompilerContext

	def Bind(document as TextDocument) as JsonArray:
		context = Bound(document)
		return JsonArray() if context is null
		return Report(document, context)

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

	Kept until the text changes. Diagnostics, hover, go to definition and
	highlight all ask about the same state between edits, and highlight
	alone asks on every move of the cursor.

	Held against the text rather than the version, since a caller may hand
	over new text without moving the version on.
	"""
		inputs = InputStamp(document)

		lock CompilerLock.Gate:
			if _bound is not null and _boundUri == document.Uri and _boundText == document.Text and _boundInputs == inputs:
				return _bound

		context = Compile(document, BindingPipeline(), true)

		lock CompilerLock.Gate:
			_boundUri = document.Uri
			_boundText = document.Text
			_boundInputs = inputs
			_bound = context
		return context

	private static def InputStamp(document as TextDocument) as string:
	"""
	What the bind was made from besides the document's own text.

	A file beside it edited on disk changes the answer while the document
	itself sits still, so the files and when they were last written are
	part of what the bind is kept against.
	"""
		path = Project.PathOf(document.Uri)
		project = Project.Find(path)
		files = (Project.SourceFiles(project) if project is not null else Project.LooseSiblings(path, document.Text))
		stamp = System.Text.StringBuilder()
		for file in files:
			stamp.Append(file).Append(':')
			try:
				stamp.Append(System.IO.File.GetLastWriteTimeUtc(file).Ticks)
			except:
				stamp.Append('?')
			stamp.Append(';')
		return stamp.ToString()

	private static def BindingPipeline() as CompilerPipeline:
	"""Names resolved, plus the check for what nothing ends up using."""
		pipeline = Pipelines.ResolveExpressions(BreakOnErrors: false)
		pipeline.Add(Steps.CheckNeverUsedMembers())
		return pipeline

	private def Run(document as TextDocument, pipeline as CompilerPipeline, withProject as bool) as JsonArray:
		context = Compile(document, pipeline, withProject)
		return JsonArray() if context is null
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

	# A step that fell over. Its message is exception text, which a reader
	# cannot act on.
	static final InternalError = "BCE0011"

	# What one document may report. One unresolved type can produce thousands.
	public static final Limit = 200

	private def Report(document as TextDocument, context as CompilerContext) as JsonArray:
		diagnostics = JsonArray()
		held = 0
		for error in context.Errors:
			continue unless Belongs(document, error.LexicalInfo)
			if diagnostics.Count >= Limit:
				held++
				continue
			if error.Code == InternalError:
				Console.Error.WriteLine("boo-ls: ${document.Uri}: ${error.Message}")
				diagnostics.Add(Diagnostic.Internal(error.Code))
				continue
			diagnostics.Add(Diagnostic.FromError(document, error))
		for warning in context.Warnings:
			continue unless Trustworthy(warning)
			continue unless Belongs(document, warning.LexicalInfo)
			if diagnostics.Count < Limit:
				diagnostics.Add(Diagnostic.FromWarning(document, warning))
			else:
				held++
		diagnostics.Add(Diagnostic.Withheld(held)) if held > 0
		return diagnostics

	private static def Trustworthy(warning as CompilerWarning) as bool:
	"""
	Whether binding knows enough to have made this report.

	Whether a private member is used is settled by steps this pipeline
	stops short of, so it reports the ones that are used as well.
	"""
		return warning.Code != "BCW0014"

	private def Belongs(document as TextDocument, location as LexicalInfo) as bool:
	"""
	Whether a report is about this document.

	The project's other files are compiled alongside it, and what they get
	wrong belongs on them, not here. A report with no location at all is kept:
	it is about the compilation, and this document is what asked for it.
	"""
		return true if location is null or string.IsNullOrEmpty(location.FileName)
		return location.FileName == document.Uri
