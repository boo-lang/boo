using System;
using System.Runtime.InteropServices;

namespace ShellLib
{
	[ComImport]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[Guid("000214E6-0000-0000-C000-000000000046")]
	internal interface IShellFolder 
	{
		// Translates a file object's or folder's display name into an item identifier list.
		// Return value: error code, if any
		[PreserveSig]
		Int32 ParseDisplayName( 
			IntPtr hwnd,				// Optional window handle
			IntPtr pbc,					// Optional bind context that controls the parsing operation. This parameter is 
			// normally set to NULL. 
			[MarshalAs(UnmanagedType.LPWStr)] 
			String pszDisplayName,		// Null-terminated UNICODE string with the display name.
			ref UInt32 pchEaten,		// Pointer to a ULONG value that receives the number of characters of the 
			// display name that was parsed.
			out IntPtr ppidl,			// Pointer to an ITEMIDLIST pointer that receives the item identifier list for 
			// the object.
			ref UInt32 pdwAttributes);	// Optional parameter that can be used to query for file attributes.
		// this can be values from the SFGAO enum
        
		// Allows a client to determine the contents of a folder by creating an item identifier enumeration object 
		// and returning its IEnumIDList interface.
		// Return value: error code, if any
		[PreserveSig]
		Int32 EnumObjects( 
			IntPtr hwnd,				// If user input is required to perform the enumeration, this window handle 
			// should be used by the enumeration object as the parent window to take 
			// user input.
			Int32 grfFlags,				// Flags indicating which items to include in the enumeration. For a list 
			// of possible values, see the SHCONTF enum. 
			out IntPtr ppenumIDList);	// Address that receives a pointer to the IEnumIDList interface of the 
		// enumeration object created by this method. 

		// Retrieves an IShellFolder object for a subfolder.
		// Return value: error code, if any
		[PreserveSig]
		Int32 BindToObject( 
			IntPtr pidl,				// Address of an ITEMIDLIST structure (PIDL) that identifies the subfolder.
			IntPtr pbc,					// Optional address of an IBindCtx interface on a bind context object to be 
			// used during this operation.
			Guid riid,					// Identifier of the interface to return. 
			out IntPtr ppv);			// Address that receives the interface pointer.
        
		// Requests a pointer to an object's storage interface. 
		// Return value: error code, if any
		[PreserveSig]
		Int32 BindToStorage( 
			IntPtr pidl,				// Address of an ITEMIDLIST structure that identifies the subfolder relative 
			// to its parent folder. 
			IntPtr pbc,					// Optional address of an IBindCtx interface on a bind context object to be 
			// used during this operation.
			Guid riid,					// Interface identifier (IID) of the requested storage interface.
			out IntPtr ppv);			// Address that receives the interface pointer specified by riid.
        
		// Determines the relative order of two file objects or folders, given their item identifier lists.
		// Return value: If this method is successful, the CODE field of the HRESULT contains one of the following 
		// values (the code can be retrived using the helper function GetHResultCode):
		// Negative A negative return value indicates that the first item should precede the second (pidl1 < pidl2). 
		// Positive A positive return value indicates that the first item should follow the second (pidl1 > pidl2). 
		// Zero A return value of zero indicates that the two items are the same (pidl1 = pidl2). 
		[PreserveSig]
		Int32 CompareIDs( 
			Int32 lParam,				// Value that specifies how the comparison should be performed. The lower 
			// sixteen bits of lParam define the sorting rule. The upper sixteen bits of 
			// lParam are used for flags that modify the sorting rule. values can be from 
			// the SHCIDS enum
			IntPtr pidl1,				// Pointer to the first item's ITEMIDLIST structure.
			IntPtr pidl2);				// Pointer to the second item's ITEMIDLIST structure.

		// Requests an object that can be used to obtain information from or interact with a folder object.
		// Return value: error code, if any
		[PreserveSig]
		Int32 CreateViewObject( 
			IntPtr hwndOwner,			// Handle to the owner window.
			Guid riid,					// Identifier of the requested interface. 
			out IntPtr ppv);			// Address of a pointer to the requested interface. 

		// Retrieves the attributes of one or more file objects or subfolders. 
		// Return value: error code, if any
		[PreserveSig]
		Int32 GetAttributesOf( 
			UInt32 cidl,				// Number of file objects from which to retrieve attributes. 
			
			[MarshalAs(UnmanagedType.LPArray, SizeParamIndex=0)]
			IntPtr[] apidl,				// Address of an array of pointers to ITEMIDLIST structures, each of which 
			// uniquely identifies a file object relative to the parent folder.
			ref UInt32 rgfInOut);		// Address of a single ULONG value that, on entry, contains the attributes that 
		// the caller is requesting. On exit, this value contains the requested 
		// attributes that are common to all of the specified objects. this value can
		// be from the SFGAO enum
       
		// Retrieves an OLE interface that can be used to carry out actions on the specified file objects or folders.
		// Return value: error code, if any
		[PreserveSig]
		Int32 GetUIObjectOf( 
			IntPtr hwndOwner,			// Handle to the owner window that the client should specify if it displays 
			// a dialog box or message box.
			UInt32 cidl,				// Number of file objects or subfolders specified in the apidl parameter. 
			IntPtr[] apidl,				// Address of an array of pointers to ITEMIDLIST structures, each of which 
			// uniquely identifies a file object or subfolder relative to the parent folder.
			Guid riid,					// Identifier of the COM interface object to return.
			ref UInt32 rgfReserved,		// Reserved. 
			out IntPtr ppv);			// Pointer to the requested interface.

		// Retrieves the display name for the specified file object or subfolder. 
		// Return value: error code, if any
		[PreserveSig]
		Int32 GetDisplayNameOf(
			IntPtr pidl,				// Address of an ITEMIDLIST structure (PIDL) that uniquely identifies the file 
			// object or subfolder relative to the parent folder. 
			UInt32 uFlags,				// Flags used to request the type of display name to return. For a list of 
			// possible values, see the SHGNO enum. 
			out ShellApi.STRRET pName);			// Address of a STRRET structure in which to return the display name.
        
		// Sets the display name of a file object or subfolder, changing the item identifier in the process.
		// Return value: error code, if any
		[PreserveSig]
		Int32 SetNameOf( 
			IntPtr hwnd,				// Handle to the owner window of any dialog or message boxes that the client 
			// displays.
			IntPtr pidl,				// Pointer to an ITEMIDLIST structure that uniquely identifies the file object
			// or subfolder relative to the parent folder. 
			[MarshalAs(UnmanagedType.LPWStr)] 
			String pszName,				// Pointer to a null-terminated string that specifies the new display name. 
			UInt32 uFlags,				// Flags indicating the type of name specified by the lpszName parameter. For 
			// a list of possible values, see the description of the SHGNO enum. 
			out IntPtr ppidlOut);		// Address of a pointer to an ITEMIDLIST structure which receives the new ITEMIDLIST. 
	}

}