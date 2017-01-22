using System;

namespace NDoc.Test
{
	/// <summary>
	/// <p>This documentation will show up as the summary of a class
	/// <see cref="NamespaceDoc"/>, when the UseNamespaceDocSummaries
	/// configuration flag is set to <see langword="false"/>. When
	/// UseNamespaceDocSummaries is set to <see langword="true"/>,
	/// the summary will show up as the summary of the <see cref="NDoc.Test"/>
	/// namespace.</p>
	/// <p>This allows you to reference other types from within the
	/// summary documentation for the namespace, without having to use
	/// fully qualified ids. E.g. the reference to <see cref="Class"/> is
	/// created by using "&lt;see cref="Class"/&gt;" instead of
	/// "&lt;see cref="T:NDoc.Test.Class"/&gt;" as you have to say using
	/// the namespace summaries dialog.</p>
	/// </summary>
	public class NamespaceDoc {}

#if UNICODE_NAMES
	/// <summary>
	/// DenGrønneKasse don't work in NDoc.
	/// </summary>
	public class DenGrønneKasse
	{
		/// <summary>
		/// Not working in NDoc
		/// </summary>
		public void TilføjÆble()
		{
		}
	}
#endif

	/// <summary>Represents a normal class.</summary>
	/// <remarks>Conceptualizing random endpoints in a access matrix
	/// provides reach extentions enterprise wide. Respective divisions
	/// historically insignificant, upscale trendlines in a management
	/// inventory analysis survivabilty format.</remarks>
	public class Class
	{
		/// <overloads>Initializes a new instance of the Class class.</overloads>
		/// <summary>Initializes a new instance of the <see cref="Class"/> class with no param.</summary>
		public Class() { }

		/// <summary>Initializes a new instance of the Class class with an integer.</summary>
		public Class(int i) { }

		/// <summary>Initializes a new instance of the Class class with a string.</summary>
		public Class(string s) { }

		/// <summary>Initializes a new instance of the Class class with a double.</summary>
		protected Class(double d) { }

		/// <summary>Initializes a new instance of the Class class with 3 integers.</summary>
		/// <param name="i1">This is the first integer parameter.
		/// This is the first integer parameter. This is the first integer
		/// parameter. This is the first integer parameter.</param>
		/// <param name="i2">This is the second integer parameter.</param>
		/// <param name="i3">This is the third integer parameter.</param>
		/// <remarks>
		/// Yes, the <paramref name="i3"/> parameter is of type int.
		/// </remarks>
		public Class(int i1, int i2, int i3) { }

		/// <summary>
		/// This is the static constructor.
		/// </summary>
		static Class() { }

		/// <summary>Holds an <c>int</c> value.</summary>
		public int Field;

		/// <summary>Holds an read-only<c>int</c> value.</summary>
		public readonly int ReadOnlyField = 12;

		/// <summary>Holds a static <c>int</c> value.</summary>
		public static int StaticField;

		/// <summary>Gets a value of <see cref="Property"/>.</summary>
		/// <remarks>Gets a value of <see cref="this"/>.</remarks>
		public int Property
		{
			get { return 0; }
		}

		/// <summary>Gets a static value.</summary>
		public static int StaticProperty
		{
			get { return 0; }
		}

		/// <summary>This overloaded indexer accepts an int.</summary>
		/// <param name="i">The int index.</param>
		/// <value>Always returns 0.</value>
		/// <remarks>No comment.</remarks>
		public int this[int i]
		{
			get { return 0; }
		}

		/// <summary>This overloaded indexer accepts a string.</summary>
		public int this[string s]
		{
			get { return 0; }
			set {}
		}

		/// <summary>This overloaded indexer accepts three ints.</summary>
		public int this[int i1, int i2, int i3]
		{
			get { return 0; }
			set {}
		}

		/// <summary>Executes some code.</summary>
		public void Method() { }

		/// <summary>Executes some code.</summary>
		public void Method(int i1, int i2, int i3) { }

		/// <summary>Executes some static code.</summary>
		public static void StaticMethod() { }

		/// <summary>Uses some parameter modifyers.</summary>
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
		public event Handler Event;

		/// <summary>
		/// Raises some events.
		/// </summary>
		/// <remarks><para>
		/// Raises the <see cref="Event"/> event when <see cref="Method"/> is called,
		/// if <see cref="Field"/> is greater than 0.
		/// </para><para>
		/// The above paragraph is only intended to test crefs on different member types...
		/// </para></remarks>
		/// <event cref="Event">Raised when something occurs.</event>
		/// <event cref="AccessorsEvent">Raised when it occurs...</event>
		/// <event cref="ProtectedEvent">Raised when something else occurs.</event>
		/// <event cref="EventWithArgs">Raised when it feels like it.</event>
		/// <event cref="System.Diagnostics.EventLog.EntryWritten">Raised when an entry was written to the event log.</event>
		/// <event cref="SomeUnknownEvent">Unknown.</event>
		/// <event cref="EventWithMoreArgs">Never raised?</event>
		/// <event cref="MultiEvent">Raised many times?</event>
		/// <exception cref="Exception">
		/// Some exception is thrown.
		/// </exception>
		/// <exception cref="MyException">
		/// Some other exception may also be thrown.
		/// </exception>
		/// <exception cref="SomeUnknownException">Unknown.</exception>
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
		private event Handler _event;

		/// <summary>This event uses the <b>add</b> and <b>remove</b> accessors.</summary>
		public event Handler AccessorsEvent
		{
			add { _event += value; }
			remove { _event -= value; }
		}

		/// <summary>This event has arguments.</summary>
		public event HandlerWithArgs EventWithArgs;

		/// <summary>This event has more arguments.</summary>
		/// <remarks>Check the links in the Event Data table...</remarks>
		public event HandlerWithMoreArgs EventWithMoreArgs;

		/// <summary>This event is protected.</summary>
		protected event Handler ProtectedEvent;

		/// <summary>Can you do this?</summary>
		public static event Handler StaticEvent;

		/// <summary>An event with one property.</summary>
		public event MulticastHandler MultiEvent;

		/// <summary>This is my first overloaded operator.</summary>
		/// <remarks>Why do we have to declare them as static?</remarks>
		public static bool operator !(Class x)
		{
			return false;
		}
	}

	/// <summary>This event has one property.</summary>
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
	public class EventArgsDerived : EventArgsTest
	{
		/// <summary>This event arguments property is declared in the derived class.</summary>
		public string EventArgsDerivedProperty
		{
			get { return ""; }
			set { }
		}
	}

	/// <summary>This is a delegate with arguments used by Class.</summary>
	public delegate void HandlerWithArgs(object sender, EventArgsTest e);

