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
import System.IO
import System.Xml
import Boo.Lang.Lsp.Json
import Boo.Lang.Parser
import System.Text.Json.Nodes

class Project:
"""
The project a source file belongs to.

A .boo file on its own compiles against nothing but the default assemblies,
so anything the project references is unresolved. Finding the project file is
the first step out of that.
"""

	static def Find(sourcePath as string) as string:
	"""
	The nearest .booproj at or above the file, or null if there is none.

	Nearest wins: a project in an inner directory owns the files under it even
	when an outer directory has one too.
	"""
		return null if string.IsNullOrEmpty(sourcePath)
		directory = Path.GetDirectoryName(Path.GetFullPath(sourcePath))
		while not string.IsNullOrEmpty(directory) and Directory.Exists(directory):
			projects = Directory.GetFiles(directory, "*.booproj")
			if projects.Length > 0:
				System.Array.Sort(projects)
				return projects[0]
			directory = Path.GetDirectoryName(directory)
		return null

	static def FindForDocument(uri as string) as string:
	"""
	The project behind a document the client named, or null.

	A document with no file behind it, such as an unsaved buffer, belongs to
	no project.
	"""
		return Find(PathOf(uri))

	static def PathOf(uri as string) as string:
	"""The file a document URI names, or null if it names no file."""
		parsed as Uri
		return null if not Uri.TryCreate(uri, UriKind.Absolute, parsed)
		return null if not parsed.IsFile
		return parsed.LocalPath

	static def UriOf(fileName as string) as string:
	"""
	The URI for a file the compiler named, or the name unchanged if it is
	already one.

	The open document is compiled from text the client named with a URI, and
	the files beside it are compiled from disk and carry a path, so what a
	node reports is one or the other. Only a URI is an answer the protocol
	takes.
	"""
		return fileName if string.IsNullOrEmpty(fileName)
		parsed as Uri
		return fileName if Uri.TryCreate(fileName, UriKind.Absolute, parsed) and not parsed.IsFile
		try:
			return Uri(fileName).AbsoluteUri
		except:
			# Whatever it is, it is not a file, so leave it as it came.
			return fileName

	static def SourceFiles(projectPath as string) as List[of string]:
	"""
	Every .boo file the project compiles.

	The glob matches what Boo.targets builds: everything below the project
	directory, less the build output.
	"""
		sources = List[of string]()
		return sources if string.IsNullOrEmpty(projectPath)
		directory = Path.GetDirectoryName(Path.GetFullPath(projectPath))
		return sources if not Directory.Exists(directory)
		for file in Directory.GetFiles(directory, "*.boo", SearchOption.AllDirectories):
			sources.Add(file) unless IsBuildOutput(directory, file)
		sources.Sort()
		return sources

	private static def IsBuildOutput(root as string, file as string) as bool:
		relative = file.Substring(root.Length).TrimStart(Path.DirectorySeparatorChar)
		head = relative.Split(Path.DirectorySeparatorChar)[0]
		return head == "bin" or head == "obj"

	# How many files a loose script pulls in. Name resolution gives out
	# somewhere past a few hundred modules.
	public static final SiblingLimit = 50

	static def LooseSiblings(sourcePath as string, text as string) as List[of string]:
	"""
	The .boo files beside this one that belong in its compilation.

	A script outside a project still uses the files next to it, and without
	them every name it takes from one reads as undefined. Two kinds qualify:
	one whose namespace an import names, and one that is a module rather
	than a program, since modules of a compilation see each other's top
	level methods with no import to name them.

	A file with top level statements is a program of its own, and a
	directory of those is a directory of unrelated scripts. Compiling those
	together reports collisions that are in none of them.

	At most SiblingLimit files are taken. A file an import names comes first:
	the import says it is wanted, where proximity only guesses.
	"""
		siblings = List[of string]()
		return siblings if string.IsNullOrEmpty(sourcePath)
		directory = Path.GetDirectoryName(Path.GetFullPath(sourcePath))
		return siblings if not Directory.Exists(directory)

		wanted = ImportedNamespaces(text)
		open = Path.GetFullPath(sourcePath)
		named = List[of string]()
		nearby = List[of string]()
		for file in Directory.GetFiles(directory, "*.boo"):
			continue if Path.GetFullPath(file) == open
			declared = DeclaredNamespace(file)
			if declared is not null and wanted.Contains(declared):
				named.Add(file)
				continue
			nearby.Add(file) if IsModule(file)
		named.Sort()
		nearby.Sort()
		for file in named:
			break if siblings.Count >= SiblingLimit
			siblings.Add(file)
		for file in nearby:
			break if siblings.Count >= SiblingLimit
			siblings.Add(file)
		siblings.Sort()
		return siblings

	private class ModuleAnswer:
		public Stamp as string
		public IsModule as bool

	private static final Answers = Dictionary[of string, ModuleAnswer]()

	private static def IsModule(file as string) as bool:
	"""
	Whether the file declares things without running any of its own.

	Kept between binds against the file's write time and length: a bind
	asks about every file beside the document, and a directory of scripts
	answers no every time at the cost of parsing all of them.
	"""
		info = FileInfo(file)
		return false if not info.Exists
		stamp = "${info.LastWriteTimeUtc.Ticks}:${info.Length}"

		lock Answers:
			remembered as ModuleAnswer
			if Answers.TryGetValue(file, remembered) and remembered.Stamp == stamp:
				return remembered.IsModule

		answer = Parses(file)
		lock Answers:
			Answers[file] = ModuleAnswer(Stamp: stamp, IsModule: answer)
		return answer

	private static def Parses(file as string) as bool:
	"""
	Parsed rather than scanned: whether a line is a statement or part of a
	declaration is a question about indentation and continuation that only
	the parser answers.
	"""
		try:
			unit = BooParser.ParseFile(file)
			return false if unit.Modules.Count == 0
			return unit.Modules[0].Globals.Statements.Count == 0
		except:
			# A file that will not parse contributes nothing.
			return false

	private static def ImportedNamespaces(text as string) as HashSet[of string]:
	"""
	The namespace each import in the text names.

	Read off the line rather than the parse tree: this decides what the
	compilation is given, so it runs before there is one.
	"""
		imported = HashSet[of string]()
		return imported if string.IsNullOrEmpty(text)
		for line in text.Split(char('\n')):
			name = Name(line, "import ")
			imported.Add(name) if name is not null
		return imported

	private static def DeclaredNamespace(file as string) as string:
	"""
	The namespace the file declares, or null if it declares none.

	A declaration comes before any code, so the search stops at the first
	line that is neither blank nor part of the header.
	"""
		try:
			for line in File.ReadLines(file):
				trimmed = line.Trim()
				continue if trimmed.Length == 0
				continue if trimmed.StartsWith("//") or trimmed.StartsWith("#")
				return Name(trimmed, "namespace ")
		except:
			# A file that cannot be read contributes nothing.
			return null
		return null

	private static def Name(line as string, keyword as string) as string:
	"""
	The namespace a `namespace` or `import` line names, or null.

	An import carries more than the namespace: `from` names the assembly to
	take it from, `as` renames it, and parentheses select names out of it.
	The namespace is what comes before any of those.
	"""
		trimmed = line.Trim()
		return null if not trimmed.StartsWith(keyword)
		rest = trimmed.Substring(keyword.Length).Trim()
		for separator in (" from ", " as ", "(", "#", "//"):
			cut = rest.IndexOf(separator)
			rest = rest.Substring(0, cut).Trim() if cut > 0
		return null if rest.Length == 0
		return rest

	static def ProjectReferences(projectPath as string) as List[of string]:
	"""
	The projects this one references, as full paths, in the order written.

	A reference marked ReferenceOutputAssembly="false" is a build ordering
	hint and produces nothing to compile against, so it is left out.
	"""
		references = List[of string]()
		document = Load(projectPath)
		return references if document is null
		directory = Path.GetDirectoryName(Path.GetFullPath(projectPath))
		for node in document.GetElementsByTagName("ProjectReference"):
			element = node as XmlElement
			continue if element is null
			continue if element.GetAttribute("ReferenceOutputAssembly").ToLowerInvariant() == "false"
			include = element.GetAttribute("Include")
			continue if string.IsNullOrEmpty(include)
			references.Add(Path.GetFullPath(Path.Combine(directory, Relative(include))))
		return references

	private static def Relative(include as string) as string:
	"""An Include as MSBuild writes it, in this platform's separators."""
		return include.Replace(char('\\'), Path.DirectorySeparatorChar).Replace(char('/'), Path.DirectorySeparatorChar)

	private static def Load(projectPath as string) as XmlDocument:
	"""The project as XML, or null if it is missing or malformed."""
		return null if string.IsNullOrEmpty(projectPath) or not File.Exists(projectPath)
		try:
			document = XmlDocument()
			document.Load(projectPath)
			return document
		except:
			# A project half way through an edit is normal, not an error.
			return null

	static def OutputAssembly(projectPath as string) as string:
	"""
	The assembly the project last built, or null if it has not been built.

	Which configuration is nobody's decision to make: the newest build under
	bin is the one the developer is working against.
	"""
		return null if string.IsNullOrEmpty(projectPath) or not File.Exists(projectPath)
		directory = Path.GetDirectoryName(Path.GetFullPath(projectPath))
		output = Path.Combine(directory, "bin")
		return null if not Directory.Exists(output)
		newest as string = null
		stamp = DateTime.MinValue
		for candidate in Directory.GetFiles(output, AssemblyName(projectPath) + ".dll", SearchOption.AllDirectories):
			written = File.GetLastWriteTimeUtc(candidate)
			continue if newest is not null and written <= stamp
			newest = candidate
			stamp = written
		return newest

	private static def AssemblyName(projectPath as string) as string:
	"""What the project calls its assembly, which defaults to its own name."""
		document = Load(projectPath)
		if document is not null:
			for node in document.GetElementsByTagName("AssemblyName"):
				text = (node as XmlNode).InnerText.Trim()
				return text if text.Length > 0
		return Path.GetFileNameWithoutExtension(projectPath)

	static def References(projectPath as string) as List[of string]:
	"""
	The assemblies to compile the project against.

	A build copies every project it references next to its own output, so the
	newest output directory is the reference set, less the project's own
	assembly: compiling sources against a stale copy of themselves would
	report every type in them as defined twice.

	A project that has been restored but not built has no output directory,
	and its packages are still on disk where restore left them. Those are
	worth having on their own: a file opened in a project nobody has built
	yet otherwise sees no references at all. What the projects it references
	built covers the rest of that case, since a solution is usually part
	built rather than not built at all.

	One assembly per file name, and the copy beside the output wins: those
	are the ones the build agreed on, and the same type offered twice is the
	same error as compiling a project against itself.
	"""
		references = List[of string]()
		return references if string.IsNullOrEmpty(projectPath) or not File.Exists(projectPath)

		seen = HashSet[of string]()
		own = OutputAssembly(projectPath)
		if own is not null:
			# Its own assembly is claimed rather than added: compiling the
			# sources against a stale copy of themselves would report every
			# type in them as defined twice.
			seen.Add(Path.GetFileName(own))
			for candidate in Directory.GetFiles(Path.GetDirectoryName(own), "*.dll"):
				Take(references, seen, candidate)

		for referenced in ProjectReferences(projectPath):
			built = OutputAssembly(referenced)
			Take(references, seen, built) if built is not null

		for package in PackageAssemblies(projectPath):
			Take(references, seen, package)

		references.Sort()
		return references

	private static def Take(references as List[of string], seen as HashSet[of string], path as string):
	"""Keep an assembly unless one of that name is already spoken for."""
		references.Add(path) if seen.Add(Path.GetFileName(path))

	private static def PackageAssemblies(projectPath as string) as List[of string]:
	"""
	The package assemblies restore wrote into the assets file.

	A build does not copy these next to its output, so the output directory
	alone leaves out everything the project takes from NuGet. Projects listed
	there are skipped: those are already beside the output.

	Where a package ships both, the runtime assembly is taken over the compile
	one: references are loaded through Assembly.LoadFrom, which refuses a
	reference assembly.
	"""
		assemblies = List[of string]()
		directory = Path.GetDirectoryName(Path.GetFullPath(projectPath))
		assets = Read(Path.Combine(directory, "obj", "project.assets.json"))
		return assemblies if assets is null
		folder = First(Section(assets, "packageFolders"))
		return assemblies if folder is null
		libraries = Section(assets, "libraries")
		for framework in Section(assets, "targets"):
			for entry in (framework.Value as JsonObject):
				package = entry.Value as JsonObject
				continue if package is null or Fields.Text(package, "type") != "package"
				path = Fields.Text(Fields.Map(libraries, entry.Key), "path")
				continue if path is null
				chosen = Assets(package, "runtime", folder, path)
				chosen = Assets(package, "compile", folder, path) if chosen.Count == 0
				assemblies.AddRange(chosen)
		return assemblies

	private static def Assets(package as JsonObject, section as string, folder as string, path as string) as List[of string]:
	"""The assemblies of one kind the package has on disk."""
		found = List[of string]()
		for entry in Section(package, section):
			asset = entry.Key
			continue if Path.GetFileName(asset) == "_._"
			full = Path.Combine(folder, Relative(path), Relative(asset))
			found.Add(full) if File.Exists(full)
		return found

	private static def Read(path as string) as JsonObject:
	"""The file as a JSON object, or null if it is missing or malformed."""
		return null if not File.Exists(path)
		try:
			return JsonCodec.Parse(File.ReadAllText(path)) as JsonObject
		except:
			return null

	private static def Section(owner as JsonObject, name as string) as JsonObject:
	"""One nested object, or an empty one so callers need not check."""
		nested = Fields.Map(owner, name)
		return nested if nested is not null
		return JsonObject()

	private static def First(owner as JsonObject) as string:
		for entry in owner:
			return entry.Key
		return null
