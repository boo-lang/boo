"""
BCE0045-4.boo(13,1): BCE0045: Macro expansion error: Enumerable or array type argument `foo` must be the last argument.
BCE0045-4.boo(21,1): BCE0045: Macro expansion error: Unsupported type `object` for argument `foo`, a macro argument type must be a literal-able primitive or an AST node.
BCE0045-4.boo(29,1): BCE0045: Macro expansion error: Cannot cast item #2 from `Boo.Lang.Compiler.Ast.IntegerLiteralExpression` to `Boo.Lang.Compiler.Ast.StringLiteralExpression`.
BCE0045-4.boo(33,1): BCE0045: Macro expansion error: `ints1` macro invocation argument(s) did not match definition: `ints1((x as string), (y as System.Collections.Generic.IEnumerable[of int]))`.
BCE0045-4.boo(34,1): BCE0045: Macro expansion error: Cannot cast item #2 from `Boo.Lang.Compiler.Ast.StringLiteralExpression` to `Boo.Lang.Compiler.Ast.IntegerLiteralExpression`.
BCE0045-4.boo(37,1): BCE0045: Macro expansion error: BCE0045: Macro expansion error: Cannot cast item #1 from `Boo.Lang.Compiler.Ast.MemberReferenceExpression` to `Boo.Lang.Compiler.Ast.MethodInvocationExpression`.
"""
import Boo.Lang.Compiler.Ast
import Boo.Lang.PatternMatching


macro notlast(foo as string*, x as int):
	pass
macro strings0(x as string*):
	pass
macro ints1(x as string, y as int*):
	pass
macro mies0(mie as MethodInvocationExpression*):
	pass
macro unsupported(foo as object*):
	pass



strings0
strings0 "ok"
strings0 "ok", "ok"
strings0 "FAIL", 0

ints1 "s"
ints1 "s", 4
ints1 4
ints1 "s", "FAIL"

mies0 Console.WriteLine("ok"), Console.Write("OK")
mies0 Console.WriteLine, Console.Write

