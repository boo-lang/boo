namespace Boo.Lang.Useful.Tests.Collections

import NUnit.Framework
import Boo.Lang.Useful.Tests.Attributes

[TestFixture]
class CollectionAttributeTestFixture(AbstractAttributeTestFixture):
	[Test]
	def TestCollection():
		code = """
import Useful.Collections

[collection(string)]
class StringCollection:
	pass
"""

		expected = """
import Useful.Collections

[Boo.Lang.EnumeratorItemTypeAttribute(typeof(System.String))]
[System.Reflection.DefaultMemberAttribute('Item')]
public class StringCollection(Boo.Lang.Useful.Collections.AbstractCollection):

	public def constructor():
		super()

	public def constructor(enumerable as System.Object):
		super()
		if (enumerable is null):
			raise System.ArgumentNullException('enumerable')
		for item as System.String in Boo.Lang.Runtime.RuntimeServices.GetEnumerable(enumerable):
			self.Add(item)

	public def Add(item as System.String) as System.Void:
		if (item is null):
			raise System.ArgumentNullException('item')
		self.InnerList.Add(item)

	public Item(index as System.Int32) as System.String:
		public get:
			return self.InnerList.get_Item(index)
"""
		RunTestCase(expected, code)