Option Strict On
Imports System.IO
Public Class Form1
    Dim BackupType As String
    Dim FolderDirectory As New Hashtable
    Dim MainFolder As String
    Dim lstAllFiles As New List(Of String)
    Dim lstDirectories As New List(Of String)
    Dim CountMarker As Integer = 0

    Private Sub btnBackup_Click(sender As Object, e As EventArgs) Handles btnBackup.Click
        If RadioButton1.Checked = True Then
            BackupType = "mods"
            PictureBox1.BackgroundImage = My.Resources.tly
            PictureBox1.Update()
            MainFolder = Environ("AppData") & "\.minecraft\mods\"
            Label2.Text = "Backup running..."
            Label2.Refresh()
        ElseIf RadioButton2.Checked = True Then
            PictureBox1.BackgroundImage = My.Resources.tly
            PictureBox1.Update()
            BackupType = "saves"
            MainFolder = Environ("AppData") & "\.minecraft\saves"
            Label2.Text = "Backup running..."
            Label2.Refresh()
        End If
        Dim fileDateTime As String = DateTime.Now.ToString("yyyyMMdd") & "_" & DateTime.Now.ToString("HHmmss")
        Dim CurrentDir As String
        CurrentDir = "C:\MCDir\Backups\" & fileDateTime & BackupType

        My.Computer.FileSystem.CreateDirectory(CurrentDir)

        Threading.Thread.Sleep(2000)
        Label2.Text = "Backup running..."
        Threading.Thread.Sleep(2000)
        Dim count As Integer = 0
        FolderDirectory.Add(count, CurrentDir)
        Label2.Text = "Backup running..."
        lstAllFiles.AddRange(Directory.GetFiles(MainFolder))
        CopyFiles(count)


        lstDirectories.AddRange(Directory.GetDirectories(MainFolder))

        Dim FolderMarker As Integer = 0
        For Each folder In lstDirectories
            CopyDirectories(folder, count, FolderMarker)
        Next

        lstAllFiles.Clear()
        lstDirectories.Clear()
        FolderDirectory.Clear()
        count = 0
        CountMarker = 0
        cbNoOverwrite.Checked = False
        Label2.Text = "Idle"
        PictureBox1.BackgroundImage = My.Resources.tlg
        MessageBox.Show("All files are successfully backed up!", "Completed!")



    End Sub

