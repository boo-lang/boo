#region license
// boo - an extensible programming language for the CLI
// Copyright (C) 2004 Rodrigo Barreto de Oliveira
//
// This library is free software; you can redistribute it and/or
// modify it under the terms of the GNU Lesser General Public
// License as published by the Free Software Foundation; either
// version 2.1 of the License, or (at your option) any later version.
// 
// This library is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
// Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public
// License along with this library; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA 02111-1307 USA
// 
// Contact Information
//
// mailto:rbo@acm.org
#endregion

import System
import System.Xml.Serialization from System.Xml
import System.IO

class GameSession:

	public Name as string
	
	def constructor():
		pass
		
	def constructor(name as string):
		Name = name
	
class GameLogData:
	
	_sessions = []
	
	Sessions as (GameSession):
		get:
			return _sessions.ToArray(GameSession)
		set:
			_sessions.Clear()
			_sessions.Extend(value)
			
	def Add([required] session as GameSession):
		_sessions.Add(session)
	
data = GameLogData()
data.Add(GameSession("Foo"))
data.Add(GameSession("Bar"))

serializer = XmlSerializer(GameLogData)
serializer.Serialize(Console.Out, data)
