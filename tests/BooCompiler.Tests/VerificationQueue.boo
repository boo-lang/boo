namespace BooCompiler.Tests

import System
import System.Collections.Concurrent
import System.Collections.Generic
import System.Diagnostics
import System.IO
import System.Text
import NUnit.Framework

internal static class VerificationQueue:
	"""
	Collects generated assemblies and verifies them all in one ilverify run.

	Verifying per testcase costs a process start each time, which dwarfs the
	verification itself. ilverify takes many assemblies per invocation.
	"""

	private final Queued = ConcurrentQueue[of string]()

	internal Directory as string:
		get: return Path.Combine(Path.GetTempPath(), "boo-tests", "verify")

	internal def Enqueue(assembly as string, testName as string):
		"""
		Takes a copy of the assembly under a name that identifies the test, so
		ilverify's output points back at it.
		"""
		return unless File.Exists(assembly)

		System.IO.Directory.CreateDirectory(Directory)

		name = testName + Path.GetExtension(assembly)
		queued = Path.Combine(Directory, name)
		try:
			File.Copy(assembly, queued, true)
			Queued.Enqueue(queued)
		except as IOException:
			# A test that could not produce an assembly has nothing to verify.
			pass

	# Assemblies ilverify checked, and assemblies it did not.
	private _verified as int
	private _skipped as int

	internal Verified as int:
		get: return _verified

	internal Skipped as int:
		get: return _skipped

	internal def VerifyAll(referenceDirectory as string) as string:
		assemblies = List[of string]()
		queued as string
		while Queued.TryDequeue(queued):
			assemblies.Add(queued)

		return null if assemblies.Count == 0

		verifier = FindVerifier()
		if verifier is null:
			_skipped = assemblies.Count
			return null

		_verified = assemblies.Count

		startInfo = ProcessStartInfo(
			FileName: verifier,
			CreateNoWindow: true,
			UseShellExecute: false,
			RedirectStandardOutput: true,
			RedirectStandardError: true)

		for pattern in QueuedPatterns(assemblies):
			startInfo.ArgumentList.Add(pattern)

		for directory in ReferenceDirectories(referenceDirectory):
			startInfo.ArgumentList.Add("-r")
			startInfo.ArgumentList.Add(Path.Combine(directory, "*.dll"))

		# ilverify does not model ref safety for byreflike types, so it rejects
		# any method handing back a Span whatever compiler emitted it.
		startInfo.ArgumentList.Add("-g")
		startInfo.ArgumentList.Add("ReturnPtrToStack")

		using process = Process.Start(startInfo):
			output = process.StandardOutput.ReadToEnd() + process.StandardError.ReadToEnd()
			process.WaitForExit()
			return null if process.ExitCode == 0
			return Failures(output)

	private def QueuedPatterns(assemblies as IEnumerable[of string]) as IEnumerable[of string]:
		"""
		The queued assemblies, named by wildcard rather than one by one.

		Windows caps a command line at 32767 characters, which the thousands
		of paths the queue holds go well past. ilverify expands wildcards
		itself and the queue is a single directory, so one argument per
		extension stands in for the lot. Only extensions actually present are
		named: a wildcard matching nothing makes ilverify fail to parse.
		"""
		extensions = List[of string]()
		for assembly in assemblies:
			extension = Path.GetExtension(assembly)
			extensions.Add(extension) unless extensions.Contains(extension)

		for extension in extensions:
			yield Path.Combine(Directory, "*" + extension)

	private def ReferenceDirectories(referenceDirectory as string) as IEnumerable[of string]:
		yield Directory
		yield AppContext.BaseDirectory

		yield referenceDirectory unless string.IsNullOrEmpty(referenceDirectory)

		# Fixtures that copy extra assemblies next to their output leave them in
		# their own directory rather than the queue.
		root = Path.Combine(Path.GetTempPath(), "boo-tests")
		if System.IO.Directory.Exists(root):
			for directory in System.IO.Directory.GetDirectories(root):
				yield directory

		# Some testcases compile against an assembly shipped beside them, such
		# as mixedbase.dll under net2/generics.
		for assembly in System.IO.Directory.GetFiles(
				BooTestCaseUtil.TestCasesPath, "*.dll", SearchOption.AllDirectories):
			yield Path.GetDirectoryName(assembly)

		for directory in RuntimeDirectories():
			yield directory

	private def RuntimeDirectories() as IEnumerable[of string]:
		"""The shared framework directories the host is running on."""
		seen = List[of string]()

		core = Path.GetDirectoryName(typeof(object).Assembly.Location)
		if not string.IsNullOrEmpty(core):
			seen.Add(core)
			yield core

		trusted = AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES") as string
		trusted = string.Empty if trusted is null
		for path in trusted.Split(Path.PathSeparator):
			directory = (null if path.Length == 0 else Path.GetDirectoryName(path))
			continue if string.IsNullOrEmpty(directory) or seen.Contains(directory)

			seen.Add(directory)
			yield directory

	private def Failures(output as string) as string:
		"""
		Keeps the reported IL errors and drops the per-assembly "Verified." noise.

		ilverify can also fail by throwing, which it does on malformed exception
		handling. That says verification did not complete rather than that the
		IL is known bad, so it is reported separately.
		"""
		failures = StringBuilder()
		crashed = false

		for line in output.Split(char('\n')):
			if line.Contains("[IL]: Error ["):
				failures.AppendLine(line.Trim())
			elif line.StartsWith("Error:") or line.Contains("Exception:"):
				crashed = true

		if crashed:
			failures.AppendLine("ilverify itself failed on at least one assembly, "
				+ "so verification is incomplete. Run it per assembly to find which.")

		return null if failures.Length == 0
		return failures.ToString()

	private def FindVerifier() as string:
		configured = Environment.GetEnvironmentVariable("BOO_ILVERIFY")
		if not string.IsNullOrEmpty(configured):
			return (configured if File.Exists(configured) else null)

		name = ("ilverify.exe" if Environment.OSVersion.Platform == PlatformID.Win32NT else "ilverify")
		home = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)

		candidates = List[of string]()
		if not string.IsNullOrEmpty(home):
			candidates.Add(Path.Combine(home, ".dotnet", "tools", name))
			candidates.Add(Path.Combine(home, ".local", "share", "dotnet", ".dotnet", "tools", name))
		pathVariable = Environment.GetEnvironmentVariable("PATH")
		pathVariable = "" if pathVariable is null
		for dir in pathVariable.Split(Path.PathSeparator):
			candidates.Add(Path.Combine(dir, name)) if dir.Length > 0

		return candidates.Find(File.Exists)

[SetUpFixture]
class VerifyGeneratedAssemblies:
	"""Runs the queued verification once, after every fixture has finished."""

	[OneTimeSetUp]
	def SetUp():
		if Directory.Exists(VerificationQueue.Directory):
			Directory.Delete(VerificationQueue.Directory, true)

	[OneTimeTearDown]
	def VerifyEverythingCompiled():
		failures = VerificationQueue.VerifyAll(AppContext.BaseDirectory)

		if VerificationQueue.Skipped > 0:
			message = string.Format(
				"ilverify was not found, so the IL of {0} generated assemblies went "
				+ "unchecked. Install it with: dotnet tool install --global dotnet-ilverify",
				VerificationQueue.Skipped)

			if "1" == Environment.GetEnvironmentVariable("BOO_REQUIRE_ILVERIFY"):
				Assert.Fail(message)

			TestContext.Progress.WriteLine(message)
			return

		if failures is not null:
			Assert.Fail("ilverify rejected IL from these testcases:" + Environment.NewLine + failures)

		TestContext.Progress.WriteLine(
			"ilverify checked {0} generated assemblies.", VerificationQueue.Verified)
