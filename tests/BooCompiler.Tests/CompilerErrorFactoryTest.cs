using Boo.Lang.Compiler;
using Boo.Lang.Compiler.Ast;
using Boo.Lang.Compiler.Services;
using Boo.Lang.Compiler.TypeSystem;
using Boo.Lang.Compiler.TypeSystem.Services;
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
			var languageAmbianceMock = new Mock<LanguageAmbiance>();
			var formatterMock = new Mock<EntityFormatter>();
			ActiveEnvironment.With(new ClosedEnvironment(languageAmbianceMock.Object, formatterMock.Object), () =>
			{
				var type = new Mock<IType>(MockBehavior.Strict).Object;

				formatterMock.Setup(f => f.FormatType(type))
					.Returns("string");

				languageAmbianceMock.Setup(ambience => ambience.DefaultGeneratorTypeFor("string"))
					.Returns("string*")
					.AtMostOnce();

				CompilerErrorFactory.InvalidGeneratorReturnType(new SimpleTypeReference(), type);

				languageAmbianceMock.VerifyAll();
			});
		}
	}
}
