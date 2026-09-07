"""
1
0
then
never entered
then
"""
import System.Threading.Tasks

class Counter:
	public n as int

	[async] def StepAsync() as Task[of bool]:
		await Task.Delay(1)
		n--
		return n >= 0

[async] def DrainAsync(start as int) as Task:
	c = Counter(n: start)
	while await(c.StepAsync()):
		print c.n
	or:
		print "never entered"
	then:
		print "then"

DrainAsync(2).Wait()
DrainAsync(0).Wait()
