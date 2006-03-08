namespace Boo.Lang.Useful.Functional.Tests

import System
import Boo.Lang.Useful.Functional
import NUnit.Framework
[TestFixture]
class AnyAllTestFixture:
	structs = [TestStruct(3), TestStruct(2), TestStruct(4), TestStruct(0), TestStruct(4)]
	[Test]
	def Any():
		list = [0, 1, 2, 3]
		obj = any(list)		
		assert obj > 2		
		assert len(obj.Passed) == 1
		assert obj < 1
		assert obj == 1
		assert obj == 2
		assert obj <= 1
		assert len(obj.Passed) == 2
		list = ['hello', 'world']
		obj = any(list)
		assert obj =~ 'hello'
		assert len(obj.Passed) == 1
		assert obj
		
		obj = any(structs).width	
		assert obj > 2		
		assert obj
		assert len(obj.Passed) == 3
	[Test]
	def All():
		list = [1, 2, 3]
		assert all(list)
		assert len(all(list).Passed) == 3
		assert all(list) < 5
		assert len(all(list).Passed) == 3
		list = ['hello', 'hello world']
		assert all(list) =~ 'hello'
		assert len(all(list).Passed) == 2
		
		obj = all(structs).width
		assert not obj > 2
		assert len(obj.Failed) == 2		
		
		assert not obj
		assert obj == false		
struct TestStruct:
	public width as int
	def constructor(val as int):
		width = val
	def ToString():
		return 'TestStruct.Width: ' + width
		