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

namespace TimeTracker.Model.Tests

import System
import System.IO
import NUnit.Framework
import TimeTracker.Model

[TestFixture]
class TimeTrackerSystemTestFixture:

	_system as TimeTrackerSystem
	
	[SetUp]
	def SetUp():		
		_system = TimeTrackerSystem(GetDataFileName())
		
	def RestartSystem():
		_system.Dispose()
		_system = TimeTrackerSystem(GetDataFileName())		
		
	[Test]
	def Projects():
		
		assert 0 == len(_system.Projects)
		
		project = Project(Name: "DC")		
		id = _system.AddProject(project)
		
		assert 1 == len(_system.Projects)
		assert project is _system.Projects[0]
		
		RestartSystem()
		
		assert 1 == len(_system.Projects)		
		assert project.Name == _system.Projects[0].Name
		assert id == _system.GetId(_system.Projects[0])
		
	[Test]
	def Tasks():
		
		project = Project(Name: "DC")
		projectId = _system.AddProject(project)		
		taskId = _system.AddTask(
					Task(Name: "Cool Task",
						Project: project))
		
		RestartSystem()
		
		project = _system.GetObject(projectId)
		tasks = _system.QueryTasks(project)
		assert 1 == len(tasks)
		
		task = tasks[0]
		assert 1 == len(_system.Tasks)
		assert task is _system.Tasks[0]
		assert taskId == _system.GetId(task)
		assert "Cool Task" == task.Name
		assert task.Project is project
		
	[Test]
	def Activities():
		
		p1 = Project(Name: "DC")
		_system.AddProject(p1)
		
		p2 = Project(Name: "boo")
		_system.AddProject(p2)
		
		t1 = Task(Name: "Cool Task", Project: p1)
		_system.AddTask(t1)
		
		t2 = Task(Name: "refactoring", Project: p2)
		_system.AddTask(t2)
		
		t3 = Task(Name: "website", Project: p1)
		_system.AddTask(t3)
		
		a1 = Activity(Task: t1,
					Started: date.Now,
					Finished: date.Now + 2h)
								
		a2 = Activity(Task: t1, 
					Started: date.Now - 1d - 3h,
					Finished: date.Now - 1d)
								
		a3 = Activity(Task: t3,
					Started: date.Now + 1s,
					Finished: date.Now + 2h)
					
		a4 = Activity(Task: t2,
					Started: date.Now + 1d,
					Finished: date.Now + 1.5d)
								
		for s in a1, a2, a3, a4:
			_system.AddActivity(s)

		assert _system.QueryActivities(t1) == (a1, a2)		
		assert _system.QueryActivities(t2) == (a4,)		
		assert _system.QueryActivities(t3) == (a3,)
		
		assert _system.QueryProjectActivities(p1) == (a3, a1, a2)
		assert _system.QueryProjectActivities(p2) == (a4,)
		
		assert _system.QueryTotalProjectActivity(p1) == (a3.Elapsed +
														a1.Elapsed +
														a2.Elapsed)
														
		assert _system.QueryTotalProjectActivity(p2) == a4.Elapsed
		
		assert _system.QueryDayActivities(date.Today-1d) == (a2,)
		assert _system.QueryDayActivities(date.Today) == (a3, a1)
		assert _system.QueryDayActivities(date.Today+1d) == (a4,)
		
		
	[TearDown]
	def TearDown():	
		_system.Dispose()
		File.Delete(GetDataFileName())
		
	def GetDataFileName():
		return Path.Combine(Path.GetTempPath(), "timetracker.yap")
		
