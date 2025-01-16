// (C) Copyright 2023 by  
//
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.Windows.ToolPalette;

using AcadApp = Autodesk.AutoCAD.ApplicationServices.Application;

// This line is not mandatory, but improves loading performances
[assembly: CommandClass(typeof(UrbanTool.MyCommands))]

namespace UrbanTool
{
	// This class is instantiated by AutoCAD for each document when
	// a command is called by the user the first time in the context
	// of a given document. In other words, non static data in this class
	// is implicitly per-document!
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
		[CommandMethod("ReloadPalettes")]
		public void MyCommand() // This method can have any name
		{
			// Put your command code here
			Document doc = AcadApp.DocumentManager.MdiActiveDocument;
			Editor ed = doc.Editor;
			if (doc != null && MyPlugin.IsReady)
			{
				Autodesk.AutoCAD.Windows.ToolPalette.ToolPaletteManager mgr = Autodesk.AutoCAD.Windows.ToolPalette.ToolPaletteManager.Manager;
				foreach (Catalog item in mgr.Catalogs)
				{
					string itemName = item.Name;
					System.Guid itemGuid= item.ID;
					if (item.HasChildren)
					{
						for (int i = 0; i < item.ChildCount; i++)
						{
							Palette child = item.GetChild(i) as Palette;
							System.Guid xml = child.ID;
						}
					}
				}
				mgr.LoadCatalogs();
			}
			else
			{
				ed.WriteMessage("Command is not ready");
			}
		}
	}
}
