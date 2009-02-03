"""
Boo
Rocks!
So true!
Assertion that `b` is true failed!
Assertion that expressions `a.ToString()` and `bool.FalseString` are equal failed!
String 'foofoo' matched regex /foo/
String 'barbar' DID NOT match regex /foo/
86423 seconds (1.00:00:23) is longer than one day.
"""
import Boo.Lang.PatternMatching

macro printYOnlyIfZIs42(x as string, y as string, z as long):
	yield [| print $x |]
	yield [| print $y |] if z == 42


macro printOnlyIfTrue(text as string, boolean as bool):
	yield [| print $text |] if boolean


macro assertTrue(variable as Boo.Lang.Compiler.Ast.ReferenceExpression):
	yield [| print "Assertion that `${$(variable.Name)}` is true failed!" if not $variable |]


macro assertEqual(a,b):
	yield [|
		if $a != $b:
			print "Assertion that expressions `${$(a.ToCodeString())}` and `${$(b.ToCodeString())}` are equal failed!"
	|]


macro matchRegex(pattern as regex, text as string):
	if pattern.IsMatch(text):
		yield [| print "String '${$text}' matched regex /${$pattern}/" |]
	else:
		yield [| print "String '${$text}' DID NOT match regex /${$pattern}/" |]


macro longerThanOneDay(duration as timespan):
	yield [| print "${$(duration.TotalSeconds)} seconds (${$duration}) is longer than one day." |] if duration > 1d


printYOnlyIfZIs42 "Boo", "Rocks!", 42L

printOnlyIfTrue "So false!", false
printOnlyIfTrue "So true!", true

a = true
b = false
assertTrue a
assertTrue b

assertEqual a.ToString(), bool.TrueString
assertEqual a.ToString(), bool.FalseString

matchRegex /foo/, "foofoo"
matchRegex /foo/, "barbar"

longerThanOneDay 86423s
longerThanOneDay 23h

