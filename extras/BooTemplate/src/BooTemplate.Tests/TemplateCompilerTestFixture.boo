namespace BooTemplate.Tests

import NUnit.Framework
import BooTemplate
import Boo.Lang.Compiler
import Boo.Lang.Compiler.IO
import System.IO

abstract class MyBaseClass(AbstractTemplate):
	def GetRange():
		return range(1, 4)

[TestFixture]
class TemplateCompilerTestFixture:

	[Test]
	def TestSimpleTemplate():
		
		text = """<%for i in GetRange():%>\${i}<%end%>"""
		
		compiler = TemplateCompiler(
						TemplateClassName: 'MyTemplate',
						TemplateBaseClass: MyBaseClass)
		results = compiler.Compile(StringInput('code', text))		
		Assert.AreEqual(0, len(results.Errors), results.Errors.ToString())
		Assert.AreEqual(0, len(results.Warnings), results.Warnings.ToString())
		
		templateType = results.GeneratedAssembly.GetType('MyTemplate')
		assert templateType is not null
		assert MyBaseClass is templateType.BaseType
		
		template as ITemplate = templateType()
		template.Output = StringWriter()
		template.Execute()
		Assert.AreEqual("123", normalize(template.Output.ToString()))
