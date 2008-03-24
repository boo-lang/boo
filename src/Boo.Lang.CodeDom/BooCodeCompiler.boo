#region license
// Copyright (c) 2004, Rodrigo B. de Oliveira (rbo@acm.org)
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

// authors:
// Arron Washington
// Ian MacLean (original C# version)

namespace Boo.Lang.CodeDom

import System.CodeDom
import System.CodeDom.Compiler
import System.IO
import Boo.Lang.Parser
import Boo.Lang.Compiler
import System.Text

#This class is *NOT* meant to be consumed direclty; use BooCodeProvider for that.
internal class BooCodeCompiler(ICodeCompiler, BooCodeGenerator):
	def constructor():
		pass
		
	def CompileAssemblyFromDom(params as System.CodeDom.Compiler.CompilerParameters, unit as CodeCompileUnit):
		return CompileAssemblyFromDomBatch(params, (unit,))
		
	def CompileAssemblyFromDomBatch(params as System.CodeDom.Compiler.CompilerParameters, units as (CodeCompileUnit)):
		params = System.CodeDom.Compiler.CompilerParameters() unless params
		files = []
		assemblies = []
		for unit as CodeCompileUnit in units:
			filename = params.TempFiles.AddExtension('boo')
			files.Add(filename)
			writer = StreamWriter(File.OpenWrite(filename))
			if unit.ReferencedAssemblies:
				for asm in unit.ReferencedAssemblies:
					assemblies.Add(asm) unless assemblies.Contains(asm)
			generator_options = CodeGeneratorOptions()
			//generator_options.IndentString = "\t" //default is "    "
			if params.CompilerOptions and "-wsa" in params.CompilerOptions.Split():
				generator_options["WhiteSpaceAgnostic"] = true
			(self as ICodeGenerator).GenerateCodeFromCompileUnit(unit, writer, generator_options)
			writer.Close()
		return self.CompileAssemblyFromFileBatch(params, array(string, files))
		
	def CompileAssemblyFromFile(params as System.CodeDom.Compiler.CompilerParameters, file as string):
		return HeavyLifter(params, (file, ), false)
		
	def CompileAssemblyFromFileBatch(params as System.CodeDom.Compiler.CompilerParameters, files as (string)):
		return HeavyLifter(params, files, false)
		
	def HeavyLifter(params as System.CodeDom.Compiler.CompilerParameters, sources as (string),
			rawSource as bool):
		params = System.CodeDom.Compiler.CompilerParameters() unless params
		results = CompilerResults(params.TempFiles)
		compiler = BooCompiler()
		#Ugly duckling, become a swan...		
		if params.OutputAssembly:
			compiler.Parameters.OutputAssembly = params.OutputAssembly
		else:
			compiler.Parameters.OutputAssembly = params.TempFiles.AddExtension("dll")
		if params.GenerateInMemory:
			compiler.Parameters.Pipeline = Pipelines.CompileToMemory()
		else:
			compiler.Parameters.Pipeline = Pipelines.CompileToFile()
		if params.GenerateExecutable:
			compiler.Parameters.OutputType = CompilerOutputType.ConsoleApplication
		else:
			compiler.Parameters.OutputType = CompilerOutputType.Library
		compiler.Parameters.Debug = params.IncludeDebugInformation
		
		if params.CompilerOptions:
			extra_options = params.CompilerOptions.Split()
			if "-wsa" in extra_options:
				compiler.Parameters.Pipeline[0] = Boo.Lang.Parser.WSABooParsingStep()
			if "-ducky" in extra_options:
				compiler.Parameters.Ducky = true
				
		if params.ReferencedAssemblies:
			for asm in params.ReferencedAssemblies:
				compiler.Parameters.References.Add(compiler.Parameters.LoadAssembly(asm, true))
		for code in sources:
			if rawSource:
				compiler.Parameters.Input.Add(Boo.Lang.Compiler.IO.StringInput("source", code))
			else:
				compiler.Parameters.Input.Add(Boo.Lang.Compiler.IO.FileInput(code))
		context = compiler.Run()
		#Bad compile, return null assembly + errors.
		if context.Errors.Count > 0:
			for error as Boo.Lang.Compiler.CompilerError in context.Errors:
				bad = System.CodeDom.Compiler.CompilerError()
				bad.Column = error.LexicalInfo.Column
				bad.Line = error.LexicalInfo.Line
				bad.FileName = error.LexicalInfo.FileName
				bad.ErrorText = error.Message				
				bad.IsWarning = false
				results.Errors.Add(bad)
				
			results.CompiledAssembly = null			
		else:
			results.CompiledAssembly = context.GeneratedAssembly
		return results
		
	def CompileAssemblyFromSource(params as System.CodeDom.Compiler.CompilerParameters, src as string):
		return HeavyLifter(params, (src,), true)
		
	def CompileAssemblyFromSourceBatch(params as System.CodeDom.Compiler.CompilerParameters, src as (string)):
		return HeavyLifter(params, (src), true)
		
	/*unused at the moment:
	static private def BuildArgs(options as System.CodeDom.Compiler.CompilerParameters, 
				files as (string)) as string:
		args = StringBuilder()
		
		args.Append("-nologo ")
		
		if options.GenerateExecutable:
			args.Append("-t:exe ")
			extension = "exe"
		else:
			args.Append("-t:library ")
			extension = "dll"
		
		if options.Win32Resource:
			args.AppendFormat("-resource:\"{0}\" ",
					options.Win32Resource)
		
		if options.IncludeDebugInformation:
			args.Append("-debug ")
		else:
			args.Append("-debug- ")
			
		//if options.TreatWarningsAsErrors:
		//	pass
		
		if options.OutputAssembly is null:
			options.OutputAssembly = options.TempFiles.AddExtension(extension, true)
		args.AppendFormat("-o:\"{0}\" ", options.OutputAssembly);
		
		if options.ReferencedAssemblies:
			for r as string in options.ReferencedAssemblies:
				args.AppendFormat("-r:\"{0}\" ",r)
		
		if options.CompilerOptions:
			args.Append(options.CompilerOptions)
			args.Append(" ")
		
		for f as string in files:
			args.AppendFormat("\"{0}\" ",f)
		
		return args.ToString()
	*/
