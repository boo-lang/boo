import sys
import antlr

version = sys.version.split()[0]
if version < '2.2.1':
    False = 0
if version < '2.3':
    True = not False

class Stack:
   def __init__(self):
      self.data = []
   def push(self,item):
      self.data.append(item)
   def pop(self):
      self.data.pop()
   def peek(self):
      return self.data[-1]
      
class HashTab:
   def __init__(self):
      self.data = {}
   def put(self,k,v):
      self.data[k] = v
   def get(self,k):
      r = None
      try:
         r = self.data[k]
      except:
         pass
      return r


class Context(object):

   def __init__(self):
      self.theGlobalScope = GlobalScope()
      self.theScopeStack  = Stack()
      self.theScopeStack.push(self.theGlobalScope)
      self.subroutineTable = HashTab()
      self.functionTable = HashTab()
      self._invar()

   def _invar(self):
      s = self.theScopeStack.peek()
      assert s
      if not isinstance(s,Scope):
         print "type(s)=",type(s)
         x = ProgramScope()
         print "type(x)=",type(x)
         assert isinstance(x,Scope)
         assert 0
    
   def insertSubroutine(self, v, t):
      self.subroutineTable.put(v.lower(),t)

   def getSubroutine(self, v):
      return self.subroutineTable.get(v.lower())

   def insertFunction(self, v, t):
      self.functionTable.put(v.lower(),t)
      
   def getFunction(self, v):
      return self.functionTable.get(v.lower())
  
   def insertGlobalVariable(self, v, t) :
      self.theGlobalScope.insertVariable(v,t)
  
   def insertVariable(self, v, t) :
      self.getCurrentScope().insertVariable(v,t)
  
   def getVariable(self, var):
      t=self.getCurrentScope().getVariable(var)
      if(t==None):
         t=self.theGlobalScope.getVariable(var)
      return t
  
   def getVariableDimension(self, var):
      dim=self.getCurrentScope().getVariableDimension(var)
      if(dim==0):
         dim=self.theGlobalScope.getVariableDimension(var)
      return dim
  
   def getVariableType(self, var):
      t=self.getCurrentScope().getVariable(var)
      if(t==None):
         t=self.theGlobalScope.getVariable(var)
      
      if t:
         return t.getType()
      else:
         return 0
  
   def isArrayVariable(self,s):
      return (self.getVariableDimension(s) > 0)
  
   def getPrev(self):
      return self.getCurrentScope().getPrev()
  
   def pushScope(self,scope):
      assert isinstance(scope,Scope)
      self.theScopeStack.push(scope)
  
   def popScope(self):
      if self.getCurrentScope() == self.theGlobalScope :
         return self.theGlobalScope
      else:
         return self.theScopeStack.pop()
  
   def getCurrentScope(self):
      self._invar()
      r = self.theScopeStack.peek()
      assert r
      assert isinstance(r,Scope)
      return r
  
  
   def getGlobalScope(self):
      return theGlobalScope
  
  
   def setProgramScope(self,scope=None):
      if not scope:
         scope = ProgramScope(self.getCurrentScope())
      self.theProgramScope = scope
      assert isinstance(self.theProgramScope,Scope)
      while self.theScopeStack.peek() != self.theGlobalScope:
         self.theScopeStack.pop()
      self.theScopeStack.push(self.theProgramScope)
  

   def getProgramScope(self):
      return self.theProgramScope

   
   def pushSubroutineScope(self):
      self.pushScope(SubroutineScope(self.getCurrentScope()))
  
   def initialize(self):
      self.setProgramScope(self.getProgramScope())
  
   def setDimension(self,s,*args):
      v = self.getVariable(s)
      v.setDimension(*args)
  

   def getDTDataType(self,s,*args):
      t = self.getVariable(s)
      r = t.getDTDataType(*args)
      return r
  
   
   def ensureVariable(self,s,t):
      v = self.getVariable(s)
      if not v:
         assert isinstance(t,int)
         v = self.getOne(t,self.getCurrentScope())
         self.insertVariable(s,v)
      return v
  
   
   def getOne(self,t,scope):
      assert isinstance(t,int)
      assert isinstance(scope,Scope)
      t = makeone(t,scope)
      assert t
      return t
   

class SaveEnv:
   def __init__(self,scope,args):
	    self.scope=scope
	    self.args=args
	
   def getScope(self): return scope
   def getArgs(self):  return args

