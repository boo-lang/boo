using System;
using System.Reflection;
using System.Reflection.Metadata;

namespace Boo.Lang.Compiler.TypeSystem.ReflectionMetadata.Resolvers
{
	public class AssemblyReferenceData
	{
		public string Name { get; }
		public Version Version { get; }
		public string CultureName { get; }
		public byte[] PublicKeyToken { get; }
		public string FullName { get; }

		public AssemblyReferenceData(AssemblyReference reference, MetadataReader reader)
		{
			Name = reader.GetString(reference.Name);
			Version = reference.Version;
			CultureName = reader.GetString(reference.Culture);
			PublicKeyToken = reader.GetBlobBytes(reference.PublicKeyOrToken);
			var flags = reference.Flags;
			var aName = new AssemblyName
			{
				Name = Name,
				Version = Version,
				Flags = AssemblyNameFlags.PublicKey & (AssemblyNameFlags)flags,
				CultureInfo = System.Globalization.CultureInfo.GetCultureInfo(CultureName),
				ContentType = (AssemblyContentType)((int)(flags & AssemblyFlags.ContentTypeMask) >> 9)
			};
			if ((flags & AssemblyFlags.PublicKey) == AssemblyFlags.PublicKey)
				aName.SetPublicKey(PublicKeyToken);
			else
				aName.SetPublicKeyToken(PublicKeyToken);
			FullName = aName.FullName;
		}
	}
}
