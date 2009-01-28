"""
Obama says: Yes! We can!
Obama shouts: YES! WE CAN!
Obama shouts: YES! WE CAN!!!!
Sarkozy dit: Casse-toi pauvre con!
Sarkozy crie: CASSE-TOI PAUVRE CON!
Sarkozy crie: CASSE-TOI PAUVRE CON!!!!
"""
###
###Internal indirect nested macro extension resolution testcase.
###
###This might not be very useful (nor supported) as an internal feature, since
###one can declare a nested macro extension that yieldAll the original macro.
###
###This is primarily used internally for external macro extension resolution.
###If a macro `YELL` extends an external macro `speaker.english` then
###generated macro type is `$SpeakerMacro$EnglishMacro$YELLMacro` since we
###cannot nest the type as usual in the parent macro.
###
###Following the same principle than code below an extension is added to the
###module in order to validate that the nested and correct, and of course provide
###the redirection to the actual generated macro type.
###

[System.Runtime.CompilerServices.CompilerGeneratedAttribute]
[extension]
def YELL(parent as SpeakerMacro.EnglishMacro, context as Boo.Lang.Compiler.CompilerContext) as SpeakerMacro.EnglishMacro.ShoutMacro:
	return SpeakerMacro.EnglishMacro.ShoutMacro(context)

[System.Runtime.CompilerServices.CompilerGeneratedAttribute]
[extension]
def GUEULE(parent as SpeakerMacro.FrenchMacro, context as Boo.Lang.Compiler.CompilerContext) as SpeakerMacro.FrenchMacro.ShoutMacro:
	return SpeakerMacro.FrenchMacro.ShoutMacro(context)


macro speaker:

	macro english:
		macro say:
			yield [| print $(speaker.Arguments[0])+" says: "+$(english.Arguments[0]) |]
		yield

	macro french:
		macro say:
			yield [| print $(speaker.Arguments[0])+" dit: "+$(french.Arguments[0]) |]
		yield

	yield


macro speaker.english.shout:
	suffix = (shout.Arguments[0] if shout.Arguments.Count == 1 else [| "" |])
	yield [| print $(speaker.Arguments[0])+" shouts: "+$(english.Arguments[0]).ToUpper()+$suffix |]


macro speaker.french.shout:
	suffix = (shout.Arguments[0] if shout.Arguments.Count == 1 else [| "" |])
	yield [| print $(speaker.Arguments[0])+" crie: "+$(french.Arguments[0]).ToUpper()+$suffix |]


speaker "Obama":
	english "Yes! We can!":
		say
		shout
		YELL "!!!"
		#GUEULE #fires error (BCE0017)

speaker "Sarkozy":
	french "Casse-toi pauvre con!":
		say
		shout
		GUEULE "!!!"
	#GUEULE #fires error

#GUEULE #fires error

