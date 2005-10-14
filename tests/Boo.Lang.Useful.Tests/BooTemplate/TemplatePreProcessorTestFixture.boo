namespace Boo.Lang.Useful.BooTemplate.Tests

import NUnit.Framework
import Useful.BooTemplate
import Boo.Lang.Compiler
import Boo.Lang.Compiler.IO

[TestFixture]
class TemplatePreProcessorTestFixture:

	[Test]
	def TestNoCode():
		expected = 'Output.Write("""no templates here""")'
		AssertOutput(expected, "no templates here")
		
	[Test]
	def TestOnlyCode():
		expected = "print 'single one here'"
		AssertOutput(expected, "<%print 'single one here'%>")
	
	[Test]
	def TemplateAndCode():
		expected = (
			'Output.Write("""before*""")\n' +
			"print 'Hello'\n" +
			'Output.Write("""*after*""")\n' +
			"print 'again'\n" +
			'Output.Write("""*""")\n')
		AssertOutput(expected, """before*<%print 'Hello'%>*after*<%print 'again'%>*""")
		
	def AssertOutput(expected as string, text as string):
		Assert.AreEqual(normalize(expected), normalize(preprocess(text)))
		
	def preprocess(text as string):
		compiler = BooCompiler()
		compiler.Parameters.Input.Add(StringInput("code", text))
		compiler.Parameters.Pipeline = CompilerPipeline()
		compiler.Parameters.Pipeline.Add(TemplatePreProcessor())
		compiler.Run()
		return compiler.Parameters.Input[0].Open().ReadToEnd()
		
