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

[Boo.Lang.EnumeratorItemTypeAttribute(typeof(string))]
[System.Reflection.DefaultMemberAttribute('Item')]
public class StringCollection(Boo.Lang.Useful.Collections.AbstractCollection):

	public def constructor():
		super()

	public def constructor(enumerable as object):
		super()
		if enumerable is null:
			raise System.ArgumentNullException('enumerable')
		for item as string in Boo.Lang.Runtime.RuntimeServices.GetEnumerable(enumerable):
			self.Add(item)

	public def Add(item as string) as void:
		if item is null:
			raise System.ArgumentNullException('item')
		self.InnerList.Add(item)

	public Item[index as int] as string:
		public get:
			return self.InnerList.get_Item(index)
"""
		RunTestCase(expected, code)