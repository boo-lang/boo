
import sys
import antlr

import tinyc_l
import inherit_p

def main():
  
    L = tinyc_l.Lexer() 
    P = inherit_p.Parser(L)
    P.setFilename(L.getFilename())

     ### Parse the input expression
    try:
        P.program()
    except antlr.ANTLRException, ex:
        print "*** error(s) while parsing."
        print ">>> exit(1)"
        sys.exit(1)


    ast = P.getAST()

    if not ast:
	print "stop - no AST generated."
	return

    ###show tree
    print "Tree: " + ast.toStringTree()
    print "List: " + ast.toStringList()
    print "Node: " + ast.toString()
    print "visit>>"
    visitor = Visitor()
    visitor.visit(ast);
    print "visit<<"


if __name__ == "__main__":
    main()
