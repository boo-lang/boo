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

import System
import Bamboo.Prevalence from Bamboo.Prevalence

class Task:

	[getter(Id)]
	_id = -1
	
	[getter(DateCreated)]
	_dateCreated = date.Now
	
	[getter(Summary)]
	_summary as string
	
	[property(Done)]
	_done = false

	def constructor([required] summary):
		_summary = summary
		
	internal def Initialize(id):
		_id = id

class TaskList(MarshalByRefObject):

	_tasks = []
	
	_nextId = 0
	
	Tasks:
		get:
			return array(Task, _tasks)
			
	PendingTasks:
		get:
			return array(task for task as Task in _tasks unless task.Done)
	
	def Add([required] task as Task):
		task.Initialize(++_nextId)
		_tasks.Add(task)
		
	def MarkDone(id as int):
		for task as Task in _tasks:
			if id == task.Id:
				task.Done = true
				break

def Menu(message as string, options as Hash):
	
	choice = prompt(message).ToLower()
	selected as callable = options[choice]
	if selected:
		selected()
	else:
		print("'${choice}' is not a valid choice")


def ShowTasks(tasks as (Task)):
	print("id\tDate Created\t\tSummary")
	for task in tasks:
		print("${task.Id}\t${task.DateCreated}\t\t${task.Summary}")
		
engine = PrevalenceActivator.CreateTransparentEngine(TaskList, "c:\\temp\\data")
system as TaskList = engine.PrevalentSystem

options = {
	"a" : { system.Add(Task(prompt("summary: "))) },
	"d" : { system.MarkDone(int.Parse(prompt("task id: "))) },
	"s" : engine.TakeSnapshot,
	"q" : { Environment.Exit(-1) }
}

while true:
	ShowTasks(system.PendingTasks)
	Menu("(A)dd task\t(D)one with task\t(S)napshot\t(Q)uit\nyour choice: ",
		options)

	
