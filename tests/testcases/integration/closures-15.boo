"""
do: 0
do: 1
do: 2
closure: 0
closure: 1
closure: 2
inline: 0
inline: 1
inline: 2
block: 0
block: 1
block: 2
"""
callable Action(item)

def each(items, action as Action):
     for item in items:
          action(item)

a = def (item):
    print("do: ${item}")

b = { item | print("closure: ${item}") }

each(range(3), a)
each(range(3), b)
each(range(3), { item | print("inline: ${item}") })

each(range(3)) do (item):
     print("block: ${item}")


