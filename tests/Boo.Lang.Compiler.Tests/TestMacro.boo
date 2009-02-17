namespace Boo.Lang.Compiler.Tests

import Boo.Lang.PatternMatching

macro test:
"""
Defines a test method that executes its body
in the context of a CompilerContext (available in 
a field named _context)
"""
	case [| test $name |]:
		yield [|
			[Test] def $name():
				_context.Run:
					$(test.Body)
		|]

