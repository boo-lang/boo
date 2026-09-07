"""
2
1
0
"""
import System.Threading.Tasks

class Counter:
	public n = 3

	[async] def StepAsync() as Task[of bool]:
		await Task.Delay(1)
		n--
		return n >= 0

[async] def DrainAsync(c as Counter) as Task:
	while await(c.StepAsync()):
		print c.n

counter = Counter()
DrainAsync(counter).Wait()
