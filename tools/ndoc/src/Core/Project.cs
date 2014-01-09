// Project.cs - project management code 
// Copyright (C) 2001  Jason Diamond
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
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Xml;
using System.Text;

namespace NDoc.Core
{
	/// <summary>Represents an NDoc project.</summary>
	public class Project
	{
		/// <summary>Initializes a new instance of the <see cref="Project"/> class.</summary>
		public Project()
		{
			_IsDirty = false;
			_referencePaths = new ReferencePathCollection();
			_namespaces = new Namespaces();
			_namespaces.ContentsChanged += new EventHandler(ContentsChanged);

			_AssemblySlashDocs = new AssemblySlashDocCollection();
			_AssemblySlashDocs.Cleared += new EventHandler(_AssemblySlashDocs_Cleared);
			_AssemblySlashDocs.ItemAdded += new AssemblySlashDocEventHandler(_AssemblySlashDocs_ItemAdded);
			_AssemblySlashDocs.ItemRemoved += new AssemblySlashDocEventHandler(_AssemblySlashDocs_ItemRemoved);
		}

		private string _projectFile;

		/// <summary>
		/// Gets or sets the project file.
		/// </summary>
		/// <value></value>
		public string ProjectFile 
		{
			get { return _projectFile; }
			set 
			{ 
				//set the base path for project Path Items
				if ((value != null) && (value.Length > 0))
					PathItemBase.BasePath=Path.GetDirectoryName(value);
				else
					PathItemBase.BasePath=null;

				_projectFile = value; 
			}
		} 

		#region 'Dirty' flag handling

		private bool _IsDirty;

		/// <summary>Raised when the project <see cref="IsDirty"/> state changes from <see langword="false"/> to <see langword="true"/>.</summary>
		public event ProjectModifiedEventHandler Modified;

		private void ContentsChanged(object sender, EventArgs e)
		{
			IsDirty = true;
		}

		/// <summary>Gets or sets a value indicating whether the contents of this project have been modified.</summary>
		/// <remarks>If a project is marked as 'dirty' then the GUI will ask to user if they wish to save the project before loading another, or exiting.</remarks>
		public bool IsDirty
		{
			get { return _IsDirty; }

			set
			{
				if (!_suspendDirtyCheck)
				{
					if (!_IsDirty && value)
					{
						_IsDirty = true;
						if (Modified != null) Modified(this, EventArgs.Empty);
					}
					else
					{
						_IsDirty = value;
					}
				}
			}
		}

		private bool _suspendDirtyCheck=false;

		/// <summary>
		/// Gets or sets a value indicating whether <see cref="IsDirty"/> is updated when a project property is modifed.
		/// </summary>
		/// <value>
		/// 	<see langword="true"/>, if changes to project properties should <b>not</b> update the value of <see cref="IsDirty"/>; otherwise, <see langword="false"/>.
		/// </value>
		/// <remarks>The default value of this property is <see langword="false"/>, however it is set to <see langword="true"/> during <see cref="Read"/> so a newly loaded project is not flagged as 'dirty'</remarks>
		public bool SuspendDirtyCheck 
		{
			get { return _suspendDirtyCheck; }
			set { _suspendDirtyCheck = value; }
		} 

		#endregion

		#region AssemblySlashDocs

		private AssemblySlashDocCollection _AssemblySlashDocs;

		/// <summary>
		/// Gets the collection of assemblies and documentation comment XML files in the project.
		/// </summary>
		/// <value>An <see cref="AssemblySlashDocCollection"/>.</value>
		public AssemblySlashDocCollection AssemblySlashDocs 
		{
			get { return _AssemblySlashDocs; }
		} 

		#endregion

		#region ReferencePaths

		/// <summary>
		/// A collection of directories that will be probed when attempting to load assemblies.
		/// </summary>
		internal ReferencePathCollection _referencePaths;
		/// <summary>Gets a collection of directories that will be probed when attempting to load assemblies.</summary>
		public ReferencePathCollection ReferencePaths
		{
			get
			{
				return _referencePaths;
			}
		}

