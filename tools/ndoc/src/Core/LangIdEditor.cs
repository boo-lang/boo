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
using System.Diagnostics;
using System.Drawing.Design;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace NDoc.Core.PropertyGridUI
{
	/// <summary>
	/// 
	/// </summary>
	public class LangIdEditor : UITypeEditor
	{
		private IWindowsFormsEditorService editorService = null;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="context"></param>
		/// <returns></returns>
		public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
		{
			if (context != null && context.Instance != null) 
			{
				return UITypeEditorEditStyle.DropDown;
			}
			return base.GetEditStyle(context);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="context"></param>
		/// <param name="provider"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
		{
			if (context != null
				&& context.Instance != null
				&& provider != null) 
				try
				{
					// get the editor service
					editorService = (IWindowsFormsEditorService)
						provider.GetService(typeof(IWindowsFormsEditorService));

					// create the ListBox
					ListBox listBox = new ListBox();
					listBox.Click += new EventHandler(List_Click);
            
					// modify the list's properties including the Item list
					FillInList(context, provider, listBox, (short)value);

					// let the editor service place the list on screen and manage its events
					editorService.DropDownControl(listBox);
   
					// return the updated value;
					if (listBox.SelectedItem !=null)
					{
						return ((LangListItem)listBox.SelectedItem).LangID;
					}
					else
					{
						return value;
					}
				}  
				finally
				{
					editorService = null;
				}
			else
				return value;

		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		private void List_Click(object sender, EventArgs args)
		{
			if (editorService != null)
				editorService.CloseDropDown();
		}  


		/// <summary>
		/// 
		/// </summary>
		/// <param name="context"></param>
		/// <param name="provider"></param>
		/// <param name="listBox"></param>
		/// <param name="value"></param>
		public void FillInList(ITypeDescriptorContext context, IServiceProvider provider, ListBox listBox, short value)
		{
			int SelectedIndex=-1;
			for (int i=0; i<Languages.Length;i++)
			{
				listBox.Items.Add(Languages[i]);
				if (Languages[i].LangID==value) SelectedIndex = i;
			}
			if (SelectedIndex!=-1) listBox.SelectedIndex=SelectedIndex;
		}

		static readonly LangListItem[] Languages=
			{
				new LangListItem(1078, "Afrikaans"),
				new LangListItem(1052, "Albanian"),
				new LangListItem(5121, "Arabic (Algeria)"),
				new LangListItem(15361, "Arabic (Bahrain)"),
				new LangListItem(3073, "Arabic (Egypt)"),
				new LangListItem(2049, "Arabic (Iraq)"),
				new LangListItem(11265, "Arabic (Jordan)"),
				new LangListItem(13313, "Arabic (Kuwait)"),
				new LangListItem(12289, "Arabic (Lebanon)"),
				new LangListItem(4097, "Arabic (Libya)"),
				new LangListItem(6145, "Arabic (Morocco)"),
				new LangListItem(8193, "Arabic (Oman)"),
				new LangListItem(16385, "Arabic (Qatar)"),
				new LangListItem(1025, "Arabic (Saudi Arabia)"),
				new LangListItem(10241, "Arabic (Syria)"),
				new LangListItem(7169, "Arabic (Tunisia)"),
				new LangListItem(14337, "Arabic (U.A.E.)"),
				new LangListItem(9217, "Arabic (Yemen)"),
				new LangListItem(1069, "Basque"),
				new LangListItem(1059, "Belarusian"),
				new LangListItem(1026, "Bulgarian"),
				new LangListItem(1027, "Catalan"),
				new LangListItem(3076, "Chinese (Hong Kong)"),
				new LangListItem(2052, "Chinese (PRC)"),
				new LangListItem(4100, "Chinese (Singapore)"),
				new LangListItem(1028, "Chinese (Taiwan)"),
				new LangListItem(1050, "Croatian"),
				new LangListItem(1029, "Czech"),
				new LangListItem(1030, "Danish"),
				new LangListItem(2067, "Dutch (Belgian)"),
				new LangListItem(1043, "Dutch (Standard)"),
				new LangListItem(3081, "English (Australian)"),
				new LangListItem(10249, "English (Belize)"),
				new LangListItem(4105, "English (Canadian)"),
				new LangListItem(9225, "English (Caribbean)"),
				new LangListItem(6153, "English (Ireland)"),
				new LangListItem(8201, "English (Jamaica)"),
				new LangListItem(5129, "English (New Zealand)"),
				new LangListItem(7177, "English (South Africa)"),
				new LangListItem(11273, "English (Trinidad)"),
				new LangListItem(2057, "English (United Kingdom)"),
				new LangListItem(1033, "English (United States)"),
				new LangListItem(1061, "Estonian"),
				new LangListItem(1080, "Faeroese"),
				new LangListItem(1065, "Farsi"),
				new LangListItem(1035, "Finnish"),
				new LangListItem(2060, "French (Belgian)"),
				new LangListItem(3084, "French (Canadian)"),
				new LangListItem(5132, "French (Luxembourg)"),
				new LangListItem(1036, "French (Standard)"),
				new LangListItem(4108, "French (Swiss)"),
				new LangListItem(3079, "German (Austrian)"),
				new LangListItem(5127, "German (Liechtenstein)"),
				new LangListItem(4103, "German (Luxembourg)"),
				new LangListItem(1031, "German (Standard)"),
				new LangListItem(2055, "German (Swiss)"),
				new LangListItem(1032, "Greek"),
				new LangListItem(1037, "Hebrew"),
				new LangListItem(1038, "Hungarian"),
				new LangListItem(1039, "Icelandic"),
				new LangListItem(1057, "Indonesian"),
				new LangListItem(1040, "Italian (Standard)"),
				new LangListItem(2064, "Italian (Swiss)"),
				new LangListItem(1041, "Japanese"),
				new LangListItem(1042, "Korean"),
				new LangListItem(2066, "Korean (Johab)"),
				new LangListItem(1062, "Latvian"),
				new LangListItem(1063, "Lithuanian"),
				new LangListItem(1044, "Norwegian (Bokmal)"),
				new LangListItem(2068, "Norwegian (Nynorsk)"),
				new LangListItem(1045, "Polish"),
				new LangListItem(1046, "Portuguese (Brazil)"),
				new LangListItem(2070, "Portuguese (Portugal)"),
				new LangListItem(1048, "Romanian"),
				new LangListItem(1049, "Russian"),
				new LangListItem(3098, "Serbian (Cyrillic)"),
				new LangListItem(2074, "Serbian (Latin)"),
				new LangListItem(1051, "Slovak"),
				new LangListItem(1060, "Slovenian"),
				new LangListItem(11274, "Spanish (Argentina)"),
				new LangListItem(16394, "Spanish (Bolivia)"),
				new LangListItem(13322, "Spanish (Chile)"),
				new LangListItem(9226, "Spanish (Colombia)"),
				new LangListItem(5130, "Spanish (Costa Rica)"),
				new LangListItem(7178, "Spanish (Dominican Republic)"),
				new LangListItem(12298, "Spanish (Ecuador)"),
				new LangListItem(17418, "Spanish (El Salvador)"),
				new LangListItem(4106, "Spanish (Guatemala)"),
				new LangListItem(18442, "Spanish (Honduras)"),
				new LangListItem(2058, "Spanish (Mexican)"),
				new LangListItem(3082, "Spanish (Modern Sort)"),
				new LangListItem(19466, "Spanish (Nicaragua)"),
				new LangListItem(6154, "Spanish (Panama)"),
				new LangListItem(15370, "Spanish (Paraguay)"),
				new LangListItem(10250, "Spanish (Peru)"),
				new LangListItem(20490, "Spanish (Puerto Rico)"),
				new LangListItem(1034, "Spanish (Traditional Sort)"),
				new LangListItem(14346, "Spanish (Uruguay)"),
				new LangListItem(8202, "Spanish (Venezuela)"),
				new LangListItem(1053, "Swedish"),
				new LangListItem(2077, "Swedish (Finland)"),
				new LangListItem(1054, "Thai"),
				new LangListItem(1055, "Turkish"),
				new LangListItem(1058, "Ukrainian"),
				new LangListItem(1066, "Vietnamese")
			};

		struct LangListItem
		{
			public short LangID;
			private string Description;

			public LangListItem(short langID, string description)
			{
				LangID = langID;
				Description = description;
			}

			public override string ToString()
			{
				return LangID.ToString() + "\t" + Description;
			}
		}
	}

}
