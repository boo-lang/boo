// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Xml;
using System.Diagnostics;
using System.ComponentModel;
using ICSharpCode.SharpDevelop.Gui.Components;
using ICSharpCode.SharpDevelop.Internal.Project;

namespace CSharpBinding
{
	public enum CompileTarget {
		Exe, 
		WinExe, 
		Library,
		Module
	};
	
	public enum CsharpCompiler {
		Csc,
		Mcs
	};
	
	public enum NetRuntime {
		Mono,
		MonoInterpreter,
		MsNet
	};
	
	/// <summary>
	/// This class handles project specific compiler parameters
	/// </summary>
	public class CSharpCompilerParameters : AbstractProjectConfiguration
	{
		[XmlNodeName("CodeGeneration")]
		public class CodeGeneration 
		{
			[XmlAttribute("runtime")]
			public NetRuntime netRuntime         = NetRuntime.MsNet;
			
			[XmlAttribute("compiler")]
			public CsharpCompiler csharpCompiler = CsharpCompiler.Csc;
			
			[XmlAttribute("warninglevel")]
			public int  warninglevel       = 4;
			
			[XmlAttribute("nowarn")]
			public string noWarnings      = String.Empty;
			
			[XmlAttribute("includedebuginformation")]
			public bool debugmode          = true;
			
			[XmlAttribute("optimize")]
			public bool optimize           = true;
			
			[XmlAttribute("unsafecodeallowed")]
			public bool unsafecode         = false;
			
			[XmlAttribute("generateoverflowchecks")]
			public bool generateOverflowChecks = true;
			
			[XmlAttribute("mainclass")]
			public string         mainclass     = null;
			
			[XmlAttribute("target")]
			public CompileTarget  compiletarget = CompileTarget.Exe;
			
			[XmlAttribute("definesymbols")]
			public string         definesymbols = String.Empty;
			
			[XmlAttribute("generatexmldocumentation")]
			public bool generateXmlDocumentation = false;
			
			[ConvertToRelativePathAttribute()]
			[XmlAttribute("win32Icon")]
			public string         win32Icon     = String.Empty;
		}
		
		[XmlNodeName("Execution")]
		public class Execution
		{
			[XmlAttribute("commandlineparameters")]
			public string  commandLineParameters = String.Empty;
			
			[XmlAttribute("consolepause")]
			public bool    pauseconsoleoutput = true;
		}
		
		protected CodeGeneration codeGeneration = new CodeGeneration();
		protected Execution      execution      = new Execution();
		
		[Browsable(false)]
		public CsharpCompiler CsharpCompiler {
			get {
				return codeGeneration.csharpCompiler;
			}
			set {
				codeGeneration.csharpCompiler = value;
			}
		}
		
		[Browsable(false)]
		public NetRuntime NetRuntime {
			get {
				return codeGeneration.netRuntime;
			}
			set {
				codeGeneration.netRuntime = value;
			}
		}
		
		[Browsable(false)]
		public string Win32Icon {
			get {
				return codeGeneration.win32Icon;
			}
			set {
				codeGeneration.win32Icon = value;
			}
		}
#region Code Generation
		[DefaultValue("")]
		[LocalizedProperty("${res:BackendBindings.CompilerOptions.CodeGeneration.MainClass}",
		                   Category    = "${res:BackendBindings.CompilerOptions.CodeGeneration}",
		                   Description = "${res:BackendBindings.CompilerOptions.CodeGeneration.MainClass.Description}")]
		public string MainClass {
			get {
				return codeGeneration.mainclass;
			}
			set {
				codeGeneration.mainclass = value;
			}
		}
		
		[DefaultValue("")]
		[LocalizedProperty("${res:BackendBindings.CompilerOptions.CodeGeneration.DefineSymbols}",
		                   Category    = "${res:BackendBindings.CompilerOptions.CodeGeneration}",
		                   Description = "${res:BackendBindings.CompilerOptions.CodeGeneration.DefineSymbols.Description}")]
		public string DefineSymbols {
			get {
				return codeGeneration.definesymbols;
			}
			set {
				codeGeneration.definesymbols = value;
			}
		}
		
