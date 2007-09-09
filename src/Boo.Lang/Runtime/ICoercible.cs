namespace Boo.Lang.Runtime
{
	/// <summary>
	/// Support for user defined dynamic conversions.
	/// 
	/// An use case is a collection class that needs to provide implicit conversions
	/// to any array type.
	/// </summary>
	public interface ICoercible
	{
		/// <summary>
		/// Coerces the object to the specified type if possible.
		/// </summary>
		/// <param name="to">target type</param>
		/// <returns>returns the coerced object or this</returns>
		object Coerce(System.Type to);
	}
}
