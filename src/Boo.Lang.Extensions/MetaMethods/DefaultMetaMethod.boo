namespace Boo.Lang.Extensions

import System
import Boo.Lang.Compiler.Ast

[Meta]
def Default(t as Expression) as Expression:
	return [|__default__($t)|]