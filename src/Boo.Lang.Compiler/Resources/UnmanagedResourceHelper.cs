// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.
// See License.txt in the project root for license information.
//
// Adapted for Boo compiler from Roslyn codebase by Mason Wheeler

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using BYTE = System.Byte;
using DWORD = System.UInt32;
using WCHAR = System.Char;
using WORD = System.UInt16;

namespace Boo.Lang.Compiler.Resources
{
    internal sealed class ResourceException : Exception
    {
        internal ResourceException(string name, Exception inner = null)
            : base(name, inner)
        {
        }
    }

    internal class Win32Resource
    {
        private readonly byte[] _data;
        private readonly DWORD _codePage;
        private readonly DWORD _languageId;
        private readonly int _id;
        private readonly string _name;
        private readonly int _typeId;
        private readonly string _typeName;

        internal Win32Resource(
            byte[] data,
            DWORD codePage,
            DWORD languageId,
            int id,
            string name,
            int typeId,
            string typeName)
        {
            _data = data;
            _codePage = codePage;
            _languageId = languageId;
            _id = id;
            _name = name;
            _typeId = typeId;
            _typeName = typeName;
        }

        public string TypeName => _typeName;

        public int TypeId => _typeId;

        public string Name => _name;

        public int Id => _id;

        public DWORD LanguageId => _languageId;

        public DWORD CodePage => _codePage;

        public IEnumerable<byte> Data => _data;
    }

    internal static class VersionHelper
    {
        internal readonly static Version NullVersion = new Version(0, 0, 0, 0);

        /// <summary>
        /// Parses a version string of the form "major [ '.' minor [ '.' build [ '.' revision ] ] ]".
        /// </summary>
        /// <param name="s">The version string to parse.</param>
        /// <param name="version">If parsing succeeds, the parsed version. Otherwise a version that represents as much of the input as could be parsed successfully.</param>
        /// <returns>True when parsing succeeds completely (i.e. every character in the string was consumed), false otherwise.</returns>
        internal static bool TryParse(string s, out Version version)
        {
            return TryParse(s, allowWildcard: false, maxValue: ushort.MaxValue, allowPartialParse: true, version: out version);
        }

        /// <summary>
        /// Parses a version string of the form "major [ '.' minor [ '.' ( '*' | ( build [ '.' ( '*' | revision ) ] ) ) ] ]"
        /// as accepted by System.Reflection.AssemblyVersionAttribute.
        /// </summary>
        /// <param name="s">The version string to parse.</param>
        /// <param name="allowWildcard">Indicates whether or not a wildcard is accepted as the terminal component.</param>
        /// <param name="version">
        /// If parsing succeeded, the parsed version. Otherwise a version instance with all parts set to zero.
        /// If <paramref name="s"/> contains * the version build and/or revision numbers are set to <see cref="ushort.MaxValue"/>.
        /// </param>
        /// <returns>True when parsing succeeds completely (i.e. every character in the string was consumed), false otherwise.</returns>
        internal static bool TryParseAssemblyVersion(string s, bool allowWildcard, out Version version)
        {
            return TryParse(s, allowWildcard: allowWildcard, maxValue: ushort.MaxValue - 1, allowPartialParse: false, version: out version);
        }

