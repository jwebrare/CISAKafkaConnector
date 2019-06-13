Option Strict Off
Option Explicit On
Public Module Gn32

    'TITLE;
    'GNet include file for Microsoft Basic (7.1 or Visual)

    '** MICROHARD 1992-1996 **

    'UPDATE LOG:

    '       $RCSfile: gnwin.bi $       $Revision: 1.1 $

    'DATE           PERSON          COMMENTS
    '-------------------------------------------------------------
    '
    ' 28/07/97      Microhard       Prima versione per VISUAL BASIC
    '-------------------------------------------------------------

    ' Physical Address:
    Structure pa
        Dim brch As Byte ' Branch ID
        Dim gnode As Byte ' GNode ID
        Dim funct As Byte ' Function ID
    End Structure

    ' GNode Physical Address:
    Structure GPA
        Dim brch As Byte ' Integer        ' Branch ID
        Dim gnode As Byte ' Integer        ' GNode ID
    End Structure

    ' Logical Address:
    Structure LA
        <VBFixedString(8), System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.ByValTStr, SizeConst:=8)> Public ltype As String ' Logical Type
        Dim lnumber As Short ' Logical Number
        <VBFixedString(8), System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.ByValTStr, SizeConst:=8)> Public mfunct As String ' Mnemonic Function ID
    End Structure

    ' GNode Logical Address:
    Structure GLA
        <VBFixedString(8), System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.ByValTStr, SizeConst:=8)> Public ltype As String ' Logical Type
        Dim lnumber As Short ' Logical Number
    End Structure

    ' brch values:
    Public Const cBrchHost As Short = &HF0S ' Host  Address
    Public Const cBrchNil As Short = &HFFS ' Null Branch
    Public Const cBrchSetup As Short = &HFFS ' Terminal Setup Branch
    Public Const cNMaxBrch As Short = 240

    ' gNode values:
    Public Const cGateMaster As Short = 0 ' Gate Master Address
    Public Const cGateSetup As Short = &HFDS ' Gate  Setup Address
    Public Const cGNodeSetup As Short = &HFFS ' Terminal Setup Node
    Public Const cGNodeNil As Short = &HFFS ' Null GNode

    Public Const cNMaxGNode As Short = 240 ' max num. of available GNodes
    Public Const cNMaxHost As Short = 240 ' max num. of available Hosts
    Public Const cNMaxNode As Short = 240 ' max num. of available Nodes

    ' funct values:
    Public Const cFunctSysN As Short = &HF0S ' Node System Function
    Public Const cFunctSysH As Short = &HF1S ' Host System Function
    Public Const cFunctScpH As Short = &HF2S ' Host GNScope Function
    Public Const cFunctNil As Short = &HFFS ' Null Function
    Public Const cNMaxFunctUsr As Short = 224 ' max num. of available functs

    Public Const cNMaxCFunct As Short = 8 ' max num. of connected functs

    ' ltype values:
    Public Const cLTypeNil As String = "        " ' Null Logical Type

    ' mfunct values:
    Public Const cMFunctSysN As String = "MHFSYSTN" ' Node System Function
    Public Const cMFunctSysH As String = "MHFSYSTH" ' Host System Function
    Public Const cMFunctScpH As String = "MHFSCOPH" ' Host GNScope Function
    Public Const cMFunctNil As String = "        " ' Null Function

    ' Calls return codes:
    Public Const GN_SUCCESS As Short = 0 ' call executed successfully
    Public Const GN_FAIL As Short = -1 ' call failed (see GnErrNo)
    Public Const GN_NULLRES As Short = 1 ' call with null result

    ' BGNErrNo return values:
    Public Const cGNENotStarted As Short = 100 ' GNet not started
    Public Const cGNERcvFull As Short = 101 ' rcv file full
    Public Const cGNENotConnect As Short = 102 ' Function not connected
    Public Const cGNEConnect As Short = 103 ' Function connected
    Public Const cGNEMNetIO As Short = 104 ' MicroNet I/O error
    Public Const cGNEBadDest As Short = 105 ' bad packet destination
    Public Const cGNENoLogFile As Short = 106 ' no log file
    Public Const cGNEBadArg As Short = 107 ' bad Call argument
    Public Const cGNERepTmout As Short = 108 ' wait Reply timeout
    Public Const cGNEReply As Short = 109 ' Reply with error code
    Public Const cGNENotOpenCnf As Short = 110 ' config file not open
    Public Const cGNEOpenCnf As Short = 111 ' config file open
    Public Const cGNENoCnfFile As Short = 112 ' no config file
    Public Const cGNEBadCnf As Short = 113 ' bad config file
    Public Const cGNENoInit As Short = 114 ' no init file
    Public Const cGNENoFunct As Short = 115 ' no funct available
    Public Const cGNEBadFunct As Short = 116 ' bad function
    Public Const cGNEBadGPA As Short = 117 ' bad GNode Physical Address
    Public Const cGNEBadGLA As Short = 118 ' bad GNode Logical Address
    Public Const cGNENoGLA As Short = 119 ' no GNode Logical Address
    Public Const cGNEStarted As Short = 120 ' GNet started
    Public Const cGNENoRcvFile As Short = 121 ' no rcv file
    Public Const cGNENoMPorts As Short = 122 ' no mports file
    Public Const cGNEConfig As Short = 123 ' config is working
    Public Const cGNENoConfig As Short = 124 ' config is not working
    Public Const cGNECnfServer As Short = 125 ' working as config server
    Public Const cGNECnfClient As Short = 126 ' working as config client
    Public Const cGNENoProgram As Short = 127 ' no program to download
    Public Const cGNEInactive As Short = 128 ' active flag is off
    Public Const cGNECnfTmout As Short = 129 ' configuration timeout
    Public Const cGNENoRoute As Short = 130 ' no route to destination
    Public Const cGNENoSndFile As Short = 131 ' no snd file
    Public Const cGNESndFull As Short = 132 ' snd file full
    Public Const cGNENoConnects As Short = 133 ' no more connections
    Public Const cGNEClientTmout As Short = 134 ' client service timeout
    Public Const cGNENoFeature As Short = 135 ' feature not available

    ' cnfreq (BGnPutCnf argument) values:
    Public Const cGNCnfSort As Short = 1 ' sort config file
    Public Const cGNCnfHInit As Short = 2 ' config Host init parameters
    Public Const cGNCnfHFunc As Short = 3 ' config Host Function table
    Public Const cGNCnfHGLAS As Short = 4 ' config Host System GLA table
    Public Const cGNCnfHGLAU As Short = 5 ' config Host User GLA table
    Public Const cGNCnfHPort As Short = 6 ' config Host MPorts
    Public Const cGNCnfHRout As Short = 7 ' config Host Routing

    Public Const cGNCnfHFAct As Short = 20 ' config Host active Functions
    Public Const cGNCnfHPAct As Short = 21 ' config Host active MPorts

    Public Const cGNCnfMMast As Short = 30 ' config MNet Master params
    Public Const cGNCnfMActi As Short = 31 ' config MNet active Nodes
    Public Const cGNCnfMSlav As Short = 32 ' config MNet Slave params

    Public Const cGNCnfNInit As Short = 40 ' config Node init parameters
    Public Const cGNCnfNRset As Short = 41 ' reset Node
    Public Const cGNCnfNDwnl As Short = 42 ' download Node program
    Public Const cGNCnfNFunc As Short = 43 ' config Node Functions

    ' flags (BGNRcvPck(P/L) argument) values:
    Public Const cPckType As Short = &HE0S ' packet type
    Public Const cPckHiPri As Short = &H80S ' high priority

    Public Const cDat As Short = &H0S ' DATA
    Public Const cCtr As Short = &H40S ' CONTROL
    Public Const cInf As Short = &H80S ' INFORMATION
    Public Const cCmd As Short = &HC0S ' COMMAND
    Public Const cRep As Short = &HE0S ' REPLY

    Public Const cSzGNPck As Short = 255 ' max GNet packet size
    Public Const cSzGNHeader As Short = 10 ' GNet header size

    ' max length User buffer:
    Public Const cSzUsrBuf As Short = 245 ' cSzGNPck - cSzGNHeader

    ' Reply status codes:
    Public Const cGNRCSuccess As Short = 0 ' CMD executed successfully
    Public Const cGNRCUser As Short = 128 ' User return codes limit

    ' GNet Event:
    Const SZ_EVBUF As Short = 15
    Structure ev
        'UPGRADE_NOTE: year was upgraded to year_Renamed. Click for more: 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="vbup1061"'
        Dim year_Renamed As Byte ' year (0 = 1900)
        Dim mon As Byte ' month - [0,11]
        Dim mday As Byte ' day - [1,31]
        'UPGRADE_NOTE: hour was upgraded to hour_Renamed. Click for more: 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="vbup1061"'
        Dim hour_Renamed As Byte ' hour - [0,23]
        Dim min As Byte ' minute - [0,59]
        Dim sec As Byte ' second - [0,59]
        Dim host As Byte ' Host Master ID
        Dim src As pa ' source Physical Address
        Dim flags As Byte ' flags
        Dim obj As pa ' object Physical Address
        'UPGRADE_NOTE: event was upgraded to event_Renamed. Click for more: 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="vbup1061"'
        Dim event_Renamed As Short ' Event code
        Dim lbuf As Byte ' real optional buffer length
        <VBFixedString(SZ_EVBUF), System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.ByValTStr, SizeConst:=SZ_EVBUF)> Public buf As String ' optional buffer
    End Structure

    ' Event flags values:
    Public Const cEvLoPri As Short = &H0S ' low priority Event
    Public Const cEvHiPri As Short = &H1S ' high priority Event

    ' GNet Events codes:
    Public Const cGNEvCheck As Short = 5 ' Illegal operation
    Public Const cGNEvNoLink As Short = 8 ' No communication link
    Public Const cGNEvFunNoCnf As Short = 64 ' Function not configured
    Public Const cGNEvFunNoCon As Short = 65 ' Function not connected
    Public Const cGNEvFunBusy As Short = 66 ' Function busy
    Public Const cGNEvEnterStp As Short = 67 ' Node enter setup mode
    Public Const cGNEvNoRoute As Short = 70 ' No route to destination
    Public Const cGNEvNodeOnL As Short = 128 ' Node gets on-line
    Public Const cGNEvNodeOffL As Short = 129 ' Node gets off-line
    Public Const cGNEvNoGate As Short = 130 ' Gate not accessible
    Public Const cGNEvGNStart As Short = 131 ' GNet start
    Public Const cGNEvNoRcvFil As Short = 132 ' rcv file not buildable
    Public Const cGNEvGNStop As Short = 133 ' GNet stop
    Public Const cGNEvBadPck As Short = 134 ' Bad packet received
    Public Const cGNEvBuildLog As Short = 135 ' log file rebuild
    Public Const cGNEvBuildRcv As Short = 136 ' rcv file rebuild
    Public Const cGNEvGNCFunct As Short = 137 ' Function connected
    Public Const cGNEvGNDFunct As Short = 138 ' Function disconnected
    Public Const cGNEvCnf As Short = 139 ' Configurations
    Public Const cGNEvCnfDone As Short = 200 ' Configuration executed
    Public Const cGNEvCnfBad As Short = 201 ' bad config file
    Public Const cGNEvCnfFail As Short = 202 ' Configuration failed
    Public Const cGNEvMemFail As Short = 203 ' memory allocation failed
    Public Const cGNEvErrOpen As Short = 204 ' open system call failed
    Public Const cGNEvErrRead As Short = 205 ' read system call failed
    Public Const cGNEvErrWrite As Short = 206 ' write system call failed
    Public Const cGNEvErrRMPor As Short = 207 ' MPort read failed
    Public Const cGNEvErrWMPor As Short = 208 ' MPort write failed
    Public Const cGNEvNoSndFil As Short = 209 ' snd file not buildable
    Public Const cGNEvHwSTmout As Short = 210 ' hardware setup timeout
    Public Const cGNEvCnfTmout As Short = 211 ' node configuration timeout
    Public Const cGNEvZCTmout As Short = 212 ' zero code timeout
    Public Const cGNEvRepTmout As Short = 213 ' wait Reply timeout
    Public Const cGNEvGateBusy As Short = 214 ' Gate busy  timeout
    Public Const cGNEvBuildSnd As Short = 215 ' snd file rebuild
    Public Const cGNEvBadReply As Short = 216 ' Reply without Cmd or Ctr
    Public Const cGNEvExitStp As Short = 217 ' Node exit setup mode
    Public Const cGNEvNoModem As Short = 218 ' Modem not usable
    Public Const cGNEvMdemNoC1 As Short = 219 'Modem not connected - 1
    Public Const cGNEvMdemNoC2 As Short = 220 'Modem not connected - 2
    Public Const cGNEvMdemNoC3 As Short = 221 'Modem not connected - 3
    Public Const cGNEvMdemNoC4 As Short = 222 'Modem not connected - 4.







    ' Sizes of string arguments
    Public Const cSzMFunct As Short = 8 ' Mnemonic Function ID
    Public Const cSzAscGPA As Short = 9 ' GNode Physical Address
    Public Const cSzAscPA As Short = 13 ' Physical Address
    Public Const cSzAscGLA As Short = 14 ' GNode Logical Address
    Public Const cSzAscLA As Short = 23 ' Logical Address
    Public Const cSzAscEvP As Short = 72 ' Event: physically represented
    Public Const cSzAscEvL As Short = 96 ' Event: logically represented
    Public Const cSzBuf As Short = 245 ' GNet receive/transmit buffer
    Public Const cSzErrDesc As Short = 40 ' GNet error description

    ' Type
    Public Const cLTypeGate As String = "GATE____"
    Public Const cLTypeHost As String = "HOST____"
    Public Const cLTypeNode As String = "NODE____"

    ' Function

    'Declare Function GNConnectSys 	Lib "libgn32" (ByVal functUsr As Byte) As Integer
    'Declare Function GNLoadGLA 	Lib "libgn32" (ByVal ltype As String, lnumber%, GLA As GLA) As Integer
    'Declare Function GNLoadLA 	Lib "libgn32" (ByVal ltype As String, lnumber%, mfunct$, LA As LA) As Integer
    'Declare Function GNLoadPA 	Lib "libgn32" (brch%, gnode%, funct%, PA As PA) As Integer
    Declare Function BGNStrError Lib "libgn32" (ByVal gnerrnum As Short, ByVal serror As String) As Short
    Declare Function GNCloseCnf Lib "libgn32" () As Short
    Declare Function GNConnect Lib "libgn32" (ByVal functUsr As Byte, ByVal flagsConn As Integer) As Short
    Declare Function GNConnectM Lib "libgn32" (ByVal mfunctUsr As String, ByVal flagsConn As Integer) As Short
    Declare Function GNConnectSys Lib "libgn32" (ByVal functUsr As Byte, ByVal flagsConn As Integer) As Short
    Declare Function GNCvAscGLA Lib "libgn32" (ByVal ascGla As String, ByRef GLA As GLA) As Short
    Declare Function GNCvAscGPA Lib "libgn32" (ByVal ascGPA As String, ByRef GPA As GPA) As Short
    Declare Function GNCvBcrhPort Lib "libgn32" (ByVal iBrch As Short, ByRef iPort As Short) As Short
    Declare Function GNCvEventAscL Lib "libgn32" (ByRef evLog As ev, ByVal ascevl As String) As Short
    Declare Function GNCvEventAscP Lib "libgn32" (ByRef evLog As ev, ByVal ascevl As String) As Short
    Declare Function GNCvGLAAsc Lib "libgn32" (ByRef GLA As GLA, ByVal ascGla As String) As Short
    Declare Function GNCvGPAAsc Lib "libgn32" (ByRef GPA As GPA, ByVal ascGPA As String) As Short
    Declare Function GNCvGPAGLA Lib "libgn32" (ByVal ifunct As Short, ByRef GPA As GPA, ByRef GLA As GLA) As Short
    Declare Function GNCvLAAsc Lib "libgn32" (ByRef LA As LA, ByVal ascla As String) As Short
    Declare Function GNCvMFunct Lib "libgn32" (ByVal mfunct As String, ByRef ifunct As Short) As Short
    Declare Function GNCvMGPAGLA Lib "libgn32" (ByVal mfunct As String, ByRef GPA As GPA, ByRef GLA As GLA) As Short
    Declare Function GNDisconnect Lib "libgn32" () As Short
    Declare Function GNErrNo Lib "libgn32" Alias "GNLoadErrNo" () As Short
    Declare Function GNExit Lib "libgn32" () As Short
    Declare Function GNGetCnfPosition Lib "libgn32" () As Integer
    Declare Function GNGetCnfRes Lib "libgn32" () As Short
    Declare Function GNGetEvent Lib "libgn32" (ByRef evLog As ev) As Short
    Declare Function GNLoadErrNo Lib "libgn32" () As Short
    Declare Function GNMonitor Lib "libgn32" () As Short
    Declare Function GNNoHangUp Lib "libgn32" () As Short
    Declare Function GNOpenCnf Lib "libgn32" (ByVal flagsCnf As Short) As Short
    Declare Function GNPortStart Lib "libgn32" (ByVal brch As Byte) As Short
    Declare Function GNPortStop Lib "libgn32" (ByVal brch As Byte) As Short
    Declare Function GNPutCnf Lib "libgn32" (ByVal cnfreq As Short, ByVal cnfcnt As Short, ByRef glaCnf As Long) As Short
    Declare Function GNRcvPckP Lib "libgn32" (ByRef flags As Short, ByRef pa As pa, ByRef lbuf As Byte, ByVal buf As String) As Short
    Declare Function GNRcvPckL Lib "libgn32" (ByRef flags As Short, ByRef LA As LA, ByRef plbuf As Byte, ByVal buf As String) As Short
    Declare Function GNReadCnf Lib "libgn32" (ByVal skey As String, ByVal smask As String, ByVal xmask As String, ByVal strCnf As String) As Short
    Declare Function GNSearchCnf Lib "libgn32" (ByVal skey As String) As Short
    Declare Function GNSndCmdL Lib "libgn32" (ByRef LA As LA, ByRef plbuf As Byte, ByVal buf As String) As Short
    Declare Function GNSndCmdP Lib "libgn32" (ByRef pa As pa, ByRef plbuf As Byte, ByVal buf As String) As Short
    Declare Function GNSndCtrL Lib "libgn32" (ByRef LA As LA, ByRef plbuf As Byte, ByVal buf As String) As Short
    Declare Function GNSndCtrP Lib "libgn32" (ByRef pa As pa, ByRef plbuf As Byte, ByVal buf As String) As Short
    Declare Function GNSndDatL Lib "libgn32" (ByRef LA As LA, ByVal lbuf As Byte, ByVal buf As String) As Short
    Declare Function GNSndDatP Lib "libgn32" (ByRef pa As pa, ByVal lbuf As Byte, ByVal buf As String) As Short
    Declare Function GNSndInfL Lib "libgn32" (ByRef LA As LA, ByVal lbuf As Byte, ByVal buf As String) As Short
    Declare Function GNSndInfP Lib "libgn32" (ByRef pa As pa, ByVal lbuf As Byte, ByVal buf As String) As Short
    Declare Function GNSndPckL Lib "libgn32" (ByVal flagsGN As Short, ByRef LA As LA, ByVal lbuf As Byte, ByVal buf As String) As Short
    Declare Function GNSndPckP Lib "libgn32" (ByVal flagsGN As Short, ByRef pa As pa, ByVal lbuf As Byte, ByVal buf As String) As Short
    Declare Function GNSndRepL Lib "libgn32" (ByRef LA As LA, ByVal lbuf As Byte, ByVal buf As String) As Short
    Declare Function GNSndRepP Lib "libgn32" (ByRef pa As pa, ByVal lbuf As Byte, ByVal buf As String) As Short
    Declare Function GNSort Lib "libgn32" (ByVal FileSort As String, ByVal Colonna As String) As Short
    Declare Function GNStart Lib "libgn32" () As Short
    Declare Function GNStop Lib "libgn32" () As Short

    ' flagsCnf (GnOpenCnf argument) values:
    Public Const cFCnfServer As Short = 0 ' config server
    Public Const cFCnfClient As Short = 1 ' config client

End Module