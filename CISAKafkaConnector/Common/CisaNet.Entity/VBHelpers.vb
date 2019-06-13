Public Class VBHelpers

    Public Function MidVB(str As String, Start As Integer, Length As Integer) As String
        Dim output As String
        output = Mid$(str, Start, Length)
        MidVB = output
    End Function


    Public Function Ascii2Hex(DATA() As Byte) As String
        Dim output As String
        Dim esa As String
        Dim I As Integer

        output = ""
        For I = 0 To DATA.Length - 1
            esa = Hex$(DATA(I))
            If Len(esa) = 1 Then esa = "0" + esa
            output = output + esa
        Next I

        Ascii2Hex = output

    End Function

    Public Function Hex2Ascii(io$) As String
        Dim output$, asc$
        Dim I As Integer
        output$ = ""
        For I = 1 To Len(io$) Step 2
            asc$ = (Val("&H" + (Mid$(io$, I, 2))))
            output$ = output$ + asc$
        Next I
        Hex2Ascii = output$
    End Function

    Public Function Hex2Bin(io$) As String
        Dim output$
        Dim asc As Byte
        Dim I As Integer
        output$ = ""
        For I = 1 To Len(io$) Step 2
            asc = (Val("&H" + (Mid$(io$, I, 2))))
            output$ = output$ + Chr(asc)
        Next I
        Hex2Bin = output$
    End Function

End Class