class CodeContext:
   def __init__(self,context,scope,args):
      self.context = context
      self.scope = scope
      self.args = args
  
    
class DTCodeType(object):
   def __init__(self,entry,cb,scope,args,name):
      self.entry  = entry 
      self.cb     = cb  
      self.scope  = scope
      self.args   = args  
      self.name   = name  
      self.callDepthStack = Stack()

   def newCall(self,context):
      codeContext = CodeContext(context,self.scope,self.args)
      self.callDepthStack.push(codeContext)
      context.pushScope(self.scope)
    
    
   def attachArg(self,argnum,arg):
      proxy = self.args[argnum-1]
      proxy.attach(arg)
    
    
   def getAST(self):
      return self.entry


### create data instance
def makeone(aType,scope):
   import basic_p
   assert isinstance(aType,int)
   if ( aType==basic_p.INT_CONST ):
      return DTInteger(scope,0)

   if ( aType==basic_p.INT_VAR     ):
      return DTInteger(scope,0)

   if ( aType==basic_p.FLT_CONST    ):
      return DTFloat(scope,0.0)

   if ( aType==basic_p.FLT_VAR     ):
      return DTFloat  (scope,0.0)

   if ( aType==basic_p.STR_CONST    ):
      return DTString (scope,"")

   if ( aType==basic_p.STR_VAR     ):
      return DTString (scope,"")
   assert 0
   return None
  

class DTDataType(object):
   def __init__(self,scope,_ttype):
      if scope:
         assert isinstance(scope,Scope)
         self.scope=scope
      else:
         self.scope = None
      assert isinstance(_ttype,int)
      self.theType=_ttype

   def getType(self):
      return self.theType
    

   def getInteger(self):
      return 12345

   def getFloat(self):
      return 12345.0
   
   def getString(self):
      return None

   def _set_int(self,tbd):
      pass
   def setInteger(self,tbd):
      if isinstance(tbd,int):
         self._set_int(tbd)
         return
      if isinstance(tbd,str):
         self._set_int(int(tbd))
         return
      self.setInteger(tbd.getInteger())

   def _set_float(self,tbd):
      pass
   def setFloat(self,tbd):
      if isinstance(tbd,float):
         self._set_float(tbd)
         return
      if isinstance(tbd,str):
         self._set_float(float(tbd))
         return
      self.setFloat(tbd.getFloat())

   def _set_string(self,tbd):
      pass
   def setString(self,tbd):
      if isinstance(tbd,str):
         self._set_string(tbd)
         return
      else:
         self.setString(tbd.getString())

   def getDTDataType(self,*args):
      return None

   def setDTDataType(self,*args):
      pass

   def assign(self,tbd):
      pass

   def getDimension(self):
      return 0
    
   def getDimensioned(self,i):
      return 0

   def multiply(self,other):  return None
   def divide(self,other):  return None
   def add(self,other):  return None
   def subtract(self,other):  return None
   def mod(self,other):  return None
   def round(self,other):  return None
   def truncate(self,other):  return None
    
   def getOne(self,arg=None):
      t = makeone(self.theType,self.scope)
      assert t
      if arg:
         t.assign(arg)
      assert t
      return t
   

   def cloneDTDataType(self):
      return self.getOne(self)
  

   def setDimension(self,*args): pass
   def compareTo(self,o): pass
   def attach(self,theBoss):pass
    

########
class DTDataTypeProxy (DTDataType):
   def __init__(self,theType,scope,dims):
      DTDataType.__init__(self,scope,theType)
      self.dims = dims

   def getType(self):
      return self.theBoss.getType()
   def getInteger(self):
      return self.theBoss.getInteger()
   def getFloat(self):
      return self.theBoss.getFloat()
   def getString(self):
      return self.theBoss.getString()

   def setInteger(self,item):
      theBoss.setInteger(item)
   def setFloat(self,item):
      theBoss.setFloat(item)
   def setString(self,item):
      theBoss.setString(item)

   def getDTDataType(self,*args):
      return self.theBoss.getDTDataType(*args)

   def setDTDataType(self,*args):
      self.theBoss.setDTDataType(*args)

   def assign(self,tbd):
      self.theBoss.assign(tbd)
       
   def getDimension(self,item=None):
      if not item:
         return self.dims
      return self.theBoss.getDimensioned(item)

   def multiply(self,other):
      return self.theBoss.multiply(other)
    
   def divide(self,other):
      return self.theBoss.divide(other)
    
   def add(self,other):
      return self.theBoss.add(other)
    
   def subtract(self,other):
      return self.theBoss.subtract(other)
    
   def mod(self,other):
      return self.theBoss.mod(other)
    
   def round(self,other):
      return self.theBoss.round(other)
    
   def truncate(self,other):
      return self.theBoss.truncate(other)
    
    
   def getOne(self):
      return self.theBoss.getOne()
    
   def compareTo(self, o):
      return self.theBoss.compareTo(o)

   def attach(self,theBoss):
      self.theBoss=theBoss
    
   def cloneDTDataType(self):
      return DTDataTypeProxy(theType,scope,dims)

   def __str__(self):
      return str(self.theBoss)

   toString = __str__


