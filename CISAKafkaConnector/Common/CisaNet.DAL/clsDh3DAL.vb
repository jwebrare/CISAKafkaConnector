Imports CisaNet.Entity

Public Class clsDh3DAL
    Public Function GetItems(ByVal oCodepass As Double) As clsGuestRoom
        Dim oItems As clsGuestRoom = Nothing

        Dim DH3DataContext As New DH3ClassesDataContext

       
        Dim pIdService = Left(oCodepass, 1)
        Dim pCode = Mid(oCodepass, 2, 3)

        'Dim QryBadgeInfoRommBadge = From G In DH3DataContext.Guest
        '                  Join ginfo In DH3DataContext.ROOM On G.Id_Room Equals ginfo.IDRoom
        '                  Where G.CodePass = oCodepass
        '                  Order By G.Id_Room
        '                  Select G.IDGuest, G.Name, G.IndxCamera, G.NrBadge, ginfo.IDRoom, ginfo.DepDate, ginfo.ArrivalDate, ginfo.BadgeType, ginfo.Numero, ginfo.CheckState, ginfo.SuiteState

        'Dim oInfo As New clsGuestRoom
        'For Each itm In QryBadgeInfoRommBadge

        '    ' Dim oInfo As New clsGuestRoom With {.IdGuest = itm.IDGuest, .NameGuest = itm.Name, .IDRoom = itm.Id_Room, .NumeroRoom = itm.Numero.ToString, .CodePass = itm.CodePass, .CheckState = itm.CheckState, .SuiteState = itm.SuiteState, .ArrivalDate = itm.ArrivalDate, .DepDate = itm.DepDate, .TipoGuest = 0, .BadgeType = itm.BadgeType, .NrBadge = itm.NrBadge, .IndxCamera = itm.IndxCamera}
        '    oInfo = New clsGuestRoom With {.IdGuest = itm.IDGuest, .NameGuest = itm.Name, .IDRoom = itm.IDRoom, .NumeroRoom = itm.Numero.ToString, .CodePass = oCodepass, .CheckState = itm.CheckState, .SuiteState = .SuiteState, .ArrivalDate = itm.ArrivalDate, .DepDate = itm.DepDate, .BadgeType = 0, .NrBadge = itm.NrBadge, .IndxCamera = itm.IndxCamera}
        'Next

        'Dim QryBadgeInfoCommonArea = From R In DH3DataContext.ROOM
        '                 Join CA In DH3DataContext.CommonAreas On CA.IDRoom Equals R.IDRoom
        '                 Where CA.IDGuest = oInfo.IdGuest
        '                 Select CA.IDCommArea

        'For Each itm In QryBadgeInfoCommonArea
        '    oInfo.IdCommonArea = itm
        'Next



        Dim oInfo As clsGuestRoom
        Dim QryBadgeInfo = From G In DH3DataContext.Guest
                      Join ginfo In DH3DataContext.ROOM On G.Id_Room Equals ginfo.IDRoom
                      Where G.CodePass = oCodepass
                      Order By G.Id_Room
                      Select G.IDGuest, G.Name, G.Id_Room, ginfo.Numero, G.CodePass, ginfo.CheckState, ginfo.SuiteState, ginfo.ArrivalDate, ginfo.DepDate, ginfo.BadgeType, G.NrBadge, G.IndxCamera

        For Each itm In QryBadgeInfo
            oInfo = New clsGuestRoom With {.IdGuest = itm.IDGuest, .NameGuest = itm.Name, .IDRoom = itm.Id_Room, .NumeroRoom = itm.Numero.ToString, .CodePass = itm.CodePass, .CheckState = itm.CheckState, .SuiteState = itm.SuiteState, .ArrivalDate = itm.ArrivalDate, .DepDate = itm.DepDate, .TipoGuest = 0, .BadgeType = itm.BadgeType, .NrBadge = itm.NrBadge, .IndxCamera = itm.IndxCamera}
        Next

        If (QryBadgeInfo.Count <> 0) Then

            Dim QryBadgeInfoCommonArea = From R In DH3DataContext.ROOM
                         Join CA In DH3DataContext.CommonAreas On CA.IDRoom Equals R.IDRoom
                         Where CA.IDGuest = oInfo.IdGuest
                         Select CA.IDCommArea

            For Each itm In QryBadgeInfoCommonArea
                oInfo.IdCommonArea = itm
            Next

            oItems = oInfo
        End If

        If (QryBadgeInfo.Count.Equals(0)) Then

            Dim QryBadgeInfoPersonal = From ginfo In DH3DataContext.PersonalGroup
                                       Where (ginfo.IDService = pIdService) And (ginfo.Code = pCode)
                                       Select ginfo.IDPersonalGroup, ginfo.IDService, ginfo.Name, ginfo.Code

            For Each itm In QryBadgeInfoPersonal

                oInfo = New clsGuestRoom With {.IdGuest = itm.IDPersonalGroup, .CodePass = itm.Code, .NameGuest = itm.Name, .NumeroRoom = itm.Name, .CheckState = False, .ArrivalDate = DateTime.Now, .DepDate = DateTime.Now, .TipoGuest = 1, .IndxCamera = 1, .NrBadge = 1, .BadgeType = 4}
                oItems = oInfo
            Next
        End If

        Return oItems
    End Function

End Class
