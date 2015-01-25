"""
0.0
1.0
"""
enum E:
  E0
  E1

def ps(s as single):
  print s.ToString('0.0')

ps E.E0 cast single
ps E.E1 cast single
