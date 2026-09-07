"""
finally
42
finally
done
inner finally
outer finally
7
"""
import System.Threading.Tasks

[async] def ValueAsync() as Task[of int]:
	try:
		return 42
	ensure:
		await Task.Delay(1)
		print "finally"

[async] def VoidAsync() as Task:
	try:
		return
	ensure:
		await Task.Delay(1)
		print "finally"

[async] def NestedAsync() as Task[of int]:
	try:
		try:
			return 7
		ensure:
			await Task.Delay(1)
			print "inner finally"
	ensure:
		await Task.Delay(1)
		print "outer finally"

print ValueAsync().Result
VoidAsync().Wait()
print "done"
print NestedAsync().Result
