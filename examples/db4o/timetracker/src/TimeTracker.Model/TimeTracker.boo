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

namespace TimeTracker.Model

import System
import System.IO
import com.db4o

class Project:

	[property(Name, value is not null)]
	_name as string
	
	override def ToString():
		return _name
	
class Task:
	
	[property(Name, value is not null)]
	_name as string
	
	[property(Project)]
	_project as Project
	
	override def ToString():
		return "'${_name}' (${_project})"
	
class Activity(IComparable):
	
	[property(Task)]
	_task as Task
	
	[property(Notes, value is not null)]
	_notes = ""
	
	[property(Started)]
	_started as date
	
	[property(Finished)]
	_finished as date
	
	Elapsed:
		get:
			return _finished - _started
			
	override def ToString():
		return "${Elapsed} on ${_task}"
		
	def CompareTo(other) as int:
		return _started.CompareTo((other as Activity).Started)
			
class TimeTrackerSystem(IDisposable):
	
	_container as ObjectContainer
	
	def constructor([required] fname as string):
		_container = Db4o.openFile(fname)
		
	Projects as (Project):
		get:
			return QueryAll(Project)
			
	Tasks as (Task):
		get:
			return QueryAll(Task)
			
	Activities as (Activity):
		get:
			return QueryAll(Activity)
			
	private def QueryAll(type as Type):
		q = _container.query()
		q.constrain(type)
		return array(type, iterate(q.execute()))
		
	private def QueryActivities(q as com.db4o.query.Query):
		q.descend("_started").orderDescending()
		return array(Activity, iterate(q.execute()))

	def QueryTasks(project as Project):
		q = _container.query()
		q.constrain(Task)
		q.descend("_project").constrain(project)
		return array(Task, iterate(q.execute()))

	def AddProject([required] project as Project):
		_container.set(project)
		_container.commit()
		return GetId(project)
		
	def AddTask([required] task as Task):
		assert task.Project is not null
		assert task.Project is GetExisting(task.Project)
		
		_container.set(task)
		_container.commit()
		
		return GetId(task)
		
	def QueryActivities([required] task as Task):
		q = _container.query()
		q.constrain(Activity)		
		q.descend("_task").constrain(task)
		return QueryActivities(q)
		
	def QueryProjectActivities([required] p as Project):
		q = _container.query()
		q.constrain(Activity)
		q.descend("_task").descend("_project").constrain(p)
		return QueryActivities(q)
		
	def QueryDayActivities(day as date):
		q = _container.query()
		q.constrain(Activity)
		
		d = day.Date 
		q.descend("_started").constrain(d).greater()
		q.descend("_started").constrain(d+1d).smaller()
		return QueryActivities(q)
		
	def QueryTotalProjectActivity([required] p as Project):
		return CalcTotalActivity(QueryProjectActivities(p))
		
	def QueryTotalDayActivity(day as date):
		return CalcTotalActivity(QueryDayActivities(day))
		
	def CalcTotalActivity(activities as (Activity)):
		elapsed = TimeSpan.Zero
		for a in activities:
			elapsed += a.Elapsed
		return elapsed
		
	def AddActivity([required] session as Activity):
		assert session.Task is GetExisting(session.Task)
		_container.set(session)
		return GetId(session)
		
	def GetExisting(obj):
		return GetObject(GetId(obj))
		
	def GetId(obj):
		return _container.ext().getID(obj)
		
	def GetObject(id as long):
		return _container.ext().getByID(id)
		
	def Dispose():
		_container.close()
		
	def iterate(os as ObjectSet):
		while os.hasNext():
			yield os.next()
	
	
