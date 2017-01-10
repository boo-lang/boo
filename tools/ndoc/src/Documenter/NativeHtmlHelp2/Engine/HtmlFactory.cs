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
using System.Xml;
using System.Xml.Xsl;
using System.Xml.XPath;
using System.Text;
using System.Diagnostics;
using System.Collections;
using System.Collections.Specialized;

using NDoc.Core; //for DocumenterException
using NDoc.Core.Reflection;
using NDoc.Documenter.NativeHtmlHelp2.Engine.NamespaceMapping;

namespace NDoc.Documenter.NativeHtmlHelp2.Engine
{
	/// <summary>
	/// Deleagate for handling file events
	/// </summary>
	public delegate void TopicEventHandler( object sender, FileEventArgs args );

	/// <summary>
	/// The Html factory orchestrates the transformation of the NDoc Xml into the
	/// set of Html files that will be compiled into the help project
	/// </summary>
	public class HtmlFactory
	{
		private ArrayList documentedNamespaces;
		private string _outputDirectory;
		private ExternalHtmlProvider _htmlProvider;
		private StyleSheetCollection _stylesheets;

		private FileNameMapper  fileNameMapper;
		private NamespaceMapper nsMapper;

		private XmlDocument xmlDocumentation;			// the NDoc generates summary Xml
		private XPathDocument xPathDocumentation;	// XPath version of the xmlDocumentation node (improves performance)

		//this encoding is used for all generated html...
		private static readonly UTF8Encoding encoding = new UTF8Encoding( false );

		//for performance reasons we are going to re-use one XsltArguments collection
		//rather than re-creating an empty one for each transformation
		// IMPORTANT - any method that adds paramater must be sure to remove them
		/// <summary>
		/// Xslt arguments passed to each transformation context
		/// </summary>
		public readonly XsltArgumentList Arguments;

