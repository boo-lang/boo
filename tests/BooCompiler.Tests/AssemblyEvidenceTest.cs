using System;
using System.Security;
using System.Security.Policy;
using Boo.Lang.Compiler;
using Boo.Lang.Compiler.Ast;
using Boo.Lang.Compiler.IO;
using NUnit.Framework;

namespace BooCompiler.Tests
{
	[TestFixture]
	public class AssemblyEvidenceTest : AbstractCompilerTestCase
	{
		[Test]
		public void DynamicAssemblyReceivesDefaultPermissions()
		{
			CompilerContext compilerContext;
			
			_parameters.Evidence = null;
			_parameters.Input.Add(new StringInput("<teststring>", "test = 1"));
			
			Run(null, out compilerContext);
			
			// The newly created Evidence for the assembly will be different from the creator as it only copies the grant sets.
			// Eg: The Compiler Evidence will have a StrongName Hash, the new assembly will not.
			// In other words, the Evidence objects will be different, but will have the same effective permissions.
			// Thus we use SecurityManager.ResolvePolicy to get the PermissionSets for each assembly and compare them.
			Assert.AreEqual(
				SecurityManager.ResolvePolicy(typeof(CompilerParameters).Assembly.Evidence),
				SecurityManager.ResolvePolicy(compilerContext.GeneratedAssembly.Evidence),
				"Resulting Assembly has different permissions than expected"
			);
		}
		
		[Test]
		public void DynamicAssemblyReceivesPassedEvidence()
		{
			CompilerContext compilerContext;
			Evidence internetEvidence;
			
			// Create the test assembly in the Internet Zone
			internetEvidence = new Evidence(new object[] { new Zone(SecurityZone.Internet) }, new object[] { });
			
			_parameters.Evidence = internetEvidence;
			_parameters.Input.Add(new StringInput("<teststring>", "test = 1"));
			
			Run(null, out compilerContext);
			
			// As we provided Evidence, it should be exactly the same as the one we passed in.
			// Thus the test is stricter than simply checking the resulting PermissionSet via SecurityManager.ResolvePolicy
			Assert.AreEqual(internetEvidence, compilerContext.GeneratedAssembly.Evidence, "Evidence is not what was passed");
		}
		
		protected override bool VerifyGeneratedAssemblies
		{
			get
			{
				return false; // Testing in-memory assemblies only
			}
		}
	}
}
