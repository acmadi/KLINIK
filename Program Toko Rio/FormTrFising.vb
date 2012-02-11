Imports System.Data.SqlClient

Public Class FormTrFising
    Dim query As String = ""
    Dim PK As String = ""
    Dim tabelH As String = ""
    Dim tabelD As String = ""

    Private Sub msgWarning(ByVal str As String)
        MsgBox(str, MsgBoxStyle.Critical, "Warning")
    End Sub

    Private Sub msgInfo(ByVal str As String)
        MsgBox(str, MsgBoxStyle.Information, "Information")
    End Sub

    Private Sub setGrid()
        With DataGridView1.ColumnHeadersDefaultCellStyle
            .Alignment = DataGridViewContentAlignment.MiddleCenter
            .Font = New Font(.Font.FontFamily, .Font.Size, _
              .Font.Style Or FontStyle.Bold, GraphicsUnit.Point)
            .ForeColor = Color.Gold
        End With
        DataGridView1.Columns(0).Width = 100
        DataGridView1.Columns(1).Width = 200
        DataGridView1.Columns(2).Width = 120
        DataGridView1.Columns(3).Width = 120
    End Sub

    Private Sub viewBarangMentah()
        query = " select kdBahanMentah Kode,NamaBahanMentah Nama,NamaBahanMentahTipe Tipe, " & _
          " BahanMentahSubKategori 'Sub Kategori' " & _
          " from  msbahanmentah " & _
          " Join MsBahanMentahTipe On MsBahanMentahTipe.KdBahanMentahTipe =msbahanmentah.KdBahanMentahTipe" & _
          " Join MsBahanMentahSubKategori On MsBahanMentahSubKategori.KdBahanMentahSubKategori = msbahanmentah.KdBahanMentahSubKategori" & _
          " Join MsSupplier On MsSupplier.KdSupplier = msbahanmentah.KdSupplier where 1 "

        query &= QueryLevel(lvlKaryawan)

        DataGridView1.DataSource = execute_datatable(query)
    End Sub


    Public Sub setComboType()
        cmbType.Items.Clear()
        Dim reader = execute_reader("Select * from msjenisfishing order by JenisFishing asc")
        Do While reader.Read
            cmbType.Items.Add(reader(1)) '  & "-" & reader(0))
        Loop
        reader.Close()
        If cmbType.Items.Count > 0 Then cmbType.SelectedIndex = 0
    End Sub

    Private Function setCodePB() As String
        Dim code As String = "FS"
        Dim angka As Integer
        Dim kode As String
        Dim temp As String
        Dim bulan As Integer = CInt(Today.Month.ToString)
        code += Today.Year.ToString.Substring(2, 2)
        If bulan < 10 Then
            code += "0" + bulan.ToString
        Else
            code += bulan.ToString
        End If

        query = " select " & PK & " from  " & tabelH & "  order by " & PK & "  desc"
        Dim reader = execute_reader(query)
        If reader.Read() Then
            kode = reader(0)

            temp = kode.Substring(0, 6)
            If temp = code Then
                angka = CInt(kode.Substring(6, 4))
            Else
                angka = 0
            End If
            reader.Close()
        Else
            angka = 0
        End If
        reader.Close()
        angka = angka + 1
        Dim len As Integer = angka.ToString().Length

        For i As Integer = 1 To 4 - len
            code += "0"
        Next
        code = code & (angka)
        Return code
    End Function

    Private Sub FormTrFising_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        PK = "no_fising"
        tabelH = "trheaderfising"
        tabelD = "trdetailfising"
        txtID.Text = setCodePB()
        DateTimePickerSO.Text = Convert.ToDateTime(Today.Date)

        setComboType()

        viewBarangMentah()
        setGrid()
    End Sub
 
End Class