	/// <summary>This is a delegate with more arguments used by Class.</summary>
	public delegate void HandlerWithMoreArgs(object sender, EventArgsDerived e);

	/// <summary>This is a simple delegate used by Class.</summary>
	public delegate void Handler(object sender, EventArgs e);

	/// <summary>This is a multicast delegate.</summary>
	public delegate int MulticastHandler(object sender, EventArgsOne e);

	/// <summary>This is an interface.</summary>
	public interface Interface
	{
		/// <summary>This is a property in an interface.</summary>
		int InterfaceProperty
		{
			get;
		}

		/// <summary>This is a method in an interface.</summary>
		void InterfaceMethod();

		/// <summary>This event is declared in an interface.</summary>
		event Handler InterfaceEvent;
	}

	/// <summary>This interface inherits from another interface.</summary>
	public interface InterfaceInherited : Interface
	{
		/// <summary>This is another method declared in an inteface.</summary>
		void OtherMethod();
	}

	/// <summary>This class implements <see cref="Interface"/>.</summary>
	public class ImplementsInterface : Interface
	{
		/// <summary>
		/// Gets the InterfaceProperty.
		/// </summary>
		/// <value></value>
		public int InterfaceProperty
		{
			get
			{
				return 0;
			}
		}

		/// <summary>
		/// InterfaceMethod.
		/// </summary>
		public void InterfaceMethod()
		{
			InterfaceEvent(this, new EventArgs());
		}

		/// <summary>
		/// InterfaceEvent
		/// </summary>
		public event Handler InterfaceEvent;

	}

	/// <summary>This class is derived from <see cref="Base"/> and implements <see cref="Interface"/>.</summary>
	public class ImplementsInterfaceDerivedBase : Base, Interface
	{
		/// <summary>
		/// Gets the InterfaceProperty.
		/// </summary>
		/// <value></value>
		public int InterfaceProperty
		{
			get
			{
				return 0;
			}
		}

		/// <summary>
		/// InterfaceMethod.
		/// </summary>
		public void InterfaceMethod()
		{
			InterfaceEvent(this, new EventArgs());
		}

		/// <summary>
		/// InterfaceEvent
		/// </summary>
		public event Handler InterfaceEvent;

	}

	/// <summary>This is an empty interface.</summary>
	public interface Interface1
	{
	}

	/// <summary>This is also an empty interface.</summary>
	public interface Interface2
	{
	}

	/// <summary>This class implements two empty interfaces.</summary>
	public class ImplementsTwoInterfaces : Interface1, Interface2
	{
	}

	/// <summary>Represents an abstract class.</summary>
	public abstract class Abstract
	{
		/// <summary>This event is decalred in the Abstract class.</summary>
		public abstract event Handler InterfaceEvent;
	}

	/// <summary>Represents a base class.</summary>
	public class Base
	{
		/// <summary>This property is declared in the Base class.</summary>
		public int BaseProperty
		{
			get { return 0; }
		}

		/// <summary>This method is declared in the Base class.</summary>
		public void BaseMethod() { }

		/// <summary>This method is declared in the Base class without the "new" keyword.</summary>
		public void NewMethod() { }

		/// <summary>This method is declared in the Base class.</summary>
		public void Overloaded(int i) { }

		/// <summary>This method is declared in the Base class.</summary>
		public void Overloaded(byte i) { }

		/// <summary>This virtual method is declared in the Base class.</summary>
		public virtual void TwoVirtualOverloads(string key) { }
		/// <summary>This virtual method is declared in the Base class.</summary>
		public virtual void TwoVirtualOverloads(int index) { }

		/// <summary>This field is declared in the Base class.</summary>
		public int BaseField;

		/// <summary>This event is declared in the Base class.</summary>
		public event Handler BaseEvent;

		private void UseBaseEvent()
		{
			BaseEvent(this, new EventArgs());
		}

		/// <summary>A static method in the Base class.</summary>
		/// <remarks>This should not appear in derived classes.</remarks>
		public static void StaticBaseMethod() {}
	}

	/// <summary>Represents a derived class.</summary>
	public class Derived : Base
	{
		/// <summary>This property is declared in the Derived class.</summary>
		public int DerivedProperty
		{
			get { return 0; }
		}

		/// <summary>This method is declared in the Derived class.</summary>
		/// <remarks>This is a reference to a parent member: <see cref="Base.BaseProperty"/></remarks>
		public void DerivedMethod() { }

		/// <summary>This method is declared in the Derived class with the "new" keyword.</summary>
		public new void NewMethod() { }

		/// <summary>This method is overloaded in the Derived class.</summary>
		public void Overloaded(string s) { }

		/// <summary>This method is also overloaded in the Derived class.</summary>
		public void Overloaded(double d) { }

		/// <summary>This method is also overloaded in the Derived class.</summary>
		public void Overloaded(char c) { }

		/// <summary>This method is also overloaded in the Derived class.</summary>
		/// <remarks>This method accepts a type declared in the same namespace.</remarks>
		public void Overloaded(Interface i) { }

		/// <summary>This method is overriden in the Derived class.</summary>
		/// <remarks>Only one of the two overloads is overriden.</remarks>
		public override void TwoVirtualOverloads(string key) { }

		/// <summary>A static method in the Derived class.</summary>
		/// <remarks>This should not appear in derived classes.</remarks>
		public static void StaticDerivedMethod() {}
	}

	/// <summary>Represents another derived class.</summary>
	public class Derived2 : Derived
	{
		/// <summary>
		/// This event is declared in the Derived2 class.
		/// </summary>
		public event Handler EventInDerived;

		private void UseDerivedEvent()
		{
			EventInDerived(this, new EventArgs());
		}

		/// <summary>This method is overriden in the Derived class.</summary>
		/// <remarks>Both overloads are overriden in this class.</remarks>
		public override void TwoVirtualOverloads(string key) { }

		/// <summary>This method is overriden in the Derived class.</summary>
		/// <remarks>Both overloads are overriden in this class.</remarks>
		public override void TwoVirtualOverloads(int key) { }

		/// <summary>
		/// Add only one overload in Derived2 class.
		/// </summary>
		public void Overloaded(object o) { }
	}

	/// <summary>Represents an outer class.</summary>
	public class Outer
	{
		/// <summary>Represents an inner class.</summary>
		/// <remarks>These are some remarks.</remarks>
		public class Inner
		{
			/// <summary>This is a field of the inner class.</summary>
			/// <remarks>These are some remarks</remarks>
			public int InnerField;

			/// <summary>This is a property of the inner class.</summary>
			/// <remarks>These are some remarks</remarks>
			public int InnerProperty { get { return 0; } }

			/// <summary>This is a method of the inner class.</summary>
			/// <remarks>These are some remarks</remarks>
			public void InnerMethod() { }

