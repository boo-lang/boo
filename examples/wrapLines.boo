import System.Text.RegularExpressions

def getWordBreaks(s as string):
	return m.Index for m as Match in /\b|$/.Matches(s)
	
def wrapLines(s as string, columns as int):
	
	lines = []
	
	nextBreak = columns	
	lastBreak = 0
	lineStart = 0
	for wb as int in getWordBreaks(s):
		
		if wb > nextBreak:
			line = s[lineStart:lastBreak]
			lines.Add(line.Trim())
			lineStart = lastBreak			
			nextBreak = lastBreak + columns
			
		lastBreak = wb
		
	lines.Add(s[lineStart:].Trim())
		
	return lines	
	
def test(s):
	print(join(wrapLines(s, 12), "*\n*"))
	
test("this is a very long string")
test("It is a long established fact that a reader will be distracted by the readable content of a page when looking at its layout.")
		
