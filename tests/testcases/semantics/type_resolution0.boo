"""
namespace TypeResolution

import TypeResolution

public class Item(object):

	public def constructor():
		super()

[System.Reflection.DefaultMemberAttribute('Item')]
public class Collection(object):

	public Item[index as int] as TypeResolution.Item:
		public get:
			pass

	public def constructor():
		super()
"""
namespace TypeResolution

class Item:
	def constructor():
		pass
	
class Collection:	
	Item[index as int] as Item:
		get:
			pass