			/// <summary>This is an enumeration nested in a nested class.</summary>
			public enum InnerInnerEnum
			{
				/// <summary>Foo</summary>
				Foo
			}
		}

		/// <summary>Function returning a public inner class oject.</summary>
		public Inner GetInnerClassObject() { return new Inner(); }

		/// <summary>Function with a public inner class oject parameter.</summary>
		public void TestInnerClassObject(Inner TheInner) { }

		/// <summary>Represents a private inner class.</summary>
		private class PrivateInner
		{
		}

		/// <summary>This is a nested enumeration.</summary>
		public enum InnerEnum
		{
			/// <summary>Foo</summary>
			Foo
		}

		/// <summary>This is a nested interface.</summary>
		public interface InnerInterface {}

		/// <summary>This is a nested structure.</summary>
		public struct InnerStruct {}

		/// <summary>This is a nested delegate.</summary>
		public delegate void InnerDelegate(string myparam);
	}

	/// <summary>This is an internal class.</summary>
	internal class Internal
	{
		/// <summary>This method is declared in the Internal class.</summary>
		internal void InternalMethod() { }
	}

	/// <summary>This is the first struct.</summary>
	public struct Struct1
	{
		/// <summary>This is the first field in the first struct.</summary>
		public int Field1;

		/// <summary>This is the second field in the first struct.</summary>
		public string Field2;

		/// <summary>A property in a struct.</summary>
		public int Property
		{
			get { return 1; }
			set { }
		}

		/// <summary>A method in a struct.</summary>
		public int Method() { return -1; }

		/// <summary>A static method in a struct.</summary>
		public static int StaticMethod() { return -1; }
	}

	/// <summary>This is the second struct.</summary>
	public struct Struct2
	{
	}

	/// <summary>This is an enumeration.</summary>
	public enum Enum
	{
		/// <summary>Represents Foo.</summary>
		Foo,
		/// <summary>Represents Bar.</summary>
		Bar,
		/// <summary>Represents Baz.</summary>
		Baz,
		/// <summary>Represents Quux.</summary>
		Quux
	}

	/// <summary>
	/// This is an enum that is not the default <see cref="System.Int32"/> type.
	/// It is an <see cref="System.Int16"/> type
	/// </summary>
	public enum EnumNonInt : short
	{
		/// <summary>
		/// The only entry in the short enum
		/// </summary>
		entry,
	}

	/// <summary>This is an enumeration with the FlagsAttribute.</summary>
	[Flags]
	public enum EnumFlags
	{
		/// <summary>Represents Foo.</summary>
		Foo=1,
		/// <summary>Represents Bar.</summary>
		Bar=2,
		/// <summary>Represents Baz.</summary>
		Baz=4,
		/// <summary>Represents Quux.</summary>
		Quux=8
	}

	/// <summary>This class has lots of &lt;see&gt; elements in the remarks.</summary>
	/// <remarks>See <see cref="Class"/>.
	/// See <see cref="Interface"/>.
	/// See <see cref="Struct1"/>.
	/// See <see cref="Base.BaseMethod"/>.
	/// See <see cref="Derived.DerivedMethod"/>.
	/// See <see cref="Outer"/>.
	/// See <see cref="Outer.Inner"/>.
	/// See <see cref="Handler"/>.
	/// See <see cref="Enum"/>.
	/// See <see cref="Enum.Bar"/>.
	/// See <see href="http://ndoc.sf.net/" />.
	/// See <see href="http://ndoc.sf.net/">NDOC</see>.
	/// </remarks>
	public class See
	{
		/// <summary>
		/// This field's documentation references <see cref="RefProp1"/>.
		/// </summary>
		public int Field1 = 0;

		/// <summary>
		/// This properties' documentation references <see cref="System.IO.TextWriter"/>.
		/// </summary>
		/// <remarks>
		/// <para>This is a link to <see cref="Prop1"/>.</para>
		/// <para>This is a labelled link to <see cref="Prop1">myself</see>.</para>
		/// <para>This is a link to <see cref="See"/>.</para>
		/// <para>This is a link to <see cref="SeeAlso"/>.</para>
		/// <para>This is also a link to <see cref="See"/>. </para>
		/// <para>This is also a link to <see cref="SeeAlso"/>. </para>
		/// <para>This is an labelled link to <see cref="See">Test.See</see>.</para>
		/// <para>This is an broken link to <see cref="something">something</see>.</para> 
		/// <para>This is an broken labelled link to <see cref="something">something</see>.</para> 
		/// </remarks>
		public string Prop1
		{
			get { return "Prop1"; }
		}

		/// <summary>
		/// This method's documentation references <see cref="NDoc.Test.See.Prop1"/> and
		/// <see cref="Field1"/>.
		/// </summary>
		public void RefProp1()
		{
		}
	}

	/// <summary>This class has lots of &lt;seealso&gt; elements.</summary>
	/// <remarks>NDoc adds a special form of the &lt;seealso&gt; element.
	/// Instead of a cref attribute, you can specify a href attribute some text
	/// content just like a normal HTML &lt;a&gt; element.</remarks>
	/// <seealso href="http://ndoc.sf.net/">the ndoc homepage</seealso>
	/// <seealso cref="Class"/>
	/// <seealso cref="Interface"/>
	/// <seealso cref="Struct1"/>
	/// <seealso cref="Base.BaseMethod"/>
	/// <seealso cref="Derived.DerivedMethod"/>
	/// <seealso cref="Outer"/>
	/// <seealso cref="Outer.Inner"/>
	/// <seealso cref="Handler"/>
	/// <seealso cref="Enum"/>
	/// <seealso cref="Enum.Bar"/>
	/// <seealso href="http://slashdot.org/">Slashdot</seealso>
	/// <seealso cref="System.Object"/>
	public class SeeAlso
	{
		/// <summary>This method has lots of &lt;seealso&gt; elements.</summary>
		/// <seealso cref="Class"/>
		/// <seealso cref="Interface"/>
		/// <seealso cref="Struct1"/>
		/// <seealso cref="Base.BaseMethod"/>
		/// <seealso cref="Derived.DerivedMethod"/>
		/// <seealso cref="Outer"/>
		/// <seealso cref="Outer.Inner"/>
		/// <seealso cref="Handler"/>
		/// <seealso cref="Enum"/>
		public void AlsoSee() { }

		/// <summary>&lt;seealso cref="System.Object"/></summary>
		/// <seealso cref="System.Object"/>
		public void SeeSystemClass()
		{
		}

		/// <summary>&lt;seealso cref="System.String.Empty"/></summary>
		/// <seealso cref="System.String.Empty"/>
		public void SeeSystemField()
		{
		}

