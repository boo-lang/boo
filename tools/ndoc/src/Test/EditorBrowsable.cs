using System;

namespace NDoc.Test.EditorBrowsableAttr
{
	using System.ComponentModel;

	/// <summary>
	/// Various tests of the EditorBrowsable filtering
	/// </summary>
	public class NamespaceDoc {}

	/// <summary>This class is marked with EditorBrowsableState.Never.</summary>
	[EditorBrowsable(EditorBrowsableState.Never)]
	public class NotBrowsableClass
	{
		/// <summary>This method is defined in NotBrowsableClass.</summary>
		public void NotBrowsableClassMethod() {}
	}

	/// <summary>This class is marked with EditorBrowsableState.Always,
	/// but inherits from a class with EditorBrowsableState.Never.</summary>
	/// <remarks>
	/// <para>Links to NotBrowsableClass and its members should be disabled,
	/// including in the namespace hierarchy.</para>
	/// <para>Note that this illustrates a very bad usage of the 
	/// EditorBrowsable attribute.</para>
	/// </remarks>
	[EditorBrowsable(EditorBrowsableState.Always)]
	public class BrowsableNotBrowsableClass : NotBrowsableClass
	{
		/// <summary>This method is defined in BrowsableNotBrowsableClass.</summary>
		public void BrowsableNotBrowsableClassMethod() {}
	}

	/// <summary>This delegate is marked with EditorBrowsableState.Advanced</summary>
	[EditorBrowsable(EditorBrowsableState.Advanced)]
	public delegate void BrowsableHandler(object sender, EventArgs e);



	/// <summary>This class is marked with EditorBrowsableState.Always. </summary>
	[EditorBrowsable(EditorBrowsableState.Always)]
	public class BrowsableClass
	{
		/// <summary>This member is marked as EditorBrowsableState.Always.</summary>
		[EditorBrowsable(EditorBrowsableState.Always)]
		public BrowsableClass() {}

		/// <summary>This member is marked as EditorBrowsableState.Advanced.</summary>
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public BrowsableClass(int advanced) {}

		/// <summary>This member is marked as EditorBrowsableState.Never.</summary>
		[EditorBrowsable(EditorBrowsableState.Never)]
		public BrowsableClass(bool never) {}



		/// <summary>This member is marked as EditorBrowsableState.Always.</summary>
		[EditorBrowsable(EditorBrowsableState.Always)]
		public void AlwaysMethod() {}

		/// <summary>This member is marked as EditorBrowsableState.Advanced.</summary>
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public void AdvancedMethod() {}

		/// <summary>This member is marked as EditorBrowsableState.Never.</summary>
		[EditorBrowsable(EditorBrowsableState.Never)]
		public void NeverMethod() {}



		/// <summary>This member is marked as EditorBrowsableState.Always.</summary>
		[EditorBrowsable(EditorBrowsableState.Always)]
		public int AlwaysField;

		/// <summary>This member is marked as EditorBrowsableState.Advanced.</summary>
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public int AdvancedField;

		/// <summary>This member is marked as EditorBrowsableState.Never.</summary>
		[EditorBrowsable(EditorBrowsableState.Never)]
		public int NeverField;



		/// <summary>This member is marked as EditorBrowsableState.Always.</summary>
		[EditorBrowsable(EditorBrowsableState.Always)]
		public int AlwaysProperty
		{
			get { return -1; }
			set {}
		}

		/// <summary>This member is marked as EditorBrowsableState.Advanced.</summary>
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public int AdvancedProperty
		{
			get { return -1; }
			set {}
		}

		/// <summary>This member is marked as EditorBrowsableState.Never.</summary>
		[EditorBrowsable(EditorBrowsableState.Never)]
		public int NeverProperty
		{
			get { return -1; }
			set {}
		}



		/// <summary>This member is marked as EditorBrowsableState.Always.</summary>
		[EditorBrowsable(EditorBrowsableState.Always)]
		public static BrowsableClass operator ++(BrowsableClass a) { return null; }

		/// <summary>This member is marked as EditorBrowsableState.Advanced.</summary>
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public static BrowsableClass operator ~(BrowsableClass a) { return null; }

		/// <summary>This member is marked as EditorBrowsableState.Never.</summary>
		[EditorBrowsable(EditorBrowsableState.Never)]
		public static BrowsableClass operator --(BrowsableClass a) { return null; }



		/// <summary>This member is marked as EditorBrowsableState.Always.</summary>
		[EditorBrowsable(EditorBrowsableState.Always)]
		public event BrowsableHandler AlwaysEvent;

		/// <summary>This member is marked as EditorBrowsableState.Advanced.</summary>
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public event BrowsableHandler AdvancedEvent;

		/// <summary>This member is marked as EditorBrowsableState.Never.</summary>
		[EditorBrowsable(EditorBrowsableState.Never)]
		public event BrowsableHandler NeverEvent;

		private void EatEvents()
		{
			AlwaysEvent(this, new EventArgs());
			AdvancedEvent(this, new EventArgs());
			NeverEvent(this, new EventArgs());
		}
	}
}
