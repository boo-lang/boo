using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using ShellLib;

namespace NDoc.ExtendedUI
{
	public class ShellBrowseForFolderDialog
	{
		// The SendMessage function sends the specified message to a window or windows. It calls the window procedure for 
		// the specified window and does not return until the window procedure has processed the message. 
		[DllImport("User32.dll")]
		public static extern Int32 SendMessage(
			IntPtr hWnd, // handle to destination window
			UInt32 Msg, // message
			UInt32 wParam, // first message parameter
			Int32 lParam			// second message parameter
);

		[DllImport("User32.dll")]
		public static extern Int32 SendMessage(
			IntPtr hWnd, // handle to destination window
			UInt32 Msg, // message
			UInt32 wParam, // first message parameter
			[MarshalAs(UnmanagedType.LPWStr)]
			String lParam			// second message parameter
);

		public enum RootTypeOptions
		{
			BySpecialFolder, 
			ByPath
		}

		[Flags]
		public enum BrowseInfoFlag // BIF
		{
			BIF_RETURNONLYFSDIRS = 0x0001, // For finding a folder to start document searching
			BIF_DONTGOBELOWDOMAIN = 0x0002, // For starting the Find Computer
			BIF_STATUSTEXT = 0x0004, // Top of the dialog has 2 lines of text for BROWSEINFO.lpszTitle and 
			// one line if this flag is set.  Passing the message 
			// BFFM_SETSTATUSTEXTA to the hwnd can set the rest of the text.  
			// This is not used with BIF_USENEWUI and BROWSEINFO.lpszTitle gets
			// all three lines of text.
			BIF_RETURNFSANCESTORS = 0x0008, 
			BIF_EDITBOX = 0x0010, // Add an editbox to the dialog
			BIF_VALIDATE = 0x0020, // insist on valid result (or CANCEL)
			BIF_NEWDIALOGSTYLE = 0x0040, // Use the new dialog layout with the ability to resize
			// Caller needs to call OleInitialize() before using this API
			BIF_USENEWUI = (BIF_NEWDIALOGSTYLE | BIF_EDITBOX), 
			BIF_BROWSEINCLUDEURLS = 0x0080, // Allow URLs to be displayed or entered. (Requires BIF_USENEWUI)
			BIF_UAHINT = 0x0100, // Add a UA hint to the dialog, in place of the edit box. May not be 
			// combined with BIF_EDITBOX
			BIF_NONEWFOLDERBUTTON = 0x0200, // Do not add the "New Folder" button to the dialog.  Only applicable 
			// with BIF_NEWDIALOGSTYLE.
			BIF_NOTRANSLATETARGETS = 0x0400, // don't traverse target as shortcut
			BIF_BROWSEFORCOMPUTER = 0x1000, // Browsing for Computers.
			BIF_BROWSEFORPRINTER = 0x2000, // Browsing for Printers
			BIF_BROWSEINCLUDEFILES = 0x4000, // Browsing for Everything
			BIF_SHAREABLE = 0x8000		// sharable resources displayed (remote shares, requires BIF_USENEWUI)
		}

		public enum	BrowseForFolderMessages	// BFFM
		{
			// message from browser
			BFFM_INITIALIZED = 1, 
			BFFM_SELCHANGED = 2, 
			BFFM_VALIDATEFAILEDA = 3, // lParam:szPath ret:1(cont),0(EndDialog)
			BFFM_VALIDATEFAILEDW = 4, // lParam:wzPath ret:1(cont),0(EndDialog)
			BFFM_IUNKNOWN = 5, // provides IUnknown to client. lParam: IUnknown*

			// messages to browser
			// 0x400 = WM_USER
			BFFM_SETSTATUSTEXTA = (0x0400 + 100), 
			BFFM_ENABLEOK = (0x0400 + 101), 
			BFFM_SETSELECTIONA = (0x0400 + 102), 
			BFFM_SETSELECTIONW = (0x0400 + 103), 
			BFFM_SETSTATUSTEXTW = (0x0400 + 104), 
			BFFM_SETOKTEXT = (0x0400 + 105), // Unicode only
			BFFM_SETEXPANDED = (0x0400 + 106)	// Unicode only
		}

