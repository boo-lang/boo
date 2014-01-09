
function InitTitle()
{
	if ( document == null ) return;
	if ( document.body == null ) return;

	InsertHeader();
		
	InsertFooter();
}

function InsertHeader()
{
	var html = "<div id=nsbanner><div id=bannerrow1>"
	html += "<table class=bannerparthead cellspacing=0 id=Table1>";
	html += "<tr id=hdr><td class=runninghead>NDoc User's Guide</td>";
	html += "<td class=product>NDoc 1.3<br></td></tr></table></div>";
	html += "<div id=TitleRow><h1 class=dtH1>";
	html += document.title;
	html += "</h1></div></div>";
	html += "<div style='padding-left:3pt;padding-top:3pt;'><font color='Red'>[This is preliminary documentation and subject to change]</font></div>";
	
	document.body.insertAdjacentHTML( "afterBegin", html );
}

function InsertFooter()
{
	var lastChild = document.body.lastChild;
	if ( lastChild == null ) return;

	var subject = "?subject=NDoc User's Guide feedback about page '" + document.title + "'"
	subject = subject.replace("(\s+)","%20");
	
	var html = "<hr/><table border=0 cellSpacing=0 class=footer>";
	html += "<tr vAlign=top><td><p>NDoc development is hosted by <a target=_blank href=http://sourceforge.net>";
	html += "<img src=images/sf.gif alt=SourceForge align=absMiddle border=0></a>";
	html += "<br/><a href=\"mailto:ndoc-helpfeedback@lists.sourceforge.net";
	html += subject;
	html += "\">Send feedback on this topic</A></p></td>";
	html += "<td><A href=http://ndoc.sourceforge.net/ target=_blank><img alt='Visit the NDoc WIKI' align=right src=images/logo.jpg></a></td></tr></table>";	

	lastChild.insertAdjacentHTML( "beforeEnd", html );
}