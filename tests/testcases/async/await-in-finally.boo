"""
body
finally
done
finally
boom
"""
import System
import System.Threading.Tasks

[async] def NoThrowAsync() as Task:
	try:
		print "body"
	ensure:
		await Task.Delay(1)
		print "finally"

[async] def ThrowAsync() as Task:
	try:
		raise InvalidOperationException("boom")
	ensure:
		await Task.Delay(1)
		print "finally"

NoThrowAsync().Wait()
print "done"
try:
	ThrowAsync().Wait()
except e as AggregateException:
	print e.InnerException.Message
