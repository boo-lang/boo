# FAQ

Frequently asked questions. Have a question? Post it here or to one of our [Mailing Lists](Mailing-Lists).

###License

boo is licensed under a MIT/BSD style license. The current license is always [available here](https://raw.github.com/bamboo/boo/master/license.txt).

###How complete is boo at the present moment? What's its current status?

Boo is already usable for a large variety of tasks but there still are lots of things in our todo lists on [ GitHub](https://github.com/bamboo/boo/issues?state=open) and [CodeHaus](http://jira.codehaus.org/secure/IssueNavigator.jspa?reset=true&pid=10671&statusIds=1).

###Performance: since it is statically typed, can I expect a performance equal or close to c# or vb.net?

Yes.

###How different is it from Python?

See [Gotchas for Python Users](https://github.com/bamboo/boo/wiki/Gotchas-for-Python-Users) for a summary.

###Is it feasable to use boo for building desktop or asp.net applications?

Yes. Boo can already be used to implement WinForms/GTK# applications. Take a look at the extras/boox folder for an example.

On the asp.net front, thanks to Ian it's already possible to directly embed boo code inside asp.net pages, handlers or webservices. **examples/asp.net** should give you an idea of how everything works right now.

###(Sharp|Mono)Develop bindings?

Daniel Grunwald has made great progress on the SharpDevelop front. Recent versions of SharpDevelop include solution creation in Boo.

As for MonoDevelop, it includes a Boo binding, written by Peter Johanson, leveraging the parser code written by Daniel Grunwald for the SharpDevelop binding. It includes Boo project creation/editing/compiling, as well as an interactive shell with Gtk# integration. See the [monodevelop page](http://www.monodevelop.com/) for more information on installing it.

###I see references on the site for .NET 1.1 and .NET 2.0, does Boo support .NET 3.0?

.NET 3.0 is actually just an update to the framework and not to the CLR or any of the "official" languages. As such, it should be supported by any .NET-2.0-supporting language, such as Boo.

Also, Boo release .78 is the last version of Boo that will support .NET 1.1.

###What's a good way to get started with Boo (editors/IDEs)?

Fire up a console and check out booish - a built-in editor to check out the basics. Then grab a copy of Sharpdevelop or monodevelop to dive into developing with Boo.

###What do people use for building 'real' Boo applications?

On Windows, Sharpdevelop is the most robust and stable IDE for developing BOO applications. Linux and Mac users develop with their favorite text editor. The monodevelop team are hard at work developing a more professional development environment that will support Boo along with other .NET languages.

###When will version 1.0 be available?

When Boo is written in Boo it will be dubbed version 1.0.

### Translations
This article is translated to [Serbo-Croatian](http://science.webhostinggeeks.com/faq-pitanje) by [WHG Team](http://webhostinggeeks.com/).

