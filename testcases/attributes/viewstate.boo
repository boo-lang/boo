"""
import Boo.Lang.Compiler.Tests from Boo.Lang.Compiler.Tests

class Control:

	Width as int:
		get:
			value = ViewState['Width']
			return 70 unless value
			return value
		set:
			ViewState['Width'] = value
"""
import Boo.Lang.Compiler.Tests from Boo.Lang.Compiler.Tests

class Control:
	[ViewState(Default: 70)]
	Width as int
