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
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Security;
using System.Security.Permissions;
using System.Text.RegularExpressions;
using Boo.Lang.Compiler.Ast;
using Boo.Lang.Compiler.Util;
using Boo.Lang.Compiler.TypeSystem;
using Boo.Lang.Compiler.TypeSystem.Reflection;

namespace Boo.Lang.Compiler
{
	/// <summary>
	/// Compiler parameters.
	/// </summary>
	public class CompilerParameters
	{
		public static IReflectionTypeSystemProvider SharedTypeSystemProvider = new ReflectionTypeSystemProvider();

		private TextWriter _outputWriter;

		private CompilerPipeline _pipeline;

		private CompilerInputCollection _input;

		private CompilerResourceCollection _resources;

		private CompilerReferenceCollection _compilerReferences;

		private int _maxExpansionIterations;

		private string _outputAssembly;

		private CompilerOutputType _outputType;

		private bool _debug;

		private bool _ducky;

		private bool _checked;

		private bool _strict;

		private bool _generateInMemory;

		private bool _StdLib;

		private string _keyFile;

		private string _keyContainer;

		private bool _delaySign;

		private ArrayList _libpaths;

		private string _systemDir;

		private Assembly _booAssembly;
		
		private bool _whiteSpaceAgnostic;

		private TraceSwitch _traceSwitch;

		private Dictionary<string, string> _defines = new Dictionary<string, string>();

		private bool _unsafe;

		private string _platform;


		private TypeMemberModifiers _defaultTypeVisibility = TypeMemberModifiers.Public;
		private TypeMemberModifiers _defaultMethodVisibility = TypeMemberModifiers.Public;
		private TypeMemberModifiers _defaultPropertyVisibility = TypeMemberModifiers.Public;
		private TypeMemberModifiers _defaultEventVisibility = TypeMemberModifiers.Public;
		private TypeMemberModifiers _defaultFieldVisibility = TypeMemberModifiers.Protected;
		private bool _defaultVisibilitySettingsRead = false;


		public CompilerParameters()
			: this(true)
		{
		}

		public CompilerParameters(IReflectionTypeSystemProvider reflectionProvider, bool loadDefaultReferences)
		{
			_libpaths = new ArrayList();
			if (Permissions.HasDiscoveryPermission)
			{
				_systemDir = GetSystemDir();
				_libpaths.Add(_systemDir);
				_libpaths.Add(Directory.GetCurrentDirectory());
			}
			_pipeline = null;
			_input = new CompilerInputCollection();
			_resources = new CompilerResourceCollection();
			_compilerReferences = new CompilerReferenceCollection(reflectionProvider);

			_maxExpansionIterations = 12;
			_outputAssembly = String.Empty;
			_outputType = CompilerOutputType.Auto;
			_outputWriter = Console.Out;
			_debug = true;
			_checked = true;
			_generateInMemory = true;
			_StdLib = true;

			if (Permissions.HasEnvironmentPermission && null != Environment.GetEnvironmentVariable("TRACE"))
				EnableTraceSwitch();

			_delaySign = false;

			Strict = false;

			if (loadDefaultReferences)
				LoadDefaultReferences();
		}

		public CompilerParameters(bool loadDefaultReferences) : this(SharedTypeSystemProvider, loadDefaultReferences)
		{	
		}

		public void LoadDefaultReferences()
		{
			//mscorlib
			_compilerReferences.Add(
				LoadAssembly("mscorlib", true)
				);
			//System
			_compilerReferences.Add(
				LoadAssembly("System", true)
				);
			//boo.lang.dll
			_booAssembly = typeof(Boo.Lang.Builtins).Assembly;
			_compilerReferences.Add(_booAssembly);

			//boo.lang.extensions.dll
			//try loading extensions next to Boo.Lang (in the same directory)
			string tentative = "Boo.Lang.Extensions.dll";
			if (Permissions.HasDiscoveryPermission)
				tentative = Path.Combine(Path.GetDirectoryName(_booAssembly.Location), tentative);
			ICompileUnit extensionsAssembly = LoadAssembly(tentative, false);
			if(extensionsAssembly == null)//if failed, try loading from the gac
				extensionsAssembly = LoadAssembly("Boo.Lang.Extensions", false);
			if(extensionsAssembly != null)
				_compilerReferences.Add(extensionsAssembly);

			if (TraceInfo)
			{
				Trace.WriteLine("BOO LANG DLL: " + _booAssembly.Location);
				Trace.WriteLine("BOO COMPILER EXTENSIONS DLL: " + 
				                (extensionsAssembly != null ? extensionsAssembly.ToString() : "NOT FOUND!"));
			}
		}

