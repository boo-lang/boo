#region license
// Copyright (c) 2004, Rodrigo B. de Oliveira (rbo@acm.org)
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

class Vector3:
	_x as double
	_y as double
	_z as double

	def constructor():
		r = System.Random()
		_x = r.Next()
		_y = r.Next()
		_z = r.Next()

	def Distance(other as Vector3):
		dx = _x - other._x
		dy = _y - other._y
		dz = _z - other._z
		return System.Math.Sqrt(dx*dx+dy*dy+dz*dz)
		#return 1.0
		
def createArray(count as int):
	a = array(Vector3, count)
	for i in range(count):
		a[i] = Vector3()
	return a

// array as (Vector3) = array(Vector3() for i in range(length))
a = createArray(25000)

start = date.Now

total = 0.0
count = 0

for v1 in a:
	for v2 in a:
		total += v2.Distance(v1)
		++count

elapsed = date.Now - start 
print("Total... ${total}.")

// a good ips value is: 13.000.000
print("Done ${count} in ${elapsed.TotalSeconds} secs - ${count/elapsed.TotalSeconds} ips.")
