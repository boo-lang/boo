#!/bin/sh
for f in antlr/*.hpp src/*.cpp; do
	cat "$f" | tr -d "\r" > $$
	cmp -s $$ "$f" || (p4 edit "$f" && mv $$ "$f" && echo "$f Fixed")
done
