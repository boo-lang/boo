@echo off
build\booc.exe /out:build\il.exe %1
ildasm /text build\il.exe
