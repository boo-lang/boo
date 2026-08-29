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