        /// <summary>
        /// Parses a version string of the form "major [ '.' minor [ '.' ( '*' | ( build [ '.' ( '*' | revision ) ] ) ) ] ]"
        /// as accepted by System.Reflection.AssemblyVersionAttribute.
        /// </summary>
        /// <param name="s">The version string to parse.</param>
        /// <param name="allowWildcard">Indicates whether or not we're parsing an assembly version string. If so, wildcards are accepted and each component must be less than 65535.</param>
        /// <param name="maxValue">The maximum value that a version component may have.</param>
        /// <param name="allowPartialParse">Allow the parsing of version elements where invalid characters exist. e.g. 1.2.2a.1</param>
        /// <param name="version">
        /// If parsing succeeded, the parsed version. When <paramref name="allowPartialParse"/> is true a version with values up to the first invalid character set. Otherwise a version with all parts set to zero.
        /// If <paramref name="s"/> contains * and wildcard is allowed the version build and/or revision numbers are set to <see cref="ushort.MaxValue"/>.
        /// </param>
        /// <returns>True when parsing succeeds completely (i.e. every character in the string was consumed), false otherwise.</returns>
        private static bool TryParse(string s, bool allowWildcard, ushort maxValue, bool allowPartialParse, out Version version)
        {
            Debug.Assert(!allowWildcard || maxValue < ushort.MaxValue);

            if (string.IsNullOrWhiteSpace(s))
            {
                version = NullVersion;
                return false;
            }

            string[] elements = s.Split('.');

            // If the wildcard is being used, the first two elements must be specified explicitly, and
            // the last must be a exactly single asterisk without whitespace.
            bool hasWildcard = allowWildcard && elements[elements.Length - 1] == "*";

            if ((hasWildcard && elements.Length < 3) || elements.Length > 4)
            {
                version = NullVersion;
                return false;
            }

            ushort[] values = new ushort[4];
            int lastExplicitValue = hasWildcard ? elements.Length - 1 : elements.Length;
            bool parseError = false;
            for (int i = 0; i < lastExplicitValue; i++)
            {

                if (!ushort.TryParse(elements[i], NumberStyles.None, CultureInfo.InvariantCulture, out values[i]) || values[i] > maxValue)
                {
                    if (!allowPartialParse)
                    {
                        version = NullVersion;
                        return false;
                    }

                    parseError = true;

                    if (string.IsNullOrWhiteSpace(elements[i]))
                    {
                        values[i] = 0;
                        break;
                    }


                    if (values[i] > maxValue)
                    {
                        //The only way this can happen is if the value was 65536
                        //The old compiler would continue parsing from here
                        values[i] = 0;
                        continue;
                    }

                    bool invalidFormat = false;
                    System.Numerics.BigInteger number = 0;

                    //There could be an invalid character in the input so check for the presence of one and
                    //parse up to that point. examples of invalid characters are alphas and punctuation
                    for (var idx = 0; idx < elements[i].Length; idx++)
                    {
                        if (!char.IsDigit(elements[i][idx]))
                        {
                            invalidFormat = true;

                            TryGetValue(elements[i].Substring(0, idx), out values[i]);
                            break;
                        }
                    }

                    if (!invalidFormat)
                    {
                        //if we made it here then there weren't any alpha or punctuation chars in the input so the
                        //element is either greater than ushort.MaxValue or possibly a fullwidth unicode digit.
                        if (TryGetValue(elements[i], out values[i]))
                        {
                            //For this scenario the old compiler would continue processing the remaining version elements
                            //so continue processing
                            continue;
                        }
                    }

                    //Don't process any more of the version elements
                    break;
                }
            }




            if (hasWildcard)
            {
                for (int i = lastExplicitValue; i < values.Length; i++)
                {
                    values[i] = ushort.MaxValue;
                }
            }

            version = new Version(values[0], values[1], values[2], values[3]);
            return !parseError;
        }

        private static bool TryGetValue(string s, out ushort value)
        {
            System.Numerics.BigInteger number;
            if (System.Numerics.BigInteger.TryParse(s, NumberStyles.None, CultureInfo.InvariantCulture, out number))
            {
                //The old compiler would take the 16 least significant bits and use their value as the output
                //so we'll do that too.
                value = (ushort)(number % 65536);
                return true;
            }

            //One case that will cause us to end up here is when the input is a Fullwidth unicode digit
            //so we'll always return zero
            value = 0;
            return false;
        }

        /// <summary>
        /// If build and/or revision numbers are 65535 they are replaced with time-based values.
        /// </summary>
        public static Version GenerateVersionFromPatternAndCurrentTime(DateTime time, Version pattern)
        {
            if (pattern == null || pattern.Revision != ushort.MaxValue)
            {
                return pattern;
            }

            // MSDN doc on the attribute: 
            // "The default build number increments daily. The default revision number is the number of seconds since midnight local time 
            // (without taking into account time zone adjustments for daylight saving time), divided by 2."
            if (time == default(DateTime))
            {
                time = DateTime.Now;
            }

            int revision = (int)time.TimeOfDay.TotalSeconds / 2;

            // 24 * 60 * 60 / 2 = 43200 < 65535
            Debug.Assert(revision < ushort.MaxValue);

            if (pattern.Build == ushort.MaxValue)
            {
                TimeSpan days = time.Date - new DateTime(2000, 1, 1);
                int build = Math.Min(ushort.MaxValue, (int)days.TotalDays);

                return new Version(pattern.Major, pattern.Minor, (ushort)build, (ushort)revision);
            }
            else
            {
                return new Version(pattern.Major, pattern.Minor, pattern.Build, (ushort)revision);
            }
        }
    }
    internal class RESOURCE
    {
        internal RESOURCE_STRING pstringType;
        internal RESOURCE_STRING pstringName;

        internal DWORD DataSize;               // size of data without header
        internal DWORD HeaderSize;     // Length of the header
        // [Ordinal or Name TYPE]
        // [Ordinal or Name NAME]
        internal DWORD DataVersion;    // version of data struct
        internal WORD MemoryFlags;    // state of the resource
        internal WORD LanguageId;     // Unicode support for NLS
        internal DWORD Version;        // Version of the resource data
        internal DWORD Characteristics;        // Characteristics of the data
        internal byte[] data;       //data
    };

    internal class RESOURCE_STRING
    {
        internal WORD Ordinal;
        internal string theString;
    };

    internal static class UnamangedResourceHelper
    {
        private struct ICONDIRENTRY
        {
            internal BYTE bWidth;
            internal BYTE bHeight;
            internal BYTE bColorCount;
            internal BYTE bReserved;
            internal WORD wPlanes;
            internal WORD wBitCount;
            internal DWORD dwBytesInRes;
            internal DWORD dwImageOffset;
        };

