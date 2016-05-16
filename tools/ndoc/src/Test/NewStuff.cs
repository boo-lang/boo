using System;

namespace NDoc.Test.NewStuff
{
	/// <summary>
	/// This is another example of namespace summary documentation,
	/// when the UseNamespaceDocSummaries flag is set.
	/// </summary>
	public class NamespaceDoc {}

	/// <summary>no comment</summary>
	public interface IInterfaceA
	{
		/// <summary>no comment</summary>
		void InheritedImplicitInterfaceMethod();
	}
	/// <summary>no comment</summary>
	public interface IInterfaceB
	{
		/// <summary>no comment</summary>
		void InheritedExplicitInterfaceMethod();
	}
	/// <summary>no comment</summary>
	public interface IInterfaceC
	{
		/// <summary>no comment</summary>
		void ImplicitInterfaceMethod();
	}
	/// <summary>no comment</summary>
	public interface IInterfaceD
	{
		/// <summary>no comment</summary>
		void ExplicitInterfaceMethod();
	}
	/// <summary>no comment</summary>
	public interface IInterfaceE
	{
		/// <summary>no comment</summary>
		void NewInterfaceMethod();
	}
	/// <summary>no comment</summary>
	public interface IInterfaceF
	{
		/// <summary>no comment</summary>
		void InterfaceMethodOverride();
	}
	/// <summary>
	/// Base class used to test the new modifier. See <see cref="NewDerived"/>
	/// for details. Members in this class are named to demonstrate syntax
	/// in the derived class.
	/// </summary>
	public class NewBase : IInterfaceA, IInterfaceB, IInterfaceE, IInterfaceF
	{
		/// <summary>no comment</summary>
		public class NewClass
		{
		}
		/// <summary>no comment</summary>
		public int this[int n]
		{
			get
			{
				return 0;
			}
			set
			{
			}
		}

		/// <summary>no comment</summary>
		public void NewPropertySNOKOM(int n)
		{
		}
		/// <summary>no comment</summary>
		public void NewVirtualPropertySNOKOM(int n)
		{
		}
		/// <summary>no comment</summary>
		public int NewMethodSNOKOM;
		/// <summary>no comment</summary>
		public int NewVirtualMethodSNOKOM;
		/// <summary>no comment</summary>
		public int NewFieldSNOKOM
		{
			get { return 0; }
			set { }
		}

		/// <summary>no comment</summary>
		public const int NewConst = 500;
		/// <summary>no comment</summary>
		public int NewField;

		/// <summary>no comment</summary>
		public int NewProperty
		{
			get { return 0; }
			set {}
		}
		/// <summary>no comment</summary>
		public virtual int OverrideProperty
		{
			get { return 0; }
			set {}
		}
		/// <summary>no comment</summary>
		public virtual int NewVirtualProperty
		{
			get { return 0; }
			set {}
		}

		/// <summary>no comment</summary>
		public void NewMethod(int n)
		{
		}
		/// <summary>no comment</summary>
		public virtual void OverrideMethod(int n)
		{
		}
		/// <summary>no comment</summary>
		public virtual void NewVirtualMethod(int n)
		{
		}
		/// <summary>no comment</summary>
		public void NewMethodWithOverload(int n)
		{
		}
		/// <summary>no comment</summary>
		public virtual void NewMethodWithOverload(double d)
		{
		}
		/// <summary>no comment</summary>
		public virtual void NewMethodWithOverload(long l)
		{
		}
		/// <summary>
		/// public void NewMethodWithOverload(short h)
		/// </summary>
		/// <param name="h"></param>
		public void NewMethodWithOverload(short h)
		{
		}

		/// <summary>no comment</summary>
		public void NewStaticMethod(int n)
		{
		}

		#region Implementation of IInterfaceA
		/// <summary>no comment</summary>
		public void InheritedImplicitInterfaceMethod()
		{
		
		}
		#endregion

		#region Implementation of IInterfaceB
		void IInterfaceB.InheritedExplicitInterfaceMethod()
		{
		
		}
		#endregion

		#region Implementation of IInterfaceE
		/// <summary>no comment</summary>
		public void NewInterfaceMethod()
		{
		
		}
		#endregion

		#region Implementation of IInterfaceF
		/// <summary>no comment</summary>
		public virtual void InterfaceMethodOverride()
		{
		
		}
		#endregion
	}

	/// <summary>
	/// This class provides new implementations for the base class members.
	/// </summary>
	public class NewDerived : NewBase, IInterfaceC, IInterfaceD
	{
		/// <summary>no comment</summary>
		new public class NewClass
		{
		}

