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

using System;
using System.Diagnostics;
using System.Collections;
using System.IO;
using System.Reflection;
using System.Globalization;

namespace Boo.Lang.Compiler
{
	/// <summary>
	/// Compiler parameters.
	/// </summary>
	public class CompilerParameters : System.MarshalByRefObject
	{
		static private bool _NET_2_0 = Environment.Version >= new Version(2, 0);
		
		static private bool _NET_1_1 = (Environment.Version.Major == 1 &&
						Environment.Version.Minor == 1);
		
		TextWriter _outputWriter;
		
		CompilerPipeline _pipeline;

		CompilerInputCollection _input;
		
		CompilerResourceCollection _resources;

		AssemblyCollection _assemblyReferences;

		int _maxAttributeSteps;
		
		string _outputAssembly;
		
		CompilerOutputType _outputType;
		
		bool _debug;
		
		bool _ducky;
		
		bool _generateInMemory;
		
		bool _StdLib;
		
		ArrayList _libpaths;
		
		public readonly TraceSwitch TraceSwitch = new TraceSwitch("booc", "boo compiler");
		
		public CompilerParameters(): this(true)
		{
		}
		
		public CompilerParameters(bool load_default_references)
		{
			_libpaths = new ArrayList();
			_libpaths.Add(GetSystemDir());
			_libpaths.Add(Directory.GetCurrentDirectory());
			
			_pipeline = null;
			_input = new CompilerInputCollection();
			_resources = new CompilerResourceCollection();
			_assemblyReferences = new AssemblyCollection();
			
			_maxAttributeSteps = 2;
			_outputAssembly = string.Empty;
			_outputType = CompilerOutputType.ConsoleApplication;
			_outputWriter = System.Console.Out;
			_debug = true;
			_generateInMemory = true;
			_StdLib = true;
			
			if (load_default_references) LoadDefaultReferences();
		}
		
		public void LoadDefaultReferences()
		{
			//mscorlib
			_assemblyReferences.Add(
				LoadAssembly("mscorlib", true)
				);
			//System
			_assemblyReferences.Add(
				LoadAssembly("System", true)
				);
			//boo.lang.dll
			_assemblyReferences.Add(LoadAssembly("Boo.Lang.dll", true));
			//boo.lang.compiler.dll
			_assemblyReferences.Add(GetType().Assembly);
		}
		
		static public bool NET_2_0
		{
			get
			{
				return _NET_2_0;
			}
		}
		
		static public bool NET_1_1
		{
			get
			{
				return _NET_1_1;
			}
		}
		
		public Assembly FindAssembly(string name)
		{
			return _assemblyReferences.Find(name);
		}
		
		public void AddAssembly(Assembly asm)
		{
			if (asm != null)
			{
				_assemblyReferences.Add(asm);
			}
		}
		
		public Assembly LoadAssembly (string assembly)
		{
			return LoadAssembly(assembly, true);
		}
		
		public Assembly LoadAssembly (string assembly, bool throw_errors)
		{
			Assembly a = null;
			
			try {
				char[] path_chars = { '/', '\\' };
				
				//hack: remove path if already in LibPaths (nant workaround)
				if (assembly.IndexOfAny (path_chars) != -1) {
					string assembly_dir = Path.GetDirectoryName(assembly);
					foreach (string dir in _libpaths)
					{
						if (string.Compare(dir,assembly_dir,true,CultureInfo.InvariantCulture)==0)
						{
							assembly = Path.GetFileName(assembly);
							break;
						}
					}
				}
				
				if (assembly.IndexOfAny(path_chars) != -1) {
					a = Assembly.LoadFrom(assembly);
				} else {
					a = LoadAssemblyFromGac(assembly, throw_errors);
				}
			} catch (FileNotFoundException f){
				return LoadAssemblyFromLibPaths(assembly, throw_errors);
			} catch (BadImageFormatException f) {
				if (throw_errors)
				{
					throw new ApplicationException(Boo.Lang.ResourceManager.Format(
						"BooC.BadFormat", 
						f.FusionLog));
				}
			} catch (FileLoadException f){
				if (throw_errors)
				{
					throw new ApplicationException(Boo.Lang.ResourceManager.Format(
						"BooC.UnableToLoadAssembly", 
						f.FusionLog));
				}
			} catch (ArgumentNullException){
				if (throw_errors)
				{
					throw new ApplicationException(Boo.Lang.ResourceManager.Format(
						"BooC.NullAssembly"));
				}
			}
			//this is for .net 1.1, which doesn't throw a filenotfoundexception:
			if (a==null)
			{
				return LoadAssemblyFromLibPaths(assembly, throw_errors);
			}
			return a;
		}
		
