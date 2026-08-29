"""
"""
i = 42
code = [| $i |]
literal as Boo.Lang.Compiler.Ast.IntegerLiteralExpression = code
assert i == literal.Value
assert not literal.IsLong
