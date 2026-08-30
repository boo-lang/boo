"""
BCW0014-3.boo(17,13): BCW0014: WARNING: Private property 'Test.NeverUsed' is never used.
"""
class Test:
	_value = 0

	private ReadOnly as int:
		get:
			return 1

	private ReadWritten as int:
		get:
			return _value
		set:
			_value = value

	private NeverUsed as int:
		get:
			return 3

	public def Use() as int:
		ReadWritten = 7
		return ReadOnly + ReadWritten
