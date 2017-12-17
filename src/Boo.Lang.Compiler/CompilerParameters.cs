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
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using Boo.Lang.Compiler.Ast;
using Boo.Lang.Compiler.Util;
using Boo.Lang.Compiler.TypeSystem;
using Boo.Lang.Compiler.TypeSystem.Reflection;
using Boo.Lang.Environments;
using Boo.Lang.Resources;

namespace Boo.Lang.Compiler
{
	/// <summary>
	/// Compiler parameters.
	/// </summary>
	public class CompilerParameters
	{
		public static IReflectionTypeSystemProvider SharedTypeSystemProvider = new ReflectionTypeSystemProvider();

		private TextWriter _outputWriter;

		private readonly CompilerInputCollection _input;

		private readonly CompilerResourceCollection _resources;

		private CompilerReferenceCollection _compilerReferences;

		private string _outputAssembly;

		private bool _strict;

		private readonly List<string> _libPaths;

		private readonly string _systemDir;

		private Assembly _booAssembly;

		private readonly Dictionary<string, string> _defines = new Dictionary<string, string>(StringComparer.Ordinal);

		private TypeMemberModifiers _defaultTypeVisibility = TypeMemberModifiers.Public;
		private TypeMemberModifiers _defaultMethodVisibility = TypeMemberModifiers.Public;
		private TypeMemberModifiers _defaultPropertyVisibility = TypeMemberModifiers.Public;
		private TypeMemberModifiers _defaultEventVisibility = TypeMemberModifiers.Public;
		private TypeMemberModifiers _defaultFieldVisibility = TypeMemberModifiers.Protected;
		private bool _defaultVisibilitySettingsRead;

		public CompilerParameters() : this(true)
		{
		}

		public CompilerParameters(bool loadDefaultReferences) : this(SharedTypeSystemProvider, loadDefaultReferences)
		{	
		}

		public CompilerParameters(IReflectionTypeSystemProvider reflectionProvider) : this(reflectionProvider, true)
		{
		}

		public CompilerParameters(IReflectionTypeSystemProvider reflectionProvider, bool loadDefaultReferences)
		{
			_libPaths = new List<string>();
			_systemDir = Permissions.WithDiscoveryPermission(() => GetSystemDir());
			if (_systemDir != null)
			{
				_libPaths.Add(_systemDir);
				_libPaths.Add(Directory.GetCurrentDirectory());
			}
			_input = new CompilerInputCollection();
			_resources = new CompilerResourceCollection();
			_compilerReferences = new CompilerReferenceCollection(reflectionProvider);

			MaxExpansionIterations = 12;
			_outputAssembly = String.Empty;
			OutputType = CompilerOutputType.Auto;
			_outputWriter = Console.Out;
			Debug = true;
			Checked = true;
#if !NET_40_OR_GREATER
            GenerateCollectible = false;
#else
            GenerateCollectible = true;
#endif
            GenerateInMemory = true;
			StdLib = true;
			DelaySign = false;

			Strict = false;
			TraceLevel = DefaultTraceLevel();

			if (loadDefaultReferences)
				LoadDefaultReferences();
		}

		private static TraceLevel DefaultTraceLevel()
		{
			var booTraceLevel = Permissions.WithEnvironmentPermission(() => System.Environment.GetEnvironmentVariable("BOO_TRACE_LEVEL"));
			return string.IsNullOrEmpty(booTraceLevel) ? TraceLevel.Off : (TraceLevel)Enum.Parse(typeof(TraceLevel), booTraceLevel);
		}

		public void LoadDefaultReferences()
		{
			//boo.lang.dll
			_booAssembly = typeof(Builtins).Assembly;
			_compilerReferences.Add(_booAssembly);

			//boo.lang.extensions.dll
			//try loading extensions next to Boo.Lang (in the same directory)
			var extensionsAssembly = TryToLoadExtensionsAssembly();
			if (extensionsAssembly != null)
				_compilerReferences.Add(extensionsAssembly);

			//boo.lang.compiler.dll
			_compilerReferences.Add(GetType().Assembly);

			//mscorlib
			_compilerReferences.Add(LoadAssembly("mscorlib", true));
			//System
			_compilerReferences.Add(LoadAssembly("System", true));
			//System.Core
			_compilerReferences.Add(LoadAssembly("System.Core", true));

			Permissions.WithDiscoveryPermission<object>(() =>
			{
				WriteTraceInfo("BOO LANG DLL: " + _booAssembly.Location);
				WriteTraceInfo("BOO COMPILER EXTENSIONS DLL: " + (extensionsAssembly != null ? extensionsAssembly.ToString() : "NOT FOUND!"));
				return null;
			});
		}

