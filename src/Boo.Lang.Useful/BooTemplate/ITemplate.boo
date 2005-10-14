namespace Boo.Lang.Useful.BooTemplate

import System.IO

interface ITemplate:
	Output as TextWriter:
		get
		set
		
	def Execute()
	
class AbstractTemplate(ITemplate):
	
	[property(Output)]
	_output as TextWriter
	
	abstract def Execute():
		pass
