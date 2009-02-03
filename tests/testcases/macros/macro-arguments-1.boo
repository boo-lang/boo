"""
Boo
Rocks!
So true!
Assertion that `a == b` is true failed!
Assertion that expressions `a.ToString()` and `bool.FalseString` are equal failed!
"""
import Boo.Lang.PatternMatching

macro printYOnlyIfZIs42(x as string, y as string, z as long):
	yield [| print $x |]
	yield [| print $y |] if z == 42


macro printOnlyIfTrue(text as string, boolean as bool):
	yield [| print $text |] if boolean


macro assertTrue(condition as Boo.Lang.Compiler.Ast.ConditionalExpression):
	yield [| print "Assertion that `${$(condition.ToCodeString())}` is true failed!" if not $condition |]


macro assertEqual(a,b):
	yield [|
		if $a != $b:
			print "Assertion that expressions `${$(a.ToCodeString())}` and `${$(b.ToCodeString())}` are equal failed!"
	|]


printYOnlyIfZIs42 "Boo", "Rocks!", 42L

printOnlyIfTrue "So false!", false
printOnlyIfTrue "So true!", true

a = true
b = false
assertTrue a != b
assertTrue a == b

assertEqual a.ToString(), bool.TrueString
assertEqual a.ToString(), bool.FalseString

