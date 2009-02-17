using System;
using Boo.Lang.Compiler;
using Boo.Lang.Compiler.TypeSystem.Services;
using NUnit.Framework;

namespace BooCompiler.Tests
{
	[TestFixture]
	public class MyTest
	{
		private CompilerContext context = new CompilerContext(false);

		[Test]
		[ExpectedException(typeof(InvalidOperationException))]
		public void MyOutsideContext()
		{
			NameResolutionService service = My<NameResolutionService>.Instance;
		}

		[Test]
		public void MyExistingService()
		{
			context.Run(delegate
			{
				Assert.AreSame(context.NameResolutionService, My<NameResolutionService>.Instance);
			});
		}

		public class DummyService
		{
		}

		[Test]
		public void AutomaticServiceRegistration()
		{
			context.Run(delegate
			{
				Assert.IsNotNull(My<DummyService>.Instance);
				Assert.AreSame(My<DummyService>.Instance, My<DummyService>.Instance);
			});
		}
	}
}
