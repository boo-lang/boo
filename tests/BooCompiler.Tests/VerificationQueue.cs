using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using NUnit.Framework;

namespace BooCompiler.Tests;

/// <summary>
/// Collects generated assemblies and verifies them all in one ilverify run.
/// </summary>
/// <remarks>
/// Verifying per testcase costs a process start each time, which dwarfs the
/// verification itself. ilverify takes many assemblies per invocation.
/// </remarks>
internal static class VerificationQueue
{
	private static readonly ConcurrentQueue<string> Queued = new ConcurrentQueue<string>();

	internal static string Directory
	{
		get { return Path.Combine(Path.GetTempPath(), "boo-tests", "verify"); }
	}

	/// <summary>
	/// Takes a copy of the assembly under a name that identifies the test, so
	/// ilverify's output points back at it.
	/// </summary>
	internal static void Enqueue(string assembly, string testName)
	{
		if (!File.Exists(assembly))
			return;

		System.IO.Directory.CreateDirectory(Directory);

		var name = testName + Path.GetExtension(assembly);
		var queued = Path.Combine(Directory, name);
		try
		{
			File.Copy(assembly, queued, true);
			Queued.Enqueue(queued);
		}
		catch (IOException)
		{
			// A test that could not produce an assembly has nothing to verify.
		}
	}

	/// <summary>Assemblies ilverify checked, and assemblies it did not.</summary>
	internal static int Verified { get; private set; }
	internal static int Skipped { get; private set; }

	internal static string VerifyAll(string referenceDirectory)
	{
		var assemblies = new List<string>();
		string queued;
		while (Queued.TryDequeue(out queued))
			assemblies.Add(queued);

		if (assemblies.Count == 0)
			return null;

		var verifier = FindVerifier();
		if (verifier == null)
		{
			Skipped = assemblies.Count;
			return null;
		}

		Verified = assemblies.Count;

		var startInfo = new ProcessStartInfo
		{
			FileName = verifier,
			CreateNoWindow = true,
			UseShellExecute = false,
			RedirectStandardOutput = true,
			RedirectStandardError = true,
		};

		foreach (var pattern in QueuedPatterns(assemblies))
			startInfo.ArgumentList.Add(pattern);

		foreach (var directory in ReferenceDirectories(referenceDirectory))
		{
			startInfo.ArgumentList.Add("-r");
			startInfo.ArgumentList.Add(Path.Combine(directory, "*.dll"));
		}

		using (var process = Process.Start(startInfo))
		{
			var output = process.StandardOutput.ReadToEnd() + process.StandardError.ReadToEnd();
			process.WaitForExit();
			return process.ExitCode == 0 ? null : Failures(output);
		}
	}

	/// <summary>
	/// The queued assemblies, named by wildcard rather than one by one.
	/// </summary>
	/// <remarks>
	/// Windows caps a command line at 32767 characters, which the thousands
	/// of paths the queue holds go well past. ilverify expands wildcards
	/// itself and the queue is a single directory, so one argument per
	/// extension stands in for the lot. Only extensions actually present are
	/// named: a wildcard matching nothing makes ilverify fail to parse.
	/// </remarks>
	private static IEnumerable<string> QueuedPatterns(IEnumerable<string> assemblies)
	{
		var extensions = new List<string>();
		foreach (var assembly in assemblies)
		{
			var extension = Path.GetExtension(assembly);
			if (!extensions.Contains(extension))
				extensions.Add(extension);
		}

		foreach (var extension in extensions)
			yield return Path.Combine(Directory, "*" + extension);
	}

	private static IEnumerable<string> ReferenceDirectories(string referenceDirectory)
	{
		yield return Directory;
		yield return AppContext.BaseDirectory;

		if (!string.IsNullOrEmpty(referenceDirectory))
			yield return referenceDirectory;

		// Fixtures that copy extra assemblies next to their output leave them in
		// their own directory rather than the queue.
		var root = Path.Combine(Path.GetTempPath(), "boo-tests");
		if (System.IO.Directory.Exists(root))
			foreach (var directory in System.IO.Directory.GetDirectories(root))
				yield return directory;

		// Some testcases compile against an assembly shipped beside them, such
		// as mixedbase.dll under net2/generics.
		foreach (var assembly in System.IO.Directory.GetFiles(
			         BooTestCaseUtil.TestCasesPath, "*.dll", SearchOption.AllDirectories))
			yield return Path.GetDirectoryName(assembly);

		foreach (var directory in RuntimeDirectories())
			yield return directory;
	}

