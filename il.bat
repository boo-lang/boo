@echo off
build\booc.exe -out:build\il.exe %1 %2 %3 %4 %5 %6 %7 %8 %9
ildasm /text build\il.exe
