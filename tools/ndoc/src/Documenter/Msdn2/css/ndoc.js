// This script works with IE 5.x+. Optimized for IE6.x
var isPersistent= false;
var ieVer = getIEVersion();

writeCSS();

function getIEVersion() {
//determines the IE version. Returns 0 if not IE
	var verNum = 0
	if (navigator.appName == "Microsoft Internet Explorer") {
		var sVer = window.navigator.userAgent
		var msie = sVer.indexOf ( "MSIE " )
		if ( msie > 0 ) {	// browser is Microsoft Internet Explorer; return version number
			verNum = parseFloat( sVer.substring ( msie+5, sVer.indexOf ( ";", msie ) ) );
		}
	}
	return verNum;
}

function writeCSS() {
	document.writeln('<SCRIPT FOR="reftip" EVENT="onclick">window.event.cancelBubble = true;</SCRIPT>');
	document.writeln('<SCRIPT FOR="cmd_lang" EVENT="onclick">langClick(this);</SCRIPT>');
	document.writeln('<SCRIPT FOR="cmd_filter" EVENT=onclick>filterClick(this);</SCRIPT>');
}

//************************** LOCALIZATION VARIABLES ***************************

var L_SeeAlso_TEXT = "See Also"
var L_Requirements_TEXT = "Requirements"
var L_QuickInfo_TEXT = "QuickInfo"
var L_FilterTip_TEXT = "Language Filter"
var L_Language_TEXT = "Language"
var L_ShowAll_TEXT = "Show All"

// defines the running head popup box. Localizable
var L_PopUpBoxStyle_Style = "WIDTH:200PX; PADDING:5px 7px 7px 7px; BACKGROUND-COLOR:#FFFFCC; BORDER:SOLID 1 #999999; VISIBILITY:HIDDEN; POSITION:ABSOLUTE; TOP:0PX; LEFT:0PX; Z-INDEX:2;";

//*************************** END LOCALIZATION ********************************

//***ScriptSettings
var bRefTips = true		//Show RefTips
var bInCHM = false		//CHM check
var popOpen, theImg, theDiv, e;
var advanced = false;
var curLangs = null;
var scrollPos = 0;
var baseUrl = ""; //jsPath comes from the dtuelink.js
var popupDIV = "<DIV ID='popUpWindow' STYLE='"+L_PopUpBoxStyle_Style+"'>" + "</DIV>";

//***************************** END VARIABLES *********************************

// ****************************************************************************
// *                           Common code                                    *
// ****************************************************************************

if (ieVer >= 5) {

	// Check for <meta Name="RefTips" Content="False">
	if (bRefTips==true){
		var mColl = document.all.tags("META");
		for (i=0; i<mColl.length; i++){
			if (mColl(i).name.toUpperCase()=="REFTIPS"){
				if (mColl(i).content.toUpperCase()=="FALSE") bRefTips = false;
			}
		}
	}

	// Check for <META Name="InChm" Content="True">
	if (bInCHM==false){
		var mColl = document.all.tags("META");
		for (i=0; i<mColl.length; i++){
			if (mColl(i).name.toUpperCase()=="INCHM"){
				if (mColl(i).content.toUpperCase()=="TRUE") bInCHM = true;
			}
		}
	}

	var advanced = true;
}

if (advanced) {
	window.onload = bodyOnLoad;
	window.onbeforeprint = set_to_print;
	window.onafterprint = reset_form;
}

//**************************** END COMMON CODE ********************************




function getXMLText(term) {
	var out = xmldoc.selectSingleNode("/UI/String[@Id='" + term + "']").text;		
	return out;
}



function finishOnLoad(){
	document.onkeypress = ieKey;
	window.onresize = closeIE4;
	document.body.onclick = bodyOnClick;
	//IF THE USER HAS IE4+ THEY WILL BE ABLE TO VIEW POPUP BOXES
	if (advanced){
		document.body.insertAdjacentHTML('beforeEnd', popupDIV);
	}
	return;
}


function bodyOnClick(){
	if (advanced) {
		var elem = window.event.srcElement;
		for (; elem; elem = elem.parentElement) {
			if (elem.id == "reftip")
				return;
		}
		hideTip();
		closeMenu();
		hideSeeAlso();
		resizeBan();
	}
}


function ieKey(){
	if (window.event.keyCode == 27){
		hideTip();
		closeMenu();
		hideSeeAlso();
		resizeBan();
		closeIE4();
	}
return;
}


function closeIE4(){
	document.all.popUpWindow.style.visibility = "hidden";
	popOpen = false;
	resizeBan();  //also resize the non-scrolling banner
return;
}


function bodyOnLoad(){

	if (advanced) {
		
		try{
			// Wire up event handlers that required the body to have been instantiated.
			document.body.oncopy = procCodeSelection;
		} catch(e){}
		
		// Process language-specific subsections and data structures
		// Create the language menu
		initLangs();
		
		// Filter topic as appropriate for the context
		if (curLangs != null) {
			curLangs.filterTopicForLang(null, false);
		}

		resizeBan();
		
		if (bRefTips==true) initReftips();
		initSeeAlso();
		
	}
	
	
	finishOnLoad();
	//set the scroll position
	try{nstext.scrollTop = scrollPos;}
	catch(e){}
	
}

