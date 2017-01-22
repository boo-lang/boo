// VERSION 9226
// This script works with IE 3.x

//************************** LOCALIZATION VARIABLES ***************************

// Variables for Feedback links
var L_FeedbackLink_TEXT = "Send feedback to Visual Studio";
var L_MessageLink_TEXT = "Microsoft Knowledgebase Link";

// Variables for Expand-Collapse functions
var L_Expand_TEXT = "<B>Expand/Collapse functionality:</B> Your browser does not support the functionality in this topic. You may install a browser that does at ";
var L_Expand_URL = "http://www.microsoft.com/windows/IE/";

//*************************** END LOCALIZATION ********************************

var baseUrl = jsPath; //jsPath comes from the dtuelink.js
var emailalias = "vsdocs";


//***************************** END VARIABLES *********************************

// ****************************************************************************
// *                             Expand-Collapse                              *
// ****************************************************************************

function makeExpandable(title, level){
	document.write("<P>" + L_Expand_TEXT + "<A href=\"" + L_Expand_URL + "\">" + L_Expand_URL + "</A>.</P>");
}


// ****************************************************************************
// *                            Graphic Animation                             *
// ****************************************************************************

function insertAnimation(name, number) {
	name = name + "1.gif";
	document.write("<img name=\"" + name + "\" src=\"" + name + "\">");
}


// ****************************************************************************
// *                        Feedback & other footer links                     *
// ****************************************************************************

function writefeedbacklink(){
	contextid = arguments[1];
	topictitle = arguments[2];

	href = "mailto:"+emailalias+"?subject=Feedback%20on%20topic%20-%20"+topictitle+",%20URL%20-%20"+contextid;
	document.writeln("<a href="+href+">"+L_FeedbackLink_TEXT+"</a>");
}


function writemessagelink(){
	//Writes jump to PSS web site redirector
	//code tbd
	//Use L_MessageLink_TEXT variable from Localization Variables located at top of script.
	msdnid = arguments[0];
	href = "http://www.microsoft.com/contentredirect.asp?prd=vs&pver=7.0&id="+msdnid;
	document.writeln("<a href="+href+">"+L_MessageLink_TEXT+"</a>");
}
