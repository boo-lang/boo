using Boo.Lang.Compiler;
using Boo.Lang.Compiler.Ast;
using Boo.Lang.Compiler.TypeSystem;
using Boo.Lang.Compiler.TypeSystem.Builders;

namespace BooCompiler.Tests.TypeSystem
{
	using NUnit.Framework;

	public class AbstractTypeSystemTest
	{
		protected CompilerContext context;

		protected BooCodeBuilder CodeBuilder
		{
			get { return context.CodeBuilder;  }
		}

		[SetUp]
		public virtual void SetUp()
		{
			context = new CompilerContext(false);
		}

		protected IType DefineInternalClass(string @namespace, string typeName)
		{
			BooClassBuilder classBuilder = CodeBuilder.CreateClass(typeName);
			Module classModule = CodeBuilder.CreateModule(typeName + "Module", @namespace);
			classModule.Members.Add(classBuilder.ClassDefinition);
			context.CompileUnit.Modules.Add(classModule);
			return classBuilder.Entity;
		}
	}
}
