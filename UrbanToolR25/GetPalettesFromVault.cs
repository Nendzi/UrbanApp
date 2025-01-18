using Autodesk.AutoCAD.Runtime;
using AcadApp = Autodesk.AutoCAD.ApplicationServices.Application;
using Autodesk.DataManagement.Client.Framework.Vault.Currency.Connections;

using VDF = Autodesk.DataManagement.Client.Framework;
using static Autodesk.DataManagement.Client.Framework.Vault.Library;
using ACW = Autodesk.Connectivity.WebServices;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.ApplicationServices;

namespace UrbanToolR25
{
	internal class GetPalettesFromVault
	{
		bool allowSync = true;
		public static string  serverName = "192.168.1.2";
		public static string  vaultName = "TestUrbanizam";
		bool validConnection = false;

		private static bool _isReady;
		public static bool IsReady
		{
			get { return _isReady; }
		}

		public GetPalettesFromVault()
		{
			// Line below can not be used in class MyPlugin which implenets IExtensionApplication interface
			VDF.Vault.Library.ConnectionManager.ConnectionEstablished += ConnectionManager_ConnectionEstablished;
		}

		~GetPalettesFromVault()
		{
			VDF.Vault.Library.ConnectionManager.ConnectionEstablished -= ConnectionManager_ConnectionEstablished;
		}

		private void ConnectionManager_ConnectionEstablished(object sender, VDF.Vault.Currency.Connections.ConnectionEventArgs e)
		{
			_isReady = Download(e.Connection);
		}

		internal static bool Download(Connection connection)
		{
			Document doc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
			Editor ed = doc.Editor;
			dynamic acad = Autodesk.AutoCAD.ApplicationServices.Application.Preferences;
			//dynamic support = acad.Files;
			string support = acad.Files.ToolPalettePath as string;
			// "C:\Users\nedeljko.sovljanski\AppData\Roaming\Autodesk\AutoCAD 2025\R25.0\enu\Support";
			string localFolderToWriteFile = Directory.GetParent(support).FullName;

			ACW.Folder baseFolder = new ACW.Folder();			
			bool allowSync = true;
			if (connection != null)
			{
				bool validConnection = (connection.Server == serverName) && (connection.Vault == vaultName);

				if (validConnection)
				{
					ed.WriteMessage("Palettes loading ...");
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
					ed.WriteMessage("Palettes are loaded");
					return true;
				}
			}
			return false;
		}
	}
}
