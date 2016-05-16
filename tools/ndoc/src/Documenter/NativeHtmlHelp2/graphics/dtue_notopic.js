// VERSION 7.1.2267

/*------------------------------------ Localizable strings ------------------------------------------*/
var L_CannotFindVSCC_ErrorMessage		= "Cannot find the Visual Studio .NET Combined Help Collection.";
var L_CannotFindNslist_ErrorMessage		= "Cannot find the Help Collection list";
var L_CannotFindPlugIn_ErrorMessage		= "Cannot find the plug-in list of Visual Studio .NET Combined Help Collection.";
var L_CommonTitle_Message				= "Visual Studio .NET Combined Help Collection Manager";
var L_ConfirmUpdate_Message				= "Before updating the Visual Studio .NET Combined Help Collection (VSCC), make certain that no application opened by any other user is currently accessing the VSCC on this machine. This update will take several minutes to complete, and will require that you close and restart all instances of Visual Studio .NET and Microsoft Document Explorer. Proceed?";
var L_UpdateSuccess_Message				= "For this VSCC update to be completed, you must now close all instances of Visual Studio .NET and Microsoft Document Explorer. When you restart either program, the contents of the VSCC will be updated. The program will take several minutes to reopen as it completes this update.";
var L_ConfirmUnsave_Message				= "Would you like to save the changes made to the Visual Studio .NET Combined Help Collection?";
var L_ErrorNum_ErrorMessage				= "Error Number: ";
var L_ErrorDesc_ErrorMessage			= "Error Description: ";
var L_NotExist_ErrorMessage				= "At least one of the collections is not available.";
var L_CannotRun_ErrorMessage			= "The ActiveX component is disable.";
var L_AccessDenied_ErrorMessage			= "Insufficient permissions to complete action.";
/*---------------------------------------------------------------------------------------------------*/

var bPlugIn = false;
var msCommonNamespaceName = "MS.VSCC.2003";

if (!hasCorrectPlugin())
	//document.write('<META HTTP-EQUIV="Refresh" CONTENT="0; URL=conHelpIsNotInstalledforVisualStudio.htm">');
	document.location.href = "conHelpIsNotInstalledforVisualStudio.htm";

function hasCorrectPlugin()
{
	try
	{
		var objRegWalker = null;
		var objRegNamespaceList = null;

		objRegWalker = new ActiveXObject("HxDs.HxRegistryWalker");
		objRegNamespaceList = objRegWalker.RegisteredNamespaceList("");
		for (var i = 1; i <= objRegNamespaceList.Count; i++)
		{	
			var objRegNamespace = objRegNamespaceList.Item(i);
			if (objRegNamespace.Name.toUpperCase() == "MS.VSCC.2003")
			{
				var objPluginList = objRegNamespace.GetProperty(1 /*HxRegNamespacePlugInList*/);
				for (var j = 1; j <= objPluginList.Count; j++)
				{
					var objPlugin = objPluginList.Item(j);
					if (isCorrectMSDNVersion(objPlugin.GetProperty(0 /*HxRegPlugInName*/).toUpperCase()))
					{
						return true;
					}
				}
				break;
			}
			
		}
	}
	catch (e)
	{
		
	}
	return false;
}

function isCorrectMSDNVersion(strVersion)
{
	if (strVersion.substr(0, 11) == "MS.MSDNQTR.")
	{
		if (isRightMonthYear(strVersion))
			return true;
	}
	return false;
}

function isRightMonthYear(strDate)
{
	var date = strDate.substr(11).substr(0, 7);
	var year = date.substr(0, 4);
	var month = date.substr(4, 3);
	if (year > 2003)
		return true;
	else if (year == 2003 && month != "JAN")
		return true;
	return false;
}