class DTExecException(Exception):
   def __init__(self,s):
      Exception.__init__(self,s)
    

class DTExitModuleException(DTExecException):
   def __init__(self,s):
      DTExecException.__init__(self,s)
 

class DTFunction(DTCodeType):
   def __init__(self,_ttype, entry, cb,scope, args,name):
      DTDataType.__init__(entry,scope,args,name)



class DTFloat(DTDataType):
   def __init__(self,scope,item):
      import basic_p
      DTDataType.__init__(self,scope,basic_p.FLT_VAR)
      self.setFloat(item)

   def _set_int(self,tbd):
      self.s = tbd * 1.0
         
   def _set_float(self,tbd):
      self.s = tbd
    
   def getFloat(self):
      return self.s

   def assign(self,tbd):
      self.setFloat(tbd)

   def multiply(self,other):
      return DTFloat(None,self.getFloat()*other.getFloat())

   def divide(self,other):
      return DTFloat(None,getFloat()/other.getFloat())
                          
   def add(self,other):
      return DTFloat(None,getFloat()+other.getFloat())

   def subtract(self,other):
      return DTFloat(None,getFloat()-other.getFloat())
  
   def mod(self,other):
      return DTFloat(None,getFloat() % other.getFloat())
  
   def round(self):
      return DTInteger(None,DTFloat(None,self.getFloat()+0.5))

   def truncate(self):
      return DTInteger(None,self.getInteger())

   def compareTo(self, o):
      if(getFloat() < (o).getFloat()):
         return -1
      if ( getFloat() > (o).getFloat()):
         return 1
      return 0

   def __str__(self):
      return str(self.s)

   toString = __str__


   
class DTInteger (DTDataType):
   def __init__(self,scope,item):
      import basic_p
      DTDataType.__init__(self,scope,basic_p.INT_VAR)
      self.setInteger(item)
  
   def getInteger(self):
      assert isinstance(self.s,int)
      return self.s

   def _set_int(self,tbd):
      assert isinstance(tbd,int)
      self.s = tbd
    
   def _set_float(self,tbd):
      self.s = float(tbd)

   def getFloat(self):
      return self.s * 1.0

   def assign(self,tbd):
      self.setInteger(tbd)

   def multiply(self,other):
      if isinstance(other, DTFloat):
         t = DTFloat(None,self)
         return t.multiply(other)
      return DTInteger(None,self.getInteger()*other.getInteger())

   def divide(self,other):
      if isinstance(other, DTFloat):
         t = DTFloat(None,self)
         return t.divide(other)
      return DTInteger(None,self.getInteger()/other.getInteger())


   def add(self,other):
      if isinstance(other, DTFloat):
         t = DTFloat(None,self)
         return t.add(other)
      return DTInteger(None,self.getInteger()+other.getInteger())

   def subtract(self,other):
      if isinstance(other, DTFloat):
         t = DTFloat(None,self)
         return t.subtract(other)
      return DTInteger(None,self.getInteger()-other.getInteger())

   def mod(self,other):
      if isinstance(other, DTFloat):
         t = DTFloat(None,self)
         return t.mod(other)
      return DTInteger(None,self.getInteger() % other.getInteger())

   def round(self):
      return self

   def truncate(self):
      return this


   def compareTo(self, o):
      if( self.getInteger() < (o).getInteger()):
         return -1
      if ( self.getInteger() > (o).getInteger()):
         return 1
      return 0

   def __str__(self):
      return str(self.s)

   toString=__str__


