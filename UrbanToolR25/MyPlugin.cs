using Autodesk.AutoCAD.Runtime;
using AcadApp = Autodesk.AutoCAD.ApplicationServices.Application;

using VDF = Autodesk.DataManagement.Client.Framework;
using ACW = Autodesk.Connectivity.WebServices;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UrbanToolR25
{
	public class MyPlugin : IExtensionApplication
	{
		public void Initialize()
		{
			AcadApp.Idle += AcadApp_Idle;
		}

		private void AcadApp_Idle(object sender, EventArgs e)
		{
			//VDF.Vault.Library.ConnectionManager.ConnectionEstablished += ConnectionManager_ConnectionEstablished;
		}

		private void ConnectionManager_ConnectionEstablished(object sender, VDF.Vault.Currency.Connections.ConnectionEventArgs e)
		{
			
		}

		public void Terminate()
		{
			//VDF.Vault.Library.ConnectionManager.ConnectionEstablished -= ConnectionManager_ConnectionEstablished;
		}
	}
}
