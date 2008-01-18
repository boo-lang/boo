"""
I have compiled successfully
"""
import System.Runtime.InteropServices

class Test:
	def constructor():
		#exit(1)
		pass

	[DllImport("libc")]
	private static def exit(status as int):
		pass

print "I have compiled successfully"
