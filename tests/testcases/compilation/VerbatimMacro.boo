"""
beep beep
beep beep
coyote falls off a cliff
roadrunner doesn't actually say that
zoom at 42.5
hunter falls off a cliff
"""
import System.Globalization
import Boo.Lang.Compiler.Ast

macro roadrunner(nemesis as Expression):
	macro say(words as verbatim):
		yield [| print $("beep beep" if words in ("beep", "beep beep") else "roadrunner doesn't actually say that") |]

	macro run(speed as verbatim):
		parsed as double
		raise "roadrunner runs at a number, not '${speed}'" unless double.TryParse(speed, NumberStyles.Float, CultureInfo.InvariantCulture, parsed)
		yield [| print $("zoom at ${speed}") |]

	macro evade:
		chased = evade.GetParentMacroByName("roadrunner").Arguments[0]
		yield [| print $chased + " falls off a cliff" |]

	yield roadrunner.Body

for nemesis in ("coyote", "hunter"):
	roadrunner nemesis:
		if nemesis == "coyote":
			# the coyote
			say beep
			say beep beep
			evade
		else:
			# anyone else
			say meep
			run 42.5
			evade
