import sys
import antlr
      

class Visitor(antlr.ASTVisitor):
   def __init__(self,*args):
      super(Visitor,self).__init__(*args)
      self.level = 0
      if not args:
         self.cout = sys.stdout
         return
      if isinstance(args[0],file):
         self.cout = args[0]
         return
      assert 0

   def tabs(self):
      print " " * self.level

   def printf(self,fmt,*args):
      if not args:
          sys.stdout.write(fmt)
          return
      argv = tuple(args)
      self.cout.write(fmt % argv)

   def flush(self):
      self.cout.flush()

   def visit1(self,node):
      if not node:
         self.printf(" nil ")
         return

      c = node.getType()
      t = node.getText()
      k = node.getFirstChild()
      s = node.getNextSibling()
    
      self.printf("( <%s> ",c)
      if t:
         self.printf(" %s ",t)
      self.visit1(k);
      self.visit1(s);
      self.printf(")")

   def visit(self,node):
      self.visit1(node);
      self.printf("\n")

### some global vars referenced by lexer and parser
selector = None


def getselector():
   import multilex
   assert multilex.selector


def check(s):
   import multilex
   assert multilex.selector
   assert multilex.selector == s


def main():
   import multilex_l
   import multilex_p
   import javadoc_l
   
   ## make a selector
   S = antlr.TokenStreamSelector()

   ## and two lexer
   L = multilex_l.Lexer()
   D = javadoc_l.Lexer(L.getInputState())

   ## setup selector with lexer, and ..
   S.addInputStream(L,"main")
   S.addInputStream(D,"doclexer")
   S.select("main")

   ## let parser use selector ..
   P = multilex_p.Parser(S)
   P.setFilename(L.getFilename())

   import multilex
   multilex.selector = S

   check(S)

   ### Parse the input expression
   try:
      P.input()
   except antlr.ANTLRException, ex:
      print "*** error(s) while parsing."
      print ">>> exit(1)"
      import sys
      sys.exit(1)

   
   ast = P.getAST()
   
   if not ast:
      print "stop - no AST generated."
      import sys
      sys.exit(0)
      
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
