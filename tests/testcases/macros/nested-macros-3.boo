"""
1
1
2
1
2
3
1
1
0
1
0
3
"""

namespace NestedMacros3

import Boo.Lang.Compiler.MetaProgramming

macro one:
	macro two:
		macro three:
			macro four:
				yield [| print $(one.Arguments.Count) |]
				yield [| print $(two.Arguments.Count) |]
				yield [| print $(three.Arguments.Count) |]
				yield# four.Body

			yield [| print $(one.Arguments.Count) |]
			yield [| print $(two.Arguments.Count) |]
			yield# three.Body

		yield [| print $(one.Arguments.Count) |]
		yield# two.Body

	yield# one.Body

one 1:
	two 1, 2:
		three 1, 2, 3:
			four 1, 2, 3, 4

code = [|
	import NestedMacros3

	one 1:
		two:
			three 1, 2, 3:
				four 1, 2, 3, 4
|]
result = compile(code, typeof(OneMacro).Assembly)
result.EntryPoint.Invoke(null, (null,))

