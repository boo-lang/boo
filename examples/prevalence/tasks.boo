import System
import System.Reflection
import Bamboo.Prevalence

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
		
def ResolveAssembly(sender, args as ResolveEventArgs) as Assembly:
	if args.Name.StartsWith("Bamboo.Prevalence"):
		return typeof(PrevalenceEngine).Assembly
	if args.Name.StartsWith("tasks"):
		return Assembly.GetExecutingAssembly()
	return null
	
AppDomain.CurrentDomain.AssemblyResolve += ResolveAssembly
		
engine = PrevalenceActivator.CreateTransparentEngine(TaskList, "c:\\temp\\data")
tasks as TaskList = engine.PrevalentSystem

for task in tasks.Tasks:
	print(task.Name)

name = prompt("Nova task: ")
tasks.Add(Task(name)) if len(name) > 0

		
	
		

