Imports System.Data.SqlClient
Public Class FormBrowseBahanMentah
    Dim status As String
    Dim posisi As Integer = 0
    Dim tab As String
    Dim jumData As Integer = 0
    Dim PK As String

    Private Sub msgWarning(ByVal str As String)
        MsgBox(str, MsgBoxStyle.Critical, "Warning")
    End Sub

    Private Sub msgInfo(ByVal str As String)
        MsgBox(str, MsgBoxStyle.Information, "Information")
    End Sub
    Private Sub viewAllData(Optional ByVal cr As String = "", Optional ByVal cr2 As String = "", Optional ByVal opt As String = "", Optional ByVal opt2 As String = "")
        sql = " select kdBahanMentah Kode,NamaBahanMentah 'Nama',MsBahanMentahTipe.KdBahanMentahTipe, " & _
              " NamaBahanMentahTipe Tipe, MsBahanMentahSubKategori.KdBahanMentahSubKategori," & _
              " BahanMentahSubKategori 'Sub Kategori', " & _
              " ( Select sum(Qty) From MsBahanMentahList " & _
              " Where KdBahanMentah = " & tab & ".KdBahanMentah " & _
              " Group By KdBahanMentah ) Stock, " & _
              " ifnull(( Select Harga from msBahanMentahlist where KdBahanMentah = " & tab & ".KdBahanMentah " & _
              " order by KdBahanMentahList asc limit 1 ),0) HargaAwal " & _
              " from  " & tab & _
              " Join MsBahanMentahTipe On MsBahanMentahTipe.KdBahanMentahTipe = " & tab & ".KdBahanMentahTipe" & _
              " Join MsBahanMentahSubKategori On MsBahanMentahSubKategori.KdBahanMentahSubKategori = " & tab & ".KdBahanMentahSubKategori " & _
              " Where 1 "

        sql &= QueryLevel(lvlKaryawan)

        Dim col As String = ""
        Dim col2 As String = ""

        If opt = "Kode" Then
            col = "KdBahanMentah"
        ElseIf opt = "Nama" Then
            col = "NamaBahanMentah"
        ElseIf opt = "Tipe" Then
            col = "NamaBahanMentahTipe"
        ElseIf opt = "Sub Kategori" Then
            col = "BahanMentahSubKategori"
        End If
        If opt2 = "Kode" Then
            col2 = "KdBahanMentah"
        ElseIf opt2 = "Nama" Then
            col2 = "NamaBahanMentah"
        ElseIf opt2 = "Tipe" Then
            col2 = "NamaBahanMentahTipe"
        ElseIf opt2 = "Sub Kategori" Then
            col2 = "BahanMentahSubKategori"
        End If

        If opt <> "" Then
            sql &= "  And " & col & "  like '%" & cr & "%'  "
        End If
        If opt2 <> "" Then
            sql &= "  And " & col2 & "  like '%" & cr2 & "%' "
        End If
        sql &= " Order by NamaBahanMentah asc "

        gridBarang.DataSource = execute_datatable(sql)
    End Sub

    Private Sub setGrid()
        With gridBarang.ColumnHeadersDefaultCellStyle
            .Alignment = DataGridViewContentAlignment.MiddleCenter
            .Font = New Font(.Font.FontFamily, .Font.Size, _
              .Font.Style Or FontStyle.Bold, GraphicsUnit.Point)
            .ForeColor = Color.Gold

        End With
        gridBarang.Columns(0).Width = 100
        gridBarang.Columns(1).Width = 100
        gridBarang.Columns(2).Visible = False
        gridBarang.Columns(3).Width = 100
        gridBarang.Columns(4).Visible = False
        gridBarang.Columns(5).Width = 100
        gridBarang.Columns(6).Width = 100
        gridBarang.Columns(7).Width = 100

        If Val(gridBarang.CurrentRow.Cells(6).Value) < 0 Then
            gridBarang.CurrentRow.Cells(6).Value = 0
        End If
    End Sub

    Private Sub setCmbCari()
        cmbCari.Items.Clear()
        cmbCari.Items.Add("Kode")
        cmbCari.Items.Add("Nama")
        cmbCari.Items.Add("Tipe")
        cmbCari.Items.Add("Sub Kategori")
        cmbCari.SelectedIndex = 0

        cmbCari2.Items.Clear()
        cmbCari2.Items.Add("Kode")
        cmbCari2.Items.Add("Nama")
        cmbCari2.Items.Add("Tipe")
        cmbCari2.Items.Add("Sub Kategori")
        cmbCari2.SelectedIndex = 2
    End Sub

    Private Sub FormMsArea_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        tab = " MsBahanMentah "
        PK = "  KdBahanMentah  "
        data_carier(0) = ""
        viewAllData("", "")
        setGrid()
        setCmbCari()
    End Sub

    Function getBarang()
        Try
            data_carier(0) = gridBarang.CurrentRow.Cells("Kode").Value
            data_carier(1) = gridBarang.CurrentRow.Cells("Nama").Value
            data_carier(2) = gridBarang.CurrentRow.Cells("Sub Kategori").Value
            data_carier(3) = gridBarang.CurrentRow.Cells("Tipe").Value
            data_carier(4) = gridBarang.CurrentRow.Cells("Stock").Value
            data_carier(5) = 0
        Catch ex As Exception
            MsgBox(ex, MsgBoxStyle.Information)
        End Try
        Return False
    End Function

    Private Sub btnExit_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnExit.Click
        Me.Close()
    End Sub

    Private Sub btnAdd_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnAdd.Click
        getBarang()
        Me.Close()
    End Sub

    Private Sub gridBarang_DoubleClick(ByVal sender As Object, ByVal e As System.EventArgs) Handles gridBarang.DoubleClick
        getBarang()
        Me.Close()
    End Sub

    Private Sub txtCari_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles txtCari.TextChanged
        viewAllData(txtCari.Text, txtCari2.Text, cmbCari.Text, cmbCari2.Text)
    End Sub

    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
        txtCari.Text = ""
        txtCari2.Text = ""
        viewAllData("", "")
    End Sub

    Private Sub cmbCari_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmbCari.SelectedIndexChanged
        txtCari.Text = ""
        viewAllData("", "")
        txtCari.Focus()
    End Sub

    Private Sub gridBarang_CellContentClick(ByVal sender As System.Object, ByVal e As System.Windows.Forms.DataGridViewCellEventArgs) Handles gridBarang.CellContentClick

    End Sub

    Private Sub txtCari2_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles txtCari2.TextChanged
        viewAllData(txtCari.Text, txtCari2.Text, cmbCari.Text, cmbCari2.Text)
    End Sub
End Class