		public Assembly BooAssembly
		{
			get { return _booAssembly; }
			set
			{
				if (null == value)
					throw new ArgumentNullException("value");

				if (value != _booAssembly)
				{
					_compilerReferences.Remove(_booAssembly);
					_booAssembly = value;
					_compilerReferences.Add(value);
				}
			}
		}

		public ICompileUnit FindAssembly(string name)
		{
			return _compilerReferences.Find(name);
		}

		public void AddAssembly(Assembly asm)
		{
			if (null == asm) throw new ArgumentNullException();
			_compilerReferences.Add(asm);
		}

		public IAssemblyReference LoadAssembly(string assembly)
		{
			return LoadAssembly(assembly, true);
		}

		public IAssemblyReference LoadAssembly(string assemblyName, bool throwOnError)
		{
			Assembly assembly = ForName(assemblyName, throwOnError);
			if (null == assembly)
				return null;
			return _compilerReferences.Provider.ForAssembly(assembly);
		}

		private Assembly ForName(string assembly, bool throwOnError)
		{
			Assembly a = null;
			try
			{
				if (assembly.IndexOfAny(new char[] {'/', '\\'}) != -1)
				{
					//nant passes full path to gac dlls, which compiler doesn't like:
					//if (assembly.ToLower().StartsWith(_systemDir.ToLower()))
					{
						//return LoadAssemblyFromGac(Path.GetFileName(assembly));
					}
					//else //load using path  
					{
						a = Assembly.LoadFrom(assembly);
					}
				}
				else
				{
					a = LoadAssemblyFromGac(assembly);
				}
			}
			catch (FileNotFoundException /*ignored*/)
			{
				return LoadAssemblyFromLibPaths(assembly, throwOnError);
			}
			catch (BadImageFormatException e)
			{
				if (throwOnError)
				{
					throw new ApplicationException(ResourceManager.Format(
					                               	"BooC.BadFormat",
					                               	e.FusionLog), e);
				}
			}
			catch (FileLoadException e)
			{
				if (throwOnError)
				{
					throw new ApplicationException(ResourceManager.Format(
					                               	"BooC.UnableToLoadAssembly",
					                               	e.FusionLog), e);
				}
			}
			catch (ArgumentNullException e)
			{
				if (throwOnError)
				{
					throw new ApplicationException(ResourceManager.Format(
					                               	"BooC.NullAssembly"), e);
				}
			}
			if (a == null)
			{
				return LoadAssemblyFromLibPaths(assembly, throwOnError);
			}
			return a;
		}

		private Assembly LoadAssemblyFromLibPaths(string assembly, bool throwOnError)
		{
			Assembly a = null;
			string fullLog = "";
			foreach (string dir in _libpaths)
			{
				string full_path = Path.Combine(dir, assembly);
				FileInfo file = new FileInfo(full_path);
				if (!IsAssemblyExtension(file.Extension))
					full_path += ".dll";

				try
				{
					a = Assembly.LoadFrom(full_path);
					if (a != null)
					{
						return a;
					}
				}
				catch (FileNotFoundException ff)
				{
					fullLog += ff.FusionLog;
					continue;
				}
			}
			if (throwOnError)
			{
				throw new ApplicationException(ResourceManager.Format(
				                               	"BooC.CannotFindAssembly",
				                               	assembly));
				//assembly, total_log)); //total_log contains the fusion log
			}
			return a;
		}

