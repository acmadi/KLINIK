Imports System.Data.SqlClient
Public Class FormSOManagamen
    Dim status As String
    Dim tab As String
    Dim PK As String = ""
    Dim NamaBarang As String = ""
    Dim Merk As String = ""
    Dim SubCat As String = ""
    Dim tempDisc = 0
    Dim tempHargaDisc = 0
    Dim tempHargaJual = 0
    Dim KomisiSales = 0
    Dim is_access As Integer

    Private Sub msgWarning(ByVal str As String)
        MsgBox(str, MsgBoxStyle.Critical, "Warning")
    End Sub

    Private Sub msgInfo(ByVal str As String)
        MsgBox(str, MsgBoxStyle.Information, "Information")
    End Sub

    Private Sub emptyField()
        dtpSO.Text = Now()
        cmbSales.SelectedIndex = 0
        cmbToko.SelectedIndex = 0
        lblDaerah.Text = ""
        lblProvinsi.Text = ""
        emptyBarang()
    End Sub

    'FlagStatus : StatusSO,Stock
    Function check_stock(ByVal KdBarang As String, ByVal addQty As String, ByVal FlagStatus As String)
        Dim readerStock = execute_reader(" Select sum(Qty) - IfNull(( Select sum(Qty) from TrSalesOrderDetail " & _
                                         " Join TrSalesOrder On TrSalesOrderDetail.kdso = TrSalesOrder.kdso " & _
                                         " Where KdBarang = MsBarangList.KdBarang " & _
                                         " And StatusSo = 3 " & _
                                         " And TrSalesOrder.KdSO <> '" & txtID.Text & "' " & _
                                         " Group By KdBarang ),0) As Stock From MsBarangList " & _
                                         " Where KdBarang = '" & KdBarang & "' " & _
                                         " And StatusBarangList = 0 " & _
                                         " Group By KdBarang ")
        If readerStock.read Then
            If FlagStatus = "StatusSO" And (Val(readerStock(0)) < Val(addQty)) Then
                readerStock.close()
                Return "Pending"
            ElseIf FlagStatus = "Stock" Then
                Dim readyStock = Val(readerStock(0))
                If Val(readerStock(0)) < 0 Then
                    readyStock = 0
                End If
                Return Val(readyStock)
            End If
        Else
            If FlagStatus = "StatusSO" And (0 < Val(addQty)) Then
                readerStock.close()
                Return "Pending"
            ElseIf FlagStatus = "Stock" Then
                Return 0
            End If
        End If
        readerStock.close()
        If FlagStatus = "Stock" Then
            Return 0
        End If
        Return "Ready"
    End Function

    Private Sub emptyBarang()
        cmbBarang.SelectedIndex = 0
        txtHargaJual.Text = ""
        txtHargaDisc.Text = ""
        txtQty.Text = ""
        txtDisc.Text = 0
    End Sub

    Function getSO(Optional ByVal KdSO As String = "")
        Dim sql As String = "Select KdSO from TrSalesOrder order by no_increment desc "

        If KdSO <> "" Then
            sql &= "KdSO = '" & KdSO & "'"
        End If
        Dim reader = execute_reader(sql)
        Return reader
    End Function

    Private Sub setData()
        Try
            Dim Sales = ""
            Dim Toko = ""
            Dim readerSO = execute_reader(" select KdSO,DATE_FORMAT(TanggalSO, '%m/%d/%Y') Tanggal, " & _
                            " MS.KdSales, NamaSales, " & _
                            " MT.KdToko, NamaToko, MT.Daerah, NamaArea,StatusSO from TrSalesOrder SO " & _
                            " Join MsSales MS On MS.KdSales = SO.KdSales " & _
                            " Join MsToko MT On MT.KdToko = SO.KdToko " & _
                            " Join MsArea MP On MP.KdArea = MT.KdArea" & _
                            " Where KdSO = '" & PK & "' ")
            If readerSO.Read Then
                txtID.Text = readerSO.Item(0)
                dtpSO.Text = readerSO.Item(1)
                lblDaerah.Text = readerSO.Item(6)
                lblProvinsi.Text = readerSO.Item(7)
                Sales = readerSO.Item(2) & " ~ " & readerSO.Item(3)
                Toko = readerSO.Item(4) & " ~ " & readerSO.Item(5)
                If readerSO.Item(8) <> 0 And readerSO.Item(8) <> 1 Then
                    btnSave.Enabled = False
                    btnConfirms.Enabled = False
                End If
            End If
            readerSO.Close()
            cmbSales.Text = Sales
            cmbToko.Text = Toko

            Dim reader = execute_reader(" Select MB.KdBarang,NamaBarang,Subkategori, " & _
                                    " Harga,sum(Qty) Qty,Disc, " & _
                                    " ifnull(( Select StockAkhir " & _
                                    " from BarangHistory where isActive = 1 " & _
                                    " And KdBarang = SO.KdBarang " & _
                                    " order by KdBarangHistory desc limit 1 ),0) stock, " & _
                                    " SO.StatusBarang,merk, HargaDisc " & _
                                    " from TrSalesOrderDetail SO " & _
                                    " Join MsBarang MB On SO.KdBarang = MB.KdBarang " & _
                                    " Join MsMerk On MsMerk.kdmerk = MB.kdMerk " & _
                                    " Join MsSubkategori On MsSubkategori.KdSub = MB.KdSub " & _
                                    " where KdSO = '" & PK & "' " & _
                                    " And StatusBarang <> 1 " & _
                                    " GROUP BY MB.KdBarang,SO.StatusBarang,Harga " & _
                                    " order by NamaBarang asc ")

            Do While reader.Read
                Dim Subtotal = Val(reader("HargaDisc")) * Val(reader("Qty"))
                Dim HargaDisc = Val(reader("HargaDisc"))
                Dim StatusBarang = "Ready"

                If reader("StatusBarang") = 2 Then
                    StatusBarang = "Faktur"
                Else
                    If reader("StatusBarang") = 1 Then
                        StatusBarang = "Ready - Pending"
                    End If
                End If
                gridBarang.Rows.Add()
                gridBarang.Rows.Item(gridBarang.RowCount - 1).Cells("clmPartNo").Value = reader("KdBarang")
                gridBarang.Rows.Item(gridBarang.RowCount - 1).Cells("clmNamaBarang").Value = reader("NamaBarang")
                gridBarang.Rows.Item(gridBarang.RowCount - 1).Cells("clmMerk").Value = reader("merk")
                gridBarang.Rows.Item(gridBarang.RowCount - 1).Cells("clmSubKategori").Value = reader("Subkategori")
                gridBarang.Rows.Item(gridBarang.RowCount - 1).Cells("clmHarga").Value = FormatNumber(reader("Harga"), 0)
                gridBarang.Rows.Item(gridBarang.RowCount - 1).Cells("clmHargaDisc").Value = FormatNumber(HargaDisc, 0)
                gridBarang.Rows.Item(gridBarang.RowCount - 1).Cells("clmDisc").Value = Math.Round(reader("Disc"), 2)
                gridBarang.Rows.Item(gridBarang.RowCount - 1).Cells("clmQty").Value = reader("Qty")
                gridBarang.Rows.Item(gridBarang.RowCount - 1).Cells("clmSubtotal").Value = FormatNumber(Subtotal, 0)
                gridBarang.Rows.Item(gridBarang.RowCount - 1).Cells("clmStock").Value = reader("stock")
                gridBarang.Rows.Item(gridBarang.RowCount - 1).Cells("clmQtyOri").Value = reader("Qty")
                gridBarang.Rows.Item(gridBarang.RowCount - 1).Cells("clmStatus").Value = StatusBarang
            Loop
            reader.Close()

            HitungTotal()
        Catch ex As Exception
        End Try
    End Sub

    Private Sub setGrid()
        With gridBarang.ColumnHeadersDefaultCellStyle
            .Alignment = DataGridViewContentAlignment.MiddleCenter
            .Font = New Font(.Font.FontFamily, .Font.Size, _
              .Font.Style Or FontStyle.Bold, GraphicsUnit.Point)
            .ForeColor = Color.Gold

        End With
    End Sub

    Public Sub setCmbSales()
        Dim varT As String = ""
        cmbSales.Items.Clear()
        cmbSales.Items.Add("- Pilih Sales -")
        Dim reader = execute_reader("Select * from MsSales  where NamaSales <>'' order by NamaSales asc")
        Do While reader.Read
            cmbSales.Items.Add(reader(0) & " ~ " & reader(1))
        Loop
        reader.Close()
        If cmbSales.Items.Count > 0 Then
            cmbSales.SelectedIndex = 0
        End If
        reader.Close()
    End Sub

    Public Sub setCmbToko()
        Dim varT As String = ""
        cmbToko.Items.Clear()
        cmbToko.Items.Add("- Pilih Toko -")
        sql = " Select * from MsToko" & _
              " Join MsEkspedisi On MsEkspedisi.KdEkspedisi = MsToko.KdEkspedisi " & _
              " Join MsArea On MsArea.KdArea = MsToko.KdArea " & _
              " where NamaToko <>'' order by NamaToko asc "
        Dim readerToko = execute_reader(sql)
        Do While readerToko.Read
            cmbToko.Items.Add(readerToko(0) & " ~ " & readerToko(1))
        Loop
        readerToko.Close()
        If cmbToko.Items.Count > 0 Then
            cmbToko.SelectedIndex = 0
        End If
        readerToko.Close()
    End Sub

    Public Sub setCmbBarang()
        Dim varT As String = ""
        cmbBarang.Items.Clear()
        cmbBarang.Items.Add("- Pilih Barang -")
        Dim reader = execute_reader("Select KdBarang,NamaBarang from MsBarang where 1 " & _
                                    QueryLevel(lvlKaryawan) & _
                                    " And NamaBarang <>'' order by NamaBarang asc")
        Do While reader.Read
            cmbBarang.Items.Add(reader("KdBarang") & " ~ " & reader("NamaBarang"))
        Loop
        reader.Close()
        If cmbBarang.Items.Count > 0 Then
            cmbBarang.SelectedIndex = 0
        End If
        reader.Close()
    End Sub

    Private Sub FormMsSupplier_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        is_access = get_access("so")
        PK = data_carier(0)
        status = data_carier(1)
        clear_variable_array()
        setCmbSales()
        setCmbToko()
        setCmbBarang()
        emptyField()

        If PK = "" Then
            If is_access = 3 Or is_access = 4 Or is_access = 5 Then
                btnSave.Enabled = True
                btnConfirms.Enabled = True
            End If
            generateCode()
        Else
            If is_access = 2 Or is_access = 4 Or is_access = 5 Then
                btnSave.Enabled = True
                btnConfirms.Enabled = True
            End If
            setData()
            txtID.Text = PK
        End If
        cmbSales.Focus()
    End Sub

    Private Sub generateCode()
        Dim code As String = "SO"
        Dim angka As Integer
        Dim kode As String
        Dim temp As String
        Dim tempCompare As String = ""

        Dim bulan As Integer = CInt(Today.Month.ToString)
        Dim currentTime As System.DateTime = System.DateTime.Now
        Dim FormatDate As String = Format(currentTime, ".dd.MM.yy")
        tempCompare &= FormatDate

        Dim reader = getSO()

        If reader.read Then
            kode = Trim(reader(0).ToString())
            temp = kode.Substring(6, 9)
            If temp = tempCompare Then
                angka = CInt(kode.Substring(2, 4))
            Else
                angka = 0
            End If
            reader.Close()
        Else
            angka = 0
            reader.Close()
        End If
        angka = angka + 1
        Dim len As Integer = angka.ToString().Length
        For i As Integer = 1 To 4 - len
            code += "0"
        Next
        code = code & (angka) & FormatDate
        txtID.Text = Trim(code)
    End Sub

    Function SaveSODetail(ByVal flag As String)
        Dim sqlHistory = ""
        Dim sqlDetail = ""
        Dim statusSO = 0 + Val(flag)
        Dim QtyReady = 0

        For i As Integer = 0 To gridBarang.RowCount - 1
            Dim statusDetail = 0
            Dim harga = gridBarang.Rows.Item(i).Cells("clmHarga").Value.ToString.Replace(".", "").Replace(",", "")
            Dim OldQty = Val(gridBarang.Rows.Item(i).Cells("clmQtyOri").Value)
            Dim Stok = Val(gridBarang.Rows.Item(i).Cells("clmStock").Value)
            Dim Qty = Val(gridBarang.Rows.Item(i).Cells("clmQty").Value)
            Dim OP = "Min"
            Dim Attribute = "QtySO_Min"
            Dim KdBarang = gridBarang.Rows.Item(i).Cells("clmPartNo").Value
            Dim HargaDisc = gridBarang.Rows.Item(i).Cells("clmHargaDisc").Value.ToString.Replace(".", "").Replace(",", "")

            If gridBarang.Rows.Item(i).Cells(11).Value = "Ready" Then
                statusDetail = 0
                QtyReady = Qty
            ElseIf gridBarang.Rows.Item(i).Cells(11).Value = "Pending" Then
                statusDetail = 1
                statusSO = 1 + Val(flag)

                Dim qtyStock = check_stock(KdBarang, Qty, "Stock")
                Dim QtyPending = Qty - qtyStock
                QtyReady = Qty - QtyPending

                sqlDetail = " insert into trsalesorderdetailpending(KdSO,KdBarang, Harga, " & _
                            " Qty, Disc,StatusBarang,HargaDisc) values( " & _
                            " '" & Trim(txtID.Text) & "','" & KdBarang & "', " & _
                            " '" & harga & "', '" & QtyPending & "', " & _
                            " '" & gridBarang.Rows.Item(i).Cells("clmDisc").Value & "', " & _
                            " '" & statusDetail & "', '" & HargaDisc & "')"
                execute_update_manual(sqlDetail)
            End If

            If QtyReady <> 0 Then
                sqlDetail = " insert into trsalesorderdetail(KdSO,KdBarang, Harga, " & _
                            " Qty, Disc,StatusBarang, HargaDisc) values( " & _
                            " '" & Trim(txtID.Text) & "','" & KdBarang & "', " & _
                            " '" & harga & "', '" & QtyReady & "', " & _
                            " '" & gridBarang.Rows.Item(i).Cells("clmDisc").Value & "', " & _
                            " '0', '" & HargaDisc & "')"
                execute_update_manual(sqlDetail)
            End If
        Next

        If flag = 3 And statusSO = 4 Then
            statusSO = 3
            msgInfo("Barang pending tidak dapat di confirm dan akan masuk pada tabel pending.")
        End If

        Dim sqlHeader = " Update TrSalesOrder set StatusSO = '" & statusSO & "' " & _
                    " Where KdSO = '" & Trim(txtID.Text) & "' "
        execute_update_manual(sqlHeader)
        Return True
    End Function

    Function save(ByVal flag As String)
        If cmbSales.SelectedIndex = 0 Then
            msgInfo("Sales harus dipilih")
            cmbSales.Focus()
        ElseIf cmbToko.SelectedIndex = 0 Then
            msgInfo("Toko harus dipilih")
            cmbToko.Focus()
        ElseIf gridBarang.RowCount = 0 Then
            msgInfo("Tambah barang terlebih dahulu")
            cmbBarang.Focus()
        Else
            Dim salesID = cmbSales.Text.ToString.Split("~")
            Dim tokoID = cmbToko.Text.ToString.Split("~")
            Dim Grandtotal = ""

            If (lblGrandtotal.Text <> "") Then
                Grandtotal = Math.Round(lblGrandtotal.Text.ToString.Replace(".", "").Replace(",", "") * 1, 2)
            End If

            Dim ReaderGrandDetail = execute_reader(" Select IfNull(sum(HargaDisc * Qty),0) from TrSalesOrderDetail " & _
                                         " Where KdSO = '" & txtID.Text & "' " & _
                                         " And StatusBarang = 1 ")

            If ReaderGrandDetail.read Then
                Grandtotal += Math.Round(ReaderGrandDetail(0) * 1, 2)
            End If
            ReaderGrandDetail.close()

            dbconmanual.Open()
            Dim trans As MySql.Data.MySqlClient.MySqlTransaction

            trans = dbconmanual.BeginTransaction(IsolationLevel.ReadCommitted)

            Try
                If status = "add" Then
                    sql = "insert into TrSalesOrder ( KdSO, TanggalSO, KdSales, " & _
                          " KdToko, GrandTotal, StatusSO, UserID, KomisiSales ) " & _
                          " values('" + Trim(txtID.Text) + "', " & _
                          " '" + dtpSO.Value.ToString("yyyy/MM/dd HH:mm:ss") + "', " & _
                          " '" & Trim(salesID(0)) & "', '" & Trim(tokoID(0)) & "'," & _
                          " '" & Trim(Grandtotal) & "', '" & flag & "','" & kdKaryawan & "', " & _
                          " '" & KomisiSales & "')"
                    execute_update_manual(sql)
                ElseIf status = "update" Then
                    Try
                        sql = " update TrSalesOrder set " & _
                              " TanggalSO='" & dtpSO.Value.ToString("yyyy/MM/dd HH:mm:ss") & "'," & _
                              " KdSales='" & Trim(salesID(0)) & "'," & _
                              " KdToko='" & Trim(tokoID(0)) & "'," & _
                              " GrandTotal='" & Trim(Grandtotal) & "', " & _
                              " UserID='" & Trim(kdKaryawan) & "', " & _
                              " KomisiSales='" & Trim(KomisiSales) & "' " & _
                              " where  KdSO = '" + txtID.Text + "' "
                        execute_update_manual(sql)
                    Catch ex As Exception
                        msgWarning(" Data tidak valid")
                        cmbSales.SelectedIndex = 0
                    End Try
                End If

                execute_update_manual(" delete from TrSalesOrderDetail " & _
                                      " where  KdSO = '" & txtID.Text & "' " & _
                                      " And statusBarang = 0 " & _
                                      " And StatusBarang <> 1 ")
                Dim statusSO = SaveSODetail(flag)

                trans.Commit()
                msgInfo("Data berhasil disimpan")
                Me.Close()
            Catch ex As Exception
                trans.Rollback()
                msgWarning("Data tidak valid")
            End Try
            dbconmanual.Close()
        End If
        Return True
    End Function

    Private Sub btnSave_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSave.Click
        save(0)
    End Sub

    Private Sub btnExit_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnExit.Click
        Me.Close()
    End Sub

    Private Sub HitungTotal()
        Try
            Dim Grandtotal = 0
            If gridBarang.Rows.Count <> 0 Then
                For i As Integer = 0 To (gridBarang.Rows.Count - 1)
                    Dim total = gridBarang.Rows.Item(i).Cells("clmSubtotal").Value.ToString.Replace(".", "").Replace(",", "")
                    Grandtotal = Val(Grandtotal) + Val(total)
                Next
            End If
            lblGrandtotal.Text = FormatNumber(Grandtotal, 0)
        Catch ex As Exception
            MsgBox(ex.ToString, MsgBoxStyle.Critical, "Warning!!!")
        End Try
    End Sub

    Private Sub btnAdd_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnAdd.Click
        Try
            check_disc()
            If cmbBarang.SelectedIndex = 0 Then
                msgInfo("Barang harus dipilih")
                cmbBarang.Focus()
            ElseIf txtQty.Text = "" Then
                msgInfo("Jumlah harus diisi")
                txtQty.Focus()
            ElseIf txtHargaJual.Text = "" Then
                msgInfo("Harga list harus diisi")
                txtHargaJual.Focus()
            ElseIf txtHargaDisc.Text = "" Then
                msgInfo("Harga Jual harus diisi")
                txtHargaDisc.Focus()
            ElseIf txtQty.Text = "0" Then
                msgInfo("Jumlah harus lebih dari 0")
                txtQty.Focus()
            Else
                Dim barangID = cmbBarang.Text.ToString.Split("~")
                Dim harga = Val(txtHargaJual.Text.ToString.Replace(".", "").Replace(",", ""))
                Dim StatusBarang = "Ready"
                Dim discount = txtDisc.Text * 1
                Dim HargaDisc = Val(txtHargaDisc.Text.ToString.Replace(".", "").Replace(",", ""))

                For i As Integer = 0 To (gridBarang.RowCount - 1)
                    If gridBarang.Rows(i).Cells("clmPartNo").Value.ToString = Trim(barangID(0)) Then
                        Dim addQty = Val(txtQty.Text) + gridBarang.Rows.Item(i).Cells("clmQty").Value
                        Dim Subtot = HargaDisc * Val(addQty)

                        StatusBarang = check_stock(barangID(0), addQty, "StatusSO")

                        gridBarang.Rows.Item(i).Cells("clmHarga").Value = FormatNumber(harga, 0)
                        gridBarang.Rows.Item(i).Cells("clmHargaDisc").Value = FormatNumber(HargaDisc, 0)
                        gridBarang.Rows.Item(i).Cells("clmDisc").Value = Math.Round(discount, 2)
                        gridBarang.Rows.Item(i).Cells("clmQty").Value = addQty
                        gridBarang.Rows.Item(i).Cells("clmSubtotal").Value = FormatNumber(Subtot, 0)
                        gridBarang.Rows.Item(i).Cells("clmStock").Value = Val(lblStock.Text)
                        gridBarang.Rows.Item(i).Cells("clmStatus").Value = StatusBarang

                        HitungTotal()
                        emptyBarang()

                        gridBarang.Focus()

                        Exit Sub
                    End If
                Next

                StatusBarang = check_stock(barangID(0), txtQty.Text, "StatusSO")

                Dim Subtotal = HargaDisc * Val(txtQty.Text)

                gridBarang.Rows.Add()
                gridBarang.Rows.Item(gridBarang.RowCount - 1).Cells("clmPartNo").Value = Trim(barangID(0))
                gridBarang.Rows.Item(gridBarang.RowCount - 1).Cells("clmNamaBarang").Value = Trim(NamaBarang)
                gridBarang.Rows.Item(gridBarang.RowCount - 1).Cells("clmMerk").Value = Trim(Merk)
                gridBarang.Rows.Item(gridBarang.RowCount - 1).Cells("clmSubKategori").Value = Trim(SubCat)
                gridBarang.Rows.Item(gridBarang.RowCount - 1).Cells("clmHarga").Value = FormatNumber(harga, 0)
                gridBarang.Rows.Item(gridBarang.RowCount - 1).Cells("clmHargaDisc").Value = FormatNumber(HargaDisc, 0)
                gridBarang.Rows.Item(gridBarang.RowCount - 1).Cells("clmDisc").Value = Math.Round(discount, 2)
                gridBarang.Rows.Item(gridBarang.RowCount - 1).Cells("clmQty").Value = Val(txtQty.Text)
                gridBarang.Rows.Item(gridBarang.RowCount - 1).Cells("clmSubtotal").Value = FormatNumber(Subtotal, 0)
                gridBarang.Rows.Item(gridBarang.RowCount - 1).Cells("clmStock").Value = Val(lblStock.Text)
                gridBarang.Rows.Item(gridBarang.RowCount - 1).Cells("clmStatus").Value = StatusBarang

                HitungTotal()
                emptyBarang()

                gridBarang.Focus()
                'End If
            End If
        Catch ex As Exception
            MsgBox(ex.ToString, MsgBoxStyle.Critical, "Warning!!!")
        End Try
    End Sub

    Private Sub cmbSales_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmbSales.SelectedIndexChanged
        If cmbSales.SelectedIndex <> 0 Then
            Dim salesID = cmbSales.Text.ToString.Split("~")

            sql = " Select Komisi from MsSales " & _
                  " where KdSales = '" & Trim(salesID(0)) & "' "
            Dim reader = execute_reader(sql)

            If reader.Read Then
                KomisiSales = reader(0)
            End If
            reader.Close()
            cmbToko.Focus()
            generateCode()
            txtID.Text &= "/" & salesID(0)
        Else
            KomisiSales = 0
            generateCode()
        End If
    End Sub

    Private Sub cmbToko_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmbToko.SelectedIndexChanged
        If cmbToko.SelectedIndex <> 0 Then
            Dim tokoID = cmbToko.Text.ToString.Split("~")

            sql = " Select NamaArea,Daerah from MsToko " & _
                  " Join MsEkspedisi On MsEkspedisi.KdEkspedisi = MsToko.KdEkspedisi " & _
                  " Join MsArea On MsArea.KdArea = MsToko.KdArea " & _
                  " where KdToko = '" & Trim(tokoID(0)) & "' "
            Dim reader = execute_reader(sql)

            If reader.Read Then
                lblProvinsi.Text = reader(0)
                lblDaerah.Text = reader(1)
            End If
            reader.Close()
            cmbBarang.Focus()
        Else
            lblProvinsi.Text = ""
            lblDaerah.Text = ""
        End If
    End Sub

    Private Sub cmbBarang_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmbBarang.SelectedIndexChanged
        If cmbBarang.SelectedIndex <> 0 Then
            Dim barangID = cmbBarang.Text.ToString.Split("~")

            sql = " Select StockAkhir, IfNull((Select Harga From MsBarangList " & _
                  " where KdBarang = BarangHistory.KdBarang And StatusBarangList = 0 " & _
                  " Order By KdList asc Limit 1 ),0) As HargaAwal, " & _
                  " NamaBarang, SubKategori,Merk,HargaList " & _
                  " from BarangHistory " & _
                  " Join MsBarang On MsBarang.KdBarang = BarangHistory.KdBarang " & _
                  " Join MsMerk On MsMerk.kdMerk = MsBarang.kdMerk" & _
                  " Join MsSubkategori On MsSubkategori.KdSub = MsBarang.KdSub" & _
                  " where isActive = 1 " & _
                  " And BarangHistory.kdBarang = '" & barangID(0) & "' " & _
                  " order by KdBarangHistory desc limit 1"
            Dim reader = execute_reader(sql)

            If reader.Read Then
                NamaBarang = reader(2)
                Merk = reader(4)
                SubCat = reader(3)
                txtHargaJual.Text = reader("HargaList")
            End If
            reader.Close()
            lblStock.Text = check_stock(barangID(0), 0, "Stock")
            txtQty.Focus()
        Else
            lblStock.Text = 0
            txtHargaJual.Text = 0
        End If
    End Sub

    Private Sub txtHarga_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txtHargaJual.KeyPress
        If Asc(e.KeyChar) <> Asc("-") And AscW(e.KeyChar) <> 13 And AscW(e.KeyChar) <> 8 And (Asc(e.KeyChar) < Asc("0") Or Asc(e.KeyChar) > Asc("9")) Then
            e.KeyChar = Nothing
        End If
        If AscW(e.KeyChar) = 13 And txtHargaJual.Text <> "" Then
            txtHargaDisc.Text = txtHargaJual.Text
            txtHargaDisc.Focus()
        End If
    End Sub

    Private Sub txtQty_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txtQty.KeyPress
        If Asc(e.KeyChar) <> Asc("-") And AscW(e.KeyChar) <> 13 And AscW(e.KeyChar) <> 8 And (Asc(e.KeyChar) < Asc("0") Or Asc(e.KeyChar) > Asc("9")) Then
            e.KeyChar = Nothing
        End If
        If AscW(e.KeyChar) = 13 And txtQty.Text <> "" Then
            txtHargaJual.Focus()
        End If
    End Sub

    Private Sub txtDisc_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txtDisc.KeyPress
        If AscW(e.KeyChar) = 13 And txtQty.Text <> "" Then
            btnAdd_Click(Nothing, Nothing)
        End If
    End Sub

    Private Sub browseSales_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles browseSales.Click
        Try
            sub_form = New FormBrowseSales
            sub_form.showDialog(FormMain)
            If data_carier(0) <> "" Then
                cmbSales.Text = data_carier(0) & " ~ " & data_carier(1)
                KomisiSales = data_carier(2)
                clear_variable_array()
            End If
        Catch ex As Exception
            MsgBox(ex.ToString, MsgBoxStyle.Critical, "Warning!!!")
        End Try
    End Sub

    Private Sub browseToko_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles browseToko.Click
        Try
            sub_form = New FormBrowseToko
            sub_form.showDialog(FormMain)
            If data_carier(0) <> "" Then
                cmbToko.Text = data_carier(0) & " ~ " & data_carier(1)
                lblDaerah.Text = data_carier(2)
                lblProvinsi.Text = data_carier(3)
                clear_variable_array()
            End If
        Catch ex As Exception
            MsgBox(ex.ToString, MsgBoxStyle.Critical, "Warning!!!")
        End Try
    End Sub

    Private Sub Button2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles browseBarang.Click
        Try
            sub_form = New FormBrowseBarang
            sub_form.showDialog(FormMain)
            If data_carier(0) <> "" Then
                cmbBarang.Text = data_carier(0) & " ~ " & data_carier(1)
                NamaBarang = data_carier(1)
                Merk = data_carier(2)
                SubCat = data_carier(3)
                lblStock.Text = data_carier(4)
                txtHargaJual.Text = data_carier(5)
                clear_variable_array()
                txtQty.Focus()
            End If
        Catch ex As Exception
            MsgBox(ex.ToString, MsgBoxStyle.Critical, "Warning!!!")
        End Try
    End Sub

    Private Sub txtHarga_Leave(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtHargaJual.Leave
        If txtHargaJual.Text <> "" And txtQty.Text <> "" Then
            txtHargaJual.Text = FormatNumber(txtHargaJual.Text, 0)
            Dim hargaJual = Val(txtHargaDisc.Text.ToString.Replace(".", "").Replace(",", ""))
            Dim hargaList = Val(txtHargaJual.Text.ToString.Replace(".", "").Replace(",", ""))
            Dim calcDisc = 100 - (Val(hargaJual / hargaList) * 100)
            txtHargaDisc.Text = FormatNumber(txtHargaDisc.Text, 0)
            txtDisc.Text = Math.Round(calcDisc, 2)
        End If
    End Sub

    Private Sub btnRemove_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnRemove.Click
        If gridBarang.RowCount <> 0 Then
            Try
                If gridBarang.CurrentRow.Cells("clmStatus").Value = "Ready - Pending" Then
                    MsgBox("Barang ini tidak dapat dihapus karena dimiliki pending table", MsgBoxStyle.Information)
                Else
                    gridBarang.Rows.RemoveAt(gridBarang.CurrentRow.Index)
                    HitungTotal()
                End If
            Catch ex As Exception
                MsgBox(ex.ToString, MsgBoxStyle.Critical, "Warning!!!")
            End Try
        Else
            msgInfo("Belum ada barang")
        End If
    End Sub

    Private Sub gridBarang_CellBeginEdit(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewCellCancelEventArgs) Handles gridBarang.CellBeginEdit
        tempDisc = Val(gridBarang.CurrentRow.Cells("clmHargaDisc").Value)
        tempDisc = Math.Round(tempDisc, 2)
        tempHargaDisc = Val(gridBarang.CurrentRow.Cells("clmHargaDisc").Value.Replace(".", "").Replace(",", ""))
        tempHargaJual = Val(gridBarang.CurrentRow.Cells("clmHarga").Value.Replace(".", "").Replace(",", ""))
    End Sub

    Private Sub gridBarang_CellClick(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewCellEventArgs) Handles gridBarang.CellClick
        tempDisc = gridBarang.CurrentRow.Cells("clmQty").Value * 1
        tempDisc = Math.Round(tempDisc, 2)
        tempHargaDisc = Val(gridBarang.CurrentRow.Cells("clmHargaDisc").Value.Replace(".", "").Replace(",", ""))
        tempHargaJual = Val(gridBarang.CurrentRow.Cells("clmHarga").Value.Replace(".", "").Replace(",", ""))
    End Sub

    Private Sub gridBarang_CellEndEdit(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewCellEventArgs) Handles gridBarang.CellEndEdit
        Try
            Dim BarangID = gridBarang.CurrentRow.Cells("clmPartNo").Value
            Dim harga = Val(gridBarang.CurrentRow.Cells("clmHarga").Value.Replace(".", "").Replace(",", ""))
            Dim hargaDisc = Val(gridBarang.CurrentRow.Cells("clmHargaDisc").Value.Replace(".", "").Replace(",", ""))
            Dim qty = Val(gridBarang.CurrentRow.Cells("clmQty").Value)
            Dim disc = gridBarang.CurrentRow.Cells("clmDisc").Value
            Dim stok = Val(gridBarang.CurrentRow.Cells("clmStock").Value)

            If IsNumeric(qty) = False Or qty < 1 Then
                MsgBox("Jumlah harus lebih besar dari 0 dan berupa angka.", MsgBoxStyle.Information, "Validation Error")
                qty = 1
                gridBarang.CurrentRow.Cells("clmQty").Value = 1
                gridBarang.CurrentRow.Cells("clmQty").Selected = True
                'ElseIf qty > stok Then
                '    MsgBox("Jumlah tidak mencukupi persedian. Persedian saat ini adalah " & stok, MsgBoxStyle.Information, "Validation Error")
                '    qty = 1
                '    gridBarang.CurrentRow.Cells("clmHargaDisc").Value = 1
                '    gridBarang.CurrentRow.Cells("clmHargaDisc").Selected = True
            ElseIf IsNumeric(harga) = False Or harga < 1 Then
                MsgBox("Harga harus lebih besar dari 0 dan berupa angka.", MsgBoxStyle.Information, "Validation Error")
                harga = 1
                gridBarang.CurrentRow.Cells("clmHarga").Value = 1
                gridBarang.CurrentRow.Cells("clmHarga").Selected = True
            ElseIf IsNumeric(hargaDisc) = False Or hargaDisc < 1 Then
                MsgBox("Harga Disc harus lebih besar dari 0 dan berupa angka.", MsgBoxStyle.Information, "Validation Error")
                hargaDisc = 1
                gridBarang.CurrentRow.Cells("clmHargaDisc").Value = 1
                gridBarang.CurrentRow.Cells("clmHargaDisc").Selected = True
            ElseIf IsNumeric(disc) = False Then
                MsgBox("Diskon harus berupa angka.", MsgBoxStyle.Information, "Validation Error")
                disc = 0
                gridBarang.CurrentRow.Cells("clmDisc").Value = 0
                gridBarang.CurrentRow.Cells("clmDisc").Selected = True
            Else
                Dim TempHarga = FormatNumber(harga, 0)
                gridBarang.CurrentRow.Cells("clmHarga").Value = TempHarga
            End If

            If hargaDisc <> tempHargaDisc Or harga <> tempHargaJual Then
                Dim calcDisc = 100 - (hargaDisc / harga * 100)
                gridBarang.CurrentRow.Cells("clmDisc").Value = Math.Round(calcDisc, 2)
            ElseIf disc <> tempDisc Then
                Dim discProses = 100 - Math.Round(disc * 1, 2)
                Dim calcHargaJual = harga * (discProses / 100)
                gridBarang.CurrentRow.Cells("clmHargaDisc").Value = FormatNumber(calcHargaJual, 0)
            End If
            Dim hargaDisc_temp = Val(gridBarang.CurrentRow.Cells("clmHargaDisc").Value.Replace(".", "").Replace(",", ""))

            gridBarang.CurrentRow.Cells(8).Value = FormatNumber((hargaDisc_temp * Val(qty)), 0)
            gridBarang.CurrentRow.Cells("clmStatus").Value = check_stock(BarangID, qty, "StatusSO")

            HitungTotal()
        Catch ex As Exception
            MsgBox(ex.ToString, MsgBoxStyle.Critical, "Warning!!!")
        End Try
    End Sub

    Private Sub btnConfirms_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnConfirms.Click
        save(3)
    End Sub

    Private Sub txtHargaJual_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txtHargaDisc.KeyPress
        If Asc(e.KeyChar) <> Asc("-") And AscW(e.KeyChar) <> 13 And AscW(e.KeyChar) <> 8 And (Asc(e.KeyChar) < Asc("0") Or Asc(e.KeyChar) > Asc("9")) Then
            e.KeyChar = Nothing
        End If
        If AscW(e.KeyChar) = 13 And txtHargaDisc.Text <> "" Then
            txtDisc.Focus()
        End If
    End Sub

    Private Sub txtHargaJual_Leave(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtHargaDisc.Leave
        If txtHargaDisc.Text <> "" And txtHargaJual.Text <> "" And txtQty.Text <> "" Then
            Dim hargaJual = Val(txtHargaDisc.Text.ToString.Replace(".", "").Replace(",", ""))
            Dim hargaList = Val(txtHargaJual.Text.ToString.Replace(".", "").Replace(",", ""))
            Dim calcDisc = 100 - (hargaJual / hargaList * 100)
            txtHargaDisc.Text = FormatNumber(txtHargaDisc.Text, 0)
            txtDisc.Text = Math.Round(calcDisc, 2)
        End If
    End Sub

    Function check_disc()
        If IsNumeric(txtDisc.Text) Then
            Dim disc = txtDisc.Text * 1
            disc = 100 - Math.Round(disc, 2)
            Dim hargaList = Val(txtHargaJual.Text.ToString.Replace(".", "").Replace(",", ""))
            Dim calcHargaJual = hargaList * (disc / 100)
            txtHargaDisc.Text = FormatNumber(calcHargaJual, 0)
        Else
            MsgBox("Diskon harus berupa angka.", MsgBoxStyle.Information, "Validation Error")
            txtDisc.Text = 0
            txtDisc.Focus()
        End If
        Return True
    End Function

    Private Sub txtDisc_Leave(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtDisc.Leave
        check_disc()
    End Sub

    Private Sub txtHargaJual_LocationChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtHargaJual.LocationChanged

    End Sub

    Private Sub txtHargaJual_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles txtHargaJual.TextChanged

    End Sub
End Class
