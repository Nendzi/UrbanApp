using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;
using Autodesk.DataManagement.Client.Framework.Vault.Currency.Connections;

using AcadApp = Autodesk.AutoCAD.ApplicationServices.Application;
using VDF = Autodesk.DataManagement.Client.Framework;

using System.Reflection;
using Autodesk.AutoCAD.Windows.ToolPalette;
using System.Linq;

[assembly: CommandClass(typeof(UrbanToolR25.MyCommands))]
namespace UrbanToolR25
{
	public class MyCommands
	{
		Connection connection;
		List<string> UrbCstmPalettes = new List<string>()
		{
			"Dopunske kategorije",
			"Granice",
			"Koridori",
			"Namena zemljišta",
			"Površine i objekti",
			"Režimi zaštite",
			"Saobraćajna infrastruktura",
			"Tehnička i komunalna",
			"Zemljište"
		};

		[CommandMethod("ReloadPalettes")]
		public void MyCommand() // This method can have any name
		{
			Document doc = AcadApp.DocumentManager.MdiActiveDocument;
			Editor ed = doc.Editor;
			if (doc != null && GetPalettesFromVault.IsReady /*|| CheckConnection())*/)
			{
				ToolPaletteManager mgr = ToolPaletteManager.Manager;
				mgr.LoadCatalogs();
				CreateXPGfile(mgr);
			}
			else
			{
				ed.WriteMessage("Command is not ready");
			}
		}
		private bool CheckConnection()
		{
			(string userName, string password, string serverName, string vaultName) = GetCredentials();
			connection = VDF.Vault.Library.ConnectionManager
				.GetExistingConnection(
				 serverName, vaultName, userName, password, AuthenticationFlags.Standard
				);
			if (connection == null)
			{
				return false;
			}
			GetPalettesFromVault.Download(connection);
			return true;
		}
		private (string userName, string password, string serverName, string vaultName) GetCredentials()
		{
			string userName;
			string password;
			string serverName;
			string vaultName;
			string currentFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
			using (StreamReader stream = new StreamReader(currentFolder + "\\userCred.txt"))
			{
				string line = stream.ReadLine();
				string[] vs = line.Split('\\');
				userName = vs[0];
				password = vs[1];
				serverName = vs[2];
				vaultName = vs[3];
			}
			return (userName, password, serverName, vaultName);
		}
		private void CreateXPGfile(ToolPaletteManager mgr)
		{
			Dictionary<System.Guid, string> collection = CollectGuids(mgr);
			using (StreamWriter stream = new StreamWriter(@"C:\TestUrbanizam\Podesavanja\PaletteGroups\Urbanizam.xpg", false))
			{
				stream.WriteLine("<ToolPaletteGroupExport>");
				stream.WriteLine("\t<ToolPaletteGroups>");
				stream.WriteLine("\t\t<ToolPaletteGroup>");
				stream.WriteLine("\t\t\t<Name>Urbanizam</Name>");
				stream.WriteLine("\t\t\t<CustomData/>");
				stream.WriteLine("\t\t\t<ActivePaletteIndex>3</ActivePaletteIndex>");
				stream.WriteLine("\t\t\t<Contents>");
				foreach (var item in collection)
				{
					stream.Write("\t\t\t\t<ToolPalette ID=\"{");
					stream.Write(item.Key.ToString().ToUpper());
					stream.WriteLine("}\"/>");
				}
				stream.WriteLine("\t\t\t</Contents>");
				stream.WriteLine("\t\t</ToolPaletteGroup>");
				stream.WriteLine("\t</ToolPaletteGroups>");
				stream.WriteLine("</ToolPaletteGroupExport>");
			}
		}
		private Dictionary<Guid, string> CollectGuids(ToolPaletteManager mgr)
		{
			Dictionary<System.Guid, string> collection = new Dictionary<Guid, string>();
			foreach (Catalog item in mgr.Catalogs)
			{
				string itemName = item.Name;
				Guid itemGuid = item.ID;
				if (item.HasChildren)
				{
					for (int i = 0; i < item.ChildCount; i++)
					{
						Palette child = item.GetChild(i) as Palette;
						Guid childId = child.ID;
						if (!collection.ContainsKey(childId))
						{
							if (UrbCstmPalettes.Exists(x=> child.Name.StartsWith(x)))
							{
								collection.Add(childId, child.Name);
							}
						}
					}
				}
			}
			return collection;
		}
	}
}
