using System;

namespace NDoc.Test.Platforms
{
	/// <summary>
	/// The classes in the namespace demonstrate the various
	/// way to specify platform support
	/// </summary>
	public class NamespaceDoc {}

	/// <summary>
	/// This class can run safely on all .NET compatible operating systems and frameworks
	/// </summary>
	/// <platform>
	///    <os predefined="all" />
	///    <frameworks>
	///			<mono>true</mono>
	///			<compact>true</compact>
	///    </frameworks>  
	/// </platform>
	public class ThisClassSupportsAllPlatforms
	{
		/// <summary>
		/// This method should display the same list of platforms as the containing class
		/// </summary>
		public void ThisMethodShouldHaveTheSamePlatformAsTheClass(){}
	}

	/// <summary>
	/// This class can doesn't support the compact framework or Mono
	/// </summary>
	/// <platform>
	///    <os predefined="all" />
	/// </platform>
	public class ThisClassSupportsAllOSButNotCFOrMono
	{
		/// <summary>
		/// This method should display the same list of platforms as the containing class
		/// </summary>
		public void ThisMethodShouldHaveTheSamePlatformAsTheClass(){}
	}

	/// <summary>
	/// This class supports the CF and all OS's, but not Mono
	/// </summary>
	/// <platform>
	///    <os predefined="all" />
	///    <frameworks>
	///			<compact>true</compact>
	///    </frameworks>  
	/// </platform>
	public class ThisClassSupportsAllOsAndCF
	{
		/// <summary>
		/// This method should display the same list of platforms as the containing class
		/// </summary>
		public void ThisMethodShouldHaveTheSamePlatformAsTheClass(){}

		/// <summary>
		/// This method doesn't support the compact framework
		/// </summary>
		/// <platform>
		///		<frameworks>
		///			<compact>false</compact>
		///		</frameworks>
		/// </platform>
		public void ThisMethodDoesNotSupportTheCF(){}
	}

	/// <summary>
	/// This class supports the Mono and all OS's, but not CF
	/// </summary>
	/// <platform>
	///    <os predefined="all" />
	///    <frameworks>
	///			<mono>true</mono>
	///    </frameworks>
	/// </platform>
	public class ThisClassSupportsAllOsAndMono
	{
		/// <summary>
		/// This method should display the same list of platforms as the containing class
		/// </summary>
		public void ThisMethodShouldHaveTheSamePlatformAsTheClass(){}
	}

	/// <summary>
	/// This class can only run on an NT5 derived OS
	/// </summary>
	/// <platform>
	///    <os predefined="nt5plus" />
	/// </platform>
	public class ThisClassNeedsNT5
	{
		/// <summary>
		/// This method should display the same list of platforms as the containing class
		/// </summary>
		public void ThisMethodShouldHaveTheSamePlatformAsTheClass(){}
	}

	/// <summary>
	/// This class needs an enterprise OS
	/// </summary>
	/// <platform>
	///    <os predefined="enterprise" />
	/// </platform>
	public class ThisClassNeedsAServerOS
	{
		/// <summary>
		/// This method should display the same list of platforms as the containing class
		/// </summary>
		public void ThisMethodShouldHaveTheSamePlatformAsTheClass(){}
	}

	/// <summary>
	/// This class supports a NT 5 and a custom list of platforms
	/// </summary>
	/// <platform>
	///    <os predefined="nt5plus">MS-DOS 2.0</os>
	///    <frameworks>
	///		<custom>DOS.NET Framework</custom>
	///    </frameworks>
	/// </platform>
	public class ThisClassSupportsACustomPlatformList
	{
		/// <summary>
		/// This method should display the same list of platforms as the containing class
		/// </summary>
		public void ThisMethodShouldHaveTheSamePlatformAsTheClass(){}
	}

	/// <summary>
	/// This class doesn't designate platform support and 
	/// should use whatever setting is specified as the
	/// project default
	/// </summary>
	public class ThisClassHasNoSpecificPlatform
	{
		/// <summary>
		/// This method should display the same list of platforms as the containing class
		/// </summary>
		public void ThisMethodShouldHaveTheProjectDefaultPlatofrmSupport(){}
	}
}
