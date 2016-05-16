// Copyright (C) 2004  Kevin Downs
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
using System.Collections;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Globalization;
using System.Reflection;
using System.Security;
using System.Security.Permissions;
using System.Security.Policy;

namespace NDoc.Core.Reflection
{
	/// <summary>
	/// Handles the resolution and loading of assemblies.
	/// </summary>
	internal class AssemblyLoader
	{
		/// <summary>primary search directories.</summary>
		private ReferencePathCollection SearchDirectories;

		/// <summary>List of subdirectory lists already scanned.</summary>
		private Hashtable directoryLists;

		/// <summary>List of directories already scanned.</summary>
		private Hashtable searchedDirectories;

		/// <summary>List of Assemblies that could not be resolved.</summary>
		private Hashtable unresolvedAssemblies;

		/// <summary>assemblies already scanned, but not loaded.</summary>
		/// <remarks>Maps Assembly FullName to Filename for assemblies scanned, 
		/// but not loaded because they were not a match to the required FullName.
		/// <p>This list is scanned twice,</p>
		/// <list type="unordered">
		/// <term>If the requested assembly has not been loaded, but is in this list, then the file is loaded.</term>
		/// <term>Once all search paths have been exhausted in an exact name match, this list is checked for a 'partial' match.</term>
		/// </list></remarks>
		private Hashtable AssemblyNameFileNameMap;

		// loaded assembly cache keyed by Assembly FileName
		private Hashtable assemblysLoadedFileName;

		/// <summary>
		/// Initializes a new instance of the <see cref="AssemblyLoader"/> class.
		/// </summary>
		/// <param name="referenceDirectories">Reference directories.</param>
		public AssemblyLoader(ReferencePathCollection referenceDirectories)
		{
			this.assemblysLoadedFileName = new Hashtable();
			this.AssemblyNameFileNameMap = new Hashtable();
			this.directoryLists = new Hashtable();
			this.unresolvedAssemblies = new Hashtable();
			this.searchedDirectories = new Hashtable();

			this.SearchDirectories = referenceDirectories;
		}

		/// <summary>
		/// Directories Searched for assemblies.
		/// </summary>
		public ICollection SearchedDirectories 
		{
			get { return searchedDirectories.Keys; }
		}

		/// <summary>
		/// Assemblies that could not be resolved.
		/// </summary>
		public ICollection UnresolvedAssemblies 
		{
			get { return unresolvedAssemblies.Keys; }
		} 

		/// <summary> 
		/// Installs the assembly resolver by hooking up to the AppDomain's AssemblyResolve event.
		/// </summary>
		public void Install() 
		{
			AppDomain.CurrentDomain.AssemblyResolve += 
				new ResolveEventHandler(this.ResolveAssembly);
		}

		/// <summary> 
		/// Deinstalls the assembly resolver.
		/// </summary>
		public void Deinstall() 
		{
			AppDomain.CurrentDomain.AssemblyResolve -= 
				new ResolveEventHandler(this.ResolveAssembly);
		}

		/// <summary>Loads an assembly.</summary>
		/// <param name="fileName">The assembly filename.</param>
		/// <returns>The assembly object.</returns>
		/// <remarks>This method loads an assembly into memory. If you
		/// use Assembly.Load or Assembly.LoadFrom the assembly file locks.
		/// This method doesn't lock the assembly file.</remarks>
		public Assembly LoadAssembly(string fileName)
		{
			// have we already loaded this assembly?
			Assembly assy = assemblysLoadedFileName[fileName] as Assembly;

			//double check assy not already loaded
			if (assy == null)
			{
				AssemblyName assyName = AssemblyName.GetAssemblyName(fileName);
				foreach (Assembly loadedAssy in AppDomain.CurrentDomain.GetAssemblies())
				{
					if (assyName.FullName == loadedAssy.FullName)
					{
						assy = loadedAssy;
						break;
					}
				}
			}
			
			// Assembly not loaded, so we must go a get it
			if (assy == null)
			{
				Trace.WriteLine(String.Format("LoadAssembly: {0}", fileName));

				// we will load the assembly image into a byte array, then get the CLR to load it
				// This allows us to side-step the host permissions which would otherwise prevent
				// loading from a network share...also we don't have the overhead over shadow-copying 
				// to avoid assembly locking
				FileStream assyFile = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read, 16384);
				byte[] bin = new byte[16384];
				long rdlen = 0;
				long total = assyFile.Length;
				int len;
				MemoryStream memStream = new MemoryStream((int)total);
				rdlen = 0;
				while (rdlen < total)
				{
					len = assyFile.Read(bin, 0, 16384);
					memStream.Write(bin, 0, len);
					rdlen = rdlen + len;
				}
				// done with input file
				assyFile.Close();

				
				// Now we have the assembly image, try to load it into the CLR
				try
				{
					Evidence evidence = CreateAssemblyEvidence(fileName);
					assy = Assembly.Load(memStream.ToArray(), null, evidence);
					// If the assembly loaded OK, cache the Assembly ref using the fileName as key.
					assemblysLoadedFileName.Add(fileName, assy);
				}
				catch (System.Security.SecurityException e)
				{
					if (e.Message.IndexOf("0x8013141A") != -1)
					{
						throw new System.Security.SecurityException(String.Format("Strong name validation failed for assembly '{0}'.", fileName));
					}
					else
						throw;
				}
				catch (System.IO.FileLoadException e)
				{
					// HACK: replace the text comparison with non-localized test when further details are available
					if ((e.Message.IndexOf("0x80131019") != -1) ||
					    (e.Message.IndexOf("contains extra relocations") != -1)) 
					{
						try
						{
							// LoadFile is really preferable, 
							// but since .Net 1.0 doesn't have it,
							// we have to use LoadFrom on that framework...
#if (NET_1_0)
							assy = Assembly.LoadFrom(fileName);
#else
							assy = Assembly.LoadFile(fileName);
#endif
						}
						catch (Exception e2)
						{
							throw new DocumenterException(string.Format(CultureInfo.InvariantCulture, "Unable to load assembly '{0}'", fileName), e2);
						}
					}
					else
						throw new DocumenterException(string.Format(CultureInfo.InvariantCulture, "Unable to load assembly '{0}'", fileName), e);
				}
				catch (Exception e)
				{
					throw new DocumenterException(string.Format(CultureInfo.InvariantCulture, "Unable to load assembly '{0}'", fileName), e);
				}
 
			}

