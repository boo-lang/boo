using System;
using NUnit.Framework;
using Boo.Lang;
using Boo.Ast;
using Boo.Ast.Compilation;

namespace Boo.Tests.Ast.Compilation
{
	/// <summary>	
	/// </summary>
	[TestFixture]
	public class ContextTestCase : Assertion
	{
		CompilerContext _context;

		[SetUp]
		public void SetUp()
		{
			_context = new CompilerContext(new CompileUnit());
		}

		[Test]
		public void TestResolveType()
		{
			UsingCollection imports = new UsingCollection();

			List resolved = _context.ResolveExternalType("RequiredAttribute", imports, null);
			AssertEquals(1, resolved.Count);
			AssertSame(typeof(RequiredAttribute), resolved[0]);

			_context.ResolveExternalType(resolved, "RequiredAttribute", null);
			AssertEquals("Tipos j existentes na lista no devem ser adicionados novamente!", 
				1, resolved.Count);

			resolved = _context.ResolveExternalType("required", null, null);
			AssertEquals(0, resolved.Count);			
		}
	}
}