        internal static void AppendIconToResourceStream(Stream resStream, Stream iconStream)
        {
            const string IconStreamUnexpectedFormat = "Icon stream is not in the expected format.";

            var iconReader = new BinaryReader(iconStream);

            //read magic reserved WORD
            var reserved = iconReader.ReadUInt16();
            if (reserved != 0)
                throw new ResourceException(IconStreamUnexpectedFormat);

            var type = iconReader.ReadUInt16();
            if (type != 1)
                throw new ResourceException(IconStreamUnexpectedFormat);

            var count = iconReader.ReadUInt16();
            if (count == 0)
                throw new ResourceException(IconStreamUnexpectedFormat);

            var iconDirEntries = new ICONDIRENTRY[count];
            for (ushort i = 0; i < count; i++)
            {
                // Read the Icon header
                iconDirEntries[i].bWidth = iconReader.ReadByte();
                iconDirEntries[i].bHeight = iconReader.ReadByte();
                iconDirEntries[i].bColorCount = iconReader.ReadByte();
                iconDirEntries[i].bReserved = iconReader.ReadByte();
                iconDirEntries[i].wPlanes = iconReader.ReadUInt16();
                iconDirEntries[i].wBitCount = iconReader.ReadUInt16();
                iconDirEntries[i].dwBytesInRes = iconReader.ReadUInt32();
                iconDirEntries[i].dwImageOffset = iconReader.ReadUInt32();
            }

            // Because Icon files don't seem to record the actual w and BitCount in
            // the ICONDIRENTRY, get the info from the BITMAPINFOHEADER at the beginning
            // of the data here:
            //EDMAURER: PNG compressed icons must be treated differently. Do what has always
            //been done for uncompressed icons. Assume modern, compressed icons set the 
            //ICONDIRENTRY fields correctly.
            //if (*(DWORD*)icoBuffer == sizeof(BITMAPINFOHEADER))
            //{
            //    grp[i].Planes = ((BITMAPINFOHEADER*)icoBuffer)->biPlanes;
            //    grp[i].BitCount = ((BITMAPINFOHEADER*)icoBuffer)->biBitCount;
            //}

            for (ushort i = 0; i < count; i++)
            {
                iconStream.Position = iconDirEntries[i].dwImageOffset;
                if (iconReader.ReadUInt32() == 40)
                {
                    iconStream.Position += 8;
                    iconDirEntries[i].wPlanes = iconReader.ReadUInt16();
                    iconDirEntries[i].wBitCount = iconReader.ReadUInt16();
                }
            }

            //read everything and no exceptions. time to write.
            var resWriter = new BinaryWriter(resStream);

            //write all of the icon images as individual resources, then follow up with
            //a resource that groups them.
            const WORD RT_ICON = 3;

            for (ushort i = 0; i < count; i++)
            {
                /* write resource header.
                struct RESOURCEHEADER
                {
                    DWORD DataSize;
                    DWORD HeaderSize;
                    WORD Magic1;
                    WORD Type;
                    WORD Magic2;
                    WORD Name;
                    DWORD DataVersion;
                    WORD MemoryFlags;
                    WORD LanguageId;
                    DWORD Version;
                    DWORD Characteristics;
                };
                */

                resStream.Position = (resStream.Position + 3) & ~3; //headers begin on 4-byte boundaries.
                resWriter.Write((DWORD)iconDirEntries[i].dwBytesInRes);
                resWriter.Write((DWORD)0x00000020);
                resWriter.Write((WORD)0xFFFF);
                resWriter.Write((WORD)RT_ICON);
                resWriter.Write((WORD)0xFFFF);
                resWriter.Write((WORD)(i + 1));       //EDMAURER this is not general. Implies you can only append one icon to the resources.
                                                      //This icon ID would seem to be global among all of the icons not just this group.
                                                      //Zero appears to not be an acceptable ID. Note that this ID is referred to below.
                resWriter.Write((DWORD)0x00000000);
                resWriter.Write((WORD)0x1010);
                resWriter.Write((WORD)0x0000);
                resWriter.Write((DWORD)0x00000000);
                resWriter.Write((DWORD)0x00000000);

                //write the data.
                iconStream.Position = iconDirEntries[i].dwImageOffset;
                resWriter.Write(iconReader.ReadBytes(checked((int)iconDirEntries[i].dwBytesInRes)));
            }

            /*
            
            struct ICONDIR
            {
                WORD           idReserved;   // Reserved (must be 0)
                WORD           idType;       // Resource Type (1 for icons)
                WORD           idCount;      // How many images?
                ICONDIRENTRY   idEntries[1]; // An entry for each image (idCount of 'em)
            }/
             
            struct ICONRESDIR
            {
                BYTE Width;        // = ICONDIRENTRY.bWidth;
                BYTE Height;       // = ICONDIRENTRY.bHeight;
                BYTE ColorCount;   // = ICONDIRENTRY.bColorCount;
                BYTE reserved;     // = ICONDIRENTRY.bReserved;
                WORD Planes;       // = ICONDIRENTRY.wPlanes;
                WORD BitCount;     // = ICONDIRENTRY.wBitCount;
                DWORD BytesInRes;   // = ICONDIRENTRY.dwBytesInRes;
                WORD IconId;       // = RESOURCEHEADER.Name
            };
            */

            const WORD RT_GROUP_ICON = RT_ICON + 11;

            resStream.Position = (resStream.Position + 3) & ~3; //align 4-byte boundary
            //write the icon group. first a RESOURCEHEADER. the data is the ICONDIR
            resWriter.Write((DWORD)(3 * sizeof(WORD) + count * /*sizeof(ICONRESDIR)*/ 14));
            resWriter.Write((DWORD)0x00000020);
            resWriter.Write((WORD)0xFFFF);
            resWriter.Write((WORD)RT_GROUP_ICON);
            resWriter.Write((WORD)0xFFFF);
            resWriter.Write((WORD)0x7F00);  //IDI_APPLICATION
            resWriter.Write((DWORD)0x00000000);
            resWriter.Write((WORD)0x1030);
            resWriter.Write((WORD)0x0000);
            resWriter.Write((DWORD)0x00000000);
            resWriter.Write((DWORD)0x00000000);

            //the ICONDIR
            resWriter.Write((WORD)0x0000);
            resWriter.Write((WORD)0x0001);
            resWriter.Write((WORD)count);

            for (ushort i = 0; i < count; i++)
            {
                resWriter.Write((BYTE)iconDirEntries[i].bWidth);
                resWriter.Write((BYTE)iconDirEntries[i].bHeight);
                resWriter.Write((BYTE)iconDirEntries[i].bColorCount);
                resWriter.Write((BYTE)iconDirEntries[i].bReserved);
                resWriter.Write((WORD)iconDirEntries[i].wPlanes);
                resWriter.Write((WORD)iconDirEntries[i].wBitCount);
                resWriter.Write((DWORD)iconDirEntries[i].dwBytesInRes);
                resWriter.Write((WORD)(i + 1));   //ID
            }
        }

