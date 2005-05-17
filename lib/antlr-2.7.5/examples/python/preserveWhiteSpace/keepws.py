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
      

stream = None

def setstream(st):
   import keepws
   keepws.stream = st

def getstream():
   assert stream
   return stream

### referenced by treewalker
def write(*args):
   import sys
   sys.stdout.write(*args)
   sys.stdout.flush()

### walk list of hidden tokens in order, printing them out
def dumpHidden(t):
   assert stream
   while t:
      write(t.getText())
      t = stream.getHiddenAfter(t)

def pr(p):
   write(p.getText())
   dumpHidden(p.getHiddenAfter())


def main():
   import keepws_l
   import keepws_p
   import keepws_w
   
   L = keepws_l.Lexer() 

   ### change token class
   L.setTokenObjectClass(antlr.CommonHiddenStreamToken)

   ### create new token stream - referenced by parser
   ### global stream
   st = antlr.TokenStreamHiddenTokenFilter(L);
   st.hide(keepws_p.WS);
   st.hide(keepws_p.SL_COMMENT);
   setstream(st)
   ### create parser with my stream
   P = keepws_p.Parser(st)
   P.setFilename(L.getFilename())

   ### use this kind of AST nodes
   P.setASTNodeClass(antlr.CommonASTWithHiddenTokens)

   ### Parse the input expression
   try:
      P.slist()
   except antlr.ANTLRException, ex:
      print "*** error(s) while parsing."
      print ">>> exit(1)"
      import sys
      sys.exit(1)

   
   ast = P.getAST()
   
   if not ast:
      print "stop - no AST generated."
      import sys
      sys.exit(1)
      
   ###show tree
   print "Tree: " + ast.toStringTree()
   print "List: " + ast.toStringList()
   print "Node: " + ast.toString()
   print "visit>>"
   visitor = Visitor()
   visitor.visit(ast);
   print "visit<<"

   W = keepws_w.Walker()
   W.slist(ast)
   print "Ast tree walked without problems."

if __name__ == "__main__":
   main()
