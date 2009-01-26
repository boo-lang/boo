"""
"""
namespace NS

class Outer:
	protected _bar = null
	private _baz = null
	public bang = null
	
	class Inner:
		def constructor(outer as Outer):
			print outer._baz
			print outer._bar
			print outer.bang
