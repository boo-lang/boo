"""
FOO BAR
BAR FOO
"""
def ToUpper(item as string):
	return item.ToUpper()

print(join(map(ToUpper, ["foo", "bar"])))
print(join(map(ToUpper, ("bar", "foo"))))