        /*
         * Dev10 alink had the following fallback behavior.
                private uint[] FileVersion
                {
                    get
                    {
                        if (fileVersionContents != null)
                            return fileVersionContents;
                        else
                        {
                            Debug.Assert(assemblyVersionContents != null);
                            return assemblyVersionContents;
                        }
                    }
                }

                private uint[] ProductVersion
                {
                    get
                    {
                        if (productVersionContents != null)
                            return productVersionContents;
                        else
                            return this.FileVersion;
                    }
                }
                */

        internal static void AppendVersionToResourceStream(Stream resStream, bool isDll,
            string fileVersion, //should be [major.minor.build.rev] but doesn't have to be
            string originalFileName,
            string internalName,
            string productVersion,  //4 ints
            Version assemblyVersion, //individual values must be smaller than 65535
            string fileDescription = " ",   //the old compiler put blank here if nothing was user-supplied
            string legalCopyright = " ",    //the old compiler put blank here if nothing was user-supplied
            string legalTrademarks = null,
            string productName = null,
            string comments = null,
            string companyName = null)
        {
            var resWriter = new BinaryWriter(resStream, Encoding.Unicode);
            resStream.Position = (resStream.Position + 3) & ~3;

            const DWORD RT_VERSION = 16;

            var ver = new VersionResourceSerializer(isDll,
                comments,
                companyName,
                fileDescription,
                fileVersion,
                internalName,
                legalCopyright,
                legalTrademarks,
                originalFileName,
                productName,
                productVersion,
                assemblyVersion);

            var startPos = resStream.Position;
            var dataSize = ver.GetDataSize();
            const int headerSize = 0x20;

            resWriter.Write((DWORD)dataSize);    //data size
            resWriter.Write((DWORD)headerSize);                 //header size
            resWriter.Write((WORD)0xFFFF);                      //identifies type as ordinal.
            resWriter.Write((WORD)RT_VERSION);                 //type
            resWriter.Write((WORD)0xFFFF);                      //identifies name as ordinal.
            resWriter.Write((WORD)0x0001);                      //only ever 1 ver resource (what Dev10 does)
            resWriter.Write((DWORD)0x00000000);                 //data version
            resWriter.Write((WORD)0x0030);                      //memory flags (this is what the Dev10 compiler uses)
            resWriter.Write((WORD)0x0000);                      //languageId
            resWriter.Write((DWORD)0x00000000);                 //version
            resWriter.Write((DWORD)0x00000000);                 //characteristics

            ver.WriteVerResource(resWriter);

            Debug.Assert(resStream.Position - startPos == dataSize + headerSize);
        }

