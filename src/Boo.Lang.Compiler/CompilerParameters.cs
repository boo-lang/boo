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
using System.Collections;
using System.IO;

namespace Boo.Lang.Compiler
{
	/// <summary>
	/// Compiler parameters.
	/// </summary>
	public class CompilerParameters : System.MarshalByRefObject
	{
		CompilerPipeline _pipeline;

		CompilerInputCollection _input;
		
		CompilerResourceCollection _resources;

		AssemblyCollection _assemblyReferences;

		int _maxAttributeSteps;
		
		string _outputAssembly;
		
		CompilerOutputType _outputType;
		
		public readonly TraceSwitch TraceSwitch = new TraceSwitch("booc", "boo compiler");

		public CompilerParameters()
		{
			_pipeline = new CompilerPipeline();
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
	}
}
