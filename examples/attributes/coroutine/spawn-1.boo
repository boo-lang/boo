"""RANDOM SCHEDULING
   the coroutines will be executed in any order, but they will still run their
   sliceshare at each round. Output could be :
id:2 / count:1
id:5 / count:1
id:6 / count:1
id:0 / count:1  
id:0 / count:2
...
id:2 / count:10
bye!
"""

import System
import System.Collections.Generic
import Coroutine


class CountToTen(ISpawnable):
	[property(Id)]
	_id as int
		
	#look ma! no yield!
	#if you want control over the placement of yields, just apply [coroutine] as usual
	def Execute() as bool:
		for i in range(1, 11):
			print "id:${_id} / count:${i}"



class LotteryCoroutineScheduler(ICoroutineScheduler,IComparer of ISpawnable):

	_lock_IsRunning = object()

	IsRunning as bool:
		get:
			lock _lock_IsRunning:
				return _isRunning
	_isRunning = false
	
	_rand as Random

	def Compare(a as ISpawnable, b as ISpawnable) as int:
		return _rand.Next(-10, 10)

	def JoinStart():
		lock _lock_IsRunning:
			if _isRunning:
				raise InvalidOperationException("Scheduler is already running.")
			_isRunning = true
		_rand = Random()
		coroutines = CoroutineSchedulerManager.Coroutines #FIXME: sync copy
		slices = CoroutineSchedulerManager.Slices
		try:
			:runSlices
			toRemove as List[of ISpawnable] = null
			while 0 != coroutines.Count:
				coroutines.Sort(self)
				for c in coroutines:
					s = slices[c]
					try:
						if 1 == s:
							c.Execute()
						else:
							for i in range(0, s):
								c.Execute()
					except e as CoroutineTerminatedException:
						if toRemove is null:
							toRemove = List[of ISpawnable]()
						toRemove.Add(c)
					except e as CoroutineFutureNotReadyException:
						pass
				if toRemove is not null:
					for c in toRemove:
						coroutines.Remove(c)
					goto runSlices
		ensure:
			lock _lock_IsRunning:
				_isRunning = false


spawn CountToTen(Id:0), 2
for i in range(1, 11):
	spawn CountToTen(Id:i)

CoroutineSchedulerManager.Scheduler = LotteryCoroutineScheduler()
spawn

print "bye!"
