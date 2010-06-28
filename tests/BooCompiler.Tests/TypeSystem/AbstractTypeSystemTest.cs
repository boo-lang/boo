using Boo.Lang.Compiler;
using Boo.Lang.Compiler.TypeSystem;
using Boo.Lang.Compiler.TypeSystem.Builders;

namespace BooCompiler.Tests.TypeSystem
{
	using NUnit.Framework;

	public class AbstractTypeSystemTest
	{
		protected CompilerContext Context;

		protected BooCodeBuilder CodeBuilder
		{
			get { return Context.CodeBuilder;  }
		}

		[SetUp]
		public virtual void SetUp()
		{
			Context = new CompilerContext(false);
		}

		protected IType DefineInternalClass(string @namespace, string typeName)
		{
			return BuildInternalClass(@namespace, typeName).Entity;
		}

		protected BooClassBuilder BuildInternalClass(string @namespace, string typeName)
		{
			var classBuilder = CodeBuilder.CreateClass(typeName);
			var classModule = CodeBuilder.CreateModule(typeName + "Module", @namespace);
			classModule.Members.Add(classBuilder.ClassDefinition);
			Context.CompileUnit.Modules.Add(classModule);
			return classBuilder;
		}
	}
}
