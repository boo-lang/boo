#region license
// boo - an extensible programming language for the CLI
// Copyright (C) 2004 Rodrigo B. de Oliveira
//
// This program is free software; you can redistribute it and/or
// modify it under the terms of the GNU General Public License
// as published by the Free Software Foundation; either version 2
// of the License, or (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.
//
// As a special exception, if you link this library with other files to
// produce an executable, this library does not by itself cause the
// resulting executable to be covered by the GNU General Public License.
// This exception does not however invalidate any other reasons why the
// executable file might be covered by the GNU General Public License.
//
// Contact Information
//
// mailto:rbo@acm.org
#endregion

using System;
using System.Diagnostics;
using Boo.Lang;
using Boo.Lang.Ast;
using Assembly = System.Reflection.Assembly;

namespace Boo.Lang.Compiler
{
	/// <summary>
	/// Contexto de compilao boo.
	/// </summary>
	public class CompilerContext
	{				
		protected CompilerParameters _parameters;

		protected CompileUnit _unit;

		protected AssemblyCollection _assemblyReferences;

		protected CompilerErrorCollection _errors;
		
		protected Bindings.BindingManager _bindingManager;		
		
		protected TraceSwitch _traceSwitch;

		protected int _localIndex;		

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
			_bindingManager = new Bindings.BindingManager();
			_traceSwitch = _parameters.TraceSwitch;
		}	

		public CompilerParameters CompilerParameters
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
		
		public Bindings.BindingManager BindingManager
		{
			get
			{
				return _bindingManager;
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

		internal void Run()
		{			
			_parameters.Pipeline.Run(this);
		}	
	}
}
