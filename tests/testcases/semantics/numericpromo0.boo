"""
[System.Runtime.CompilerServices.CompilerGlobalScopeAttribute]
public final transient class Numericpromo0Module(object):

	public static def d(dummy as int) as double:
		if dummy == 0:
			return 3.0
		if dummy == 1:
			return 1L
		return 2

	public static def l(dummy as int) as long:
		if dummy == 0:
			return 1
		return 2L

	private def constructor():
		super()
"""
def d(dummy as int):
	return 3.0 if dummy == 0
	return 1L if dummy == 1
	return 2

def l(dummy as int):
	return 1 if dummy == 0
	return 2L
