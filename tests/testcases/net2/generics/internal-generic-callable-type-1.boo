callable GenericDelegate[of TIn, TOut](argument as TIn) as TOut
assert typeof(GenericDelegate[of int, string]) == typeof(GenericDelegate[of *, *]).MakeGenericType(int, string)