function Load(key)
{
	var value;
	lct = document.location + ".";
	xax = 10;
	xax = lct.indexOf("mk:@MSITStore");
	if (xax != -1) 
	{
		lct = "ms-its:" + lct.substring(14,lct.length-1);
		//alert("before reload : " + document.location);
		//alert("replace with : " + lct);
		isPersistent = false;
		document.location.replace(lct);
		isPersistent = true;
	}	 
	else
	{ 	 
		userDataCache.load("ndocSettings");
		try
		{
		value = userDataCache.getAttribute(key);
		}
		catch(e){}
		isPersistent = true;
//		alert("load key=" + key + "  value=" + value);
	}
	return value;
}

function Save(key, value)
{
	userDataCache.setAttribute(key, value);

	//alert("isPersistent : " + isPersistent);
        if (!isPersistent)
	{
		return;
	}

	lct = document.location + ".";
	xax = 10;
	xax = lct.indexOf("mk:@MSITStore");
	if (xax != -1) 
	{
		lct = "ms-its:" + lct.substring(14,lct.length-1);
		//alert("before reload : " + document.location);
		//alert("replace with : " + lct);
		isPersistent = false;
		document.location.replace(lct);
		isPersistent = true;
		//alert("after reload : " + document.location);
		//alert("isPersistent : " + isPersistent);
	}	 
	else
	{ 	 
		userDataCache.save("ndocSettings");
		//alert("save key=" + key + "  value=" + value);
	}
}

// ****************************************************************************
// *                        Language filtering                                *
// ****************************************************************************



// ------------------------- CurLangList class --------------------------------

function CurLangList(docLangList){
	// Current Lang Object ctor: Only instantiated if needed.

	// Member fields
	this.langList = null;
	this.showAll = true;

	var address = location.href;
	var bookmarkStart = address.indexOf("#");
	var tempLangList = null;
	
	// (Try 1) Check the dynamic help window for current language.
	if (tempLangList == null) {
		// Could result in a multi-item langList
		try{
			for (i=1; i< window.external.ContextAttributes.Count; i++){
				if(window.external.ContextAttributes(i).Name.toUpperCase()=="DEVLANG"){
					var b = window.external.ContextAttributes(i).Values.toArray();
				}
			}
		}
		catch(e){}
		
		// Convert lang names to user-readable values
		if (b != null ) {
			
			var listLength = b.length;
			tempLangList = new Array(listLength);
			
			for (var i = 0, end = listLength; i < end; ++i ) {
				
				// Map attrib Value to attrib Display values for select langs, otherwise use as is.
				switch (b[i].toUpperCase()) {
					case "VB" :
						tempLangList[i] = "Visual Basic";
						break;
					case "VC" :
						tempLangList[i] = "C++";
						break;
					case "CSHARP" :
						tempLangList[i] = "C#";
						break;
					case "JSCRIPT" :
						tempLangList[i] = "JScript";
						break;
					case "VBSCRIPT" :
						tempLangList[i] = "VBScript";
						break;
					case "VJ#" :
						tempLangList[i] = "VJ#";
						break;
					default :
						tempLangList[i] = b[i];
						break;
				}
			}
		}
	}

	// (Try 2) Read the persisted value 
	if (tempLangList == null){
		// Results in a single-item langList
		var lang = Load("lang");
		if (lang)
		{
			if (lang!="All")
			{
				var found = false;
				for (var i = 0, end = docLangList.length; ((i < end) && (!found)); ++i) {
					if (docLangList[i] == lang) {
						// Read if found in the current document
						found = true;
						tempLangList = new Array(1);
						tempLangList[0] = lang;
					}
				}
			}
		}
	}
	

	// Create the list of filterable languages for this context
	if (tempLangList == null) {
		// No curLang
		this.showAll = true;

	} else {
		// Add langs to the current lang list if found in the docLangList

		var foundCount = 0;
		// (1) Find how many match
		for (var i = 0, endOuter = tempLangList.length; i < endOuter; ++i ) {
			var testLang = tempLangList[i].toLowerCase();
			for (var j = 0, endInner = docLangList.length; j < endInner; ++j ) {
				if (testLang == docLangList[j].toLowerCase()) {
					++foundCount;
				}
			} 
		} 
		
		if (foundCount > 0) {

			// (2) Allocate this.langList and add the found langs to the array
			this.langList = new Array(foundCount);
			var k = 0;
			for (var i = 0, endOuter = tempLangList.length; i < endOuter; ++i ) {
				var testLang = tempLangList[i].toLowerCase();
				for (var j = 0, endInner = docLangList.length; j < endInner; ++j ) {
					if (testLang == docLangList[j].toLowerCase()) {
						this.langList[k++] = docLangList[j];
					}
				} 
			} 
			
			// Sort the list
			this.langList = this.langList.sort();

			// There is a curLang
			this.showAll = false;
		}

	}

	return;
}



function declareFilterTopicForLang(langName, saveLang){
		
	// Have something to filter on
	if (langName == null) {
		
		if (this.langList == null) {
			// Show all if no langs were found that have subsections defined in the topic
			this.showAll = true;
			unfilterLang();
			
		} else {

			// Show all context-specific langs
			// NOTE: A language would not appear in this.langList unless 
			//   it was somewhere on the topic.
			this.showAll = false;
			resetLangBlocks();
			var isMultiLang = (this.langList.length > 1);
			for (var i = 0, end = this.langList.length; i < end; ++i ) {
				filterLang(this.langList[i], isMultiLang);
			}
		}
		
	} else {
		// Show only the specified lang
		this.showAll = false;
		resetLangBlocks();
		filterLang(langName, false);
	}
	
	// Always persist the lang setting when requested by the user
	if (saveLang && isLangPersistable(langName)) {
			Save('lang', langName);
	}

	return;
}


