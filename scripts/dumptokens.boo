import System
import antlr from antlr.runtime
import Boo.AntlrParser from Boo.AntlrParser

lexer = BooParser.CreateBooLexer("stdin", Console.In)
while token=lexer.nextToken():
	if token.Type == 1:
		break
	print(token)

