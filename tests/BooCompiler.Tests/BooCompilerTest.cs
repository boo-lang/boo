using Boo.Lang.Compiler;
using Boo.Lang.Compiler.Steps;
using Boo.Lang.Compiler.TypeSystem.Services;
using Boo.Lang.Environments;
using NUnit.Framework;

namespace BooCompiler.Tests
{
	[TestFixture]
	public class BooCompilerTest
	{
		[Test]
		public void EnvironmentBindingsCanBeCustomizedThroughCompilerParametersEnvironment()
		{
			EntityFormatter actualFormatter = null;
			var expectedFormatter = new EntityFormatter();
			
			var compiler = new Boo.Lang.Compiler.BooCompiler();
			compiler.Parameters.Pipeline = new CompilerPipeline { new ActionStep(() => actualFormatter = My<EntityFormatter>.Instance) };
			compiler.Parameters.Environment = new ClosedEnvironment(expectedFormatter);
			compiler.Run();

			Assert.AreSame(expectedFormatter, actualFormatter);
		}
	}
}