		private MsdnXsltUtilities _utilities;
		/// <summary>
		/// Constructs a new instance of HtmlFactory
		/// </summary>
		/// <param name="tempFileName">NDoc generated temp xml file</param>
		/// <param name="outputDirectory">The directory to write the Html files to</param>
		/// <param name="htmlProvider">Object the provides additional Html content</param>
		/// <param name="config"></param>
		public HtmlFactory( string tempFileName, string outputDirectory, ExternalHtmlProvider htmlProvider, NativeHtmlHelp2Config config )
		{
			Debug.WriteLine("mem before doc load " + GC.GetTotalMemory(true).ToString());
			// Load the XML documentation.
			xmlDocumentation = new XmlDocument();
			Stream tempFile=null;
			try
			{
				tempFile=File.Open(tempFileName,FileMode.Open,FileAccess.Read);

				FilteringXmlTextReader fxtr = new FilteringXmlTextReader(tempFile);
				xmlDocumentation.Load(fxtr);

				tempFile.Seek(0,SeekOrigin.Begin);

				XmlTextReader reader = new XmlTextReader(tempFile);
				xPathDocumentation = new XPathDocument(reader,XmlSpace.Preserve);

			}
			finally
			{
				if (tempFile!=null) tempFile.Close();
				if (File.Exists(tempFileName)) File.Delete(tempFileName);
			}

			//check if there is anything to document
			XmlNodeList typeNodes = xmlDocumentation.SelectNodes("/ndoc/assembly/module/namespace/*[name()!='documentation']");
			if ( typeNodes.Count == 0 )			
				throw new DocumenterException("There are no documentable types in this project.\n\nAny types that exist in the assemblies you are documenting have been excluded by the current visibility settings.\nFor example, you are attempting to document an internal class, but the 'DocumentInternals' visibility setting is set to False.\n\nNote: C# defaults to 'internal' if no accessibilty is specified, which is often the case for Console apps created in VS.NET...");

			if ( !Directory.Exists( outputDirectory ) )
				throw new Exception( string.Format( "The output directory {0}, does not exist", outputDirectory ) );

			_outputDirectory = outputDirectory;
			documentedNamespaces = new ArrayList();
		


			string NSName="";
			string FrameworkVersion="";
			if ( config.SdkDocVersion == SdkVersion.SDK_v1_0 )
			{
				NSName = "ms-help://MS.NETFrameworkSDK";
				FrameworkVersion="1.0";
			}
			else if ( config.SdkDocVersion == SdkVersion.SDK_v1_1 )
			{
				NSName = "ms-help://MS.NETFrameworkSDKv1.1";
				FrameworkVersion="1.1";
			}
			else
				Debug.Assert( false );		// remind ourselves to update this list when new framework versions are supported

			string DocLangCode = Enum.GetName(typeof(SdkLanguage),config.SdkDocLanguage).Replace("_","-");
			if (DocLangCode != "en")
				NSName = NSName + "." + DocLangCode;

			nsMapper = new NamespaceMapper( Path.Combine( Directory.GetParent( _outputDirectory ).ToString(), "NamespaceMap.xml" ) );
			nsMapper.SetSystemNamespace(NSName);

			fileNameMapper = new FileNameMapper(xmlDocumentation);
			_htmlProvider = htmlProvider;
			_utilities = new MsdnXsltUtilities( this.nsMapper, this.fileNameMapper );

			this.Arguments = new XsltArgumentList();
			this.Arguments.AddExtensionObject( "urn:ndoc-sourceforge-net:documenters.NativeHtmlHelp2.xsltUtilities", _utilities );
			this.Arguments.AddExtensionObject( "urn:NDocExternalHtml", _htmlProvider );

			// add properties passed to the stylesheets
			this.Arguments.AddParam( "ndoc-title", "", config.Title );
			this.Arguments.AddParam( "ndoc-document-attributes", "", config.DocumentAttributes );
			this.Arguments.AddParam( "ndoc-net-framework-version", "", FrameworkVersion );
			this.Arguments.AddParam( "ndoc-version", "", config.Version );

			XPathDocument DocumenterSpecificXml = GetDocumenterSpecificXmlData(config);
			XPathNodeIterator it = DocumenterSpecificXml.CreateNavigator().Select("*");
			this.Arguments.AddParam( "documenter-specific-xml", "", it );
			
		}

		#region events
		/// <summary>
		/// Event raised when a topic is started
		/// </summary>
		public event TopicEventHandler TopicStart;

		/// <summary>
		/// Raises the <see cref="TopicStart"/> event
		/// </summary>
		/// <param name="fileName">File name of the topic being started</param>
		protected virtual void OnTopicStart( string fileName )
		{
			if ( TopicStart != null )
				TopicStart( this, new FileEventArgs( fileName ) );
		}
	
		/// <summary>
		/// Event raises when a topic is closed
		/// </summary>
		public event EventHandler TopicEnd;
		/// <summary>
		/// Raises the <see cref="TopicEnd"/> event
		/// </summary>
		protected virtual void OnTopicEnd()
		{
			if ( TopicEnd != null )
				TopicEnd( this, EventArgs.Empty );
		}
	

		/// <summary>
		/// Event raised when a file is being added to a topic
		/// </summary>
		public event TopicEventHandler AddFileToTopic;
		/// <summary>
		/// Raises the <see cref="AddFileToTopic"/> event
		/// </summary>
		/// <param name="fileName">The file being added</param>
		protected virtual void OnAddFileToTopic( string fileName )
		{
			if ( AddFileToTopic != null )
				AddFileToTopic( this, new FileEventArgs( fileName ) );
		}
		#endregion

		/// <summary>
		/// loads and compiles all the stylesheets
		/// </summary>
		public void LoadStylesheets(string extensibiltyStylesheet)
		{
			_stylesheets = StyleSheetCollection.LoadStyleSheets(extensibiltyStylesheet);
		}


