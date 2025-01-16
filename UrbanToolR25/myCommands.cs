using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;
using Autodesk.DataManagement.Client.Framework.Vault.Currency.Connections;

using AcadApp = Autodesk.AutoCAD.ApplicationServices.Application;


using System.Reflection;



[assembly: CommandClass(typeof(UrbanToolR25.MyCommands))]
namespace UrbanToolR25
{
	public class MyCommands
	{
		[CommandMethod("ReloadPalettes")]
		public void MyCommand() // This method can have any name
		{
			// Put your command code here
			Document doc = AcadApp.DocumentManager.MdiActiveDocument;
			Editor ed = doc.Editor;
			if (doc != null && MyPlugin.IsReady)
			{
				Autodesk.AutoCAD.Windows.ToolPalette.ToolPaletteManager mgr = Autodesk.AutoCAD.Windows.ToolPalette.ToolPaletteManager.Manager;
				mgr.LoadCatalogs();
			}
			else
			{
				ed.WriteMessage("Command is not ready");
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
	}
}
