using System;

namespace NDoc.Test.Attributes
{
	using System;
	using System.Collections;
	using System.Xml.Serialization;

	/// <summary>
	/// Defining and using custom attributes
	/// </summary>
	public class NamespaceDoc {}

	/// <summary>
	/// "IsTested" custom attribute class
	/// </summary>
	public class IsTestedAttribute : Attribute
	{
	}

	/// <summary>
	/// "Author" custom attribute class
	/// </summary>
	[AttributeUsage(AttributeTargets.Class)]
	public class TestAttribute : Attribute
	{
		public TestAttribute(){}

		public int field1;
		public string field2;

	}

	/// <summary>
	/// "Author" custom attribute class
	/// </summary>
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
	public class AuthorAttribute : Attribute
	{
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="name"></param>
		public AuthorAttribute(string name)
		{
			this.name = name;
			this.version = 0;
		}

		/// <summary>
		/// Name property of the Author attribute
		/// </summary>
		public string Name 
		{
			get 
			{
				return name;
			}
		}

		/// <summary>
		/// "IsTested" Attribute applied onto a property.
		/// </summary>
		[IsTested]
		public int Version
		{
			get 
			{
				return version;
			}
			set 
			{
				version = value;
			}
		}

		private string name;
		private int version;
	}

	/// <summary>
	/// Class with the "Author" attribute
	/// </summary>
	[Author("Joe Programmer")]
	[Test(field1=2, field2="SomeString")]
	public class Account
	{
		/// <summary>
		/// Method with the "IsTested" attribute
		/// </summary>
		[IsTested]
		public void AddOrder(Order orderToAdd)
		{
		}
	}

	/// <summary>
	/// Class with 3 attributes:  Author(name="Jane Programmer", Version=2), IsTested and XmlType.
	/// </summary>
	/// <remarks>This class has the [Serializable] attribute.</remarks>
	[Author("Jane Programmer", Version = 2), IsTested()]
	[XmlType(Namespace="NDoc/Test/Order")]
	[Serializable]
	public class Order
	{
		/// <summary>
		/// Field with XmlElement attribute.
		/// </summary>
		[XmlElement]
		public int Number = 0;

		/// <summary>
		/// Another field with XmlElement attribute.
		/// </summary>
		[XmlElement]
		public string What = "";

		/// <summary>
		/// This field has the [NonSerialized] attribute.
		/// </summary>
		[NonSerialized]
		public bool dummy = true;
	}
}
