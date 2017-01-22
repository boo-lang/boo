using System;
using System.IO;
using System.Diagnostics;

namespace NDoc.Documenter.HtmlHelp2.Compiler
{
	/// <summary>
	/// HxObject is the base class wrapper around the HTML Help v2 compiler
	/// executables that ship with the HTML v2 SDK
	/// </summary>
	public abstract class HxObject : object
	{
		private string _CompilerPath = String.Empty;
		private string _AppName = String.Empty;

		/// <summary>
		/// Create a new instance of the HxObject class
		/// </summary>
		/// <param name="compilerPath">See <see cref="CompilerPath"/></param>
		/// <param name="appName">The name of the executable that implements 
		/// the functionality wrapped by an HxObject derived class</param>
		public HxObject( string compilerPath, string appName )
		{
			if ( !Directory.Exists( compilerPath ) )
				throw new ArgumentException( "The specifed directory does not exist:" + compilerPath, "compilerPath" );
			
			if ( !File.Exists( Path.Combine( compilerPath, appName ) ) )
				throw new ArgumentException( "Could not find the specified compiler:" + appName, "appName" );

			_CompilerPath = compilerPath;
			_AppName = appName;
		}

		/// <summary>
		/// The location of the executable file
		/// <see cref="AppName"/>
		/// </summary>
		public string CompilerPath{ get{ return _CompilerPath; } }

		/// <summary>
		/// The full path and file name of the Hx executable file
		/// </summary>
		protected string CompilerEXEPath{ get{ return Path.Combine( CompilerPath, AppName ); } }

		/// <summary>
		/// The name of the executable that the class wraps
		/// </summary>
		public string AppName{ get{ return _AppName; } }

		/// <summary>
		/// Invokes the Hx executable (see <see cref="AppName"/>)
		/// </summary>
		/// <param name="arguments">The command line arguments to passed to the compiler</param>
		/// <param name="workingDirectory">The working directory for the process</param>
		protected void Execute( string arguments, string workingDirectory )
		{
			Trace.WriteLine( String.Format( "Executing '{0}' with arguments '{1}'", _AppName, arguments ) );
			
			Process HxProcess = new Process();

			try
			{
				ProcessStartInfo processStartInfo = new ProcessStartInfo();

				processStartInfo.FileName = CompilerEXEPath;
				processStartInfo.Arguments = arguments;
				processStartInfo.ErrorDialog = false;
				processStartInfo.WorkingDirectory = workingDirectory; 
				processStartInfo.WindowStyle = ProcessWindowStyle.Hidden;

				HxProcess.StartInfo = processStartInfo;

				// Start the executable and bail if it takes longer than 10 minutes.
				try
				{
					HxProcess.Start();
				}
				catch ( Exception e )
				{
					string msg = String.Format("An error occured while attempting to run {0}", _AppName);
					throw new Exception(msg, e);
				}

				if ( !HxProcess.WaitForExit( ProcessTimeout ) )
				{
					throw new Exception( string.Format( "{0} did not complete after {1} seconds and was aborted", _AppName, ProcessTimeout / 1000)  );
				}

				Trace.WriteLine( String.Format( "{0} returned an exit code of {1}", _AppName, HxProcess.ExitCode ) );
				
				// 0 means a successful exit
				if ( HxProcess.ExitCode != 0 )
				{
					throw new Exception( String.Format( "{0} returned an exit code of {1}", _AppName, HxProcess.ExitCode ) );
				}

			}
			finally
			{
				HxProcess.Close();
			}
		}

		/// <summary>
		/// The number of milliseconds to wait before timing out the process once Execute is called
		/// see <see cref="Execute"/>
		/// </summary>
		/// <remarks>Can be overridden by derived classes to provide custom timeout intervals</remarks>
		/// <value>600000</value>
		protected virtual int ProcessTimeout{ get{ return 600000; } }
		
	}
}
