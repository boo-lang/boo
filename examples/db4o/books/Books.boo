#region license
// Copyright (c) 2003, 2004, Rodrigo B. de Oliveira (rbo@acm.org)
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
This example shows how to use db4o 3.0 (http://www.db4o.com), a great
oodbm infrastructure, to implement a simple Book/Authors application.
"""
import System
import System.IO
import com.db4o

class Author:
	
	[property(Name)]
	_name as string
	
	[property(Age)]
	_age as int
	
	override def ToString():
		return "${_name} (${_age})"
	
class Book:
	
	[property(Author)]
	_author as Author
	
	[property(Name)]
	_name as string	
	
	override def ToString():
		return "${_name} by ${_author}"
	
class Application:
	
	_container as ObjectContainer
	_continue = true
	
	def run():
		
		options = {
			"na" : newAuthor,
			"nb" : newBook,
			"la" : listAuthors,
			"lb" : listBooks,
			"y" : booksByYoungAuthors,
			"q" : quit
		}
		
		_container = Db4o.openFile("books.yap")
		
		while _continue:			
		
			print
			print "(na) new author, (nb) new book"
			print "(la) list authors, (lb) list books"
			print "(y) books by youngsters, (q)uit"
			choice = prompt("your choice? ")
			print
			
			action = options[choice]
			if action is null:
				print choice, "is not a valid choice."
			else:
				(action as callable)()
				
		_container.close()		
		
	def quit():
		_continue = false
	
	def newAuthor():
		_container.set(
			Author(
				Name: prompt("author's name: "),
				Age: int.Parse(prompt("author's age: "))))
			
	def newBook():
		q = _container.query()
		q.constrain(Author)
	
		authors = List(iterate(q.execute()))
		if 0 == len(authors):
			print "You must record an author first."
			return
			
		for index, author as Author in enumerate(authors):
			print index, ")", author.Name
			
		author = authors[int.Parse(prompt("Select the book's author: "))]
		_container.set(
			Book(
				Author: author,
				Name: prompt("book's name: ")))
		
	def booksByYoungAuthors():
		q = _container.query()
		q.constrain(Book)
		q.descend("_author").descend("_age").constrain(30).smaller()
		display(q.execute())
		
	def listBooks():
		q = _container.query()
		q.constrain(Book)
		display(q.execute())
		
	def listAuthors():
		q = _container.query()
		q.constrain(Author)
		display(q.execute())
		
	def display(objects as ObjectSet):
		for o in iterate(objects):
			print o

	def iterate(objects as ObjectSet):
		while objects.hasNext():
			yield objects.next()

Application().run()
		