		/// <summary>&lt;seealso cref="System.String.Length"/></summary>
		/// <seealso cref="System.String.Length"/>
		public void SeeSystemProperty()
		{
		}

		/// <summary>&lt;seealso cref="System.Collections.ArrayList.Item"/></summary>
		/// <seealso cref="System.Collections.ArrayList.this"/>
		public void SeeSystemIndexer()
		{
		}

		/// <summary>&lt;seealso cref="System.Object.ToString"/></summary>
		/// <seealso cref="System.Object.ToString"/>
		public void SeeSystemMethod()
		{
		}

		/// <summary>&lt;seealso cref="System.Object.ToString"/></summary>
		/// <seealso cref="System.String.Equals"/>
		public void SeeSystemOverloadedMethod()
		{
		}

		/// <summary>&lt;seealso cref="System.Xml.XmlDocument.NodeChanged"/></summary>
		/// <seealso cref="System.Xml.XmlDocument.NodeChanged"/>
		public void SeeSystemEvent()
		{
		}

		/// <summary>&lt;seealso cref="System.IDisposable"/></summary>
		/// <seealso cref="System.IDisposable"/>
		public void SeeSystemInterface()
		{
		}

		/// <summary>&lt;seealso cref="System.DateTime"/></summary>
		/// <seealso cref="System.DateTime"/>
		public void SeeSystemStructure()
		{
		}

		/// <summary>&lt;seealso cref="System.EventHandler"/></summary>
		/// <seealso cref="System.EventHandler"/>
		public void SeeSystemDelegate()
		{
		}

		/// <summary>&lt;seealso cref="System.DayOfWeek"/></summary>
		/// <seealso cref="System.DayOfWeek"/>
		public void SeeSystemEnumeration()
		{
		}

		/// <summary>&lt;seealso cref="System.IO"/></summary>
		/// <seealso cref="System.IO"/>
		public void SeeSystemNamespace()
		{
		}
	}

	/// <summary>Represents a class containing properties.</summary>
	public abstract class Properties
	{
		/// <summary>This property has a getter and a setter.</summary>
		public int GetterAndSetter
		{
			get { return 0; }
			set { }
		}

		/// <summary>This property has a getter only.</summary>
		public int GetterOnly
		{
			get { return 0; }
		}

		/// <summary>This property has a setter only.</summary>
		public int SetterOnly
		{
			set { }
		}

		/// <summary>This property is abstract.</summary>
		public abstract int AbstractProperty
		{
			get;
			set;
		}

		/// <summary>This property is virtual.</summary>
		public virtual int VirtualProperty
		{
			get { return 0; }
			set { }
		}

		/// <summary>This is an overloaded indexer.</summary>
		/// <remarks>This indexer accepts an int parameter.</remarks>
		public int this[int foo]
		{
			get { return 0; }
		}

		/// <summary>This is an overloaded indexer.</summary>
		/// <remarks>This indexer accepts a string parameter.</remarks>
		public int this[string foo]
		{
			get { return 0; }
		}
	}

	/// <summary>Represents a class that has lots of links
	/// in its documentation.</summary>
	public class Links
	{
		/// <summary>Holds an integer.</summary>
		public int IntField;

		/// <summary>Gets or sets an integer.</summary>
		/// <value>an integer</value>
		public int IntProperty
		{
			get { return 0; }
			set { }
		}

		/// <summary>Returns nothing.</summary>
		/// <returns>Nada.</returns>
		public void VoidMethod() { }

		/// <summary>Returns an int.</summary>
		public int IntMethod() { return 0; }

		/// <summary>Returns a string.</summary>
		public string StringMethod() { return null; }

		/// <summary>This method accepts lots of parameters.</summary>
		/// <param name="i">an integer</param>
		/// <param name="s">a string</param>
		/// <param name="c">a character</param>
		/// <param name="d">a double</param>
		/// <remarks>The <paramref name="i"/> param is an integer.
		/// The <paramref name="s"/> param is a string.</remarks>
		public void LotsOfParams(int i, string s, char c, double d)
		{
		}
	}

	/// <summary>This class contains some example code.</summary>
	/// <example><code>
	/// public class HelloWorld {
	///		static void Main() {
	///			System.Console.WriteLine("Hello, World!");
	///		}
	/// }
	/// </code></example>
	public class Example
	{
	}

	/// <summary>This class contains a method that throws exceptions.</summary>
	public class Exceptions
	{
		/// <summary>This method throws exceptions.</summary>
		/// <exception cref="Exception">A generic exception.</exception>
		/// <exception cref="ApplicationException">An application-specific exception.</exception>
		public void Throw()
		{
		}
	}

	/// <summary>This class contains &lt;see langword=""&gt; elements in the remarks.</summary>
	/// <remarks>The default style is <see langword="bold"/>.
	/// But <see langword="null"/>, <see langword="sealed"/>,
	/// <see langword="static"/>, <see langword="abstract"/>,
	/// and <see langword="virtual"/> do more.</remarks>
	public class Langword
	{
	}

	/// <summary>The remarks in this class contains examples of list elements.</summary>
	public class Lists
	{
		/// <summary>BulletMethodSummary</summary>
		/// <remarks>
		///  <list type="bullet">
		///   <item><description>Item One</description></item>
		///   <item><description>Item Two</description></item>
		///   <item><description>Item Three</description></item>
		///  </list>
		/// </remarks>
		public void BulletMethod()
		{
		}

		/// <summary>NumberMethodSummary</summary>
		/// <remarks>
		///  <list type="number">
		///   <item><description>Item One</description></item>
		///   <item><description>Item Two</description></item>
		///   <item><description>Item Three</description></item>
		///  </list>
		/// </remarks>
		public void NumberMethod()
		{
		}

		/// <summary>TermMethodSummary</summary>
		/// <remarks>
		///  <list type="bullet">
		///   <item><term>Term One</term><description>Item One</description></item>
		///   <item><term>Term Two</term><description>Item Two</description></item>
		///   <item><term>Term Three</term><description>Item Three</description></item>
		///  </list>
		/// </remarks>
		public void TermMethod()
		{
		}

		/// <summary>TableMethodSummary</summary>
		/// <remarks>
		///		<list type="table">
		///			<item><description>Cell One</description></item>
		///			<item><description>Cell Two</description></item>
		///			<item><description>Cell Three</description></item>
		///		</list>
		/// </remarks>
		public void TableMethod()
		{
		}

		/// <summary>TableWithHeaderMethodSummary</summary>
		/// <remarks>
		///		<list type="table">
		///			<listheader><description>Header</description></listheader>
		///			<item><description>Cell One</description></item>
		///			<item><description>Cell Two</description></item>
		///			<item><description>Cell Three</description></item>
		///		</list>
		/// </remarks>
		public void TableWithHeaderMethod()
		{
		}

