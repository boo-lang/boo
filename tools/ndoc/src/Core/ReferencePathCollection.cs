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
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Text;
using System.Reflection;
using System.Collections;
using System.Drawing.Design;
using System.Runtime.InteropServices;
using System.Xml;
using System.Diagnostics;
using System.Windows.Forms;

namespace NDoc.Core
{
	/// <summary>
	/// Type safe collection class for <see cref="ReferencePath"/> objects. 
	/// </summary>
	/// <remarks>
	/// <para>Extends the base class <see cref="CollectionBase"/> to inherit base collection functionality.</para>
	/// <para>Implementation of <see cref="ICustomTypeDescriptor"/> to provide customized type description.</para>
	/// </remarks>
	[Serializable]
	[TypeConverter(typeof(ReferencePathCollection.ReferencePathCollectionTypeConverter))]
	[Editor(typeof(ReferencePathCollection.ReferencePathCollectionEditor), typeof(UITypeEditor))]
	public class ReferencePathCollection : CollectionBase, ICustomTypeDescriptor
	{
		#region collection methods
		
		/// <summary>
		/// Adds the specified <see cref="ReferencePath"/> object to the collection.
		/// </summary>
		/// <param name="refPath">The <see cref="ReferencePath"/> to add to the collection.</param>
		/// <exception cref="ArgumentNullException"><paramref name="refPath"/> is a <see langword="null"/>.</exception>
		/// <remarks>
		/// If the path in <paramref name="refPath"/> matches one already existing in the collection, the
		/// operation is silently ignored.
		/// </remarks>
		public void Add(ReferencePath refPath)
		{
			if (refPath == null)
				throw new ArgumentNullException("refPath");

			if (!base.InnerList.Contains(refPath))
				this.List.Add(refPath);
		}
		
		/// <summary>
		/// Adds the elements of an <see cref="ICollection"/> to the end of the collection.
		/// </summary>
		/// <param name="c">The <see cref="ICollection"/> whose elements should be added to the end of the collection. 
		/// The collection itself cannot be a <see langword="null"/>.</param>
		/// <exception cref="ArgumentNullException"><paramref name="c"/> is a <see langword="null"/>.</exception>
		public virtual void AddRange(ICollection c)
		{
			base.InnerList.AddRange(c);

		}

		/// <summary>
		/// Removes the first occurence of a specific <see cref="ReferencePath"/> from the collection.
		/// </summary>
		/// <param name="refPath">The <see cref="ReferencePath"/> to remove from the collection.</param>
		/// <exception cref="ArgumentNullException"><paramref name="refPath"/> is a <see langword="null"/>.</exception>
		/// <remarks>
		/// Elements that follow the removed element move up to occupy the vacated spot and the indexes of the elements that are moved are also updated.
		/// </remarks>
		public void Remove(ReferencePath refPath)
		{
			if (refPath == null)
				throw new ArgumentNullException("refPath");

			this.List.Remove(refPath);
		}
		
		/// <summary>
		/// Gets or sets the <see cref="ReferencePath"/> at the specified index.
		/// </summary>
		/// <param name="index">The zero-based index of the <see cref="ReferencePath"/> to get or set.</param>
		/// <value>The <see cref="ReferencePath"/> at the specified index</value>
		/// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> is not a valid index 
		/// in the collection.</exception>
		/// <exception cref="ArgumentNullException">set <i>value</i> is a <see langword="null"/>.</exception>
		public ReferencePath this[int index] 
		{
			get
			{
				return this.List[index] as ReferencePath;
			}
			set
			{
				if (value == null)
					throw new ArgumentNullException("Set Value");

				this.List[index] = value;
			}
		}

		/// <summary>
		/// Determines whether the collection contains a specified path.
		/// </summary>
		/// <param name="path">The path to locate in the collection.</param>
		/// <returns><see langword="true"/> if the collection contains the specified path, 
		/// otherwise <see langword="false"/>.</returns>
		/// <exception cref="ArgumentNullException"><paramref name="path"/> is a <see langword="null"/>.</exception>
		/// <remarks>Path comparison is case-insensitive.</remarks>
		public bool Contains(string path)
		{
			if (path == null)
				throw new ArgumentNullException("path");

			bool result = false;
			foreach (object obj in base.InnerList)
			{
				ReferencePath refPath = obj as ReferencePath;
				if (String.Compare(refPath.Path, path, true) == 0)
				{
					result = true;
					break;
				}
			}
			return result;
		}
		#endregion

		#region Persistance