		[DefaultValue(true)]
		[LocalizedProperty("${res:BackendBindings.CompilerOptions.CodeGeneration.DebugMode}",
		                   Category    = "${res:BackendBindings.CompilerOptions.CodeGeneration}",
		                   Description = "${res:BackendBindings.CompilerOptions.CodeGeneration.DebugMode.Description}")]
		public bool Debugmode {
			get {
				return codeGeneration.debugmode;
			}
			set {
				codeGeneration.debugmode = value;
			}
		}
		
		[DefaultValue(true)]
		[LocalizedProperty("${res:BackendBindings.CompilerOptions.CodeGeneration.Optimize}",
		                   Category    = "${res:BackendBindings.CompilerOptions.CodeGeneration}",
		                   Description = "${res:BackendBindings.CompilerOptions.CodeGeneration.Optimize.Description}")]
		public bool Optimize {
			get {
				return codeGeneration.optimize;
			}
			set {
				codeGeneration.optimize = value;
			}
		}
		
		[DefaultValue(false)]
		[LocalizedProperty("${res:BackendBindings.CompilerOptions.CodeGeneration.UnsafeCode}",
		                   Category    = "${res:BackendBindings.CompilerOptions.CodeGeneration}",
		                   Description = "${res:BackendBindings.CompilerOptions.CodeGeneration.UnsafeCode.Description}")]
		public bool UnsafeCode {
			get {
				return codeGeneration.unsafecode;
			}
			set {
				codeGeneration.unsafecode = value;
			}
		}
		
		[DefaultValue(true)]
		[LocalizedProperty("${res:BackendBindings.CompilerOptions.CodeGeneration.GenerateOverflowChecks}",
		                   Category    = "${res:BackendBindings.CompilerOptions.CodeGeneration}",
		                   Description = "${res:BackendBindings.CompilerOptions.CodeGeneration.GenerateOverflowChecks.Description}")]
		public bool GenerateOverflowChecks {
			get {
				return codeGeneration.generateOverflowChecks;
			}
			set {
				codeGeneration.generateOverflowChecks = value;
			}
		}
		
		[DefaultValue(false)]
		[LocalizedProperty("${res:BackendBindings.CompilerOptions.CodeGeneration.GenerateXmlDocumentation}",
		                   Category    = "${res:BackendBindings.CompilerOptions.CodeGeneration}",
		                   Description = "${res:BackendBindings.CompilerOptions.CodeGeneration.GenerateXmlDocumentation.Description}")]
		public bool GenerateXmlDocumentation {
			get {
				return codeGeneration.generateXmlDocumentation;
			}
			set {
				codeGeneration.generateXmlDocumentation = value;
			}
		}
		
#endregion

#region Errors and Warnings 
		[DefaultValue(4)]
		[LocalizedProperty("${res:BackendBindings.CompilerOptions.WarningAndErrorCategory.WarningLevel}",
		                   Category    = "${res:BackendBindings.CompilerOptions.WarningAndErrorCategory}",
		                   Description = "${res:BackendBindings.CompilerOptions.WarningAndErrorCategory.WarningLevel.Description}")]
		public int WarningLevel {
			get {
				return codeGeneration.warninglevel;
			}
			set {
				codeGeneration.warninglevel = value;
			}
		}
		
		[DefaultValue("")]
		[LocalizedProperty("${res:BackendBindings.CompilerOptions.WarningAndErrorCategory.NoWarnings}",
		                   Category    = "${res:BackendBindings.CompilerOptions.WarningAndErrorCategory}",
		                   Description = "${res:BackendBindings.CompilerOptions.WarningAndErrorCategory.NoWarnings.Description}")]
		public string NoWarnings {
			get {
				return codeGeneration.noWarnings;
			}
			set {
				codeGeneration.noWarnings = value;
			}
		}
#endregion
		[Browsable(false)]
		public string CommandLineParameters {
			get {
				return execution.commandLineParameters;
			}
			set {
				execution.commandLineParameters = value;
			}
		}
		
		[Browsable(false)]
		public bool PauseConsoleOutput {
			get {
				return execution.pauseconsoleoutput;
			}
			set {
				execution.pauseconsoleoutput = value;
			}
		}
		
		
		[Browsable(false)]
		public CompileTarget CompileTarget {
			get {
				return codeGeneration.compiletarget;
			}
			set {
				codeGeneration.compiletarget = value;
			}
		}
		
		public CSharpCompilerParameters()
		{
		}
		public CSharpCompilerParameters(string name)
		{
			this.name = name;
		}
	}
}