		/// <summary>This indexer is new</summary>
		new public int this[int n]
		{
			get
			{
				return 0;
			}
			set
			{
			}
		}
		/// <summary>This indexer is an overload</summary>
		public int this[string n]
		{
			get
			{
				return 0;
			}
			set
			{
			}
		}

		/// <summary>no comment</summary>
		public const int Const = 500;
		/// <summary>no comment</summary>
		public new const int NewConst = 500;
		/// <summary>no comment</summary>
		public int Field;
		/// <summary>no comment</summary>
		public new int NewField;
		/// <summary>
		/// "Same Name Other Kind Of Member"
		/// </summary>
		public new int NewFieldSNOKOM;

		/// <summary>no comment</summary>
		public int Property
		{
			get { return 0; }
			set {}
		}
		/// <summary>no comment</summary>
		public new int NewProperty
		{
			get { return 0; }
			set {}
		}
		/// <summary>no comment</summary>
		public virtual int VirtualProperty
		{
			get { return 0; }
			set {}
		}
		/// <summary>no comment</summary>
		public override int OverrideProperty
		{
			get { return 0; }
			set {}
		}
		/// <summary>no comment</summary>
		public new virtual int NewVirtualProperty
		{
			get { return 0; }
			set {}
		}
		/// <summary>
		/// "Same Name Other Kind Of Member"
		/// </summary>
		public new int NewPropertySNOKOM
		{
			get { return 0; }
			set { }
		}
		/// <summary>
		/// "Same Name Other Kind Of Member"
		/// </summary>
		public new virtual int NewVirtualPropertySNOKOM
		{
			get { return 0; }
			set { }
		}
		/// <summary>no comment</summary>
		public void Method(int n)
		{
		}
		/// <summary>no comment</summary>
		public new void NewMethod(int n)
		{
		}
		/// <summary>no comment</summary>
		public virtual void VirtualMethod(int n)
		{
		}
		/// <summary>no comment</summary>
		public override void OverrideMethod(int n)
		{
		}
		/// <summary>no comment</summary>
		public new virtual void NewVirtualMethod(int n)
		{
		}
		/// <summary>
		/// "Same Name Other Kind Of Member"
		/// </summary>
		public new void NewMethodSNOKOM(int n)
		{
		}
		/// <summary>
		/// "Same Name Other Kind Of Member"
		/// </summary>
		public new virtual void NewVirtualMethodSNOKOM(int n)
		{
		}
		/// <summary>
		/// public void NewMethodWithOverload(string s)
		/// </summary>
		/// <param name="s"></param>
		public void NewMethodWithOverload(string s)
		{
		}
		/// <summary>
		/// new public void NewMethodWithOverload(int n)
		/// </summary>
		/// <param name="n"></param>
		public new void NewMethodWithOverload(int n)
		{
		}
		/// <summary>
		/// public virtual void NewMethodWithOverload(object o)
		/// </summary>
		/// <param name="o"></param>
		public virtual void NewMethodWithOverload(object o)
		{
		}
		/// <summary>
		/// public override void NewMethodWithOverload(double d)
		/// </summary>
		/// <param name="d"></param>
		public override void NewMethodWithOverload(double d)
		{
		}
		/// <summary>
		/// new public virtual void NewMethodWithOverload(long l)
		/// </summary>
		/// <param name="l"></param>
		public new virtual void NewMethodWithOverload(long l)
		{
		}

		/// <summary>no comment</summary>
		public new static void NewStaticMethod(int n)
		{
		}

		#region Implementation of IInterfaceC
		/// <summary>no comment</summary>
		public void ImplicitInterfaceMethod()
		{
		
		}
		#endregion

		#region Implementation of IInterfaceD
		void IInterfaceD.ExplicitInterfaceMethod()
		{
		
		}
		#endregion

		/// <summary>no comment</summary>
		public new void NewInterfaceMethod()
		{
		
		}

		/// <summary>no comment</summary>
		public override void InterfaceMethodOverride()
		{
		
		}
	}

	/// <summary>
	/// Do you see F?
	/// </summary>
	public class Base
	{
		/// <summary>no comment</summary>
		public static void F() {}
	}
	/// <summary>
	/// Now F is gone (private)!
	/// </summary>
	public class Derived: Base
	{
		new private static void F() {}   // Hides Base.F in Derived only
	}
	/// <summary>
	/// Where is F?
	/// </summary>
	public class MoreDerived: Derived
	{
	}
	/// <summary>
	/// Uses MoreDerived.F (actually Base.F)
	/// </summary>
	public class SomeClass
	{
		/// <summary>
		/// Works, so MoreDerived.F is there, but where?
		/// </summary>
		static void G() { MoreDerived.F(); }         // Invokes Base.F
	}
}
