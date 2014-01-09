// version 9324
// links to HxLink.css, DTUE CSS, and DTUE topics scripts

var ieVer = getIEVersion();
var jsPath = scriptPath();

writeCSS(jsPath);

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

function scriptPath() { 
//Determine path to JS-the CSS is in the same directory as the script
	if ( ieVer >= 4 ) {
		var spath = document.scripts[document.scripts.length - 1].src;
		spath = spath.toLowerCase();
		return spath.replace("dtuelink.js", "");
		}
	else {
		var spath = "..\\scripts\\"; //defaults to a scripts folder
		return spath;
	}
}

function writeCSS(spath) {
	var dtueCSS = "";
	var HxLinkPath = "";
	var HxLink = "";
	// Get a CSS name based on the browser.
	if ( ieVer >= 5) {
		dtueCSS = "DTUE.css";
		HxLink = "HxLink.css";
		HxLinkPath = "ms-help://Hx/HxRuntime/";
		document.writeln('<SCRIPT SRC="' + spath + '\dtue_ie5.js"></SCRIPT>');
		document.writeln('<SCRIPT FOR="reftip" EVENT="onclick">window.event.cancelBubble = true;</SCRIPT>');
		document.writeln('<SCRIPT FOR="cmd_lang" EVENT="onclick">langClick(this);</SCRIPT>');
		document.writeln('<SCRIPT FOR="cmd_filter" EVENT=onclick>filterClick(this);</SCRIPT>');
		}
	else {
		if (ieVer >=4 && ieVer <5) {
			document.writeln('<SCRIPT SRC="' + spath + '\dtue_ie4.js"></SCRIPT>');
			dtueCSS = "msdn_ie4.css";
			HxLink = "";
			HxLinkPath = "";
			}
		else {
			if (ieVer < 4) 
				document.writeln('<SCRIPT SRC="' + spath + '\dtue_ie3.js"></SCRIPT>');
				dtueCSS = "msdn_ie3.css";
				HxLink = "";
				HxLinkPath = "";
				//document.write ("<link disabled rel='stylesheet' href='"+ spath + dtueCSS + "'>")
				//document.write ("<style>@import url("+ spath + dtueCSS + ");</style>")
			}
		}
	// Insert CSS calls
	document.writeln('<LINK REL="stylesheet" HREF="' + HxLinkPath + HxLink + '">');
	document.writeln('<LINK REL="stylesheet" HREF="' + spath + dtueCSS + '">');
}
