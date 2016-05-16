using System;

namespace NDoc.Test.Obsolete 
{

	/// <summary>This namespace has tests for obsolete attribute.</summary>
	public class NamespaceDoc {}

	/// <summary>Represents a class with obsolete attributes.</summary>
	/// <obsolete>
	/// <para>This is additional info for the user regarding the obsolete status of this class and any alternatives that exist.</para>
	/// <para>These alternatives include links to <see cref="EventArgsOne">Another Class</see></para>
	/// </obsolete>
	/// <remarks>
	/// <para>These are the remarks</para>
	/// <para>This a link to <see cref="Class">Another Class</see></para>
	/// </remarks>
	[Obsolete("This is an obsolete class")]
	public class ObsoleteClass 
	{
		/// <overloads>
		/// <summary>
		/// overloads summary for constructor
		/// </summary>
		/// <remarks>This is the remarks for all the overloads of the contructors</remarks>
		/// </overloads>
		/// <summary>Initializes a new instance of the ObsoleteClass class with no param.</summary>
		[Obsolete("This is an obsolete constructor")]
		public ObsoleteClass() { }

		/// <summary>Initializes a new instance of the Class ObsoleteClass with an integer.
		/// <para>no message on ObsoleteAttribute</para> 
		///</summary>
		[Obsolete]
		public ObsoleteClass(int i) { }

		/// <summary>Initializes a new instance of the Class ObsoleteClass with a string.</summary>
		/// <obsolete><p>This is an obsolete message with html markup</p><p><font color='green'>hello</font></p></obsolete>
		[Obsolete("This is an obsolete constructor(string)")]
		public ObsoleteClass(string s) { }

		/// <summary>Initializes a new instance of the Class ObsoleteClass with a double.</summary>
		[Obsolete("This is an obsolete constructor")]
		protected ObsoleteClass(double d) { }

		/// <summary>Initializes a new instance of the Class ObsoleteClass with 3 integers.</summary>
		/// <param name="i1">This is the first integer parameter.
		/// </param>
		/// <param name="i2">This is the second integer parameter.</param>
		/// <param name="i3">This is the third integer parameter.</param>
		/// <remarks>
		/// Yes, the <paramref name="i3"/> parameter is of type int.
		/// </remarks>
		public ObsoleteClass(int i1, int i2, int i3) { }

		/// <summary>
		/// This is the static constructor.
		/// </summary>
		[Obsolete("This is an obsolete static constructor")]
		static ObsoleteClass() { }

		/// <summary>Holds an <c>int</c> value.</summary>
		[Obsolete("This is an obsolete field")]
		public int Field;

		/// <summary>Holds an read-only<c>int</c> value.</summary>
		[Obsolete("This is an obsolete field")]
		public readonly int ReadOnlyField = 12;

		/// <summary>Holds a static <c>int</c> value.</summary>
		[Obsolete("This is an obsolete field")]
		public static int StaticField;

		/// <summary>Gets a value.</summary>
		[Obsolete("This is an obsolete property")]
		public int Property 
		{
			get { return 0; }
		}

		/// <summary>Gets a static value.</summary>
		[Obsolete("This is an obsolete property")]
		public static int StaticProperty 
		{
			get { return 0; }
		}

		/// <overloads>
		/// <summary>
		/// <para>These are the indexers.</para>
		/// </summary>
		/// <remarks><para>This is the first para of the overload remarks</para><para>this is the second.</para></remarks>
		/// </overloads>
		/// <summary>This overloaded indexer accepts an int.</summary>
		/// <param name="i">The int index.</param>
		/// <value>Always returns 0.</value>
		/// <remarks><para>This is the first para in the remarks</para><para>this is the second.</para></remarks>
		[Obsolete("This is an obsolete indexer")]
		public int this[int i] 
		{
			get { return 0; }
		}

		/// <summary>This overloaded indexer accepts a string.</summary>
		[Obsolete("This is an obsolete indexer")]
		public int this[string s] 
		{
			get { return 0; }
		}

		/// <summary>This overloaded indexer accepts three ints.</summary>
		public int this[int i1, int i2, int i3] 
		{
			get { return 0; }
		}

		/// <summary>Executes some code Method().</summary>
		[Obsolete("This is an obsolete Method")]
		public void Method() { }

		/// <summary>Executes some code Method(int i1, int i2, int i3).</summary>
		public void Method(int i1, int i2, int i3) { }

		/// <summary>Executes some static code.</summary>
		[Obsolete("This is an obsolete Static Method")]
		public static void StaticMethod(System.DayOfWeek dow) { }

