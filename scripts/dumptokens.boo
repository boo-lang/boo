import System
import System.IO
import antlr from antlr.runtime
import Boo.AntlrParser from Boo.AntlrParser

def Consume(reader as TextReader):
	writer = StringWriter()
	for line in reader:
		writer.WriteLine(line)
	return StringReader(writer.ToString())
	
def CreateRawLexer(name, reader as TextReader):
	lexer = BooLexer(reader)
	lexer.setFilename("stdin")
	return lexer

reader as TextReader

if len(argv):
	reader = File.OpenText(argv[0])
else:
	reader = Consume(Console.In)
	
// lexer = CreateRawLexer("stdin", reader)
lexer = BooParser.CreateBooLexer("stdin", reader)
while token=lexer.nextToken():
	if token.Type == Token.EOF_TYPE:
		break
	print(token)