		/// <summary>TwoColumnTableMethodSummary</summary>
		/// <remarks>
		///		<list type="table">
		///			<listheader>
		///				<term>Something</term>
		///				<description>Details</description>
		///			</listheader>
		///			<item>
		///				<term>Item 1</term>
		///				<description>This is the first item</description>
		///			</item>
		///			<item>
		///				<term>Item 2</term>
		///				<description>This is the second item</description>
		///			</item>
		///			<item>
		///				<term>Item 3</term>
		///				<description>This is the third item</description>
		///			</item>
		///		</list>
		/// </remarks>
		public void TwoColumnTableMethod()
		{
		}

		/// <summary>DefinitionMethodSummary</summary>
		/// <remarks>
		///  <list type="definition">
		///   <item><term>Term 1</term><description>Definition One</description></item>
		///   <item><term>Term 2</term><description>Definition Two</description></item>
		///   <item><term>Term 3</term><description>Definition Three</description></item>
		///  </list>
		/// </remarks>
		public void DefinitionMethod()
		{
		}

	}

	/// <summary>This class has para elements in its remarks.</summary>
	/// <remarks><para>This is the first paragraph.</para>
	/// <para>This is the second paragraph.</para></remarks>
	public class Paragraphs
	{
	}

	/// <summary>This class shows how permission elements are used.</summary>
	/// <permission cref="System.Security.PermissionSet">to inherit from this class.</permission>
	public class Permissions
	{
		/// <summary>This constructor has permissions.</summary>
		/// <permission cref="System.Security.PermissionSet">to instanciate the <see cref="Permissions"/> class.</permission>
		public Permissions()
		{
		}
		/// <summary>This field has permissions.</summary>
		/// <permission cref="System.Security.PermissionSet">to access this field.</permission>
		public string RestrictedField;

		/// <summary>This property has permissions.</summary>
		/// <permission cref="System.Security.PermissionSet">to access this property.</permission>
		public string RestrictedProperty
		{
			get { return ""; }
		}
		/// <summary>This method has permissions.</summary>
		/// <permission cref="System.Security.PermissionSet">to execute this method.</permission>
		public void RestrictedMethod()
		{
			RestrictedEvent(this, new EventArgs());
		}
		/// <summary>This event has permissions.</summary>
		/// <permission cref="System.Security.PermissionSet">to register with this event.</permission>
		public event Handler RestrictedEvent;
	}

	/// <summary>This is a sealed class.</summary>
	public sealed class SealedClass
	{
	}

	/// <summary>This class covers all member visibilities.</summary>
	public class VisibilityTester
	{
		/// <summary>Public constructor</summary>
		public VisibilityTester() {}

		/// <summary>Public method</summary>
		public void PublicMethod() {}

		/// <summary>Public read-only property</summary>
		public bool PublicReadOnlyProperty
		{
			get { return false; }
		}

		/// <summary>Public write-only property</summary>
		public bool PublicWriteOnlyProperty
		{
			set {}
		}

		/// <summary>Public field</summary>
		public bool publicField;

		/// <summary>Public event</summary>
		public event Handler PublicEvent;

		/// <summary>Protected constructor</summary>
		protected VisibilityTester(bool a) {}

		/// <summary>Protected method</summary>
		protected void ProtectedMethod() {}

		/// <summary>Protected read-only property</summary>
		protected bool ProtectedReadOnlyProperty
		{
			get { return false; }
		}

		/// <summary>Protected write-only property</summary>
		protected bool ProtectedWriteOnlyProperty
		{
			set {}
		}

		/// <summary>Protected field</summary>
		protected bool protectedField;

		/// <summary>Protected event</summary>
		protected event Handler ProtectedEvent;

		/// <summary>Private constructor</summary>
		private VisibilityTester(int a) {}

		/// <summary>Private method</summary>
		private void PrivateMethod() {}

		/// <summary>Private read-only property</summary>
		private bool PrivateReadOnlyProperty
		{
			get { return false; }
		}

		/// <summary>Private write-only property</summary>
		private bool PrivateWriteOnlyProperty
		{
			set {}
		}

		/// <summary>Private field</summary>
		private bool privateField = false;

		/// <summary>Private event</summary>
		private event Handler PrivateEvent;

		/// <summary>Protected Internal constructor</summary>
		protected internal VisibilityTester(short a) {}

		/// <summary>Protected Internal method</summary>
		protected internal void ProtectedInternalMethod() {}

		/// <summary>Protected Internal read-only property</summary>
		protected internal bool ProtectedInternalReadOnlyProperty
		{
			get { return false; }
		}

		/// <summary>Protected Internal write-only property</summary>
		protected internal bool ProtectedInternalWriteOnlyProperty
		{
			set {}
		}

		/// <summary>Protected Internal field</summary>
		protected internal bool protectedInternalField;

		/// <summary>Protected Internal event</summary>
		protected internal event Handler ProtectedInternalEvent;

		/// <summary>Internal constructor</summary>
		internal VisibilityTester(long a) {}

		/// <summary>Internal method</summary>
		internal bool InternalMethod()
		{
			return (PublicEvent != null ||
				ProtectedEvent != null ||
				PrivateEvent != null ||
				ProtectedInternalEvent != null ||
				InternalEvent != null ||
				publicField ||
				protectedField ||
				privateField ||
				protectedInternalField ||
				internalField);
		}

		/// <summary>Internal read-only property</summary>
		internal bool InternalReadOnlyProperty
		{
			get { return false; }
		}

		/// <summary>Internal write-only property</summary>
		internal bool InternalWriteOnlyProperty
		{
			set {}
		}

		/// <summary>Internal field</summary>
		internal bool internalField = false;

		/// <summary>Internal event</summary>
		internal event Handler InternalEvent;
	}

	/// <summary>
	/// </summary>
	public class MissingDocumentationBase
	{
		/// <summary>
		/// This one's documented!
		/// </summary>
		/// <param name="a">A param</param>
		/// <param name="b">Anotner param</param>
		/// <returns>returns something</returns>
		/// <remarks><para>
		/// This is a remark.
		/// </para></remarks>
		public int SomeMethod( int a, bool b ) { return 0; }

		/// <summary>
		/// This one's overloaded and documented!
		/// </summary>
		/// <param name="a">A param</param>
		/// <param name="b">Anotner param</param>
		/// <returns>returns something</returns>
		/// <remarks><para>
		/// This is a remark.
		/// </para></remarks>
		public int SomeMethod( int a, int b ) { return 0; }

		/// <summary>
		///
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <returns></returns>
		/// <remarks> </remarks>
		public int MethodWithEmptyDoc(int a, int b) { return 0; }
	}

