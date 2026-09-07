"""
modifier ran
modifier skipped
block ran
block skipped
once: 1
"""
import System.Threading.Tasks

class Picker:
	public calls as int

	[async] def PickAsync(value as bool) as Task[of bool]:
		await Task.Delay(1)
		calls++
		return value

[async] def ModifierAsync(value as bool) as Task[of string]:
	return "modifier ran" unless await(Picker().PickAsync(value))
	return "modifier skipped"

[async] def BlockAsync(value as bool) as Task[of string]:
	unless await(Picker().PickAsync(value)):
		return "block ran"
	return "block skipped"

[async] def OnceAsync(p as Picker) as Task:
	return unless await(p.PickAsync(true))

print ModifierAsync(false).Result
print ModifierAsync(true).Result
print BlockAsync(false).Result
print BlockAsync(true).Result

counted = Picker()
OnceAsync(counted).Wait()
print "once: ${counted.calls}"
