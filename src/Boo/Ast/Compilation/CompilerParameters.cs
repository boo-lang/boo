using System;
using System.Collections;
using System.IO;

namespace Boo.Ast.Compilation
{
	/// <summary>
	/// Compiler parameters.
	/// </summary>
	public class CompilerParameters
	{
		Pipeline _pipeline;

		CompilerInputCollection _input;

		AssemblyCollection _assemblyReferences;

		bool _verbose;

		int _maxAttributeSteps;
		
		string _outputAssembly;
		
		CompilerOutputType _outputType;

		public CompilerParameters()
		{
			_pipeline = new Pipeline();
			_input = new CompilerInputCollection();
			_assemblyReferences = new AssemblyCollection();
			_assemblyReferences.Add(GetType().Assembly);
			_assemblyReferences.Add(typeof(string).Assembly);
			_verbose = false;
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

		public bool Verbose
		{
			get
			{
				return _verbose;
			}

			set
			{
				_verbose = value;
			}
		}

		public CompilerInputCollection Input
		{
			get
			{
				return _input;
			}
		}

		public AssemblyCollection References
		{
			get
			{
				return _assemblyReferences;
			}
		}

		public Pipeline Pipeline
		{
			get
			{
				return _pipeline;
			}
		}
		
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
					throw new ArgumentNullException("value");
				}
				if (0 == value.Length)
				{
					throw new ArgumentException("value");
				}
				_outputAssembly = value;
			}
		}
		
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
