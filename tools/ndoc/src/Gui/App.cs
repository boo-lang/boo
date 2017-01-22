// App.cs - Application startup and properties
//
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
using System.Text;
using System.Reflection;
using System.Windows.Forms;
using System.Globalization;
using System.Collections;
using System.Diagnostics;
using System.Threading;

namespace NDoc.Gui
{
	/// <summary>
	/// Static class with the Main method and application properties
	/// </summary>
	public sealed class App
	{
		private App() 
		{
		}

		/// <summary>
		/// Application entry point
		/// </summary>
		/// <param name="args">Command line arguments</param>
		[STAThread]
		public static void Main(string[] args)
		{
			Thread.CurrentThread.Name = "GUI";

			Application.Run( new MainForm( (args.Length == 1) ? args[0] : null ) );
		}

		/// <summary>
		/// The NDoc SourceForge URI
		/// </summary>
		public static string WebSiteUri
		{
			get
			{
				return "http://ndoc.sourceforge.net";
			}
		}

		/// <summary>
		/// The location where the application is running 
		/// </summary>
		public static string RuntimeLocation
		{
			get
			{
				Uri uri = new Uri( Assembly.GetExecutingAssembly().CodeBase, true );
				return Path.GetDirectoryName( uri.AbsolutePath );
			}
		}

		/// <summary>
		/// The path to the license file
		/// </summary>
		public static string LicenseFilePath
		{
			get
			{
				// first try to locate license file in directory in which NDocGui is located
				string path = Path.Combine( RuntimeLocation, "gpl.rtf" );
				if ( File.Exists( path ) == false ) 
					path = GetSourceTreePath( @"setup\gpl.rtf" );

				return path;
			}
		}

		/// <summary>
		/// Returns a path to a file based resource that
		/// is sompatible with the source tree structure, rather than the
		/// deploymeed strucutre
		/// </summary>
		/// <param name="fileName">The file to build a path for</param>
		/// <returns>Relative path to the file in the source tree</returns>
		public static string GetSourceTreePath(string fileName)
		{
			return Path.Combine(
				RuntimeLocation, 
				string.Format( CultureInfo.InvariantCulture, "..{0}..{0}..{0}{1}", Path.DirectorySeparatorChar, fileName ) );
		}

		/// <summary>
		/// The path to the help file
		/// </summary>
		public static string HelpFilePath
		{
			get
			{
				// first try to locate help file in directory in which NDocGui is located
				string path = Path.Combine( RuntimeLocation, "NDocUsersGuide.chm" );
				if ( File.Exists( path ) == false ) 
					path = GetSourceTreePath( @"UsersGuide\NDocUsersGuide.chm" );

				return path;
			}
		}

		/// <summary>
		/// Given an <see cref="Exception"/> returns the deepest
		/// non-null InnerException
		/// </summary>
		/// <param name="e">The top Exception</param>
		/// <returns>The innermost Exception</returns>
		public static Exception GetInnermostException( Exception e )
		{
			if ( e == null )
				return null;

			return WalkExceptionStack( e, e.InnerException );
		}

		private static Exception WalkExceptionStack( Exception parent, Exception inner )
		{
			Debug.Assert( parent != null );

			if ( inner == null )
				return parent;

			return WalkExceptionStack( inner, inner.InnerException );
		}

		/// <summary>
		/// Streams an exception
		/// </summary>
		/// <param name="strBld">A StringBuilder to hold the exception details</param>
		/// <param name="ex">The Exception</param>
		public static void DumpException(StringBuilder strBld, Exception ex)
		{
			if (ex != null)
			{
				strBld.Append("\r\n\r\n");
				Exception tmpEx = ex;
				while (tmpEx != null)
				{
					strBld.AppendFormat("Exception: {0}\r\n", tmpEx.GetType());
					strBld.Append(tmpEx.Message);
					strBld.Append("\r\n\r\n");
					tmpEx = tmpEx.InnerException;
				}
				strBld.Append("\r\n");
			}

			ReflectionTypeLoadException rtle = ex as ReflectionTypeLoadException;
			if (rtle != null)
			{
				Hashtable fileLoadExceptions = new Hashtable();
				foreach (Exception loaderEx in rtle.LoaderExceptions)
				{
					System.IO.FileLoadException fileLoadEx = loaderEx as System.IO.FileLoadException;
					if (fileLoadEx != null)
					{
						if (fileLoadExceptions.ContainsKey(fileLoadEx.FileName) == false)
						{
							fileLoadExceptions.Add(fileLoadEx.FileName, null);
							strBld.AppendFormat("Unable to load: {0}\r\n", fileLoadEx.FileName);
						}
					}
					strBld.Append(loaderEx.Message + "\r\n");
					strBld.Append(loaderEx.StackTrace + Environment.NewLine);
				}
			}
		}

		/// <summary>
		/// Dumps the details of an exception to the Trace window
		/// </summary>
		/// <param name="ex">The Exception</param>
		public static void BuildTraceError(Exception ex)
		{
			System.Text.StringBuilder strBld = new System.Text.StringBuilder();

			App.DumpException(strBld, ex);

			if (ex != null) 
			{
				strBld.Append("\r\n");
				Exception tmpEx = ex;
				while (tmpEx != null)
				{
					strBld.AppendFormat("Exception: {0}\r\n", tmpEx.GetType());
					strBld.Append(tmpEx.StackTrace.Replace("\r\n", "\n").Replace("\r", "\n").Replace("\n", "\r\n"));
					strBld.Append("\r\n\r\n");
					tmpEx = tmpEx.InnerException;
				}
			}

			Trace.WriteLine(strBld);
		}
	}
}
