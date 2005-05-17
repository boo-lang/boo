import antlr
import System

lexer = CalcLexer(CharBuffer(Console.In))
lexer.setFilename("<stdin>")

parser = CalcParser(lexer)
parser.setFilename("<stdin>")

// Parse the input expression
parser.expr()
t as CommonAST = parser.getAST()

// Print the resulting tree out in LISP notation
print t.ToStringTree()

walker = CalcTreeWalker()

// Traverse the tree created by the parser
r = walker.expr(t)
print "value is ${r}"