		/// <summary>
		/// Sets the custom namespace map to use while constructing XLinks
		/// </summary>
		/// <param name="path">Path to the namespace map. (This file must confrom to NamespaceMap.xsd)</param>
		public void SetNamespaceMap( string path )
		{
			// merge the custom map into the default map
			nsMapper.MergeMaps( new NamespaceMapper( path ) );

			// then save it so the user has some indication of what was actually used
			nsMapper.Save( Path.Combine( Directory.GetParent( _outputDirectory ).ToString(), "NamespaceMap.xml" ) );
		}

		/// <summary>
		/// Generates HTML for the NDoc XML
		/// </summary>
		public void MakeHtml()
		{					
			XmlNodeList assemblyNodes = xmlDocumentation.SelectNodes( "/ndoc/assembly" );
			int[] indexes = SortNodesByAttribute( assemblyNodes, "name" );

			NameValueCollection namespaceAssemblies	= new NameValueCollection();

			for ( int i = 0; i < assemblyNodes.Count; i++ )
			{
				XmlNode assemblyNode = assemblyNodes[indexes[i]];
				if ( assemblyNode.ChildNodes.Count > 0 )
				{
					string assemblyName = assemblyNode.Attributes["name"].Value;
					GetNamespacesFromAssembly( assemblyName, namespaceAssemblies );
				}
			}

			string [] namespaces = namespaceAssemblies.AllKeys;
			Array.Sort( namespaces );

			for ( int i = 0; i < namespaces.Length; i++ )
			{
				string namespaceName = namespaces[i];
				foreach ( string assemblyName in namespaceAssemblies.GetValues( namespaceName ) )
					MakeHtmlForNamespace( assemblyName, namespaceName );
			}		
		}

		private void GetNamespacesFromAssembly( string assemblyName, System.Collections.Specialized.NameValueCollection namespaceAssemblies)
		{
			XmlNodeList namespaceNodes = xmlDocumentation.SelectNodes("/ndoc/assembly[@name=\"" + assemblyName + "\"]/module/namespace");
			foreach ( XmlNode namespaceNode in namespaceNodes )
			{
				string namespaceName = namespaceNode.Attributes["name"].Value;
				namespaceAssemblies.Add( namespaceName, assemblyName );
			}
		}


		private void TransformAndWriteResult( string transformName, string fileName )
		{
			Trace.WriteLine( fileName );
#if DEBUG
			int start = Environment.TickCount;
#endif
			_htmlProvider.SetFilename( fileName );
			_utilities.ResetDescriptions();

			try
			{
				using ( StreamWriter streamWriter = new StreamWriter(
							File.Open( Path.Combine( _outputDirectory, fileName ), FileMode.CreateNew, FileAccess.Write, FileShare.None ), encoding ) )
				{
#if(NET_1_0)
				//Use overload that is obsolete in v1.1
				_stylesheets[transformName].Transform( xPathDocumentation, this.Arguments, streamWriter );
#else
					//Use new overload so we don't get obsolete warnings - clean compile :)
					_stylesheets[transformName].Transform( xPathDocumentation, this.Arguments, streamWriter, null );
#endif
				}
			}				      
			catch(PathTooLongException e)
			{
				throw new PathTooLongException(e.Message + "\nThe file that NDoc was trying to create had the following name:\n" + Path.Combine( _outputDirectory, fileName ));
			}

#if DEBUG
			Debug.WriteLine( ( Environment.TickCount - start ).ToString() + " msec." );
#endif
		}

		private void MakeHtmlForNamespace( string assemblyName, string namespaceName )
		{
			if ( !documentedNamespaces.Contains( namespaceName ) ) 
			{
				documentedNamespaces.Add( namespaceName );
				
				string fileName = fileNameMapper["N:" + namespaceName];

				this.Arguments.AddParam( "namespace", String.Empty, namespaceName );
				TransformAndWriteResult( "namespace", fileName );
				OnTopicStart( fileName );

				TransformAndWriteResult( "namespacehierarchy", FileNameMapper.GetFileNameForNamespaceHierarchy( namespaceName ) );

				MakeHtmlForTypes( namespaceName );
				this.Arguments.RemoveParam( "namespace", string.Empty );

				OnTopicEnd();
			}
		}