        internal static void AppendManifestToResourceStream(Stream resStream, Stream manifestStream, bool isDll)
        {
            resStream.Position = (resStream.Position + 3) & ~3;
            const WORD RT_MANIFEST = 24;

            var resWriter = new BinaryWriter(resStream);
            resWriter.Write((DWORD)(manifestStream.Length));    //data size
            resWriter.Write((DWORD)0x00000020);                 //header size
            resWriter.Write((WORD)0xFFFF);                      //identifies type as ordinal.
            resWriter.Write((WORD)RT_MANIFEST);                 //type
            resWriter.Write((WORD)0xFFFF);                      //identifies name as ordinal.
            resWriter.Write((WORD)((isDll) ? 0x0002 : 0x0001));  //EDMAURER executables are named "1", DLLs "2"
            resWriter.Write((DWORD)0x00000000);                 //data version
            resWriter.Write((WORD)0x1030);                      //memory flags
            resWriter.Write((WORD)0x0000);                      //languageId
            resWriter.Write((DWORD)0x00000000);                 //version
            resWriter.Write((DWORD)0x00000000);                 //characteristics

            manifestStream.CopyTo(resStream);
        }

        private class VersionResourceSerializer
        {
            private readonly string _commentsContents;
            private readonly string _companyNameContents;
            private readonly string _fileDescriptionContents;
            private readonly string _fileVersionContents;
            private readonly string _internalNameContents;
            private readonly string _legalCopyrightContents;
            private readonly string _legalTrademarksContents;
            private readonly string _originalFileNameContents;
            private readonly string _productNameContents;
            private readonly string _productVersionContents;
            private readonly Version _assemblyVersionContents;

            private const string vsVersionInfoKey = "VS_VERSION_INFO";
            private const string varFileInfoKey = "VarFileInfo";
            private const string translationKey = "Translation";
            private const string stringFileInfoKey = "StringFileInfo";
            private readonly string _langIdAndCodePageKey; //should be 8 characters
            private const DWORD CP_WINUNICODE = 1200;

            private const ushort sizeVS_FIXEDFILEINFO = sizeof(DWORD) * 13;
            private readonly bool _isDll;

            internal VersionResourceSerializer(bool isDll, string comments, string companyName, string fileDescription, string fileVersion,
                string internalName, string legalCopyright, string legalTrademark, string originalFileName, string productName, string productVersion,
                Version assemblyVersion)
            {
                _isDll = isDll;
                _commentsContents = comments;
                _companyNameContents = companyName;
                _fileDescriptionContents = fileDescription;
                _fileVersionContents = fileVersion;
                _internalNameContents = internalName;
                _legalCopyrightContents = legalCopyright;
                _legalTrademarksContents = legalTrademark;
                _originalFileNameContents = originalFileName;
                _productNameContents = productName;
                _productVersionContents = productVersion;
                _assemblyVersionContents = assemblyVersion;
                _langIdAndCodePageKey = System.String.Format("{0:x4}{1:x4}", 0 /*langId*/, CP_WINUNICODE /*codepage*/);
            }

            private const uint VFT_APP = 0x00000001;
            private const uint VFT_DLL = 0x00000002;

            private IEnumerable<KeyValuePair<string, string>> GetVerStrings()
            {
                if (_commentsContents != null) yield return new KeyValuePair<string, string>("Comments", _commentsContents);
                if (_companyNameContents != null) yield return new KeyValuePair<string, string>("CompanyName", _companyNameContents);
                if (_fileDescriptionContents != null) yield return new KeyValuePair<string, string>("FileDescription", _fileDescriptionContents);

                yield return new KeyValuePair<string, string>("FileVersion", _fileVersionContents);

                if (_internalNameContents != null) yield return new KeyValuePair<string, string>("InternalName", _internalNameContents);
                if (_legalCopyrightContents != null) yield return new KeyValuePair<string, string>("LegalCopyright", _legalCopyrightContents);
                if (_legalTrademarksContents != null) yield return new KeyValuePair<string, string>("LegalTrademarks", _legalTrademarksContents);
                if (_originalFileNameContents != null) yield return new KeyValuePair<string, string>("OriginalFilename", _originalFileNameContents);
                if (_productNameContents != null) yield return new KeyValuePair<string, string>("ProductName", _productNameContents);

                yield return new KeyValuePair<string, string>("ProductVersion", _productVersionContents);

                if (_assemblyVersionContents != null) yield return new KeyValuePair<string, string>("Assembly Version", _assemblyVersionContents.ToString());
            }

            private uint FileType { get { return (_isDll) ? VFT_DLL : VFT_APP; } }