		[Flags]
		public enum ShellFolderGetAttributesOfFlags : uint // SFGAO
		{
			SFGAO_CANCOPY = 0x00000001, // Objects can be copied, DROPEFFECT_COPY 
			SFGAO_CANMOVE = 0x00000002, // Objects can be moved,  DROPEFFECT_MOVE 
			SFGAO_CANLINK = 0x00000004, // Objects can be linked, 			DROPEFFECT_LINK 
			SFGAO_CANRENAME = 0x00000010, // Objects can be renamed
			SFGAO_CANDELETE = 0x00000020, // Objects can be deleted
			SFGAO_HASPROPSHEET = 0x00000040, // Objects have property sheets
			SFGAO_DROPTARGET = 0x00000100, // Objects are drop target
			SFGAO_CAPABILITYMASK = 0x00000177, 
			SFGAO_LINK = 0x00010000, // Shortcut (link)
			SFGAO_SHARE = 0x00020000, // shared
			SFGAO_READONLY = 0x00040000, // read-only
			SFGAO_GHOSTED = 0x00080000, // ghosted icon
			SFGAO_DISPLAYATTRMASK = 0x000F0000, 
			SFGAO_FILESYSANCESTOR = 0x10000000, // It contains file system folder
			SFGAO_FOLDER = 0x20000000, // It's a folder.
			SFGAO_FILESYSTEM = 0x40000000, // is a file system thing (file/folder/root)
			SFGAO_HASSUBFOLDER = 0x80000000, // Expandable in the map pane
			SFGAO_CONTENTSMASK = 0x80000000, 
			SFGAO_VALIDATE = 0x01000000, // invalidate cached information
			SFGAO_REMOVABLE = 0x02000000, // is this removeable media?
			SFGAO_COMPRESSED = 0x04000000, // Object is compressed (use alt color)
		}


		public class InitializedEventArgs : EventArgs
		{
			public InitializedEventArgs(IntPtr hwnd)
			{
				this.hwnd = hwnd;
			}
			public readonly IntPtr hwnd;
		}

		public class IUnknownEventArgs : EventArgs
		{
			public IUnknownEventArgs(IntPtr hwnd, IntPtr iunknown)
			{
				this.hwnd = hwnd;
				this.iunknown = iunknown;
			}
			public readonly IntPtr hwnd;
			public readonly IntPtr iunknown;
		}

		public class SelChangedEventArgs : EventArgs
		{
			public SelChangedEventArgs(IntPtr hwnd, IntPtr pidl)
			{
				this.hwnd = hwnd;
				this.pidl = pidl;
			}
			public readonly IntPtr hwnd;
			public readonly IntPtr pidl;
		}

		public class ValidateFailedEventArgs : EventArgs
		{
			public ValidateFailedEventArgs(IntPtr hwnd, string invalidSel)
			{
				this.hwnd = hwnd;
				this.invalidSel = invalidSel;
			}
			public readonly IntPtr hwnd;
			public readonly string invalidSel;
		}
		
		
		public delegate void InitializedHandler(ShellBrowseForFolderDialog sender, InitializedEventArgs args);
		public delegate void IUnknownHandler(ShellBrowseForFolderDialog sender, IUnknownEventArgs args);
		public delegate void SelChangedHandler(ShellBrowseForFolderDialog sender, SelChangedEventArgs args);
		public delegate int ValidateFailedHandler(ShellBrowseForFolderDialog sender, ValidateFailedEventArgs args);

		public event InitializedHandler OnInitialized;
		public event IUnknownHandler OnIUnknown;
		public event SelChangedHandler OnSelChanged;
		public event ValidateFailedHandler OnValidateFailed;

		public void EnableOk(IntPtr hwnd, bool Enabled)
		{
			SendMessage(hwnd, (uint)BrowseForFolderMessages.BFFM_ENABLEOK, 0, Enabled ? 1 : 0);
		}

		public void SetExpanded(IntPtr hwnd, string path)
		{
			SendMessage(hwnd, (uint)BrowseForFolderMessages.BFFM_SETEXPANDED, 1, path);
		}

		public void SetOkText(IntPtr hwnd, string text)
		{
			SendMessage(hwnd, (uint)BrowseForFolderMessages.BFFM_SETOKTEXT, 0, text);
		}