		private void MakeHtmlForTypes( string namespaceName )
		{
			XmlNodeList typeNodes =
				xmlDocumentation.SelectNodes("/ndoc/assembly/module/namespace[@name=\"" + namespaceName + "\"]/*[local-name()!='documentation']");

			int[] indexes = SortNodesByAttribute( typeNodes, "id" );

			for ( int i = 0; i < typeNodes.Count; i++ )
			{
				XmlNode typeNode = typeNodes[ indexes[i] ];

				switch( FileNameMapper.GetWhichType( typeNode ) )
				{
					case WhichType.Class:
					case WhichType.Interface:
					case WhichType.Structure:
						MakeHtmlForInterfaceOrClassOrStructure( typeNode );
						break;

					case WhichType.Enumeration:
					case WhichType.Delegate:
						MakeHtmlForEnumerationOrDelegate( typeNode );
						break;

					default:
						break;
				}
			}
		}

		private void MakeHtmlForEnumerationOrDelegate( XmlNode typeNode )
		{
			string typeName = typeNode.Attributes["name"].Value;
			string typeID = typeNode.Attributes["id"].Value;

			string fileName = fileNameMapper[typeID];

			this.Arguments.AddParam( "type-id", String.Empty, typeID );
			TransformAndWriteResult( "type", fileName );
			this.Arguments.RemoveParam( "type-id", String.Empty );

			OnAddFileToTopic( fileName );
		}

		private void MakeHtmlForInterfaceOrClassOrStructure(XmlNode typeNode )
		{
			string typeName = typeNode.Attributes["name"].Value;
			string typeID = typeNode.Attributes["id"].Value;

			string fileName = fileNameMapper[typeID];

			this.Arguments.AddParam( "type-id", String.Empty, typeID );
			TransformAndWriteResult( "type", fileName );
			this.Arguments.RemoveParam( "type-id", String.Empty );

			OnTopicStart( fileName );

			if ( typeNode.SelectNodes( "derivedBy" ).Count > 5 )
			{
				fileName = FileNameMapper.GetFileNameForTypeHierarchy(typeID);
				this.Arguments.AddParam( "type-id", String.Empty, typeID );
				TransformAndWriteResult( "typehierarchy", fileName );
				this.Arguments.RemoveParam( "type-id", String.Empty );
			}

			if ( typeNode.SelectNodes( "constructor|field|property|method|operator|event" ).Count > 0 )
			{
				fileName = FileNameMapper.GetFilenameForOverviewPage(typeID,"Members");

				this.Arguments.AddParam( "id", String.Empty, typeID );
				TransformAndWriteResult( "allmembers", fileName );
				this.Arguments.RemoveParam( "id", String.Empty );

				OnAddFileToTopic( fileName );

				MakeHtmlForConstructors( typeNode );
				MakeHtmlForFields( typeNode );
				MakeHtmlForProperties( typeNode );
				MakeHtmlForMethods( typeNode );
				MakeHtmlForOperators( typeNode );
				MakeHtmlForEvents( typeNode );
			}

			OnTopicEnd();
		}