            private void WriteVSFixedFileInfo(BinaryWriter writer)
            {
                //There's nothing guaranteeing that these are n.n.n.n format.
                //The documentation says that if they're not that format the behavior is undefined.
                Version fileVersion;
                VersionHelper.TryParse(_fileVersionContents, version: out fileVersion);


                Version productVersion;
                VersionHelper.TryParse(_productVersionContents, version: out productVersion);

                writer.Write((DWORD)0xFEEF04BD);
                writer.Write((DWORD)0x00010000);
                writer.Write((DWORD)((uint)fileVersion.Major << 16) | (uint)fileVersion.Minor);
                writer.Write((DWORD)((uint)fileVersion.Build << 16) | (uint)fileVersion.Revision);
                writer.Write((DWORD)((uint)productVersion.Major << 16) | (uint)productVersion.Minor);
                writer.Write((DWORD)((uint)productVersion.Build << 16) | (uint)productVersion.Revision);
                writer.Write((DWORD)0x0000003F);   //VS_FFI_FILEFLAGSMASK  (EDMAURER) really? all these bits are valid?
                writer.Write((DWORD)0);    //file flags
                writer.Write((DWORD)0x00000004);   //VOS__WINDOWS32
                writer.Write((DWORD)this.FileType);
                writer.Write((DWORD)0);    //file subtype
                writer.Write((DWORD)0);    //date most sig
                writer.Write((DWORD)0);    //date least sig
            }

            /// <summary>
            /// Assume that 3 WORDs preceded this string and that they began 32-bit aligned.
            /// Given the string length compute the number of bytes that should be written to end
            /// the buffer on a 32-bit boundary</summary>
            /// <param name="cb"></param>
            /// <returns></returns>
            private static int PadKeyLen(int cb)
            {
                //add previously written 3 WORDS, round up, then subtract the 3 WORDS.
                return PadToDword(cb + 3 * sizeof(WORD)) - 3 * sizeof(WORD);
            }
            /// <summary>
            /// assuming the length of bytes submitted began on a 32-bit boundary,
            /// round up this length as necessary so that it ends at a 32-bit boundary.
            /// </summary>
            /// <param name="cb"></param>
            /// <returns></returns>
            private static int PadToDword(int cb)
            {
                return (cb + 3) & ~3;
            }

            private const int HDRSIZE = 3 * sizeof(ushort);

            private static ushort SizeofVerString(string lpszKey, string lpszValue)
            {
                int cbKey, cbValue;

                cbKey = (lpszKey.Length + 1) * 2;  // Make room for the NULL
                cbValue = (lpszValue.Length + 1) * 2;

                return checked((ushort)(PadKeyLen(cbKey) +    // key, 0 padded to DWORD boundary
                                cbValue +               // value
                                HDRSIZE));             // block header.
            }

            private static void WriteVersionString(KeyValuePair<string, string> keyValuePair, BinaryWriter writer)
            {
                Debug.Assert(keyValuePair.Value != null);

                ushort cbBlock = SizeofVerString(keyValuePair.Key, keyValuePair.Value);
                int cbKey = (keyValuePair.Key.Length + 1) * 2;     // includes terminating NUL
                int cbVal = (keyValuePair.Value.Length + 1) * 2;     // includes terminating NUL

                var startPos = writer.BaseStream.Position;
                Debug.Assert((startPos & 3) == 0);

                writer.Write((WORD)cbBlock);
                writer.Write((WORD)(keyValuePair.Value.Length + 1)); //add 1 for nul
                writer.Write((WORD)1);
                writer.Write(keyValuePair.Key.ToCharArray());
                writer.Write((WORD)'\0');
                writer.Write(new byte[PadKeyLen(cbKey) - cbKey]);
                Debug.Assert((writer.BaseStream.Position & 3) == 0);
                writer.Write(keyValuePair.Value.ToCharArray());
                writer.Write((WORD)'\0');
                //writer.Write(new byte[PadToDword(cbVal) - cbVal]);

                Debug.Assert(cbBlock == writer.BaseStream.Position - startPos);
            }

            /// <summary>
            /// compute number of chars needed to end up on a 32-bit boundary assuming that three
            /// WORDS preceded this string.
            /// </summary>
            /// <param name="sz"></param>
            /// <returns></returns>
            private static int KEYSIZE(string sz)
            {
                return PadKeyLen((sz.Length + 1) * sizeof(WCHAR)) / sizeof(WCHAR);
            }
            private static int KEYBYTES(string sz)
            {
                return KEYSIZE(sz) * sizeof(WCHAR);
            }

            private int GetStringsSize()
            {
                int sum = 0;

                foreach (var verString in GetVerStrings())
                {
                    sum = (sum + 3) & ~3;   //ensure that each String data structure starts on a 32bit boundary.
                    sum += SizeofVerString(verString.Key, verString.Value);
                }

                return sum;
            }

            internal int GetDataSize()
            {
                int sizeEXEVERRESOURCE = sizeof(WORD) * 3 * 5 + 2 * sizeof(WORD) + //five headers + two words for CP and lang
                    KEYBYTES(vsVersionInfoKey) +
                    KEYBYTES(varFileInfoKey) +
                    KEYBYTES(translationKey) +
                    KEYBYTES(stringFileInfoKey) +
                    KEYBYTES(_langIdAndCodePageKey) +
                    sizeVS_FIXEDFILEINFO;

                return GetStringsSize() + sizeEXEVERRESOURCE;
            }

