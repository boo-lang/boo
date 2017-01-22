using System;

namespace NDoc.Test.Unsafe
{
	/// <summary>
	/// The NDoc.Test.Unsafe namespace contains tests for types
	/// that expose pointer types and are marked with the "unsafe" keyword
	/// </summary>
	public class NamespaceDoc {}

	/// <summary>
	/// An interface with pointers in its method signatures
	/// </summary>
	public interface IUnsafe
	{
		/// <summary>
		/// Return a pointer to an Int32
		/// </summary>
		/// <returns>The pointer</returns>
		unsafe Int32* GetIntPointer();
	}

	/// <summary>
	/// An unsafe struct with a member of type pointer
	/// </summary>
	public struct UNSAFE
	{
		/// <summary>
		/// A pointer field
		/// </summary>
		unsafe public Int32* p;
	}

	/// <summary>
	/// This class has various mebers that are marked as unsafe and that return
	/// pointers
	/// </summary>
	public class ClassWithUnsafeMembers : IUnsafe
	{
		/// <summary>
		/// An unsafe constructor
		/// </summary>
		/// <param name="p">a pointer</param>
		unsafe public ClassWithUnsafeMembers( Int32* p )
		{

		}

		/// <summary>
		/// A public pointer field
		/// </summary>
		unsafe public Int32* pointerField;

		/// <summary>
		/// A property that is a pointer type
		/// </summary>
		unsafe public Int32* PointerProperty
		{
			get
			{
				return pointerField;
			}
			set
			{
				pointerField = value;
			}
		}
		/// <summary>
		/// Pass an unsafe pointer as a paramater
		/// </summary>
		/// <param name="p">A pointer to an int32</param>
		unsafe public void PassAPointer( Int32* p )
		{

		}
		/// <summary>
		/// unsafe method return
		/// </summary>
		/// <returns>The address of an int32</returns>
		unsafe public Int32* GetIntPointer()
		{
			return pointerField;
		}
	}
}