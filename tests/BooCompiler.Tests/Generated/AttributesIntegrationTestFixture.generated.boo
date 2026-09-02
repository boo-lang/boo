namespace BooCompiler.Tests

import NUnit.Framework

[TestFixture]
class AttributesIntegrationTestFixture(AbstractCompilerTestCase):

	[Test]
	def @assembly_attributes_1():
		RunCompilerTestCase("assembly-attributes-1.boo")

	[Test]
	def @assembly_attributes_2():
		RunCompilerTestCase("assembly-attributes-2.boo")

	[Test]
	def @attributes_1():
		RunCompilerTestCase("attributes-1.boo")

	[Test]
	def @attributes_2():
		RunCompilerTestCase("attributes-2.boo")

	[Test]
	def @attributes_3():
		RunCompilerTestCase("attributes-3.boo")

	[Test]
	def @attributes_4():
		RunCompilerTestCase("attributes-4.boo")

	[Test]
	def @attributes_5():
		RunCompilerTestCase("attributes-5.boo")

	[Test]
	def @attributes_6():
		RunCompilerTestCase("attributes-6.boo")

	[Test]
	def @attributes_7():
		RunCompilerTestCase("attributes-7.boo")

	[Test]
	def @attributes_8():
		RunCompilerTestCase("attributes-8.boo")

	[Test]
	def @conditionalattribute_1():
		RunCompilerTestCase("conditionalattribute-1.boo")

	[Test]
	def @conditionalattribute_2():
		RunCompilerTestCase("conditionalattribute-2.boo")

	[Test]
	def @conditionalattribute_3():
		RunCompilerTestCase("conditionalattribute-3.boo")

	[Test]
	def @conditionalattribute_4():
		RunCompilerTestCase("conditionalattribute-4.boo")

	[Test]
	def @module_ast_attribute():
		RunCompilerTestCase("module-ast-attribute.boo")

	[Test]
	def @ns_alias_on_attribute():
		RunCompilerTestCase("ns_alias_on_attribute.boo")

	[Test]
	def @transient():
		RunCompilerTestCase("transient.boo")

	[Test]
	def @varargs_attribute_external():
		RunCompilerTestCase("varargs-attribute-external.boo")

	[Test]
	def @varargs_attribute_internal():
		RunCompilerTestCase("varargs-attribute-internal.boo")

	override protected def GetRelativeTestCasesPath() as string:
		return "integration/attributes"
