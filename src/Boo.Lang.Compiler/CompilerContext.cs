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
using Boo.Lang;
using Boo.Lang.Compiler.Ast;
using Assembly = System.Reflection.Assembly;

namespace Boo.Lang.Compiler
{
	/// <summary>
	/// boo compilation context.
	/// </summary>
	public class CompilerContext// : System.MarshalByRefObject
	{				
		protected CompilerParameters _parameters;

		protected CompileUnit _unit;

		protected AssemblyCollection _assemblyReferences;

		protected CompilerErrorCollection _errors;
		
		protected readonly TypeSystem.TypeSystemServices _typeSystemServices;

		protected readonly TypeSystem.BooCodeBuilder _codeBuilder;		
		
		protected readonly TypeSystem.NameResolutionService _nameResolutionService;
		
		protected TraceSwitch _traceSwitch;

		protected int _localIndex;
		
		protected System.Reflection.MethodInfo _generatedEntryPoint;
		
		protected Assembly _generatedAssembly;
		
		protected Hash _properties;
		
		public CompilerContext() : this(new CompileUnit())
		{
		}

		public CompilerContext(CompileUnit unit) : this(new CompilerParameters(), unit)
		{				
		}

		public CompilerContext(CompilerParameters options, CompileUnit unit)
		{
			if (null == options)
			{
				throw new ArgumentNullException("options");
			}

			if (null == unit)
			{
				throw new ArgumentNullException("unit");
			}

			_unit = unit;
			_errors = new CompilerErrorCollection();
			_assemblyReferences = options.References;
			_parameters = options;
			_typeSystemServices = new TypeSystem.TypeSystemServices(this);
			_codeBuilder = _typeSystemServices.CodeBuilder;
			_nameResolutionService = new TypeSystem.NameResolutionService(this); 
			_traceSwitch = _parameters.TraceSwitch;
			_properties = new Hash();
		}
		
		public Hash Properties
		{
			get
			{
				return _properties;
			}
		}
		
		public object this[object key]
		{
			get
			{
				return _properties[key];
			}
			
			set
			{
				_properties[key] = value;
			}
		}

		public CompilerParameters Parameters
		{
			get
			{
				return _parameters;
			}
		}

		public AssemblyCollection References
		{
			get
			{
				return _assemblyReferences;
			}
		}

		public CompilerErrorCollection Errors
		{
			get
			{
				return _errors;
			}
		}

		public CompileUnit CompileUnit
		{
			get
			{
				return _unit;
			}
		}
		
		public TypeSystem.TypeSystemServices TypeSystemServices
		{
			get
			{
				return _typeSystemServices;
			}
		}		
		
		public TypeSystem.BooCodeBuilder CodeBuilder
		{
			get
			{
				return _codeBuilder;
			}
		}
		
		public TypeSystem.NameResolutionService NameResolutionService
		{
			get
			{
				return _nameResolutionService;
			}
		}
		
		public Assembly GeneratedAssembly
		{
			get
			{
				return _generatedAssembly;
			}
			
			set
			{
				_generatedAssembly = value;
			}
		}
		
		public System.Reflection.MethodInfo GeneratedAssemblyEntryPoint
		{
			get
			{
				return _generatedEntryPoint;
			}
			
			set
			{
				_generatedEntryPoint = value;
			}
		}
		
		public int AllocIndex()
		{
			return ++_localIndex;
		}
		
		public void TraceEnter(string format, object param)
		{
			if (_traceSwitch.TraceInfo)
			{
				Trace.WriteLine(string.Format(format, param));
				++Trace.IndentLevel;
			}
		}
		
		public void TraceLeave(string format, object param)
		{
			if (_traceSwitch.TraceInfo)
			{
				--Trace.IndentLevel;
				Trace.WriteLine(string.Format(format, param));
			}
		}
		
		public void TraceInfo(string format, params object[] args)
		{			
			if (_traceSwitch.TraceInfo)
			{
				Trace.WriteLine(string.Format(format, args));
			}			
		}
		
		public void TraceInfo(string message)
		{
			if (_traceSwitch.TraceInfo)
			{
				Trace.WriteLine(message);
			}
		}
		
		public void TraceWarning(string message)
		{
			if (_traceSwitch.TraceWarning)
			{
				Trace.WriteLine(message);
			}
		}
		
		public void TraceVerbose(string format, params object[] args)
		{			
			if (_traceSwitch.TraceVerbose)
			{
				Trace.WriteLine(string.Format(format, args));
			}			
		}
		
		public void TraceVerbose(string format, object param1, object param2)
		{
			if (_traceSwitch.TraceVerbose)
			{
				Trace.WriteLine(string.Format(format, param1, param2));
			}
		}
		
		public void TraceVerbose(string format, object param1, object param2, object param3)
		{
			if (_traceSwitch.TraceVerbose)
			{
				Trace.WriteLine(string.Format(format, param1, param2, param3));
			}
		}
		
		public void TraceVerbose(string format, object param)
		{
			if (_traceSwitch.TraceVerbose)			
			{
				Trace.WriteLine(string.Format(format, param));
			}
		}
		
		public void TraceVerbose(string message)
		{
			if (_traceSwitch.TraceVerbose)
			{
				Trace.WriteLine(message);
			}
		}	
	}
}
