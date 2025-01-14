using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;
using Autodesk.DataManagement.Client.Framework.Vault.Currency.Connections;

using AcadApp = Autodesk.AutoCAD.ApplicationServices.Application;


using VDF = Autodesk.DataManagement.Client.Framework;
using static Autodesk.DataManagement.Client.Framework.Vault.Library;
using ACW = Autodesk.Connectivity.WebServices;
using System.Reflection;



[assembly: CommandClass(typeof(UrbanToolR25.MyCommands))]
namespace UrbanToolR25
{
	public class MyCommands
	{
		// The CommandMethod attribute can be applied to any public  member 
		// function of any public class.
		// The function should take no arguments and return nothing.
		// If the method is an intance member then the enclosing class is 
		// intantiated for each document. If the member is a static member then
		// the enclosing class is NOT intantiated.
		//
		// NOTE: CommandMethod has overloads where you can provide helpid and
		// context menu.

		// Modal Command with localized name
		// Modal Command with localized name
		[CommandMethod("ReloadPalettes")]
		public void MyCommand() // This method can have any name
		{
			// Put your command code here
			Document doc = AcadApp.DocumentManager.MdiActiveDocument;
			Editor ed = doc.Editor;
			if (doc != null && AmIReady())
			{
				Autodesk.AutoCAD.Windows.ToolPalette.ToolPaletteManager mgr = Autodesk.AutoCAD.Windows.ToolPalette.ToolPaletteManager.Manager;
				mgr.LoadCatalogs();
			}
			else
			{
				ed.WriteMessage("Command is not ready");
			}
		}

		private bool AmIReady()
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

			bool _isReady = false;
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
			return _isReady;
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
	}
}
