using System;
using Boo.Ast;
using Boo.Ast.Compilation;

namespace Boo.Ast.Compilation.Steps
{
	public abstract class AbstractCompilerStep : Boo.Ast.DepthFirstSwitcher, ICompilerStep
	{
		protected CompilerContext _context;
		
		protected AbstractCompilerStep()
		{			
		}
		
		protected CompilerContext CompilerContext
		{
			get
			{
				return _context;
			}
		}
		
		protected CompileUnit CompileUnit
		{
			get
			{
				return _context.CompileUnit;
			}
		}
		
		protected CompilerParameters CompilerParameters
		{
			get
			{
				return _context.CompilerParameters;
			}
		}
		
		protected ErrorCollection Errors
		{
			get
			{
				return _context.Errors;
			}
		}
		
		protected NameBinding.BindingManager BindingManager
		{
			get
			{
				return _context.BindingManager;
			}
		}
		
		public virtual void Initialize(CompilerContext context)
		{
			if (null == context)
			{
				throw new ArgumentNullException("context");
			}
			_context = context;
		}
		
		public abstract void Run();
		
		public virtual void Dispose()
		{
			_context = null;
		}
	}
}
