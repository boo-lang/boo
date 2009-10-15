namespace BooCompiler.Tests.SupportingClasses
{
	public class Person
	{
		string _fname;
		string _lname;
		uint _age;
		
		public Person()
		{			
		}
		
		public uint Age
		{
			get
			{
				return _age;
			}
			
			set
			{
				_age = value;
			}
		}
		
		public string FirstName
		{
			get
			{
				return _fname;
			}
			
			set
			{
				_fname = value;
			}
		}
		
		public string LastName
		{
			get
			{
				return _lname;
			}
			
			set
			{
				_lname = value;
			}
		}
	}
}