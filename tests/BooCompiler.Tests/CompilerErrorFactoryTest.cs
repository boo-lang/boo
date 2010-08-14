using Boo.Lang.Compiler;
using Boo.Lang.Compiler.Ast;
using Boo.Lang.Compiler.Services;
using Boo.Lang.Environments;
using Moq;
using NUnit.Framework;

namespace BooCompiler.Tests
{
	[TestFixture]
	public class CompilerErrorFactoryTest
	{
		[Test]
		public void DefaultGeneratorTypeRepresentationComesFromLanguageAmbience()
		{
			var mock = new Mock<LanguageAmbiance>();
			ActiveEnvironment.With(new ClosedEnvironment(mock.Object), () =>
			{
				mock.Setup(ambience => ambience.DefaultGeneratorTypeFor("string"))
					.Returns("string*")
					.AtMostOnce();

				CompilerErrorFactory.InvalidGeneratorReturnType(new SimpleTypeReference(), "string");

				mock.VerifyAll();
			});
		}
	}
}