		private void MakeHtmlForConstructors( XmlNode typeNode )
		{
			string typeName = typeNode.Attributes["name"].Value;
			string typeID = typeNode.Attributes["id"].Value;
			XmlNodeList constructorNodes = typeNode.SelectNodes( "constructor[@contract!='Static']" );

			// If the constructor is overloaded then make an overload page.
			if ( constructorNodes.Count > 1 )
			{
				string constructorID = constructorNodes[0].Attributes["id"].Value;
				string fileName = FileNameMapper.GetFilenameForOverviewPage(typeID,"Constructor");

				this.Arguments.AddParam( "member-id", String.Empty, constructorID );
				TransformAndWriteResult( "memberoverload", fileName );
				this.Arguments.RemoveParam( "member-id", String.Empty );

				OnTopicStart( fileName );
			}

			foreach ( XmlNode constructorNode in constructorNodes )
			{
				string constructorID = constructorNode.Attributes["id"].Value;
				string fileName = fileNameMapper[constructorID];

				this.Arguments.AddParam( "member-id", String.Empty, constructorID );
				TransformAndWriteResult( "member", fileName );
				this.Arguments.RemoveParam( "member-id", String.Empty );

				OnAddFileToTopic( fileName );
			}

			if ( constructorNodes.Count > 1 )
				OnTopicEnd();
			
			XmlNode staticConstructorNode = typeNode.SelectSingleNode("constructor[@contract='Static']");
			if ( staticConstructorNode != null )
			{
				string constructorID = staticConstructorNode.Attributes["id"].Value;
				string fileName = fileNameMapper[constructorID];

				this.Arguments.AddParam( "member-id", String.Empty, constructorID );
				TransformAndWriteResult( "member", fileName );
				this.Arguments.RemoveParam( "member-id", String.Empty );

				OnAddFileToTopic( fileName );
			}
		}

		private void MakeHtmlForFields( XmlNode typeNode )
		{
			XmlNodeList fields = typeNode.SelectNodes("field[not(@declaringType)]");

			if ( fields.Count > 0 )
			{
				string typeName = typeNode.Attributes["name"].Value;
				string typeID = typeNode.Attributes["id"].Value;
				string fileName = FileNameMapper.GetFilenameForOverviewPage( typeID,"Fields" );

				this.Arguments.AddParam( "id", String.Empty, typeID );
				this.Arguments.AddParam( "member-type", String.Empty, "field" );
				TransformAndWriteResult( "individualmembers",  fileName );
				this.Arguments.RemoveParam( "id", String.Empty );
				this.Arguments.RemoveParam( "member-type", String.Empty );

				OnTopicStart( fileName );

				int[] indexes = SortNodesByAttribute(fields, "id");

				foreach ( int index in indexes )
				{
					XmlNode field = fields[index];

					string fieldName = field.Attributes["name"].Value;
					string fieldID = field.Attributes["id"].Value;

					fileName = fileNameMapper[fieldID];

					this.Arguments.AddParam( "field-id", String.Empty, fieldID );
					TransformAndWriteResult( "field", fileName );
					this.Arguments.RemoveParam( "field-id", String.Empty );

					OnAddFileToTopic( fileName );
				}

				OnTopicEnd();
			}
		}

