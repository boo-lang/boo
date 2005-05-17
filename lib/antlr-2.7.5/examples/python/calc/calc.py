## This file is part of PyANTLR. See LICENSE.txt for license
## details..........Copyright (C) Wolfgang Haefelinger, 2004.
##
## $Id$

import sys
import antlr

class Parser(antlr.LLkParser):
   def __init__(self,*args):
      super(Parser,self).__init__(*args)
      self.num_err = 0
      self.num_wrn = 0

   def reportError(self,err):
      self.num_err += 1
      super(Parser,self).reportError(err)

   def reportWarning(self,err):
      self.num_wrn += 1
      super(Parser,self).reportWarning(err)





class Walker(antlr.TreeParser):
   def __init__(self,*args):
      super(Walker,self).__init__(*args)
      self.depth = 0
      
   def tofloat(self,ast):
      s = ast.getText()
      return float(s)

  
   def howmanysiblings(self,ast):
      if ast == None:
         return -1
    
      r = 0
      ast = ast.getNextSibling()
      while(ast != None) :
         r += 1
         ast = ast.getNextSibling()
      return r
  
   def traceIn(self,s,ast) :
      self.depth += 1
      print ">" * self.depth
      print " " + s + "( `"
      if (ast==None):
         print("()")
      else:
         print(ast.toStringList())
      print " ') | siblings:",self.howmanysiblings(ast)

  
   def traceOut(self,s,ast):
      self.depth += 1
      print(">" * self.depth)
      print(" " + s + "( `")
      if (ast==None):
         print("()")
      else:
         print(ast.toStringList())
      print(" ')")        
      self.depth -= 1


class TNode(antlr.CommonAST):
   def __init__(self,token=None):
      antlr.CommonAST.__init__(self,token)

   ### change printing style 
   def toStringTree(self):
      ts = ""
      kid = self.getFirstChild()
      if kid:
         ts += "{"
      ts += " " + self.toString()
      if kid:
         ts += kid.toStringList()
         ts += "}"
      return ts

class BodyNode(antlr.CommonAST):
   def __init__(self,token=None):
      antlr.CommonAST.__init__(self,token)

   ### change printing style 
   def toStringTree(self):
      ts = "BODY: "
      kid = self.getFirstChild()
      if kid:
         ts += "{"
      ts += " " + self.toString()
      if kid:
         ts += kid.toStringList()
         ts += "}"
      return ts

class ExprNode(antlr.CommonAST):
   def __init__(self,token=None):
      antlr.CommonAST.__init__(self,token)

   ### change printing style 
   def toStringTree(self):
      ts = "EXPR: "
      kid = self.getFirstChild()
      if kid:
         ts += "{"
      ts += " " + self.toString()
      if kid:
         ts += kid.toStringList()
         ts += "}"
      return ts

      
class NumberNode(antlr.CommonAST):
   def __init__(self,token=None):
      antlr.CommonAST.__init__(self,token)

   ### change printing style 
   def toStringTree(self):
      ts = "NUMBER: "
      kid = self.getFirstChild()
      if kid:
         ts += "{"
      ts += " " + self.toString()
      if kid:
         ts += kid.toStringList()
         ts += "}"
      return ts

      



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
      


def main():
   import CalcLexer
   import CalcParser
   import CalcWalker
   
   L = CalcLexer.Lexer() 
   P = CalcParser.Parser(L)
   P.setFilename(L.getFilename())

   ### Parse the input expression
   P.body()
      
   if(P.num_err>0):
      print "*** " + P.num_err + " error(s) while parsing."
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

   W = CalcWalker.Walker()
   s = W.body(ast)
   print "*sum =>",s


if __name__ == "__main__":
   main()
