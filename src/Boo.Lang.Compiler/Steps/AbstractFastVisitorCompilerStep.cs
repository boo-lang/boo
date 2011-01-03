using Boo.Lang.Compiler.Ast;
using Boo.Lang.Compiler.TypeSystem;
using Boo.Lang.Compiler.TypeSystem.Services;
using Boo.Lang.Environments;

namespace Boo.Lang.Compiler.Steps
{
	public class AbstractFastVisitorCompilerStep : FastDepthFirstVisitor, ICompilerStep
	{
		private CompilerContext _context;

		protected CompilerContext Context
		{
			get { return _context; }
		}

		protected CompilerErrorCollection Errors
		{
			get { return _context.Errors; }
		}

		protected CompilerWarningCollection Warnings
		{
			get { return _context.Warnings; }
		}

		protected CompilerParameters Parameters
		{
			get { return _context.Parameters; }
		}

		protected BooCodeBuilder CodeBuilder
		{
			get { return _codeBuilder;  }
		}

		private EnvironmentProvision<BooCodeBuilder> _codeBuilder;

		protected TypeSystemServices TypeSystemServices
		{
			get { return _typeSystemServices; }
		}

		private EnvironmentProvision<TypeSystemServices> _typeSystemServices;

		protected NameResolutionService NameResolutionService
		{
			get { return _nameResolutionService; }
		}

		private EnvironmentProvision<NameResolutionService> _nameResolutionService;

		protected IType GetType(Node node)
		{
			return TypeSystemServices.GetType(node);
		}

		protected void Error(Expression node, CompilerError error)
		{
			Error(node);
			Errors.Add(error);
		}

		protected void Error(CompilerError error)
		{
			Errors.Add(error);
		}

		protected void Error(Expression node)
		{
			node.ExpressionType = TypeSystemServices.ErrorEntity;
		}

		protected void NotImplemented(Node node, string feature)
		{
			throw CompilerErrorFactory.NotImplemented(node, feature);
		}

		protected IType GetEntity(TypeReference node)
		{
			return (IType)TypeSystemServices.GetEntity(node);
		}

		protected IEntity GetEntity(Node node)
		{
			return TypeSystemServices.GetEntity(node);
		}

		protected IMethod GetEntity(Method node)
		{
			return (IMethod)TypeSystemServices.GetEntity(node);
		}

		protected IProperty GetEntity(Property node)
		{
			return (IProperty)TypeSystemServices.GetEntity(node);
		}

		protected virtual IType GetExpressionType(Expression node)
		{
			return TypeSystemServices.GetExpressionType(node);
		}

		protected void BindExpressionType(Expression node, IType type)
		{
			_context.TraceVerbose("{0}: Type of expression '{1}' bound to '{2}'.", node.LexicalInfo, node, type);
			node.ExpressionType = type;
		}

		public virtual void Initialize(CompilerContext context)
		{
			_context = context;
			_codeBuilder = new EnvironmentProvision<BooCodeBuilder>();
			_typeSystemServices = new EnvironmentProvision<TypeSystemServices>();
			_nameResolutionService = new EnvironmentProvision<NameResolutionService>();
		}

		public virtual void Dispose()
		{
			_context = null;
		}

		public virtual void Run()
		{
			CompileUnit.Accept(this);
		}

		protected CompileUnit CompileUnit
		{
			get { return _context.CompileUnit; }
		}
	}
}