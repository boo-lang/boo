namespace NAnt.SampleTask

using NAnt.Core
using NAnt.Core.Attributes

[TaskName("usertask")]
class TestTask(Task):
	
	_message as string
	
	[TaskAttribute("message", Required: true)]
	Message:
		get:
			return _message
		set:
			_message = value
			
	protected def ExecuteTask():
		self.Log(Level.Info, "${LogPrefix}${_message.ToUpper()}")
