namespace Boo.Lang.Useful.BooTemplate.Tests

import NUnit.Framework
import Useful.BooTemplate
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
		CheckTemplate("123", compiler, text)
		
	[Test]
	def TestDefaultImports():
		text = """<%
		node = SimpleTypeReference('foo')
		%>\${node}"""
		
		compiler = TemplateCompiler()
		compiler.DefaultImports.Add("Boo.Lang.Compiler.Ast")
		CheckTemplate("foo", compiler, text)
		
	def CheckTemplate(expected as string, compiler as TemplateCompiler, text as string):
		results = compiler.Compile(StringInput('code', text))
		Assert.AreEqual(0, len(results.Errors), results.Errors.ToString())
		Assert.AreEqual(0, len(results.Warnings), results.Warnings.ToString())
		
		templateType = results.GeneratedAssembly.GetType(compiler.TemplateClassName)		
		assert templateType is not null
		assert compiler.TemplateBaseClass is templateType.BaseType
		assert 1 == len(templateType.GetConstructors())
		ctor = templateType.GetConstructors()[0]
		assert ctor.IsPublic
		assert 0 == len(ctor.GetParameters())
		
		template as ITemplate = templateType()
		template.Output = StringWriter()
		template.Execute()
		Assert.AreEqual(expected, normalize(template.Output.ToString()))
		
