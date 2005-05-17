
import sys
import traceback
import antlr

import SimpleLexer
import SimpleParser
import SimpleParser2

lexer = None
parser = None
parser2 = None

class Main:

    global lexer, parser, parser2

    def __init__(self):
	try:
	    lexer = SimpleLexer.Lexer(sys.stdin);

	    # Invoke first parser
	    sys.stdout.write("first parser" + '\n')
	    parser = SimpleParser.Parser(lexer)
	    parser.simple();

	    # Now we need to get the inputState from the first parser
	    # this includes data about guessing and stuff like it.
	    # If we don't do this and create the second parser
	    # with just the lexer object we might (doh! will!) miss tokens
	    # read for lookahead tests.
	    self.inputstate = parser.getInputState()

	    # When first parser runs out, invoke secnond parser
	    sys.stdout.write("second parser" + '\n')
	    parser2 = SimpleParser2.Parser(self.inputstate)
	    parser2.simple()

	except antlr.ANTLRException, e:
	    sys.stderr.write("exception: " + str(e) + '\n')
	    #apply(traceback.print_exception, sys.exc_info())
	except Exception, e:
	    sys.stderr.write("exception: " + str(e) + '\n')
	    #apply(traceback.print_exception, sys.exc_info())

if __name__ == '__main__':
    Main()
