using System;
using System.Reflection;
using System.Reflection.Emit;
using Boo.Ast;
using Boo.Ast.Compilation;
using Boo.Ast.Compilation.Binding;

namespace Boo.Ast.Compilation.Steps
{
	public abstract class AbstractTransformerCompilerStep : Boo.Ast.DepthFirstTransformer, ICompilerStep
	{
		protected CompilerContext _context;
		
		protected AbstractTransformerCompilerStep()
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
		
		protected Binding.BindingManager BindingManager
		{
			get
			{
				return _context.BindingManager;
			}
		}
		
		public IBinding GetBinding(Node node)
		{
			return BindingManager.GetBinding(node);
		}
		
		public ITypeBinding GetTypeBinding(Node node)
		{
			return BindingManager.GetTypeBinding(node);
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
