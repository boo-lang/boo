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
using System.Collections;
using System.IO;

namespace Boo.Lang.Compiler
{
	/// <summary>
	/// Compiler parameters.
	/// </summary>
	public class CompilerParameters : System.MarshalByRefObject
	{
		TextWriter _outputWriter;
		
		CompilerPipeline _pipeline;

		CompilerInputCollection _input;
		
		CompilerResourceCollection _resources;

		AssemblyCollection _assemblyReferences;

		int _maxAttributeSteps;
		
		string _outputAssembly;
		
		CompilerOutputType _outputType;
		
		bool _debug;
		
		public readonly TraceSwitch TraceSwitch = new TraceSwitch("booc", "boo compiler");

		public CompilerParameters()
		{
			_pipeline = null;
			_input = new CompilerInputCollection();
			_resources = new CompilerResourceCollection();
			_assemblyReferences = new AssemblyCollection();
			_assemblyReferences.Add(typeof(Boo.Lang.Builtins).Assembly);
			_assemblyReferences.Add(GetType().Assembly);
			_assemblyReferences.Add(typeof(object).Assembly); // corlib
			_assemblyReferences.Add(System.Reflection.Assembly.LoadWithPartialName("System")); // System
			_maxAttributeSteps = 2;
			_outputAssembly = string.Empty;
			_outputType = CompilerOutputType.ConsoleApplication;
			_outputWriter = System.Console.Out;
			_debug = true;
		}

		/// <summary>
		/// Max number of steps for the resolution of AST attributes.		
		/// </summary>
		public int MaxAttributeSteps
		{
			get
			{
				return _maxAttributeSteps;
			}

			set
			{
				_maxAttributeSteps = value;
			}
		}

		public CompilerInputCollection Input
		{
			get
			{
				return _input;
			}
		}
		
		public CompilerResourceCollection Resources
		{
			get
			{
				return _resources;
			}
		}

		public AssemblyCollection References
		{
			get
			{
				return _assemblyReferences;
			}
		}

		/// <summary>
		/// The compilation pipeline.
		/// </summary>
		public CompilerPipeline Pipeline
		{
			get
			{
				return _pipeline;
			}
			
			set
			{
				_pipeline = value;
			}
		}
		
		/// <summary>
		/// The name (full or partial) for the file
		/// that should receive the resulting assembly.
		/// </summary>
		public string OutputAssembly
		{
			get
			{
				return _outputAssembly;
			}
			
			set
			{
				if (null == value)
				{
					throw new ArgumentNullException("OutputAssembly");
				}
				if (0 == value.Length)
				{
					throw new ArgumentException("OutputAssembly");
				}
				_outputAssembly = value;
			}
		}
		
		/// <summary>
		/// Type and execution subsystem for the generated portable
		/// executable file.
		/// </summary>
		public CompilerOutputType OutputType
		{
			get
			{
				return _outputType;
			}
			
			set
			{
				_outputType = value;
			}
		}
		
		public TextWriter OutputWriter
		{
			get
			{
				return _outputWriter;
			}
			
			set
			{
				if (null == value)
				{
					throw new ArgumentNullException("OutputWriter");
				}
				_outputWriter = value;
			}
		}
		
		public bool Debug
		{
			get
			{
				return _debug;
			}
			
			set
			{
				_debug = value;
			}
		}
	}
}
