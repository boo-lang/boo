namespace BooCompiler.Tests.SupportingClasses

class PersonCollection(System.Collections.CollectionBase):
	def constructor():
		pass

	public self[index as int] as Person:
		get: return InnerList[index]
		set: InnerList[index] = value

	public self[fname as string] as Person:
		get:
			for p as Person in InnerList:
				return p if p.FirstName == fname
			return null
		set:
			index = 0
			for p as Person in InnerList:
				if p.FirstName == fname:
					InnerList[index] = value
					break
				++index

	public def Add(person as Person):
		InnerList.Add(person)
