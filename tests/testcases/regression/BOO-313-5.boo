import System.Globalization

class Foo:

	public static IC = CultureInfo.InvariantCulture
	
assert Foo.IC.Name == CultureInfo.InvariantCulture.Name
