using System;
using System.IO;
using System.Collections;
using System.Diagnostics;
using System.Reflection;

namespace NDoc.Core
{
	/// <summary>
	/// Static class containing the collection of documenters 
	/// currently avialable
	/// </summary>
	public sealed class InstalledDocumenters
	{
		/// <summary>
		/// Holds the list of directories that will be scanned for documenters.
		/// </summary>
		private static ArrayList _probePath = new ArrayList();
		private static ArrayList _Documenters;

		static InstalledDocumenters()
		{
			_Documenters = FindDocumenters();
		}

		private InstalledDocumenters()
		{
		}

		/// <summary>
		/// Gets the list of available documenters.
		/// </summary>
		public static ArrayList Documenters
		{
			get
			{
				return _Documenters;
			}
		}

		/// <summary>
		/// Find a documenter by name
		/// </summary>
		/// <param name="name">The name to search for</param>
		/// <returns>An IdocumenterInfo describing the documenter</returns>
		public static IDocumenterInfo GetDocumenter(string name)
		{
			foreach ( IDocumenterInfo documenter in _Documenters )
			{
				if ( documenter.Name == name )
					return documenter;
			}

			return null;
		}

		/// <summary>
		/// Appends the specified directory to the documenter probe path.
		/// </summary>
		/// <param name="path">The directory to add to the probe path.</param>
		/// <exception cref="ArgumentNullException"><paramref name="path" /> is <see langword="null" />.</exception>
		/// <exception cref="ArgumentException"><paramref name="path" /> is a zero-length <see cref="string" />.</exception>
		/// <remarks>
		/// <para>
		/// The probe path is the list of directories that will be scanned for
		/// assemblies that have classes implementing <see cref="IDocumenter" />.
		/// </para>
		/// </remarks>
		private static void AppendProbePath(string path) 
		{
			if (path == null)
				throw new ArgumentNullException("path");

			if (path.Length == 0)
				throw new ArgumentException("A zero-length string is not a valid value.", "path");

			// resolve relative path to full path
			string fullPath = Path.GetFullPath(path);

			if (!_probePath.Contains(fullPath)) 
				_probePath.Add(fullPath);
		}

		/// <summary>
		/// Searches the module directory and all directories in the probe path
		/// for assemblies containing classes that implement <see cref="IDocumenter" />.
		/// </summary>
		/// <returns>
		/// An <see cref="ArrayList" /> containing new instances of all the 
		/// found documenters.
		/// </returns>
		private static ArrayList FindDocumenters()
		{
			ArrayList documenters = new ArrayList();

			string mainModuleDirectory = System.Windows.Forms.Application.StartupPath;

			// make sure module directory is probed
			AppendProbePath( mainModuleDirectory );

			// scan all assemblies in probe path for documenters
			foreach (string path in _probePath) 
				FindDocumentersInPath( documenters, path );

			// sort documenters
			documenters.Sort();

			return documenters;
		}

		/// <summary>
		/// Searches the specified directory for assemblies containing classes 
		/// that implement <see cref="IDocumenter" />.
		/// </summary>
		/// <param name="documenters">The collection of <see cref="IDocumenter" /> instances to fill.</param>
		/// <param name="path">The directory to scan for assemblies containing documenters.</param>
		private static void FindDocumentersInPath( ArrayList documenters, string path) 
		{
			foreach (string fileName in Directory.GetFiles(path, "NDoc.Documenter.*.dll")) 
			{
				Assembly assembly = null;

				try
				{
					assembly = Assembly.LoadFrom(fileName);
				}
				catch (BadImageFormatException) 
				{
					// The DLL must not be a .NET assembly.
					// Don't need to do anything since the
					// assembly reference should still be null.
					Debug.WriteLine("BadImageFormatException loading " + fileName);
				}

				if (assembly != null) 
				{
					try 
					{
						foreach (Type type in assembly.GetTypes()) 
						{
							if (type.IsClass && !type.IsAbstract && (type.GetInterface("NDoc.Core.IDocumenterInfo") != null))
							{
								IDocumenterInfo documenter = Activator.CreateInstance(type) as IDocumenterInfo;
								if (documenter != null)
									documenters.Add(documenter);

								else
									Trace.WriteLine(String.Format("Documenter {0} in file {1} does not implement a current version of IDocumenter and so was not instantiated.", type.FullName, fileName));
							}
						}
					}
					catch (ReflectionTypeLoadException) 
					{
						// eat this exception and just ignore this assembly
						Debug.WriteLine("ReflectionTypeLoadException reflecting " + fileName);
					}
				}
			}
		}
	}
}