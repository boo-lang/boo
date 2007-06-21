"""
"""
namespace NS

class Outer:
	protected _bar
	private _baz
	public bang
	
	class Inner:
		def constructor(outer as Outer):
			print outer._baz
			print outer._bar
			print outer.bang