		private void MakeHtmlForProperties( XmlNode typeNode )
		{
			XmlNodeList declaredPropertyNodes = typeNode.SelectNodes("property[not(@declaringType)]");

			if ( declaredPropertyNodes.Count > 0 )
			{
				string previousPropertyName;
				string nextPropertyName;

				string typeName = typeNode.Attributes["name"].Value;
				string typeID = typeNode.Attributes["id"].Value;
				XmlNodeList propertyNodes = typeNode.SelectNodes( "property[not(@declaringType)]" );

				int[] indexes = SortNodesByAttribute( propertyNodes, "id" );

				string fileName = FileNameMapper.GetFilenameForOverviewPage( typeID, "Properties" );

				this.Arguments.AddParam( "id", String.Empty, typeID );
				this.Arguments.AddParam( "member-type", String.Empty, "property" );
				TransformAndWriteResult( "individualmembers", fileName );
				this.Arguments.RemoveParam( "id", String.Empty );
				this.Arguments.RemoveParam( "member-type", String.Empty );

				OnTopicStart( fileName );

				for ( int i = 0; i < propertyNodes.Count; i++ )
				{
					XmlNode propertyNode = propertyNodes[indexes[i]];

					string propertyName = propertyNode.Attributes["name"].Value;
					string propertyID = propertyNode.Attributes["id"].Value;

					previousPropertyName = ( (i - 1 < 0 ) || ( propertyNodes[indexes[i - 1]].Attributes.Count == 0 ) )
						? "" : propertyNodes[indexes[i - 1]].Attributes[0].Value;
					nextPropertyName = ( ( i + 1 == propertyNodes.Count ) || ( propertyNodes[indexes[i + 1]].Attributes.Count == 0 ) )
						? "" : propertyNodes[indexes[i + 1]].Attributes[0].Value;

					if ( ( previousPropertyName != propertyName ) && ( nextPropertyName == propertyName ) )
					{
						fileName = FileNameMapper.GetFilenameForPropertyOverloads( typeID, propertyName );

						this.Arguments.AddParam( "member-id", String.Empty, propertyID );
						TransformAndWriteResult( "memberoverload", fileName );
						this.Arguments.RemoveParam( "member-id", String.Empty );

						OnTopicStart( fileName );
					}

					fileName = fileNameMapper[propertyID];

					this.Arguments.AddParam( "property-id", String.Empty, propertyID );
					TransformAndWriteResult( "property", fileName );
					this.Arguments.RemoveParam( "property-id", String.Empty );

					OnAddFileToTopic( fileName );

					if ( ( previousPropertyName == propertyName ) && ( nextPropertyName != propertyName ) )
						OnTopicEnd();
				}

				OnTopicEnd();
			}
		}
		private void MakeHtmlForMethods( XmlNode typeNode )
		{
			XmlNodeList declaredMethodNodes = typeNode.SelectNodes( "method[not(@declaringType)]" );

			if ( declaredMethodNodes.Count > 0 )
			{
				bool bOverloaded = false;

				string typeName = typeNode.Attributes["name"].Value;
				string typeID = typeNode.Attributes["id"].Value;
				XmlNodeList methodNodes = typeNode.SelectNodes( "method" );

				int[] indexes = SortNodesByAttribute( methodNodes, "id" );

				string fileName = FileNameMapper.GetFilenameForOverviewPage( typeID, "Methods" );

				this.Arguments.AddParam( "id", String.Empty, typeID );
				this.Arguments.AddParam( "member-type", String.Empty, "method" );
				TransformAndWriteResult( "individualmembers", fileName );
				this.Arguments.RemoveParam( "id", String.Empty );
				this.Arguments.RemoveParam( "member-type", String.Empty );

				OnTopicStart( fileName );

				for (int i = 0; i < methodNodes.Count; i++)
				{
					XmlNode methodNode = methodNodes[indexes[i]];
					string methodName = methodNode.Attributes["name"].Value;
					string methodID = methodNode.Attributes["id"].Value;

					if ( MethodHelper.IsMethodFirstOverload( methodNodes, indexes, i ) )
					{
						bOverloaded = true;

						fileName = FileNameMapper.GetFilenameForMethodOverloads( typeID, methodName );

						this.Arguments.AddParam( "member-id", String.Empty, methodID );
						TransformAndWriteResult( "memberoverload", fileName );
						this.Arguments.RemoveParam( "member-id", String.Empty );

						OnTopicStart( fileName );
					}

					if ( methodNode.Attributes["declaringType"] == null )
					{
						fileName = fileNameMapper[methodID];

						this.Arguments.AddParam( "member-id", String.Empty, methodID );
						TransformAndWriteResult( "member", fileName );
						this.Arguments.RemoveParam( "member-id", String.Empty );

						OnAddFileToTopic( fileName );
					}

					if ( bOverloaded && MethodHelper.IsMethodLastOverload( methodNodes, indexes, i ) )
					{
						bOverloaded = false;
						OnTopicEnd();
					}
				}

				OnTopicEnd();
			}
		}