class DTString (DTDataType):
   def __init__(self,scope,item):
      import basic_p
      DTDataType.__init__(self,scope,basic_p.STR_VAR)
      self.setString(item)


   def _set_string(self,s):
      self.s=s

   def getString(self):
      return self.s
    
   def compareTo(self, o):
      return s.compareTo(o.getString())
    
   def assign(self,tbd):
      self.setString(tbd)

   def __str__(self):
      return self.s

   toString=__str__


class DTSubroutine(DTCodeType):
   def __init__(self,entry,cb,scope,args,name):
      DTCodeType.__init__(self,entry,cb,scope,args,name)


      
class DTArray1D(DTDataType):
   def __init__(self,_type,scope):
      DTDataType.__init__(self,scope,_type)
      self.data = []
      self.dim1 = 1
      self.base = 0

   def init(self):
      self.data = [] * self.dim1


   def _get(self,idx1):
      assert isinstance(idx1,int)
      t = self.data[idx1]
      return t
   def _set(self,item,idx1):
      assert isinstance(idx1,int)
      self.data[idx1] = item
     
   def getDTDataType(self,i1):
      idx1 = i1.getInteger()-self.base
      if self.dim1==0 :
         self.dim1=10
         self.init()
    
      if idx1>self.dim1:
         return None

      t = self._get(idx1)
      if not t:
         self._set(self.getOne(),idx1)
      return t
    
      

   def setDTDataType(self,i1,s):
      idx1 = i1.getInteger() - self.base
      if self.dim1==0:
         self.dim1 = 10
         self.init()
    
      if(idx1<=self.dim1):
         t = self._get(idx1)
         if not t:
            t = self._set(self.getOne(s),idx1)
         else:
            t.assign(s)
        
    
   def getDimension(self):
      return 1

   def getDimensiond(self,i):
      if i==1:
         return self.dim1
      return 0
  
   def setDimension(self,i1):
      if isinstance(i1,int):
         self.dim1 = i1
         self.init()
         return
      if isinstance(i1,DTInteger):
         self.setDimension(i1.getInteger())
         return
      assert 0
    

   def compareTo(self,o):
      return 0
    


class DTArray2D(DTDataType):
   def __init__(self,_type,scope):
      DTDataType.__init__(self,scope,_type)
      self.data = []
      self.dim1 = 0
      self.dim2 = 0
      self.base = 0
      self._checkdim()
      
   def _checkdim(self):
      assert isinstance(self.dim1,int)
      assert isinstance(self.dim2,int)

   def init(self):
      self._checkdim()
      self.data = [None] * (self.dim2 * self.dim1)

   def _get(self,idx1,idx2):
      assert isinstance(idx1,int)
      assert isinstance(idx2,int)
      t = self.data[idx1*self.dim1+idx2]
      return t
   
   def _set(self,item,idx1,idx2):
      assert isinstance(idx1,int)
      assert isinstance(idx2,int)
      self.data[idx1*self.dim1+idx2] = item
      return self._get(idx1,idx2)

   def getDTDataType(self,i1,i2):
      idx1=i1.getInteger()-self.base
      idx2=i2.getInteger()-self.base
      
      if(self.dim1==0):
         self.dim1=10
         self.dim2=10
         self.init()
      if(idx1>self.dim1):
         return None
      if(idx2>self.dim2):
         return None
      t = self._get(idx1,idx2)
      if not t:
         t = self._set(self.getOne(),idx1,idx2)
      assert t
      return t
    
      

   def setDTDataType(self,i1,i2,s):
      idx1=i1.getInteger()-self.base
      idx2=i2.getInteger()-self.base
      if(self.dim1==0):
         self.dim1=10
         self.dim2=10
         self.init()
        
      if idx1<=self.dim1 and idx2<=self.dim2:
         t = self._get(idx1,idx2)
         if not t:
            t = self._set(self.getOne(s),idx1,idx2)
         else:
            t.assign(s)
      else:
         raise Exception("index out of range:")
        
    
   def getDimension(self):
      return 2

   def getDimensioned(self,i):
      if (i==1):
         return self.dim1
      if (i==2):
         return self.dim2
      return 0

   def _toint(self,item):
      if isinstance(item,int):
         return item
      if isinstance(item,DTInteger):
         return item.getInteger()
      assert 0


   def setDimension(self,i1,i2):
      self.dim1 = self._toint(i1)
      self.dim2 = self._toint(i2)
      self._checkdim()
      self.init()

   def compareTo(self,o):
      return 0
    

