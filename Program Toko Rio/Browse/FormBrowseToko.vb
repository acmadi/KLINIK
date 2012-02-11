Imports System.Data.SqlClient
Public Class FormBrowseToko
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
    Private Sub viewAllData(ByVal cr As String, ByVal opt As String)
        sql = "select kdtoko Kode,NamaToko 'Nama Toko',NamaOwner 'Nama Owner', " & _
              "AlamatToko 'Alamat Toko',NamaArea 'Area', " & _
              "Daerah,NamaEkspedisi 'Ekspedisi',NoTelp 'Nomor Telepon',NoHP 'Nomor HP',NoFax 'Nomor Fax' " & _
              " from  " & tab & _
              "Join MsEkspedisi On MsEkspedisi.KdEkspedisi = MsToko.KdEkspedisi " & _
              "Join MsArea On MsArea.KdArea = MsToko.KdArea "

        If opt <> "" Then
            Dim col As String = ""
            If opt = "Nama Toko" Then
                col = "NamaToko"
            ElseIf opt = "Nama Owner" Then
                col = "NamaOwner"
            ElseIf opt = "Alamat" Then
                col = "AlamatToko"
            ElseIf opt = "Nama Area" Then
                col = "NamaArea"
            ElseIf opt = "Nama Dearah" Then
                col = "Daerah"
            ElseIf opt = "Ekpedisi" Then
                col = "NamaEkspedisi"
            End If
            sql &= "  where " & col & "  like '%" & cr & "%' "
        End If

        gridToko.DataSource = execute_datatable(sql)
    End Sub

    Private Sub setGrid()
        With gridToko.ColumnHeadersDefaultCellStyle
            .Alignment = DataGridViewContentAlignment.MiddleCenter
            .Font = New Font(.Font.FontFamily, .Font.Size, _
              .Font.Style Or FontStyle.Bold, GraphicsUnit.Point)
            .ForeColor = Color.Gold

        End With
        gridToko.Columns(0).Width = 100
        gridToko.Columns(1).Width = 200
        gridToko.Columns(2).Width = 100
        gridToko.Columns(3).Width = 100
        gridToko.Columns(4).Width = 100
        gridToko.Columns(5).Width = 100
        gridToko.Columns(6).Width = 100
        gridToko.Columns(7).Width = 100
        gridToko.Columns(8).Width = 100
        gridToko.Columns(9).Width = 100

    End Sub

    Private Sub setCmbCari()
        cmbCari.Items.Clear()
        cmbCari.Items.Add("Nama Toko")
        cmbCari.Items.Add("Nama Owner")
        cmbCari.Items.Add("Alamat")
        cmbCari.Items.Add("Nama Area")
        cmbCari.Items.Add("Nama Dearah")
        cmbCari.Items.Add("Ekpedisi")
        cmbCari.SelectedIndex = 0
    End Sub

    Private Sub FormMsArea_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        tab = " MsToko "
        PK = "  KdToko  "
        data_carier(0) = ""
        viewAllData("", "")
        setGrid()
        setCmbCari()
    End Sub

    Private Sub txtCari_TextChanged1(ByVal sender As Object, ByVal e As System.EventArgs)
        viewAllData(txtCari.Text, cmbCari.Text)
    End Sub

    Function getSales()
        Try
            data_carier(0) = gridToko.CurrentRow.Cells(0).Value
            data_carier(1) = gridToko.CurrentRow.Cells(1).Value
            data_carier(2) = gridToko.CurrentRow.Cells(5).Value
            data_carier(3) = gridToko.CurrentRow.Cells(4).Value
        Catch ex As Exception
            MsgBox(ex.ToString, MsgBoxStyle.Information)
        End Try
        Return False
    End Function

    Private Sub btnExit_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnExit.Click
        Me.Close()
    End Sub

    Private Sub btnAdd_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnAdd.Click
        getSales()
        Me.Close()
    End Sub

    Private Sub gridSales_DoubleClick(ByVal sender As Object, ByVal e As System.EventArgs) Handles gridToko.DoubleClick
        getSales()
        Me.Close()
    End Sub

    Private Sub txtCari_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles txtCari.TextChanged
        viewAllData(txtCari.Text, cmbCari.Text)
    End Sub

    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
        txtCari.Text = ""
        viewAllData("", "")
    End Sub

    Private Sub cmbCari_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmbCari.SelectedIndexChanged
        txtCari.Text = ""
        viewAllData("", "")
        txtCari.Focus()
    End Sub
End Class
