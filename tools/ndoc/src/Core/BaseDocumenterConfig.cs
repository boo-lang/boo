// BaseDocumenterConfig.cs - base XML documenter config class
// Copyright (C) 2001 Kral Ferch, Jason Diamond
// Parts Copyright (C) 2004  Kevin Downs
//
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA 02111-1307 USA

using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing.Design;
using System.Reflection;
using System.Windows.Forms.Design;
using System.Diagnostics;
using System.Xml;

namespace NDoc.Core
{

	/// <summary>Provides an abstract base class for documenter configurations.</summary>
	/// <remarks>
	/// This is a base class for NDoc Documenter Configs.  
	/// It implements all the methods required by the <see cref="IDocumenterConfig"/> interface. 
	/// It also provides some basic properties which are shared by all configs. 
	/// </remarks>
	abstract public class BaseDocumenterConfig : IDocumenterConfig
	{
		private IDocumenterInfo _info;

		/// <summary>Initializes a new instance of the <see cref="BaseDocumenterConfig"/> class.</summary>
		protected BaseDocumenterConfig( IDocumenterInfo info )
		{
			Debug.Assert( info != null );
			_info = info;
		}

		private Project _Project;
		/// <summary>
		/// Gets the <see cref="Project"/> that this config is associated with, if any
		/// </summary>
		/// <value>The <see cref="Project"/> that this config is associated with, or a <see langword="null"/> if it is not associated with a project.</value>
		protected Project Project
		{
			get{return _Project;}
		}

		/// <summary>
		/// Creates an instance of a documenter <see cref="IDocumenterConfig.CreateDocumenter"/>
		/// </summary>
		/// <returns>IDocumenter instance</returns>		
		public abstract IDocumenter CreateDocumenter();

		/// <summary>Associates this config with a <see cref="Project"/>.</summary>
		/// <param name="project">A <see cref="Project"/> to associate with this config.</param>
		public void SetProject(Project project)
		{
			_Project = project;
		}

		/// <summary>Sets the <see cref="NDoc.Core.Project.IsDirty"/> property on the <see cref="Project"/>.</summary>
		protected void SetDirty()
		{
			if (_Project != null)
			{
				_Project.IsDirty = true;
			}
		}

		/// <summary>
		/// Gets the display name of the documenter.
		/// </summary>
		[Browsable(false)]
		public IDocumenterInfo DocumenterInfo
		{
			get { return _info;}
		}

		/// <summary>Gets an enumerable list of <see cref="PropertyInfo"/> objects representing the properties of this config.</summary>
		/// <remarks>properties are represented by <see cref="PropertyInfo"/> objects.</remarks>
		public IEnumerable GetProperties()
		{
			ArrayList properties = new ArrayList();

			foreach (PropertyInfo property in GetType().GetProperties())
			{
				object[] attr = property.GetCustomAttributes(typeof(BrowsableAttribute),true);
				if (attr.Length>0)
				{
					if( ((BrowsableAttribute)attr[0]).Browsable )
					{
						properties.Add(property);
					}
				}
				else
				{
					properties.Add(property);
				}
			}

			return properties;
		}

		/// <summary>
		/// Sets the value of a named config property.
		/// </summary>
		/// <param name="name">The name of the property to set.</param>
		/// <param name="value">A string representation of the desired property value.</param>
		/// <remarks>Property name matching is case-insensitive.</remarks>
		public void SetValue(string name, string value)
		{
			name = name.ToLower();

			foreach (PropertyInfo property in GetType().GetProperties())
			{
				if (name == property.Name.ToLower())
				{
					string result = ReadProperty(property.Name, value);
					if (result.Length>0)
					{
						System.Diagnostics.Trace.WriteLine(result);
					}
				}
			}
		}

