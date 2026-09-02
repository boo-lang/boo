namespace BooCompiler.Tests.SupportingClasses

class ThisReturnTypeIsAttribute(System.Attribute):
	private _what as string

	def constructor(what as string):
		What = what

	public What as string:
		get: return _what
		set: _what = value