	/// <summary>
	/// The shared framework directories the host is running on.
	/// </summary>
	private static IEnumerable<string> RuntimeDirectories()
	{
		var seen = new List<string>();

		var core = Path.GetDirectoryName(typeof(object).Assembly.Location);
		if (!string.IsNullOrEmpty(core))
		{
			seen.Add(core);
			yield return core;
		}

		var trusted = AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES") as string;
		foreach (var path in (trusted ?? string.Empty).Split(Path.PathSeparator))
		{
			var directory = path.Length == 0 ? null : Path.GetDirectoryName(path);
			if (string.IsNullOrEmpty(directory) || seen.Contains(directory))
				continue;

			seen.Add(directory);
			yield return directory;
		}
	}

	/// <summary>
	/// Keeps the reported IL errors and drops the per-assembly "Verified." noise.
	/// </summary>
	/// <remarks>
	/// ilverify can also fail by throwing, which it does on malformed exception
	/// handling. That says verification did not complete rather than that the
	/// IL is known bad, so it is reported separately.
	/// </remarks>
	private static string Failures(string output)
	{
		var failures = new StringBuilder();
		var crashed = false;

		foreach (var line in output.Split('\n'))
		{
			if (line.Contains("[IL]: Error ["))
				failures.AppendLine(line.Trim());
			else if (line.StartsWith("Error:") || line.Contains("Exception:"))
				crashed = true;
		}

		if (crashed)
			failures.AppendLine("ilverify itself failed on at least one assembly, "
				+ "so verification is incomplete. Run it per assembly to find which.");

		return failures.Length == 0 ? null : failures.ToString();
	}

	private static string FindVerifier()
	{
		var configured = Environment.GetEnvironmentVariable("BOO_ILVERIFY");
		if (!string.IsNullOrEmpty(configured))
			return File.Exists(configured) ? configured : null;

		var name = Environment.OSVersion.Platform == PlatformID.Win32NT ? "ilverify.exe" : "ilverify";
		var home = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

		var candidates = new List<string>();
		if (!string.IsNullOrEmpty(home))
		{
			candidates.Add(Path.Combine(home, ".dotnet", "tools", name));
			candidates.Add(Path.Combine(home, ".local", "share", "dotnet", ".dotnet", "tools", name));
		}
		foreach (var dir in (Environment.GetEnvironmentVariable("PATH") ?? "").Split(Path.PathSeparator))
			if (dir.Length > 0)
				candidates.Add(Path.Combine(dir, name));

		return candidates.Find(File.Exists);
	}
}

/// <summary>
/// Runs the queued verification once, after every fixture has finished.
/// </summary>
[SetUpFixture]
public class VerifyGeneratedAssemblies
{
	[OneTimeSetUp]
	public void SetUp()
	{
		if (Directory.Exists(VerificationQueue.Directory))
			Directory.Delete(VerificationQueue.Directory, true);
	}

	[OneTimeTearDown]
	public void VerifyEverythingCompiled()
	{
		var failures = VerificationQueue.VerifyAll(AppContext.BaseDirectory);

		if (VerificationQueue.Skipped > 0)
		{
			var message = string.Format(
				"ilverify was not found, so the IL of {0} generated assemblies went "
				+ "unchecked. Install it with: dotnet tool install --global dotnet-ilverify",
				VerificationQueue.Skipped);

			if ("1" == Environment.GetEnvironmentVariable("BOO_REQUIRE_ILVERIFY"))
				Assert.Fail(message);

			TestContext.Progress.WriteLine(message);
			return;
		}

		if (failures != null)
			Assert.Fail("ilverify rejected IL from these testcases:" + Environment.NewLine + failures);

		TestContext.Progress.WriteLine(
			"ilverify checked {0} generated assemblies.", VerificationQueue.Verified);
	}
}
