using System;

namespace NDoc.Test.ThreadSafety
{
	/// <summary>
	/// The classes in this namesapce test the various permuations
	/// of threadsafety specification
	/// </summary>
	public class NamespaceDoc {}

	/// <summary>
	/// This class is thread safe for everything
	/// </summary>
	/// <threadsafety static="true" instance="true">
	/// <para>Here's some extra information about using this class across threads</para>
	/// </threadsafety>
	public class ThreadSafe
	{

	}
	/// <summary>
	/// This class is not thread safe
	/// </summary>
	/// <threadsafety static="false" instance="false">
	/// <para>Here's some extra information about using this class across threads</para>
	/// </threadsafety>
	public class NotThreadSafe
	{

	}
	/// <summary>
	/// This class is not thread safe
	/// </summary>
	/// <threadsafety static="true" instance="false">
	/// <para>Here's some extra information about using this class across threads</para>
	/// </threadsafety>
	public class StaticSafeInstanceNot
	{

	}
	/// <summary>
	/// This class is not thread safe
	/// </summary>
	/// <threadsafety static="false" instance="true">
	/// <para>Here's some extra information about using this class across threads</para>
	/// </threadsafety>
	public class StaticNotInstanceSafe
	{

	}
}
