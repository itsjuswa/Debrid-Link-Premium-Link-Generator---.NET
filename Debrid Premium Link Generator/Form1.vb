Imports System.Net
Imports System.IO
Imports Newtonsoft.Json.Linq
Imports System.Net.Http
Imports System.Threading
Imports Microsoft.Win32
Imports System.ComponentModel
Imports Newtonsoft.Json

Public Class Form1

    '////////////////////////////////////////////////////////////////////////////
    '// A proper credit will do, made by JOSH.                                ///
    '// Purchase API key here > https://debrid-link.com/                     ///
    '// You can find the supporting websites as well on their page.         ///
    '// it's a simple GUI for Debrid-Link, just incase you want to create   ///
    '// your own Premium Link Generator                                    ///
    '////////////////////////////////////////////////////////////////////////

    ''DON'T FORGET TO ADD JSON PACKAGE FROM NUGET TO AVOID ERROR!

    Dim WithEvents webClient As WebClient
    Dim totalBytes As Long
    Dim downloadedBytes As Long
    Dim lastBytesDownloaded As Long
    Dim downloadStartTime As DateTime
    Public Class DownloadInfo
        Public Property Id As String
        Public Property Filename As String
        Public Property DownloadUrl As String
        Public Property FileSize As Long
    End Class

    Private Async Function Button1_ClickAsync(sender As Object, e As EventArgs) As Task Handles Button1.Click
        Dim url As String = txtUserInputLink.Text
        Dim downloadInfo As DownloadInfo = Await GetDownloadUrlAsync(url)
        If downloadInfo IsNot Nothing Then
            txtPremiumLink.Text = downloadInfo.DownloadUrl
            Label2.Text = "Filename: " & vbNewLine & downloadInfo.Filename
        Else
        End If
    End Function
    Private Async Function GetDownloadUrlAsync(url As String) As Task(Of DownloadInfo)
        Dim downloadInfo As DownloadInfo = Nothing

        Try
            Dim request As HttpWebRequest = CType(WebRequest.Create("https://debrid-link.com/api/v2/downloader/add"), HttpWebRequest)
            request.Method = "POST"
            request.ContentType = "application/json"
            request.Headers.Add("Authorization", "Bearer YOUR API HERE")
            request.Accept = "application/json"

            Dim jsonData As String = "{ ""url"": """ & url & """ }"

            Dim byteArray As Byte() = System.Text.Encoding.UTF8.GetBytes(jsonData)
            request.ContentLength = byteArray.Length

            Using dataStream As Stream = Await request.GetRequestStreamAsync()
                Await dataStream.WriteAsync(byteArray, 0, byteArray.Length)
            End Using

            Dim response As HttpWebResponse = CType(Await request.GetResponseAsync(), HttpWebResponse)

            Using reader As New StreamReader(response.GetResponseStream())
                Dim jsonResponse As String = Await reader.ReadToEndAsync()

                Dim jsonObject As JObject = JObject.Parse(jsonResponse)
                downloadInfo = New DownloadInfo With {
                .Id = jsonObject("value")("id").ToString(),
                .Filename = jsonObject("value")("name").ToString(),
                .DownloadUrl = jsonObject("value")("downloadUrl").ToString(),
                .FileSize = jsonObject("value")("size").ToObject(Of Long)()
            }
            End Using

        Catch ex As Exception

        End Try

        Return downloadInfo
    End Function
    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load

    End Sub


End Class
