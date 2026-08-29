namespace Boo.Lang.Parser.Tests.Util;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using Boo.Lang.Compiler.Ast;

/// <summary>
/// Renders an AST as deterministic text, for use as a characterization snapshot.
///
/// It walks the backing fields astgen emits rather than public properties, so
/// it records exactly the state the parser sets and nothing computed on top of
/// it. A node type or field added later shows up without anyone remembering to
/// extend this.
///
/// Positions are line and column only, never file names, so the output does
/// not depend on where the repository is checked out.
/// </summary>
public static class AstDump
{
	/// <summary>
	/// Fields on Node that describe context or compiler state rather than what
	/// was parsed. _lexicalInfo is here because it is written on the node's own
	/// header line instead.
	/// </summary>
	private static readonly HashSet<string> IgnoredFields = new HashSet<string>
	{
		"_parent",
		"_annotations",
		"_entity",
		"_lexicalInfo",
		"_isSynthetic",
	};

	private static readonly Dictionary<Type, FieldInfo[]> FieldCache = new Dictionary<Type, FieldInfo[]>();

	public static string Of(Node node)
	{
		var builder = new StringBuilder();
		WriteNode(builder, node, 0);
		return builder.ToString();
	}

	private static void WriteNode(StringBuilder builder, Node node, int depth)
	{
		Indent(builder, depth);
		builder.Append(node.NodeType.ToString());
		WritePosition(builder, node.LexicalInfo);
		builder.Append('\n');

		foreach (var field in FieldsOf(node.GetType()))
			WriteMember(builder, Label(field.Name), field.GetValue(node), depth + 1);
	}

	private static void WriteMember(StringBuilder builder, string name, object value, int depth)
	{
		if (value == null)
			return;

		var child = value as Node;
		if (child != null)
		{
			Indent(builder, depth);
			builder.Append(name).Append(':').Append('\n');
			WriteNode(builder, child, depth + 1);
			return;
		}

		var children = value as IEnumerable;
		if (children != null && !(value is string))
		{
			var nodes = children.Cast<object>().OfType<Node>().ToList();
			if (nodes.Count == 0)
				return;

			Indent(builder, depth);
			builder.Append(name).Append(':').Append('\n');
			foreach (var item in nodes)
				WriteNode(builder, item, depth + 1);
			return;
		}

		var text = ScalarText(value);
		if (text == null)
			return;

		Indent(builder, depth);
		builder.Append(name).Append(" = ").Append(text).Append('\n');
	}

	/// <summary>
	/// Renders a leaf value, or returns null for one that carries no signal.
	/// </summary>
	private static string ScalarText(object value)
	{
		var location = value as SourceLocation;
		if (location != null)
			return location.IsValid ? location.Line + "," + location.Column : null;

		if (value is bool)
			return (bool)value ? "true" : null;

		if (value is string)
			return Quote((string)value);

		if (value is IFormattable)
			return ((IFormattable)value).ToString(null, CultureInfo.InvariantCulture);

		return value.ToString();
	}

	private static void WritePosition(StringBuilder builder, LexicalInfo start)
	{
		if (start == null || !start.IsValid)
			return;

		builder.Append(" (").Append(start.Line).Append(',').Append(start.Column).Append(')');
	}

	private static string Quote(string value)
	{
		return "\"" + value
			.Replace("\\", "\\\\")
			.Replace("\r", "\\r")
			.Replace("\n", "\\n")
			.Replace("\t", "\\t")
			.Replace("\"", "\\\"") + "\"";
	}

	private static void Indent(StringBuilder builder, int depth)
	{
		builder.Append(' ', depth * 2);
	}

	private static string Label(string fieldName) => fieldName.TrimStart('_');

	/// <summary>
	/// The fields of a node type worth rendering, sorted by name so the output
	/// does not depend on the order reflection happens to hand them back.
	/// </summary>
	private static FieldInfo[] FieldsOf(Type type)
	{
		lock (FieldCache)
		{
			FieldInfo[] cached;
			if (FieldCache.TryGetValue(type, out cached))
				return cached;

			var fields = new List<FieldInfo>();
			for (var current = type; current != null && typeof(Node).IsAssignableFrom(current); current = current.BaseType)
				fields.AddRange(current
					.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly)
					.Where(f => !f.IsStatic)
					.Where(f => !IgnoredFields.Contains(f.Name))
					.Where(f => !IsCompilerState(f.FieldType)));

			var result = fields.OrderBy(f => f.Name, StringComparer.Ordinal).ToArray();
			FieldCache.Add(type, result);
			return result;
		}
	}

	private static bool IsCompilerState(Type type)
	{
		return type.Namespace != null
			&& type.Namespace.StartsWith("Boo.Lang.Compiler.TypeSystem", StringComparison.Ordinal);
	}
}