		public void SetSelection(IntPtr hwnd, string path)
		{
			SendMessage(hwnd, (uint)BrowseForFolderMessages.BFFM_SETSELECTIONW, 1, path);
		}

		public void SetStatusText(IntPtr hwnd, string text)
		{
			SendMessage(hwnd, (uint)BrowseForFolderMessages.BFFM_SETSTATUSTEXTW, 1, text);
		}


		private Int32 myBrowseCallbackProc(IntPtr hwnd, UInt32 uMsg, Int32 lParam, Int32 lpData)
		{
			switch ((BrowseForFolderMessages)uMsg)
			{
				case BrowseForFolderMessages.BFFM_INITIALIZED : 
					//System.Diagnostics.Debug.WriteLine("BFFM_INITIALIZED");

					if (OnInitialized != null)
					{
						InitializedEventArgs args = new InitializedEventArgs(hwnd);
						OnInitialized(this, args);
					}

					break;

				case BrowseForFolderMessages.BFFM_IUNKNOWN : 
					//System.Diagnostics.Debug.WriteLine("BFFM_IUNKNOWN");

					if (OnIUnknown != null)
					{
						IUnknownEventArgs args = new IUnknownEventArgs(hwnd, (IntPtr)lParam);
						OnIUnknown(this, args);
					}

					break;

				case BrowseForFolderMessages.BFFM_SELCHANGED : 
					//System.Diagnostics.Debug.WriteLine("BFFM_SELCHANGED");
					
					if (OnSelChanged != null)
					{
						SelChangedEventArgs args = new SelChangedEventArgs(hwnd, (IntPtr)lParam);
						OnSelChanged(this, args);
					}

					EnableOk(hwnd, IsPIDLFileSystem((IntPtr)lParam));
				
					break;
				
				case BrowseForFolderMessages.BFFM_VALIDATEFAILEDA : 
					//System.Diagnostics.Debug.WriteLine("BFFM_VALIDATEFAILEDA");
				
					if (OnValidateFailed != null)
					{
						string failedSel = Marshal.PtrToStringAnsi((IntPtr)lParam);
						ValidateFailedEventArgs args = new ValidateFailedEventArgs(hwnd, failedSel);
						return OnValidateFailed(this, args);
					}
					break;

				case BrowseForFolderMessages.BFFM_VALIDATEFAILEDW : 
					//System.Diagnostics.Debug.WriteLine("BFFM_VALIDATEFAILEDW");
					
					if (OnValidateFailed != null)
					{
						string failedSel = Marshal.PtrToStringUni((IntPtr)lParam);
						ValidateFailedEventArgs args = new ValidateFailedEventArgs(hwnd, failedSel);
						return OnValidateFailed(this, args);
					}
										
					break;
			}
		
			return 0;
		}


		public ShellBrowseForFolderDialog()
		{
			hwndOwner = IntPtr.Zero;
			RootType = RootTypeOptions.BySpecialFolder;
			RootSpecialFolder = ShellApi.CSIDL.CSIDL_DESKTOP;
			RootPath = "";
			m_DisplayName = "";
			Title = "";
			UserToken = IntPtr.Zero;
			m_FullName = "";

			// Default flags values
			DetailsFlags = BrowseInfoFlag.BIF_RETURNONLYFSDIRS 
				| BrowseInfoFlag.BIF_RETURNFSANCESTORS
				| BrowseInfoFlag.BIF_EDITBOX 
				| BrowseInfoFlag.BIF_NEWDIALOGSTYLE 
				| BrowseInfoFlag.BIF_SHAREABLE
				| BrowseInfoFlag.BIF_STATUSTEXT
				| BrowseInfoFlag.BIF_USENEWUI
				| BrowseInfoFlag.BIF_VALIDATE;

		}

