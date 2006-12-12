"""
def foo():
	return [1, 2, 3].Find({ item as int | return (item > 2) })

print(foo())
"""
def foo():
	return [1, 2, 3].Find({ item as int | return (item > 2) })
end

print(foo())