		private static bool IsAssemblyExtension(string extension)
		{
			switch (extension.ToLower())
			{
				case ".dll":
				case ".exe":
					return true;
			}
			return false;
		}

		private static Assembly LoadAssemblyFromGac(string assemblyName)
		{
			assemblyName = NormalizeAssemblyName(assemblyName);
			// This is an intentional attempt to load an assembly with partial name
			// so ignore the compiler warning
			#pragma warning disable 618	
			Assembly assembly = Assembly.LoadWithPartialName(assemblyName);
			#pragma warning restore 618
			if (assembly != null) return assembly;
			return Assembly.Load(assemblyName);
		}

		private static string NormalizeAssemblyName(string assembly)
		{
			if (assembly.EndsWith(".dll") || assembly.EndsWith(".exe"))
			{
				assembly = assembly.Substring(0, assembly.Length - 4);
			}
			return assembly;
		}

		public void LoadReferencesFromPackage(string package)
		{
			string[] libs = Regex.Split(pkgconfig(package), @"\-r\:", RegexOptions.CultureInvariant);
			foreach (string r in libs)
			{
				string reference = r.Trim();
				if (reference.Length == 0) continue;
				Trace.WriteLine("LOADING REFERENCE FROM PKGCONFIG '" + package + "' : " + reference);
				References.Add(LoadAssembly(reference));
			}
		}

		private static string pkgconfig(string package)
		{
#if NO_SYSTEM_DLL
	        throw new System.NotSupportedException();
#else
			Process process;
			try
			{
				process = Builtins.shellp("pkg-config", String.Format("--libs {0}", package));
			}
			catch (Exception e)
			{
				throw new ApplicationException(ResourceManager.GetString("BooC.PkgConfigNotFound"), e);
			}
			process.WaitForExit();
			if (process.ExitCode != 0)
			{
				throw new ApplicationException(
					ResourceManager.Format("BooC.PkgConfigReportedErrors", process.StandardError.ReadToEnd()));
			}
			return process.StandardOutput.ReadToEnd();
#endif
		}

		private string GetSystemDir()
		{
			return Path.GetDirectoryName(typeof(string).Assembly.Location);
		}

		/// <summary>
		/// Max number of iterations for the application of AST attributes and the
		/// expansion of macros.		
		/// </summary>
		public int MaxExpansionIterations
		{
			get { return _maxExpansionIterations; }

			set { _maxExpansionIterations = value; }
		}

		public CompilerInputCollection Input
		{
			get { return _input; }
		}

		public ArrayList LibPaths
		{
			get { return _libpaths; }
		}

		public CompilerResourceCollection Resources
		{
			get { return _resources; }
		}

		public CompilerReferenceCollection References
		{
			get { return _compilerReferences; }

			set
			{
				if (null == value) throw new ArgumentNullException("References");
				_compilerReferences = value;
			}
		}

		/// <summary>
		/// The compilation pipeline.
		/// </summary>
		public CompilerPipeline Pipeline
		{
			get { return _pipeline; }

			set { _pipeline = value; }
		}

		/// <summary>
		/// The name (full or partial) for the file
		/// that should receive the resulting assembly.
		/// </summary>
		public string OutputAssembly
		{
			get { return _outputAssembly; }

			set
			{
				if (String.IsNullOrEmpty(value)) throw new ArgumentNullException("OutputAssembly");
				_outputAssembly = value;
			}
		}

		/// <summary>
		/// Type and execution subsystem for the generated portable
		/// executable file.
		/// </summary>
		public CompilerOutputType OutputType
		{
			get { return _outputType; }

			set { _outputType = value; }
		}

		public bool GenerateInMemory
		{
			get { return _generateInMemory; }

			set { _generateInMemory = value; }
		}

		public bool StdLib
		{
			get { return _StdLib; }

			set { _StdLib = value; }
		}

		public TextWriter OutputWriter
		{
			get { return _outputWriter; }

			set
			{
				if (null == value) throw new ArgumentNullException("OutputWriter");
				_outputWriter = value;
			}
		}

