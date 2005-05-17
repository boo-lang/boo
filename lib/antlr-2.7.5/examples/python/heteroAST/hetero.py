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


      
class CalcAST(antlr.BaseAST):
   def __init__(self,*args):
      antlr.BaseAST.__init__(self)

class BinaryOperatorAST(CalcAST):
   def __init__(self,*args):
      CalcAST.__init__(self,*args)
      
   def left(self):
      return self.getFirstChild()

   def right(self):
      t = self.left();
      if not t: return None
      return t.getNextSibling()

   def c2(self):
      t = self.left()
      if t: t = t.getNextSibling()
      assert t
      return t

### A simple node to represent PLUS operation 
class PLUSNode(BinaryOperatorAST):
   def __init__(self,*args):
      BinaryOperatorAST.__init__(self,*args)
      
   ### Compute value of subtree; this is heterogeneous part :)
   def value(self):
      left = self.left()
      assert self
      r = self.c2()
      assert r
      return left.value() + r.value()

   def toString(self):
      return " +";

   def __str__(self):
      return self.toString()

   def __repr__(self):
      return str(self)

### A simple node to represent MULT operation 
class MULTNode(BinaryOperatorAST):
   def __init__(self,*args):
      BinaryOperatorAST.__init__(self,*args)
      
   # Compute value of subtree; this is heterogeneous part :)
   def value(self):
      return self.left().value() * self.c2().value()
   
   def toString(self):
      return " *";
   
   def __str__(self):
      return self.toString()
   
   def __repr__(self):
      return str(self)

### A simple node to represent an INT
class INTNode(CalcAST):
   def __init__(self,*args):
      CalcAST.__init__(self,*args)
      self.v = 0
      if args and isinstance(args[0],antlr.Token):
         self.v = int(args[0].getText())

   # Compute value of subtree; this is heterogeneous part :)
   def value(self):
      return self.v

   def toString(self):
      return " " + str(self.v)


def main():
   import hetero_l
   import hetero_p
   
   L = hetero_l.Lexer() 
   P = hetero_p.Parser(L)
   P.setFilename(L.getFilename())

   ### Parse the input expression
   try:
      P.expr()
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

   ### compute value and return
   r = ast.value()
   print "value is", r
   

if __name__ == "__main__":
   main()
