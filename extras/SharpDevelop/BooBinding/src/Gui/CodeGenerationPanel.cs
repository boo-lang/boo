// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.IO;
using System.Drawing;
using System.Windows.Forms;

using ICSharpCode.SharpDevelop.Internal.Project;
using ICSharpCode.SharpDevelop.Internal.ExternalTool;
using ICSharpCode.SharpDevelop.Gui.Dialogs;
using ICSharpCode.Core.Services;
using ICSharpCode.Core.Properties;
using ICSharpCode.Core.AddIns.Codons;

namespace BooBinding
{
	public class CodeGenerationPanel : AbstractOptionPanel
	{
		BooCompilerParameters compilerParameters = null;
		
		public override void LoadPanelContents()
		{
			/*
			this.compilerParameters = (CSharpCompilerParameters)((IProperties)CustomizationObject).GetProperty("Config");
			
			System.Windows.Forms.PropertyGrid grid = new System.Windows.Forms.PropertyGrid();
			grid.Dock = DockStyle.Fill;
			grid.SelectedObject = compilerParameters;
			Controls.Add(grid);
			
//			SetupFromXml(Path.Combine(PropertyService.DataDirectory, 
//			                          @"resources\panels\ProjectOptions\CodeGenerationPanel.xfrm"));
//			
//			((ComboBox)ControlDictionary["compileTargetComboBox"]).Items.Add(StringParserService.Parse("${res:Dialog.Options.PrjOptions.Configuration.CompileTarget.Exe}"));
//			((ComboBox)ControlDictionary["compileTargetComboBox"]).Items.Add(StringParserService.Parse("${res:Dialog.Options.PrjOptions.Configuration.CompileTarget.WinExe}"));
//			((ComboBox)ControlDictionary["compileTargetComboBox"]).Items.Add(StringParserService.Parse("${res:Dialog.Options.PrjOptions.Configuration.CompileTarget.Library}"));
//			((ComboBox)ControlDictionary["compileTargetComboBox"]).Items.Add(StringParserService.Parse("${res:Dialog.Options.PrjOptions.Configuration.CompileTarget.Module}"));
//			
			
//			
//			((ComboBox)ControlDictionary["compileTargetComboBox"]).SelectedIndex = (int)compilerParameters.CompileTarget;
//			ControlDictionary["symbolsTextBox"].Text   = compilerParameters.DefineSymbols;
//			ControlDictionary["mainClassTextBox"].Text = compilerParameters.MainClass;

//
//			((CheckBox)ControlDictionary["generateDebugInformationCheckBox"]).Checked = compilerParameters.Debugmode;
//			((CheckBox)ControlDictionary["generateXmlOutputCheckBox"]).Checked        = compilerParameters.GenerateXmlDocumentation;
//			((CheckBox)ControlDictionary["enableOptimizationCheckBox"]).Checked       = compilerParameters.Optimize;
//			((CheckBox)ControlDictionary["allowUnsafeCodeCheckBox"]).Checked       = compilerParameters.UnsafeCode;
//			((CheckBox)ControlDictionary["generateOverflowChecksCheckBox"]).Checked       = compilerParameters.GenerateOverflowChecks;
//			((CheckBox)ControlDictionary["warningsAsErrorsCheckBox"]).Checked       = !compilerParameters.RunWithWarnings;
//			
//			((NumericUpDown)ControlDictionary["warningLevelNumericUpDown"]).Value = compilerParameters.WarningLevel;
			*/
		}
		
		public override bool StorePanelContents()
		{
			/*
//			if (compilerParameters == null) {
//				return true;
//			}
//			FileUtilityService fileUtilityService = (FileUtilityService)ServiceManager.Services.GetService(typeof(FileUtilityService));
//			
//			if (ControlDictionary["win32IconTextBox"].Text.Length > 0) {
//				if (!fileUtilityService.IsValidFileName(ControlDictionary["win32IconTextBox"].Text)) {
//					MessageBox.Show("Invalid Win32Icon specified", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1);
//					return false;
//				}
//				if (!File.Exists(ControlDictionary["win32IconTextBox"].Text)) {
//					MessageBox.Show("Win32Icon doesn't exists", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1);
//					return false;
//				}
//			}
//			
//			compilerParameters.CompileTarget = (CompileTarget)((ComboBox)ControlDictionary["compileTargetComboBox"]).SelectedIndex;
//			compilerParameters.DefineSymbols = ControlDictionary["symbolsTextBox"].Text;
//			compilerParameters.MainClass     = ControlDictionary["mainClassTextBox"].Text;
//			
//			compilerParameters.Debugmode                = ((CheckBox)ControlDictionary["generateDebugInformationCheckBox"]).Checked;
//			compilerParameters.GenerateXmlDocumentation = ((CheckBox)ControlDictionary["generateXmlOutputCheckBox"]).Checked;
//			compilerParameters.Optimize                 = ((CheckBox)ControlDictionary["enableOptimizationCheckBox"]).Checked;
//			compilerParameters.UnsafeCode               = ((CheckBox)ControlDictionary["allowUnsafeCodeCheckBox"]).Checked;
//			compilerParameters.GenerateOverflowChecks   = ((CheckBox)ControlDictionary["generateOverflowChecksCheckBox"]).Checked;
//			compilerParameters.RunWithWarnings          = !((CheckBox)ControlDictionary["warningsAsErrorsCheckBox"]).Checked;
//			compilerParameters.WarningLevel             = (int)((NumericUpDown)ControlDictionary["warningLevelNumericUpDown"]).Value;
			*/
			return true;
		}
	}
}
