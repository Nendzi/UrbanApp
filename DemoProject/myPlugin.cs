// (C) Copyright 2012 by  
//
using System;
using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.EditorInput;

// This line is not mandatory, but improves loading performances
[assembly: ExtensionApplication(typeof(DemoProject.MyPlugin))]

namespace DemoProject
{
    public class MyPlugin : IExtensionApplication
    {

        void IExtensionApplication.Initialize()
        {
        }

        void IExtensionApplication.Terminate()
        {
        }

    }

}
