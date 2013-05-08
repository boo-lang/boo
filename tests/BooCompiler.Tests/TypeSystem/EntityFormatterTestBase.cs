using Boo.Lang.Compiler.TypeSystem;
using Boo.Lang.Compiler.TypeSystem.Services;
using Boo.Lang.Environments;
using Moq;
using NUnit.Framework;

namespace BooCompiler.Tests.TypeSystem
{
	public abstract class EntityFormatterTestBase : AbstractTypeSystemTest
	{
		[Test]
		public void SimpleTypeDisplayNameGoesThroughEntityFormatter()
		{
			AssertDisplayNameGoesThroughEntityFormatter(SimpleType());
		}

		[Test]
		public void ArrayTypeDisplayNameGoesThroughEntityFormatter()
		{
			AssertDisplayNameGoesThroughEntityFormatter(ArrayType());
		}

		[Test]
		public void CallableTypeDisplayNameGoesThroughEntityFormatter()
		{
			AssertDisplayNameGoesThroughEntityFormatter(CallableType());
		}

		[Test]
		public void GenericTypeDisplayNameGoesThroughEntityFormatter()
		{
			AssertDisplayNameGoesThroughEntityFormatter(GenericType());
		}

		protected abstract IType SimpleType();
		protected abstract IType CallableType();
		protected abstract IType GenericType();
		protected abstract IType ArrayType();

		protected static void AssertDisplayNameGoesThroughEntityFormatter(IType entity)
		{
			var mock = new Mock<EntityFormatter>();
			ActiveEnvironment.With(new ClosedEnvironment(mock.Object), () => {
         		mock.Setup(formatter => formatter.FormatType(entity))
         			.Returns("")
         			.AtMostOnce();

         		entity.DisplayName();

         		mock.VerifyAll();
         	});
		}
	}
}
