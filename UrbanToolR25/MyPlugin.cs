using Autodesk.AutoCAD.Runtime;
using AcadApp = Autodesk.AutoCAD.ApplicationServices.Application;

using VDF = Autodesk.DataManagement.Client.Framework;
using ACW = Autodesk.Connectivity.WebServices;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.DataManagement.Client.Framework.Vault.Currency.Connections;

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
			//VDF.Vault.Library.ConnectionManager.ConnectionEstablished += ConnectionManager_ConnectionEstablished;
		}

		private void ConnectionManager_ConnectionEstablished(object sender, VDF.Vault.Currency.Connections.ConnectionEventArgs e)
		{
			/* This method is moved in myCommands.AmIReady()
			 * Vault libraries works fine after complete opening autocad 2025			 */
		}

		public void Terminate()
		{
			//VDF.Vault.Library.ConnectionManager.ConnectionEstablished -= ConnectionManager_ConnectionEstablished;
		}
	}
}