		#endregion

		#region Fixed/Relative Paths

		/// <summary>
		/// Gets the base directory used for relative references.
		/// </summary>
		/// <value>
		/// The directory of the project file, or the current working directory 
		/// if the project was not loaded from a project file.
		/// </value>
		public string BaseDirectory 
		{
			get 
			{ 
				if (_projectFile == null) 
				{
					ProjectFile = Path.Combine(Directory.GetCurrentDirectory(),"Untitled.ndoc");
				}
				return Path.GetDirectoryName(ProjectFile);
			}
		}

		/// <summary>
		/// Combines the specified path with the <see cref="BaseDirectory"/> of 
		/// the <see cref="Project" /> to form a full path to file or directory.
		/// </summary>
		/// <param name="path">The relative or absolute path.</param>
		/// <returns>
		/// A rooted path.
		/// </returns>
		public string GetFullPath(string path) 
		{
			return PathUtilities.GetFullPath( BaseDirectory, path );
		}

		/// <summary>
		/// Gets the relative path of the passed path with respect to the <see cref="BaseDirectory"/> of 
		/// the <see cref="Project" />.
		/// </summary>
		/// <param name="path">The relative or absolute path.</param>
		/// <returns>
		/// A relative path.
		/// </returns>
		public string GetRelativePath(string path) 
		{
			return PathUtilities.GetRelativePath( BaseDirectory, path );
		}
		#endregion

		#region Namespaces

		private Namespaces _namespaces;

		/// <summary>
		/// Gets the project namespace summaries collection.
		/// </summary>
		/// <value></value>
		public Namespaces Namespaces 
		{
			get { return _namespaces; }
		} 

		#endregion

		#region Read from Disk

		/// <summary>Reads an NDoc project file from disk.</summary>
		public void Read(string filename)
		{
			Clear();

			ProjectFile = Path.GetFullPath(filename);

			XmlTextReader reader = null;

			// keep track of whether or not any assemblies fail to load
			CouldNotLoadAllAssembliesException assemblyLoadException = null;

			// keep track of whether or not any errors in documenter property values
			DocumenterPropertyFormatException documenterPropertyFormatExceptions = null;

			try
			{
				StreamReader streamReader = new StreamReader(filename);
				reader = new XmlTextReader(streamReader);

				reader.MoveToContent();
				reader.ReadStartElement("project");

				while (!reader.EOF)
				{
					if (reader.NodeType == XmlNodeType.Element)
					{
						switch (reader.Name)
						{
							case "assemblies" : 
								// continue even if we don't load all assemblies
								try
								{
									_AssemblySlashDocs.ReadXml(reader);
								}
								catch (CouldNotLoadAllAssembliesException e)
								{
									assemblyLoadException = e;
								}
								break;
							case "referencePaths" : 
								_referencePaths.ReadXml(reader);
								break;
							case "namespaces" : 
								//GetNamespacesFromAssemblies();
								Namespaces.Read(reader);
								break;
							case "documenters" : 
								// continue even if we have errors in documenter properties
								try
								{
									ReadDocumenters(reader);
								}
								catch (DocumenterPropertyFormatException e)
								{
									documenterPropertyFormatExceptions = e;
								}
								break;
							default : 
								reader.Read();
								break;
						}
					}
					else
					{
						reader.Read();
					}
				}
			}
			catch (Exception ex)
			{
				//Clear the project to ensure everything is back to default state
				Clear();

				throw new DocumenterException("Error reading in project file " 	+ filename + ".\n" + ex.Message, ex);
			}
			finally
			{
				if (reader != null)
				{
					reader.Close(); // Closes the underlying stream.
				}
			}

			if (assemblyLoadException != null)
			{
				throw assemblyLoadException;
			}

			if (documenterPropertyFormatExceptions != null)
			{
				throw documenterPropertyFormatExceptions;
			}

//			IsDirty = false;
		}

