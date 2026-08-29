"""
BCE0154-2.boo(9,31): BCE0154: 'System.Runtime.InteropServices.MarshalAsAttribute' cannot be applied multiple times on the same target.
BCE0154-2.boo(9,73): BCE0154: 'System.Runtime.InteropServices.MarshalAsAttribute' cannot be applied multiple times on the same target.
"""
import System.Runtime.InteropServices

class Test:
	[DllImport("libc")]
	private static def Dummy([MarshalAs(UnmanagedType.CustomMarshaler)][MarshalAs(UnmanagedType.CustomMarshaler)] s as string):
		pass

