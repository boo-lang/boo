"""
Usage: booi genchangelog.boo previousVersion nextVersion
"""
import System
import System.Text.RegularExpressions

def IssueLink(match as Match):
	return "<a href=\"http://jira.codehaus.org/browse/${match}\">${match}</a>"

def Item(match as Match):
	return if not match.ToString().Length
	if match.ToString()[0] != char(' '):
		return "</dl><dl><dt>${match}</dt>" if match.Index
		return "<dl><dt>${match}</dt>"
	return "<dd>${match}</dd>"

args = Environment.GetCommandLineArgs()
previous = (args[2] if len(args) > 2 else "")
next = (args[3] if len(args) > 3 else "")

shortlog = shell("git", "shortlog -n ${previous}..${next}")
issue = /BOO-[0-9]+/
item = /.*/

print "<html><head>"
print "<title>BOO: Changelog between ${previous} and ${next}</title>"
print "<style type=\"text/css\">body { font-family: Helvetica, Arial; font-size: 0.9em; } dt { font-weight: bold; } dd { padding-top: 2px; }</style>"
print "</head><body>"
print "<h2><a href=\"http://boo.codehaus.org/\">BOO</a>: Changelog between ${previous} and ${next}</h2>"
shortlog = item.Replace(shortlog, Item)
print issue.Replace(shortlog, IssueLink)
print "</dl></body></html>"

