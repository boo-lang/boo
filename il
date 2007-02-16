#!/bin/sh
mono build/booc.exe -out:build/il.exe "$@" && monodis build/il.exe