	/// <summary>
	/// </summary>
	public class MissingDocumentationDerived : MissingDocumentationBase
	{
	}

	/// <summary>
	/// This is an exception.
	/// </summary>
	public class MyException : ApplicationException
	{
		/// <summary>
		/// This is a constructor for the exception.
		/// </summary>
		/// <param name="message">Message for this exception.</param>
		public MyException( string message ) : base( message ) {}
	}

	/// <summary>This class has custom attributes on it.</summary>
	[CLSCompliant(false)]
	public class CustomAttributes
	{
	}

	/// <summary>This class contains just an indexer so that we can see
	/// if that's what causes the DefaultMemberAttribute attribute to appear.</summary>
	public class JustIndexer
	{
		/// <summary>Am I the default member?</summary>
		public int this[int i]
		{
			get { return 0; }
		}
	}

	/// <summary>The remarks contain some &lt;para> and &lt;code> elements with lang attributes.</summary>
	/// <remarks>
	///		<para>This paragraph has no lang attribute.</para>
	///		<para lang="Visual Basic">This paragraph has a Visual Basic lang attribute.</para>
	///		<para lang="VB">This paragraph has a VB lang attribute.</para>
	///		<para lang="C#">This paragraph has a C# lang attribute.</para>
	///		<para lang="C++, JScript">This paragraph has a C++, JScript lang attribute.</para>
	///		<code lang="Visual Basic">
	///			' This is some Visual Basic code.
	///		</code>
	///		<code lang="VB">
	///			' This is some VB code.
	///		</code>
	///		<code lang="C#">
	///			// This is some C# code.
	///		</code>
	///		<code lang="C++, JScript">
	///			// This is either C++ or JScript code.
	///		</code>
	/// </remarks>
	public class LangAttributes
	{
	}

	/// <include file='include.xml' path='documentation/class[@name="IncludeExample"]/*'/>
	public class IncludeExample
	{
	}

	/// <summary>This class has two methods with the same name but one is an instance method
	/// and the other is static.</summary>
	public class BothInstanceAndStaticOverloads
	{
		/// <summary>This is the instance method.</summary>
		public void Foo()
		{
		}

		/// <summary>This is the static method.</summary>
		public static void Foo(int i)
		{
		}
	}

	/// <summary>This class has two methods with the same name but one is an instance method
	/// and the other is static.</summary>
	public class BothInstanceAndStaticOverloads2
	{
		/// <summary>This is the instance method.</summary>
		public void Foo()
		{
		}
		/// <summary>This is another instance method.</summary>
		public void Foo(string name)
		{
		}

		/// <summary>This is the static method.</summary>
		public static void Foo(int i)
		{
		}
		/// <summary>This is another static method.</summary>
		public static void Foo(object o)
		{
		}
	}

	// The following two examples were submitted by Ross.Nelson@devnet.ato.gov.au
	// in order to demonstrate two bugs.

	/// <summary> this is fred </summary>
	public enum fred 
	{
		/// <summary>aaaa</summary>
		valuea,
		/// <summary>bbbb</summary>
		valueb
	}

	/// <summary>this is jjj</summary>
	public class jjj
	{
		/// <summary> this is fred </summary>
		public enum fred {
			/// <summary>aaaa</summary>
			valuea,
			/// <summary>bbbb</summary>
			valueb
		}

		/// <summary>jjj constructor</summary>
		/// <remarks>jjj blah</remarks>
		/// <param name="f">f blah</param>
		public jjj(fred f)
		{
		}

		/// <summary>mmm method</summary>
		/// <remarks>mmm blah</remarks>
		/// <param name="f">f blah</param>
		public void mmm(fred f)
		{
		}
	}

	/// <summary>This class has an event that throws an exception.</summary>
	public class EventWithException
	{
		/// <exception cref="System.Exception">Thrown when... .</exception>
		public event EventHandler ServiceRequest
		{
			add {}
			remove {}
		}
	}

	/// <summary>This class has a method that's overloaded where one of the
	/// overloads doesn't have any parameters.</summary>
	public class OverloadedWithNoParameters
	{
		/// <summary>This is an overloaded method.</summary>
		/// <remarks>This overload has no parameters.</remarks>
		public void Method() {}

		/// <summary>This is an overloaded method.</summary>
		/// <remarks>This overload has one parameter.</remarks>
		public void Method(int i) {}
	}

	/// <summary>This class wants to ref the method with no parameters
	/// in the OverloadedWithNoParameters class.
	/// See <see cref="OverloadedWithNoParameters.Method" />
	/// ("OverloadedWithNoParameters.Method").
	/// See <see cref="OverloadedWithNoParameters.Method()" />
	/// ("OverloadedWithNoParameters.Method()").
	/// See <see cref="OverloadedWithNoParameters.Method(int)" />
	/// ("OverloadedWithNoParameters.Method(int)").
	/// </summary>
	/// <remarks>
	/// The link to the method with parameters should point to that correct page.
	/// </remarks>
	public class CRefToOverloadWithNoParameters
	{
	}

	/// <summary>Explicit interface test (public)</summary>
	public interface ExplicitInterface
	{
		/// <summary>Explicit method test</summary>
		int ExplicitProperty { get; }

		/// <summary>Implicit method test</summary>
		int ImplicitProperty { get; }

		/// <summary>Explicit method test</summary>
		void ExplicitMethod();

		/// <summary>Implicit method test</summary>
		void ImplicitMethod();
	}

	/// <summary>Explicit interface test (internal)</summary>
	internal interface ExplicitInternalInterface
	{
		/// <summary>Explicit method test</summary>
		void ExplicitMethodOfInternalInterface();
	}

	/// <summary>Testing explicit interface implementations</summary>
	public class ExplicitImplementation : ExplicitInterface, ExplicitInternalInterface
	{
		/// <summary>an explicitly implemented property</summary>
		int ExplicitInterface.ExplicitProperty
		{
			get { return 0; }
		}

		/// <summary>an implicitely implemented property</summary>
		public int ImplicitProperty { get { return -1; } }

		/// <summary>an explicitly implemented method</summary>
		void ExplicitInterface.ExplicitMethod()
		{
		}

		/// <summary>an implicitely implemented method</summary>
		public void ImplicitMethod()
		{
		}

		/// <summary>an explicitly implemented method of an internal interface</summary>
		void ExplicitInternalInterface.ExplicitMethodOfInternalInterface()
		{
		}
	}

	/// <summary>Test the new overloads tag.</summary>
	public class OverloadsTag
	{
		/// <overloads>This constructor is overloaded.</overloads>
		/// 
		/// <summary>This overloaded constructor accepts no parameters.</summary>
		public OverloadsTag()
		{
		}
		/// <summary>This overloaded constructor accepts one int parameter.</summary>
		public OverloadsTag(int i)
		{
		}

