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

if len(argv) and "-" != argv[0]:
	reader = File.OpenText(argv[0])
else:
	reader = Consume(Console.In)
	
lexer as antlr.TokenStream
if "/r" in argv: 
	lexer = CreateRawLexer("stdin", reader)
else:
	lexer = BooParser.CreateBooLexer("stdin", reader)
while token=lexer.nextToken():
	break if token.Type == Token.EOF_TYPE
	print(token)

