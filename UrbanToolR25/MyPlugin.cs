
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using Autodesk.AutoCAD.Runtime;

[assembly: ExtensionApplication(typeof(UrbanToolR25.MyPlugin))]
namespace UrbanToolR25
{
	public class MyPlugin : IExtensionApplication
	{		
		public void Initialize()
		{
			GetPalettesFromVault getPalettesFromVault = new GetPalettesFromVault();
		}	

		public void Terminate()
		{
			
		}
	}
}