function declareUnfilterTopicForLang(saveLang){

	this.showAll = true;
	
	if (saveLang) {
		Save('lang', 'All');
	}

	unfilterLang();
	
	return;
}


function declareCurLangListToString() {
// Creates a comma-space dilimited display string of the langs for use in the banner heading
	var outString;
	if ( this.langList == null ) {
		outString = "";
	}
	else {
		outString = this.langList[0];
		var i = 1;
		while ( i < this.langList.length ) {
			outString += ", " + this.langList[i++];
		}
	}
		
	return outString;
}




// Bind the CurLangList member function declarations to the function prototype property
CurLangList.prototype.filterTopicForLang = declareFilterTopicForLang;
CurLangList.prototype.unfilterTopicForLang = declareUnfilterTopicForLang;
CurLangList.prototype.toString = declareCurLangListToString;




// ------------------ Language filtering accessory functions ---------------------------


function isLangPersistable(langName) {
	
	var pass = false;
	var testLang = langName.toLowerCase();
	
	switch ( testLang ) {
		// Fall through to default: If listed here is persistable
		case "visual basic":
		case "c#":
		case "c++":
		case "jscript":
		case "vj#":
			pass = true;
			break;
		default:
			pass = false;
			break;
	}
	
	return pass;
}



function findLanguage(bookmark){
// Find span associated with bookmark
	var found = false
	var aColl = document.all.tags("A");
	for (i=0; i<aColl.length; i++){
		if (aColl(i).name.toUpperCase()==bookmark.toUpperCase()){
			var elem = null
			for(t = 1; t<4; t++){
				elem = document.all(aColl(i).sourceIndex + t);
				if (elem.tagName.toUpperCase()=="SPAN")
					found = true;
					break;
			}
			break;
		}
	}
//if found, filter language
	if (found){
		var lang = elem.innerText
		return lang.substring(lang.indexOf("[") + 1, lang.indexOf("]"));
	}
}



