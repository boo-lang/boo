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
using Boo.Lang.Compiler.Ast;
using Boo.Lang.Compiler.Services;
using Boo.Lang.Compiler.TypeSystem.Reflection;
using Assembly = System.Reflection.Assembly;
using Boo.Lang.Compiler.TypeSystem;
using Boo.Lang.Environments;

namespace Boo.Lang.Compiler
{
	/// <summary>
	/// boo compilation context.
	/// </summary>
	public class CompilerContext
	{
		public static CompilerContext Current
		{
			get { return ActiveEnvironment.Instance != null ? My<CompilerContext>.Instance : null; }
		}

		protected CompilerParameters _parameters;

		protected CompileUnit _unit;

		protected CompilerReferenceCollection _references;

		protected CompilerErrorCollection _errors;
		
		protected CompilerWarningCollection _warnings;
		
		protected TraceSwitch _traceSwitch;

		protected Assembly _generatedAssembly;
		
		protected string _generatedAssemblyFileName;
		
		protected Hash _properties;
		
		public CompilerContext() : this(new CompileUnit())
		{
		}

		public CompilerContext(CompileUnit unit) : this(new CompilerParameters(), unit)
		{
		}

		public CompilerContext(bool stdlib) : this(new CompilerParameters(stdlib), new CompileUnit())
		{
		}
		
		public CompilerContext(CompilerParameters options) : this(options, new CompileUnit())
		{
		}
		
		public CompilerContext(CompilerParameters options, CompileUnit unit)
		{
			if (null == options) throw new ArgumentNullException("options");
			if (null == unit) throw new ArgumentNullException("unit");

			_unit = unit;
			_errors = new CompilerErrorCollection();
			_warnings = new CompilerWarningCollection();
			_warnings.Adding += OnCompilerWarning;

			_references = options.References;
			_parameters = options;

			if (_parameters.Debug && !_parameters.Defines.ContainsKey("DEBUG"))
				_parameters.Defines.Add("DEBUG", null);

			_properties = new Hash();

			var activator = new InstantiatingEnvironment();
			_environment = _parameters.Environment != null
				? new CachingEnvironment(new EnvironmentChain(_parameters.Environment, activator))
				: new CachingEnvironment(activator);
			_environment.InstanceCached += InitializeService;

			// FIXME: temporary hack to make sure the singleton is visible
			// using the My<IReflectionTypeSystemProvider> idiom
			RegisterService<IReflectionTypeSystemProvider>(_references.Provider);
            RegisterService<CompilerParameters>(_parameters);
			RegisterService<CompilerErrorCollection>(_errors);
			RegisterService<CompilerWarningCollection>(_warnings);
			RegisterService<CompileUnit>(_unit);
            RegisterService<CompilerContext>(this);
		}

		public IEnvironment Environment
		{
			get { return _environment;  }
		}

		public Hash Properties
		{
			get { return _properties; }
		}
		
		public string GeneratedAssemblyFileName
		{
			get { return _generatedAssemblyFileName; }
			
			set
			{
				if (string.IsNullOrEmpty(value))
					throw new ArgumentNullException("GeneratedAssemblyFileName");
				_generatedAssemblyFileName = value;
			}
		}
		
		public object this[object key]
		{
			get { return _properties[key]; }
			
			set { _properties[key] = value; }
		}

		public CompilerParameters Parameters
		{
			get { return _parameters; }
		}

		public CompilerReferenceCollection References
		{
			get { return _references; }
		}

		public CompilerErrorCollection Errors
		{
			get { return _errors; }
		}
		
		public CompilerWarningCollection Warnings
		{
			get { return _warnings; }
		}

		public CompileUnit CompileUnit
		{
			get { return _unit; }
		}

		public TypeSystem.BooCodeBuilder CodeBuilder
		{
			get { return My<BooCodeBuilder>.Instance; }
		}
		
		public Assembly GeneratedAssembly
		{
			get { return _generatedAssembly; }
			set { _generatedAssembly = value; }
		}

		public string GetUniqueName(params string[] components)
		{
			return My<UniqueNameProvider>.Instance.GetUniqueName(components);
		}

		[Conditional("TRACE")]
		public void TraceEnter(string format, params object[] args)
		{
			if (_parameters.TraceInfo)
			{
				Trace.WriteLine(string.Format(format, args));
				++Trace.IndentLevel;
			}
		}
		
