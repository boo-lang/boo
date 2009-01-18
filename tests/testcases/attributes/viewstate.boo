"""
import BooCompiler.Tests from BooCompiler.Tests

class Control(object):

	Width as int:
		get:
			value = ViewState['Width']
			return 70 unless value
			return value
		set:
			ViewState['Width'] = value
"""
import BooCompiler.Tests from BooCompiler.Tests

class Control:
	[ViewState(Default: 70)]
	Width as int