function initLangs() {
	
	// instantiates curLangs
	
	var hdr = document.all.hdr;
	if (!hdr)
		return;

	var langs = new Array;
	var spans = document.all.tags("SPAN");
	if (spans) {
		var iElem = spans.length;
		for (iElem = 0; iElem < spans.length; iElem++) {
			var elem = spans[iElem];
			if (elem.className == "lang") {

				// Update the array of unique language names.
				var a = elem.innerText.split(",");
				for (var iTok = 0; iTok < a.length; iTok++) {
					if (a[iTok]=="[A]"){
						langs[0]="A";
						elem.parentElement.outerText="";
					}
					var m = a[iTok].match(/([A-Za-z].*[A-Za-z+#0-9])/);
					if (m) {
						var iLang = 0;
						while (iLang < langs.length && langs[iLang] < m[1])
							iLang++;
						if (iLang == langs.length || langs[iLang] != m[1]) {
							var before = langs.slice(0,iLang);
							var after = langs.slice(iLang);
							langs = before.concat(m[1]).concat(after);
						}
					}
				}
			}
		}
	}

	if (langs.length > 0) {
	
		// No language button created in this case.
		
		var pres = document.all.tags("PRE");
		if (pres) {
			for (var iPre = 0; iPre < pres.length; iPre++)
				initPreElem(pres[iPre]);
		}
		
		
		// Instantiate curLangs
		curLangs = new CurLangList(langs);

		// Add current DevLang to the heading
		var iLim = document.body.all.length;
		var head = null;
		for (var i = 0; i < iLim; i++) {
			var elem = document.body.all[i];
			if (elem.tagName.match(/^(P)|(PRE)|([DOU]L)$/))
				break;
			if (elem.tagName.match(/^H[1-6]$/)) {
				head = elem;
				head.insertAdjacentHTML('beforeEnd', '<SPAN CLASS=ilang></SPAN>');
			}
		}

		iLang = 0;
		foundA = false;
		while (iLang != langs.length){
			if (langs[iLang]=="A")
				foundA = true;
			iLang++;
		}
		if (!foundA)
			// don't language button on
			var td = hdr.insertCell(0);
			
		if (td) {
			// Add the language button to the button bar.
			td.className = "button1";
			td.onkeyup = ieKey;
			td.onkeypress = langMenu;
			td.onclick = langMenu;
			td.innerHTML = '<IMG id=button1 SRC="' + baseUrl + 'Filter1a.gif' + '" ALT="'  +
				L_FilterTip_TEXT + '" BORDER=0 TABINDEX=0>';

			// Create the current lang selection menu.
			var div = '<DIV ID="lang_menu" CLASS=langMenu onkeypress=ieKey><B>' + L_Language_TEXT + '</B><UL>';
			for (var i = 0; i < langs.length; i++)
				div += '<LI><A HREF="" ONCLICK="chooseLang(this)">' + langs[i] + '</A><BR>';
			div += '<LI><A HREF="" ONCLICK="chooseAll()">' + L_ShowAll_TEXT + '</A></UL></DIV>';
			try{nsbanner.insertAdjacentHTML('afterEnd', div);}
			catch(e){try{scrbanner.insertAdjacentHTML('afterEnd', div);}catch(e){}}
		}
	}
}


function initPreElem(pre){
	var htm0 = pre.outerHTML;

	var reLang = /<span\b[^>]*class="?lang"?[^>]*>/i;
	var iFirst = -1;
	var iSecond = -1;

	iFirst = htm0.search(reLang);
	if (iFirst >= 0) {
		iPos = iFirst + 17;
		iMatch = htm0.substr(iPos).search(reLang);
		if (iMatch >= 0)
			iSecond = iPos + iMatch;
	}

	if (iSecond < 0) {
		var htm1 = trimPreElem(htm0);
		if (htm1 != htm0) {
			pre.insertAdjacentHTML('afterEnd', htm1);
			pre.outerHTML = "";
		}
	}
	else {
		var rePairs = /<(\w+)\b[^>]*><\/\1>/gi;

		var substr1 = htm0.substring(0,iSecond);
		var tags1 = substr1.replace(/>[^<>]+(<|$)/g, ">$1");
		var open1 = tags1.replace(rePairs, "");
		open1 = open1.replace(rePairs, "");

		var substr2 = htm0.substring(iSecond);
		var tags2 = substr2.replace(/>[^<>]+</g, "><");
		var open2 = tags2.replace(rePairs, "");
		open2 = open2.replace(rePairs, "");

		pre.insertAdjacentHTML('afterEnd', open1 + substr2);
		pre.insertAdjacentHTML('afterEnd', trimPreElem(substr1 + open2));
		pre.outerHTML = "";
	}	
}


function trimPreElem(htm){
	return htm.replace(/[ \r\n]*((<\/[BI]>)*)[ \r\n]*<\/PRE>/g, "$1</PRE><P></P>").replace(
		/\w*<\/SPAN>\w*((<[BI]>)*)\r\n/g, "\r\n</SPAN>$1"
		);
}


function getBlock(elem){
	while (elem && elem.tagName.match(/^([BIUA]|(SPAN)|(CODE)|(TD))$/))
		elem = elem.parentElement;
	return elem;
}


function langMenu(){
	bodyOnClick();
	var btn = window.event.srcElement
	if (btn.id=="button1"){
	btn.src = btn.src.replace("a.gif", "c.gif");}

	window.event.returnValue = false;
	window.event.cancelBubble = true;

	var div = document.all.lang_menu;
	var lnk = window.event.srcElement;
	if (div && lnk) {
		var x = lnk.offsetLeft + lnk.offsetWidth - div.offsetWidth;
		div.style.pixelLeft = (x < 0) ? 0 : x;
		div.style.pixelTop = lnk.offsetTop + lnk.offsetHeight;
		div.style.visibility = "visible";
	}
}


function chooseLang(item){

	// Called from the Lang Menu onclick event
	
	window.event.returnValue = false;
	window.event.cancelBubble = true;

	if (item != null) {
		closeMenu();
		var langName = item.innerText;
		curLangs.filterTopicForLang(langName, true);
	}
	return;
}


function chooseAll(){

	// Called from the Lang Menu onclick event

	window.event.returnValue = false;
	window.event.cancelBubble = true;

	closeMenu();

	if ( curLangs != null ) {
		curLangs.unfilterTopicForLang(true);
	}
	
	return;
}




function filterLang(langName, isMultiLang){
	
	// Call resetLangBlocks() first to reset
	// Then call filterLang() repeatedly for each lang that needs to be filtered.
	
	var spans = document.all.tags("SPAN");
	for (var i = 0; i < spans.length; i++) {
		var elem = spans[i];
		if (elem.className == "lang") {
			var isMatch = filterMatch(elem.innerText, langName);
			var block = getBlock(elem);
			if (isMatch) {
				block.style.display = "block";
			}
			elem.style.display = isMultiLang ? "inline" : "none";

			if (block.tagName == "DT") {
				var next = getNext(block);
				if (next && next.tagName == "DD") {
					if (isMatch) {
						next.style.display = "block";
					}
				}
			}
			else if (block.tagName == "DIV") {
				block.className = "filtered2";
			}
			else if (block.tagName.match(/^H[1-6]$/)) {
				if (topicHeading(block)) {
					if (isMatch) {
						var tag = null;
						if (block.children && block.children.length) {
							tag = block.children[block.children.length - 1];
							if (tag.className == "ilang") {
								tag.innerHTML = '&nbsp; [' + langName + ']';
							}
						}
					}
				}
				else {
					var next = getNext(block);
					while (next && !next.tagName.match(/^(H[1-6])$/)) {
						if (next.tagName =="DIV"){
							if (next.className.toUpperCase() != "TABLEDIV") break;
						}
						if (isMatch) {
							next.style.display = "block";
						}
						next = getNext(next);
					}
				}
			}
		}
		else if (elem.className == "ilang") {
			var block = getBlock(elem);
			if (block.tagName == "H1") {
				if (isMultiLang) {
					elem.innerHTML = '&nbsp; [' + curLangs.toString() + ']';
				} else {
					elem.innerHTML = '&nbsp; [' + langName + ']';
				}
			}
		}
	}
	resizeBan();
	
	return;
}


function unfilterLang(){

	var spans = document.all.tags("SPAN");
	for (var i = 0; i < spans.length; i++) {
		var elem = spans[i];
		if (elem.className == "lang") {
			var block = getBlock(elem);
			block.style.display = "block";
			elem.style.display = "inline";

			if (block.tagName == "DT") {
				var next = getNext(block);
				if (next && next.tagName == "DD")
					next.style.display = "block";
			}
			else if (block.tagName == "DIV") {
				block.className = "filtered";
			}
			else if (block.tagName.match(/^H[1-6]$/)) {
				if (topicHeading(block)) {
					var tag = null;
					if (block.children && block.children.length) {
						tag = block.children[block.children.length - 1];
						if (tag && tag.className == "ilang")
							tag.innerHTML = "";
					}
				}
				else {
					var next = getNext(block);
					while (next && !next.tagName.match(/^(H[1-6])$/)) {
						if (next.tagName =="DIV"){
							if (next.className.toUpperCase() != "TABLEDIV") break;
						}
						next.style.display = "block";
						next = getNext(next);
					}
				}
			}
		}
		else if (elem.className == "ilang") {
			elem.innerHTML = "";
		}
	}
	resizeBan();
	
	return;
}

function resetLangBlocks(){
	// Hides all lang blocks to reset display.
	// Call before filterLang(), which selectively enables.

	var spans = document.all.tags("SPAN");
	for (var i = 0; i < spans.length; i++) {
		var elem = spans[i];
		if (elem.className == "lang") {
			var block = getBlock(elem);
			block.style.display = "none";
			elem.style.display = "inline";

			if (block.tagName == "DT") {
				var next = getNext(block);
				if (next && next.tagName == "DD")
					next.style.display = "none";
			}
			else if (block.tagName == "DIV") {
				block.className = "filtered";
			}
			else if (block.tagName.match(/^H[1-6]$/)) {
				if (topicHeading(block)) {
					var tag = null;
					if (block.children && block.children.length) {
						tag = block.children[block.children.length - 1];
						if (tag && tag.className == "ilang")
							tag.innerHTML = "";
					}
				}
				else {
					var next = getNext(block);
					while (next && !next.tagName.match(/^(H[1-6])$/)) {
						if (next.tagName =="DIV"){
							if (next.className.toUpperCase() != "TABLEDIV") break;
						}
						next.style.display = "none";
						next = getNext(next);
					}
				}
			}
		}
		else if (elem.className == "ilang") {
			elem.innerHTML = "";
		}
	}
	resizeBan();
	
	return;
}





function closeMenu(){
	var div = document.all.lang_menu;
	if (div && div.style.visibility != "hidden") {
		var lnk = document.activeElement;
		if (lnk && lnk.tagName == "A")
			lnk.blur();

		div.style.visibility = "hidden";
	}
}


function getNext(elem){
	for (var i = elem.sourceIndex + 1; i < document.all.length; i++) {
		var next = document.all[i];
		if (!elem.contains(next))
			return next;
	}
	return null;
}


function filterMatch(text, name){
	var a = text.split(",");
	for (var iTok = 0; iTok < a.length; iTok++) {
		var m = a[iTok].match(/([A-Za-z].*[A-Za-z+#0-9])/);
		if (m && m[1] == name)
			return true;
	}
	return false;
}


function topicHeading(head){
	try{var iLim = nstext.children.length;
	Section = nstext;}
	catch(e){try{var iLim = scrtext.children.length;
		Section = scrtext;}
		catch(e){var iLim = document.body.children.length;
		Section = document.body;
		}
	}
	var idxLim = head.sourceIndex;
	for (var i = 0; i < iLim; i++) {
		var elem = Section.children[i];
		if (elem.sourceIndex < idxLim) {
			if (elem.tagName.match(/^(P)|(PRE)|([DOU]L)$/))
				return false;
		}
		else
			break;
	}
	return true;
}


// ****************************************************************************
// *                         Clipboard Processing                             *
// ****************************************************************************

function procCodeSelection() {
	
	try {
		var oRange = document.selection.createRange();
		
		if (document.selection.type != "none") {
		
			var sMsg = "Nothing selected.";
			var oParent = oRange.parentElement();
			var sParentName = oParent.tagName;
			var sClassValue = oParent.getAttribute("className", 0);
			
			if ((sParentName != null) && (sClassValue != null)) {
				if ((sParentName.toLowerCase() == "pre") && (sClassValue.toLowerCase() == "code")) {

					var re = /\s*$/gi;
					var r = oRange.text.replace(re, "\r\n");
					
					if (r != null) {
						window.event.returnValue = false;
						window.clipboardData.setData("Text", r);
					}
				}
			}
		}
	} 
	catch(e){}
	
	return;
}

// ****************************************************************************
// *                      Reftips (parameter popups)                          *
// ****************************************************************************

function initReftips(){
	var DLs = document.all.tags("DL");
	var PREs = document.all.tags("PRE");
	if (DLs && PREs) {
		var iDL = 0;
		var iPRE = 0;
		var iSyntax = -1;
		for (var iPRE = 0; iPRE < PREs.length; iPRE++) {
			if (PREs[iPRE].className == "syntax") {
				while (iDL < DLs.length && DLs[iDL].sourceIndex < PREs[iPRE].sourceIndex)
					iDL++;			
				if (iDL < DLs.length) {
					initSyntax(PREs[iPRE], DLs[iDL]);
					iSyntax = iPRE;
				}
				else
					break;
			}
		}

		if (iSyntax >= 0) {
			var last = PREs[iSyntax];
			if (last.parentElement.tagName == "DIV") last = last.parentElement;						
			last.insertAdjacentHTML('afterEnd','<DIV ID=reftip CLASS=reftip STYLE="position:absolute;visibility:hidden;overflow:visible;"></DIV>');
		}
	}
}


function initSyntax(pre, dl){
	var strSyn = pre.outerHTML;
	var ichStart = strSyn.indexOf('>', 0) + 1;
	var terms = dl.children.tags("DT");
	if (terms) {
		for (var iTerm = 0; iTerm < terms.length; iTerm++) {
			if (terms[iTerm].innerHTML.indexOf("<!--join-->")!=-1){
				var word = terms[iTerm].innerText.replace(/\s$/, "");
				var ichMatch = findTerm(strSyn, ichStart, word);
				if (ichMatch < 1){
					word = word.replace(/\s/, "&nbsp;")
					ichMatch = findTerm(strSyn, ichStart, word);
				}
				while (ichMatch > 0) {
					var strTag = '<A HREF="" ONCLICK="showTip(this)" CLASS="synParam">' + word + '</A>';

					strSyn =
						strSyn.slice(0, ichMatch) +
						strTag +
						strSyn.slice(ichMatch + word.length);
					ichMatch = findTerm(strSyn, ichMatch + strTag.length, word);
				}
				
			}
		}
		for (var iTerm = 0; iTerm < terms.length; iTerm++) {
			if (terms[iTerm].innerHTML.indexOf("<!--join-->")==-1){
			var words = terms[iTerm].innerText.replace(/\[.+\]/g, " ").replace(/,/g, " ").split(" ");
				var htm = terms[iTerm].innerHTML;
				for (var iWord = 0; iWord < words.length; iWord++) {
					var word = words[iWord];

					if (word.length > 0 && htm.indexOf(word, 0) < 0)
						word = word.replace(/:.+/, "");

					if (word.length > 0) {
						var ichMatch = findTerm(strSyn, ichStart, word);
						while (ichMatch > 0) {
							if (!isLinkText(strSyn.substring(ichMatch))){
								var strTag = '<A HREF="" ONCLICK="showTip(this)" CLASS="synParam">' + word + '</A>';

								strSyn =
									strSyn.slice(0, ichMatch) +
									strTag +
									strSyn.slice(ichMatch + word.length);

								ichMatch = findTerm(strSyn, ichMatch + strTag.length, word);
							}
							else ichMatch = findTerm(strSyn, ichMatch + word.length, word);
						}
					}
				}
			}
		}
	}

	// Replace the syntax block with our modified version.
	pre.outerHTML = strSyn;
}


function findTerm(strSyn, ichPos, strTerm)
{
	var ichMatch = strSyn.indexOf(strTerm, ichPos);
	while (ichMatch >= 0) {
		var prev = (ichMatch == 0) ? '\0' : strSyn.charAt(ichMatch - 1);
		var next = strSyn.charAt(ichMatch + strTerm.length);
		if (!isalnum(prev) && !isalnum(next) && !isInTag(strSyn, ichMatch)) {
			var ichComment = strSyn.indexOf("/*", ichPos);
			while (ichComment >= 0) {
				if (ichComment > ichMatch) { 
					ichComment = -1;
					break; 
				}
				var ichEnd = strSyn.indexOf("*/", ichComment);
				if (ichEnd < 0 || ichEnd > ichMatch)
					break;
				ichComment = strSyn.indexOf("/*", ichEnd);
			}
			if (ichComment < 0) {
				ichComment = strSyn.indexOf("//", ichPos);
				var newPos = 0;
				if (ichComment >= 0) {
					while (isInTag(strSyn, ichComment)) { //checks to see if the comment is in a tag (and thus part of a URL)
						newPos = ichComment + 1;
						ichComment = strSyn.indexOf("//", newPos);
						if (ichComment < 0) 
							break;
					}
					while (ichComment >= 0) {
						if (ichComment > ichMatch) {
							ichComment = -1;
							break; 
						}
						var ichEnd = strSyn.indexOf("\n", ichComment);
						if (ichEnd < 0 || ichEnd > ichMatch)
							break;
						ichComment = strSyn.indexOf("//", ichEnd);
					}
				}
			}
			if (ichComment < 0)
				break;
		}
		ichMatch = strSyn.indexOf(strTerm, ichMatch + strTerm.length);
	}
	return ichMatch;
}
function isLinkText(strHtml){
	return strHtml.indexOf("<")==strHtml.toLowerCase().indexOf("<\/a>");
}

function isInTag(strHtml, ichPos)
{
	return strHtml.lastIndexOf('<', ichPos) >
		strHtml.lastIndexOf('>', ichPos);
}


function isalnum(ch){
	return ((ch >= 'a' && ch <= 'z') || (ch >= 'A' && ch <= 'Z') || (ch >= '0' && ch <= '9') || (ch == '_') || (ch == '-'));
}


function showTip(link){
	bodyOnClick();
	var tip = document.all.reftip;
	if (!tip || !link)
		return;

	window.event.returnValue = false;
	window.event.cancelBubble = true;

	// Hide the tip if necessary and initialize its size.
	tip.style.visibility = "hidden";
	tip.style.pixelWidth = 260;
	tip.style.pixelHeight = 24;

	// Find the link target.
	var term = null;
	var def = null;
	var DLs = document.all.tags("DL");
	for (var iDL = 0; iDL < DLs.length; iDL++) {
		if (DLs[iDL].sourceIndex > link.sourceIndex) {
			var dl = DLs[iDL];
			var iMax = dl.children.length - 1;
			for (var iElem = 0; iElem < iMax; iElem++) {
				var dt = dl.children[iElem];
				if (dt.tagName == "DT" && dt.style.display != "none") {
					if (findTerm(dt.innerText, 0, link.innerText) >= 0) {
						var dd = dl.children[iElem + 1];
						if (dd.tagName == "DD") {
							term = dt;
							def = dd;
						}
						break;
					}
				}
			}
			break;
		}
	}

	if (def) {

		var popupText = null;

		for (var ddIdx = 0, ddEnd = def.children.length; ddIdx < ddEnd; ++ddIdx) {

			var tmp = def.children[ddIdx].tagName.toLowerCase();
			if (tmp == "span") {
				popupText = def.children[ddIdx].innerHTML;
				break;
			}
		}

		if (popupText == null) {
			popupText = def.innerHTML;
		}

		window.linkElement = link;
		window.linkTarget = term;
		tip.innerHTML = '<DL><DT>' + term.innerHTML + '</DT><DD>' + popupText + '</DD></DL>';
		window.setTimeout("moveTip()", 0);
	}
}


function jumpParam(){
	hideTip();

	window.linkTarget.scrollIntoView();
	document.body.scrollLeft = 0;

	flash(3);
}


function flash(c){
	window.linkTarget.style.background = (c & 1) ? "#CCCCCC" : "";
	if (c)
		window.setTimeout("flash(" + (c-1) + ")", 200);
}


function moveTip(){
	var tip = document.all.reftip;
	var link = window.linkElement;
	if (!tip || !link)
		return; //error

	var w = tip.offsetWidth;
	var h = tip.offsetHeight;

	if (w > tip.style.pixelWidth) {
		tip.style.pixelWidth = w;
		window.setTimeout("moveTip()", 0);
		return;
	}

	var maxw = document.body.clientWidth-20;
	var maxh = document.body.clientHeight - 200;

	if (h > maxh) {
		if (w < maxw) {
			w = w * 3 / 2;
			tip.style.pixelWidth = (w < maxw) ? w : maxw;
			window.setTimeout("moveTip()", 0);
			return;
		}
	}

	var x,y;

	var linkLeft = link.offsetLeft - document.body.scrollLeft;
	var linkRight = linkLeft + link.offsetWidth;

	try{var linkTop = link.offsetTop - nstext.scrollTop + nstext.offsetTop;}
	catch(e){var linkTop = link.offsetTop;}
	var linkBottom = linkTop + link.offsetHeight + 4;

	var cxMin = link.offsetWidth - 24;
	if (cxMin < 16)
		cxMin = 16;

	if ((linkLeft + cxMin + w <= maxw)&&(h+linkTop <= maxh + 150)) {
		x = linkLeft;
		y = linkBottom;
	}
	if ((linkLeft + cxMin + w <= maxw)&&(h+linkTop > maxh + 150)) {
		x = maxw - w;
		if (x > linkRight + 8)
			x = linkRight + 8;
		x = linkLeft;
		y = linkTop-h;
	}
	if ((linkLeft + cxMin + w >= maxw)&&(h+linkTop <= maxh + 150)) {
		x = linkRight - w;
		if (x < 0)
			x = 0;
		y=linkBottom;
	}
	if ((linkLeft + cxMin + w >= maxw)&&(h+linkTop > maxh + 150)) {
		x = linkRight - w;
		if (x < 0)
			x = 0;
		y = linkTop-h;
		if (y<0)
			y = 0;
	}
	link.style.background = "#CCCCCC";
	tip.style.pixelLeft = x + document.body.scrollLeft;
	tip.style.pixelTop = y;
	tip.style.visibility = "visible";
}


function hideTip(){
	if (window.linkElement) {
		window.linkElement.style.background = "";
		window.linkElement = null;
	}

	var tip = document.all.reftip;
	if (tip) {
		tip.style.visibility = "hidden";
		tip.innerHTML = "";
	}
}


function beginsWith(s1, s2){
	// Does s1 begin with s2?
	return s1.toLowerCase().substring(0, s2.length) == s2.toLowerCase();
}


// ****************************************************************************
// *                           See Also popups                                *
// ****************************************************************************

function initSeeAlso(){
	var hdr = document.all.hdr;
	if (!hdr)
		return;

	var divS = new String;
	var divR = new String;
	var heads = document.all.tags("H4");
	if (heads) {
		for (var i = 0; i < heads.length; i++) {
			var head = heads[i];
			var txt = head.innerText;
			if (beginsWith(txt, L_SeeAlso_TEXT) ) {
				divS += head.outerHTML;
				var next = getNext(head);
				while (next && !next.tagName.match(/^(H[1-4])|(DIV)$/)) {
					divS += next.outerHTML;
					next = getNext(next);
				}
				while ((divS.indexOf("<MSHelp:ktable")!=-1)&&(divS.indexOf("<\/MSHelp:ktable>")!=-1)){
					divS=divS.substring(0, divS.indexOf("<MSHelp:ktable")) + divS.substring(divS.indexOf("<\/MSHelp:ktable>")+16);
				}
			}
			else if (beginsWith(txt, L_Requirements_TEXT) || beginsWith(txt, L_QuickInfo_TEXT) ) {
				divR += head.outerHTML;
				var next = getNext(head);
				var isValid = true;
				while (isValid){
					if (next && !next.tagName.match(/^(H[1-4])$/)){
						if (next.tagName == "DIV" && next.outerHTML.indexOf("tablediv")==-1)
								isValid = false;
						if (isValid){
							divR += next.outerHTML;
							next = getNext(next);
						}
					}
					else
						isValid = false;
				}
			}
		}
	}

	var pos = getNext(hdr.parentElement);
	if (pos) {
		if (divR != "") {
			divR = '<DIV ID=rpop CLASS=sapop onkeypress=ieKey>' + divR + '</DIV>';
			var td = hdr.insertCell(0);
			if (td) {
				td.className = "button1";
				td.onclick = showRequirements;
				td.onkeyup = ieKey;
				td.onkeypress = showRequirements;
				td.innerHTML = '<IMG id=button1 SRC="' + baseUrl + 'Requirements1a.gif' + '" ALT="' + L_Requirements_TEXT + '" BORDER=0 TABINDEX=0>';
				if (advanced)
					try{nsbanner.insertAdjacentHTML('afterEnd', divR);}
					catch(e){try{scrbanner.insertAdjacentHTML('afterEnd', divR);}catch(e){}}
				else
					document.body.insertAdjacentHTML('beforeEnd', divR);
			}
		}
		if (divS != "") {
			divS = '<DIV ID=sapop CLASS=sapop onkeypress=ieKey>' + divS + '</DIV>';
			var td = hdr.insertCell(0);
			if (td) {
				td.className = "button1";
				td.onclick = showSeeAlso;
				td.onkeyup = ieKey;
				td.onkeypress = showSeeAlso;
				td.innerHTML = '<IMG id=button1 SRC="' + baseUrl + 'SeeAlso1a.gif' + '" ALT="' + L_SeeAlso_TEXT + '" BORDER=0 TABINDEX=0>';
				if (advanced)
					try{nsbanner.insertAdjacentHTML('afterEnd', divS);}
					catch(e){try{scrbanner.insertAdjacentHTML('afterEnd', divS);}catch(e){}}
				else
					document.body.insertAdjacentHTML('beforeEnd', divS);
			}
		}
	}
}

function resetButtons(){
	//unclick buttons...
	var btns = document.all.button1;
	if (btns) {
		if (btns.src!=null) btns.src=btns.src.replace("c.gif", "a.gif"); //if there is only one button.
		for (var i = 0; i < btns.length; i++){
			btns[i].src = btns[i].src.replace("c.gif", "a.gif");
		}
	}
}

function showSeeAlso(){
	bodyOnClick();
	var btn = window.event.srcElement
	if (btn.id=="button1"){
	btn.src = btn.src.replace("a.gif", "c.gif");}

	window.event.returnValue = false;
	window.event.cancelBubble = true;

	var div = document.all.sapop;
	var lnk = window.event.srcElement;

	if (div && lnk) {
		div.style.pixelTop = lnk.offsetTop + lnk.offsetHeight;
		div.style.visibility = "visible";
	}
}


function showRequirements(){
	bodyOnClick();
	var btn = window.event.srcElement
	if (btn.id=="button1"){
	btn.src = btn.src.replace("a.gif", "c.gif");}

	window.event.returnValue = false;
	window.event.cancelBubble = true;

	var div = document.all.rpop;
	var lnk = window.event.srcElement;

	if (div && lnk) {
		div.style.pixelTop = lnk.offsetTop + lnk.offsetHeight;
		div.style.visibility = "visible";
	}
}


function hideSeeAlso(){
	var div = document.all.sapop;
	if (div)
		div.style.visibility = "hidden";

	var div = document.all.rpop;
	if (div)
		div.style.visibility = "hidden";
}

function loadAll(){
	try {
		scrollPos = allHistory.getAttribute("Scroll");
	}
	catch(e){	}
}

function saveAll(){
	try{
		allHistory.setAttribute("Scroll",nstext.scrollTop);
	}
	catch(e){	}
}

// ****************************************************************************
// *                           Nonscrolling region                            *
// ****************************************************************************

function resizeBan(){
//resizes nonscrolling banner
	if (document.body.clientWidth==0) return;
	var oBanner= document.all.item("nsbanner");
	var oText= document.all.item("nstext");
	if (oText == null) return;
	var oBannerrow1 = document.all.item("bannerrow1");
	var oTitleRow = document.all.item("titlerow");
	if (oBannerrow1 != null){
		var iScrollWidth = bodyID.scrollWidth;
		oBannerrow1.style.marginRight = 0 - iScrollWidth;
	}
	if (oTitleRow != null){
		oTitleRow.style.padding = "0px 10px 0px 22px; ";
	}
	if (oBanner != null){
		document.body.scroll = "no"
		oText.style.overflow= "auto";
 		oBanner.style.width= document.body.offsetWidth - 2;
		oText.style.paddingRight = "20px"; // Width issue code
		oText.style.width= document.body.offsetWidth - 4;
		oText.style.top=0;  
		if (document.body.offsetHeight > oBanner.offsetHeight + 4)
    			oText.style.height= document.body.offsetHeight - (oBanner.offsetHeight + 4) 
		else oText.style.height=0
	}	
	try{nstext.setActive();} //allows scrolling from keyboard as soon as page is loaded. Only works in IE 5.5 and above.
	catch(e){}
	resetButtons();
} 


function set_to_print(){
//breaks out of divs to print

	var i;

	if (window.text)document.all.text.style.height = "auto";
			
	for (i=0; i < document.all.length; i++){
		if (document.all[i].tagName == "BODY") {
			document.all[i].scroll = "yes";
			}
		if (document.all[i].id == "nsbanner") {
			document.all[i].style.margin = "0px 0px 0px 0px";
			document.all[i].style.width = "100%";
			}
		if (document.all[i].id == "nstext") {
			document.all[i].style.overflow = "visible";
			document.all[i].style.top = "5px";
			document.all[i].style.width = "100%";
			document.all[i].style.padding = "0px 10px 0px 30px";
			}
		}
}


function reset_form(){
//returns to the div nonscrolling region after print

	 document.location.reload();
}