#Region "Methods"
    ''' <summary>
    ''' This is a method that copies files in the chosen directory into the backup location
    ''' </summary>
    ''' <param name="count">This parameter is used to pick the correct folder directory when copying files over</param>
    ''' <remarks></remarks>
    Sub CopyFiles(ByVal count As Integer)
        Try
            For Each File In lstAllFiles
                Dim FileInfo As New FileInfo(File)
                Dim Filename As String = FileInfo.Name
                Dim FileLocation As String = FileInfo.FullName
                Dim FileDate As Date = FileInfo.LastWriteTime

                If System.IO.File.Exists(Path.Combine(CStr(FolderDirectory(count)), Filename)) Then
                    Dim TempFileinfo As New FileInfo(Path.Combine(CStr(FolderDirectory(count)), Filename))
                    Dim TempFileDate As Date = TempFileinfo.LastWriteTime
                    Dim result As Integer = DateTime.Compare(FileDate, TempFileDate)

                    If result > 0 AndAlso cbNoOverwrite.Checked = False Then
                        System.IO.File.Copy(FileLocation, Path.Combine(CStr(FolderDirectory(count)), Filename), True)
                    End If
                Else
                    System.IO.File.Copy(FileLocation, Path.Combine(CStr(FolderDirectory(count)), Filename))
                End If
            Next
        Catch ex As Exception
            PictureBox1.BackgroundImage = My.Resources.tlr
            MessageBox.Show("Oops! Something went wrong! " & ex.ToString, "DTA stream error ")
        End Try
    End Sub
    ''' <summary>
    ''' This sub copies folder directories over to the backup location and uses recursion to call itself so that it looks at directories in itself and also calls CopyFiles to copy the files over in those directories
    ''' </summary>
    ''' <param name="Folder">This parameter holds the folder location</param>
    ''' <param name="count">This parameter holds the count used to add more directories into the hashtable and used so that it creates a unique ID for each folder</param>
    ''' <param name="FolderMarker">this parameter is used a marker so that it adds the correct folder directory path</param>
    ''' <remarks></remarks>
    Sub CopyDirectories(ByVal Folder As String, ByVal count As Integer, ByVal FolderMarker As Integer)
        Try
            'Check if folder exists in backup and if it doesnt copy entire folder over
            'if folder exists then go through each folder and copy files/folders over
            'use recursion to call 
            Dim FolderInfo As New DirectoryInfo(Folder)
            Dim Folderlocation As String = FolderInfo.FullName
            Dim FolderName As String = FolderInfo.Name
            Dim lstTempFolderDirectories As New List(Of String)
            lstTempFolderDirectories.AddRange(Directory.GetDirectories(Folderlocation))

            'if the folder does not exist then copy folder and contents over otherwise read through contents and update if necassary
            If Directory.Exists(Path.Combine(CStr(FolderDirectory(count)), FolderName)) Then
                lstAllFiles.Clear()
                lstAllFiles.AddRange(Directory.GetFiles(Folderlocation))

                'Loop until the count is higher than the countmarker so that the count is a new number and that the FolderDirectory doesnt try to add a directory in a used key
                Do Until count > CountMarker
                    count += 1
                Loop
                CountMarker = count

                'Add directory to hashtable
                FolderDirectory.Add((count), (Path.Combine(CStr(FolderDirectory(FolderMarker)), FolderName)))

                'Add 1 to the foldermarker here
                FolderMarker += 1

                'Copy files in this directory over
                CopyFiles(count)

                If lstTempFolderDirectories.Contains(Nothing) Then
                Else
                    'Use recursion to call this same subroutine so that it can go through each subfolders
                    For Each Folder In lstTempFolderDirectories
                        CopyDirectories(Folder, count, FolderMarker)
                    Next
                End If
            Else
                'if folder does not exist then copy over whole of the directory including everything inside it
                My.Computer.FileSystem.CopyDirectory(Folderlocation, Path.Combine(CStr(FolderDirectory(FolderMarker)), FolderName))
            End If

        Catch ex As Exception
            MessageBox.Show("Something went wrong. " & ex.ToString, "Data error ")
        End Try
    End Sub
#End Region



    Private Sub Button1_Click(sender As Object, e As EventArgs)
        FolderBrowserDialog1.ShowDialog()
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs)
        FolderBrowserDialog2.ShowDialog()
    End Sub

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Label2.Text = "Idle"
        MainFolder = "%AppData%\.minecraft\"
        PictureBox1.BackgroundImage = My.Resources.tlg
    End Sub

    Private Sub Button1_Click_1(sender As Object, e As EventArgs)
        FolderBrowserDialog1.ShowDialog()
    End Sub

    Private Sub Label2_Click(sender As Object, e As EventArgs) Handles Label2.Click

    End Sub

    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        My.Computer.FileSystem.CreateDirectory(
            "C:\MCDir")
        My.Computer.FileSystem.CreateDirectory(
            "C:\MCDir\Backups")
    End Sub

    Private Sub Button1_Click_2(sender As Object, e As EventArgs) Handles Button1.Click
        FolderBrowserDialog3.ShowDialog()
        My.Computer.FileSystem.CopyDirectory(FolderBrowserDialog3.SelectedPath, Environ("AppData") & "\.minecraft\mods\")
    End Sub

    Private Sub RadioButton1_CheckedChanged(sender As Object, e As EventArgs) Handles RadioButton1.CheckedChanged

    End Sub
End Class