			return assy;
		}

		static Evidence CreateAssemblyEvidence(string fileName)
		{
			//HACK: I am unsure whether 'Hash' evidence is required - since this will be difficult to obtain, we will not supply it...
 
			Evidence newEvidence = new Evidence();

			//We must have zone evidence, or we will get a policy exception
			Zone zone = new Zone(SecurityZone.MyComputer);
			newEvidence.AddHost(zone);

			//If the assembly is strong-named, we must supply this evidence
			//for StrongNameIdentityPermission demands
			AssemblyName assemblyName = AssemblyName.GetAssemblyName(fileName);
			byte[] pk = assemblyName.GetPublicKey();
			if (pk!=null && pk.Length != 0)
			{
				StrongNamePublicKeyBlob blob = new StrongNamePublicKeyBlob(pk);
				StrongName strongName = new StrongName(blob, assemblyName.Name, assemblyName.Version);
				newEvidence.AddHost(strongName);
			}

			return newEvidence;
		}

		/// <summary> 
		/// Resolves the location and loads an assembly not found by the system.
		/// </summary>
		/// <remarks>The CLR will take care of loading Framework and GAC assemblies.
		/// <p>The resolution process uses the following heuristic</p>
		/// </remarks>
		/// <param name="sender">the sender of the event</param>
		/// <param name="args">event arguments</param>
		/// <returns>the loaded assembly, null, if not found</returns>
		protected Assembly ResolveAssembly(object sender, ResolveEventArgs args) 
		{

			// first, have we already loaded the required assembly?
			Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
			foreach (Assembly a in assemblies) 
			{
				if (IsAssemblyNameEquivalent(a.FullName, args.Name)) 
				{
					return a;
				}
			}

			Debug.WriteLine(
				"Attempting to resolve assembly " + args.Name + ".", 
				"AssemblyResolver");

			string fileName;

			// we may have already located the assembly but not loaded it...
			fileName = (string)AssemblyNameFileNameMap[args.Name];
			if (fileName != null && fileName.Length > 0)
			{
				return LoadAssembly((string)AssemblyNameFileNameMap[args.Name]);
			}

			string[] assemblyInfo = args.Name.Split(new char[] {','});

			string fullName = args.Name;
			
			Assembly assy = null;

			// first we will try filenames derived from the assembly name.
	
			// Project Path DLLs
			if (assy == null)
			{
				fileName = assemblyInfo[0] + ".dll";
				assy = LoadAssemblyFrom(fullName, fileName);
			}

			// Project Path Exes
			if (assy == null)
			{
				fileName = assemblyInfo[0] + ".exe";
				assy = LoadAssemblyFrom(fullName, fileName);
			}

			// Reference Path DLLs
			if (assy == null)
			{
				fileName = assemblyInfo[0] + ".dll";
				assy = LoadAssemblyFrom(fullName, fileName);
			}

			// Reference Path Exes
			if (assy == null)
			{
				fileName = assemblyInfo[0] + ".exe";
				assy = LoadAssemblyFrom(fullName, fileName);
			}

			//if the requested assembly did not have a strong name, we can
			//get even more desperate and start looking for partial name matches
			if (assemblyInfo.Length < 4 || assemblyInfo[3].Trim() == "PublicKeyToken=null")
			{
				if (assy == null)
				{
					//start looking for partial name matches in
					//the assemblies we have already loaded...
					assemblies = AppDomain.CurrentDomain.GetAssemblies();
					foreach (Assembly a in assemblies) 
					{
						string[] assemblyNameParts = a.FullName.Split(new char[] {','});
						if (assemblyNameParts[0] == assemblyInfo[0])
						{
							assy = a;
							break;
						}
					}
				}

				if (assy == null)
				{
					//get even more desperate and start looking for partial name matches
					//the assemblies we have already scanned...
					foreach (string assemblyName in AssemblyNameFileNameMap.Keys)
					{

						string[] assemblyNameParts = assemblyName.Split(new char[] {','});
						if (assemblyNameParts[0] == assemblyInfo[0])
						{
							assy = LoadAssembly((string)AssemblyNameFileNameMap[assemblyName]);
							break;
						}
					}
				}
			}

			if (assy == null)
			{
				if (!unresolvedAssemblies.ContainsKey(args.Name)) 
					unresolvedAssemblies.Add(args.Name, null);
			}

			return assy;
		}

