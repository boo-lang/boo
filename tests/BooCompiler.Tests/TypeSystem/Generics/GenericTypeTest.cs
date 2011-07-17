using System.Collections.Generic;
using System.Linq;
using Boo.Lang.Compiler.Ast;
using Boo.Lang.Compiler.TypeSystem;
using NUnit.Framework;

namespace BooCompiler.Tests.TypeSystem.Generics
{
	[TestFixture]
	public class GenericTypeTest : AbstractTypeSystemTest
	{
		[Test]
		public void DeclaringTypeOfConstructedMethod()
		{
			RunInCompilerContextEnvironment(() =>
			{
				var genericType = TypeSystemServices.Map(typeof(IEnumerable<>));
				var internalType = BuildInternalClass("", "Bar").Entity;

				var constructedType = genericType.GenericInfo.ConstructType(internalType);
				var firstMethod = constructedType.GetMembers().OfType<IMethod>().First();
				Assert.AreSame(constructedType, firstMethod.DeclaringType);
			});
		}
	}
}
