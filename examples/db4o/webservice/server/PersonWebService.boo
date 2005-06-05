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


namespace BooWebService.Server

import System.Web.Services

class Person:
"""
The application is built around plain objects.
"""
	[property(Name)]
	_name as string
	
	def constructor(name as string):
		_name = name
		

class PersonData:
"""
The world talks to the application through data objects
which assemble the data and the db4o id together.
"""	
	public Id as long
	
	public Name as string
	
	
[WebService]
class PersonWebServiceImpl:

	[WebMethod]
	def Save(data as PersonData):
	"""
	Updates an existing Person or creates a new one.
	"""
		container = PersonApplication.OpenSession()
		try:
			person as Person = container.ext().getByID(data.Id)
			if person is null:
				person = Person(data.Name)
			else:
				person.Name = data.Name
			container.set(person)
		ensure:
			container.close()
			
	[WebMethod]
	def QueryAll() as (PersonData):
	"""
	Returns all existing Person instances.
	"""
		container = PersonApplication.OpenSession()
		try:
			data = []
			os = container.get(Person)
			while os.hasNext():
				person as Person = os.next()
				data.Add(
					PersonData(
						Id: container.ext().getID(person),
						Name: person.Name))
			return data.ToArray(PersonData)
		ensure:
			container.close()