		/// <summary>Writes the current state of the config to the specified <see cref="XmlWriter"/>.</summary>
		/// <param name="writer">An open <see cref="XmlWriter"/>.</param>
		/// <remarks>
		/// This method uses reflection to serialize the public properties in the config.
		/// <para>
		/// A property will <b>not</b> be persisted if,
		/// <list type="bullet">
		/// <item>The value is equal to the default value, or</item>
		/// <item>The string representation of the value is an empty string, or</item>
		/// <item>The property has a Browsable(false) attribute, or</item>
		/// <item>The property has a NonPersisted attribute.</item>
		/// </list>
		/// </para>
		/// </remarks>
		public void Write(XmlWriter writer)
		{
			writer.WriteStartElement("documenter");
			writer.WriteAttributeString("name", this.DocumenterInfo.Name );

			PropertyInfo[] properties = GetType().GetProperties();

			foreach (PropertyInfo property in properties)
			{
				if (!property.IsDefined(typeof(NonPersistedAttribute),true))
				{
					object value = property.GetValue(this, null);

					if (value != null)
					{
						bool writeProperty = true;
						string value2 = Convert.ToString(value);

						if (value2 != null)
						{
							//see if the property has a default value
							object[] defaultValues=property.GetCustomAttributes(typeof(DefaultValueAttribute),true);
							if (defaultValues.Length > 0)
							{
								if(Convert.ToString(((DefaultValueAttribute)defaultValues[0]).Value)==value2)
									writeProperty=false;
							}
							else
							{
								if(value2=="")
									writeProperty=false;
							}
						}
						else
						{
							writeProperty=false;
						}

						//being lazy and assuming only one BrowsableAttribute...
						BrowsableAttribute[] browsableAttributes=(BrowsableAttribute[])property.GetCustomAttributes(typeof(BrowsableAttribute),true);
						if (browsableAttributes.Length>0 && !browsableAttributes[0].Browsable)
						{
							writeProperty=false;
						}

						if (writeProperty)
						{
							writer.WriteStartElement("property");
							writer.WriteAttributeString("name", property.Name);
							writer.WriteAttributeString("value", value2);
							writer.WriteEndElement();
						}
					}
				}			
			}

			writer.WriteEndElement();
		}

		/// <summary>Loads config details from the specified <see cref="XmlReader"/>.</summary>
		/// <param name="reader">An <see cref="XmlReader"/> positioned on a &lt;documenter&gt; element.</param>
		/// <remarks>Each property found in the XML is loaded into current config using <see cref="ReadProperty"/>.</remarks>
		public void Read(XmlReader reader)
		{
			// we don't want to set the project IsDirty flag during the read...
			_Project.SuspendDirtyCheck=true;

			string FailureMessages="";

			while(!reader.EOF && !(reader.NodeType == XmlNodeType.EndElement && reader.Name == "documenter"))
			{
				if (reader.NodeType == XmlNodeType.Element && reader.Name == "property")
				{
					FailureMessages += ReadProperty(reader["name"], reader["value"]);
				}
				reader.Read(); // Advance.
			}

			// Restore the project IsDirty checking.
			_Project.SuspendDirtyCheck=false;
			if (FailureMessages.Length > 0)
				throw new DocumenterPropertyFormatException(FailureMessages);
		}

