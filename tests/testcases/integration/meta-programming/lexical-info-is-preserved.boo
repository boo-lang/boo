"""
lexical-info-is-preserved.boo
5
"""
location = [| foo |].LexicalInfo
print System.IO.Path.GetFileName(location.FileName)
print location.Line