		private Assembly LoadAssemblyFromLibPaths(string assembly, bool throw_errors)
		{
			Assembly a = null;
			string total_log = "";
			foreach (string dir in _libpaths){
				string full_path = Path.Combine(dir, assembly);
				if (!assembly.EndsWith(".dll") && !assembly.EndsWith(".exe"))
					full_path += ".dll";

				try {
					a = Assembly.LoadFrom(full_path);
					if (a != null)
					{
						return a;
					}
				} catch (FileNotFoundException ff) {
					total_log += ff.FusionLog;
					continue;
				}
			}
			if (throw_errors)
			{
				throw new ApplicationException(Boo.Lang.ResourceManager.Format(
					"BooC.CannotFindAssembly", 
					assembly)); 
					//assembly, total_log)); //total_log contains the fusion log
			}
			return a;
		}
		
		private Assembly LoadAssemblyFromGac(string assembly, bool throw_errors)
		{
			Assembly a = null;
			string ass = assembly;
			if (ass.EndsWith(".dll") || ass.EndsWith(".exe"))
				ass = ass.Substring(0, ass.Length - 4);
			if (_NET_2_0)
			{
				a = Assembly.Load(ass);
			}
			else
			{
				a = Assembly.LoadWithPartialName(ass);
			}
			return a;
		}
		
		private string GetSystemDir()
		{
			Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();

			foreach (Assembly a in assemblies){
				string codebase = a.Location;
                                string fn = System.IO.Path.GetFileName(codebase);
				if (fn == "corlib.dll" || fn == "mscorlib.dll")
				{
					return codebase.Substring(0, codebase.LastIndexOf(Path.DirectorySeparatorChar));
				}
			}
			throw new ApplicationException(Boo.Lang.ResourceManager.Format(
						"BooC.NoSystemPath"));
		}
		
		/// <summary>
		/// Max number of steps for the resolution of AST attributes.		
		/// </summary>
		public int MaxAttributeSteps
		{
			get
			{
				return _maxAttributeSteps;
			}

			set
			{
				_maxAttributeSteps = value;
			}
		}

		public CompilerInputCollection Input
		{
			get
			{
				return _input;
			}
		}
		
		public ArrayList LibPaths
		{
			get
			{
				return _libpaths;
			}
		}
		
		public CompilerResourceCollection Resources
		{
			get
			{
				return _resources;
			}
		}

		public AssemblyCollection References
		{
			get
			{
				return _assemblyReferences;
			}
			
			set
			{
				if (null == value)
				{
					throw new ArgumentNullException("References");
				}
				_assemblyReferences = value;
			}
		}

		/// <summary>
		/// The compilation pipeline.
		/// </summary>
		public CompilerPipeline Pipeline
		{
			get
			{
				return _pipeline;
			}
			
			set
			{
				_pipeline = value;
			}
		}
		
		/// <summary>
		/// The name (full or partial) for the file
		/// that should receive the resulting assembly.
		/// </summary>
		public string OutputAssembly
		{
			get
			{
				return _outputAssembly;
			}
			
			set
			{
				if (null == value)
				{
					throw new ArgumentNullException("OutputAssembly");
				}
				if (0 == value.Length)
				{
					throw new ArgumentException("OutputAssembly");
				}
				_outputAssembly = value;
			}
		}
		
		/// <summary>
		/// Type and execution subsystem for the generated portable
		/// executable file.
		/// </summary>
		public CompilerOutputType OutputType
		{
			get
			{
				return _outputType;
			}
			
			set
			{
				_outputType = value;
			}
		}
		
		public bool GenerateInMemory
		{
			get
			{
				return _generateInMemory;
			}
			
			set
			{
				_generateInMemory = value;
			}
		}
		
		public bool StdLib
		{
			get
			{
				return _StdLib;
			}
			
			set
			{
				_StdLib = value;
			}
		}
		
		public TextWriter OutputWriter
		{
			get
			{
				return _outputWriter;
			}
			
			set
			{
				if (null == value)
				{
					throw new ArgumentNullException("OutputWriter");
				}
				_outputWriter = value;
			}
		}
		
		public bool Debug
		{
			get
			{
				return _debug;
			}
			
			set
			{
				_debug = value;
			}
		}
		
		/// <summary>
		/// Use duck instead of object as the most generic type.
		/// </summary>
		public bool Ducky
		{
			get
			{
				return _ducky;
			}
			
			set
			{
				_ducky = value;
			}
		}
	}
}
