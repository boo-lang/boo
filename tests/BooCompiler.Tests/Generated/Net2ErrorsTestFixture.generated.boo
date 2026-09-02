namespace BooCompiler.Tests

import NUnit.Framework

partial class Net2ErrorsTestFixture:

	[Test]
	def @BCE0004_1():
		RunCompilerTestCase("BCE0004-1.boo")

	[Test]
	def @BCE0138_1():
		RunCompilerTestCase("BCE0138-1.boo")

	[Test]
	def @BCE0138_2():
		RunCompilerTestCase("BCE0138-2.boo")

	[Test]
	def @BCE0139_1():
		RunCompilerTestCase("BCE0139-1.boo")

	[Test]
	def @BCE0139_2():
		RunCompilerTestCase("BCE0139-2.boo")

	[Test]
	def @BCE0139_3():
		RunCompilerTestCase("BCE0139-3.boo")

	[Test]
	def @BCE0139_4():
		RunCompilerTestCase("BCE0139-4.boo")

	[Test]
	def @BCE0139_5():
		RunCompilerTestCase("BCE0139-5.boo")

	[Test]
	def @BCE0147_external():
		RunCompilerTestCase("BCE0147-external.boo")

	[Test]
	def @BCE0149_1():
		RunCompilerTestCase("BCE0149-1.boo")

	[Test]
	def @BCE0149_2():
		RunCompilerTestCase("BCE0149-2.boo")

	[Test]
	def @BCE0149_3():
		RunCompilerTestCase("BCE0149-3.boo")

	[Test]
	def @BCE0149_4():
		RunCompilerTestCase("BCE0149-4.boo")

	[Test]
	def @BCE0159():
		RunCompilerTestCase("BCE0159.boo")

	[Test]
	def @BCE0160():
		RunCompilerTestCase("BCE0160.boo")

	[Test]
	def @BCE0161_1():
		RunCompilerTestCase("BCE0161-1.boo")

	[Test]
	def @BCE0161_2():
		RunCompilerTestCase("BCE0161-2.boo")

	[Test]
	def @BCE0162_1():
		RunCompilerTestCase("BCE0162-1.boo")

	[Test]
	def @BCE0162_2():
		RunCompilerTestCase("BCE0162-2.boo")

	[Test]
	def @BCE0162_3():
		RunCompilerTestCase("BCE0162-3.boo")

	[Test]
	def @BCE0162_4():
		RunCompilerTestCase("BCE0162-4.boo")

	[Test]
	def @BCE0162_5():
		RunCompilerTestCase("BCE0162-5.boo")

	[Test]
	def @BCE0162_6():
		RunCompilerTestCase("BCE0162-6.boo")

	[Test]
	def @BCE0162_7():
		RunCompilerTestCase("BCE0162-7.boo")

	[Test]
	def @BCE0163():
		RunCompilerTestCase("BCE0163.boo")

	[Test]
	def @BCE0164_1():
		RunCompilerTestCase("BCE0164-1.boo")

	[Test]
	def @BCE0164_2():
		RunCompilerTestCase("BCE0164-2.boo")

	[Test]
	def @BCE0164_3():
		RunCompilerTestCase("BCE0164-3.boo")

	[Test]
	def @BCE0164_4():
		RunCompilerTestCase("BCE0164-4.boo")

	override protected def GetRelativeTestCasesPath() as string:
		return "net2/errors"