            internal void WriteVerResource(BinaryWriter writer)
            {
                /*
                    must be assumed to start on a 32-bit boundary.
                 * 
                 * the sub-elements of the VS_VERSIONINFO consist of a header (3 WORDS) a string
                 * and then beginning on the next 32-bit boundary, the elements children
                 
                    struct VS_VERSIONINFO
                    {
                        WORD cbRootBlock;                                     // size of whole resource
                        WORD cbRootValue;                                     // size of VS_FIXEDFILEINFO structure
                        WORD fRootText;                                       // root is text?
                        WCHAR szRootKey[KEYSIZE("VS_VERSION_INFO")];          // Holds "VS_VERSION_INFO"
                        VS_FIXEDFILEINFO vsFixed;                             // fixed information.
                          WORD cbVarBlock;                                      //   size of VarFileInfo block
                          WORD cbVarValue;                                      //   always 0
                          WORD fVarText;                                        //   VarFileInfo is text?
                          WCHAR szVarKey[KEYSIZE("VarFileInfo")];               //   Holds "VarFileInfo"
                            WORD cbTransBlock;                                    //     size of Translation block
                            WORD cbTransValue;                                    //     size of Translation value
                            WORD fTransText;                                      //     Translation is text?
                            WCHAR szTransKey[KEYSIZE("Translation")];             //     Holds "Translation"
                              WORD langid;                                          //     language id
                              WORD codepage;                                        //     codepage id
                          WORD cbStringBlock;                                   //   size of StringFileInfo block
                          WORD cbStringValue;                                   //   always 0
                          WORD fStringText;                                     //   StringFileInfo is text?
                          WCHAR szStringKey[KEYSIZE("StringFileInfo")];         //   Holds "StringFileInfo"
                            WORD cbLangCpBlock;                                   //     size of language/codepage block
                            WORD cbLangCpValue;                                   //     always 0
                            WORD fLangCpText;                                     //     LangCp is text?
                            WCHAR szLangCpKey[KEYSIZE("12345678")];               //     Holds hex version of language/codepage
                        // followed by strings
                    };
                */

                var debugPos = writer.BaseStream.Position;
                var dataSize = GetDataSize();

                writer.Write((WORD)dataSize);
                writer.Write((WORD)sizeVS_FIXEDFILEINFO);
                writer.Write((WORD)0);
                writer.Write(vsVersionInfoKey.ToCharArray());
                writer.Write(new byte[KEYBYTES(vsVersionInfoKey) - vsVersionInfoKey.Length * 2]);
                Debug.Assert((writer.BaseStream.Position & 3) == 0);
                WriteVSFixedFileInfo(writer);
                writer.Write((WORD)(sizeof(WORD) * 2 + 2 * HDRSIZE + KEYBYTES(varFileInfoKey) + KEYBYTES(translationKey)));
                writer.Write((WORD)0);
                writer.Write((WORD)1);
                writer.Write(varFileInfoKey.ToCharArray());
                writer.Write(new byte[KEYBYTES(varFileInfoKey) - varFileInfoKey.Length * 2]);   //padding
                Debug.Assert((writer.BaseStream.Position & 3) == 0);
                writer.Write((WORD)(sizeof(WORD) * 2 + HDRSIZE + KEYBYTES(translationKey)));
                writer.Write((WORD)(sizeof(WORD) * 2));
                writer.Write((WORD)0);
                writer.Write(translationKey.ToCharArray());
                writer.Write(new byte[KEYBYTES(translationKey) - translationKey.Length * 2]);   //padding
                Debug.Assert((writer.BaseStream.Position & 3) == 0);
                writer.Write((WORD)0);      //langId; MAKELANGID(LANG_NEUTRAL, SUBLANG_NEUTRAL)) = 0
                writer.Write((WORD)CP_WINUNICODE);   //codepage; 1200 = CP_WINUNICODE
                Debug.Assert((writer.BaseStream.Position & 3) == 0);
                writer.Write((WORD)(2 * HDRSIZE + KEYBYTES(stringFileInfoKey) + KEYBYTES(_langIdAndCodePageKey) + GetStringsSize()));
                writer.Write((WORD)0);
                writer.Write((WORD)1);
                writer.Write(stringFileInfoKey.ToCharArray());      //actually preceded by 5 WORDS so not consistent with the
                                                                    //assumptions of KEYBYTES, but equivalent.
                writer.Write(new byte[KEYBYTES(stringFileInfoKey) - stringFileInfoKey.Length * 2]); //padding. 
                Debug.Assert((writer.BaseStream.Position & 3) == 0);
                writer.Write((WORD)(HDRSIZE + KEYBYTES(_langIdAndCodePageKey) + GetStringsSize()));
                writer.Write((WORD)0);
                writer.Write((WORD)1);
                writer.Write(_langIdAndCodePageKey.ToCharArray());
                writer.Write(new byte[KEYBYTES(_langIdAndCodePageKey) - _langIdAndCodePageKey.Length * 2]); //padding
                Debug.Assert((writer.BaseStream.Position & 3) == 0);

                Debug.Assert(writer.BaseStream.Position - debugPos == dataSize - GetStringsSize());
                debugPos = writer.BaseStream.Position;

                foreach (var entry in GetVerStrings())
                {
                    var writerPos = writer.BaseStream.Position;

                    //write any padding necessary to align the String struct on a 32 bit boundary.
                    writer.Write(new byte[((writerPos + 3) & ~3) - writerPos]);

                    Debug.Assert(entry.Value != null);
                    WriteVersionString(entry, writer);
                }

                Debug.Assert(writer.BaseStream.Position - debugPos == GetStringsSize());
            }
        }

