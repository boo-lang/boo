import System
import Bamboo.Prevalence from Bamboo.Prevalence

class Task:

	[getter(ID)]
	_id = Guid.NewGuid()
	
	[getter(Name)]
	_name as string

	def constructor(name):
		_name = name

		
class TaskList(MarshalByRefObject):

	_tasks = []
	
	Tasks as (Task):
		get:
			return _tasks.ToArray(Task)
	
	def Add([required] task as Task):
		_tasks.Add(task)
		
engine = PrevalenceActivator.CreateTransparentEngine(TaskList, "c:\\temp\\data")
tasks as TaskList = engine.PrevalentSystem

for task in tasks.Tasks:
	print(task.Name)

name = prompt("Nova task: ")
tasks.Add(Task(name)) if len(name) > 0

		
	
		

