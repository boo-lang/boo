"""
Description:
	TraceService class is the compiler service used by `trace` macro.
	Read `trace.boo` for more information.
"""

import System
import System.IO
import Boo.Lang.Compiler
import Boo.Lang.Compiler.Ast


class TraceService (IDisposable):
	context as CompilerContext
	locations = List[of string]()

	def constructor(context as CompilerContext):
		.context = context

	def AddLocation(lexicalInfo as LexicalInfo, method as Method):
		locations.Add("${lexicalInfo.ToString()} : ${method.ToString()}")

	def Dispose():
		if len(context.Errors):
			return #do not write file if there was an error during compilation

		output = context.Parameters.OutputAssembly
		return unless output #do not write file if there no output assembly (eg. compilation in memory)

		print "NOTICE: writing trace locations to file `${output}.traces`"
		using writer = StreamWriter(output+".traces"):
			for location in locations:
				writer.WriteLine(location)

