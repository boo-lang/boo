namespace BooCompiler.Tests.TypeSystem.Services

import System
import System.IO
import System.Reflection
import System.Reflection.Emit
import Boo.Lang.Compiler
import Boo.Lang.Compiler.Ast
import Boo.Lang.Compiler.TypeSystem
import Boo.Lang.Environments
import BooCompiler.Tests.TypeSystem
import NUnit.Framework

[TestFixture]
class TypeSystemServicesTest(AbstractTypeSystemTest):

	_directory as string

	[SetUp]
	override def SetUp():
		super()
		_directory = Path.Combine(Path.GetTempPath(), "boo-signature-" + Guid.NewGuid().ToString("N"))
		Directory.CreateDirectory(_directory)

	[TearDown]
	def TearDown():
		try:
			Directory.Delete(_directory, true)
		except as IOException:
			pass # the assembly stays loaded, so Windows may keep the file
		except as UnauthorizedAccessException:
			pass

	# A method of an assembly that takes a type from one that is not there to load.
	def MethodTakingAMissingType() as MethodInfo:
		library = PersistedAssemblyBuilder(AssemblyName("MissingLib"), typeof(object).Assembly)
		thing = library.DefineDynamicModule("MissingLib").DefineType("Thing", TypeAttributes.Public)
		thing.CreateType()

		consumer = PersistedAssemblyBuilder(AssemblyName("UsesMissingLib"), typeof(object).Assembly)
		type = consumer.DefineDynamicModule("UsesMissingLib").DefineType("Consumer", TypeAttributes.Public)
		method = type.DefineMethod("Take", MethodAttributes.Public | MethodAttributes.Static, typeof(void), (thing,))
		method.GetILGenerator().Emit(OpCodes.Ret)
		type.CreateType()

		path = Path.Combine(_directory, "UsesMissingLib.dll")
		consumer.Save(path)
		return Assembly.LoadFrom(path).GetType("Consumer").GetMethod("Take")

	[Test]
	def CanReadSignatureOfAnOrdinaryMethod():
		RunInCompilerContextEnvironment() do:
			method = TypeSystemServices.Map(typeof(object).GetMethod("ToString"))
			Assert.IsTrue(TypeSystemServices.CanReadSignature(method))

	[Test]
	def CanReadSignatureOfAMethodBeingCompiled():
		RunInCompilerContextEnvironment() do:
			method = CodeBuilder.CreateMethod("Bar", TypeSystemServices.VoidType, TypeMemberModifiers.Public)
			CodeBuilder.CreateClass("Foo").ClassDefinition.Members.Add(method)
			Assert.IsTrue(TypeSystemServices.CanReadSignature(method.Entity))

	[Test]
	def CannotReadSignatureThatNamesAnAssemblyThatIsNotThere():
		method = MethodTakingAMissingType()
		RunInCompilerContextEnvironment() do:
			entity = TypeSystemServices.Map(method)
			Assert.IsFalse(TypeSystemServices.CanReadSignature(entity))
			# and the answer holds the second time it is asked
			Assert.IsFalse(TypeSystemServices.CanReadSignature(entity))
