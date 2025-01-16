using Autodesk.AutoCAD.Runtime;
using AcadApp = Autodesk.AutoCAD.ApplicationServices.Application;

using VDF = Autodesk.DataManagement.Client.Framework;
using static Autodesk.DataManagement.Client.Framework.Vault.Library;
using ACW = Autodesk.Connectivity.WebServices;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.DataManagement.Client.Framework.Vault.Currency.Connections;
using System.Reflection;

[assembly: ExtensionApplication(typeof(UrbanToolR25.MyPlugin))]
namespace UrbanToolR25
{
	public class MyPlugin : IExtensionApplication
	{

		/* Two lines below can not be placed here. If they are uncommented plugin will not be loaded
		 * if they are commented this class will be executed, and plug in will be loaded. 
		 * This two lines are moved in myCommands.AmIReady() and there are works fine.*/
		//Connection connection = null;
		//ACW.Folder baseFolder = new ACW.Folder();

		string localFolderToWriteFile;// = @"C:\Users\nedeljko.sovljanski\AppData\Roaming\Autodesk\AutoCAD 2025\R25.0\enu\Support";
		bool allowSync = true;

		string serverName = "192.168.1.2";
		string vaultName = "TestUrbanizam";

		bool validConnection = false;

		private static bool _isReady;

		public static bool IsReady
		{
			get { return _isReady; }
		}
		public void Initialize()
		{
			dynamic acad = Autodesk.AutoCAD.ApplicationServices.Application.Preferences;
			dynamic support = acad.Files;
			localFolderToWriteFile = Directory.GetParent(support.ToolPalettePath as string).FullName;

			// Line below can not be used in class MyPlugin which implenets IExtensionApplication interface
			VDF.Vault.Library.ConnectionManager.ConnectionEstablished += ConnectionManager_ConnectionEstablished;
		}

		private void ConnectionManager_ConnectionEstablished(object sender, VDF.Vault.Currency.Connections.ConnectionEventArgs e)
		{
			dynamic acad = Autodesk.AutoCAD.ApplicationServices.Application.Preferences;
			dynamic support = acad.Files;
			string localFolderToWriteFile = Directory.GetParent(support.ToolPalettePath as string).FullName;

			ACW.Folder baseFolder = new ACW.Folder();
			string serverName = "192.168.1.2";
			string vaultName = "TestUrbanizam";
			(string userName, string password) = GetCredentials();
			Connection connection = ConnectionManager
				.GetExistingConnection(
				 serverName, vaultName, userName, password, AuthenticationFlags.Standard
				);
			bool allowSync = true;

			_isReady = false;
			if (connection != null)
			{
				bool validConnection = (connection.Server == serverName) && (connection.Vault == vaultName);

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
		private (string userName, string password) GetCredentials()
		{
			string userName;
			string password;
			string currentFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
			using (StreamReader stream = new StreamReader(currentFolder + "\\userCred.txt"))
			{
				string line = stream.ReadLine();
				string[] vs = line.Split('\\');
				userName = vs[0];
				password = vs[1];
			}
			return (userName, password);
		}

		public void Terminate()
		{
			VDF.Vault.Library.ConnectionManager.ConnectionEstablished -= ConnectionManager_ConnectionEstablished;
		}
	}
}
