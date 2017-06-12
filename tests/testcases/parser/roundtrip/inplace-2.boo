"""
namespace Pythia.Runtime

import Boo.Lang.Compiler.Ast

[Meta]
def inc(x as Expression) as Expression:
	return [|++$x|]

[Meta]
def inc(x as Expression, operand as Expression) as Expression:
	return [|$x += $operand|]

[Meta]
def dec(x as Expression) as Expression:
	return [|--$x|]

[Meta]
def dec(x as Expression, operand as Expression) as Expression:
	return [|$x -= $operand|]
"""
namespace Pythia.Runtime

import Boo.Lang.Compiler.Ast

[Meta]
def inc(x as Expression) as Expression:
	return [|++$x|]

[Meta]
def inc(x as Expression, operand as Expression) as Expression:
	return [|$x += $operand|]

[Meta]
def dec(x as Expression) as Expression:
	return [|--$x|]

[Meta]
def dec(x as Expression, operand as Expression) as Expression:
	return [|$x -= $operand|]