		/// <summary>
		/// Sets the value of a named property. 
		/// </summary>
		/// <param name="name">A property name.</param>
		/// <param name="value">A string respesentation of the desired property value.</param>
		/// <returns>A string containing any messages generated while attempting to set the property.</returns>
		protected string ReadProperty(string name, string value)
		{
			// if value is an empty string, do not bother with anything else
			if (value==null) return String.Empty;
			if (value.Length==0) return String.Empty;

			string FailureMessages=String.Empty;
			PropertyInfo property = GetType().GetProperty(name);

			if (property == null)
			{
				FailureMessages += HandleUnknownPropertyType(name, value);
			}
			else
			{
				bool ValueParsedOK = false;
				object value2 = null;
						
				// if the string in the project file is not a valid member
				// of the enum, or cannot be parsed into the property type
				// for some reason,we don't want to throw an exception and
				// ditch all the settings stored later in the file!
				// save the exception details, and  we will throw a 
				// single exception at the end..
				try
				{
					if (property.PropertyType.IsEnum)
					{
						//parse is now case-insensitive...
						value2 = Enum.Parse(property.PropertyType, value, true);
						ValueParsedOK = true;
					}
					else
					{
						TypeConverter tc = TypeDescriptor.GetConverter(property.PropertyType);
						value2 = tc.ConvertFromString(value);
						ValueParsedOK = true;
					}
				}
				catch(System.ArgumentException)
				{
					Project.SuspendDirtyCheck=false;
					FailureMessages += HandleUnknownPropertyValue(property, value);
					Project.SuspendDirtyCheck=true;
				}
				catch(System.FormatException)
				{
					Project.SuspendDirtyCheck=false;
					FailureMessages += HandleUnknownPropertyValue(property, value);
					Project.SuspendDirtyCheck=true;
				}
				// any other exception will be thrown immediately

				if (property.CanWrite && ValueParsedOK)
				{
					property.SetValue(this, value2, null);
				}
			}
			return FailureMessages;
		}

		/// <summary>
		/// When overridden in a derived class, handles a property found by <see cref="Read"/> which does not 
		/// correspond to any property in the config object.
		/// </summary>
		/// <param name="name">The unknown property name.</param>
		/// <param name="value">A string representation of the desired property value.</param>
		/// <returns>A string containing any messages generated by the handler.</returns>
		/// <remarks>
		/// As implemented in this class, no action is taken.
		/// <note type="inheritinfo">
		/// <para>If a handler can translate the unknown property, it can call the protected method 
		/// <see cref="ReadProperty"/> to process to translated name/value.</para>
		/// </note>
		/// </remarks>
		protected virtual string HandleUnknownPropertyType(string name, string value)
		{
			// As a default, we will ignore unknown property types
			return "";
		}

		/// <summary>
		/// When overridden in a derived class, handles a unknown or invalid property value read by <see cref="Read"/>.
		/// </summary>
		/// <param name="property">A valid Property name.</param>
		/// <param name="value">A string representation of the desired property value.</param>
		/// <returns>A string containing any messages generated by the handler.</returns>
		/// <remarks>
		/// As implemented in this class, an error message is returned which details the 
		/// property name, type and the invalid value.
		/// <note type="inheritinfo">
		/// <para>If a handler can translate the unknown value, it can call the protected method <see cref="ReadProperty"/> to
		/// process to translated name/value.</para>
		/// </note>
		/// </remarks>
		protected virtual string HandleUnknownPropertyValue(PropertyInfo property, string value)
		{
			// we cannot handle this, so return an error message
			return String.Format("     Property '{0}' has an invalid value for type {1} ('{2}') \n", property.Name, property.PropertyType.ToString() ,value);
		}


		#region Documentation Main Settings 

		private bool _CleanIntermediates = false;

		/// <summary>Gets or sets a value indicating whether to delete intermediate files after a successful build.</summary>
		/// <remarks>
		/// <value>
		/// <see langword="true"/> if intermediate files should be deleted after a successful build;
		/// otherwise, <see langword="false"/>. By default, the value of this property is <see langword="false"/>.</value>
		/// <para>For documenters that result in a compiled output, like the MSDN and VS.NET
		/// documenters, intermediate files include all of the HTML Help project files, as well as the generated
		/// HTML files.</para></remarks>
		[Category("Documentation Main Settings")]
		[Description("When true, intermediate files will be deleted after a successful build.")]
		[DefaultValue(false)]
		public bool CleanIntermediates
		{
			get { return _CleanIntermediates; }
			set
			{
				_CleanIntermediates = value;
				SetDirty();
			}
		}
		

		#endregion
		
	}

	/// <summary>
	/// 
	/// </summary>
	[AttributeUsage(AttributeTargets.Property)]
	public class NonPersistedAttribute : Attribute
	{
	}
}
