namespace Boo.Lang.Useful.BooTemplate

import Boo.Lang.Compiler
import Boo.Lang.Compiler.IO
import Boo.Lang.Compiler.Steps

class TemplatePreProcessor(AbstractCompilerStep):
	override def Run():
		new = []
		for input in self.Parameters.Input:
			using reader=input.Open():
				code = booify(reader.ReadToEnd())
				new.Add(StringInput(input.Name, code))
		self.Parameters.Input.Clear()
		for input in new:
			self.Parameters.Input.Add(input)

def booify(code as string):
	buffer = System.IO.StringWriter()
	output = def(code as string):
		return if len(code) == 0
		buffer.Write('Output.Write("""')
		buffer.Write(code)
		buffer.WriteLine('""")')

	lastIndex = 0
	index = code.IndexOf("<%")
	while index > -1:
		output(code[lastIndex:index])
		lastIndex = code.IndexOf("%>", index + 2)
		raise 'expected %>' if lastIndex < 0
		buffer.WriteLine(code[index+2:lastIndex])
		lastIndex += 2
		index = code.IndexOf("<%", lastIndex)
		
	output(code[lastIndex:])
	return buffer.ToString()

