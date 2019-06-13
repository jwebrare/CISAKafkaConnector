Option Strict Off
Option Explicit On
Imports System.Runtime.InteropServices

Public Module Csemks32
    '
    'TITLE;
    'CSEMKS global declarations
    '
    '** CISA 1998-2003 **
    '
    'UPDATE LOG:
    '
    '
    'DATE           PERSON          COMMENTS
    '_____________________________________________________________
    '
    '30/10/98	G. Fano		Release 1.1
    '13/05/99	G. Fano		Release 1.3
    '08/01/00	G. Fano		Release 2.1
    '11/06/01	L. Fano		Release 2.2
    '27/03/03	G. Fano		Release 2.4
    '_____________________________________________________________
    '
    '
    ' CONSTANTS FOR MAX. NUMBERS AND SIZES:
    Public Const N_ACCESSTARGET As Short = 5 ' max. access targets per card
    Public Const N_TIMESHIFT As Short = 4 ' max. timeshifts per card
    Public Const N_UTILITY As Short = 128 ' max. utility octets per card
    '
    Public Const SZ_ACCESSID As Short = 16 ' card owner ID for access sys.
    Public Const SZ_ACCESSTNAME As Short = 6 ' access target name
    Public Const SZ_BUFCARD As Short = 221 ' card physical buffer
    Public Const SZ_PASSWD As Short = 8 ' operator's password
    Public Const SZ_LOCKMAP As Short = 128 ' utility locks access map
    Public Const SZ_BUFHCARD As Short = 17 ' card history physical buffer
    Public Const SZ_BUFHISTORY As Short = 15 ' history physical buffer
    '
    ' STRUCTURES:
    '
    Structure csdate
        Public year_Renamed As Byte
        Public month_Renamed As Byte
        Public day_Renamed As Byte
    End Structure
    '
    Structure cstime
        Public hours As Byte
        Public minutes As Byte
    End Structure
    '
    Structure mtimeshift
        Public start1 As Byte
        Public end1 As Byte
        Public start2 As Byte
        Public end2 As Byte
        Public days As Byte
    End Structure

    Structure accesstime
        Dim dateStart As csdate
        Dim dateEnd As csdate
        Dim timeStart As cstime
        Dim timeEnd As cstime

        <VBFixedArray(3)> Dim mtimeshift_Renamed() As mtimeshift
        Public Sub Initialize()
            ReDim mtimeshift_Renamed(3)

        End Sub


    End Structure

    '
    ' Verificare questa struttura dati in CISA
    'Structure accesstime
    '    Dim dateStart As csdate
    '    Dim dateEnd As csdate
    '    Dim timeStart As cstime
    '    Dim timeEnd As cstime
    '    <VBFixedArray(N_TIMESHIFT)> Dim mtimeshift_Renamed() As mtimeshift
    'End Structure
    '	
    Structure accesstarget
        Dim id As Short
        Public bed As Byte
    End Structure
    '
    '<StructLayout(LayoutKind.Explicit)>
    Structure cardinfo
        Public status As Short
        Public ncard As Short
        Public ncopy As Byte
        Public icopy As Byte
        Public fCancelled As Byte
        Public ncardOp As Short
        Public fPMS As Byte
        Public dateStartValid As csdate
        Public timeStartValid As cstime
        Public dateEndValid As csdate
        Public timeEndValid As cstime
        Public nCardToDo As Short
    End Structure
    '
    Structure cardsearch
        Public cardtype As Byte
        <VBFixedString(SZ_ACCESSID), System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.ByValTStr, SizeConst:=SZ_ACCESSID)> Public accessid As String
        Dim accesstarget_Renamed As accesstarget
        Dim ncard As Short
        Dim ncardOp As Short
        Public fPMS As Byte
        Public fActiveOnly As Byte
    End Structure
    '
    Structure operatorinfo
        <VBFixedString(SZ_PASSWD), System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.ByValTStr, SizeConst:=SZ_PASSWD)> Public passwd As String
        Public hierlev As Byte
    End Structure
    '
    '<StructLayout(LayoutKind.Sequential)>
    Public Structure card
        Public cardtype As Byte
        <VBFixedString(SZ_ACCESSID), System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.ByValTStr, SizeConst:=SZ_ACCESSID)> Public accessid As String
        Public cardinfo_Renamed As cardinfo
        Public accesstarget1 As accesstarget
        Public accesstarget2 As accesstarget
        Public accesstarget3 As accesstarget
        Public accesstarget4 As accesstarget
        Public accesstarget5 As accesstarget
        Public accesstime_Renamed As accesstime
        Public fOvrPrivacy As Byte
        Public fOffice As Byte
        Public operatorinfo_Renamed As operatorinfo
        Public lockoutmap As Short
        Public credits As Integer
        Public fCardHistory As Byte
        Public nhoursvalid As Short
    End Structure
    '	
    Structure operation
        Public nrecord As Short
        Public ncardOp As Short
        Public opcode As Byte
        Public ncard As Short
        Public ndevice As Byte
        Public fPMS As Byte
        Public dateOperation As csdate
        Public timeOperation As cstime
        Public dateStartCard As csdate
    End Structure
    '
    Structure operationsearch
        Dim ncardOp As Short
        Public opcode As Byte
        Dim ncard As Short
        Public ndevice As Byte
        Public devicetype As Byte
        Public fPMS As Byte
        Dim dateOperation As csdate
    End Structure
    '
    Structure LOCKPARAMS
        Public flags As Byte
        Public hierlev As Byte
        <VBFixedString(8), System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.ByValTStr, SizeConst:=8)> Public groupmap As String
        Dim hierprofiles As Short
    End Structure
    '
    Structure ZONEPARAMS
        Dim idFirst As Short
        Dim idLast As Short
        Public group As Byte
        Dim cardtypemap As Short
        Public hierlev As Byte
        Dim hierprofiles As Short
    End Structure
    '
    Structure categoryparams
        Dim cardtypemap As Short
        Public hierlev As Byte
        <VBFixedString(SZ_LOCKMAP), System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.ByValTStr, SizeConst:=SZ_LOCKMAP)> Public lockmap As String
        Dim hierprofiles As Short
    End Structure
    '
    ' CONSTANTS:
    '
    ' Calls return codes:
    Public Const CSE_SUCCESS As Short = 0 ' call executed successfully
    Public Const CSE_FAIL As Short = -1 ' call failed (see CSEErrNo)
    Public Const CSE_NULLRES As Short = 1 ' call with null result
    '
    ' flags values to be used in structures:
    Public Const F_FALSE As Short = 0 ' false
    Public Const F_TRUE As Short = 255 ' true
    Public Const F_ALL As Short = 128 ' ignore (only for searching)
    '
    ' card.cardtype values:
    Public Const CT_GUEST As Short = 0 ' Guest
    Public Const CT_STAFF1 As Short = 1 ' Staff Level 1
    Public Const CT_STAFF2 As Short = 2 ' Staff Level 2
    Public Const CT_STAFF3 As Short = 3 ' Staff Level 3
    Public Const CT_STAFF4 As Short = 4 ' Staff Level 4
    Public Const CT_EMG As Short = 5 ' Emergency
    Public Const CT_RDNG As Short = 6 ' Reversal Danger
    Public Const CT_DNG As Short = 7 ' Danger
    Public Const CT_OS As Short = 8 ' One Shot
    Public Const CT_LOCKOUT As Short = 12 ' Lockout
    Public Const CT_RLOCKOUT As Short = 13 ' Reversal Lockout
    Public Const CT_CLC As Short = 14 ' CLC
    Public Const CT_OP80 As Short = 31 ' Operator - level 80
    Public Const CT_OP As Short = 32 ' Operator
    Public Const CT_ALL As Short = 128 ' Any Type(used only by search)
    '
    ' card.cardinfo.status values:
    Public Const ST_ACTIVE As Short = 0 ' Active Card
    Public Const ST_FUTURE As Short = 9 ' Future Card (only Guest)
    Public Const ST_EXPIRED As Short = 8 ' Expired Card
    Public Const ST_CANCELLED As Short = 10 ' Cancelled Card
    Public Const ST_NOTFOUND As Short = 7 ' Card not found in memory(only the physical card data will be unpacked)
    Public Const ST_TODO As Short = 41 ' To Do Card
    '
    ' warning (CSEUnpackCard argument) values:
    Public Const W_NO As Short = 0 ' no warnings
    Public Const W_NOPRIVACY As Short = 12 ' Guest Active, but Not Override Privacy and encoder default is Override Privacy
    Public Const W_PRIVACY As Short = 20 ' Guest Active, but Override Privacy and encoder default is Not Override Privacy
    Public Const W_CANCEL As Short = 11 ' Cancel Card
    Public Const W_NOTFOUND As Short = 7 ' Card not found in memory(only the physical card data will be unpacked)
    '
    ' CSELoadErrNo return values:
    Public Const CSEE_GNETERROR As Short = 1 ' GNet error
    Public Const CSEE_NOTFOUND As Short = 2 ' item not found
    Public Const CSEE_ENCODERBUSY As Short = 3 ' encoder is busy
    Public Const CSEE_LOGGEDOUT As Short = 4 ' PC user is not logged in
    Public Const CSEE_BADFORMAT As Short = 5 ' bad data format
    Public Const CSEE_UNAUTHORIZED As Short = 6 ' unauthorized operation
    Public Const CSEE_WRONGPLANT As Short = 7 ' card is from another plant
    Public Const CSEE_TOOMANYCARDS As Short = 8 ' cards file is full
    Public Const CSEE_TOOMANYOPS As Short = 9 ' operators file is full
    Public Const CSEE_WARNINGBUSY As Short = 10 ' warning: access target is busy
    Public Const CSEE_WRITEFAIL As Short = 11 ' card writing/verifying error
    Public Const CSEE_TOOMANYCOPIES As Short = 12 ' too many copies of the card
    Public Const CSEE_BADSMART As Short = 13 ' unsupported smart card format
    Public Const CSEE_TOOMANYCATS As Short = 14 ' too many categories
    Public Const CSEE_DUPCATEGORY As Short = 15 ' duplicated category
    Public Const CSEE_ALREADYDONE As Short = 16 ' card to do already done
    Public Const CSEE_BADCARDTYPE As Short = 17 ' bad card type
    Public Const CSEE_BADARGUMENT As Short = 19 ' bad argument
    '
    ' operation.opcode values:
    Public Const EOP_DEVON As Short = 1 ' device on
    Public Const EOP_DEVOFF As Short = 2 ' device off
    Public Const EOP_LOGIN As Short = 3 ' login
    Public Const EOP_LOGOUT As Short = 4 ' logout
    Public Const EOP_COPYGST As Short = 5 ' guest card copy
    Public Const EOP_FUTGST As Short = 6 ' future guest card issue
    Public Const EOP_CINGST As Short = 7 ' checkin guest
    Public Const EOP_COUTGST As Short = 8 ' checkout guest
    Public Const EOP_DBOOT As Short = 9 ' switch  server/mirror
    Public Const EOP_ENMIR As Short = 10 ' enable mirroring
    Public Const EOP_DISMIR As Short = 11 ' disable mirroring
    Public Const EOP_REBUILD As Short = 12 ' database rebuild
    Public Const EOP_ERRVER As Short = 13 ' card verify error
    Public Const EOP_NEWOP As Short = 16 ' operator card issue
    Public Const EOP_NEWGST As Short = 32 ' guest card issue
    Public Const EOP_NEWSTF1 As Short = 33 ' staff 1 card issue
    Public Const EOP_NEWSTF2 As Short = 34 ' staff 2 card issue
    Public Const EOP_NEWSTF3 As Short = 35 ' staff 3 card issue
    Public Const EOP_NEWSTF4 As Short = 36 ' staff 4 card issue
    Public Const EOP_NEWEMG As Short = 37 ' emergency card issue
    Public Const EOP_NEWRDNG As Short = 38 ' reversal danger card issue
    Public Const EOP_NEWDNG As Short = 39 ' danger card issue
    Public Const EOP_NEWOS As Short = 40 ' one-shot card issue
    Public Const EOP_NEWINIT As Short = 41 ' initialization card issue
    Public Const EOP_NEWLCK As Short = 44 ' lockout card issue
    Public Const EOP_NEWRLCK As Short = 45 ' reversal lockout card issue
    Public Const EOP_NEWCLC As Short = 46 ' CLC card issue
    Public Const EOP_NULLOP As Short = 63 ' cancel operator card issue
    Public Const EOP_NULLGST As Short = 64 ' cancel guest card issue
    Public Const EOP_NULLS1 As Short = 65 ' cancel staff 1 card issue
    Public Const EOP_NULLS2 As Short = 66 ' cancel staff 2 card issue
    Public Const EOP_NULLS3 As Short = 67 ' cancel staff 3 card issue
    Public Const EOP_NULLS4 As Short = 68 ' cancel staff 4 card issue
    Public Const EOP_NULLEMG As Short = 69 ' cancel emergency card issue
    Public Const EOP_NEWSTF1TODO As Short = 70 ' Staff 1 To do issue
    Public Const EOP_NEWSTF2TODO As Short = 71 ' Staff 2 To do issue
    Public Const EOP_NEWSTF3TODO As Short = 72 ' Staff 3 To do issue
    Public Const EOP_NEWSTF4TODO As Short = 73 ' Staff 4 To do issue
    Public Const EOP_NEWOPTODO As Short = 74 ' Operator To do issue
    Public Const EOP_CHSTF1TODO As Short = 75 ' Change Staff 1 To do
    Public Const EOP_CHSTF2TODO As Short = 76 ' Change Staff 2 To do
    Public Const EOP_CHSTF3TODO As Short = 77 ' Change Staff 3 To do
    Public Const EOP_CHSTF4TODO As Short = 78 ' Change Staff 4 To do
    Public Const EOP_CHOPTODO As Short = 79 ' Change Operat. To do
    Public Const EOP_NULLSTF1TODO As Short = 80 ' Cancel Staff 1 To do
    Public Const EOP_NULLSTF2TODO As Short = 81 ' Cancel Staff 2 To do
    Public Const EOP_NULLSTF3TODO As Short = 82 ' Cancel Staff 3 To do
    Public Const EOP_NULLSTF4TODO As Short = 83 ' Cancel Staff 4 To do
    Public Const EOP_NULLOPTODO As Short = 84 ' Cancel Operat. To do
    Public Const EOP_RECODING As Short = 85 ' Recoding card
    Public Const EOP_TODOLINK As Short = 86 ' Link to card To do number
    Public Const EOP_MAINT As Short = 128 ' maintenance entered
    Public Const EOP_ALL As Short = 255 ' Any Op.(used only by search)
    '
    ' operation.devicetype values:
    Public Const DT_SERVER As Short = 0 ' Server Encoder
    Public Const DT_MIRROR As Short = 1 ' Mirror Encoder
    Public Const DT_CLIENT As Short = 2 ' Client Encoder
    Public Const DT_HHI As Short = 4 ' HHI
    Public Const DT_SLOCK As Short = 6 ' Special Lock
    Public Const DT_ALL As Short = 255 ' Any Type(used only by search)
    '
    ' accesstarget.id values:
    Public Const AT_NULLID As Short = 0
    Public Const AT_NULLLOCK As Short = 0
    Public Const AT_FIRSTLOCK As Short = 1
    Public Const AT_LASTLOCK As Short = 8191
    Public Const AT_NULLZONE As Short = 8192
    Public Const AT_FIRSTZONE As Short = 8193
    Public Const AT_LASTZONE As Short = 16383
    Public Const AT_NULLCAT As Short = 16384
    Public Const AT_FIRSTCAT As Short = 16385
    Public Const AT_LASTCAT As Short = 32767
    '

    Declare Function CSEBuffer2Card Lib "csemks32" (ByVal bufCard() As Byte, ByVal accesstname1 As String, ByVal accesstname2 As String, ByVal accesstname3 As String, ByVal accesstname4 As String, ByVal accesstname5 As String, ByRef card As card, ByVal warning As String) As Short
    Declare Function CSECard2Buffer Lib "csemks32" (ByRef card As card, ByVal accesstname1 As String, ByVal accesstname2 As String, ByVal accesstname3 As String, ByVal accesstname4 As String, ByVal accesstname5 As String, ByVal bufCard() As Byte) As Short
    Declare Function CSECheckOutCard Lib "csemks32" (ByRef card As card) As Short
    Declare Function CSECollectNextHCard Lib "csemks32" (ByRef gpaSpecLock As GPA, ByVal bufHCard As String) As Short
    Declare Function CSECollectNextHistory Lib "csemks32" (ByRef gpaSpecLock As GPA, ByVal bufHistory As String) As Short
    Declare Function CSECollectOldestHCard Lib "csemks32" (ByRef gpaSpecLock As GPA, ByVal bufHCard As String) As Short
    Declare Function CSECollectOldestHistory Lib "csemks32" (ByRef gpaSpecLock As GPA, ByVal bufHistory As String) As Short
    Declare Function CSECollectSendAckH Lib "csemks32" (ByRef gpaSpecLock As GPA) As Short
    Declare Function CSECollectSendAckHC Lib "csemks32" (ByRef gpaSpecLock As GPA) As Short
    Declare Function CSEConvertAccessTarget Lib "csemks32" (ByVal accesstname As String, ByVal cardtype As String, ByRef accesstarget As accesstarget) As Short
    'Declare Function CSECopy2Buffer Lib "csemks32" (ByVal accesstname As String, ByVal bufCard As String) As Short
    Declare Function CSECopy2Buffer Lib "csemks32" (ByVal accesstname As String, ByVal bufCard() As Byte) As Short
    Declare Function CSECopyCard Lib "csemks32" (ByRef card As card, ByRef cardOp As card, ByVal bufCard As String) As Short
    Declare Function CSECreateCategory Lib "csemks32" (ByVal accesstname As String, ByRef categoryparams As categoryparams, ByRef accesstarget As accesstarget) As Short
    Declare Function CSEDeleteCard Lib "csemks32" (ByRef card As card) As Short
    Declare Function CSEDeleteCategory Lib "csemks32" (ByRef accesstarget As accesstarget) As Short
    Declare Function CSEErrNo Lib "csemks32" Alias "CSELoadErrNo" () As Short
    Declare Function CSEFreeEncoder Lib "csemks32" (ByRef gpaEncoder As GPA) As Short
    Declare Function CSEExit Lib "csemks32" () As Short
    Declare Function CSEGenerateCard Lib "csemks32" (ByRef card As card, ByRef cardOp As card, ByVal fCancel As Short, ByVal bufCard As String) As Short
    Declare Function CSECreateCardToDo Lib "csemks32" (ByRef card As card, ByRef cardOp As card, ByRef cardToRecode As card) As Short
    Declare Function CSEGetAccessTName Lib "csemks32" (ByRef gpaSpecLock As GPA, ByVal accesstname As String) As Short
    Declare Function CSEIsUsedCategory Lib "csemks32" (ByRef accesstarget As accesstarget, ByRef fIsUsed As Short) As Short
    Declare Function CSELogin Lib "csemks32" (ByRef cardOp As card) As Short
    Declare Function CSELogout Lib "csemks32" (ByRef cardOp As card) As Short
    Declare Function CSEModifyPassword Lib "csemks32" (ByRef card As card, ByRef cardOp As card) As Short
    Declare Function CSEMyKeyBuffer2Card Lib "csemks32" (ByVal mykeybufCard As String, ByVal mykeyserialnumber As String, ByVal accesstname1 As String, ByVal accesstname2 As String, ByVal accesstname3 As String, ByVal accesstname4 As String, ByVal accesstname5 As String, ByRef card As card, ByVal warning As String) As Short
    Declare Function CSEMyKeyCard2Buffer Lib "csemks32" (ByRef card As card, ByVal accesstname1 As String, ByVal accesstname2 As String, ByVal accesstname3 As String, ByVal accesstname4 As String, ByVal accesstname5 As String, ByVal mykeyserialnumber As String, ByVal mykeyalgoid As Short, ByVal mykeybufCard As String) As Short
    Declare Function CSEMyKeyDecode Lib "csemks32" (ByVal mykeybufCard As String, ByVal mykeyserialnumber As String, ByVal bufCard As String) As Short
    Declare Function CSEMyKeyEncode Lib "csemks32" (ByVal bufCard As String, ByVal mykeyserialnumber As String, ByVal mykeyalgoid As Short, ByVal mykeybufCard As String) As Short
    Declare Function CSEReadAccessTarget Lib "csemks32" (ByRef accesstarget As accesstarget, ByVal fNext As Short, ByVal accesstname As String, ByVal bufParams As String) As Short
    Declare Function CSEReadCard Lib "csemks32" (ByRef gpaEncoder As GPA, ByVal fLogin As Short, ByVal bufCard As String) As Short
    Declare Function CSEReadCardHistory Lib "csemks32" (ByRef gpaEncoder As GPA) As Short
    Declare Function CSEReadDateTime Lib "csemks32" (ByRef csdate As csdate, ByRef cstime As cstime, ByVal seconds As String) As Short
    Declare Function CSEReadHHIData Lib "csemks32" (ByRef gpaHHI As GPA) As Short
    Declare Function CSEReadHolidays Lib "csemks32" () As Short
    Declare Function CSESaveCard Lib "csemks32" (ByRef card As card) As Short
    Declare Function CSESearchCard Lib "csemks32" (ByRef cardOp As Long, ByRef card As card, ByRef cardsearch As cardsearch) As Short
    Declare Function CSESearchOperation Lib "csemks32" (ByRef operation As operation, ByRef operationsearch As operationsearch) As Short
    Declare Function CSEUnpackCard Lib "csemks32" (ByVal bufCard As String, ByRef cardOp As Long, ByRef card As card, ByVal warning As String, ByVal lockmap As String) As Short
    Declare Function CSEUseEncoder Lib "csemks32" (ByRef gpaEncoder As GPA) As Short
    Declare Function CSEWriteCard Lib "csemks32" (ByRef gpaEncoder As GPA, ByVal bufCard As String) As Short
    Declare Function CSEWriteHolidays Lib "csemks32" (ByVal fMirror As Short) As Short

    Declare Function CSEWaveModeEncode Lib "csemks32" (ByVal bufCard() As Byte, ByVal wavemodecarduid As String, ByVal wavemodecardheader As Byte, ByVal wavemodebufCard() As Byte) As Short



End Module