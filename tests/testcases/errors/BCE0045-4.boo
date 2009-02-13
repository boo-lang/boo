"""
BCE0045-4.boo(13,1): BCE0045: Macro expansion error: Enumerable or array type argument `foo` must be the last argument.
BCE0045-4.boo(21,1): BCE0045: Macro expansion error: Unsupported type `object` for argument `foo`, a macro argument type must be a literal-able primitive or an AST node.
BCE0045-4.boo(23,1): BCE0045: Macro expansion error: `body` argument must be the last argument.
BCE0045-4.boo(24,1): BCE0045: Macro expansion error: `body` argument must be of enumerable type. Did you mean `body as string*`?.
BCE0045-4.boo(29,1): BCE0045: Macro expansion error: Cannot cast item #2 from `Boo.Lang.Compiler.Ast.IntegerLiteralExpression` to `Boo.Lang.Compiler.Ast.StringLiteralExpression`.
BCE0045-4.boo(33,1): BCE0045: Macro expansion error: `ints1` macro invocation argument(s) did not match definition: `ints1((x as string), (y as System.Collections.Generic.IEnumerable[of int]))`.
BCE0045-4.boo(34,1): BCE0045: Macro expansion error: Cannot cast item #2 from `Boo.Lang.Compiler.Ast.StringLiteralExpression` to `Boo.Lang.Compiler.Ast.IntegerLiteralExpression`.
BCE0045-4.boo(37,1): BCE0045: Macro expansion error: Cannot cast item #1 from `Boo.Lang.Compiler.Ast.MemberReferenceExpression` to `Boo.Lang.Compiler.Ast.MethodInvocationExpression`.
BCE0045-4.boo(40,1): BCE0045: Macro expansion error: `noargument` macro invocation argument(s) did not match definition: `noargument()`.
"""
import Boo.Lang.PatternMatching
macro notlast(foo as string*, x as int): pass
macro strings0(x as string*):
	pass
macro ints1(x as string, y as int*):
	pass
macro mies0(mie as Boo.Lang.Compiler.Ast.MethodInvocationExpression*):
	pass

macro unsupported(foo as object*): pass
macro noargument(): pass
macro bodynotlast(body as string*, x as int): pass
macro bodynotenumerable(body as string): pass

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

noargument
noargument 1