		/// <summary>
		/// Saves reference paths to an XmlWriter.
		/// </summary>
		/// <param name="writer">An open XmlWriter.</param>
		public void WriteXml(XmlWriter writer)
		{
			if (this.Count > 0)
			{
				writer.WriteStartElement("referencePaths");

				foreach (ReferencePath refPath in this)
				{
					writer.WriteStartElement("referencePath");
					writer.WriteAttributeString("path", refPath.ToString());
					writer.WriteEndElement();
				}

				writer.WriteEndElement();
			}
		}

		/// <summary>
		/// Loads reference paths from an XMLReader.
		/// </summary>
		/// <param name="reader">
		/// An open XmlReader positioned before or on the referencePaths element.</param>
		public void ReadXml(XmlReader reader)
		{
			while (!reader.EOF && !(reader.NodeType == XmlNodeType.EndElement && reader.Name == "referencePaths"))
			{
				if (reader.NodeType == XmlNodeType.Element && reader.Name == "referencePath")
				{
					string path = reader["path"];
					if (!Contains(path))
					{
						Add(new ReferencePath(path));
					}
				}
				reader.Read();
			}
		}

		#endregion

		#region ICustomTypeDescriptor impl 
		// Implementation of interface ICustomTypeDescriptor 

		String ICustomTypeDescriptor.GetClassName()
		{
			return TypeDescriptor.GetClassName(this, true);
		}

		AttributeCollection ICustomTypeDescriptor.GetAttributes()
		{
			return TypeDescriptor.GetAttributes(this, true);
		}

		String ICustomTypeDescriptor.GetComponentName()
		{
			return TypeDescriptor.GetComponentName(this, true);
		}

		TypeConverter ICustomTypeDescriptor.GetConverter()
		{
			return TypeDescriptor.GetConverter(this, true);
		}

		EventDescriptor ICustomTypeDescriptor.GetDefaultEvent() 
		{
			return TypeDescriptor.GetDefaultEvent(this, true);
		}

		PropertyDescriptor ICustomTypeDescriptor.GetDefaultProperty() 
		{
			return TypeDescriptor.GetDefaultProperty(this, true);
		}

		object ICustomTypeDescriptor.GetEditor(Type editorBaseType) 
		{
			return TypeDescriptor.GetEditor(this, editorBaseType, true);
		}

		EventDescriptorCollection ICustomTypeDescriptor.GetEvents(Attribute[] attributes) 
		{
			return TypeDescriptor.GetEvents(this, attributes, true);
		}

		EventDescriptorCollection ICustomTypeDescriptor.GetEvents()
		{
			return TypeDescriptor.GetEvents(this, true);
		}

		object ICustomTypeDescriptor.GetPropertyOwner(PropertyDescriptor pd) 
		{
			return this;
		}


		/// <summary>
		/// Called to get the properties of this type. Returns properties with certain
		/// attributes. this restriction is not implemented here.
		/// </summary>
		/// <param name="attributes"></param>
		/// <returns></returns>
		PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties(Attribute[] attributes)
		{
			return GetProperties();
		}

		PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties()
		{
			return GetProperties();
		}
		
		private PropertyDescriptorCollection GetProperties()
		{
			// Create a collection object to hold property descriptors
			PropertyDescriptorCollection pds = new PropertyDescriptorCollection(null);

			// Iterate the list of Paths
			for (int i = 0; i < this.List.Count; i++)
			{
				// Create a property descriptor for the ReferencePath item and add to the property descriptor collection
				ReferencePathCollectionPropertyDescriptor pd = new ReferencePathCollectionPropertyDescriptor(this, i);
				pds.Add(pd);
			}
			// return the property descriptor collection
			return pds;
		}

		#endregion

		#region PropertyDescriptor 
		/// <summary>
		/// Summary description for CollectionPropertyDescriptor.
		/// </summary>
		internal class ReferencePathCollectionPropertyDescriptor : PropertyDescriptor
		{
			private ReferencePathCollection collection = null;
			private int index = -1;

			/// <inheritDoc/>
			public ReferencePathCollectionPropertyDescriptor(ReferencePathCollection coll, int idx) : 
				base("Path #" + (idx + 1).ToString(), null)
			{
				this.collection = coll;
				this.index = idx;
			} 

			/// <inheritDoc/>
			public override AttributeCollection Attributes
			{
				get 
				{ 
					return new AttributeCollection(null);
				}
			}

			/// <inheritDoc/>
			public override bool CanResetValue(object component)
			{
				return false;
			}

			/// <inheritDoc/>
			public override Type ComponentType
			{
				get 
				{ 
					return this.collection.GetType();
				}
			}

