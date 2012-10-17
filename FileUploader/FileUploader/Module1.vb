Imports System
Imports System.IO
Imports System.Text
Imports FoxCavesAPI

Module Module1
    Public File As String
    Public AppPath As String
    Public Password As String
    Public Username As String
    Sub Main()

        Console.WriteLine("FoxFolder0.0.2, Now with actual multiple file upload, Most likey cause of failure is API update or wrong login.")
        Console.WriteLine("To use, place in folder, run, login, done :)")
        Console.WriteLine("Created using FoxCaveAPI, Made by Dordian: foxcav.es")
        Console.WriteLine("Side note, the program currantly exits by crashing, the alternative worse, trust me.")
        Console.WriteLine("Username:  ")
        Username = Console.ReadLine()
        Console.WriteLine("Password:  ")
        Password = Console.ReadLine()

        System.Threading.Thread.Sleep(2000)

        AppPath = System.AppDomain.CurrentDomain.BaseDirectory

        Dim FileName As String



            Console.WriteLine(AppPath)
            For Each foundFile As String In My.Computer.FileSystem.GetFiles(AppPath)
                FileName = My.Computer.FileSystem.GetName(foundFile)

                If (Upload(foundFile, FileName) = True) Then

                    Console.WriteLine("Uploaded " & FileName)

                Else

                    Console.WriteLine("Could not upload " & FileName)

                End If
        Next


        Console.WriteLine("Press Enter to exit")
        Console.ReadKey()
        Stop


    End Sub

    Private Sub Login()




    End Sub
    Function Upload(ByVal FileDir As String, ByVal FileName As String) As Boolean

        Dim MemoryStream As MemoryStream = New MemoryStream(My.Computer.FileSystem.ReadAllBytes(FileDir))
        Dim FoxDLL As New FoxCavesAPI.Uploader()



        FoxDLL.SetCredentials(Username, Password)

        If (FoxDLL.QueueSync(FileName, MemoryStream) = "") Then
            Upload = False
        Else
            Upload = True

        End If

    End Function




End Module
