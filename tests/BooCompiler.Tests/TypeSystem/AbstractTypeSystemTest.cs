using System;
using Boo.Lang.Compiler;
using Boo.Lang.Compiler.TypeSystem;
using Boo.Lang.Compiler.TypeSystem.Builders;
using Boo.Lang.Environments;

namespace BooCompiler.Tests.TypeSystem
{
	using NUnit.Framework;

	public class AbstractTypeSystemTest
	{
		protected CompilerContext Context;

		protected IEnvironment Environment
		{
			get { return Context.Environment;  }
		}

		protected BooCodeBuilder CodeBuilder
		{
			get { return Context.CodeBuilder;  }
		}

		protected static TypeSystemServices TypeSystemServices
		{
			get { return My<TypeSystemServices>.Instance; }
		}

		[SetUp]
		public virtual void SetUp()
		{
			Context = new CompilerContext(false);
		}

		protected void RunInCompilerContextEnvironment(System.Action action)
		{
			Environment.Run(action);
		}

		protected T InvokeInCompilerContextEnvironment<T>(System.Func<T> function)
		{
			return Environment.Invoke(function);
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
