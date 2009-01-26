"""
Usage: booi genchangelog.boo previousVersion nextVersion
"""
import System
import System.Web from "System.Web.dll"
import System.Collections.Generic
import System.Text.RegularExpressions
import Boo.Lang.PatternMatching


highlights as (string)
contributors = Dictionary[of string,string]()

def GetName(ckey as string):
	match ckey:
		case "avish":
			return "Avishay Lavie"
		case "bamboo":
			return "Rodrigo B. De Oliveira"
		case "cedricv":
			return "Cedric Vivier"
		case "grunwald":
			return "Daniel Grunwald"
		case "neoeinstein":
			return "Marcus Griep"
		otherwise:
			return ckey

def AuthorLink(ckey as string, name as string):
	return "<a name=\"${ckey}\">${name}</a>"

def IssueLink(match as Match):
	return "<a href=\"http://jira.codehaus.org/browse/${match}\">${match}</a>"

def Item(match as Match):
	s = match.ToString()
	return if not s
	if s[0] != char(' '): #contributor
		ckey = s.Substring(0, s.IndexOf(char(' ')))
		name = s.Replace(ckey,GetName(ckey))[0:-1]
		contributors.Add(ckey, name)
		dl = ("</dl>" if match.Index else "")
		return "${dl}<dl><dt>${AuthorLink(ckey, name)}</dt>"
	for h in highlights:
		if s.IndexOf(h) != -1:
			return "<dd class=\"highlight\">${s}</dd>"
	return "<dd>${System.Web.HttpUtility.HtmlEncode(s)}</dd>"

args = Environment.GetCommandLineArgs()
previous = (args[2] if len(args) > 2 else "")
next = (args[3] if len(args) > 3 else "")
highlights = (args[4:] if len(args) > 4 else (,))

shortlog = shell("git", "shortlog ${previous}..${next}")
issue = /BOO-[0-9]+/
item = /.*/

print "<html><head>"
print "<title>BOO: Changelog between ${previous} and ${next}</title>"
print "<style type=\"text/css\">body { font-family: Helvetica, Arial; font-size: 0.9em; width:100%; margin:0;padding:0;} h2 { font-size: 22px; height: 72px; background: #118811; color:white; padding: 4px;} #content { margin: 12px; } dt { font-weight: bold; text-decoration: underline; } dd { padding-top: 2px; } .contributors a { margin-right: 3em;} img {border:0px; position:absolute;} h4 {margin: 0;} h2 span { padding-top:30px;margin-left:78px;} .highlight {color:red; font-weight:bold;}</style>"
print "</head><body>"
print "<h2><a href=\"http://boo.codehaus.org/\" onclick=\"window.open(this.href);return false;\"><img src=\"http://neonux.com/img/boo-logo-64.png\"/></a> <span>Changelog between ${previous} and ${next}</span></h2>"
print "<div id=\"content\">"
print "<h4>Contributors to this release:</h4>"
print "<p class=\"contributors\">"
shortlog = item.Replace(shortlog, Item)
for c in contributors: print "<a href=\"#${c.Key}\">${c.Value}</a>"
print "</p>"
print "<hr/>"
print issue.Replace(shortlog, IssueLink)
print "</dl></div></body></html>"

