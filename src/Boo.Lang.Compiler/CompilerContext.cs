#region license
// boo - an extensible programming language for the CLI
// Copyright (C) 2004 Rodrigo Barreto de Oliveira
//
// This library is free software; you can redistribute it and/or
// modify it under the terms of the GNU Lesser General Public
// License as published by the Free Software Foundation; either
// version 2.1 of the License, or (at your option) any later version.
// 
// This library is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
// Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public
// License along with this library; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA 02111-1307 USA
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
		
		protected TypeSystem.TypeSystemServices _typeSystemServices;		
		
		protected TypeSystem.NameResolutionService _nameResolutionService;
		
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
