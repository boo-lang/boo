class Task:

	[property(Done)]
	_done = false
	
	[property(Name)]
	_name = ""
	
	override def ToString():
		return _name
	
class TaskList:
	
	_tasks = []
	
	def Add([required] task as Task):
		_tasks.Add(task)
	
	def GetTasks(done as bool):
		for task as Task in _tasks:
			yield task if done == task.Done

tl = TaskList()
tl.Add(Task(Name: "task 1"))
tl.Add(Task(Name: "task 2", Done: true))
tl.Add(Task(Name: "task 3"))
tl.Add(Task(Name: "task 4"))
tl.Add(Task(Name: "task 5", Done: true))

assert "task 1, task 3, task 4" == join(tl.GetTasks(false), ", ")
assert "task 2, task 5" == join(tl.GetTasks(true), ", ")


