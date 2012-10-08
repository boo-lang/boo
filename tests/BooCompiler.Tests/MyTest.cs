using System;
using Boo.Lang.Compiler;
using Boo.Lang.Compiler.TypeSystem;
using Boo.Lang.Compiler.TypeSystem.Services;
using Boo.Lang.Environments;
using NUnit.Framework;

namespace BooCompiler.Tests
{
	[TestFixture]
	public class MyTest
	{  
	    [Test]
		public void MyOutsideContext()
		{
			try
	    	{
	    		NameResolutionService service = My<NameResolutionService>.Instance;
				Assert.Fail();
	    	}
			catch (InvalidOperationException)
	    	{	
	    	}
		}

		[Test]
		public void MyExistingService()
		{
			RunInCompilerContextEnvironment(() => Assert.AreSame(My<CompilerContext>.Instance.CodeBuilder, My<BooCodeBuilder>.Instance));
		}

		public class DummyService
		{
		}

		[Test]
		public void AutomaticServiceRegistration()
		{
			RunInCompilerContextEnvironment(delegate
			{
				Assert.IsNotNull(My<DummyService>.Instance);
				Assert.AreSame(My<DummyService>.Instance, My<DummyService>.Instance);
			});
		}

        class DummyServiceExtension : DummyService
        {   
        }

        [Test]
        public void MyExistingServiceThroughSubTyping()
        {
			RunInCompilerContextEnvironment(delegate
            {
                Assert.IsNotNull(My<DummyServiceExtension>.Instance);
                Assert.AreSame(My<DummyService>.Instance, My<DummyServiceExtension>.Instance);
            });
        }

		private void RunInCompilerContextEnvironment(Action action)
		{
			ActiveEnvironment.With(new CompilerContext(false).Environment, () => {
				action();
			});
		}
	}
}
