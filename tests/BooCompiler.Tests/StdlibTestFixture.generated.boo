namespace BooCompiler.Tests

import NUnit.Framework

[TestFixture]
class StdlibTestFixture(AbstractCompilerTestCase):

	[Test]
	def @DynamicVariable_1():
		RunCompilerTestCase("DynamicVariable-1.boo")

	[Test]
	def @cat_1():
		RunCompilerTestCase("cat-1.boo")

	[Test]
	def @cat_2():
		RunCompilerTestCase("cat-2.boo")

	[Test]
	def @environments_1():
		RunCompilerTestCase("environments-1.boo")

	[Test]
	def @formatting_1():
		RunCompilerTestCase("formatting-1.boo")

	[Test]
	def @hash_1():
		RunCompilerTestCase("hash-1.boo")

	[Test]
	def @hash_2():
		RunCompilerTestCase("hash-2.boo")

	[Test]
	def @hash_3():
		RunCompilerTestCase("hash-3.boo")

	[Test]
	def @join_1():
		RunCompilerTestCase("join-1.boo")

	[Test]
	def @len_1():
		RunCompilerTestCase("len-1.boo")

	[Test]
	def @list_add_1():
		RunCompilerTestCase("list-add-1.boo")

	[Test]
	def @list_equals_1():
		RunCompilerTestCase("list-equals-1.boo")

	[Test]
	def @list_find_1():
		RunCompilerTestCase("list-find-1.boo")

	[Test]
	def @list_indexof_1():
		RunCompilerTestCase("list-indexof-1.boo")

	[Test]
	def @list_indexof_2():
		RunCompilerTestCase("list-indexof-2.boo")

	[Test]
	def @list_indexof_3():
		RunCompilerTestCase("list-indexof-3.boo")

	[Test]
	def @list_removeall_1():
		RunCompilerTestCase("list-removeall-1.boo")

	[Test]
	def @list_sort_1():
		RunCompilerTestCase("list-sort-1.boo")

	[Test]
	def @map_1():
		RunCompilerTestCase("map-1.boo")

	[Test]
	def @range_1():
		RunCompilerTestCase("range-1.boo")

	[Test]
	def @range_2():
		RunCompilerTestCase("range-2.boo")

	[Test]
	def @range_3():
		RunCompilerTestCase("range-3.boo")

	[Test]
	def @reversed_1():
		RunCompilerTestCase("reversed-1.boo")

	override protected def GetRelativeTestCasesPath() as string:
		return "stdlib"
