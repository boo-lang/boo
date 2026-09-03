namespace BooCompiler.Tests

import Boo.Lang.Compiler
import Boo.Lang.Compiler.Ast
import Boo.Lang.Compiler.Services
import Boo.Lang.Compiler.TypeSystem
import Boo.Lang.Compiler.TypeSystem.Services
import Boo.Lang.Environments
import NUnit.Framework

class RecordingLanguageAmbiance(LanguageAmbiance):
	"""Answers for one type name and counts the asking, in place of a mock."""

	[getter(Asked)]
	_asked = 0

	override def DefaultGeneratorTypeFor(typeName as string) as string:
		assert "string" == typeName
		++_asked
		return "string*"

class FixedEntityFormatter(EntityFormatter):
	"""Formats the one type it was given and nothing else."""

	_expected as IType

	def constructor(expected as IType):
		_expected = expected

	override def FormatType(type as IType) as string:
		assert _expected is type
		return "string"

[TestFixture]
class CompilerErrorFactoryTest:
	[Test]
	def DefaultGeneratorTypeRepresentationComesFromLanguageAmbience():
		ambiance = RecordingLanguageAmbiance()
		type = UntouchableType()
		formatter = FixedEntityFormatter(type)

		ActiveEnvironment.With(ClosedEnvironment(ambiance, formatter)):
			CompilerErrorFactory.InvalidGeneratorReturnType(SimpleTypeReference(), type)

			Assert.AreEqual(1, ambiance.Asked)