        internal static void AppendNullResource(Stream resourceStream)
        {
            var writer = new BinaryWriter(resourceStream);
            writer.Write((UInt32)0);
            writer.Write((UInt32)0x20);
            writer.Write((UInt16)0xFFFF);
            writer.Write((UInt16)0);
            writer.Write((UInt16)0xFFFF);
            writer.Write((UInt16)0);
            writer.Write((UInt32)0);            //DataVersion
            writer.Write((UInt16)0);            //MemoryFlags
            writer.Write((UInt16)0);            //LanguageId
            writer.Write((UInt32)0);            //Version 
            writer.Write((UInt32)0);            //Characteristics 
        }

        private static void AppendDefaultVersionResource(Stream resourceStream, AssemblyBuilder sourceAssembly,
            bool isApplication)
        {
            var context = CompilerContext.Current;
            var versionAttr = sourceAssembly.GetCustomAttribute<AssemblyVersionAttribute>() ?? new AssemblyVersionAttribute("0.0.0.0");
            var titleAttr = sourceAssembly.GetCustomAttribute<AssemblyTitleAttribute>();
            var filename = context.Parameters.OutputAssembly;
            var informationVersionAttr = sourceAssembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>();
            var copytrightAttr = sourceAssembly.GetCustomAttribute<AssemblyCopyrightAttribute>();
            var trademarkAttr = sourceAssembly.GetCustomAttribute<AssemblyTrademarkAttribute>();
            var productNameAttr = sourceAssembly.GetCustomAttribute<AssemblyProductAttribute>();
            var descAttr = sourceAssembly.GetCustomAttribute<AssemblyDescriptionAttribute>();
            var companyAttr = sourceAssembly.GetCustomAttribute<AssemblyCompanyAttribute>();
            AppendVersionToResourceStream(resourceStream,
                !isApplication,
                fileVersion: versionAttr.Version,
                originalFileName: filename,
                internalName: filename,
                productVersion: informationVersionAttr?.InformationalVersion ?? versionAttr.Version,
                fileDescription: titleAttr?.Title ?? " ", //alink would give this a blank if nothing was supplied.
                assemblyVersion: new Version(versionAttr.Version),
                legalCopyright: copytrightAttr?.Copyright ?? " ", //alink would give this a blank if nothing was supplied.
                legalTrademarks: trademarkAttr?.Trademark,
                productName: productNameAttr?.Product,
                comments: descAttr?.Description,
                companyName: companyAttr?.Company);
        }

        private const string DefaultManifest = @"﻿<?xml version=""1.0"" encoding=""UTF-8"" standalone=""yes""?>

<assembly xmlns = ""urn:schemas-microsoft-com:asm.v1"" manifestVersion=""1.0"">
  <assemblyIdentity version = ""1.0.0.0"" name=""MyApplication.app""/>
  <trustInfo xmlns = ""urn:schemas-microsoft-com:asm.v2"" >
    < security >
      < requestedPrivileges xmlns=""urn:schemas-microsoft-com:asm.v3"">
        <requestedExecutionLevel level = ""asInvoker"" uiAccess=""false""/>
      </requestedPrivileges>
    </security>
  </trustInfo>
</assembly>";

        /// <summary>
        /// Create a stream filled with default win32 resources, and return its contents as a byte buffer.
        /// </summary>
        internal static byte[] CreateDefaultWin32Resources(bool noManifest, bool isApplication,
            Stream manifestContents, Stream iconInIcoFormat, AssemblyBuilder sourceAssembly)
        {
            //Win32 resource encodings use a lot of 16bit values. Do all of the math checked with the
            //expectation that integer types are well-chosen with size in mind.
            checked
            {
                var result = new MemoryStream(1024);

                //start with a null resource just as rc.exe does
                AppendNullResource(result);

                AppendDefaultVersionResource(result, sourceAssembly, isApplication);

                if (!noManifest)
                {
                    if (isApplication)
                    {
                        // Applications use a default manifest if one is not specified.
                        if (manifestContents == null)
                        {
                            manifestContents = new MemoryStream();
                            using (var sw = new StreamWriter(manifestContents))
                            {
                                sw.Write(DefaultManifest);
                            }
                            manifestContents.Position = 0;
                        }
                    }

                    if (manifestContents != null)
                    {
                        AppendManifestToResourceStream(result, manifestContents, !isApplication);
                    }
                }

                if (iconInIcoFormat != null)
                {
                    AppendIconToResourceStream(result, iconInIcoFormat);
                }

                result.Position = 0;
                return result.ToArray();
            }
        }

    }

}
