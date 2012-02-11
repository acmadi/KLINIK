Imports System.Data.SqlClient
Public Class FormBarangHistory
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
            txtSupplier.Text = reader(7).ToString
            txtNote.Text = reader(8).ToString
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

        txtReturRusak.Text = 0
        txtRetur.Text = 0
        txtSO.Text = 0
        txtPB.Text = 0
        txtReturPB.Text = 0
        txtFaktur.Text = 0
        txtUpdatePlus.Text = 0
        txtUpdateMin.Text = 0
        txtProd.Text = 0
        While reader_history.read
            txtRetur.Text = reader_history(0)
            txtReturRusak.Text = reader_history(1)
            txtSO.Text = reader_history(2)
            txtPB.Text = reader_history(3)
            txtReturPB.Text = reader_history(4)
            txtFaktur.Text = reader_history(5)
            txtUpdatePlus.Text = reader_history(6)
            txtUpdateMin.Text = reader_history(7)
            txtProd.Text = reader_history("prod")
        End While
        reader_history.close()
        txtTotalPlus.Text = Val(txtPB.Text) + Val(txtRetur.Text) + Val(txtUpdatePlus.Text) + Val(txtProd.Text)
        txtTotalMin.Text = Val(txtTotalPlus.Text) - Val(txtFaktur.Text) - Val(txtReturPB.Text) - Val(txtUpdateMin.Text)

    End Sub

    Private Sub viewAllData(ByVal cr As String, ByVal opt As String)
        sql = " select kdBarang 'No. Part', KdIdentifikasi 'No. Identifikasi', " & _
              " NamaBarang Nama,MsMerk.Merk, " & _
              " Kategori Kategori, SubKategori 'Sub Kategori', " & _
              " ifnull(( Select StockAkhir from BarangHistory where isActive = 1 And KdBarang = " & tab & ".KdBarang order by KdBarangHistory desc limit 1 ),0) Stock, " & _
              " MsSupplier.Nama Supplier,Note, " & _
              " ifnull(( Select sum(Qty) from msbaranglist where StatusBarangList = 1 And KdBarang = " & tab & ".KdBarang ),0) StockRusak, " & _
              " ifnull(( select sum(QtyFaktur_Min) from baranghistory where IfNull(QtyFaktur_Min,0) <> 0 And KdBarang = " & tab & ".KdBarang ),0) TotalFaktur " & _
              " from  " & tab & _
              " Join Mskategori On Mskategori.KdKategori = " & tab & ".KdKategori" & _
              " Join MsSubkategori On MsSubkategori.KdSub = " & tab & ".KdSub" & _
              " Join MsSupplier On MsSupplier.KdSupplier = " & tab & ".KdSupplier" & _
              " Join MsMerk On MsMerk.KdMerk = " & tab & ".KdMerk " & _
              " Where MsBarang.KdBarang = '" & PK & "' "

        Dim sql_stock = " select DATE_FORMAT(TanggalHistory,'%d %M %Y') Tanggal, StockAwal 'Stock Awal', " & _
                        " StockAkhir - StockAwal Qty, StockAkhir 'Stock Akhir', " & _
                        " Module " & _
                        " from BarangHistory " & _
                        " Where BarangHistory.KdBarang = '" & PK & "' "

        If opt <> "" Then
            Dim col As String = ""
            If opt = "Tanggal" Then
                col = "TanggalHistory"
            ElseIf opt = "Stock Awal" Then
                col = "StockAwal"
            ElseIf opt = "Qty" Then
                col = "StockAkhir - StockAwal"
            ElseIf opt = "Stock Akhir" Then
                col = "StockAkhir"
            ElseIf opt = "Module" Then
                col = "Module"
            End If

            If col = "TanggalHistory" Then
                sql_stock &= " And DATE_FORMAT(TanggalHistory, '%Y/%m/%d') >= '" & dtpFrom.Value.ToString("yyyy/MM/dd") & "' " & _
                                   " and DATE_FORMAT(TanggalHistory, '%Y/%m/%d') <= '" & dtpTo.Value.ToString("yyyy/MM/dd") & "' "
            Else
                sql_stock &= "  And " & col & "  like '%" & cr & "%' "
            End If
        End If

        gridStock.DataSource = execute_datatable(sql_stock)
        jumData = gridStock.RowCount
        setData(sql)

        gridStock.Columns(0).Width = 150
        gridStock.Columns(4).Width = 170
    End Sub

    Private Sub viewAllDataProd(ByVal cr As String, ByVal opt As String)
        Dim sql_stock = " select RefNumber 'No. Produksi', " & _
                        " DATE_FORMAT(TanggalHistory,'%d %M %Y') Tanggal,QtyProd_Plus " & _
                        " from baranghistory " & _
                        " Where baranghistory.Kdbarang = '" & PK & "' " & _
                        " And ifNull(QtyProd_Plus,0) <> 0 "

        If opt <> "" Then
            Dim col As String = ""
            If opt = "No. Prod" Then
                col = "RefNumber"
            ElseIf opt = "Tanggal" Then
                col = "TanggalHistory"
            ElseIf opt = "Qty" Then
                col = "QtyProd_Plus"
            End If

            If col = "TanggalHistory" Then
                sql_stock &= " And DATE_FORMAT(TanggalHistory, '%Y/%m/%d') >= '" & dtpFromProd.Value.ToString("yyyy/MM/dd") & "' " & _
                                   " and DATE_FORMAT(TanggalHistory, '%Y/%m/%d') <= '" & dtpToProd.Value.ToString("yyyy/MM/dd") & "' "
            Else
                sql_stock &= "  And " & col & "  like '%" & cr & "%' "
            End If
        End If

        gridProd.DataSource = execute_datatable(sql_stock)

        gridProd.Columns(1).Width = 150
    End Sub

    Private Sub viewAllDataFaktur(ByVal cr As String, ByVal opt As String)
        Dim sql_stock = " select trfaktur.KdFaktur 'No. Faktur', DATE_FORMAT(TanggalFaktur,'%d %M %Y') Tanggal, " & _
                        " NamaSales 'Sales', NamaToko 'Toko', Qty " & _
                        " from trfakturdetail " & _
                        " Join trfaktur On trfaktur.kdFaktur = trfakturdetail.KdFaktur " & _
                        " Join MsSales MS On trfaktur.KdSales = MS.KdSales " & _
                        " Join MsToko MT On trfaktur.KdToko = MT.KdToko " & _
                        " Where trfakturdetail.KdBarang = '" & PK & "' " & _
                        " And StatusFaktur <> 0 "

        If opt <> "" Then
            Dim col As String = ""
            If opt = "No. Faktur" Then
                col = "trfaktur.KdFaktur"
            ElseIf opt = "Tanggal" Then
                col = "TanggalFaktur"
            ElseIf opt = "Sales" Then
                col = "NamaSales"
            ElseIf opt = "Toko" Then
                col = "NamaToko"
            ElseIf opt = "Qty" Then
                col = "Qty"
            End If

            If col = "TanggalFaktur" Then
                sql_stock &= " And DATE_FORMAT(TanggalFaktur, '%Y/%m/%d') >= '" & dtpFromFaktur.Value.ToString("yyyy/MM/dd") & "' " & _
                                   " and DATE_FORMAT(TanggalFaktur, '%Y/%m/%d') <= '" & dtpToFaktur.Value.ToString("yyyy/MM/dd") & "' "
            Else
                sql_stock &= "  And " & col & "  like '%" & cr & "%' "
            End If
        End If

        gridFaktur.DataSource = execute_datatable(sql_stock)

        gridFaktur.Columns(0).Width = 150
        gridFaktur.Columns(1).Width = 170
    End Sub

    Private Sub viewAllDataReturFaktur(ByVal cr As String, ByVal opt As String)
        Dim sql_stock = " select trretur.KdRetur 'No. Retur', DATE_FORMAT(TanggalRetur,'%d %M %Y') Tanggal, " & _
                        " KdSO 'No. SO', NamaSales 'Sales', NamaToko 'Toko', sum(Qty) qty " & _
                        " from trreturdetail " & _
                        " Join trretur On trretur.kdretur = trreturdetail.Kdretur " & _
                        " Join MsSales MS On trretur.KdSales = MS.KdSales " & _
                        " Join MsToko MT On trretur.KdToko = MT.KdToko " & _
                        " Where trreturdetail.KdBarang = '" & PK & "' " & _
                        " And StatusRetur <> 0 "

        If opt <> "" Then
            Dim col As String = ""
            If opt = "No. Retur" Then
                col = "trretur.KdRetur"
            ElseIf opt = "No. SO" Then
                col = "trretur.KdSO"
            ElseIf opt = "Tanggal" Then
                col = "TanggalRetur"
            ElseIf opt = "Sales" Then
                col = "NamaSales"
            ElseIf opt = "Toko" Then
                col = "NamaToko"
            ElseIf opt = "Qty" Then
                col = "Qty"
            End If

            If col = "TanggalRetur" Then
                sql_stock &= " And DATE_FORMAT(TanggalRetur, '%Y/%m/%d') >= '" & dtpFromReturFaktur.Value.ToString("yyyy/MM/dd") & "' " & _
                                   " and DATE_FORMAT(TanggalRetur, '%Y/%m/%d') <= '" & dtpToReturFaktur.Value.ToString("yyyy/MM/dd") & "' "
            Else
                sql_stock &= "  And " & col & "  like '%" & cr & "%' "
            End If
        End If

        gridReturFaktur.DataSource = execute_datatable(sql_stock & " Group by DATE_FORMAT(TanggalRetur,'%d %M %Y'),KdSO ")

        gridReturFaktur.Columns(0).Width = 150
        gridReturFaktur.Columns(1).Width = 170
        gridReturFaktur.Columns(2).Width = 150
    End Sub

    Private Sub viewAllDataPB(ByVal cr As String, ByVal opt As String)
        Dim sql_stock = " select trheaderpb.No_PB 'No. Penerimaan', DATE_FORMAT(Tanggal_TerimaBarang,'%d %M %Y') Tanggal, " & _
                        " No_PO 'No. PO', MS.Nama 'Supplier', Qty " & _
                        " from trdetailpb " & _
                        " Join trheaderpb On trheaderpb.No_PB = trdetailpb.No_PB " & _
                        " Join MsSupplier MS On trheaderpb.KdSupplier = MS.KdSupplier " & _
                        " Where trdetailpb.KdBarang = '" & PK & "' " & _
                        " And StatusTerimaBarang <> 0 "

        If opt <> "" Then
            Dim col As String = ""
            If opt = "No. Penerimaan" Then
                col = "trheaderpb.No_PB"
            ElseIf opt = "No. PO" Then
                col = "No_PO"
            ElseIf opt = "Tanggal" Then
                col = "Tanggal_TerimaBarang"
            ElseIf opt = "Supplier" Then
                col = "MS.Nama"
            ElseIf opt = "Qty" Then
                col = "Qty"
            End If

            If col = "Tanggal_TerimaBarang" Then
                sql_stock &= " And DATE_FORMAT(Tanggal_TerimaBarang, '%Y/%m/%d') >= '" & dtpFromPB.Value.ToString("yyyy/MM/dd") & "' " & _
                                   " and DATE_FORMAT(Tanggal_TerimaBarang, '%Y/%m/%d') <= '" & dtpToPB.Value.ToString("yyyy/MM/dd") & "' "
            Else
                sql_stock &= "  And " & col & "  like '%" & cr & "%' "
            End If
        End If

        gridPB.DataSource = execute_datatable(sql_stock)

        gridPB.Columns(1).Width = 170
    End Sub

    Private Sub viewAllDataReturPB(ByVal cr As String, ByVal opt As String)
        Dim sql_stock = " select trheaderreturbeli.KdRetur 'No. Retur Penerimaan', DATE_FORMAT(TanggalRetur,'%d %M %Y') Tanggal, " & _
                        " No_PO 'No. PO', MS.Nama 'Supplier', sum(Qty) qty " & _
                        " from trdetailreturbeli " & _
                        " Join trheaderreturbeli On trheaderreturbeli.KdRetur = trdetailreturbeli.KdRetur " & _
                        " Join MsSupplier MS On trheaderreturbeli.KdSupplier = MS.KdSupplier " & _
                        " Where trdetailreturbeli.KdBarang = '" & PK & "' " & _
                        " And StatusRetur <> 0 "


        If opt <> "" Then
            Dim col As String = ""
            If opt = "No. Retur Penerimaan" Then
                col = "trheaderreturbeli.KdRetur"
            ElseIf opt = "No. PO" Then
                col = "No_PO"
            ElseIf opt = "Tanggal" Then
                col = "TanggalRetur"
            ElseIf opt = "Supplier" Then
                col = "MS.Nama"
            ElseIf opt = "Qty" Then
                col = "Qty"
            End If

            If col = "TanggalRetur" Then
                sql_stock &= " And DATE_FORMAT(TanggalRetur, '%Y/%m/%d') >= '" & dtpFromReturPB.Value.ToString("yyyy/MM/dd") & "' " & _
                                   " and DATE_FORMAT(TanggalRetur, '%Y/%m/%d') <= '" & dtpToReturPB.Value.ToString("yyyy/MM/dd") & "' "
            Else
                sql_stock &= "  And " & col & "  like '%" & cr & "%' "
            End If
        End If

        gridReturPB.DataSource = execute_datatable(sql_stock & " Group by DATE_FORMAT(TanggalRetur,'%d %M %Y'),No_PO ")

        gridReturPB.Columns(1).Width = 170
    End Sub

    Private Sub setGrid()
        With gridStock.ColumnHeadersDefaultCellStyle
            .Alignment = DataGridViewContentAlignment.MiddleCenter
            .Font = New Font(.Font.FontFamily, .Font.Size, _
              .Font.Style Or FontStyle.Bold, GraphicsUnit.Point)
            .ForeColor = Color.Gold

        End With
    End Sub

    Private Sub setCmbCari()
        cmbCari.Items.Clear()
        cmbCari.Items.Add("Tanggal")
        cmbCari.Items.Add("Stock Awal")
        cmbCari.Items.Add("Qty")
        cmbCari.Items.Add("Stock Akhir")
        cmbCari.Items.Add("Module")
        cmbCari.SelectedIndex = 0
    End Sub

    Private Sub setCmbCariFaktur()
        cmbCariFaktur.Items.Clear()
        cmbCariFaktur.Items.Add("No. Faktur")
        cmbCariFaktur.Items.Add("Tanggal")
        cmbCariFaktur.Items.Add("Sales")
        cmbCariFaktur.Items.Add("Toko")
        cmbCariFaktur.Items.Add("Qty")
        cmbCariFaktur.SelectedIndex = 0
    End Sub

    Private Sub setCmbCariProd()
        cmbCariProd.Items.Clear()
        cmbCariProd.Items.Add("No. Prod")
        cmbCariProd.Items.Add("Tanggal")
        cmbCariProd.Items.Add("Qty")
        cmbCariProd.SelectedIndex = 0
    End Sub

    Private Sub setCmbCariReturFaktur()
        cmbCariReturFaktur.Items.Clear()
        cmbCariReturFaktur.Items.Add("No. Retur")
        cmbCariReturFaktur.Items.Add("Tanggal")
        cmbCariReturFaktur.Items.Add("No. SO")
        cmbCariReturFaktur.Items.Add("Sales")
        cmbCariReturFaktur.Items.Add("Toko")
        cmbCariReturFaktur.Items.Add("Qty")
        cmbCariReturFaktur.SelectedIndex = 0
    End Sub

    Private Sub setCmbCariPB()
        cmbCariPB.Items.Clear()
        cmbCariPB.Items.Add("No. Penerimaan")
        cmbCariPB.Items.Add("Tanggal")
        cmbCariPB.Items.Add("No. PO")
        cmbCariPB.Items.Add("Supplier")
        cmbCariPB.Items.Add("Qty")
        cmbCariPB.SelectedIndex = 0
    End Sub

    Private Sub setCmbCariReturPB()
        cmbCariReturPB.Items.Clear()
        cmbCariReturPB.Items.Add("No. Retur Penerimaan")
        cmbCariReturPB.Items.Add("Tanggal")
        cmbCariReturPB.Items.Add("No. PO")
        cmbCariReturPB.Items.Add("Supplier")
        cmbCariReturPB.Items.Add("Qty")
        cmbCariReturPB.SelectedIndex = 0
    End Sub

    Function visibleDate()
        dtpFrom.Visible = True
        dtpTo.Visible = True
        lblSeperator.Visible = True
        btnCari.Visible = True
        btnReset2.Visible = True

        txtCari.Visible = False
        btnReset.Visible = False
        Return False
    End Function

    Function visibleCari()
        dtpFrom.Visible = False
        dtpTo.Visible = False
        lblSeperator.Visible = False
        btnCari.Visible = False
        btnReset2.Visible = False

        txtCari.Visible = True
        btnReset.Visible = True
        Return False
    End Function

    Function visibleDateProd()
        dtpFromProd.Visible = True
        dtpToProd.Visible = True
        lblSeperatorProd.Visible = True
        btnCariProd.Visible = True
        btnResetProd2.Visible = True

        txtCariProd.Visible = False
        btnResetProd.Visible = False
        Return False
    End Function

    Function visibleCariProd()
        dtpFromProd.Visible = False
        dtpToProd.Visible = False
        lblSeperatorProd.Visible = False
        btnCariProd.Visible = False
        btnResetProd2.Visible = False

        txtCariProd.Visible = True
        btnResetProd.Visible = True
        Return False
    End Function

    Function visibleDateFaktur()
        dtpFromFaktur.Visible = True
        dtpToFaktur.Visible = True
        lblSeperatorFaktur.Visible = True
        btnCariFaktur.Visible = True
        btnResetFaktur2.Visible = True

        txtCariFaktur.Visible = False
        btnResetFaktur.Visible = False
        Return False
    End Function

    Function visibleCariFaktur()
        dtpFromFaktur.Visible = False
        dtpToFaktur.Visible = False
        lblSeperatorFaktur.Visible = False
        btnCariFaktur.Visible = False
        btnResetFaktur2.Visible = False

        txtCariFaktur.Visible = True
        btnResetFaktur.Visible = True
        Return False
    End Function

    Function visibleDateReturFaktur()
        dtpToReturFaktur.Visible = True
        dtpFromReturFaktur.Visible = True
        lblSeperatorReturFaktur.Visible = True
        btnCariReturFaktur.Visible = True
        btnResetReturFaktur2.Visible = True

        txtCariReturFaktur.Visible = False
        btnResetReturFaktur.Visible = False
        Return False
    End Function

    Function visibleCariReturFaktur()
        dtpToReturFaktur.Visible = False
        dtpFromReturFaktur.Visible = False
        lblSeperatorReturFaktur.Visible = False
        btnCariReturFaktur.Visible = False
        btnResetReturFaktur2.Visible = False

        txtCariReturFaktur.Visible = True
        btnResetReturFaktur.Visible = True
        Return False
    End Function

    Function visibleDatePB()
        dtpFromPB.Visible = True
        dtpToPB.Visible = True
        lblSeperatorPB.Visible = True
        btnCariPB.Visible = True
        btnResetPB2.Visible = True

        txtCariPB.Visible = False
        btnResetPB.Visible = False
        Return False
    End Function

    Function visibleCariPB()
        dtpFromPB.Visible = False
        dtpToPB.Visible = False
        lblSeperatorPB.Visible = False
        btnCariPB.Visible = False
        btnResetPB2.Visible = False

        txtCariPB.Visible = True
        btnResetPB.Visible = True
        Return False
    End Function

    Function visibleDateReturPB()
        dtpFromReturPB.Visible = True
        dtpToReturPB.Visible = True
        lblSeperatorReturPB.Visible = True
        btnCariReturPB.Visible = True
        btnResetReturPB2.Visible = True

        txtCariReturPB.Visible = False
        btnResetReturPB.Visible = False
        Return False
    End Function

    Function visibleCariReturPB()
        dtpFromReturPB.Visible = False
        dtpToReturPB.Visible = False
        lblSeperatorReturPB.Visible = False
        btnCariReturPB.Visible = False
        btnResetReturPB2.Visible = False

        txtCariReturPB.Visible = True
        btnResetReturPB.Visible = True
        Return False
    End Function

    Private Sub FormMsSupplier_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        tab = " MsBarang "
        PK = data_carier(0)
        clear_variable_array()
        visibleDate()
        setCmbCari()
        setCmbCariFaktur()
        setCmbCariReturFaktur()
        setCmbCariPB()
        setCmbCariProd()
        setCmbCariReturPB()
        viewAllData("", "")
        viewAllDataFaktur("", "")
        viewAllDataProd("", "")
        viewAllDataReturFaktur("", "")
        viewAllDataPB("", "")
        viewAllDataReturPB("", "")
        posisi = 0
        setGrid()
    End Sub

    Private Sub cmbCari_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmbCari.SelectedIndexChanged
        If cmbCari.SelectedIndex = 0 Then
            visibleDate()
        Else
            visibleCari()
        End If
    End Sub

    Private Sub btnCari_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnCari.Click
        viewAllData("", cmbCari.Text)
    End Sub

    Private Sub btnReset_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnReset.Click
        txtCari.Text = ""
        viewAllData("", "")
    End Sub

    Private Sub btnReset2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnReset2.Click
        txtCari.Text = ""
        viewAllData("", "")
    End Sub

    Private Sub txtCari_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtCari.TextChanged
        viewAllData(txtCari.Text, cmbCari.Text)
    End Sub

    Private Sub cmbCariFaktur_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles cmbCariFaktur.SelectedIndexChanged
        If cmbCariFaktur.SelectedIndex = 1 Then
            visibleDateFaktur()
        Else
            visibleCariFaktur()
        End If
    End Sub

    Private Sub btnCariFaktur_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnCariFaktur.Click
        viewAllDataFaktur("", cmbCariFaktur.Text)
    End Sub

    Private Sub btnResetFaktur_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnResetFaktur.Click
        txtCariFaktur.Text = ""
        viewAllDataFaktur("", "")
    End Sub

    Private Sub btnResetFaktur2_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnResetFaktur2.Click
        txtCariFaktur.Text = ""
        viewAllDataFaktur("", "")
    End Sub

    Private Sub txtCariFaktur_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtCariFaktur.TextChanged
        viewAllDataFaktur(txtCariFaktur.Text, cmbCariFaktur.Text)
    End Sub

    Private Sub cmbCariReturFaktur_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles cmbCariReturFaktur.SelectedIndexChanged
        If cmbCariReturFaktur.SelectedIndex = 1 Then
            visibleDateReturFaktur()
        Else
            visibleCariReturFaktur()
        End If
    End Sub

    Private Sub btnCariReturFaktur_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnCariReturFaktur.Click
        viewAllDataReturFaktur("", cmbCariReturFaktur.Text)
    End Sub

    Private Sub btnResetReturFaktur2_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnResetReturFaktur2.Click
        txtCariReturFaktur.Text = ""
        viewAllDataReturFaktur("", "")
    End Sub

    Private Sub btnResetReturFaktur_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnResetReturFaktur.Click
        txtCariReturFaktur.Text = ""
        viewAllDataReturFaktur("", "")
    End Sub

    Private Sub txtCariReturFaktur_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtCariReturFaktur.TextChanged
        viewAllDataReturFaktur(txtCariReturFaktur.Text, cmbCariReturFaktur.Text)
    End Sub

    Private Sub cmbCariPB_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmbCariPB.SelectedIndexChanged
        If cmbCariPB.SelectedIndex = 1 Then
            visibleDatePB()
        Else
            visibleCariPB()
        End If
    End Sub

    Private Sub btnCariPB_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnCariPB.Click
        viewAllDataPB("", cmbCariPB.Text)
    End Sub

    Private Sub btnResetPB_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnResetPB.Click
        txtCariPB.Text = ""
        viewAllDataPB("", "")
    End Sub

    Private Sub btnResetPB2_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnResetPB2.Click
        txtCariPB.Text = ""
        viewAllDataPB("", "")
    End Sub

    Private Sub txtCariPB_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtCariPB.TextChanged
        viewAllDataPB(txtCariPB.Text, cmbCariPB.Text)
    End Sub

    Private Sub cmbCariReturPB_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles cmbCariReturPB.SelectedIndexChanged
        If cmbCariReturPB.SelectedIndex = 0 Then
            visibleDateReturPB()
        Else
            visibleCariReturPB()
        End If
    End Sub

    Private Sub btnCariReturPB_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnCariReturPB.Click
        viewAllDataReturPB("", cmbCariReturPB.Text)
    End Sub

    Private Sub btnResetReturPB_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnResetReturPB.Click
        txtCariReturPB.Text = ""
        viewAllDataReturPB("", "")
    End Sub

    Private Sub btnResetReturPB2_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnResetReturPB2.Click
        txtCariReturPB.Text = ""
        viewAllDataReturPB("", "")
    End Sub

    Private Sub txtCariReturPB_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtCariReturPB.TextChanged
        viewAllDataReturPB(txtCariReturPB.Text, cmbCariReturPB.Text)
    End Sub

    Private Sub cmbCariProd_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmbCariProd.SelectedIndexChanged
        If cmbCariProd.SelectedIndex = 1 Then
            visibleDateProd()
        Else
            visibleCariProd()
        End If
    End Sub

    Private Sub btnCariProd_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnCariProd.Click
        viewAllDataProd("", cmbCariProd.Text)
    End Sub

    Private Sub btnResetProd2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnResetProd2.Click
        txtCariProd.Text = ""
        viewAllDataProd("", "")
    End Sub

    Private Sub btnResetProd_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnResetProd.Click
        txtCariProd.Text = ""
        viewAllDataProd("", "")
    End Sub

    Private Sub txtCariProd_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles txtCariProd.TextChanged
        viewAllDataProd(txtCariProd.Text, cmbCariProd.Text)
    End Sub
End Class
