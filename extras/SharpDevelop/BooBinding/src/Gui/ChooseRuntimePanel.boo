namespace BooBinding.Gui

import System

import ICSharpCode.SharpDevelop.Gui.Dialogs

//using System.IO;
//using System.Drawing;
//using System.Windows.Forms;

//using ICSharpCode.SharpDevelop.Internal.Project;
//using ICSharpCode.SharpDevelop.Internal.ExternalTool;
//using ICSharpCode.SharpDevelop.Gui.Dialogs;
//using ICSharpCode.Core.Services;
//using ICSharpCode.Core.Properties;
//using ICSharpCode.Core.AddIns.Codons;

class ChooseRuntimePanel(AbstractOptionPanel):
	public override def LoadPanelContents() as void:
		pass
	
	public override def StorePanelContents() as bool:
		return true


//using System;
//using System.IO;
//using System.Drawing;
//using System.Collections;
//using System.ComponentModel;
//using System.Windows.Forms;

//using ICSharpCode.SharpDevelop.Internal.Project;
//using ICSharpCode.Core.Properties;

//using ICSharpCode.Core.AddIns.Codons;
//using ICSharpCode.SharpDevelop.Gui.Dialogs;
//using ICSharpCode.Core.Services;

//namespace CSharpBinding
//{
//	public class ChooseRuntimePanel : AbstractOptionPanel
//	{
//		CSharpCompilerParameters config = null;
//		
//		public override void LoadPanelContents()
//		{
//			SetupFromXml(Path.Combine(PropertyService.DataDirectory, 
//			                          @"resources\panels\ChooseRuntimePanel.xfrm"));
//			
//			this.config = (CSharpCompilerParameters)((IProperties)CustomizationObject).GetProperty("Config");
//			
//			((RadioButton)ControlDictionary["msnetRadioButton"]).Checked = config.NetRuntime == NetRuntime.MsNet;
//			((RadioButton)ControlDictionary["monoRadioButton"]).Checked  = config.NetRuntime == NetRuntime.Mono;
//			((RadioButton)ControlDictionary["mintRadioButton"]).Checked  = config.NetRuntime == NetRuntime.MonoInterpreter;
//			
//			((RadioButton)ControlDictionary["cscRadioButton"]).Checked = config.CsharpCompiler == CsharpCompiler.Csc;
//			((RadioButton)ControlDictionary["mcsRadioButton"]).Checked = config.CsharpCompiler == CsharpCompiler.Mcs;
//		}
//		
//		public override bool StorePanelContents()
//		{
//			if (((RadioButton)ControlDictionary["msnetRadioButton"]).Checked) {
//				config.NetRuntime =  NetRuntime.MsNet;
//			} else if (((RadioButton)ControlDictionary["monoRadioButton"]).Checked) {
//				config.NetRuntime =  NetRuntime.Mono;
//			} else {
//				config.NetRuntime =  NetRuntime.MonoInterpreter;
//			}
//			config.CsharpCompiler = ((RadioButton)ControlDictionary["cscRadioButton"]).Checked ? CsharpCompiler.Csc : CsharpCompiler.Mcs;
//			
//			return true;
//		}
//	}
//}