		public bool Debug
		{
			get { return _debug; }

			set { _debug = value; }
		}

		/// <summary>
		/// Use duck instead of object as the most generic type.
		/// </summary>
		public bool Ducky
		{
			get { return _ducky; }

			set { _ducky = value; }
		}

		public bool Checked
		{
			get { return _checked; }

			set { _checked = value; }
		}

		public string KeyFile
		{
			get { return _keyFile; }

			set { _keyFile = value; }
		}

		public string KeyContainer
		{
			get { return _keyContainer; }

			set { _keyContainer = value; }
		}

		public bool DelaySign
		{
			get { return _delaySign; }

			set { _delaySign = value; }
		}
		
		public bool WhiteSpaceAgnostic
		{
			get { return _whiteSpaceAgnostic; }
			set { _whiteSpaceAgnostic = value; }
		}

		public Dictionary<string, string> Defines
		{
			get { return _defines; }
		}

		public TypeMemberModifiers DefaultTypeVisibility
		{
			get
			{
				if (!_defaultVisibilitySettingsRead)
					ReadDefaultVisibilitySettings();
				return _defaultTypeVisibility;
			}
			set
			{
				_defaultTypeVisibility = value & TypeMemberModifiers.VisibilityMask;
			}
		}

		public TypeMemberModifiers DefaultMethodVisibility
		{
			get
			{
				if (!_defaultVisibilitySettingsRead)
					ReadDefaultVisibilitySettings();
				return _defaultMethodVisibility;
			}
			set
			{
				_defaultMethodVisibility = value & TypeMemberModifiers.VisibilityMask;
			}
		}

		public TypeMemberModifiers DefaultPropertyVisibility
		{
			get
			{
				if (!_defaultVisibilitySettingsRead)
					ReadDefaultVisibilitySettings();
				return _defaultPropertyVisibility;
			}
			set
			{
				_defaultPropertyVisibility = value & TypeMemberModifiers.VisibilityMask;
			}
		}

		public TypeMemberModifiers DefaultEventVisibility
		{
			get
			{
				if (!_defaultVisibilitySettingsRead)
					ReadDefaultVisibilitySettings();
				return _defaultEventVisibility;
			}
			set
			{
				_defaultEventVisibility = value & TypeMemberModifiers.VisibilityMask;
			}
		}

		public TypeMemberModifiers DefaultFieldVisibility
		{
			get
			{
				if (!_defaultVisibilitySettingsRead)
					ReadDefaultVisibilitySettings();
				return _defaultFieldVisibility;
			}
			set
			{
				_defaultFieldVisibility = value & TypeMemberModifiers.VisibilityMask;
			}
		}

		internal TraceSwitch TraceSwitch
		{
			get { return _traceSwitch; }
			set
			{
				if (null == _traceSwitch)
					_traceSwitch = value;
			}
		}

		public bool TraceInfo
		{
			get { return (null != _traceSwitch && _traceSwitch.TraceInfo); }
		}

		public bool TraceWarning
		{
			get { return (null != _traceSwitch && _traceSwitch.TraceWarning); }
		}

		public bool TraceError
		{
			get { return (null != _traceSwitch && _traceSwitch.TraceError); }
		}

		public bool TraceVerbose
		{
			get { return (null != _traceSwitch && _traceSwitch.TraceVerbose); }
		}

		public TraceLevel TraceLevel
		{
			get {
				return (null != _traceSwitch)
							? _traceSwitch.Level : TraceLevel.Off;
			}
			set {
				EnableTraceSwitch();
				_traceSwitch.Level = value;
			}
		}

		public void EnableTraceSwitch()
		{
			if (null == _traceSwitch)
				_traceSwitch = new TraceSwitch("booc", "boo compiler");
		}
		
