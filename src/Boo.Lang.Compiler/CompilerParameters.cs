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
