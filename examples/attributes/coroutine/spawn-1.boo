#region license
// Copyright (c) 2003, 2004, 2005 Rodrigo B. de Oliveira (rbo@acm.org)
// All rights reserved.
// 
// Redistribution and use in source and binary forms, with or without modification,
// are permitted provided that the following conditions are met:
// 
//     * Redistributions of source code must retain the above copyright notice,
//     this list of conditions and the following disclaimer.
//     * Redistributions in binary form must reproduce the above copyright notice,
//     this list of conditions and the following disclaimer in the documentation
//     and/or other materials provided with the distribution.
//     * Neither the name of Rodrigo B. de Oliveira nor the names of its
//     contributors may be used to endorse or promote products derived from this
//     software without specific prior written permission.
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
// ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
// DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE
// FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
// DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
// SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER
// CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
// OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF
// THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
#endregion


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
