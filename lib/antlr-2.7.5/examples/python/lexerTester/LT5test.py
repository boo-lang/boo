
import sys
import antlr

import SimpleLexer5
import LexerTester

if __name__ == '__main__':

    try:
	lexer = SimpleLexer5.Lexer(sys.stdin)
	parser = LexerTester.Parser(lexer)
	parser.setFilename('<stdin>')
	# Parse the input expression
	parser.source_text()

    except antlr.TokenStreamException, e:
	sys.stderr.write('exception: ' + str(e) + '\n')

    except antlr.RecognitionException, e:
	sys.stderr.write('exception: ' + str(e) + '\n')
