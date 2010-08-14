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
		public void SimpleTypeToStringGoesThroughEntityFormatter()
		{
			AssertToStringGoesThroughEntityFormatter(SimpleType());
		}

		[Test]
		public void ArrayTypeToStringGoesThroughEntityFormatter()
		{
			AssertToStringGoesThroughEntityFormatter(ArrayType());
		}

		[Test]
		public void CallableTypeToStringGoesThroughEntityFormatter()
		{
			AssertToStringGoesThroughEntityFormatter(CallableType());
		}

		[Test]
		public void GenericTypeToStringGoesThroughEntityFormatter()
		{
			AssertToStringGoesThroughEntityFormatter(GenericType());
		}

		protected abstract IType SimpleType();
		protected abstract IType CallableType();
		protected abstract IType GenericType();
		protected abstract IType ArrayType();

		protected static void AssertToStringGoesThroughEntityFormatter(IType entity)
		{
			var mock = new Mock<EntityFormatter>();
			new ClosedEnvironment(mock.Object).Run(() =>
         	{
         		mock.Setup(formatter => formatter.FormatType(entity))
         			.Returns("")
         			.AtMostOnce();

         		entity.ToString();

         		mock.VerifyAll();
         	});
		}
	}
}