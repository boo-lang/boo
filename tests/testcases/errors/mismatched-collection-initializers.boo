"""
mismatched-collection-initializers.boo(7,30): BCE0017: The best overload for the method 'System.Collections.Generic.List.Add(string)' is not compatible with the argument list '(string, string)'.
mismatched-collection-initializers.boo(8,39): BCE0017: The best overload for the method 'System.Collections.Generic.Dictionary.Add(string, string)' is not compatible with the argument list '(string)'.
"""
import System.Collections.Generic

l = List[of string]() { "foo": "bar" }
h = Dictionary[of string, string]() { "foo" } 
