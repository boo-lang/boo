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
// Ian MacLean

using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Configuration;
using System.IO;
using System.Text;
using System.Reflection;
using System.Collections;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Text.RegularExpressions;

using Boo.Lang.Compiler.Pipelines;
using BooC = Boo.Lang.Compiler;
using Boo.Lang.Compiler.IO;
using Boo.Lang.Compiler.Ast;
using Boo.Lang.Compiler.Steps;

namespace Boo.CodeDom
{
	/// <summary>
	/// ICodeCompiler implementation for Boo.
	/// </summary>
	internal class BooCodeCompiler : BooCodeGenerator, ICodeCompiler
	{
		public BooCodeCompiler ()
		{
		}

		public CompilerResults CompileAssemblyFromDom(
			CompilerParameters options, CodeCompileUnit e)
		{
			return CompileAssemblyFromDomBatch(options,
				new CodeCompileUnit [] {e});
		}
		public CompilerResults CompileAssemblyFromDomBatch( 
			CompilerParameters options, CodeCompileUnit [] ea)
		{
			string[] fileNames = new string [ea.Length];
			int i = 0;
			if (options == null)
				options = new CompilerParameters ();
			
			StringCollection assemblies = options.ReferencedAssemblies;

			foreach (CodeCompileUnit e in ea)
			{
				fileNames [i] = GetTempFileNameWithExtension(options.TempFiles, i.ToString () + ".boo");
				FileStream f = new FileStream (fileNames [i],FileMode.OpenOrCreate);
				StreamWriter s = new StreamWriter (f, Encoding.UTF8);
				if (e.ReferencedAssemblies != null)
				{
					foreach (string str in e.ReferencedAssemblies)
					{
						if (!assemblies.Contains (str))
						{
							assemblies.Add (str);
						}
					}
				}

				((ICodeGenerator) this).GenerateCodeFromCompileUnit (e, s, new CodeGeneratorOptions());
				s.Close();
				f.Close();
				i++;
			}
			return CompileAssemblyFromFileBatch (options, fileNames);
		}
		
		public CompilerResults CompileAssemblyFromFile( 
			CompilerParameters options, string fileName)
		{
			return CompileAssemblyFromFileBatch (options, new string [] {fileName});
		}

	protected bool processCompileResult(BooC.CompilerContext context, CompilerResults results)
	{
		foreach (BooC.CompilerError booError in context.Errors)
		{
			CompilerError error=new CompilerError();
			error.ErrorNumber = booError.Code;
			error.Line = booError.LexicalInfo.Line;
			error.Column = booError.LexicalInfo.Column;
			error.FileName = booError.LexicalInfo.FileName;
			error.ErrorText = booError.Message;
			error.IsWarning = false;
			results.Errors.Add (error);
		}
		if (context.Errors.Count > 0)
		{
			return false;
		}
		else
		{
			return true;
		}
	}
	
	public CompilerResults CompileAssemblyFromFileBatch(CompilerParameters options, string [] fileNames)
	{
		if (null == options)
			throw new ArgumentNullException("options");
		if (null == fileNames)
			throw new ArgumentNullException("fileNames");

		CompilerResults results = new CompilerResults(options.TempFiles);
		BooC.BooCompiler compiler = new BooC.BooCompiler();
		BooC.CompilerParameters parameters = compiler.Parameters;
		
		if (options.OutputAssembly == null)
		{
			options.OutputAssembly = GetTempFileNameWithExtension (options.TempFiles, "dll");
		}
		parameters.OutputAssembly = options.OutputAssembly;
		
		// set compile options
		if ( options.GenerateInMemory )
		{
			parameters.Pipeline = new CompileToMemory();
		}
		else
		{
			if (options.OutputAssembly == null)
			{
				options.OutputAssembly = GetTempFileNameWithExtension (options.TempFiles, "dll");
			}
			parameters.Pipeline = new CompileToFile();
		}
		
		if (options.GenerateExecutable)
		{
			parameters.OutputType = BooC.CompilerOutputType.ConsoleApplication; // winexe ??
		}
		else
		{
			parameters.OutputType = BooC.CompilerOutputType.Library;
		}
		parameters.Debug = options.IncludeDebugInformation;
			
		if (null != options.ReferencedAssemblies)
		{
			foreach (string import in options.ReferencedAssemblies)
			{
				parameters.References.Add( Assembly.LoadFrom(import));
			}
		}
		
		foreach ( string fileName in fileNames )
		{
			parameters.Input.Add(new FileInput(fileName));
		}
		 // run the compiler
		 BooC.CompilerContext context = compiler.Run();
		  
		bool loadIt = processCompileResult(context, results );
		
		if (loadIt)
		{
			results.CompiledAssembly = Assembly.LoadFrom(options.OutputAssembly);
		}
		else
		{
			results.CompiledAssembly = null;
		}
		
		return results;
	}
	
	
	public CompilerResults CompileAssemblyFromSource (
			CompilerParameters options, string source)
	{
		return CompileAssemblyFromSourceBatch (options,
		                                       new string [] {source});
	}
	
	public CompilerResults CompileAssemblyFromSourceBatch (
			CompilerParameters options, string [] sources)
	{
		if (null == options)
			throw new ArgumentNullException("options");
		if (null == sources)
			throw new ArgumentNullException("fileNames");

			CompilerResults results = new CompilerResults (options.TempFiles);
			BooC.BooCompiler compiler = new BooC.BooCompiler();
			BooC.CompilerParameters parameters = compiler.Parameters;
		
			if (options.OutputAssembly == null)
			{
				options.OutputAssembly = GetTempFileNameWithExtension (options.TempFiles, "dll");
			}
			parameters.OutputAssembly = options.OutputAssembly;
		
			// set compile options
			if ( options.GenerateInMemory )
			{
				parameters.Pipeline = new CompileToMemory();
			}
			else
			{
				parameters.Pipeline = new CompileToFile();
			}
		
			if (options.GenerateExecutable)
			{
				parameters.OutputType = BooC.CompilerOutputType.ConsoleApplication; // winexe ??
			}
			else
			{
				parameters.OutputType = BooC.CompilerOutputType.Library;
			}
			parameters.Debug = options.IncludeDebugInformation;
			
			if (null != options.ReferencedAssemblies)
			{
				foreach (string import in options.ReferencedAssemblies)
				{
					parameters.References.Add( Assembly.LoadFrom(import));
				}
			}
		 
			foreach (string source in sources)
			{
				parameters.Input.Add(new StringInput("source", source));
			}
			// run the compiler
			BooC.CompilerContext context = compiler.Run();
			
			bool loadIt = processCompileResult(context, results );
			
			if (loadIt)
			{
				results.CompiledAssembly = context.GeneratedAssembly;
				//results.CompiledAssembly = Assembly.LoadFrom(options.OutputAssembly);
			}
			else
			{
				results.CompiledAssembly = null;
			}
		
			return results;
		}

		static string GetTempFileNameWithExtension (
			TempFileCollection temp_files, string extension)
		{
			return temp_files.AddExtension (extension);
		}
	}
}