		private void MakeHtmlForEvents( XmlNode typeNode )
		{
			XmlNodeList declaredEventNodes = typeNode.SelectNodes( "event[not(@declaringType)]" );

			if ( declaredEventNodes.Count > 0 )
			{
				XmlNodeList events = typeNode.SelectNodes( "event" );

				if ( events.Count > 0 )
				{
					string typeName = typeNode.Attributes["name"].Value;
					string typeID = typeNode.Attributes["id"].Value;

					string fileName = FileNameMapper.GetFilenameForOverviewPage( typeID, "Events" );

					this.Arguments.AddParam( "id", String.Empty, typeID );
					this.Arguments.AddParam( "member-type", String.Empty, "event" );
					TransformAndWriteResult( "individualmembers",  fileName );
					this.Arguments.RemoveParam( "id", String.Empty );
					this.Arguments.RemoveParam( "member-type", String.Empty );

					OnTopicStart( fileName );

					int[] indexes = SortNodesByAttribute( events, "id" );

					foreach ( int index in indexes )
					{
						XmlNode eventElement = events[index];

						if ( eventElement.Attributes["declaringType"] == null )
						{
							string eventName = eventElement.Attributes["name"].Value;
							string eventID = eventElement.Attributes["id"].Value;

							fileName = fileNameMapper[eventID];

							this.Arguments.AddParam( "event-id", String.Empty, eventID );
							TransformAndWriteResult( "event", fileName );
							this.Arguments.RemoveParam( "event-id", String.Empty );

							OnAddFileToTopic( fileName );
						}
					}

					OnTopicEnd();
				}
			}
		}

		private void MakeHtmlForOperators( XmlNode typeNode )
		{
			XmlNodeList operators = typeNode.SelectNodes( "operator" );

			if ( operators.Count > 0 )
			{
				string typeName = typeNode.Attributes["name"].Value;
				string typeID = typeNode.Attributes["id"].Value;
				XmlNodeList opNodes = typeNode.SelectNodes( "operator" );
				bool bOverloaded = false;

				string fileName = FileNameMapper.GetFilenameForOverviewPage( typeID, "Operators" );
				
				this.Arguments.AddParam( "id", String.Empty, typeID );
				this.Arguments.AddParam( "member-type", String.Empty, "operator" );
				TransformAndWriteResult( "individualmembers", fileName );
				this.Arguments.RemoveParam( "id", String.Empty );
				this.Arguments.RemoveParam( "member-type", String.Empty );

				OnTopicStart( fileName );

				int[] indexes = SortNodesByAttribute( operators, "id" );

				//operators
				for ( int i = 0; i < opNodes.Count; i++ )
				{
					XmlNode operatorNode = operators[indexes[i]];
					string operatorID = operatorNode.Attributes["id"].Value;
					string opName = operatorNode.Attributes["name"].Value;

					if ( ( opName != "op_Implicit" ) && ( opName != "op_Explicit" ) )
					{
						if ( MethodHelper.IsMethodFirstOverload( opNodes, indexes, i ) )
						{
							bOverloaded = true;

							fileName = FileNameMapper.GetFilenameForOperatorsOverloads( typeID, opName );

							this.Arguments.AddParam( "member-id", String.Empty, operatorID );
							TransformAndWriteResult( "memberoverload", fileName );
							this.Arguments.RemoveParam( "member-id", String.Empty );
							
							OnTopicStart( fileName );
						}

						if ( operatorNode.Attributes["declaringType"] == null )
						{
							fileName = fileNameMapper[operatorID];

							this.Arguments.AddParam( "member-id", String.Empty, operatorID );
							TransformAndWriteResult( "member", fileName);
							this.Arguments.RemoveParam( "member-id", String.Empty );

							OnAddFileToTopic( fileName );
						}

						if ( bOverloaded && MethodHelper.IsMethodLastOverload( opNodes, indexes, i ) )
						{
							bOverloaded = false;
							OnTopicEnd();
						}
					}
				}

				//type converters
				for ( int i = 0; i < opNodes.Count; i++ )
				{
					XmlNode operatorNode = operators[indexes[i]];
					string operatorID = operatorNode.Attributes["id"].Value;
					string opName = operatorNode.Attributes["name"].Value;

					if ( ( opName == "op_Implicit" ) || ( opName == "op_Explicit" ) )
					{
						if ( operatorNode.Attributes["declaringType"] == null )
						{
							fileName = fileNameMapper[operatorID];

							this.Arguments.AddParam( "member-id", String.Empty, operatorID );
							TransformAndWriteResult( "member", fileName );
							this.Arguments.RemoveParam( "member-id", String.Empty );

							OnAddFileToTopic( fileName );
						}
					}
				}

				OnTopicEnd();
			}
		}

