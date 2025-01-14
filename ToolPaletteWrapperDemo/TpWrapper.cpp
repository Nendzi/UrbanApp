// This is the main DLL file.

#include "stdafx.h"

#include "TpWrapper.h"


#include "mgdinterop.h"
#include <vcclr.h>

using namespace ToolPaletteWrapper;
using namespace System;


// Default constructor
MyUtilities::MyUtilities()
{
}

void MyUtilities::ActivateToolPalette(System::String^ toolPaletteName)
{
	CAcTcUiToolPalette* pTp;
	CAcTcUiToolPaletteGroup* pTpGroup = AcTcUiGetToolPaletteWindow()->GetActivePaletteGroup();

	for (int i = 0; i < pTpGroup->GetItemCount(); i++)
	{
		pTpGroup->GetItem(i, pTp);
		if (toolPaletteName == pTp->GetName())
		{
			pTpGroup->SetActivePalette(i);
			break;
		}
	}
}

void MyUtilities::Iterate()
{
	CAcTcUiToolPaletteSet *pTpset = AcTcUiGetToolPaletteWindow();
    
	// Root PaletteGroup. Can be iterated to find the palette groups and their palettes
	CAcTcUiToolPaletteGroup *pRootTpgroup = pTpset->GetToolPaletteGroup(false);
	DisplayName(pRootTpgroup);

	// Only Palettes
	//CAcTcUiToolPaletteGroup *pAllPalettesGroup = pTpset->GetAllPalettesGroup();
	//DisplayName(pRootTpgroup);
}

void MyUtilities::DisplayName(CAcTcUiToolPaletteGroup *pTpgroup)
{
	int iCount = pTpgroup->GetItemCount();
	for(int i = 0; i<iCount; i++)
	{
		if(pTpgroup->IsItemPalette(i))
		{
			CAcTcUiToolPalette *pPalette = NULL;
			pTpgroup->GetItem(i, pPalette); 
			CString paletteName = pPalette->GetName();
			::MessageBox(NULL, paletteName, L"Palette", MB_OK);
		}
		else
		{
			CAcTcUiToolPaletteGroup *pTpSubGroup = NULL;
			pTpgroup->GetItem(i, pTpSubGroup); 
			CString groupName = pTpSubGroup->GetName();
			::MessageBox(NULL, groupName, L"Palette Group", MB_OK);

			DisplayName(pTpSubGroup);
		}
	}
}

void MyUtilities::CreateToolPaletteGroup(System::String^ toolPaletteGroupName)
{
	CAcTcUiToolPaletteSet *pTpset = AcTcUiGetToolPaletteWindow(); 

	CAcTcUiToolPaletteGroup *pRootTpgroup = pTpset->GetToolPaletteGroup(FALSE);

	CAcTcUiToolPaletteGroup *pMyToolPaletteGroup = NULL;
	pMyToolPaletteGroup = pRootTpgroup->FindGroup(StringToCIF(toolPaletteGroupName), TRUE);
	if(pMyToolPaletteGroup == NULL)
	{ // Does not exist, lets create it

		// Create a new Palette Group
		CAcTcUiToolPaletteGroup * pMyToolPaletteGroup = NULL;
		pRootTpgroup->Clone(pMyToolPaletteGroup);
		pMyToolPaletteGroup->Reset();
		pMyToolPaletteGroup->SetName(StringToCIF(toolPaletteGroupName));

		// Any Palette can be found in the "All Palettes group"
		CAcTcUiToolPaletteGroup *pAllPalettesGroup = pTpset->GetAllPalettesGroup();
		CAcTcUiToolPalette *pMechanicalPalette = pAllPalettesGroup->FindPalette(ACRX_T("Mechanical"), NULL);

		// Add it to the palette group
		pMyToolPaletteGroup->AddItem(pMechanicalPalette);

		// Create a new palette and add it to the palette group
		CAcTcUiManager *pToolPaletteManager = AcTcUiGetManager();
		CAcTcUiToolPalette *pMyToolPalette = pToolPaletteManager->CreatePalette(ACRX_T("MyPalette"));
		pMyToolPaletteGroup->AddItem(pMyToolPalette);

		pRootTpgroup->AddItem(pMyToolPaletteGroup);
	}

	// Set the newly create Palette group as active
	pTpset->SetActivePaletteGroup(pMyToolPaletteGroup);
}

void MyUtilities::RemoveToolPaletteGroup(System::String^ toolPaletteGroupName)
{
	CAcTcUiToolPaletteSet *pTpset = AcTcUiGetToolPaletteWindow(); 

	CAcTcUiToolPaletteGroup *pActivePaletteGroup = pTpset->GetActivePaletteGroup();
	CString activePaletteGroupName = pActivePaletteGroup->GetName();

	if(activePaletteGroupName == StringToCIF(toolPaletteGroupName))
	{// Set All palettes as the active group before we mess around with our tool palette group :)
		CAcTcUiToolPaletteGroup *pAllPalettesGroup = pTpset->GetAllPalettesGroup();
		pTpset->SetActivePaletteGroup(pAllPalettesGroup);
	}

	CAcTcUiToolPaletteGroup *pRootTpgroup = pTpset->GetToolPaletteGroup(FALSE);

	CAcTcUiToolPaletteGroup * pMyToolPaletteGroup = pRootTpgroup->FindGroup(StringToCIF(toolPaletteGroupName), TRUE);
	if(pMyToolPaletteGroup != NULL)
	{ // Lets remove the group
	
		// Remove all sub groups and palettes from this palette group
		pMyToolPaletteGroup->DeleteAllItems();

		// Other options...
		//pMyToolPaletteGrp->DeleteAllPalettes();

		//CAcTcUiToolPalette *pPal1 = pMyToolPaletteGrp->FindPalette(ACRX_T("MyPalette"), NULL);
		//if(pPal1 != NULL)
		//{
		//	pMyToolPaletteGrp->RemoveItem(pPal1, TRUE);
		//}

		pRootTpgroup->RemoveItem(pMyToolPaletteGroup);

		delete pMyToolPaletteGroup;
	}

	CAcTcUiToolPaletteGroup *pAllPalettesGroup = pTpset->GetAllPalettesGroup();
	CAcTcUiToolPalette * pMyToolPalette = pAllPalettesGroup->FindPalette(ACRX_T("MyPalette"), NULL, TRUE);
	if(pMyToolPalette != NULL)
	{ // remove the palette from the palette group
		pAllPalettesGroup->RemoveItem(pMyToolPalette, TRUE);

		delete pMyToolPalette;
	}
}