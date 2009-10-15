namespace BooCompiler.Tests.SupportingClasses
{
	public class PersonCollection : System.Collections.CollectionBase
	{
		public PersonCollection()
		{
		}
		
		public Person this[int index]
		{
			get
			{
				return (Person)InnerList[index];
			}
			
			set
			{
				InnerList[index] = value;
			}
		}
		
		public Person this[string fname]
		{
			get
			{
				foreach (Person p in InnerList)
				{
					if (p.FirstName == fname)
					{
						return p;
					}
				}
				return null;
			}
			
			set
			{
				int index = 0;
				foreach (Person p in InnerList)
				{
					if (p.FirstName == fname)
					{
						InnerList[index] = value;
						break;						
					}
					++index;
				}
			}
		}
		
		public void Add(Person person)
		{
			InnerList.Add(person);
		}
	}
}