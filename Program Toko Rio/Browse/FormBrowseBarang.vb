Imports System.Data.SqlClient
Public Class FormBrowseBarang
    Dim status As String
    Dim posisi As Integer = 0
    Dim tab As String
    Dim jumData As Integer = 0
    Dim PK As String
    Dim barang_type = ""
    Dim sub_kategori = ""

    Private Sub msgWarning(ByVal str As String)
        MsgBox(str, MsgBoxStyle.Critical, "Warning")
    End Sub

    Private Sub msgInfo(ByVal str As String)
        MsgBox(str, MsgBoxStyle.Information, "Information")
    End Sub
    Private Sub viewAllData(Optional ByVal cr As String = "", Optional ByVal cr2 As String = "", Optional ByVal opt As String = "", Optional ByVal opt2 As String = "")
        Dim addQuery = ""
        sql = " select kdBarang Kode, KdIdentifikasi 'No. Idenifikasi', NamaBarang 'Nama Barang',Merk,MsMerk.kdMerk, " & _
              " MsKategori.KdKategori,Kategori, " & _
              " MsSubkategori.KdSub,SubKategori 'Sub Kategori', " & _
              " IfNull(( Select IfNull(sum(Qty),0) - IfNull(( Select sum(Qty) from TrSalesOrderDetail " & _
              " Join TrSalesOrder On TrSalesOrderDetail.kdso = TrSalesOrder.kdso " & _
              " Where KdBarang = MsBarangList.KdBarang " & _
              " And StatusSo = 3 " & _
              " Group By KdBarang ),0) As Stock From MsBarangList " & _
              " Where KdBarang = " & tab & ".KdBarang And StatusBarangList = 0 " & _
              " Group By KdBarang ),0) Stock, " & _
              " ifNull(HargaList,0) 'Harga List' " & _
              " from  " & tab & _
              " Join MsMerk On MsMerk.kdMerk = " & tab & ".kdMerk" & _
              " Join Mskategori On Mskategori.KdKategori = " & tab & ".KdKategori" & _
              " Join MsSubkategori On MsSubkategori.KdSub = " & tab & ".KdSub " & _
              " Where 1 "

        sql &= QueryLevel(lvlKaryawan)

        If barang_type = "transfer_stock" Then
            addQuery &= " AND MsSubkategori.kdsub='" & sub_kategori & "' "
        End If

        sql &= addQuery

        Dim col As String = ""
        Dim col2 As String = ""

        If opt = "Kode" Then
            col = "KdBarang"
        ElseIf opt = "No.Idenifikasi" Then
            col = "KdIdentifikasi"
        ElseIf opt = "Nama Barang" Then
            col = "NamaBarang"
        ElseIf opt = "Merk" Then
            col = "Merk"
        ElseIf opt = "Kategori" Then
            col = "Kategori"
        ElseIf opt = "Sub Kategori" Then
            col = "SubKategori"
        End If

        If opt2 = "Kode" Then
            col2 = "KdBarang"
        ElseIf opt2 = "No.Idenifikasi" Then
            col2 = "KdIdentifikasi"
        ElseIf opt2 = "Nama Barang" Then
            col2 = "NamaBarang"
        ElseIf opt2 = "Merk" Then
            col2 = "Merk"
        ElseIf opt2 = "Kategori" Then
            col2 = "Kategori"
        ElseIf opt2 = "Sub Kategori" Then
            col2 = "SubKategori"
        End If

        If opt <> "" Then
            sql &= "  And " & col & "  like '%" & cr & "%'  "
        End If
        If opt2 <> "" Then
            sql &= "  And " & col2 & "  like '%" & cr2 & "%' "
        End If

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
        gridBarang.Columns(2).Width = 200
        gridBarang.Columns(3).Width = 100
        gridBarang.Columns(4).Visible = False
        gridBarang.Columns(5).Visible = False
        gridBarang.Columns(6).Width = 100
        gridBarang.Columns(7).Visible = False
        gridBarang.Columns(8).Width = 100
        gridBarang.Columns(9).Width = 100
        gridBarang.Columns(10).Width = 100
        If gridBarang.RowCount > 0 Then
            If Val(gridBarang.CurrentRow.Cells(9).Value) < 0 Then
                gridBarang.CurrentRow.Cells(9).Value = 0
            End If
        End If
    End Sub

    Private Sub setCmbCari()
        cmbCari.Items.Clear()
        cmbCari.Items.Add("Kode")
        cmbCari.Items.Add("No.Idenifikasi")
        cmbCari.Items.Add("Nama Barang")
        cmbCari.Items.Add("Merk")
        cmbCari.Items.Add("Kategori")
        cmbCari.Items.Add("Sub Kategori")
        cmbCari.SelectedIndex = 0

        cmbCari2.Items.Clear()
        cmbCari2.Items.Add("No.Identifikasi")
        cmbCari2.Items.Add("Nama Barang")
        cmbCari2.Items.Add("Merk")
        cmbCari2.Items.Add("Kategori")
        cmbCari2.Items.Add("Sub Kategori")
        cmbCari2.Items.Add("Supplier")
        cmbCari2.SelectedIndex = 3
    End Sub

    Private Sub FormMsArea_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        tab = " MsBarang "
        PK = "  KdBarang  "
        barang_type = data_carier(0)
        sub_kategori = data_carier(1)
        data_carier(0) = ""
        data_carier(1) = ""
        viewAllData("", "")
        setGrid()
        setCmbCari()
    End Sub

    Private Sub txtCari_TextChanged1(ByVal sender As Object, ByVal e As System.EventArgs)
        viewAllData(txtCari.Text, cmbCari.Text)
    End Sub

    Function getBarang()
        Try
            If gridBarang.RowCount > 0 Then
                data_carier(0) = gridBarang.CurrentRow.Cells("Kode").Value
                data_carier(1) = gridBarang.CurrentRow.Cells("Nama Barang").Value
                data_carier(2) = gridBarang.CurrentRow.Cells("Merk").Value
                data_carier(3) = gridBarang.CurrentRow.Cells("Sub Kategori").Value
                data_carier(4) = gridBarang.CurrentRow.Cells("Stock").Value
                data_carier(5) = gridBarang.CurrentRow.Cells("Harga List").Value
            End If
        Catch ex As Exception
            MsgBox(ex.ToString, MsgBoxStyle.Information)
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

    Private Sub cmbCari2_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmbCari2.SelectedIndexChanged

    End Sub
End Class
