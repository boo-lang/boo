"""
Bumps the project version.

Usage: booi scripts/bumpversion.boo <minor|patch|X.Y.Z> [--dry-run]

.bumpversion.toml holds current_version and names the files carrying it:

    [bumpversion]
    current_version = "0.9.7"

    [[file]]
    path = "version.txt"
    search = "{current_version}"
    replace = "{new_version}"

search and replace are literal, not patterns, and both default to the
placeholder alone. Only the subset of TOML shown above is understood: tables,
arrays of tables, and single-line basic or literal strings.

Boo stays on 0.x, so there is no major bump: a release is a minor. The binding
version in the AssemblyInfo sources carries a fixed 2. prefix ahead of the
product version, and is stamped from here too.

Run from the root of a clone.
"""

import System
import System.IO
import System.Collections.Generic

class Entry:
	[property(Path)] path as string
	[property(Root)] root as string
	[property(Pattern)] pattern as string
	[property(Search)] search as string
	[property(Replace)] replace as string

CONFIG = ".bumpversion.toml"

def Fail(message as string):
	Console.Error.WriteLine("bumpversion: $message")
	Environment.Exit(2)

# Cuts a trailing comment, leaving any # that sits inside quotes alone.
def StripComment(raw as string) as string:
	quote = char('\0')
	i = 0
	while i < len(raw):
		c = raw[i]
		if quote != char('\0'):
			if c == char('\\') and quote == char('"'):
				i += 1
			elif c == quote:
				quote = char('\0')
		elif c == char('"') or c == char('\''):
			quote = c
		elif c == char('#'):
			return raw[:i]
		i += 1
	return raw

def Unquote(raw as string, where as string) as string:
	value = StripComment(raw).Trim()
	if value.StartsWith("'") and value.EndsWith("'") and len(value) >= 2:
		return value[1:-1]
	unless value.StartsWith('"') and value.EndsWith('"') and len(value) >= 2:
		Fail("$CONFIG: $where is not a quoted string")
	body = value[1:-1]
	out = System.Text.StringBuilder()
	i = 0
	while i < len(body):
		c = body[i]
		if c == char('\\') and i + 1 < len(body):
			i += 1
			n = body[i]
			if n == char('n'): out.Append(char('\n'))
			elif n == char('t'): out.Append(char('\t'))
			elif n == char('"'): out.Append(char('"'))
			elif n == char('\\'): out.Append(char('\\'))
			else: Fail("$CONFIG: unsupported escape \\$n in $where")
		else:
			out.Append(c)
		i += 1
	return out.ToString()

def ReadConfig(path as string):
	version as string = null
	files = List[of Entry]()
	table as string = null
	current as Entry = null
	for raw in File.ReadAllLines(path):
		line = raw.Trim()
		continue if line == "" or line.StartsWith("#")
		if line.StartsWith("[[") and line.EndsWith("]]"):
			table = line[2:-2].Trim()
			Fail("$CONFIG: unknown array of tables [[$table]]") if table != "file"
			current = Entry(Search: "{current_version}", Replace: "{new_version}")
			files.Add(current)
		elif line.StartsWith("[") and line.EndsWith("]"):
			table = line[1:-1].Trim()
			current = null
		else:
			at = line.IndexOf(char('='))
			Fail("$CONFIG: '$line' is not key = value") if at < 0
			key = line[:at].Trim()
			value = Unquote(line[at + 1:], key)
			if table == "bumpversion" and key == "current_version":
				version = value
			elif current is not null:
				if key == "path": current.Path = value
				elif key == "root": current.Root = value
				elif key == "pattern": current.Pattern = value
				elif key == "search": current.Search = value
				elif key == "replace": current.Replace = value
				else: Fail("$CONFIG: unknown key '$key' under [[file]]")
	Fail("$CONFIG has no [bumpversion] current_version") if version is null
	for f as Entry in files:
		if f.Path is null and (f.Root is null or f.Pattern is null):
			Fail("$CONFIG: a [[file]] needs either path, or root and pattern")
	return version, files

def ParseVersion(text as string):
	parts = text.Trim().Split(char('.'))
	Fail("cannot read a X.Y.Z version from '$text'") if len(parts) != 3
	numbers = List[of int]()
	for p in parts:
		try:
			numbers.Add(int.Parse(p))
		except:
			Fail("cannot read a X.Y.Z version from '$text'")
	return numbers.ToArray()

Fail("$CONFIG not found. Run from the root of a clone.") unless File.Exists(CONFIG)

dryRun = "--dry-run" in argv
args = array(a for a in argv if not a.StartsWith("--"))
Fail("usage: bumpversion <minor|patch|X.Y.Z> [--dry-run]") if len(args) != 1

current, files = ReadConfig(CONFIG)
parts = ParseVersion(current)

bump = args[0]
if bump == "major":
	Fail("Boo stays on 0.x. Use minor.")
elif bump == "minor":
	next = "$(parts[0]).$(parts[1] + 1).0"
elif bump == "patch":
	next = "$(parts[0]).$(parts[1]).$(parts[2] + 1)"
else:
	ParseVersion(bump)
	next = bump.Trim()

Fail("already at $next") if next == current

print "$current -> $next"

# Staged per path so nothing is written until every file has matched, and so
# two entries touching one file both survive.
staged = Dictionary[of string, string]()
order = List[of string]()
failed = false

for entry as Entry in files:
	search = entry.Search.Replace("{current_version}", current).Replace("{new_version}", next)
	replace = entry.Replace.Replace("{current_version}", current).Replace("{new_version}", next)

	paths = List[of string]()
	if entry.Path is not null:
		unless File.Exists(entry.Path):
			Console.Error.WriteLine("  missing   ${entry.Path}")
			failed = true
			continue
		paths.Add(entry.Path)
	else:
		unless Directory.Exists(entry.Root):
			Console.Error.WriteLine("  missing   ${entry.Root}/")
			failed = true
			continue
		found = Directory.GetFiles(entry.Root, entry.Pattern, SearchOption.AllDirectories)
		if len(found) == 0:
			Console.Error.WriteLine("  no files  ${entry.Root}/**/${entry.Pattern}")
			failed = true
			continue
		for f in found:
			paths.Add(f.Replace("\\", "/"))
		paths.Sort()

	for path in paths:
		unless staged.ContainsKey(path):
			staged[path] = File.ReadAllText(path)
			order.Add(path)
		text = staged[path]
		unless text.Contains(search):
			Console.Error.WriteLine("  no match  $path")
			failed = true
			continue
		staged[path] = text.Replace(search, replace)

Fail("nothing was written") if failed

for path in order:
	if dryRun:
		print "  would write $path"
	else:
		File.WriteAllText(path, staged[path])
		print "  wrote     $path"

unless dryRun:
	text = File.ReadAllText(CONFIG)
	File.WriteAllText(CONFIG, text.Replace(
		'current_version = "' + current + '"', 'current_version = "' + next + '"'))
	print "  wrote     $CONFIG"
