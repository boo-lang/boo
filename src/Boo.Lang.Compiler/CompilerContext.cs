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
	public class CompilerContext
	{				
		protected CompilerParameters _parameters;

		protected CompileUnit _unit;

		protected AssemblyCollection _assemblyReferences;

		protected CompilerErrorCollection _errors;
		
		protected CompilerWarningCollection _warnings;
		
		protected TypeSystem.TypeSystemServices _typeSystemServices;
		
		protected readonly TypeSystem.NameResolutionService _nameResolutionService;
		
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
		
		public CompilerContext(CompilerParameters options, CompileUnit unit)
		{
			if (null == options) throw new ArgumentNullException("options");
			if (null == unit) throw new ArgumentNullException("unit");

			_unit = unit;
			_errors = new CompilerErrorCollection();			
			_warnings = new CompilerWarningCollection();
			_assemblyReferences = options.References;
			_parameters = options;
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
		
		public string GeneratedAssemblyFileName
		{
			get
			{
				return _generatedAssemblyFileName;
			}
			
			set
			{
				if (null == value || 0 == value.Length)
				{
					throw new ArgumentException("GeneratedAssemblyFileName");
				}
				_generatedAssemblyFileName = value;
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
		
		public CompilerWarningCollection Warnings
		{
			get
			{
				return _warnings;
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
			
			set
			{
				if (null == value)
				{
					throw new ArgumentNullException("TypeSystemServices");
				}
				_typeSystemServices = value;
			}
		}		
		
		public TypeSystem.BooCodeBuilder CodeBuilder
		{
			get
			{
				return _typeSystemServices.CodeBuilder;
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

		public int AllocIndex()
		{
			return ++_localIndex;
		}
		
		[Conditional("TRACE")]
		public void TraceEnter(string format, object param)
		{
			if (_traceSwitch.TraceInfo)
			{
				Trace.WriteLine(string.Format(format, param));
				++Trace.IndentLevel;
			}
		}
		
		[Conditional("TRACE")]
		public void TraceLeave(string format, object param)
		{
			if (_traceSwitch.TraceInfo)
			{
				--Trace.IndentLevel;
				Trace.WriteLine(string.Format(format, param));
			}
		}
		
		[Conditional("TRACE")]
		public void TraceInfo(string format, params object[] args)
		{			
			if (_traceSwitch.TraceInfo)
			{
				Trace.WriteLine(string.Format(format, args));
			}			
		}
		
		[Conditional("TRACE")]
		public void TraceInfo(string message)
		{
			if (_traceSwitch.TraceInfo)
			{
				Trace.WriteLine(message);
			}
		}
		
		[Conditional("TRACE")]
		public void TraceWarning(string message)
		{
			if (_traceSwitch.TraceWarning)
			{
				Trace.WriteLine(message);
			}
		}

		[Conditional("TRACE")]
		public void TraceWarning(string message, params object[] args)
		{
			if (_traceSwitch.TraceWarning)
			{
				Trace.WriteLine(string.Format(message, args));
			}
		}
		
		[Conditional("TRACE")]
		public void TraceVerbose(string format, params object[] args)
		{			
			if (_traceSwitch.TraceVerbose)
			{
				Trace.WriteLine(string.Format(format, args));
			}			
		}
		
		[Conditional("TRACE")]
		public void TraceVerbose(string format, object param1, object param2)
		{
			if (_traceSwitch.TraceVerbose)
			{
				Trace.WriteLine(string.Format(format, param1, param2));
			}
		}
		
		[Conditional("TRACE")]
		public void TraceVerbose(string format, object param1, object param2, object param3)
		{
			if (_traceSwitch.TraceVerbose)
			{
				Trace.WriteLine(string.Format(format, param1, param2, param3));
			}
		}
		
		[Conditional("TRACE")]
		public void TraceVerbose(string format, object param)
		{
			if (_traceSwitch.TraceVerbose)			
			{
				Trace.WriteLine(string.Format(format, param));
			}
		}
		
		[Conditional("TRACE")]
		public void TraceVerbose(string message)
		{
			if (_traceSwitch.TraceVerbose)
			{
				Trace.WriteLine(message);
			}
		}	
		
		[Conditional("TRACE")]
		public void TraceError(string message, params object[] args)
		{
			if (_traceSwitch.TraceError)
			{
				Trace.WriteLine(string.Format(message, args));
			}
		}
		
		[Conditional("TRACE")]
		public void TraceError(Exception x)
		{
			if (_traceSwitch.TraceError)
			{
				Trace.WriteLine(x);
			}
		}
	}
}
