using System;
using System.Diagnostics;
using Boo.Lang;
using Boo.Ast;
using Assembly = System.Reflection.Assembly;

namespace Boo.Ast.Compilation
{
	/// <summary>
	/// Contexto de compilao boo.
	/// </summary>
	public class CompilerContext
	{				
		protected CompilerParameters _parameters;

		protected CompileUnit _unit;

		protected AssemblyCollection _assemblyReferences;

		protected ErrorCollection _errors;
		
		protected Binding.BindingManager _bindingManager;		
		
		protected TraceSwitch _traceSwitch;

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
			_errors = new ErrorCollection();
			_assemblyReferences = options.References;
			_parameters = options;
			_bindingManager = new Binding.BindingManager();
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

		public ErrorCollection Errors
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
		
		public Binding.BindingManager BindingManager
		{
			get
			{
				return _bindingManager;
			}
		}	
		
		public void TraceVerbose(string format, params object[] args)
		{			
			if (_traceSwitch.TraceVerbose)
			{
				Trace.WriteLine(string.Format(format, args));
			}
		}

		internal void Run()
		{
			_parameters.Pipeline.Run(this);
		}	
	}
}
