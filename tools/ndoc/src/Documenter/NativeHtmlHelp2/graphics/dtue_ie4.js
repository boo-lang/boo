// VERSION 9226
// This script works with IE4.x

//************************** LOCALIZATION VARIABLES ***************************

// Variables for Feedback links
var L_FeedbackLink_TEXT = "Send feedback to Visual Studio";
var L_MessageLink_TEXT = "Microsoft Knowledgebase Link";

// Variable for Animation text
var L_Animation_Text = "Click to animate";

// Variables for Expand-Collapse functions
var L_ExpandAll_TEXT = "Expand All";
var L_CollapseAll_TEXT = "Collapse All";
var L_ExColl_TEXT = "Click to Expand or Collapse";

//*************************** END LOCALIZATION ********************************

var theImg, theDiv, e;
var imgArray = new Array(new Image(), new Image(), new Image(), new Image(), new Image());
var baseUrl = jsPath; //jsPath comes from the dtuelink.js
var emailalias = "vsdocs";

//***************************** END VARIABLES *********************************


// ****************************************************************************
// *                             Expand-Collapse                              *
// ****************************************************************************

function makeExpandable(title, level){
	if (title!="")document.write("<a href=\"\#\" onClick='callExpand()' id=\"ExPand\" Class=\"expandLink" + level + "\"><IMG CLASS=\"ExPand\" SRC=\"" + baseUrl + "coe.gif\" HEIGHT=9 WIDTH=9 ALT=\"" + L_ExColl_TEXT + "\" BORDER=0>&nbsp;" + title + "</a><BR><div CLASS=\"expandBody" + level + "\">");
	else document.write("<a href=\"\#\" id=\"ExPandAll\" onClick='callExpandAll()' Class=\"expandLink" + level + "\"><IMG CLASS=\"ExPandAll\" SRC=\"" + baseUrl + "coe.gif\" HEIGHT=9 WIDTH=9 ALT=\"" + L_ExColl_TEXT + "\" BORDER=0>&nbsp;" + L_ExpandAll_TEXT + "</A>");
}

function getImage(){
	for (var a = 0; a < 7; a++){
      	if ((e.tagName != 'A') && (e.parentElement != null)){e = e.parentElement;}
		var elemImg = e;
		if(elemImg.tagName == 'A'){elemImg = e.all.tags('IMG')(0); break;}}
return elemImg;}

function callExpand(){
//DO EXPAND/COLLAPSE
	e = window.event.srcElement;

	//PREVENTS NAVIGATION ON HREF TAGS
	event.returnValue = false;

	//FIND THE EXPAND/COLLAPSE PORTION AND ASCERTAIN BLOCK VS NONE
	var theDiv = GrabtheExpandDiv(e);
		
	//THIS PART WRITES THE PROPER IMAGE BESIDE THE TEXT
	if (theDiv.style.display == 'block'){
		var theImg = getImage(e);
		theImg.src = baseUrl + "coe.gif";
		theDiv.style.display = "none";}
	else {
		var theImg = getImage(e);
		theImg.src = baseUrl + "coc.gif";
		theDiv.style.display = "block";}
return;
}

function GrabtheExpandDiv(e){
//FIND AREA TO EXPAND/COLLAPSE
	var theExpandDiv;
	for (var a = 0; a < 7; a++){
    	var theTag = e.sourceIndex + e.children.length + a;
    	theExpandDiv= document.all(theTag);
     	if (((theExpandDiv.tagName == 'DIV') && ((theExpandDiv.className.toLowerCase().indexOf("expandbody")!=-1))) || theTag == document.all.length){break;}}
return theExpandDiv;
}

function callExpandAll(){
	e = window.event.srcElement;
	//PREVENTS NAVIGATION ON HREF TAGS
	event.returnValue = false;
	if (e.tagName=="IMG") e = e.parentElement;
	//Expand or Collapse?
	if (e.innerHTML.indexOf(L_ExpandAll_TEXT) != -1){eOrC="block"}else{eOrC="none"}
	if (eOrC=="block"){
		e.innerHTML="<IMG CLASS='ExPand' SRC=\"" + baseUrl + "coc.gif\" HEIGHT='9' WIDTH='9' ALT='" + L_ExColl_TEXT + "' BORDER='0'>&nbsp;" + L_CollapseAll_TEXT;}
	else{
		e.innerHTML="<IMG CLASS='ExPand' SRC=\"" + baseUrl + "coe.gif\" HEIGHT='9' WIDTH='9' ALT='" + L_ExColl_TEXT + "' BORDER='0'>&nbsp;" + L_ExpandAll_TEXT;}
	for (var a = 0; a < document.all.length; a++){ 
		e=document.all[a];

		if (e.id.indexOf('ExPand') != -1){

			if (e.id.indexOf('ExPandAll') == -1){

			var theDiv = GrabtheExpandDiv(e);
			if (eOrC == 'none'){
				theImg = getImage(e);
				theImg.src = baseUrl + "coe.gif";
				theDiv.style.display = eOrC;}
			else {
				theImg = getImage(e);
				theImg.src = baseUrl + "coc.gif";
				theDiv.style.display = eOrC;}
			}
		}
	}
return;
}


// ****************************************************************************
// *                            Graphic Animation                             *
// ****************************************************************************

function insertAnimation(name, number) {
	imgArray[number].src = name + ".gif";
	document.write("<input type=\"image\" src=\"" + baseUrl + "AnimButton1.gif\" onClick=\"changeToAnimate('" + name + "', " + number + ");\" onMouseDown=\"src='" + baseUrl + "AnimButton2.gif';\" onMouseUp=\"src='" + baseUrl + "AnimButton1.gif';\">&nbsp;" + L_Animation_Text + "<br><br><img name=\"" + name + "\" src=\"" + name + "1.gif\">");
	}

function changeToAnimate(imgName, number) {
	document[imgName].src = imgArray[number].src;
	}


// ****************************************************************************
// *                        Feedback & other footer links                     *
// ****************************************************************************

function writefeedbacklink(){
	//writes feedback link
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
