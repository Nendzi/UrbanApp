// (C) Copyright 2023 by Nedeljko Sovljanski
//
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.Runtime;
using AcadApp = Autodesk.AutoCAD.ApplicationServices.Application;

using VDF = Autodesk.DataManagement.Client.Framework;
using ACW = Autodesk.Connectivity.WebServices;
using Autodesk.DataManagement.Client.Framework.Vault.Currency.Connections;
using System.IO;

// This line is not mandatory, but improves loading performances
[assembly: ExtensionApplication(typeof(UrbanTool.MyPlugin))]

namespace UrbanTool
{
	// This class is instantiated by AutoCAD once and kept alive for the 
	// duration of the session. If you don't do any one time initialization 
	// then you should remove this class.
	public class MyPlugin : IExtensionApplication
	{
		Connection connection = null;

		ACW.Folder baseFolder = new ACW.Folder();

		string localFolderToWriteFile;// = @"C:\Users\nedeljko.sovljanski\AppData\Roaming\Autodesk\AutoCAD 2024\R24.3\enu\Support";
		bool allowSync = true;

		string serverName = "192.168.1.2";
		string vaultName = "TestUrbanizam";

		bool validConnection = false;

		private static bool _isReady;

		public static bool IsReady
		{
			get { return _isReady; }
		}


		void IExtensionApplication.Initialize()
		{
			// Add one time initialization here
			// One common scenario is to setup a callback function here that 
			// unmanaged code can call. 
			// To do this:
			// 1. Export a function from unmanaged code that takes a function
			//    pointer and stores the passed in value in a global variable.
			// 2. Call this exported function in this function passing delegate.
			// 3. When unmanaged code needs the services of this managed module
			//    you simply call acrxLoadApp() and by the time acrxLoadApp 
			//    returns  global function pointer is initialized to point to
			//    the C# delegate.
			// For more info see: 
			// http://msdn2.microsoft.com/en-US/library/5zwkzwf4(VS.80).aspx
			// http://msdn2.microsoft.com/en-us/library/44ey4b32(VS.80).aspx
			// http://msdn2.microsoft.com/en-US/library/7esfatk4.aspx
			// as well as some of the existing AutoCAD managed apps.

			// Initialize your plug-in application here
			dynamic acad = Autodesk.AutoCAD.ApplicationServices.Application.Preferences;
			dynamic support = acad.Files;//.SupportPath as string;
			localFolderToWriteFile = Directory.GetParent(support.ToolPalettePath as string).FullName;

			VDF.Vault.Library.ConnectionManager.ConnectionEstablished += ConnectionManager_ConnectionEstablished;
		}
		private Connection ConnectToVault()
		{
			string userName = "";
			string password = "";
			VDF.Vault.Results.LogInResult results = VDF.Vault.Library.ConnectionManager.LogIn(serverName,
				 vaultName, userName, password, AuthenticationFlags.Standard, null);

			if (results.Success)
			{
				return results.Connection;
			}

			return null;
		}
		protected void ConnectionManager_ConnectionEstablished(object sender, ConnectionEventArgs e)
		{
			connection = e.Connection;
			_isReady = false;
			if (connection != null)
			{
				validConnection = (connection.Server == serverName) && (connection.Vault == vaultName);

				if (validConnection)
				{
					baseFolder = connection.WebServiceManager.DocumentService.GetFolderByPath("$/Podesavanja/Urbanist");
					ACW.Folder[] subFolders = connection.WebServiceManager.DocumentService.GetFoldersByParentId(baseFolder.Id, true);

					VDF.Vault.Settings.AcquireFilesSettings settings = new VDF.Vault.Settings.AcquireFilesSettings(connection, false);

					if (allowSync)
					{
						settings.OptionsResolution.SyncWithRemoteSiteSetting = VDF.Vault.Settings.AcquireFilesSettings.SyncWithRemoteSite.Always;
					}
					else
					{
						settings.OptionsResolution.SyncWithRemoteSiteSetting = VDF.Vault.Settings.AcquireFilesSettings.SyncWithRemoteSite.Never;
					}
					foreach (var folder in subFolders)
					{
						ACW.File[] files = connection.WebServiceManager.DocumentService.GetLatestFilesByFolderId(folder.Id, true);
						if (files != null)
						{
							settings.LocalPath = new VDF.Currency.FolderPathAbsolute(localFolderToWriteFile);
							foreach (var file in files)
							{
								VDF.Vault.Currency.Entities.FileIteration fileToDownLoad = new VDF.Vault.Currency.Entities.FileIteration(connection, file);
								settings.AddFileToAcquire(fileToDownLoad, VDF.Vault.Settings.AcquireFilesSettings.AcquisitionOption.Download);
							}
						}
					}
					connection.FileManager.AcquireFiles(settings);
					_isReady = true;
				}
			}
		}

		void IExtensionApplication.Terminate()
		{
			// Do plug-in application clean up here
			VDF.Vault.Library.ConnectionManager.ConnectionEstablished -= ConnectionManager_ConnectionEstablished;
		}
	}
}
