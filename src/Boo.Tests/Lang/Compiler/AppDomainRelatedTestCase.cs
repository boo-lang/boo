namespace Boo.Tests.Lang.Compiler
{
	using System;
	using System.IO;
	using System.Security.Policy;
	using NUnit.Framework;
	using Boo.Lang.Compiler;
	using Boo.Lang.Compiler.IO;
	
	[TestFixture]
	public class AppDomainRelatedTestCase
	{
		[Test]
		public void TargetDomain()
		{
			Evidence baseEvidence = AppDomain.CurrentDomain.Evidence;
			Evidence evidence = new Evidence(baseEvidence);

			AppDomain domain = AppDomain.CreateDomain("Target Boo Domain",
			                                          evidence);
			try
			{
				string code = "print(System.AppDomain.CurrentDomain.FriendlyName)";
				
				BooCompiler compiler = (BooCompiler)domain.CreateInstanceAndUnwrap("Boo", "Boo.Lang.Compiler.BooCompiler");				
				compiler.Parameters.Pipeline.Load("boom");
				compiler.Parameters.Input.Add(new StringInput("code", code));
				
				CompilerContext context = compiler.Run();
				Assert.AreEqual(0, context.Errors.Count);
			}
			finally
			{
				AppDomain.Unload(domain);
			}
		}		
	}
}
