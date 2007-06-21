"""
BCE0022-10.boo(13,20): BCE0022: Cannot convert 'System.IntPtr' to 'System.Runtime.InteropServices.GCHandle'.
"""
import System
import System.Runtime.InteropServices

obj = object()

//succeeds:
ptr as IntPtr = cast(IntPtr, GCHandle.Alloc( obj ))

//should fail because op_Explicit requires explicit cast:
gch2 as GCHandle = ptr

gch2.Free()

print "test should have failed"

