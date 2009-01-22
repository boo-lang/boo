"""
System.Collections.Generic
"""
macro defaultImports:
	yield [| import System |]
	yield [| import System.Collections.Generic |]
	
defaultImports
defaultImports # shouldn't make a difference
Console.WriteLine(typeof(List[of*]).Namespace)
