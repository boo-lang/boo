namespace BooCompiler.Tests.TypeSystem

import Boo.Lang.Compiler.TypeSystem
import Boo.Lang.Compiler.TypeSystem.Services
import Boo.Lang.Environments
import NUnit.Framework

class RecordingEntityFormatter(EntityFormatter):
	"""Counts what DisplayName asked it to format, in place of a mock."""

	_formatted = Boo.Lang.List[of IType]()

	FormattedTypes as Boo.Lang.List[of IType]:
		get: return _formatted

	override def FormatType(type as IType) as string:
		_formatted.Add(type)
		return ""

abstract class EntityFormatterTestBase(AbstractTypeSystemTest):
	[Test]
	def SimpleTypeDisplayNameGoesThroughEntityFormatter():
		AssertDisplayNameGoesThroughEntityFormatter(SimpleType())

	[Test]
	def ArrayTypeDisplayNameGoesThroughEntityFormatter():
		AssertDisplayNameGoesThroughEntityFormatter(ArrayType())

	[Test]
	def CallableTypeDisplayNameGoesThroughEntityFormatter():
		AssertDisplayNameGoesThroughEntityFormatter(CallableType())

	[Test]
	def GenericTypeDisplayNameGoesThroughEntityFormatter():
		AssertDisplayNameGoesThroughEntityFormatter(GenericType())

	protected abstract def SimpleType() as IType:
		pass

	protected abstract def CallableType() as IType:
		pass

	protected abstract def GenericType() as IType:
		pass

	protected abstract def ArrayType() as IType:
		pass

	protected static def AssertDisplayNameGoesThroughEntityFormatter(entity as IType):
		formatter = RecordingEntityFormatter()
		ActiveEnvironment.With(ClosedEnvironment(formatter)):
			entity.DisplayName()

			Assert.AreEqual(1, formatter.FormattedTypes.Count)
			Assert.AreSame(entity, formatter.FormattedTypes[0])
