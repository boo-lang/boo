using reader=File.OpenText(fname):
	print(reader.ReadLine())

try:
	reader = File.OpenText(fname)
	print(reader.ReadLine())
ensure:
	 if (__disposable__ = (reader as System.IDisposable))
		__disposable__.Dispose()
		__disposable__ = null
 	reader = null
	