		/// <summary>Uses some parameter modifyers.</summary>
		[Obsolete("This is an obsolete Method")]
		public void ParameterModifyers( ref int refParam, out int outParam, params object[] paramArray ) 
		{
			outParam = 0;
		}

		/// <summary>An overload.</summary>
		public void ParameterModifyers( int a, ref int refParam, out int outParam, params object[] paramArray ) 
		{
			outParam = 0;
		}

		/// <summary>This is a simple event that uses the Handler delegate.</summary>
		[Obsolete("This is an obsolete event Message")]
		public event Handler Event;

		/// <summary>
		/// Raises some events.
		/// </summary>
		/// <remarks></remarks>
		[Obsolete("This is an obsolete Method Message")]
		public void RaisesSomeEvents() 
		{
			Event(this, new EventArgs());
			ProtectedEvent(this, new EventArgs());
			StaticEvent(this, new EventArgs());
			EventWithArgs(this, new EventArgsTest());
			EventWithMoreArgs(this, new EventArgsDerived());
			MultiEvent(this, new EventArgsOne());
		}

		/// <summary>A private event.</summary>
		[Obsolete("This is an obsolete event Handler")]
		private event Handler _event;

		/// <summary>This event uses the <b>add</b> and <b>remove</b> accessors.</summary>
		[Obsolete("This is an obsolete event Handler")]
		public event Handler AccessorsEvent 
		{
			add { _event += value; }
			remove { _event -= value; }
		}

		/// <summary>This event has arguments.</summary>
		[Obsolete("This is an obsolete event")]
		public event HandlerWithArgs EventWithArgs;

		/// <summary>This event has more arguments.</summary>
		/// <remarks>Check the links in the Event Data table...</remarks>
		[Obsolete("This is an obsolete event")]
		public event HandlerWithMoreArgs EventWithMoreArgs;

		/// <summary>This event is protected.</summary>
		[Obsolete("This is an obsolete event")]
		protected event Handler ProtectedEvent;

		/// <summary>static event</summary>
		[Obsolete("This is an obsolete event")]
		public static event Handler StaticEvent;

		/// <summary>An event with one property.</summary>
		public event MulticastHandler MultiEvent;

		/// <summary>This is my first operator.</summary>
		[Obsolete("This is an obsolete operator")]
		public static bool operator !(ObsoleteClass x) 
		{
			return false;
		}
		/// <summary>This is an operator.</summary>
		[Obsolete("This is an obsolete explicit operator")]
		public static explicit operator ObsoleteClass (double x) 
		{
			return new ObsoleteClass();
		}
	
	}
	/// <summary>This is a delegate with arguments used by Class.</summary>
	[Obsolete("This is an obsolete delegate")]
	public delegate void HandlerWithArgs(object sender, EventArgsTest e);

	/// <summary>This is a delegate with more arguments used by Class.</summary>
	[Obsolete("This is an obsolete delegate")]
	public delegate void HandlerWithMoreArgs(object sender, EventArgsDerived e);

	/// <summary>This is a simple delegate used by Class.</summary>
	[Obsolete("This is an obsolete delegate")]
	public delegate void Handler(object sender, EventArgs e);

	/// <summary>This is a multicast delegate. not obsolete..</summary>
	public delegate int MulticastHandler(object sender, EventArgsOne e);

	/// <summary>This event has one property.</summary>
	[Obsolete("This is an obsolete class")]
	public class EventArgsOne 
	{
		/// <summary>This is a unique event argument property.</summary>
		public bool OneProperty 
		{
			get { return true; }
			set { }
		}
	}

	/// <summary>This is an event arguments class.</summary>
	[Obsolete("This is an obsolete class")]
	public class EventArgsTest : System.ComponentModel.CancelEventArgs 
	{
		/// <summary>This is an event arguments property.</summary>
		public string EventArgsTestProperty 
		{
			get { return ""; }
			set { }
		}
	}

	/// <summary>This is a derived event arguments class.</summary>
	[Obsolete("This is an obsolete class")]
	public class EventArgsDerived : EventArgsTest 
	{
		/// <summary>This event arguments property is declared in the derived class.</summary>
		public string EventArgsDerivedProperty 
		{
			get { return ""; }
			set { }
		}
	}

	/// <summary>This is a derived event arguments class.</summary>
	[Obsolete("This is an obsolete Struct")]
	public struct ObsoleteStruct
	{
		/// <summary>This is a property</summary>
		public string ObsoleteStructProperty
		{
			get { return ""; }
			set { }
		}
	}

	/// <summary>
	/// This is an obsolete enum
	/// </summary>
	[Obsolete("This is an obsolete Enum")]
	public enum Enum
	{
		/// <summary>
		/// The only entry in the enum
		/// </summary>
		entry,
	}
}
