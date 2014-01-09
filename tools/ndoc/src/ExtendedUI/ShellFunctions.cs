using System;
using System.Runtime.InteropServices;


namespace ShellLib
{

	internal class ShellFunctions
	{
		public static IMalloc GetMalloc()
		{
			IntPtr ptrRet;
			ShellApi.SHGetMalloc(out ptrRet);
				
			Object obj = Marshal.GetTypedObjectForIUnknown(ptrRet,GetMallocType());
			IMalloc imalloc = (IMalloc)obj;
				
			return imalloc;
		}

		public static IShellFolder GetDesktopFolder()
		{
			IntPtr ptrRet;
			ShellApi.SHGetDesktopFolder(out ptrRet);

			System.Type shellFolderType = System.Type.GetType("ShellLib.IShellFolder");
			Object obj = Marshal.GetTypedObjectForIUnknown(ptrRet,shellFolderType);
			IShellFolder ishellFolder = (IShellFolder)obj;

			return ishellFolder;
		}

		public static Type GetShellFolderType()
		{
			System.Type shellFolderType = System.Type.GetType("ShellLib.IShellFolder");
			return shellFolderType;
		}

		public static Type GetMallocType()
		{
			System.Type mallocType = System.Type.GetType("ShellLib.IMalloc");
			return mallocType;
		}

		public static Type GetFolderFilterType()
		{
			System.Type folderFilterType = System.Type.GetType("ShellLib.IFolderFilter");
			return folderFilterType;
		}

		public static Type GetFolderFilterSiteType()
		{
			System.Type folderFilterSiteType = System.Type.GetType("ShellLib.IFolderFilterSite");
			return folderFilterSiteType;
		}

		public static IShellFolder GetShellFolder(IntPtr ptrShellFolder)
		{
			System.Type shellFolderType = GetShellFolderType();
			Object obj = Marshal.GetTypedObjectForIUnknown(ptrShellFolder,shellFolderType);
			IShellFolder RetVal = (IShellFolder)obj;
			return RetVal;
		}
			
	}

}