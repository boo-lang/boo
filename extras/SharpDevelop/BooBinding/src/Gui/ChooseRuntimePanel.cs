using System;
using System.IO;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

using ICSharpCode.SharpDevelop.Internal.Project;
using ICSharpCode.Core.Properties;

using ICSharpCode.Core.AddIns.Codons;
using ICSharpCode.SharpDevelop.Gui.Dialogs;
using ICSharpCode.Core.Services;

namespace BooBinding
{
	public class ChooseRuntimePanel : AbstractOptionPanel
	{
		BooCompilerParameters config = null;
		
		public override void LoadPanelContents()
		{
			/*
			SetupFromXml(Path.Combine(PropertyService.DataDirectory, 
			                          @"resources\panels\ChooseRuntimePanel.xfrm"));
			
			this.config = (CSharpCompilerParameters)((IProperties)CustomizationObject).GetProperty("Config");
			
			((RadioButton)ControlDictionary["msnetRadioButton"]).Checked = config.NetRuntime == NetRuntime.MsNet;
			((RadioButton)ControlDictionary["monoRadioButton"]).Checked  = config.NetRuntime == NetRuntime.Mono;
			((RadioButton)ControlDictionary["mintRadioButton"]).Checked  = config.NetRuntime == NetRuntime.MonoInterpreter;
			
			((RadioButton)ControlDictionary["cscRadioButton"]).Checked = config.CsharpCompiler == CsharpCompiler.Csc;
			((RadioButton)ControlDictionary["mcsRadioButton"]).Checked = config.CsharpCompiler == CsharpCompiler.Mcs;
			*/
		}
		
		public override bool StorePanelContents()
		{
			/*
			if (((RadioButton)ControlDictionary["msnetRadioButton"]).Checked) {
				config.NetRuntime =  NetRuntime.MsNet;
			} else if (((RadioButton)ControlDictionary["monoRadioButton"]).Checked) {
				config.NetRuntime =  NetRuntime.Mono;
			} else {
				config.NetRuntime =  NetRuntime.MonoInterpreter;
			}
			config.CsharpCompiler = ((RadioButton)ControlDictionary["cscRadioButton"]).Checked ? CsharpCompiler.Csc : CsharpCompiler.Mcs;
			*/			
			return true;
		}
	}
}
