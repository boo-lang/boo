import NUnit.Framework

class Clerk:
	def punch():
		pass
		
address = __addressof__(Clerk.punch)
Assert.AreSame(System.IntPtr, address.GetType())
Assert.AreEqual(address, typeof(Clerk).GetMethod("punch").MethodHandle.GetFunctionPointer())