		private static int[] SortNodesByAttribute( XmlNodeList nodes, string attributeName )
		{
			string[] names = new string[nodes.Count];
			int[] indexes = new int[nodes.Count];
			int i = 0;

			foreach ( XmlNode node in nodes )
			{
				names[i] = node.Attributes[attributeName].Value;
				indexes[i] = i++;
			}

			Array.Sort( names, indexes );

			return indexes;
		}

		private XPathDocument GetDocumenterSpecificXmlData(NativeHtmlHelp2Config config)   
		{
			// create a StringWriter to hold the constructed xml
			StringWriter sw = new StringWriter();
			XmlTextWriter writer = new XmlTextWriter(sw);
 
			writer.WriteStartElement("root");

			// add the default docset for this help title   
			AddDocSet( writer, config.HtmlHelpName );   
    
			// also add the items from the custom docset list   
			foreach ( string s in config.DocSetList.Split( new char [] { ',' } ) )   
				AddDocSet( writer, s );
   
			writer.WriteEndElement();   

			// now we load the constructed xml into an XPathDoc
			XmlTextReader reader = new XmlTextReader(sw.ToString(), XmlNodeType.Document, null);
			XPathDocument xpd = new XPathDocument(reader);

			return xpd;
		}   
    
		private void AddDocSet(XmlWriter writer, string id )   
		{   
			if ( id.Length > 0 )   
			{   
				writer.WriteStartElement( "docSet" );   
				writer.WriteString(id.Trim());   
				writer.WriteEndElement();   
			}   
		}
 
		/// <summary>
		/// This custom reader is used to load the XmlDocument. It removes elements that are not required *before* 
		/// they are loaded into memory, and hence lowers memory requirements significantly.
		/// </summary>
		private class FilteringXmlTextReader:XmlTextReader
		{
			object oNamespaceHierarchy;
			object oDocumentation;
			object oImplements;
			object oAttribute;

			public FilteringXmlTextReader(System.IO.Stream file):base(file)
			{
				base.WhitespaceHandling=WhitespaceHandling.None;
				oNamespaceHierarchy = base.NameTable.Add("namespaceHierarchy");
				oDocumentation = base.NameTable.Add("documentation");
				oImplements = base.NameTable.Add("implements");
				oAttribute = base.NameTable.Add("attribute");
			}
		
			private bool ShouldSkipElement()
			{
				return
					(
					base.Name.Equals(oNamespaceHierarchy)||
					base.Name.Equals(oDocumentation)||
					base.Name.Equals(oImplements)||
					base.Name.Equals(oAttribute)
					);
			}

			public override bool Read()
			{
				bool notEndOfDoc=base.Read();
				if (!notEndOfDoc) return false;
				while (notEndOfDoc && (base.NodeType == XmlNodeType.Element) && ShouldSkipElement() )
				{
					notEndOfDoc=SkipElement(this.Depth);
				}
				return notEndOfDoc;
			}

			private bool SkipElement(int startDepth)
			{
				if (base.IsEmptyElement) return base.Read();
				bool notEndOfDoc=true;
				while (notEndOfDoc)
				{
					notEndOfDoc=base.Read();
					if ((base.NodeType == XmlNodeType.EndElement) && (this.Depth==startDepth) ) 
						break;
				}
				if (notEndOfDoc) notEndOfDoc=base.Read();
				return notEndOfDoc;
			}

		}
	}

}
