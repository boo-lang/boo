"""
Adds the license header to source files that lack one.

Usage: booi scripts/insert-license.boo [--dry-run] [file-or-directory ...]

A file needs the header if its first line is not "#region license". The text
comes from scripts/license-header.txt, with {{ Year }} replaced by the year
the file first appeared in git, so an old file is not stamped with today's
date. A file git does not know about is new, and gets the current year.
Nothing already carrying a header is touched.

Given files, it stamps those. Given directories, it walks them. Given nothing,
it looks at src and examples, which is what the NAnt target it replaces did. tests is left out on purpose: nearly everything under
it is a test case a few lines long, and a BSD block on each would be absurd.

A leading byte order mark is removed from any file it touches. Files it does
not touch keep theirs.

Generated sources are skipped. astgen and ANTLR write their own headers, and
anything added here would be lost the next time they run.

Run from the root of a clone.
"""

import System
import System.IO
import System.Collections.Generic
import System.Diagnostics
import System.Text

TEMPLATE = "scripts/license-header.txt"
MARKER = "#region license"

def Fail(message as string):
	Console.Error.WriteLine("insert-license: $message")
	Environment.Exit(2)

def IsGenerated(path as string) as bool:
	p = path.Replace("\\", "/")
	return true if "/Generated/" in p or "/Ast/Impl/" in p
	return true if p.EndsWith(".Generated.cs")
	return "/bin/" in p or "/obj/" in p

# The year the file first appeared, so a file written in 2010 is not stamped
# with today's. Anything git does not know about is new.
def OriginYear(path as string) as string:
	info = ProcessStartInfo("git")
	for arg in ("log", "--follow", "--format=%ad", "--date=format:%Y", "--", path):
		info.ArgumentList.Add(arg)
	info.RedirectStandardOutput = true
	info.RedirectStandardError = true
	info.UseShellExecute = false
	years = List[of string]()
	try:
		using proc = Process.Start(info):
			while null != (line = proc.StandardOutput.ReadLine()):
				years.Add(line.Trim()) if line.Trim() != ""
			proc.WaitForExit()
	except:
		return DateTime.Now.Year.ToString()
	return DateTime.Now.Year.ToString() if len(years) == 0
	return years[len(years) - 1]

def NeedsHeader(path as string) as bool:
	first as string = null
	using reader = StreamReader(path):
		first = reader.ReadLine()
	return first is not null and not first.Trim().StartsWith(MARKER)

Fail("$TEMPLATE not found. Run from the root of a clone.") unless File.Exists(TEMPLATE)

dryRun = "--dry-run" in argv
targets = array(a for a in argv if not a.StartsWith("--"))
targets = ("src", "examples") if len(targets) == 0

template = File.ReadAllText(TEMPLATE).TrimEnd() + "\n\n"

def IsSource(path as string) as bool:
	return path.EndsWith(".cs") or path.EndsWith(".boo")

pending = List[of string]()
for target in targets:
	if File.Exists(target):
		# Named outright, so say why nothing happened rather than passing over
		# it the way a directory walk would.
		Fail("$target is not a .cs or .boo file") unless IsSource(target)
		if IsGenerated(target):
			print "  skipped $target, generated"
		elif not NeedsHeader(target):
			print "  skipped $target, already has a header"
		else:
			pending.Add(target.Replace("\\", "/"))
	elif Directory.Exists(target):
		for pattern in ("*.cs", "*.boo"):
			for path in Directory.GetFiles(target, pattern, SearchOption.AllDirectories):
				continue if IsGenerated(path)
				continue unless NeedsHeader(path)
				pending.Add(path.Replace("\\", "/"))
	else:
		Fail("$target is not a file or a directory")
pending.Sort()

if len(pending) == 0:
	print "nothing to do"
	return

for path in pending:
	if dryRun:
		print "  would add $path ($(OriginYear(path)))"
	else:
		year = OriginYear(path)
		header = template.Replace("{{ Year }}", year)
		# ReadAllText drops a leading byte order mark and WriteAllText does not
		# write one back, so the mark goes with the same pass.
		File.WriteAllText(path, header + File.ReadAllText(path))
		print "  added $path ($year)"

print "$(len(pending)) file(s)"
