using System;
using System.Reflection;
using System.Reflection.Emit;
using Boo.Ast;
using Boo.Ast.Compilation;
using Boo.Ast.Compilation.Binding;

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
		
		public TypeBuilder GetTypeBuilder(TypeDefinition type)
		{
			return ((InternalTypeBinding)GetBinding(type)).TypeBuilder;
		}
		
		public MethodBuilder GetMethodBuilder(Method method)
		{
			return ((InternalMethodBinding)GetBinding(method)).MethodBuilder;
		}
		
		public MethodInfo GetMethodInfo(Node node)
		{
			return (MethodInfo)((IMethodBinding)GetBinding(node)).MethodInfo;
		}
		
		public ITypeBinding GetTypeBinding(Node node)
		{
			return BindingManager.GetTypeBinding(node);
		}
		
		public System.Type GetBoundType(Node node)
		{
			return BindingManager.GetBoundType(node);
		}		
		
		public LocalBinding GetLocalBinding(Node local)
		{
			return (LocalBinding)GetBinding(local);
		}
		
		public LocalBuilder GetLocalBuilder(Node local)
		{
			return GetLocalBinding(local).LocalBuilder;
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
