import System
import System.IO
import antlr from antlr.runtime
import Boo.AntlrParser from Boo.AntlrParser

def Consume(reader as TextReader):
	writer = StringWriter()
	for line in reader:
		writer.WriteLine(line)
	return StringReader(writer.ToString())

lexer = BooParser.CreateBooLexer("stdin", Consume(Console.In))
while token=lexer.nextToken():
	if token.Type == Token.EOF_TYPE:
		break
	print(token)