		private IAssemblyReference TryToLoadExtensionsAssembly()
		{
			const string booLangExtensionsDll = "Boo.Lang.Extensions.dll";
			return Permissions.WithDiscoveryPermission(() =>
			{
				var path = Path.Combine(Path.GetDirectoryName(_booAssembly.Location), booLangExtensionsDll);
				return File.Exists(path) ? AssemblyReferenceFor(Assembly.LoadFrom(path)) : null;
			}) ?? LoadAssembly(booLangExtensionsDll, false);
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
			var assembly = ForName(assemblyName, throwOnError);
			return assembly != null ? AssemblyReferenceFor(assembly) : null;
		}

		private IAssemblyReference AssemblyReferenceFor(Assembly assembly)
		{
			return _compilerReferences.Provider.ForAssembly(assembly);
		}

		protected virtual Assembly ForName(string assembly, bool throwOnError)
		{
			Assembly a = null;
			try
			{
				if (assembly.IndexOfAny(new char[] {'/', '\\'}) != -1)
					a = Assembly.LoadFrom(assembly);
				else
					a = LoadAssemblyFromGac(assembly);
			}
			catch (FileNotFoundException /*ignored*/)
			{
				return LoadAssemblyFromLibPaths(assembly, throwOnError);
			}
			catch (BadImageFormatException e)
			{
				if (throwOnError)
					throw new ApplicationException(string.Format(Boo.Lang.Resources.StringResources.BooC_BadFormat, e.FusionLog), e);
			}
			catch (FileLoadException e)
			{
				if (throwOnError)
					throw new ApplicationException(string.Format(Boo.Lang.Resources.StringResources.BooC_UnableToLoadAssembly, e.FusionLog), e);
			}
			catch (ArgumentNullException e)
			{
				if (throwOnError)
					throw new ApplicationException(Boo.Lang.Resources.StringResources.BooC_NullAssembly, e);
			}
			return a ?? LoadAssemblyFromLibPaths(assembly, false);
		}

		private Assembly LoadAssemblyFromLibPaths(string assembly, bool throwOnError)
		{
			Assembly a = null;
			string fullLog = "";
			foreach (string dir in _libPaths)
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
				throw new ApplicationException(string.Format(Boo.Lang.Resources.StringResources.BooC_CannotFindAssembly, assembly));
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
			var assembly = Permissions.WithDiscoveryPermission(()=> Assembly.LoadWithPartialName(assemblyName));
#pragma warning restore 618
			return assembly ?? Assembly.Load(assemblyName);
		}

		private static string NormalizeAssemblyName(string assembly)
		{
			var extension = Path.GetExtension(assembly).ToLower();
			if (extension == ".dll" || extension == ".exe")
				return assembly.Substring(0, assembly.Length - 4);
			return assembly;
		}

		public void LoadReferencesFromPackage(string package)
		{
			string[] libs = Regex.Split(pkgconfig(package), @"\-r\:", RegexOptions.CultureInvariant);
			foreach (string r in libs)
			{
				string reference = r.Trim();
				if (reference.Length == 0) continue;
				WriteTraceInfo("LOADING REFERENCE FROM PKGCONFIG '" + package + "' : " + reference);
				References.Add(LoadAssembly(reference));
			}
		}

		[Conditional("TRACE")]
		private void WriteTraceInfo(string message)
		{
			if (TraceInfo)
				Console.Error.WriteLine(message);
		}

		private static string pkgconfig(string package)
		{
#if NO_SYSTEM_PROCESS
	        throw new System.NotSupportedException();
#else
			Process process;
			try
			{
				process = Builtins.shellp("pkg-config", String.Format("--libs {0}", package));
			}
			catch (Exception e)
			{
				throw new ApplicationException(StringResources.BooC_PkgConfigNotFound, e);
			}
			process.WaitForExit();
			if (process.ExitCode != 0)
			{
				throw new ApplicationException(string.Format(StringResources.BooC_PkgConfigReportedErrors, process.StandardError.ReadToEnd()));
			}
			return process.StandardOutput.ReadToEnd();
#endif
		}

