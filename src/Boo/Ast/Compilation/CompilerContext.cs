using System;
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
		
		protected NameBinding.BindingManager _bindingManager;

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
			_bindingManager = new NameBinding.BindingManager();
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
		
		public NameBinding.BindingManager BindingManager
		{
			get
			{
				return _bindingManager;
			}
		}

		public List ResolveExternalType(List target, string name)
		{
			return ResolveExternalType(target, name, null);
		}

		public List ResolveExternalType(List target, string name, Predicate condition)
		{
			InnerResolveExternalType(target, name, condition);
			InnerResolveExternalType(target, "Boo.Lang." + name, condition);
			return target;
		}

		public List ResolveExternalType(string name, UsingCollection imports, Predicate condition)
		{			
			return ResolveExternalType(new List(), name, imports, condition);
		}

		public List ResolveExternalType(List target, string name, UsingCollection imports, Predicate condition)
		{
			if (null != imports)
			{
				foreach (Using import in imports)
				{
					InnerResolveExternalType(target, import.Namespace + "." + name, condition);
				}
			}

			// Tenta resolver o nome sem qualquer qualificao
			return ResolveExternalType(target, name, condition);
		}

		internal void Run()
		{
			_parameters.Pipeline.Run(this);
		}

		void InnerResolveExternalType(List target, string name, Predicate condition)
		{
			foreach (Assembly asm in _assemblyReferences)
			{
				Type type = asm.GetType(name, false, false);
				if (null != type)
				{
					if (null == condition || condition(type))
					{
						target.AddUnique(type);
					}
				}
			}
		}
	}
}
