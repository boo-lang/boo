"""
DO>
<DO
Soviet Russia says hello to EnclosingClass+Foo!
Soviet Russia says hello to EnclosingClass+Bar!
Soviet Russia says hello to EnclosingClass+Zap!
Mission accomplished, nested classes, methods and fields are defeated!
"""

import Boo.Lang.Compiler
import Boo.Lang.Compiler.Ast


macro mixed:

	yield cast(Field, [| (_i = 100) as int |])
	#expression used to to disambiguate from BinaryExpression/assignment within block.
	#another possibility: yield CodeBuilder.CreateField("_i", TypeSystemServices.IntType)

	for typeName in mixed.Arguments:
		yield [|
			class $typeName:
				_ec as EnclosingClass

				def constructor(ec as EnclosingClass):
					_ec = ec

				override def ToString() as string:
					_ec._i++
					return "Soviet Russia says hello to ${$typeName}!"
		|]

	yield [|
		def MissionAccomplished():
			if _i == 103: #because must have started at 100 if above cast OK
				print "Mission accomplished, nested classes, methods and fields are defeated!"
	|]


class EnclosingClass:
	def Do():
		print "DO>"
		mixed Foo, Bar, Zap
		print "<DO"


ec = EnclosingClass()
ec.Do()
print EnclosingClass.Foo(ec)
print EnclosingClass.Bar(ec)
print EnclosingClass.Zap(ec)
ec.MissionAccomplished()