		public DialogResult ShowDialog()
		{
			DialogResult result;
			m_FullName = "";
			m_DisplayName = "";

			// Get shell's memory allocator, it is needed to free some memory later
			IMalloc pMalloc;
			pMalloc = ShellFunctions.GetMalloc();
		
			IntPtr pidlRoot;
			
			if (RootType == RootTypeOptions.BySpecialFolder)
			{
				ShellApi.SHGetFolderLocation(hwndOwner, (int)RootSpecialFolder, UserToken, 0, out pidlRoot);
			}
			else	// m_RootType = RootTypeOptions.ByPath
			{
				uint iAttribute;
				ShellApi.SHParseDisplayName(RootPath, IntPtr.Zero, out pidlRoot, 0, out iAttribute);
			}
			this.pidlRoot = pidlRoot;
									
			ShellApi.BROWSEINFO bi = new ShellApi.BROWSEINFO();
			
			bi.hwndOwner = hwndOwner;
			bi.pidlRoot = pidlRoot;
			bi.pszDisplayName = new String(' ', 256);
			bi.lpszTitle = Title;
			bi.ulFlags = (uint)DetailsFlags;
			bi.lParam = 0;
			bi.lpfn = new ShellApi.BrowseCallbackProc(this.myBrowseCallbackProc);
			
			// Show dialog
			IntPtr pidlSelected;
			pidlSelected = ShellLib.ShellApi.SHBrowseForFolder(ref bi);

			// Save the display name
			m_DisplayName = bi.pszDisplayName.ToString();

			IShellFolder isf = ShellFunctions.GetDesktopFolder();

			ShellApi.STRRET ptrDisplayName;
			isf.GetDisplayNameOf(pidlSelected, (uint)ShellApi.SHGNO.SHGDN_NORMAL | (uint)ShellApi.SHGNO.SHGDN_FORPARSING, out ptrDisplayName);
			
			String sDisplay;
			ShellLib.ShellApi.StrRetToBSTR(ref ptrDisplayName, pidlRoot, out sDisplay);
			m_FullName = sDisplay;

			if (pidlSelected != IntPtr.Zero)
			{
				result = DialogResult.OK;
			}
			else
			{
				result = DialogResult.Cancel;
			}

			if (pidlRoot != IntPtr.Zero)
				pMalloc.Free(pidlRoot);
			
			if (pidlSelected != IntPtr.Zero)
				pMalloc.Free(pidlSelected);
			
			Marshal.ReleaseComObject(pMalloc);
			Marshal.ReleaseComObject(isf);

			return result;
		}

		IntPtr pidlRoot;


		///  <summary> Handle to the owner window for the dialog box.  </summary>
		public IntPtr hwndOwner;

		/// <summary> Select the root type </summary>
		public RootTypeOptions RootType;

		/// <summary> valid only if RootType is RootTypeOptions.ByPath </summary>
		public string RootPath;

		/// <summary> valid only if RootType is RootTypeOptions.BySpecialFolder </summary>
		internal ShellApi.CSIDL RootSpecialFolder;

		/// <summary> Address of a buffer to receive the display name of the folder selected by the user. </summary>
		public string DisplayName
		{
			get
			{
				return m_DisplayName;
			}
		}
		private string m_DisplayName;

		/// <summary> 
		/// Address of a null-terminated string that is displayed above the tree view control in the dialog box. 
		/// </summary>
		public string Title;

		/// <summary> Token that can be used to represent a particular user. </summary>
		public IntPtr UserToken;

		/// <summary> Return the result of the dialog </summary>
		public string FullName
		{
			get
			{
				return m_FullName;
			}
		}
		private string m_FullName;

		public BrowseInfoFlag DetailsFlags;

		public bool IsPIDLFileSystem(IntPtr pidlItem)
		{
			IShellFolder isf = ShellFunctions.GetDesktopFolder();
			ShellApi.STRRET ptrDisplayName;
			isf.GetDisplayNameOf(pidlItem, (uint)ShellApi.SHGNO.SHGDN_NORMAL | (uint)ShellApi.SHGNO.SHGDN_FORPARSING, out ptrDisplayName);
			String sDisplay;
			ShellLib.ShellApi.StrRetToBSTR(ref ptrDisplayName, this.pidlRoot, out sDisplay);

//			uint attrib = 0xFFFFFFFF ; //(uint)ShellFolderGetAttributesOfFlags.SFGAO_FILESYSTEM;
//			IntPtr[] apidl = new IntPtr[1];
//			apidl[0] = pidlItem;
//			isf.GetAttributesOf(1, apidl, ref attrib);
            bool result = !sDisplay.StartsWith("::");
			//System.Diagnostics.Debug.WriteLine(attrib.ToString("x8") + "  ==>" + sDisplay);
			return result;
		}

	}

}
