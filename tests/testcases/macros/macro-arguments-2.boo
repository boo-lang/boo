"""
the sum of those 6 numbers is 108
foofoo
barbar
invocation #1 of 2
invocation #2 of 2
line 'How do you bou?' contains unknown word 'bou'. Did you mean 'boo'?
"""
import System
import Boo.Lang.Compiler.Ast
import Boo.Lang.PatternMatching

macro sum(numbers as int*):
	s = 0
	for n in numbers:
		s += n
	yield [| print "the sum of those ${$(numbers.Count)} numbers is ${$s}" |]

macro repeatLines(repeatCount as int, lines as string*):
	for line in lines:
		yield [| print $line * $repeatCount |]

macro invokeWithCount(body as MethodInvocationExpression*):
	for invocation in body:
		invocation.Arguments.Add([| $(body.Count) |])
		yield invocation

macro spellCheck(lang as string, body as string*):
	raise "Unknown language `${lang}`" if lang != "en-EN"
	for line in body:
		if line.Contains("bou"):
			yield [| print "line '${$line}' contains unknown word 'bou'. Did you mean 'boo'?" |]

macro defaultBodyArg(body):
	assert body isa NodeCollection[of Statement]


sum 4, 8, 15, 16, 23, 42

repeatLines 2, "foo", "bar"

invokeWithCount:
	Console.WriteLine("invocation #1 of {0}")
	Console.WriteLine("invocation #2 of {0}")

spellCheck "en-EN":
	"Hello boo!"
	"How do you bou?"

defaultBodyArg:
	pass