		private void ReadDocumenters(XmlReader reader)
		{
			string FailureMessages = "";

			while (!reader.EOF && !(reader.NodeType == XmlNodeType.EndElement && reader.Name == "documenters"))
			{
				if (reader.NodeType == XmlNodeType.Element && reader.Name == "documenter")
				{
					string name = reader["name"];
					IDocumenterInfo info = InstalledDocumenters.GetDocumenter(name);

					if (info != null)
					{
						IDocumenterConfig config = info.CreateConfig( this );

						reader.Read(); // Advance to next node.
						try
						{
							config.Read(reader);
						}
						catch (DocumenterPropertyFormatException e)
						{
							FailureMessages += name + " Documenter\n" + e.Message + "\n";
						}

						if ( _configs.ContainsKey( info.Name ) == false )
							_configs.Add( info.Name, config );
						else
							_configs[info.Name] = config;
					}
				}
				reader.Read();
			}

			if (FailureMessages.Length > 0)
				throw new DocumenterPropertyFormatException(FailureMessages);
		}

		private Hashtable _configs = new Hashtable();

		/// <summary>
		/// Event rasied when the <see cref="ActiveConfig"/> changes
		/// </summary>
		public event EventHandler ActiveConfigChanged;

		/// <summary>
		/// Raises the <see cref="ActiveConfigChanged"/> event
		/// </summary>
		protected virtual void OnActiveConfigChanged()
		{
			if ( ActiveConfigChanged != null )
				ActiveConfigChanged( this, EventArgs.Empty );
		}
		/// <summary>
		/// The active documenter type
		/// </summary>
		public IDocumenterInfo ActiveDocumenter
		{
			get
			{
				if ( _currentConfig != null )
					return _currentConfig.DocumenterInfo;

				return null;
			}
			set
			{
				_currentConfig = null;

				if ( value != null )
				{
					// add a new config for this type if not already present
					if ( _configs.ContainsKey( value.Name ) == false )
						_configs.Add( value.Name, value.CreateConfig( this ) );

					// set the active config
					_currentConfig = _configs[value.Name] as IDocumenterConfig;
				}

				OnActiveConfigChanged();
			}
		}

		private IDocumenterConfig _currentConfig;
		/// <summary>
		/// The curently active <see cref="IDocumenterConfig"/>
		/// </summary>
		public IDocumenterConfig ActiveConfig
		{
			get
			{
				return _currentConfig;
			}
		}

		#endregion

		#region Write to Disk

		/// <summary>Writes an NDoc project to a disk file.</summary>
		/// <remarks>A project is written to file in a 2 stage process;
		/// <list type="number">
		/// <item>The project data is serialised to an in-memory store.</item>
		/// <item>If no errors occured during serialization, the data is written to disk.</item>
		/// </list>
		/// <p>This technique ensures that any fatal error during serialization will not cause a
		/// a corrupt or incomplete project file to be written to disk.</p>
		/// </remarks>
		public void Write(string filename)
		{
			//save the previous project file location.
			//If an error occurs during serialization, we we need to restore this...
			string oldProjectFile = ProjectFile;

			//Let the project know where it is being stored. This is used when deriving
			//pathnames relative to the project file
			ProjectFile = Path.GetFullPath(filename);

			
			//We will assume an reasonable initial capacity of 8k,
			//So the store does not normally need to grow.
			MemoryStream tempDataStore = new MemoryStream(8192);
			XmlTextWriter writer = null;

			try
			{
				//open an xml text writer to the memory stream
				writer = new XmlTextWriter(tempDataStore,new UTF8Encoding( false ));
				writer.Formatting = Formatting.Indented;
				writer.Indentation = 4;

				writer.WriteStartElement("project");
				writer.WriteAttributeString("SchemaVersion", "1.3");

				//do not change the order of those lines!
				_AssemblySlashDocs.WriteXml(writer);
				_referencePaths.WriteXml(writer);
				Namespaces.Write(writer);
				WriteDocumenters(writer);

				writer.WriteEndElement();
				//Flush the writer - note that we cannot close it yet,
				//as that would also close the temporary data store
				writer.Flush();

				//OK, we have managed to create the project file in memory.
				//Now we can try to write it to disk
				using(Stream stream = File.Create(filename))
					tempDataStore.WriteTo(stream);
			}
			catch (Exception ex)
			{
				//ouch, something went horribly wrong!
				//restore the original filename
				ProjectFile = oldProjectFile ;

				throw new DocumenterException("Error saving project file.\n" + ex.Message, ex);
			}
			finally
			{
				if (writer != null)
					writer.Close(); // Closes the underlying stream.
			}

			IsDirty = false;
		}

