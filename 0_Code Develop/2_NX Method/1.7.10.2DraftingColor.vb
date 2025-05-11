Imports System
Imports NXOpen

Module Module123
    Sub Main(ByVal args() As String)

        Dim theSession As NXOpen.Session = NXOpen.Session.GetSession()
        Dim workPart As NXOpen.Part = theSession.Parts.Work

        Dim displayPart As NXOpen.Part = theSession.Parts.Display
		
		Dim sheet As Drawings.DrawingSheet

        Dim markId1 As NXOpen.Session.UndoMarkId = Nothing
        markId1 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Visible, "Edit Object Display")

		' Change Dimension Colour 1

        Dim displayModification1 As NXOpen.DisplayModification = Nothing
        displayModification1 = theSession.DisplayManager.NewDisplayModification()

        displayModification1.ApplyToAllFaces = True

        displayModification1.ApplyToOwningParts = False

        displayModification1.NewColor = 95

        displayModification1.Apply(workPart.Dimensions.ToArray)

        Dim nErrs1 As Integer = Nothing
        nErrs1 = theSession.UpdateManager.DoUpdate(markId1)

        displayModification1.Dispose()
        theSession.CleanUpFacetedFacesAndEdges()
		
		'Change Note Colour 2

 		'Dim displayModification2 As NXOpen.DisplayModification = Nothing
        'displayModification2 = theSession.DisplayManager.NewDisplayModification()

        'displayModification2.ApplyToAllFaces = True

        'displayModification2.ApplyToOwningParts = False

        'displayModification2.NewColor = 103

        'displayModification2.Apply(workpart.Notes.ToArray)

        'Dim nErrs2 As Integer = Nothing
        'nErrs2 = theSession.UpdateManager.DoUpdate(markId1)

        'displayModification2.Dispose()
        'theSession.CleanUpFacetedFacesAndEdges()

		' Change Centreline Colour 3

		Dim displayModification3 As NXOpen.DisplayModification = Nothing
        displayModification3 = theSession.DisplayManager.NewDisplayModification()

        displayModification3.ApplyToAllFaces = True

        displayModification3.ApplyToOwningParts = False

        displayModification3.NewColor = 108

        displayModification3.Apply(workpart.Annotations.Centerlines.ToArray)

        Dim nErrs3 As Integer = Nothing
        nErrs3 = theSession.UpdateManager.DoUpdate(markId1)

        displayModification3.Dispose()
        theSession.CleanUpFacetedFacesAndEdges()

		' Change Symbol Colour 4
		
		Dim displayModification4 As NXOpen.DisplayModification = Nothing
        displayModification4 = theSession.DisplayManager.NewDisplayModification()

        displayModification4.ApplyToAllFaces = True

        displayModification4.ApplyToOwningParts = False

        displayModification4.NewColor = 108

        displayModification4.Apply(workpart.Annotations.IdSymbols.ToArray)

        Dim nErrs4 As Integer = Nothing
        nErrs4 = theSession.UpdateManager.DoUpdate(markId1)

        displayModification4.Dispose()
        theSession.CleanUpFacetedFacesAndEdges()	
	
		' Change Section Lines Colour 5
	
		Dim displayModification5 As NXOpen.DisplayModification = Nothing
        displayModification5 = theSession.DisplayManager.NewDisplayModification()

        displayModification5.ApplyToAllFaces = True

        displayModification5.ApplyToOwningParts = False

        displayModification5.NewColor = 108

        displayModification5.Apply(workpart.Drafting.Sectionlines.ToArray)

        Dim nErrs5 As Integer = Nothing
        nErrs5 = theSession.UpdateManager.DoUpdate(markId1)

        displayModification5.Dispose()
        theSession.CleanUpFacetedFacesAndEdges()	
		
		' Change Surface Finish Symbol Colour 6		
		
		Dim displayModification6 As NXOpen.DisplayModification = Nothing
        displayModification6 = theSession.DisplayManager.NewDisplayModification()

        displayModification6.ApplyToAllFaces = True

        displayModification6.ApplyToOwningParts = False

        displayModification6.NewColor = 103

        displayModification6.Apply(workpart.Annotations.DraftingSurfaceFinishSymbols.ToArray)

        Dim nErrs6 As Integer = Nothing
        nErrs6 = theSession.UpdateManager.DoUpdate(markId1)

        displayModification6.Dispose()
        theSession.CleanUpFacetedFacesAndEdges()	
		
		' Change Label Colour 7
		
		Dim displayModification7 As NXOpen.DisplayModification = Nothing
        displayModification7 = theSession.DisplayManager.NewDisplayModification()

        displayModification7.ApplyToAllFaces = True

        displayModification7.ApplyToOwningParts = False

        displayModification7.NewColor = 103

        displayModification7.Apply(workpart.Labels.ToArray)

        Dim nErrs7 As Integer = Nothing
        nErrs7 = theSession.UpdateManager.DoUpdate(markId1)

        displayModification7.Dispose()
        theSession.CleanUpFacetedFacesAndEdges()	
		
		' Change GDT Colour 8
		
		Dim displayModification8 As NXOpen.DisplayModification = Nothing
        displayModification8 = theSession.DisplayManager.NewDisplayModification()

        displayModification8.ApplyToAllFaces = True

        displayModification8.ApplyToOwningParts = False

        displayModification8.NewColor = 103

        displayModification8.Apply(workpart.GDTs.ToArray)

        Dim nErrs8 As Integer = Nothing
        nErrs8 = theSession.UpdateManager.DoUpdate(markId1)

        displayModification8.Dispose()
        theSession.CleanUpFacetedFacesAndEdges()
		
		' Change Projected View Arrows Colour 9
		
		'Dim displayModification9 As NXOpen.DisplayModification = Nothing
        'displayModification9 = theSession.DisplayManager.NewDisplayModification()

        'displayModification9.ApplyToAllFaces = True

       ' displayModification9.ApplyToOwningParts = False

        'displayModification9.NewColor = 108

        'displayModification9.Apply(workpart.ViewingDirectionArrow.ToArray)

        'Dim nErrs9 As Integer = Nothing
        'nErrs9 = theSession.UpdateManager.DoUpdate(markId1)

        'displayModification9.Dispose()
        'theSession.CleanUpFacetedFacesAndEdges()
		
		' Update All Views
		Try
         theSession.Parts.Work.DraftingViews.UpdateViews(Drawings.DraftingViewCollection.ViewUpdateOption.All)
		 MsgBox("Updated All Views" , vbinformation + vbokonly, "Success")
		 Catch ex As exception
                        MsgBox("No Views updated" , vbinformation + vbokonly, "Error")
                        Exit Sub
 		End Try
		
    End Sub
End Module 