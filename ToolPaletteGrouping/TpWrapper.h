#ifndef __ToolPaletteWRAPPER_H__
#define __ToolPaletteWRAPPER_H__

#include "AcTc.h"
#include "AcTcUiToolPalette.h"
#include "AcTcUiToolPaletteGroup.h"
#include "AcTcUiToolPaletteSet.h"
#include "AcTcUiManager.h"


namespace ToolPaletteWrapper
{
	public ref class MyUtilities
	{

	public:

		/// Gets the singleton instance.
		static MyUtilities^ GetInstance()
		{
			if (mToolPaletteUtilities == nullptr)
				mToolPaletteUtilities = gcnew MyUtilities();

			return mToolPaletteUtilities;
		}

		void ActivateToolPalette(System::String^ toolPaletteName);
		void CreateToolPaletteGroup(System::String^ toolPaletteGroupName);
		void RemoveToolPaletteGroup(System::String^ toolPaletteGroupName);
		void Iterate();

	private:

		// Default constructor.
		MyUtilities();

		/// The instance
		static MyUtilities^ mToolPaletteUtilities;

		void DisplayName(CAcTcUiToolPaletteGroup* pTpgroup);
	};
}

#endif __ToolPaletteWRAPPER_H__