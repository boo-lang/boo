"""
test passed
"""
import System
import System.Runtime.InteropServices

obj = object()

//succeeds:
ptr as IntPtr = cast(IntPtr, GCHandle.Alloc( obj ))

//previously failed:
gch = cast(GCHandle, ptr)

//Should fail because op_Explicit requires explicit cast:
//Tested in ../errors/BCE0022-10.boo
//gch2 as GCHandle = ptr

gch.Free()

print "test passed"

