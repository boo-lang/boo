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

class GameObject:
	pass
	
class Ship(GameObject):
	pass
	
class Missile(GameObject):
	pass
	
class CollisionHandler:
"""
Takes advantage of runtime dispatching on all arguments
to handle collisions between concrete game objects.
"""
	def handle(ship as Ship, missile as Missile):
		print "a ship collided with a missile"
		
	def handle(missile as Missile, ship as Ship):
		print "a missile collided with a ship"
		
	def handle(missile1 as Missile, missile2 as Missile):
		print "a missile collided with another missile"
		
	def handle(ship1 as Ship, ship2 as Ship):
		print "a ship collided with another ship"
		
def gameLoop():	
	
	objects = [Ship(), Missile(), Missile(), Ship(), Missile()]
	
	random = System.Random()
	select = def:
		return objects[random.Next(len(objects))]
	
	# duck enables runtime dispatch
	handler as duck = CollisionHandler()	
	while true:
		handler.handle(select(), select())
		yield
		
for _ in zip(range(10), gameLoop()):
	pass
