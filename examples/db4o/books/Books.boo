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
		
