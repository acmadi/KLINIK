Imports System.Data.SqlClient
Public Class FormBarangList
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

    Private Sub setData(ByVal sql As String)

        Dim reader = execute_reader(sql)

        While reader.read
            txtID.Text = reader(0).ToString
            txtIdentifikasi.Text = reader(1).ToString
            txtNama.Text = reader(2).ToString
            txtMerk.Text = reader(3).ToString
            txtKategori.Text = reader(4).ToString
            txtSubKategori.Text = reader(5).ToString
            txtQty.Text = reader(6).ToString
            txtRusak.Text = reader(9).ToString
            txtTotalAset.Text = FormatNumber(reader("GrandTotalAset").ToString, 0)
        End While
        reader.close()

        Dim sql_history = " select ifNull(sum(QtyRetur_Plus),0), " & _
                          " IfNull(( Select sum(QtyRetur_Plus) from baranghistory bh where StatusBarangList = 1 " & _
                          " And bh.kdBarang = baranghistory.kdBarang ),0), " & _
                          " IfNull(( Select sum(Qty) from trsalesorder so " & _
                          " Join trsalesorderdetail so_d On so_d.kdSO = so.KdSO " & _
                          " where StatusSO = 3 " & _
                          " And so_d.kdBarang = baranghistory.kdBarang ),0), " & _
                          " IfNull(sum(QtyPurchase_Plus),0),IfNull(sum(QtyRetur_Min),0), " & _
                          " IfNull(sum(QtyFaktur_Min),0),IfNull(sum(QtyUpdate_Plus+QtyAdj_Plus),0), " & _
                          " IfNull(sum(QtyUpdate_Min+QtyAdj_Min),0), " & _
                          " IfNull(sum(QtyProd_Plus),0) prod " & _
                          " from baranghistory where StatusBarangList <> 1 and KdBarang = '" & txtID.Text & "' "
        Dim reader_history = execute_reader(sql_history)

        txtSO.Text = 0
        While reader_history.read
            txtSO.Text = reader_history(2)
        End While
        reader_history.close()

    End Sub

    Private Sub viewAllData(ByVal cr As String, ByVal opt As String)
        sql = " select kdBarang 'No. Part', KdIdentifikasi 'No. Identifikasi', " & _
              " NamaBarang Nama,MsMerk.Merk, " & _
              " Kategori Kategori, SubKategori 'Sub Kategori', " & _
              " ifnull(( Select StockAkhir from BarangHistory where isActive = 1 And KdBarang = " & tab & ".KdBarang order by KdBarangHistory desc limit 1 ),0) Stock, " & _
              " MsSupplier.Nama Supplier,Note, " & _
              " ifnull(( Select sum(Qty) from msbaranglist where StatusBarangList = 1 And KdBarang = " & tab & ".KdBarang ),0) StockRusak, " & _
              " ifnull(( select sum(QtyFaktur_Min) from baranghistory where IfNull(QtyFaktur_Min,0) <> 0 And KdBarang = " & tab & ".KdBarang ),0) TotalFaktur, " & _
              " ifnull(( Select sum(Harga*Qty) from msbaranglist where KdBarang = " & tab & ".KdBarang ),0) GrandTotalAset " & _
              " from  " & tab & _
              " Join Mskategori On Mskategori.KdKategori = " & tab & ".KdKategori" & _
              " Join MsSubkategori On MsSubkategori.KdSub = " & tab & ".KdSub" & _
              " Join MsSupplier On MsSupplier.KdSupplier = " & tab & ".KdSupplier" & _
              " Join MsMerk On MsMerk.KdMerk = " & tab & ".KdMerk " & _
              " Where MsBarang.KdBarang = '" & PK & "' "

        Dim sql_stock = " select sum(Qty) 'Stock', " & _
                        " Harga 'Harga Modal', sum(Harga*Qty) 'Subtotal Asset' " & _
                        " from msbaranglist " & _
                        " Where KdBarang = '" & PK & "' " & _
                        " Group By Harga "

        If opt <> "" Then
            Dim col As String = ""
            If opt = "Stock" Then
                col = "Qty"
            ElseIf opt = "Harga Modal" Then
                col = "Harga"
            ElseIf opt = "Subtotal Asset" Then
                col = "Harga*Qty"
            End If

            sql_stock &= "  And " & col & "  like '%" & cr & "%' "
        End If

        gridStock.DataSource = execute_datatable(sql_stock)
        jumData = gridStock.RowCount
        setData(sql)
    End Sub

    Private Sub setCmbCari()
        cmbCari.Items.Clear()
        cmbCari.Items.Add("Stock")
        cmbCari.Items.Add("Harga Modal")
        cmbCari.Items.Add("Subtotal Asset")
        cmbCari.SelectedIndex = 0
    End Sub

    Private Sub FormMsSupplier_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        tab = " MsBarang "
        PK = data_carier(0)
        clear_variable_array()
        setCmbCari()
        viewAllData("", "")
        posisi = 0
    End Sub

    Private Sub btnReset_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnReset.Click
        txtCari.Text = ""
        viewAllData("", "")
    End Sub

    Private Sub txtCari_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtCari.TextChanged
        viewAllData(txtCari.Text, cmbCari.Text)
    End Sub
End Class
