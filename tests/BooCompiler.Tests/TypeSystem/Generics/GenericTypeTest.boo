namespace BooCompiler.Tests.TypeSystem.Generics

import System.Collections.Generic
import System.Linq
import Boo.Lang.Compiler.TypeSystem
import BooCompiler.Tests.TypeSystem
import NUnit.Framework

[TestFixture]
class GenericTypeTest(AbstractTypeSystemTest):
	[Test]
	def DeclaringTypeOfConstructedMethod():
		RunInCompilerContextEnvironment() do:
			genericType = TypeSystemServices.Map(typeof(IEnumerable[of *]))
			internalType = BuildInternalClass("", "Bar").Entity

			constructedType = genericType.GenericInfo.ConstructType(internalType)
			firstMethod = constructedType.GetMembers().OfType[of IMethod]().First()
			Assert.AreSame(constructedType, firstMethod.DeclaringType)
