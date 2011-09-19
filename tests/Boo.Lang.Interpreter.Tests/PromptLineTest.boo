namespace Boo.Lang.Interpreter.Tests

import System
import System.IO
import System.Text
import NUnit.Framework
import Boo.Lang.Interpreter

[TestFixture]
class PromptLineTest:

	class StubPromtLine(PromptLine):

		stream = MemoryStream()
		windowWidth as int
		left as int
		top as int

		Buffer as string:
			get:
				return Encoding.UTF8.GetString(stream.ToArray()).Trim()

		override property WindowWidth as int

		override property Left as int

		override property Top as int

		override def Write(data as string):
			stream.Position = Left + Top * WindowWidth
			buffer = Encoding.UTF8.GetBytes(data)
			stream.Write(buffer, 0, buffer.Length)
			Left = stream.Position
			if Left >= WindowWidth:
				Top = Left / WindowWidth
				Left = Left % WindowWidth

	line as StubPromtLine

	[SetUp]
	def SetUp():
		line = StubPromtLine()
		line.WindowWidth = 10

	[Test]
	def ReplacePromtString():
		line.Text = "12345678901"
		line.Text = "456"
		Assert.AreEqual(line.Top, 0)
		Assert.AreEqual(line.Left, 3)
		Assert.AreEqual(line.Buffer, "456")

	[Test]
	def InsertChar():
		line.Text = "abc"
		line.Position--
		line.Append(char('f'))
		Assert.AreEqual(line.Left, 3)
		Assert.AreEqual(line.Text, "abfc")
		Assert.AreEqual(line.Buffer, "abfc")

	[Test]
	def RemoveChar():
		line.Text = "abc"
		line.Position -= 2
		line.Remove(1)
		Assert.AreEqual(line.Text, "ac")
		Assert.AreEqual(line.Buffer, "ac")
		Assert.AreEqual(line.Position, 1)