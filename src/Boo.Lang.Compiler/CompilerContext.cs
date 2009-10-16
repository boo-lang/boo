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
using Boo.Lang.Compiler.TypeSystem.Reflection;
using Boo.Lang.Compiler.TypeSystem.Services;
using Assembly = System.Reflection.Assembly;
using System.Collections.Generic;
using Boo.Lang.Compiler.TypeSystem;

namespace Boo.Lang.Compiler
{
	/// <summary>
	/// boo compilation context.
	/// </summary>
	public class CompilerContext
	{
		public static CompilerContext Current
		{
			get { return _current != null ? _current.Value : null; }
		}

		[ThreadStatic] private static DynamicVariable<CompilerContext> _current;

		protected CompilerParameters _parameters;

		protected CompileUnit _unit;

		protected CompilerReferenceCollection _references;

		protected CompilerErrorCollection _errors;
		
		protected CompilerWarningCollection _warnings;
		
		protected TraceSwitch _traceSwitch;

		protected int _localIndex;
		
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

			// FIXME: temporary hack to make sure the singleton is visible
			// using the My<IReflectionTypeSystemProvider> idiom
			RegisterService<IReflectionTypeSystemProvider>(_references.Provider);
			RegisterService<CompilerErrorCollection>(_errors);
			RegisterService<CompilerWarningCollection>(_warnings);
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

		public TypeSystemServices TypeSystemServices
		{
			get { return Produce<TypeSystemServices>(); }
			set { RegisterService<TypeSystemServices>(value); }
		}

		public NameResolutionService NameResolutionService
		{
			get { return Produce<NameResolutionService>(); }
		}

		public TypeSystem.BooCodeBuilder CodeBuilder
		{
			get { return Produce<BooCodeBuilder>(); }
		}
		
		public Assembly GeneratedAssembly
		{
			get { return _generatedAssembly; }
			
			set { _generatedAssembly = value; }
		}

		[Obsolete("AllocIndex is obsolete, use GetUniqueName instead")]
		public int AllocIndex()
		{
			return ++_localIndex;
		}

		///<summary>Generates a name that will be unique within the CompilerContext.</summary>
		///<param name="components">Zero or more string(s) that will compose the generated name.</param>
		///<returns>Returns the generated unique name.</returns>
		public string GetUniqueName(params string[] components)
		{
			int len = 0;
			if (null != components)
				len = components.Length;

			//ignore obsolete warning  TODO: remove when AllocIndex is private
			#pragma warning disable 618
			string index = string.Concat("$", AllocIndex().ToString());
			#pragma warning restore 618

			if (0 == len)
				return index;

			System.Text.StringBuilder sb = new System.Text.StringBuilder();
			foreach (string component in components)
			{
				sb.Append("$");
				sb.Append(component);
			}
			sb.Append(index);
			return sb.ToString();
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

		/// <summary>
		/// Runs the given action with this context ensuring CompilerContext.Current
		/// returns the right context.
		/// </summary>
		/// <param name="action"></param>
		public void Run(System.Action<CompilerContext> action)
		{
			CurrentVariable().With(this, action);
		}

		private static DynamicVariable<CompilerContext> CurrentVariable()
		{
			if (null == _current) _current = new DynamicVariable<CompilerContext>();
			return _current;
		}

		#region Compiler services registry
		protected IDictionary<Type, object> _services = new Dictionary<Type, object>();

		///<summary>Registers a (new) compiler service.</summary>
		///<param name="T">The Type of the service to register. It must be a reference type.</param>
		///<param name="service">An instance of the service.</param>
		///<exception cref="ArgumentException">Thrown when <paramref name="T"/> is already registered.</exception>
		///<exception cref="ArgumentNullException">Thrown when <paramref name="service"/> is null.</exception>
		///<remarks>Services are unregistered (and potentially disposed) when a pipeline has been ran.</remarks>
		public void RegisterService<T>(T service) where T : class
		{
			RegisterService(typeof(T), service);
		}

		private void RegisterService(Type serviceType, object service)
		{
			if (null == service)
				throw new ArgumentNullException("service");

			AddService(serviceType, service);
			InitializeService(serviceType, service);
		}

		private void AddService(Type serviceType, object service)
		{
			try
			{	
				_services.Add(serviceType, service);
			}
			catch (KeyNotFoundException)
			{
				throw new ArgumentException(string.Format("Compiler service of type `{0}` is already registered", serviceType), "T");
			}
		}

		private void InitializeService(Type serviceType, object service)
		{
			ICompilerComponent component = service as ICompilerComponent;
			if (null == component)
				return;

			try
			{
				component.Initialize(this);
			}
			catch (Exception)
			{
				_services.Remove(serviceType);
				throw;
			}
		}

		///<summary>Unregisters a compiler service.</summary>
		///<param name="T">The type of the service to unregister.</param>
		///<returns>Returns true if the service is successfuly found and removed, false otherwise.</returns>
		///<remarks>If service implements IDisposable, the service is disposed.</remarks>
		public bool UnregisterService<T>() where T : class
		{
			return UnregisterService(typeof(T));
		}

		internal bool UnregisterService(Type type)
		{
			object service = null;
			if (_services.TryGetValue(type, out service))
			{
				IDisposable d = service as IDisposable;
				if (null != d)
					d.Dispose();
			}
			return _services.Remove(type);
		}

		///<summary>Gets a registered compiler service of a specific Type.</summary>
		///<param name="T">The type of the requested service.</param>
		///<returns>Returns the requested service instance.</returns>
		///<exception cref="ArgumentException">Thrown when requested service of type <paramref name="T"/> has not been found.</exception>
		public T GetService<T>() where T : class
		{
			try
			{
				return (T)_services[typeof(T)];
			}
			catch (KeyNotFoundException)
			{
				throw new ArgumentException(string.Format("No compiler service of type `{0}` has been found", typeof(T)), "T");
			}
		}

		///<summary>Gets a registered compiler service of a specific Type or registers a new instance of Type if not yet registered.</summary>
		///<param name="T">The type of the requested service.</param>
		///<returns>Returns the requested service instance.</returns>
		///<exception cref="ArgumentException">Thrown when requested service of type <paramref name="T"/> has not been found.</exception>
		public T Produce<T>() where T : class
		{	
			object existing;
			if (_services.TryGetValue(typeof(T), out existing))
				return (T)existing;
			T newService = Activator.CreateInstance<T>();
			RegisterService(typeof(T), newService);
			return newService;
		}

		///<summary>Gets currently registered compiler services.</summary>
		///<returns>Returns an enumerable of available services types.</returns>
		public IEnumerable<Type> RegisteredServices
		{
			get
			{
				Type[] keys = new Type[_services.Keys.Count];
				_services.Keys.CopyTo(keys, 0);
				return keys;
			}
		}
		#endregion


		void OnCompilerWarning(object o, CompilerWarningEventArgs args)
		{
			CompilerWarning warning = args.Warning;
			if (Parameters.NoWarn || Parameters.DisabledWarnings.Contains(warning.Code))
				args.Cancel();
			if (Parameters.WarnAsError || Parameters.WarningsAsErrors.Contains(warning.Code)) {
				Errors.Add(new CompilerError(warning.Code, warning.LexicalInfo, warning.Message, null));
				args.Cancel();
			}
		}
	}
}

