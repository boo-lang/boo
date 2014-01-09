// MsdnDocumenter.cs - a MSDN-like documenter
// Copyright (C) 2003 Don Kackman
// Parts copyright 2001  Kral Ferch, Jason Diamond
//
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA

using System;
using System.IO;
using System.Diagnostics;

using Microsoft.Win32;

namespace NDoc.Documenter.NativeHtmlHelp2.Compiler
{
	/// <summary>
	/// HxObject is the base class wrapper around the HTML Help v2 compiler
	/// executables that ship with the HTML v2 SDK
	/// </summary>
	public abstract class HxObject : object
	{

		private string _AppName = String.Empty;

		static HxObject()
		{
			string path = Path.Combine(
												Environment.GetFolderPath( Environment.SpecialFolder.ProgramFiles ),
												"Microsoft Help 2.0 SDK" );

			try
			{
				if ( !HxPathIsGood( path ) )
				{
					path = HxPathFromRegistry;
			
					HxCompFound = HxPathIsGood( path );
				}
				else
				{
					HxCompFound = true;
				}

				if ( HxCompFound )
					_HtmlHelp2CompilerPath = path;
			}
			catch ( Exception )
			{
				HxCompFound = false;
			}
		}


		private static bool HxCompFound = false;
		/// <summary>
		/// Determines if the Html Help 2 compiler was found
		/// </summary>
		public static bool HxCompIsInstalled
		{
			get
			{
				return HxCompFound;
			}
		}

		private static string _HtmlHelp2CompilerPath; 
		/// <summary>
		/// Returns the path to the Html Help 2 compiler
		/// </summary>
		protected static string HtmlHelp2CompilerPath
		{
			get
			{
				return _HtmlHelp2CompilerPath;
			}
		}

		private static bool HxPathIsGood( string path )
		{
			return File.Exists( Path.Combine( path, "hxcomp.exe" ) );
		}

		private static string HxPathFromRegistry
		{
			get
			{
				RegistryKey key = Registry.ClassesRoot.OpenSubKey( "Hxcomp.HxComp" );
				if ( key != null )
				{
					key = key.OpenSubKey( "CLSID" );
					if ( key != null )
					{
						object val = key.GetValue( null );
						if ( val != null )				
						{
							string clsid = (string)val;
							key = Registry.ClassesRoot.OpenSubKey( "CLSID" );
							if ( key != null )
							{
								key = key.OpenSubKey(clsid);
								if ( key != null )
								{
									key = key.OpenSubKey( "LocalServer32" );
									if ( key != null )
									{
										val = key.GetValue( null );
										if ( val != null )
										{
											return Path.GetDirectoryName( (string)val );
										}
									}
								}
							}
						}
					}
				}

				//still not finding the compiler, give up
				throw new Exception(
					"Unable to find the HTML Help 2 Compiler. Please verify that the Microsoft Visual Studio .NET Help Integration Kit has been installed.");			
			}
		}

		/// <summary>
		/// Create a new instance of the HxObject class
		/// </summary>
		/// <param name="appName">The name of the executable that implements 
		/// the functionality wrapped by an HxObject derived class</param>
		public HxObject( string appName )
		{
			if ( !HxObject.HxCompIsInstalled ) 
				throw new Exception( "VSHIK is not installed" );
			
			if ( !File.Exists( Path.Combine( HtmlHelp2CompilerPath, appName ) ) )
				throw new ArgumentException( "Could not find the specified compiler:" + appName, "appName" );

			_AppName = appName;
		}


		/// <summary>
		/// The full path and file name of the Hx executable file
		/// </summary>
		protected string CompilerEXEPath{ get{ return Path.Combine( HtmlHelp2CompilerPath, AppName ); } }

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