class DTArray3D(DTDataType):
   def __init__(self,_type,scope):
      DTDataType.__init__(self,scope,_type)
      self.data = []
      self.dim1 = 0
      self.dim2 = 0
      self.dim3 = 0
      self.base = 0

   def init(self):
      self.data = [None] * (self.dim1 * self.dim2 * self.dim3)

   def _get(self,idx1,idx2,idx3):
      assert isinstance(idx1,int)
      assert isinstance(idx2,int)
      assert isinstance(idx3,int)
      t = self.data[idx1 * (self.dim1*self.dim2) + idx2*self.dim2 + idx3]
      return t
   
   def _set(self,item,idx1,idx2,idx3):
      assert isinstance(idx1,int)
      assert isinstance(idx2,int)
      assert isinstance(idx3,int)
      self.data[idx1 * (self.dim1*self.dim2) + idx2*self.dim2 + idx3] = item

   def getDTDataType(self,i1,i2,i3):
      idx1=i1.getInteger()-self.base
      idx2=i2.getInteger()-self.base
      idx3=i3.getInteger()-self.base
      if not dim1:
         self.dim1 = self.dim2 = self.dim3 =10
         self.init()
      if(idx1>self.dim1):
         return None
      if(idx2>self.dim2):
         return None
      if(idx3>self.dim3):
         return None
      t = self._get(idx1,idx2,idx3)
      if not t:
         t = self._set(self.getOne(),idx1,idx2,idx3)
      return t

    
   def setDTDataType(self,i1,i2,i3,s):
      idx1=i1.getInteger()-self.base
      idx2=i2.getInteger()-self.base
      idx3=i3.getInteger()-self.base
      if(self.dim1==0):
         self.dim1 = self.dim2 = self.dim3 = 10
      self.init()
    
      if(idx1<=self.dim1 and idx2<=self.dim2 and idx3<=self.dim3):
         t= self._get(idx1,idx2,idx3)
         if not t:
            t = self._set(self.getOne(s),idx1,idx2,idx3)
         else:
            t.assign(s)

   def getDimension(self):
      return 3

   def getDimensioned(self,i):
      if(i==1):
         return self.dim1
      if (i==2):
         return self.dim2
      if (i==3):
         return self.dim3
      return 0
  
   def setDimension(self,i1,i2,i3):
      self.dim1 = i1
      self.dim2 = i2
      self.dim3 = i3
      self.init()
    
   def compareTo(self,o):
      return 0

class Scope(object):
   def __init__(self,prev=None):
      self.prev = prev
      self.symbolTable = HashTab()
  
   def cloneScope(self,prev):
      newScope = Scope(prev)
      return newScope

  
   def insertVariable(self,v,t):
      self.symbolTable.put(v.lower(),t)
  
  
   def getVariable(self,v):
      t=self.symbolTable.get(v.lower())
      return t
  
  
   def getVariableDimension(self,v):
      t=self.getVariable(v)
      
      if t:
         return t.getDimension()
      else:
         return 0
      
   def getVariableType(self,v):
      t=self.getVariable(v)
      
      if t:
         return t.getType()
      else:
         return 0
      
   def isArrayVariable(self,s):
      return (self.getVariableDimension(s) > 0)
  
   def getPrev(self):
      return self.prev
    

class FunctionScope(Scope):
   def __init(self,prev):
      Scope.__init__(self,prev)

class GlobalScope(Scope):
   def __init(self,prev=None):
      Scope.__init__(self,prev)


class ProgramScope(Scope):
   def __init(self,prev):
      Scope.__init__(self,prev)
      assert isinstance(self,Scope)

class SubroutineScope(Scope):
   def __init(self,prev):
      Scope.__init__(self,prev)
      
      

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
      self.visit1(k)
      self.visit1(s)
      self.printf(")")

   def visit(self,node):
      self.visit1(node)
      self.printf("\n")
      

if __name__ == "__main__":
   import basic_l
   import basic_p
   import basic_w
   
   L = basic_l.Lexer() 
   P = basic_p.Parser(L)
   P.setFilename(L.getFilename())

   ### Parse the input expression
   C = P.compilationUnit(None)

   
   ast = P.getAST()
   
   if not ast:
      print "stop - no AST generated."
      import sys
      sys.exit(1)

   W = basic_w.Walker()
   W.compilationUnit(ast,C)