		/// <summary> 
		/// Search for and load the specified assembly in a set of directories.
		/// This will optionally search recursively.
		/// </summary>
		/// <param name="fullName">
		/// Fully qualified assembly name. If not empty, the full name of each assembly found is
		/// compared to this name and the assembly is accepted only, if the names match.
		/// </param>
		/// <param name="fileName">The name of the assembly.</param>
		/// <returns>The assembly, or null if not found.</returns>
		private Assembly LoadAssemblyFrom(string fullName, string fileName) 
		{
			Assembly assy = null;
			
			if ((SearchDirectories == null) || (SearchDirectories.Count == 0)) return (null);

			foreach (ReferencePath rp in SearchDirectories)
			{
				if (Directory.Exists(rp.Path))
				{
					assy = LoadAssemblyFrom(fullName, fileName, rp.Path, rp.IncludeSubDirectories);
					if (assy != null) return assy;
				}
			}
			return null;
		}
		
		/// <summary> 
		/// Search for and load the specified assembly in a given directory.
		/// This will optionally search recursively into sub-directories if requested.
		/// </summary>
		/// <param name="path">The directory to look in.</param>
		/// <param name="fullName">
		/// Fully qualified assembly name. If not empty, the full name of each assembly found is
		/// compared to this name and the assembly is accepted only, if the names match.
		/// </param>
		/// <param name="fileName">The name of the assembly.</param>
		/// <param name="includeSubDirs">true, search subdirectories.</param>
		/// <returns>The assembly, or null if not found.</returns>
		private Assembly LoadAssemblyFrom(string fullName, string fileName, string path, bool includeSubDirs) 
		{
			Assembly assembly = null;
			if (!searchedDirectories.ContainsKey(path))
			{
				searchedDirectories.Add(path, null);
			}
			string fn = Path.Combine(path, fileName);
			if (File.Exists(fn)) 
			{
				// file exists, check it's the right assembly
				try
				{
					AssemblyName assyName = AssemblyName.GetAssemblyName(fn);
					if (IsAssemblyNameEquivalent(assyName.FullName, fullName))
					{
						//This looks like the right assembly, try loading it
						try
						{
							assembly = LoadAssembly(fn);
							return (assembly);
						}
						catch (Exception e)
						{
							Debug.WriteLine("Assembly Load Error: " + e.Message, "AssemblyResolver");
						}
					}
					else
					{
						//nope, names don't match; save the FileName and AssemblyName map
						//in case we need this assembly later...
						//only first found occurence of fully-qualifed assembly name is cached
						if (!AssemblyNameFileNameMap.ContainsKey(assyName.FullName))
						{
							AssemblyNameFileNameMap.Add(assyName.FullName, fn);
						}
					}
				}
				catch (Exception e)
				{
					//oops this wasn't a valid assembly
					Debug.WriteLine("AssemblyResolver: File " + fn + " not a valid assembly");
					Debug.WriteLine(e.Message);
				}
			}
			else
			{
				Debug.WriteLine("AssemblyResolver: File " + fileName + " not in " + path);
			}

			// not in this dir (or load failed), scan subdirectories
			if (includeSubDirs) 
			{
				string[] subdirs = GetSubDirectories(path);
				foreach (string subdir in subdirs)
				{
					Assembly assy = LoadAssemblyFrom(fullName, fileName, subdir, true);
					if (assy != null) return assy;
				}
			}

			return null;
		}

		private string[] GetSubDirectories(string parentDir) 
		{
			string[] subdirs = (string[])this.directoryLists[parentDir];
			if (null == subdirs) 
			{
				subdirs = Directory.GetDirectories(parentDir);
				this.directoryLists.Add(parentDir, subdirs);
			}
			return subdirs;
		}

		/// <summary>
		/// 
		/// </summary>
		private bool IsAssemblyNameEquivalent(string AssyFullName, string RequiredAssyName)
		{
			if (RequiredAssyName.Length < AssyFullName.Length)
				return (AssyFullName.Substring(0, RequiredAssyName.Length) == RequiredAssyName);
			else
				return (AssyFullName == RequiredAssyName.Substring(0, AssyFullName.Length));
		}

	}
}

