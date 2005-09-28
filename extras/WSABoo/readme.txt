White Space Agnostic Boo Parser
===============================

This is a sample on how to create a different syntax flavor of boo. Since antlr
is not good at grammar inheritance I decided to copy the entire boo grammar
and go from there.

This particular syntax was created specifically for use with the
Brail view engine (http://www.castleproject.org/index.php/MonoRail:Brail).

With this new parser indentation is no longer used as a block delimiter but COLON end.

	class Foo:
		def foo():
			print 'Hello'
		end
	end