		[Conditional("TRACE")]
		public void TraceLeave(string format, params object[] args)
		{
			if (_parameters.TraceInfo)
			{
				--Trace.IndentLevel;
				Trace.WriteLine(string.Format(format, args));
			}
		}
		
		[Conditional("TRACE")]
		public void TraceInfo(string format, params object[] args)
		{			
			if (_parameters.TraceInfo)
			{
				Trace.WriteLine(string.Format(format, args));
			}			
		}
		
		[Conditional("TRACE")]
		public void TraceInfo(string message)
		{
			if (_parameters.TraceInfo)
			{
				Trace.WriteLine(message);
			}
		}
		
		[Conditional("TRACE")]
		public void TraceWarning(string message)
		{
			if (_parameters.TraceWarning)
			{
				Trace.WriteLine(message);
			}
		}

		[Conditional("TRACE")]
		public void TraceWarning(string message, params object[] args)
		{
			if (_parameters.TraceWarning)
			{
				Trace.WriteLine(string.Format(message, args));
			}
		}
		
		[Conditional("TRACE")]
		public void TraceVerbose(string format, params object[] args)
		{
			if (_parameters.TraceVerbose)
			{
				Trace.WriteLine(string.Format(format, args));
			}			
		}
		
		[Conditional("TRACE")]
		public void TraceVerbose(string format, object param1, object param2)
		{
			if (_parameters.TraceVerbose)
			{
				Trace.WriteLine(string.Format(format, param1, param2));
			}
		}
		
		[Conditional("TRACE")]
		public void TraceVerbose(string format, object param1, object param2, object param3)
		{
			if (_parameters.TraceVerbose)
			{
				Trace.WriteLine(string.Format(format, param1, param2, param3));
			}
		}
		
		[Conditional("TRACE")]
		public void TraceVerbose(string format, object param)
		{
			if (_parameters.TraceVerbose)
			{
				Trace.WriteLine(string.Format(format, param));
			}
		}
		
		[Conditional("TRACE")]
		public void TraceVerbose(string message)
		{
			if (_parameters.TraceVerbose)
			{
				Trace.WriteLine(message);
			}
		}	
		
		[Conditional("TRACE")]
		public void TraceError(string message, params object[] args)
		{
			if (_parameters.TraceError)
			{
				Trace.WriteLine(string.Format(message, args));
			}
		}
		
		[Conditional("TRACE")]
		public void TraceError(Exception x)
		{
			if (_parameters.TraceError)
			{
				Trace.WriteLine(x);
			}
		}

		private readonly CachingEnvironment _environment;

		///<summary>Registers a (new) compiler service.</summary>
		///<param name="T">The Type of the service to register. It must be a reference type.</param>
		///<param name="service">An instance of the service.</param>
		///<exception cref="ArgumentException">Thrown when <paramref name="T"/> is already registered.</exception>
		///<exception cref="ArgumentNullException">Thrown when <paramref name="service"/> is null.</exception>
		///<remarks>Services are unregistered (and potentially disposed) when a pipeline has been ran.</remarks>
		public void RegisterService<T>(T service) where T : class
		{
			if (null == service)
				throw new ArgumentNullException("service");

			AddService(typeof(T), service);
		}

		private void AddService(Type serviceType, object service)
		{
			_environment.Add(serviceType, service);
		}

		private void InitializeService(object service)
		{
			var component = service as ICompilerComponent;
			if (null == component)
				return;
			component.Initialize(this);
		}

		///<summary>Gets a registered compiler service of a specific Type or registers a new instance of Type if not yet registered.</summary>
		///<param name="T">The type of the requested service.</param>
		///<returns>Returns the requested service instance.</returns>
		///<exception cref="ArgumentException">Thrown when requested service of type <paramref name="T"/> has not been found.</exception>
		public T Provide<T>() where T : class
		{
			return _environment.Provide<T>();
		}

		void OnCompilerWarning(object o, CompilerWarningEventArgs args)
		{
			CompilerWarning warning = args.Warning;
			if (Parameters.NoWarn || Parameters.DisabledWarnings.Contains(warning.Code))
				args.Cancel();
			if (Parameters.WarnAsError || Parameters.WarningsAsErrors.Contains(warning.Code))
			{
				Errors.Add(new CompilerError(warning.Code, warning.LexicalInfo, warning.Message, null));
				args.Cancel();
			}
		}
	}
}

