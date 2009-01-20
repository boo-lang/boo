"""
Foo.Bar
"""
namespace Foo.Bar

import Boo.Lang.Compiler.Ast

macro printNamespace:

	namespaceName = printNamespace.GetAncestor[of Module]().Namespace.Name.ToString()

	yield [| print $namespaceName |]
	
macro producePrinter:
	
	yield [|
		printNamespace
	|]
	
producePrinter
