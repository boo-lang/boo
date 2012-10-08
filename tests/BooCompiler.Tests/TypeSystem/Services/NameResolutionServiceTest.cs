using System;
using Boo.Lang.Compiler;
using Boo.Lang.Compiler.IO;
using Boo.Lang.Compiler.Pipelines;
using Boo.Lang.Compiler.Steps;
using Boo.Lang.Compiler.TypeSystem;
using Boo.Lang.Compiler.TypeSystem.Core;
using Boo.Lang.Compiler.TypeSystem.Services;
using Boo.Lang.Environments;
using NUnit.Framework;

namespace BooCompiler.Tests.TypeSystem.Services
{
	[TestFixture]
	public class NameResolutionServiceTest
	{
		private static IEntity ResolveQualifiedName(string qualifiedName)
		{
			return Subject().ResolveQualifiedName(qualifiedName);
		}

		private static NameResolutionService Subject()
		{
			return My<NameResolutionService>.Instance;
		}

		private static bool MatchIgnoringCase(IEntity candidate, string name)
		{
			return 0 == string.Compare(candidate.Name, name, true);
		}

		[Test]
		public void NameMatchingCanBeCustomized()
		{
			var parameters = new CompilerParameters();
			const string code = @"
l = []
l.ADD(42)
l.add(42)
print JOIN(l, "", "")
";
			parameters.Input.Add(new StringInput("code", code));
			parameters.Pipeline = new ResolveExpressions();
			parameters.Pipeline.Insert(0, new ActionStep(
			                           	() => My<NameResolutionService>.Instance.EntityNameMatcher = MatchIgnoringCase));
			CompilerContext result = new Boo.Lang.Compiler.BooCompiler(parameters).Run();
			Assert.AreEqual(0, result.Errors.Count, result.Errors.ToString());
		}

		[Test]
		public void ResolveQualifiedName()
		{
			RunInCompilerContextEnvironment(delegate
			{
				Subject().EnterNamespace(My<GlobalNamespace>.Instance);
				IEntity result = ResolveQualifiedName("Boo.Lang");
				Assert.IsNotNull(result);
				Assert.AreEqual(EntityType.Namespace, result.EntityType);

				var builtinsType = ResolveQualifiedName("Boo.Lang.Builtins") as IType;
				Assert.IsNotNull(builtinsType);
			});
		}

		private void RunInCompilerContextEnvironment(Action action)
		{
			ActiveEnvironment.With(new CompilerContext().Environment, () => {
				action();
			});
		}
	}
}
