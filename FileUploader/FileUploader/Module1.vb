Imports System
Imports System.IO
Imports System.Text
Imports FoxScreen

Module Module1
    Public File As String
    Public AppPath As String
    Sub Main()

        Console.WriteLine("Ok, Whist this is still in like alpha version -0.1, the file being uploaded is going to be upload.txt in the same place as the exe")
        System.Threading.Thread.Sleep(2000)

        AppPath = System.AppDomain.CurrentDomain.BaseDirectory
        If (My.Computer.FileSystem.FileExists(AppPath & "upload.txt")) Then

            File = AppPath & "upload.txt"

            Upload(File, "UploadTest.txt")

        Else

            MsgBox("File upload.txt does not exist, please create this file then try again")

        End If




    End Sub


    Function Upload(ByVal FileDir As String, ByVal FileName As String) As Boolean

        Dim MemoryStream As MemoryStream = New MemoryStream(My.Computer.FileSystem.ReadAllBytes(FileDir))
        Dim FoxDLL As New FoxScreen.UploadOrganizer



        FoxDLL.AddUpload(FileName, MemoryStream)



        Upload = True

    End Function




End Module