		private void ReadDefaultVisibilitySettings()
		{
			string visibility = null;

			if (_defines.TryGetValue("DEFAULT_TYPE_VISIBILITY", out visibility))
				DefaultTypeVisibility = ParseVisibility(visibility);

			if (_defines.TryGetValue("DEFAULT_METHOD_VISIBILITY", out visibility))
				DefaultMethodVisibility = ParseVisibility(visibility);

			if (_defines.TryGetValue("DEFAULT_PROPERTY_VISIBILITY", out visibility))
				DefaultPropertyVisibility = ParseVisibility(visibility);

			if (_defines.TryGetValue("DEFAULT_EVENT_VISIBILITY", out visibility))
				DefaultEventVisibility = ParseVisibility(visibility);

			if (_defines.TryGetValue("DEFAULT_FIELD_VISIBILITY", out visibility))
				DefaultFieldVisibility = ParseVisibility(visibility);

			_defaultVisibilitySettingsRead = true;
		}

		private static TypeMemberModifiers ParseVisibility(string visibility)
		{
			if (String.IsNullOrEmpty(visibility))
				throw new ArgumentNullException("visibility");

			visibility = visibility.ToLower();
			switch (visibility)
			{
				case "public":
					return TypeMemberModifiers.Public;
				case "protected":
					return TypeMemberModifiers.Protected;
				case "internal":
					return TypeMemberModifiers.Internal;
				case "private":
					return TypeMemberModifiers.Private;
			}
			throw new ArgumentException("visibility", String.Format("Invalid visibility: '{0}'", visibility));
		}

		bool _noWarn = false;
		bool _warnAsError = false;
		Util.Set<string> _disabledWarnings = new Util.Set<string>();
		Util.Set<string> _promotedWarnings = new Util.Set<string>();

		public bool NoWarn
		{
			get { return _noWarn; }
			set { _noWarn = value; }
		}

		public bool WarnAsError
		{
			get { return _warnAsError; }
			set { _warnAsError = value; }
		}

		public ICollection<string> DisabledWarnings
		{
			get { return _disabledWarnings; }
		}

		public ICollection<string> WarningsAsErrors
		{
			get { return _promotedWarnings; }
		}

		public void EnableWarning(string code)
		{
			if (_disabledWarnings.Contains(code))
				_disabledWarnings.Remove(code);
		}

		public void DisableWarning(string code)
		{
			_disabledWarnings.Add(code);
		}

		public void ResetWarnings()
		{
			_noWarn = false;
			_disabledWarnings.Clear();
			Strict = _strict;
		}

		public void EnableWarningAsError(string code)
		{
			_promotedWarnings.Add(code);
		}

		public void DisableWarningAsError(string code)
		{
			if (_promotedWarnings.Contains(code))
				_promotedWarnings.Remove(code);
		}

		public void ResetWarningsAsErrors()
		{
			_warnAsError = false;
			_promotedWarnings.Clear();
		}

		public bool Strict
		{
			get { return _strict; }
			set {
				_strict = value;
				if (_strict)
				{
					/*strict mode*/
					_defaultTypeVisibility = TypeMemberModifiers.Private;
					_defaultMethodVisibility = TypeMemberModifiers.Private;
					_defaultPropertyVisibility = TypeMemberModifiers.Private;
					_defaultEventVisibility = TypeMemberModifiers.Private;
					_defaultFieldVisibility = TypeMemberModifiers.Private;

					EnableWarning("BCW0023");
					EnableWarning("BCW0024");
					DisableWarning("BCW0028"); //by default strict mode forbids implicit downcasts
					                           //disable warning so we get only the regular incompatible type error
				}
				else
				{
					/*default mode*/
					_defaultTypeVisibility = TypeMemberModifiers.Public;
					_defaultMethodVisibility = TypeMemberModifiers.Public;
					_defaultPropertyVisibility = TypeMemberModifiers.Public;
					_defaultEventVisibility = TypeMemberModifiers.Public;
					_defaultFieldVisibility = TypeMemberModifiers.Protected;

					DisableWarning("BCW0023");
					DisableWarning("BCW0024");
					DisableWarning("BCW0028");
				}
			}
		}

		public bool Unsafe
		{
			get { return _unsafe; }
			set { _unsafe = value; }
		}

		public string Platform
		{
			get { return _platform; }
			set { _platform = value; }
		}
	}
}
