namespace BooCompiler.Tests.SupportingClasses

class ImplicitConversionToDouble:
	public Value as double

	def constructor(value as double):
		self.Value = value

	public static def op_Implicit(o as ImplicitConversionToDouble) as double:
		return o.Value