		/// <overloads>This indexer is overloaded.</overloads>
		/// 
		/// <summary>This overloaded indexer accepts one int parameter.</summary>
		public int this[int i]
		{
			get { return 0; }
		}
		/// <summary>This overloaded indexer accepts one string parameter.</summary>
		public int this[string s]
		{
			get { return 0; }
		}

		/// <overloads>
		/// This method is overloaded.
		/// </overloads>
		/// 
		/// <summary>This overload accepts no parameters.</summary>
		public void OverloadedMethod()
		{
		}
		/// <summary>This overload accepts one int parameter.</summary>
		public void OverloadedMethod(int i)
		{
		}

		/// <summary>This method is not overloaded and should not override
		/// the OverloadedMethod(int i) page.</summary>
		public void OverloadedMethod2()
		{
		}

		/// <overloads>
		///   <summary>This method is overloaded.</summary>
		///   <remarks>
		///		<para>This remark should also appear.</para>
		///     <note>This is a note.</note>
		///   </remarks>
		///   <example>
		///     <para>This is some example code.</para>
		///     <code>Foo.Bar.Baz.Quux();</code>
		///   </example>
		/// </overloads>
		/// 
		/// <summary>This overload accepts no parameters.</summary>
		public void FullDocOverloadedMethod()
		{
		}
		/// <summary>This overload accepts one int parameter.</summary>
		public void FullDocOverloadedMethod(int i1)
		{
		}
		/// <summary>This overload accepts one int parameter.</summary>
		public void FullDocOverloadedMethod(int i1, int i2)
		{
		}
		/// <summary>This overload accepts one int parameter.</summary>
		public void FullDocOverloadedMethod(int i1, int i2, int i3)
		{
		}

		/// <overloads>The Addition for <b>OverloadsTag</b>.</overloads>
		/// 
		/// <summary>
		/// Addition that takes two <b>OverloadsTag</b>.
		/// </summary>
		public static OverloadsTag operator +(OverloadsTag x, OverloadsTag y)
		{
			return new OverloadsTag();
		}
		/// <summary>
		/// Addition that takes an <b>OverloadsTag</b> and an <b>Int32</b>.
		/// </summary>
		public static OverloadsTag operator +(OverloadsTag x, int i)
		{
			return new OverloadsTag(i);
		}
	}

	/// <summary>This class uses note elements on its members.</summary>
	public class NotesTest
	{
		/// <summary>
		///   <para>This summary has a note.</para>
		///   <note>This is a test of a note tag which has lots of text in it and so should, if we are lucky, wrap to more than one line. To ensure this, here is another extremely long sentence that is probably gramatically incorrect and may have terrible spelling, but who cares, it is long isn't it?</note>
		/// </summary>
		public void NoteInSummary()
		{
		}

		/// <summary>This method has a note in its remarks.</summary>
		/// <remarks>
		///   <para>These remarks have a note.</para>
		///   <note>This is a test of a note tag which has lots of text in it and so should, if we are lucky, wrap to more than one line. To ensure this, here is another extremely long sentence that is probably gramatically incorrect and may have terrible spelling, but who cares, it is long isn't it?</note>
		/// </remarks>
		public void NoteInRemarks()
		{
		}

		/// <summary>This method has cautionary note in its remarks.</summary>
		/// <remarks>
		///   <para>These remarks have a cautionary note.</para>
		///   <note type="caution">This is a test of a note tag which has lots of text in it and so should, if we are lucky, wrap to more than one line. To ensure this, here is another extremely long sentence that is probably gramatically incorrect and may have terrible spelling, but who cares, it is long isn't it?</note>
		/// </remarks>
		public void CautionNote()
		{
		}

		/// <summary>This method has a note in its remarks.</summary>
		/// <remarks>
		///   <para>These remarks have a note.</para>
		///   <note style="background-color:PaleGoldenrod;">This is a test of a note tag which has lots of text in it and so should, if we are lucky, wrap to more than one line. To ensure this, here is another extremely long sentence that is probably gramatically incorrect and may have terrible spelling, but who cares, it is long isn't it?</note>
		/// </remarks>
		public void NoteInRemarksColored()
		{
		}

	}

	/// <summary>This class has an indexer with a name other than Item.</summary>
	public class IndexerNotNamedItem
	{
		/// <summary>This indexer is not named Item.</summary>
		[System.Runtime.CompilerServices.IndexerName("MyItem")]
		public int this[int i]
		{
			get { return 0; }
		}
	}

	/// <summary>This is a private class.</summary>
	class PrivateClass
	{
		/// <summary>This is a public enum nested in a private class.</summary>
		/// <remarks>This type should not appear when DocumentInternals is false.</remarks>
		public enum PublicEnumInPrivateClass
		{
			/// <summary>Foo</summary>
			Foo,
			/// <summary>Bar</summary>
			Bar
		}
	}

	/// <summary>This class has a member that uses 2D rectangular arrays.</summary>
	public class Matrix
	{
		/// <summary>Returns the inverse of a matrix.</summary>
		/// <param name="matrix">A matrix.</param>
		/// <returns>The inverted matrix.</returns>
		public static double[,] Inverse2(double[,] matrix)
		{
			return null;
		}

		/// <summary>Returns the inverse of a matrix.</summary>
		/// <param name="matrix">A matrix.</param>
		/// <returns>The inverted matrix.</returns>
		public static double[,,] Inverse3(double[,,] matrix)
		{
			return null;
		}
	}

	/// <summary>This public class contains a private enum.</summary>
	public class PublicClassWithPrivateEnum
	{
		/// <summary>This is a private enum in a public class.</summary>
		private enum PrivateEnum
		{
			/// <summary>foo</summary>
			Foo
		}
	}

	/// <summary>This class has a method that accepts a ref to a byte array.</summary>
	public class RefToByteArrayTest
	{
		/// <summary>This method that accepts a ref to a byte array.</summary>
		/// <param name="array">A ref to a byte array.</param>
		public void RefToByteArray(ref byte[] array)
		{
		}
	}

	/// <summary>This class causes the &lt;PrivateImplementationDetails> 
	/// class to appear in the compiled assembly.</summary>
	public class PrivateImplementationDetails
	{
		static byte[] bar = new byte[] {1,2,3};
	}

	/// <summary>See <see cref="Enum.Foo" />.</summary>
	/// <remarks>The summary contains a cref to an enum member.</remarks>
	public class LinkToEnumMember
	{
	}

	/// <summary>See <see cref="SeeOverloadedStatic.StaticOverload" />.</summary>
	public class SeeOverloadedStatic
	{
		/// <summary>Overload one.</summary>
		public static void StaticOverload(int i)
		{
		}

