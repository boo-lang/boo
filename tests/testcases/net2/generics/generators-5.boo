import System
import System.Collections.Generic

def GetTypeName(t as Type):
   return t.Name

l = typeof(List of *)
a = array(GetTypeName(t) for t in l.GetInterfaces())
assert a isa (string)
