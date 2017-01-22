using System;

using NDoc.Core;

namespace NDoc.Gui
{
	/// <summary>
	/// Interface for receiving status notification during a build
	/// </summary>
	public interface IBuildStatus
	{
		/// <summary>
		/// An exception occurred during the build
		/// </summary>
		/// <param name="e">The exception</param>
		void BuildException( Exception e );

		/// <summary>
		/// The build is completed
		/// </summary>
		/// <remarks>This is called on succes, failure and cancellation</remarks>
		void BuildComplete();

		/// <summary>
		/// Called when the build is cancelled
		/// </summary>
		void BuildCancelled();

		/// <summary>
		/// Reports build progress
		/// </summary>
		/// <param name="e">Build progress arguments</param>
		void ReportProgress( ProgressArgs e );
	}
}
		