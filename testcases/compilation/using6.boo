"""
Boo.Tests
System.Drawing
"""
using System
using System.Drawing from System.Drawing as SD
using System.Drawing from Boo.Tests

Console.WriteLine(Point(0, 0).GetType().Assembly.GetName().Name)
Console.Write(SD.Point(0, 0).GetType().Assembly.GetName().Name)
