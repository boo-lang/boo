import NUnit.Framework

class Task:

	[getter(Name)]
	_name as string
	
	[getter(Done)]
	_done as bool
	
	def constructor(name, done):
		_name = name
		_done = done
		
	override def ToString():
		return _name
		
class TaskList:

	_tasks = []
	
	def Add([required] task as Task):
		_tasks.Add(task)
		
	def GetDone():
		return task for task as Task in _tasks if task.Done
		
	def GetToDo():
		return task for task as Task in _tasks unless task.Done		
		
list = TaskList()
list.Add(Task("generator expressions", false))
list.Add(Task("classes", true))
list.Add(Task("generator methods", false))
list.Add(Task("mixins", false))
list.Add(Task("attributes", true))

Assert.AreEqual("classes, attributes", join(list.GetDone(), ", "))
Assert.AreEqual("generator expressions, generator methods, mixins", join(list.GetToDo(), ", "))
