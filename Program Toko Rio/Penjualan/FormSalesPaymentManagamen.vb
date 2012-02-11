Imports System.Data.SqlClient
Public Class FormSalesPaymentManagamen
    Dim status As String
    Dim posisi As Integer = 0
    Dim tab As String
    Dim jumData As Integer = 0
    Dim PK As String = ""
    Dim NamaBarang As String = ""
    Dim Merk As String = ""
    Dim SubCat As String = ""
    Dim kdsales As String = ""
    Dim kdtoko As String = ""
    Dim is_access As Integer

    Private Sub msgWarning(ByVal str As String)
        MsgBox(str, MsgBoxStyle.Critical, "Warning")
    End Sub

    Private Sub msgInfo(ByVal str As String)
        MsgBox(str, MsgBoxStyle.Information, "Information")
    End Sub

    Private Sub emptyField()
        dtpPayment.Text = Now()
        cmbFaktur.SelectedIndex = 0
        txtSales.Text = ""
        txtToko.Text = ""
        lblDaerah.Text = ""
        lblProvinsi.Text = ""
        cmbBayar.SelectedIndex = 0
    End Sub

    Function getPayment(Optional ByVal KdPayment As String = "")
        Dim sql As String = "Select KdSalesPayment from trSalesPayment order by no_increment desc "

        If KdPayment <> "" Then
            sql &= "KdSalesPayment = '" & KdPayment & "'"
        End If
        Dim reader = execute_reader(sql)
        Return reader
    End Function

    Private Sub setData()
        Try
            Dim Sales = ""
            Dim Toko = ""
            Dim kdFaktur = ""
            Dim readerPayment = execute_reader(" select KdSalesPayment,DATE_FORMAT(TanggalSalesPayment, '%m/%d/%Y') Tanggal, " & _
                                               " kdFaktur, MS.KdSales, NamaSales, " & _
                                               " MT.KdToko, NamaToko, MT.Daerah, NamaArea,StatusSalesPayment, " & _
                                               " Note, TotalSalesPayment,PaidBy from trSalesPayment payment " & _
                                               " Join MsSales MS On MS.KdSales = payment.KdSales " & _
                                               " Join MsToko MT On MT.KdToko = payment.KdToko " & _
                                               " Join MsArea MP On MP.KdArea = MT.KdArea" & _
                                               " Where kdSalesPayment = '" & PK & "' ")

            If readerPayment.Read Then
                txtID.Text = readerPayment.Item("KdSalesPayment")
                dtpPayment.Text = readerPayment.Item("Tanggal")
                lblDaerah.Text = readerPayment.Item("Daerah")
                'lblProvinsi.Text = readerPayment.Item(8)
                kdFaktur = readerPayment.Item("kdFaktur")
                Sales = readerPayment.Item("KdSales") & " - " & readerPayment.Item("NamaSales")
                Toko = readerPayment.Item("KdToko") & " - " & readerPayment.Item("NamaToko")
                If readerPayment.Item("StatusSalesPayment") <> 0 Then
                    btnSave.Enabled = False
                    btnConfirms.Enabled = False
                End If
                txtNote.Text = readerPayment.Item("Note")
                txtPaid.Text = FormatNumber(readerPayment.Item("TotalSalesPayment"), 0)
                cmbBayar.Text = readerPayment.Item("PaidBy")
            End If
            readerPayment.Close()
            txtSales.Text = Sales
            txtToko.Text = Toko
            cmbFaktur.Text = kdFaktur

            Dim reader = execute_reader(" Select MB.KdBarang,NamaBarang,Subkategori, " & _
                                        " HargaDisc, Harga, Qty, Disc,StatusBarang,Merk " & _
                                        " from TrsalesPaymentDetail payment " & _
                                        " Join MsBarang MB On payment.KdBarang = MB.KdBarang " & _
                                        " Join MsMerk On MsMerk.kdMerk = MB.kdMerk " & _
                                        " Join MsSubkategori On MsSubkategori.KdSub = MB.KdSub " & _
                                        " where kdsalesPayment = '" & PK & "' " & _
                                        " order by NamaBarang asc ")

            gridBarang.Rows.Clear()
            Do While reader.Read
                Dim Subtotal = Val(reader("HargaDisc")) * Val(reader("Qty"))

                gridBarang.Rows.Add()
                gridBarang.Rows.Item(gridBarang.RowCount - 1).Cells("clmPartNo").Value = reader("KdBarang")
                gridBarang.Rows.Item(gridBarang.RowCount - 1).Cells("clmNamaBarang").Value = reader("NamaBarang")
                gridBarang.Rows.Item(gridBarang.RowCount - 1).Cells("clmMerk").Value = reader("Merk")
                gridBarang.Rows.Item(gridBarang.RowCount - 1).Cells("clmSubKategori").Value = reader("Subkategori")
                gridBarang.Rows.Item(gridBarang.RowCount - 1).Cells("clmHarga").Value = FormatNumber(reader("Harga"), 0)
                gridBarang.Rows.Item(gridBarang.RowCount - 1).Cells("clmHargaDisc").Value = FormatNumber(reader("HargaDisc"), 0)
                gridBarang.Rows.Item(gridBarang.RowCount - 1).Cells("clmDisc").Value = reader("Disc")
                gridBarang.Rows.Item(gridBarang.RowCount - 1).Cells("clmQty").Value = reader("Qty")
                gridBarang.Rows.Item(gridBarang.RowCount - 1).Cells("clmSubtotal").Value = Subtotal
                gridBarang.Rows.Item(gridBarang.RowCount - 1).Cells("clmStatus").Value = reader("StatusBarang")
            Loop
            reader.Close()

            HitungTotal()
        Catch ex As Exception
            MsgBox(ex, MsgBoxStyle.Information)
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

    Public Sub setCmbFaktur()
        Dim varT As String = ""
        Dim addQuery = ""
        cmbFaktur.Items.Clear()
        cmbFaktur.Items.Add("- Pilih Faktur -")
        If PK <> "" Then
            addQuery = " And exists( Select 1 from trsalesPayment where kdsalesPayment = '" & PK & "' And kdFaktur = trfaktur.kdFaktur ) "
            cmbFaktur.Enabled = False
            BrowseFaktur.Enabled = False
        Else
            addQuery = " And statusfaktur <> 0 " & _
                       " And StatusPayment <> 1 " & _
                       " GROUP BY kdFaktur " & _
                       " HAVING SUM(GrandTotal) - " & _
                       " IFNULL(( SELECT SUM(TotalSalesPayment) " & _
                       " FROM trsalespayment WHERE trfaktur.kdFaktur = kdFaktur ),0) > 0 "
        End If

        Dim reader = execute_reader("Select kdFaktur from trfaktur " & _
                                    " Join msuser On msuser.UserID = trfaktur.UserID " & _
                                    " where 1 " & _
                                    QueryLevel(lvlKaryawan, "msuser.", "level") & _
                                    addQuery & " Order By TanggalFaktur Desc,StatusFaktur asc ")
        Do While reader.Read
            cmbFaktur.Items.Add(reader("kdFaktur"))
        Loop
        reader.Close()
        If cmbFaktur.Items.Count > 0 Then
            cmbFaktur.SelectedIndex = 0
        End If
        reader.Close()
    End Sub

    Private Sub FormMsSupplier_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        is_access = get_access("paymentJual")
        PK = data_carier(0)
        status = data_carier(1)
        clear_variable_array()
        setCmbFaktur()
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
        cmbFaktur.Focus()
    End Sub

    Private Sub generateCode()
        Dim code As String = "PF"
        Dim angka As Integer
        Dim kode As String
        Dim temp As String
        Dim tempCompare As String = ""

        Dim bulan As Integer = CInt(Today.Month.ToString)
        Dim currentTime As System.DateTime = System.DateTime.Now
        Dim FormatDate As String = Format(currentTime, ".dd.MM.yy")
        tempCompare &= FormatDate

        Dim reader = getPayment()

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

    Function SavePaymentDetail(ByVal flag As String)
        Dim sqlDetail = ""
        Dim statusFaktur = 1

        For i As Integer = 0 To gridBarang.RowCount - 1
            Dim statusDetail = 0
            Dim harga = gridBarang.Rows.Item(i).Cells("clmHarga").Value.ToString.Replace(".", "").Replace(",", "")
            Dim hargaDisc = gridBarang.Rows.Item(i).Cells("clmHargaDisc").Value.ToString.Replace(".", "").Replace(",", "")
            Dim Qty = Val(gridBarang.Rows.Item(i).Cells("clmQty").Value)
            Dim disc = Val(gridBarang.Rows.Item(i).Cells("clmDisc").Value)
            Dim KdBarang = gridBarang.Rows.Item(i).Cells("clmPartNo").Value
            Dim StatusBarang = gridBarang.Rows.Item(i).Cells("clmStatus").Value

            sqlDetail = " insert into TrSalesPaymentDetail(KdSalesPayment, KdBarang, Harga, " & _
                        " Qty, Disc, StatusBarang, HargaDisc) values( " & _
                        " '" & Trim(txtID.Text) & "','" & KdBarang & "', " & _
                        " '" & harga & "', '" & Qty & "', " & _
                        " '" & disc & "', '" & StatusBarang & "', '" & hargaDisc & "') "

            execute_update_manual(sqlDetail)
        Next

        If flag = 1 Then
            If Val(txtPaid.Text.ToString.Replace(".", "").Replace(",", "")) < Val(lblTotalBayar.Text.ToString.Replace(".", "").Replace(",", "")) Then
                statusFaktur = 2
            End If
            Dim sqlFaktur = " Update TrFaktur set StatusPayment = '" & statusFaktur & "' " & _
                        " Where kdFaktur = '" & Trim(cmbFaktur.Text) & "' "
            execute_update_manual(sqlFaktur)
            Return True
        End If
        Return True
    End Function

    Function save(ByVal flag As String)
        If cmbFaktur.SelectedIndex = 0 Then
            msgInfo("Faktur harus dipilih")
            cmbFaktur.Focus()
        ElseIf cmbBayar.SelectedIndex = 0 Then
            msgInfo("Cara Bayar harus dipilih")
            cmbBayar.Focus()
        ElseIf txtPaid.Text = "" Then
            msgInfo("Bayar harus diisi")
            txtPaid.Focus()
        ElseIf Val(lblTotalBayar.Text.Replace(".", "").Replace(",", "")) <= 0 And Val(txtPaid.Text.Replace(".", "").Replace(",", "")) <> 0 Then
            msgInfo("Total paid harus 0. Dikarenakan pembayaran Faktur sudah tidak ada.")
        Else
            Dim totalPaid = txtPaid.Text.ToString.Replace(".", "").Replace(",", "")

            dbconmanual.Open()
            Dim trans As MySql.Data.MySqlClient.MySqlTransaction

            trans = dbconmanual.BeginTransaction(IsolationLevel.ReadCommitted)

            Try
                If PK = "" Then
                    sql = " insert into  trSalesPayment(KdSalesPayment, " & _
                          " TanggalSalesPayment, KdSales, KdToko, TotalSalesPayment, " & _
                          " StatusSalesPayment, Note, UserID, PaidBy, kdFaktur) " & _
                          " values('" + Trim(txtID.Text) + "', " & _
                          " '" & dtpPayment.Value.ToString("yyyy/MM/dd HH:mm:ss") & "', " & _
                          " '" & Trim(kdsales) & "','" & Trim(kdtoko) & "'," & _
                          " '" & Trim(totalPaid) & "','" & flag & "', " & _
                          " '" & txtNote.Text & "','" & kdKaryawan & "', " & _
                          " '" & cmbBayar.Text & "','" & cmbFaktur.Text & "')"

                    execute_update_manual(sql)
                Else
                    sql = " update trSalesPayment set " & _
                          " TanggalSalesPayment = '" & dtpPayment.Value.ToString("yyyy/MM/dd HH:mm:ss") & "'," & _
                          " KdSales = '" & Trim(kdsales) & "'," & _
                          " KdToko = '" & Trim(kdtoko) & "'," & _
                          " TotalSalesPayment = '" & Trim(totalPaid) & "', " & _
                          " Note = '" & Trim(txtNote.Text) & "', " & _
                          " StatusSalesPayment = '" & flag & "', " & _
                          " UserID = '" & kdKaryawan & "', " & _
                          " PaidBy = '" & cmbBayar.Text & "', " & _
                          " kdFaktur = '" & cmbFaktur.Text & "' " & _
                          " where  KdSalesPayment = '" & txtID.Text & "' "
                    execute_update_manual(sql)
                End If

                execute_update_manual("delete from TrSalesPaymentdetail where  kdSalesPayment = '" & txtID.Text & "'")
                SavePaymentDetail(flag)

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
            lblTotalBayar.Text = FormatNumber(Grandtotal - Val(lblTelahBayar.Text.ToString.Replace(".", "").Replace(",", "")), 0)
            lblGrandtotal.Text = FormatNumber(Grandtotal, 0)
        Catch ex As Exception
            MsgBox(ex.ToString, MsgBoxStyle.Critical, "Warning!!!")
        End Try
    End Sub

    Private Sub BrowseFaktur_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BrowseFaktur.Click
        Try
            data_carier(0) = PK
            data_carier(1) = "SalesPayment"
            sub_form = New FormBrowseFaktur
            sub_form.showDialog(FormMain)
            If data_carier(0) <> "" Then
                cmbFaktur.Text = data_carier(0)
                txtSales.Text = data_carier(1)
                txtToko.Text = data_carier(2)
                lblDaerah.Text = data_carier(3)
                kdsales = data_carier(4)
                kdtoko = data_carier(5)
                clear_variable_array()

                'generateCode()
                'txtID.Text &= "/" & kdsales

                'Dim addQuery = ""
                'If PK <> "" Then
                '    addQuery = " And TrRetur.kdFaktur <> '" & cmbFaktur.Text & "' "
                'End If

                'Dim reader = execute_reader(" Select MB.KdBarang,NamaBarang,Subkategori, " & _
                '                            " HargaDisc, Harga, sum(Qty - ifnull(( Select sum(Qty) " & _
                '                            " from TrReturDetail " & _
                '                            " Join TrRetur On TrReturDetail.KdRetur = TrRetur.KdRetur " & _
                '                            " where KdBarang = faktur.KdBarang " & _
                '                            " And kdFaktur = Trfaktur.kdFaktur " & _
                '                            " Group By KdFaktur,KdBarang ),0)) Qty, " & _
                '                            " Disc, ifnull(( Select 'Retur' " & _
                '                            " from TrReturDetail " & _
                '                            " Join TrRetur On TrReturDetail.KdRetur = TrRetur.KdRetur " & _
                '                            " where KdBarang = faktur.KdBarang " & _
                '                            " And kdFaktur = Trfaktur.kdFaktur " & _
                '                            " Group By KdFaktur,KdBarang ),'Normal') StatusBarang,Merk, " & _
                '                            " IfNull(( Select sum(TotalSalesPayment) from trsalespayment " & _
                '                            " Where kdFaktur = Trfaktur.kdFaktur " & _
                '                            addQuery & " ) ,0) " & _
                '                            " from TrfakturDetail faktur " & _
                '                            " Join Trfaktur On Trfaktur.kdFaktur = faktur.kdFaktur " & _
                '                            " Join MsBarang MB On faktur.KdBarang = MB.KdBarang " & _
                '                            " Join MsMerk On MsMerk.kdMerk = MB.kdMerk " & _
                '                            " Join MsSubkategori On MsSubkategori.KdSub = MB.KdSub " & _
                '                            " where Trfaktur.kdFaktur = '" & cmbFaktur.Text & "' " & _
                '                            " Group by MB.KdBarang,Trfaktur.kdFaktur " & _
                '                            " order by NamaBarang asc ")

                'gridBarang.Rows.Clear()
                'Do While reader.Read
                '    Dim Subtotal = Val(reader("HargaDisc")) * Val(reader("Qty"))

                '    gridBarang.Rows.Add()
                '    gridBarang.Rows.Item(gridBarang.RowCount - 1).Cells("clmPartNo").Value = reader("KdBarang")
                '    gridBarang.Rows.Item(gridBarang.RowCount - 1).Cells("clmNamaBarang").Value = reader("NamaBarang")
                '    gridBarang.Rows.Item(gridBarang.RowCount - 1).Cells("clmMerk").Value = reader("Merk")
                '    gridBarang.Rows.Item(gridBarang.RowCount - 1).Cells("clmSubKategori").Value = reader("Subkategori")
                '    gridBarang.Rows.Item(gridBarang.RowCount - 1).Cells("clmHarga").Value = FormatNumber(reader("Harga"), 0)
                '    gridBarang.Rows.Item(gridBarang.RowCount - 1).Cells("clmHargaDisc").Value = FormatNumber(reader("HargaDisc"), 0)
                '    gridBarang.Rows.Item(gridBarang.RowCount - 1).Cells("clmDisc").Value = reader("Disc")
                '    gridBarang.Rows.Item(gridBarang.RowCount - 1).Cells("clmQty").Value = reader("Qty")
                '    gridBarang.Rows.Item(gridBarang.RowCount - 1).Cells("clmSubtotal").Value = Subtotal
                '    gridBarang.Rows.Item(gridBarang.RowCount - 1).Cells("clmStatus").Value = reader("StatusBarang")
                'Loop
                'reader.Close()

                'HitungTotal()
            End If
        Catch ex As Exception
            MsgBox(ex.ToString, MsgBoxStyle.Critical, "Warning!!!")
        End Try
    End Sub

    Private Sub cmbfaktur_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmbFaktur.SelectedIndexChanged
        If cmbFaktur.SelectedIndex <> 0 Then
            Dim addQuery = ""
            If PK <> "" Then
                addQuery = " And KdSalesPayment <> '" & txtID.Text & "' "
            End If
            Dim reader = execute_reader(" Select MB.KdBarang,NamaBarang,Subkategori, " & _
                                        " HargaDisc, Harga, sum(Qty - ifnull(( Select sum(Qty) " & _
                                        " from TrReturDetail " & _
                                        " Join TrRetur On TrReturDetail.KdRetur = TrRetur.KdRetur " & _
                                        " where KdBarang = faktur.KdBarang " & _
                                        " AND kdFaktur = trfaktur.kdFaktur " & _
                                        " AND Harga = faktur.Harga " & _
                                        " Group By KdFaktur,KdBarang ),0)) Qty, " & _
                                        " Disc, ifnull(( Select 'Retur' " & _
                                        " from TrReturDetail " & _
                                        " Join TrRetur On TrReturDetail.KdRetur = TrRetur.KdRetur " & _
                                        " where KdBarang = faktur.KdBarang " & _
                                        " AND kdFaktur = trfaktur.kdFaktur " & _
                                        " AND Harga = faktur.Harga " & _
                                        " Group By KdFaktur,KdBarang ),'Normal') StatusBarang, " & _
                                        " MS.KdSales, NamaSales, " & _
                                        " MT.KdToko, NamaToko, MT.Daerah, NamaArea,Merk, " & _
                                        " IfNull(( Select sum(TotalSalesPayment) from trsalespayment " & _
                                        " Where kdFaktur = trfaktur.kdFaktur " & _
                                        addQuery & " ) ,0) TotalSalesPayment, faktur.KdFaktur, " & _
                                        " TotalDiscKomisiSales " & _
                                        " from TrFakturDetail faktur " & _
                                        " Join trfaktur On faktur.kdfaktur = trfaktur.kdfaktur " & _
                                        " Join MsBarang MB On faktur.KdBarang = MB.KdBarang " & _
                                        " Join MsMerk On MsMerk.kdMerk = MB.kdMerk " & _
                                        " Join MsSubkategori On MsSubkategori.KdSub = MB.KdSub " & _
                                        " Join MsSales MS On MS.KdSales = trfaktur.KdSales " & _
                                        " Join MsToko MT On MT.KdToko = trfaktur.KdToko " & _
                                        " Join MsArea MP On MP.KdArea = MT.KdArea " & _
                                        " where trfaktur.kdFaktur = '" & cmbFaktur.Text & "' " & _
                                        " Group by MB.KdBarang,trfaktur.kdFaktur " & _
                                        " order by NamaBarang asc ")
            '" And StatusSalesPayment <> 0 " & _

            Dim idxfaktur = 0
            gridBarang.Rows.Clear()
            Do While reader.Read
                If idxfaktur = 0 Then
                    txtSales.Text = reader("NamaSales")
                    txtToko.Text = reader("NamaToko")
                    lblDaerah.Text = reader("Daerah")
                    'lblProvinsi.Text = reader(12)
                    kdsales = reader("KdSales")
                    kdtoko = reader("KdToko")
                    lblTelahBayar.Text = FormatNumber(reader("TotalSalesPayment"), 0)
                End If

                Dim Subtotal = Val(reader("HargaDisc")) * Val(reader("Qty"))
                gridBarang.Rows.Add()
                gridBarang.Rows.Item(gridBarang.RowCount - 1).Cells("clmPartNo").Value = reader("KdBarang")
                gridBarang.Rows.Item(gridBarang.RowCount - 1).Cells("clmNamaBarang").Value = reader("NamaBarang")
                gridBarang.Rows.Item(gridBarang.RowCount - 1).Cells("clmMerk").Value = reader("Merk")
                gridBarang.Rows.Item(gridBarang.RowCount - 1).Cells("clmSubKategori").Value = reader("Subkategori")
                gridBarang.Rows.Item(gridBarang.RowCount - 1).Cells("clmHarga").Value = FormatNumber(reader("Harga"), 0)
                gridBarang.Rows.Item(gridBarang.RowCount - 1).Cells("clmHargaDisc").Value = FormatNumber(reader("HargaDisc"), 0)
                gridBarang.Rows.Item(gridBarang.RowCount - 1).Cells("clmDisc").Value = reader("Disc")
                gridBarang.Rows.Item(gridBarang.RowCount - 1).Cells("clmQty").Value = reader("Qty")
                gridBarang.Rows.Item(gridBarang.RowCount - 1).Cells("clmSubtotal").Value = Subtotal
                gridBarang.Rows.Item(gridBarang.RowCount - 1).Cells("clmStatus").Value = reader("StatusBarang")
                idxfaktur += 1
            Loop
            reader.Close()

            generateCode()
            txtID.Text &= "/" & kdsales

            HitungTotal()
        Else
            txtSales.Text = ""
            txtToko.Text = ""
            lblDaerah.Text = ""
            lblProvinsi.Text = ""
            kdsales = ""
            kdtoko = ""
            gridBarang.Rows.Clear()
            generateCode()
        End If
    End Sub

    Private Sub btnConfirms_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnConfirms.Click
        save(1)
    End Sub

    Private Sub GroupBox2_Enter(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles GroupBox2.Enter

    End Sub

    Private Sub txtPaid_Leave(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtPaid.Leave
        If txtPaid.Text <> "" Then
            txtPaid.Text = FormatNumber(txtPaid.Text, 0)
        End If
    End Sub

    Private Sub txtPaid_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles txtPaid.TextChanged

    End Sub
End Class
