import sys
import antlr

version = sys.version.split()[0]
if version < '2.2.1':
    False = 0
if version < '2.3':
    True = not False

class MyAST(antlr.CommonAST): pass

class ASTType49(antlr.CommonAST): pass




def testDefaultCreate():
   t =factory.create()
   return checkNode(t, antlr.CommonAST, antlr.INVALID_TYPE)


def testSpecificHomoCreate() :
   ### class names as strings not supported
   factory.setASTNodeClass(MyAST)
   t = factory.create()
   factory.setASTNodeClass(antlr.CommonAST)
   return checkNode(t, MyAST, antlr.INVALID_TYPE)


def testDynamicHeteroCreate() :
    factory.setTokenTypeASTNodeType(49,ASTType49)
    t = factory.create(49)
    a = checkNode(t, ASTType49, 49)
    u = factory.create(55)
    b = checkNode(u,antlr.CommonAST, 55)
    v = factory.create(49,"",MyAST)
    c = checkNode(v, MyAST, 49)
    factory.setTokenTypeASTNodeType(49,None)
    return a and b and c
  

def testNodeDup() :
    t = factory.create()
    a = t.equals(antlr.dup(t,factory))
    b = not t.equals(None)
    u = factory.create(49,"",ASTType49)
    c = checkNode(antlr.dup(u,factory),ASTType49, 49)
    d = u.equals(antlr.dup(u,factory))
    return a and b and c and d

def testHeteroTreeDup() :
    x = factory.create(1,"[type 1]",MyAST) ## will be root
    y = factory.create(2,"[type 2]",MyAST)
    z = factory.create(3,"[type 3]",MyAST)
    sub = factory.create(49,"[type 49]",ASTType49)
    sub.addChild(factory.create(3,"[type 3 #2]",MyAST))
    t = antlr.make(x,y,sub,z)
    dup_t = antlr.dupList(t,factory)
    ## check structure
    a = dup_t.equalsList(t)
    ## check types
    b = equalsNodeTypesList(t,dup_t)
    return a and b 

def checkNode(t,c, tokenType) :
   if not t:
      return False
   if t.__class__ != c:
      return False

   if t.getType()!=tokenType:
      return False
    
   return True 
  

def equalsNodeTypesList(this, t) :
   return antlr.cmptree(this,t,partial=False)

def error(test) :
    print "Test "+test+" FAILED"

def success(test) :
    print "Test "+test+" succeeded"

if __name__ == "__main__" :
   factory = antlr.ASTFactory()

   funcs = [
      testDefaultCreate,
      testSpecificHomoCreate,
      testDefaultCreate,
      testSpecificHomoCreate,
      testNodeDup,
      testHeteroTreeDup
      ]

   for f in funcs:
      if f():
         success(f.__name__)
      else:
         error(f.__name__)
          
   import ASTsupportParser
   P = ASTsupportParser.Parser()

   P.main()
