#region license
// boo - an extensible programming language for the CLI
// Copyright (C) 2004 Rodrigo B. de Oliveira
//
// Permission is hereby granted, free of charge, to any person 
// obtaining a copy of this software and associated documentation 
// files (the "Software"), to deal in the Software without restriction, 
// including without limitation the rights to use, copy, modify, merge, 
// publish, distribute, sublicense, and/or sell copies of the Software, 
// and to permit persons to whom the Software is furnished to do so, 
// subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included 
// in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, 
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF 
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. 
// IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY 
// CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, 
// TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE 
// OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// 
// Contact Information
//
// mailto:rbo@acm.org
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
	public class CompilerContext : System.MarshalByRefObject
	{				
		protected CompilerParameters _parameters;

		protected CompileUnit _unit;

		protected AssemblyCollection _assemblyReferences;

		protected CompilerErrorCollection _errors;
		
		protected Services.TaxonomyManager _bindingService;		
		
		protected INameResolutionService _nameResolutionService;
		
		protected TraceSwitch _traceSwitch;

		protected int _localIndex;
		
		protected System.Reflection.MethodInfo _generatedEntryPoint;
		
		protected Assembly _generatedAssembly;
		
		protected Hash _properties;

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
			_bindingService = new Services.TaxonomyManager();
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
		
		public Services.TaxonomyManager TaxonomyHelper
		{
			get
			{
				return _bindingService;
			}
		}		
		
		public INameResolutionService NameResolutionService
		{
			get
			{
				return _nameResolutionService;
			}
			
			set
			{
				_nameResolutionService = value;
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
