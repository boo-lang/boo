namespace BooBinding.Gui

import System
//using System.IO;
//using System.Drawing;
//using System.Windows.Forms;

import ICSharpCode.SharpDevelop.Gui.Dialogs

//using ICSharpCode.SharpDevelop.Internal.Project;
//using ICSharpCode.SharpDevelop.Internal.ExternalTool;
//using ICSharpCode.Core.Services;
//using ICSharpCode.Core.Properties;
//using ICSharpCode.Core.AddIns.Codons;

class CodeGenerationPanel(AbstractOptionPanel):
	public override def LoadPanelContents() as void:
		pass
	
	public override def StorePanelContents() as bool:
		return true



//namespace CSharpBinding
//{
//	public class CodeGenerationPanel : AbstractOptionPanel
//	{
//		CSharpCompilerParameters compilerParameters = null;
//		
//		public override void LoadPanelContents()
//		{
//			this.compilerParameters = (CSharpCompilerParameters)((IProperties)CustomizationObject).GetProperty("Config");
//			
//			System.Windows.Forms.PropertyGrid grid = new System.Windows.Forms.PropertyGrid();
//			grid.Dock = DockStyle.Fill;
//			grid.SelectedObject = compilerParameters;
//			Controls.Add(grid);
//		}
//		
//		public override bool StorePanelContents()
//		{
//			return true;
//		}
//	}
//}
//
