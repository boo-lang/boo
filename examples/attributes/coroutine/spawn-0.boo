"""
id:0 / count:1  
id:0 / count:2  << id0 light-thread will count twice faster than the others!
id:1 / count:1
id:2 / count:1
...
id:10 / count:1
id:0 / count:3
id:0 / count:4
id:1 / count:2
id:2 / count:2
...
id:9 / count:10
id:10 / count:10
bye!
"""

import Coroutine


class CountToTen(ISpawnable):
	[property(Id)]
	_id as int
		
	#look ma! no yield!
	#if you want control over the placement of yields, just apply [coroutine] as usual
	def Execute() as bool:
		for i in range(1, 11):
			print "id:${_id} / count:${i}"


spawn CountToTen(Id:0), 2  #spawn the first coroutine with a double scheduler slice
for i in range(1, 11):     #we could spawn thousands of threads here without pain
	spawn CountToTen(Id:i)
spawn #with no argument, it will execute the spawned coroutines and block until all
      #coroutines have terminated.
print "bye!"