		private void WriteDocumenters(XmlWriter writer)
		{
			if ( _configs.Count > 0 )
			{
				writer.WriteStartElement( "documenters" );

				foreach ( IDocumenterConfig documenter in _configs.Values )
					documenter.Write(writer);

				writer.WriteEndElement();
			}
		}

		#endregion

		/// <summary>Clears the project.</summary>
		public void Clear()
		{
			_AssemblySlashDocs.Clear();
			if (_namespaces != null) _namespaces = new Namespaces();
			if (_referencePaths != null) _referencePaths = new ReferencePathCollection();

			// cache the current active info object
			IDocumenterInfo currentInfo = null;
			if ( this._currentConfig != null )
				currentInfo = _currentConfig.DocumenterInfo;

			// create a new configs collection populated with new configs for each type
			Hashtable newConfigs = new Hashtable();
			foreach ( IDocumenterConfig config in _configs.Values )
				newConfigs.Add( config.DocumenterInfo.Name, config.DocumenterInfo.CreateConfig( this ) );

			_configs = newConfigs;

			// reset the active config
			if ( currentInfo != null )
				_currentConfig = _configs[currentInfo.Name] as IDocumenterConfig;

			IsDirty = false;
			ProjectFile = "";
		}

		private void _AssemblySlashDocs_Cleared(object sender, EventArgs e)
		{
			IsDirty = true;
		}

		private void _AssemblySlashDocs_ItemAdded(object sender, AssemblySlashDocEventArgs args)
		{
			IsDirty = true;
		}

		private void _AssemblySlashDocs_ItemRemoved(object sender, AssemblySlashDocEventArgs args)
		{
			IsDirty = true;
		}
	}

	/// <summary>Handles Project <see cref="Project.Modified"/> events.</summary>
	public delegate void ProjectModifiedEventHandler(object sender, EventArgs e);

	/// <summary>
	/// This exception is thrown when one or more assemblies can not be loaded.
	/// </summary>
	[Serializable]
	public class CouldNotLoadAllAssembliesException : ApplicationException
	{ 
		/// <summary/>
		public CouldNotLoadAllAssembliesException() { }

		/// <summary/>
		public CouldNotLoadAllAssembliesException(string message)
			: base(message) { }

		/// <summary/>
		public CouldNotLoadAllAssembliesException(string message, Exception inner)
			: base(message, inner) { }

		/// <summary/>
		protected CouldNotLoadAllAssembliesException(
			System.Runtime.Serialization.SerializationInfo info, 
			System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
	}


	/// <summary>
	/// This exception is thrown when there were invalid values in the documenter properties.
	/// </summary>
	[Serializable]
	public class DocumenterPropertyFormatException : ApplicationException
	{ 
		/// <summary/>
		public DocumenterPropertyFormatException() { }

		/// <summary/>
		public DocumenterPropertyFormatException(string message)
			: base(message) { }

		/// <summary/>
		public DocumenterPropertyFormatException(string message, Exception inner)
			: base(message, inner) { }

		/// <summary/>
		protected DocumenterPropertyFormatException(
			System.Runtime.Serialization.SerializationInfo info, 
			System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
	}
}
