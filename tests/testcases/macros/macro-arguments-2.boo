"""
the sum of those 6 numbers is 108
foofoo
barbar
answer is 42
42 is everything
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

macro invokeWithAdded42Argument(invocations as MethodInvocationExpression*):
	for invoke in invocations:
		invoke.Arguments.Add([| 42 |])
		yield invoke


sum 4, 8, 15, 16, 23, 42
repeatLines 2, "foo", "bar"
invokeWithAdded42Argument Console.WriteLine("answer is {0}"), Console.WriteLine("{0} is everything")