			/// <inheritDoc/>
			public override string DisplayName
			{
				get 
				{
					return "Path #" + (index + 1).ToString();
				}
			}

			/// <inheritDoc/>
			public override string Description
			{
				get
				{
					ReferencePath rp = this.collection[index];
					return rp.ToString();
				}
			}

			/// <inheritDoc/>
			public override object GetValue(object component)
			{
				return this.collection[index];
			}

			/// <inheritDoc/>
			public override bool IsReadOnly
			{
				get { return false; }
			}

			/// <inheritDoc/>
			public override string Name
			{
				get { return "Path #" + (index + 1).ToString(); }
			}

			/// <inheritDoc/>
			public override Type PropertyType
			{
				get { return this.collection[index].GetType(); }
			}

			/// <inheritDoc/>
			public override void ResetValue(object component)
			{
			}

			/// <inheritDoc/>
			public override bool ShouldSerializeValue(object component)
			{
				return true;
			}

			/// <inheritDoc/>
			public override void SetValue(object component, object value)
			{
				Debug.WriteLine("RefPathColl:PropDesc.SetValue  value->" + value.GetType().ToString());
				
				if (value is ReferencePath)
				{
					this.collection[index] = (ReferencePath)value;
				}
				else
				{
					if ((string) value != this.collection[index].Path)
					{
						this.collection[index].Path = (string)value;
					}
				}
			}
		
			/// <inheritDoc/>
			public override TypeConverter Converter
			{
				get
				{
					return new ReferencePath.TypeConverter();
				}
			}
		}
		#endregion

		// This is a special type converter which will be associated with the ReferencePathCollection class.
		// It converts an ReferencePathCollection object to a string representation for use in a property grid.
		internal class ReferencePathCollectionTypeConverter : ExpandableObjectConverter
		{
			public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destType)
			{
				if (destType == typeof(string) && value is ReferencePathCollection)
				{
					ReferencePathCollection rpc = (ReferencePathCollection)value;
					if (rpc.Count == 0)
						return "(None)";
					else
						return "(Count=" + ((ReferencePathCollection)value).Count + ")";
				}
				return base.ConvertTo(context, culture, value, destType);
			}
	
		}

		/// <summary>
		/// 
		/// </summary>
		internal class ReferencePathCollectionEditor : UITypeEditor //System.ComponentModel.Design.CollectionEditor
		{
			/// <summary>
			/// Creates a new <see cref="ReferencePathCollectionEditor"/> instance.
			/// </summary>
			public ReferencePathCollectionEditor() : base()
			{
			}

			/// <summary>
			/// Edits the value.
			/// </summary>
			/// <param name="context">Context.</param>
			/// <param name="provider">Provider.</param>
			/// <param name="value">Value.</param>
			/// <returns></returns>
			[RefreshProperties(RefreshProperties.All)] 
			public override object EditValue(System.ComponentModel.ITypeDescriptorContext context, IServiceProvider provider, object value)
			{
				if (context != null && context.Instance != null)
				{
					GridItem gridItem = context as GridItem;

					ReferencePathCollectionEditorForm form = new ReferencePathCollectionEditorForm();
					form.ReferencePaths = (ReferencePathCollection)value;
					DialogResult result = form.ShowDialog();

					object returnObject = null;
					if (result == DialogResult.OK)
					{
						returnObject = form.ReferencePaths;
						RemoveBlankPaths(returnObject as ReferencePathCollection);
					}
					else
					{
						//HACK: we must collapse any child members, otherwise a PropertyGrid bug will cause an error msg to appear
						CollapseChildren(gridItem);
						returnObject = value;
					}

					return returnObject;
				}
				else
					return value;
			}

			private void CollapseChildren(GridItem gridItem)
			{
				if (!gridItem.Expanded) return;
				foreach(GridItem childGridItem in gridItem.GridItems)
				{
					if(childGridItem.Expanded)
					{
						childGridItem.Expanded=false;
					}
				}
			}
	
			/// <summary>
			/// Gets the edit style.
			/// </summary>
			/// <param name="context">Context.</param>
			/// <returns></returns>
			public override System.Drawing.Design.UITypeEditorEditStyle GetEditStyle(System.ComponentModel.ITypeDescriptorContext context)
			{
				return UITypeEditorEditStyle.Modal;
			}

			private void RemoveBlankPaths(ReferencePathCollection coll)
			{
				if (coll == null) return;

				for (int i = 0; i < coll.Count; i++)
				{
					ReferencePath refPath = coll[i];
					if (refPath.Path.Length == 0)
					{
						coll.RemoveAt(i);
						i--;
					}
				}
			}
		}
	}
}
