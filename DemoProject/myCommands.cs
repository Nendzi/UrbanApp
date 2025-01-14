// (C) Copyright 2012 by  
//
using System;
using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.EditorInput;

// This line is not mandatory, but improves loading performances
[assembly: CommandClass(typeof(DemoProject.MyCommands))]

namespace DemoProject
{
    public class MyCommands
    {
        // To demonstrate accessing iterating of Palette groups and palettes
        [CommandMethod("Demo1", CommandFlags.NoHistory)]
        public void Demo1()
        {
            Document activeDoc = Application.DocumentManager.MdiActiveDocument;
            Editor ed = activeDoc.Editor;
            ToolPaletteWrapper.MyUtilities.GetInstance().Iterate();
        }

        // To demonstrate activating a specific palette using its name
        [CommandMethod("Demo2", CommandFlags.NoHistory)]
        public void Demo2() 
        {
            Document activeDoc = Application.DocumentManager.MdiActiveDocument;
            Editor ed = activeDoc.Editor;

            String toolPaletteName = "Mechanical";

            ToolPaletteWrapper.MyUtilities.GetInstance().ActivateToolPalette(toolPaletteName);
        }

        // To demonstrate creation of a toolpalette group, creation of a toolpalette and adding palettes to a group.
        [CommandMethod("Demo3", CommandFlags.NoHistory)]
        public void Demo3()
        {
            Document activeDoc = Application.DocumentManager.MdiActiveDocument;
            Editor ed = activeDoc.Editor;

            String toolPaletteGroupName = "MyTPGroup";

            ToolPaletteWrapper.MyUtilities.GetInstance().CreateToolPaletteGroup(toolPaletteGroupName);
        }

        // To demonstrate removal of a palette and a group 
        [CommandMethod("Demo4", CommandFlags.NoHistory)]
        public void Demo4()
        {
            Document activeDoc = Application.DocumentManager.MdiActiveDocument;
            Editor ed = activeDoc.Editor;

            String toolPaletteGroupName = "MyTPGroup";

            ToolPaletteWrapper.MyUtilities.GetInstance().RemoveToolPaletteGroup(toolPaletteGroupName);
        }
    }
}