		private static string GetSystemDir()
		{
			return Path.GetDirectoryName(typeof(string).Assembly.Location);
		}

		/// <summary>
		/// Max number of iterations for the application of AST attributes and the
		/// expansion of macros.		
		/// </summary>
		public int MaxExpansionIterations { get; set; }

		public CompilerInputCollection Input
		{
			get { return _input; }
		}

		public List<string> LibPaths
		{
			get { return _libPaths; }
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
		public CompilerPipeline Pipeline { get; set; }

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
		public CompilerOutputType OutputType { get; set; }

        public bool GenerateCollectible { get; set; }

        public bool GenerateInMemory { get; set; }

		public bool StdLib { get; set; }

		public TextWriter OutputWriter
		{
			get { return _outputWriter; }

			set
			{
				if (null == value) throw new ArgumentNullException("OutputWriter");
				_outputWriter = value;
			}
		}

		public bool Debug { get; set; }

		/// <summary>
		/// Treat System.Object as duck
		/// </summary>
		public virtual bool Ducky { get; set; }

		public bool Checked { get; set; }

		public string KeyFile { get; set; }

		public string KeyContainer { get; set; }

		public bool DelaySign { get; set; }

		public bool WhiteSpaceAgnostic { get; set; }

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

		public bool TraceInfo
		{
			get { return TraceLevel >= TraceLevel.Info; }
		}

		public bool TraceWarning
		{
			get { return TraceLevel >= TraceLevel.Warning; }
		}

		public bool TraceError
		{
			get { return TraceLevel >= TraceLevel.Error; }
		}

		public bool TraceVerbose
		{
			get { return TraceLevel >= TraceLevel.Verbose; }
		}

		public TraceLevel TraceLevel { get; set; }
		
		private void ReadDefaultVisibilitySettings()
		{
			string visibility;

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

		Util.Set<string> _disabledWarnings = new Util.Set<string>();
		Util.Set<string> _promotedWarnings = new Util.Set<string>();

		public bool NoWarn { get; set; }

		public bool WarnAsError { get; set; }

        public string Icon { get; set; }

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
			NoWarn = false;
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
			WarnAsError = false;
			_promotedWarnings.Clear();
		}

		public bool Strict
		{
			get { return _strict; }
			set {
				_strict = value;
				if (_strict)
					OnStrictMode();
				else
					OnNonStrictMode();
			}
		}

		protected virtual void OnNonStrictMode()
		{
			_defaultTypeVisibility = TypeMemberModifiers.Public;
			_defaultMethodVisibility = TypeMemberModifiers.Public;
			_defaultPropertyVisibility = TypeMemberModifiers.Public;
			_defaultEventVisibility = TypeMemberModifiers.Public;
			_defaultFieldVisibility = TypeMemberModifiers.Protected;

			DisableWarning(CompilerWarningFactory.Codes.ImplicitReturn);
			DisableWarning(CompilerWarningFactory.Codes.VisibleMemberDoesNotDeclareTypeExplicitely);
			DisableWarning(CompilerWarningFactory.Codes.ImplicitDowncast);
		}

		protected virtual void OnStrictMode()
		{
			_defaultTypeVisibility = TypeMemberModifiers.Private;
			_defaultMethodVisibility = TypeMemberModifiers.Private;
			_defaultPropertyVisibility = TypeMemberModifiers.Private;
			_defaultEventVisibility = TypeMemberModifiers.Private;
			_defaultFieldVisibility = TypeMemberModifiers.Private;

			EnableWarning(CompilerWarningFactory.Codes.ImplicitReturn);
			EnableWarning(CompilerWarningFactory.Codes.VisibleMemberDoesNotDeclareTypeExplicitely);

			//by default strict mode forbids implicit downcasts
			//disable warning so we get only the regular incompatible type error
			DisableWarning(CompilerWarningFactory.Codes.ImplicitDowncast);
		}

		public bool Unsafe { get; set; }

		public string Platform { get; set; }

		public IEnvironment Environment { get; set; }
	}
}
