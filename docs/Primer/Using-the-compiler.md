The Boo Compiler is typically called in this fashion:

`booc <options> <files>`

## Compiler Options

<table><tbody>
<tr>
<th><p> Option </p></th>
<th><p> Description </p></th>
</tr>
<tr>
<td><p> <code>-v</code> </p></td>
<td><p> Verbose </p></td>
</tr>
<tr>
<td><p> <code>-vv</code> </p></td>
<td><p> More Verbose </p></td>
</tr>
<tr>
<td><p> <code>-vvv</code> </p></td>
<td><p> Most Verbose </p></td>
</tr>
<tr>
<td><p> <code>-r:</code><em>&lt;reference_name&gt;</em> </p></td>
<td><p> Add a reference to your project </p></td>
</tr>
<tr>
<td><p> <code>-t:</code><em>&lt;type_name_to_generate&gt;</em> </p></td>
<td><p> Type of file to generate, can be either <code>exe</code> or <code>winexe</code> to make executables (.exe files), or <code>library</code> to make a .dll file </p></td>
</tr>
<tr>
<td><p> <code>-p:</code><em>&lt;pipeline&gt;</em> </p></td>
<td><p> Adds a step <em>&lt;pipeline&gt;</em> to the compile. </p></td>
</tr>
<tr>
<td><p> <code>-c:</code><em>&lt;culture&gt;</em> </p></td>
<td><p> Sets which <em>CultureInfo</em> to use. </p></td>
</tr>
<tr>
<td><p> <code>-o:</code><em>&lt;output_file&gt;</em> </p></td>
<td><p> Sets the name of the output file </p></td>
</tr>
<tr>
<td><p> <code>-srcdir:</code><em>&lt;source_files&gt;</em> </p></td>
<td><p> Specify where to find the source files. </p></td>
</tr>
<tr>
<td><p> <code>-debug</code> </p></td>
<td><p> Adds debug flags to your code. Good for non-production. (On by default) </p></td>
</tr>
<tr>
<td><p> <code>-debug-</code> </p></td>
<td><p> Does not add debug flags to your code. Good for production environment. </p></td>
</tr>
<tr>
<td><p> <code>-debug-steps</code> </p></td>
<td><p> See AST after each compilation step. </p></td>
</tr>
<tr>
<td><p> <code>-resource:</code><em>&lt;resource_file&gt;</em><code>,</code><em>&lt;name&gt;</em> </p></td>
<td><p> Add a resource file. <em>&lt;name&gt;</em> is optional. </p></td>
</tr>
<tr>
<td><p> <code>-embedres:</code><em>&lt;resource_file&gt;</em><code>,</code><em>&lt;name&gt;</em> </p></td>
<td><p> Add an embedded resource file. <em>&lt;name&gt;</em> is optional. </p></td>
</tr>
</tbody></table>


## Using NAnt

When working on a large project with multiple files or libraries, it is a lot easier to use [NAnt](http://nant.sourceforge.net/). It is a free .NET build tool.

To do the same command as above, you would create the following build file:

```xml
<?xml version="1.0" ?>

<project name="Goomba" default="build">
    <target name="build" depends="database" />
    <target name="database">
        <mkdir dir="bin" />
        <booc output="bin/Database.dll" target="library">
            <references basedir="bin">
                <include name="System.Data.dll" />
            </references>
            <sources>
                <include name="Database.boo" />
            </sources>
        </booc>
    </target>
</project>
```

    $ nant
    NAnt 0.85 (Build 0.85.1869.0; rc2; 2/12/2005)
    Copyright (C) 2001-2005 Gerry Shaw
    http://nant.sourceforge.net
    
    Buildfile: file:///path/to/default.build
    Target framework: Microsoft .NET Framework 1.1
    Target(s) specified: build
    
    build:
    
    database:
    
         [booc] Compiling 1 file(s) to /path/to/bin/Database.dll.
    
    BUILD SUCCEEDED
    
    Total time: 0.2 seconds.

And although that was a long and drawnout version of something so simple, it does make things a lot easier when dealing with multiple files. It also helps that if you make a change to your source files, you don't have to type a long `booc` phrase over again. The important part of the build file is the `<booc>` section. It relays commands to the compiler.

There are four attributes available to use in it:

<table><tbody>
<tr>
<th><p> Attribute </p></th>
<th><p> Description </p></th>
</tr>
<tr>
<td><p> <code>target</code> </p></td>
<td><p> Output type, one of <code>library</code>, <code>exe</code>, <code>winexe</code>. Optional. Default: <code>exe</code>. </p></td>
</tr>
<tr>
<td><p> <code>output</code> </p></td>
<td><p> The name of the output assembly. <strong>Required</strong>. </p></td>
</tr>
<tr>
<td><p> <code>pipeline</code> </p></td>
<td><p> AssemblyQualifiedName for the CompilerPipeline type to use. Optional. </p></td>
</tr>
<tr>
<td><p> <code>tracelevel</code> </p></td>
<td><p> Enables compiler tracing, useful for debugging the compiler, one of: Off, Error, Warning, Info, Verbose. Optional. Default: Off. </p></td>
</tr>
</tbody></table>

You are most likely only to use `target` and `output`.

For nested elements, you have 3 possibilities:

<table><tbody>
<tr>
<th><p> Nested Element </p></th>
<th><p> Description </p></th>
</tr>
<tr>
<td><p> <code>&lt;sources&gt;</code> </p></td>
<td><p> Source files. <strong>Required</strong>. </p></td>
</tr>
<tr>
<td><p> <code>&lt;references&gt;</code> </p></td>
<td><p> Assembly references. </p></td>
</tr>
<tr>
<td><p> <code>&lt;resources&gt;</code> </p></td>
<td><p> Embedded resources. </p></td>
</tr>
</tbody></table>

Inside these you are to put <include /> elements, as in the example.

This is merely a brief overview of NAnt, please go to their website <http://nant.sourceforge.net> for more information.

