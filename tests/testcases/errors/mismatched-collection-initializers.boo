"""
mismatched-collection-initializers.boo(7,30): BCE0017: The best overload for the method 'System.Collections.Generic.List[of string].Add(string)' is not compatible with the argument list '(string, string)'.
mismatched-collection-initializers.boo(8,39): BCE0017: The best overload for the method 'System.Collections.Generic.Dictionary[of string, string].Add(string, string)' is not compatible with the argument list '(string)'.
"""
import System.Collections.Generic

l = List[of string]() { "foo": "bar" }
h = Dictionary[of string, string]() { "foo" } 