		/// <summary>Overload two.</summary>
		public static void StaticOverload(string s)
		{
		}
	}

	/// <summary>This class contains constant fields.</summary>
	public class ConstFields
	{
		/// <summary>This is a constant string.</summary>
		public const string ConstString = "ConstString";
		/// <summary>This is a constant int.</summary>
		public const int    ConstInt = 5;
		/// <summary>This is a constant int.</summary>
		public const double ConstDouble1 = 3.14159265358979323846d;
		/// <summary>This is a constant int.</summary>
		public const double ConstDouble2 = 314159265358979323846d;
		/// <summary>This is a constant enum.</summary>
		public const EnumFlags ConstEnum =  EnumFlags.Foo;
		/// <summary>This is a constant with 2 enum flags.</summary>
		public const EnumFlags ConstEnum2 =  EnumFlags.Foo|EnumFlags.Bar;
		/// <summary>This is a constant decimal.</summary>
		public const decimal ConstDecimal = 3.14159265358979323846m;
		/// <summary>This is a constant decimal.</summary>
		public const decimal ConstDecimal2 = 314159265358979323846m;
	}

	/// <summary>
	/// A class that inherits fields from a System class.
	/// </summary>
	public class InheritedFields : System.Resources.ResourceManager
	{
		/// <summary>
		/// This field is added for comparison.
		/// </summary>
		public static readonly int MyField = 5;
	}

	/// <summary>
	/// Demonstrates overloads with different access.
	/// </summary>
	public class OverloadsWithDifferentAccess
	{
		/// <summary>
		/// Public method with int param.
		/// </summary>
		/// <param name="index">Some int value</param>
		public void SomeMethod(int index)
		{
		}
		/// <summary>
		/// Protected method with string param.
		/// </summary>
		/// <param name="name">Some string value</param>
		protected void SomeMethod(string name)
		{
		}
		/// <summary>
		/// Public static method with double param.
		/// </summary>
		/// <param name="index">Some double value</param>
		public static void SomeMethod(double index)
		{
		}
		/// <summary>
		/// Protected static method with Type param.
		/// </summary>
		/// <param name="name">Some Type value</param>
		protected static void SomeMethod(Type name)
		{
		}
	}
	/// <summary>
	/// This class should be visible - it does not have an exclude tag
	/// </summary>
	public class VisibleClass
	{
		/// <summary>
		/// This should be visible
		/// </summary>
		public int VisibleField;

		/// <summary>
		/// This should NOT be visible
		/// </summary>
		/// <exclude/>
		public int NotVisibleField;

		/// <summary>
		/// This should NOT be visible
		/// </summary>
		/// <exclude />
		public int NotVisibleField2;

		/// <summary>
		/// This should be visible
		/// </summary>
		public string VisibleProperty {
			get { return ""; }
			set { }
		}

		/// <summary>
		/// This should NOT be visible
		/// </summary>
		/// <exclude />
		public string NotVisibleProperty {
			get { return ""; }
			set { }
		}

		/// <summary>
		/// This should be visible
		/// </summary>
		/// <returns></returns>
		public int VisibleMethod(){return 0;}

		/// <summary>
		/// This should NOT be visible
		/// </summary>
		/// <returns></returns>
		/// <exclude/>
		public int NotVisibleMethod(){return 0;}

		/// <overloads>There should only be two overloads visible</overloads>
		/// <summary>
		/// This should be visible
		/// </summary>
		/// <returns></returns>
		public int Overload1(){return 0;}
		/// <summary>
		/// This should NOT be visible
		/// </summary>
		/// <param name="a"></param>
		/// <returns></returns>
		/// <exclude />
		public int Overload1(int a){return 0;}
		/// <summary>
		/// This should be visible
		/// </summary>
		/// <param name="a"></param>
		/// <returns></returns>
		public int Overload1(long a){return 0;}
	}

	/// <summary>
	/// This class should NOT be visible - it has an exclude tag
	/// </summary>
	/// <exclude/>
	public class NotVisibleClass
	{
		/// <summary>
		/// This should NOT be visible
		/// </summary>
		public int VisibleField;

	}

	/// <summary>
	/// This class has html tags in the remarks
	/// </summary>
	/// <remarks>
	/// <para><b>This text is surrounded by &lt;b&gt;</b></para>
	/// <para><i>This text is surrounded by &lt;i&gt;</i></para>
	/// <para><em>This text is surrounded by &lt;em&gt;</em></para>
	/// <para><strong>This text is surrounded by &lt;strong&gt;</strong></para>
	/// <para>this line has a break<br/>here</para>
	/// <para align="center">this line is centered</para>
	/// <para style="padding 2px;border:2px solid red;background-color:PaleGoldenrod">this has a red border and PaleGoldenrod background.</para>
	/// </remarks>
	public class HtmlTags
	{
	}

	/// <summary>
	/// This class has ecma block tags in the remarks
	/// </summary>
	/// <remarks>
	/// <block type="note">This is a test of an ECMA "note" block tag which has lots of text in it and so should, if we are lucky, wrap to more than one line. To ensure this, here is another extremely long sentence that is probably gramatically incorrect and may have terrible spelling, but who cares, it is long isn't it?</block>
	/// <block type="example">This is a test of an ECMA "example" block tag which has lots of text in it and so should, if we are lucky, wrap to more than one line. To ensure this, here is another extremely long sentence that is probably gramatically incorrect and may have terrible spelling, but who cares, it is long isn't it?</block>
	/// <block type="behaviors">This is a test of an ECMA "behaviors" block tag which has lots of text in it and so should, if we are lucky, wrap to more than one line. To ensure this, here is another extremely long sentence that is probably gramatically incorrect and may have terrible spelling, but who cares, it is long isn't it?</block>
	/// <block type="overrides">This is a test of an ECMA "overrides" block tag which has lots of text in it and so should, if we are lucky, wrap to more than one line. To ensure this, here is another extremely long sentence that is probably gramatically incorrect and may have terrible spelling, but who cares, it is long isn't it?</block>
	/// <block type="usage">This is a test of an ECMA "usage" block tag which has lots of text in it and so should, if we are lucky, wrap to more than one line. To ensure this, here is another extremely long sentence that is probably gramatically incorrect and may have terrible spelling, but who cares, it is long isn't it?</block>
	/// <block type="default">This is a test of an ECMA "default" block tag which has lots of text in it and so should, if we are lucky, wrap to more than one line. To ensure this, here is another extremely long sentence that is probably gramatically incorrect and may have terrible spelling, but who cares, it is long isn't it?</block>
	/// </remarks>
	public class EcmaBlocks
	{
	}
}

///// <summary>
///// This class is in the global (unqualified) namespace.
///// </summary>
//public class GlobalNamespaceClass
//{
//}









