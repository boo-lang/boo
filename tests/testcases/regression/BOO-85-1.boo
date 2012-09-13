import System.IO


def PathJoin(files):
	buffer = System.Text.StringBuilder()
	for fname in files:
		buffer.Append(fname)
		buffer.Append(Path.PathSeparator)
	return buffer.ToString()[:-1]

files = "foo.bar", "spam.eggs"
separator = string((Path.PathSeparator,))
assert PathJoin(files) == join(files, separator)
