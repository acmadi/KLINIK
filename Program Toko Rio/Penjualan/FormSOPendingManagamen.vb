Imports System.Data.SqlClient
Public Class FormSOPendingManagamen
    Dim status As String
    Dim tab As String
    Dim PK As String = ""
    Dim NamaBarang As String = ""
    Dim Merk As String = ""
    Dim SubCat As String = ""
    Dim tempDisc = 0
    Dim tempHargaDisc = 0
    Dim tempHargaJual = 0
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
        Return "Ready"
    End Function

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
                                          " MT.KdToko, NamaToko, MT.Daerah, NamaArea,StatusSO " & _
                                          " from TrSalesOrder SO " & _
                                          " Join MsSales MS On MS.KdSales = SO.KdSales " & _
                                          " Join MsToko MT On MT.KdToko = SO.KdToko " & _
                                          " Join MsArea MP On MP.KdArea = MT.KdArea" & _
                                          " Where KdSO = '" & PK & "' ")
            If readerSO.Read Then
                txtID.Text = readerSO.Item("KdSO")
                dtpSO.Text = readerSO.Item("Tanggal")
                lblDaerah.Text = readerSO.Item("Daerah")
                'lblProvinsi.Text = readerSO.Item(7)
                Sales = readerSO.Item("KdSales") & " ~ " & readerSO.Item("NamaSales")
                Toko = readerSO.Item("KdToko") & " ~ " & readerSO.Item("NamaToko")
                If readerSO.Item("StatusSO") <> 0 And readerSO.Item("StatusSO") <> 1 Then
                    btnSave.Enabled = False
                End If
            End If
            readerSO.Close()
            cmbSales.Text = Sales
            cmbToko.Text = Toko

            Dim reader = execute_reader(" Select MB.KdBarang,NamaBarang,Subkategori, " & _
                                        " HargaDisc, Harga,sum(Qty) Qty,Disc, " & _
                                        " ifnull(( Select sum(Qty) - IfNull(( Select sum(Qty) from TrSalesOrderDetail " & _
                                        " Join TrSalesOrder On TrSalesOrderDetail.kdso = TrSalesOrder.kdso " & _
                                        " Where KdBarang = MsBarangList.KdBarang " & _
                                        " And StatusSo = 3 " & _
                                        " And TrSalesOrder.KdSO <> '" & txtID.Text & "' " & _
                                        " Group By KdBarang ),0) As Stock From MsBarangList " & _
                                        " Where KdBarang = SO.KdBarang " & _
                                        " And StatusBarangList = 0 " & _
                                        " Group By KdBarang ),0) Stock, SO.StatusBarang,merk " & _
                                        " from TrSalesOrderDetailPending SO " & _
                                        " Join MsBarang MB On SO.KdBarang = MB.KdBarang " & _
                                        " Join MsMerk On MsMerk.kdmerk = MB.kdMerk " & _
                                        " Join MsSubkategori On MsSubkategori.KdSub = MB.KdSub " & _
                                        " where KdSO = '" & PK & "' " & _
                                        " And StatusBarang = 1 " & _
                                        " GROUP BY MB.KdBarang " & _
                                        " order by NamaBarang asc ")

            Do While reader.Read
                Dim Subtotal = Val(reader("HargaDisc")) * Val(reader("Qty"))
                Dim StatusBarang = "Ready"

                If Val(reader("Qty")) > Val(reader("StatusBarang")) Then
                    StatusBarang = "Pending"
                End If

                gridBarang.Rows.Add()
                gridBarang.Rows.Item(gridBarang.RowCount - 1).Cells("clmPartNo").Value = reader("KdBarang")
                gridBarang.Rows.Item(gridBarang.RowCount - 1).Cells("clmNamaBarang").Value = reader("NamaBarang")
                gridBarang.Rows.Item(gridBarang.RowCount - 1).Cells("clmMerk").Value = reader("merk")
                gridBarang.Rows.Item(gridBarang.RowCount - 1).Cells("clmSubKategori").Value = reader("Subkategori")
                gridBarang.Rows.Item(gridBarang.RowCount - 1).Cells("clmHarga").Value = FormatNumber(reader("Harga"), 0)
                gridBarang.Rows.Item(gridBarang.RowCount - 1).Cells("clmHargaDisc").Value = FormatNumber(reader("HargaDisc"), 0)
                gridBarang.Rows.Item(gridBarang.RowCount - 1).Cells("clmDisc").Value = Math.Round(reader("Disc"), 2)
                gridBarang.Rows.Item(gridBarang.RowCount - 1).Cells("clmQty").Value = reader("Qty")
                gridBarang.Rows.Item(gridBarang.RowCount - 1).Cells("clmSubtotal").Value = FormatNumber(Subtotal, 0)
                gridBarang.Rows.Item(gridBarang.RowCount - 1).Cells("clmStock").Value = reader("Stock")
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

    Private Sub FormMsSupplier_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        is_access = get_access("so")
        PK = data_carier(0)
        status = data_carier(1)
        clear_variable_array()
        setCmbSales()
        setCmbToko()
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
        gridBarang_CellEndEdit(Nothing, Nothing)
    End Sub

    Private Sub generateCode()
        Dim code As String = "SO"
        Dim angka As Integer
        Dim kode As String
        Dim temp As String

        Dim bulan As Integer = CInt(Today.Month.ToString)
        Dim currentTime As System.DateTime = System.DateTime.Now
        Dim FormatDate As String = Format(currentTime, ".dd.MM.yy")
        'code += FormatDate

        Dim reader = getSO()

        If reader.read Then
            kode = Trim(reader(0).ToString())
            temp = kode.Substring(0, 8)
            If temp = code Then
                angka = CInt(kode.Substring(8, 4))
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

        For i As Integer = 0 To gridBarang.RowCount - 1
            Dim statusDetail = 0
            Dim harga = gridBarang.Rows.Item(i).Cells("clmHarga").Value.ToString.Replace(".", "").Replace(",", "")
            Dim hargaDisc = gridBarang.Rows.Item(i).Cells("clmHarga").Value.ToString.Replace(".", "").Replace(",", "")
            Dim OldQty = Val(gridBarang.Rows.Item(i).Cells("clmQtyOri").Value)
            Dim Stok = Val(gridBarang.Rows.Item(i).Cells("clmStock").Value)
            Dim Qty = Val(gridBarang.Rows.Item(i).Cells("clmQty").Value)
            Dim OP = "Min"
            Dim Attribute = "QtySO_Min"
            Dim KdBarang = gridBarang.Rows.Item(i).Cells("clmPartNo").Value

            If gridBarang.Rows.Item(i).Cells("clmStatus").Value = "Ready" Then
                statusDetail = 0

                sqlDetail = "insert into TrSalesOrderDetail( KdSO, KdBarang, Harga, " & _
                            " Qty, Disc, StatusBarang, HargaDisc ) values( " & _
                            " '" & Trim(txtID.Text) & "', '" & KdBarang & "', " & _
                            " '" & harga & "', '" & gridBarang.Rows.Item(i).Cells("clmQty").Value & "', " & _
                            " '" & gridBarang.Rows.Item(i).Cells("clmDisc").Value & "', '" & statusDetail & "', " & _
                            " '" & hargaDisc & "' )"
                execute_update_manual(sqlDetail)
            ElseIf gridBarang.Rows.Item(i).Cells("clmStatus").Value = "Pending" Then
                statusDetail = 1
                statusSO = 1 + Val(flag)

                sqlDetail = "insert into TrSalesOrderDetailPending( KdSO, KdBarang, Harga, " & _
                            " Qty, Disc, StatusBarang, HargaDisc ) values( " & _
                            " '" & Trim(txtID.Text) & "', '" & KdBarang & "', " & _
                            " '" & harga & "', '" & gridBarang.Rows.Item(i).Cells("clmQty").Value & "', " & _
                            " '" & gridBarang.Rows.Item(i).Cells("clmDisc").Value & "', '" & statusDetail & "', " & _
                            " '" & hargaDisc & "' )"
                execute_update_manual(sqlDetail)
            End If
            'End If
        Next

        If flag = 3 And statusSO = 4 Then
            msgInfo("SO tidak dapat di confirm karena salah satu produk masih pending.")
        Else
            Dim sqlHeader = " Update TrSalesOrder set StatusSO = '" & statusSO & "' " & _
                    " Where KdSO = '" & Trim(txtID.Text) & "' "
            execute_update_manual(sqlHeader)
        End If

        Return True
    End Function

    Function save(ByVal flag As String)
        If cmbSales.SelectedIndex = 0 Then
            msgInfo("Sales harus dipilih")
            cmbSales.Focus()
        ElseIf cmbToko.SelectedIndex = 0 Then
            msgInfo("Toko harus dipilih")
            cmbToko.Focus()
        Else
            Dim salesID = cmbSales.Text.ToString.Split("~")
            Dim tokoID = cmbToko.Text.ToString.Split("~")
            Dim Grandtotal = ""

            If (lblGrandtotal.Text <> "") Then
                Grandtotal = lblGrandtotal.Text.ToString.Replace(".", "").Replace(",", "")
            End If

            Dim ReaderGrandDetail = execute_reader(" Select IfNull(sum(HargaDisc * Qty),0) " & _
                                                   " from TrSalesOrderDetail " & _
                                                   " Where KdSO = '" & txtID.Text & "' " & _
                                                   " And StatusBarang <> 1 ")

            If ReaderGrandDetail.read Then
                Grandtotal += Math.Round(ReaderGrandDetail(0) * 1, 2)
            End If
            ReaderGrandDetail.close()

            dbconmanual.Open()
            Dim trans As MySql.Data.MySqlClient.MySqlTransaction

            trans = dbconmanual.BeginTransaction(IsolationLevel.ReadCommitted)

            Try
                If status = "add" Then
                    sql = " insert into  TrSalesOrder  values( '" + Trim(txtID.Text) + "', " & _
                          " '" + dtpSO.Value.ToString("yyyy/MM/dd HH:mm:ss") + "', " & _
                          " '" & Trim(salesID(0)) & "', '" & Trim(tokoID(0)) & "'," & _
                          " '" & Trim(Grandtotal) & "', '" & flag & "', '" & kdKaryawan & "' )"

                    execute_update_manual(sql)
                ElseIf status = "update" Then
                    Try
                        sql = " update   TrSalesOrder  set  TanggalSO='" & dtpSO.Value.ToString("yyyy/MM/dd HH:mm:ss") & "'," & _
                              " KdSales = '" & Trim(salesID(0)) & "'," & _
                              " KdToko = '" & Trim(tokoID(0)) & "'," & _
                              " GrandTotal = '" & Trim(Grandtotal) & "', " & _
                              " UserID = '" & Trim(kdKaryawan) & "' " & _
                              " where  KdSO = '" + txtID.Text + "' "
                        execute_update_manual(sql)
                    Catch ex As Exception
                        msgWarning(" Data tidak valid")
                        cmbSales.SelectedIndex = 0
                    End Try
                End If

                execute_update_manual(" delete from TrSalesOrderDetailPending " & _
                                      " where  KdSO = '" & txtID.Text & "' " & _
                                      " And statusBarang = 1 ")
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

    Private Sub cmbSales_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmbSales.SelectedIndexChanged
        If cmbSales.SelectedIndex <> 0 Then
            cmbToko.Focus()
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
        Else
            lblProvinsi.Text = ""
            lblDaerah.Text = ""
        End If
    End Sub

    Private Sub browseSales_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
        Try
            sub_form = New FormBrowseSales
            sub_form.showDialog(FormMain)
            If data_carier(0) <> "" Then
                cmbSales.Text = data_carier(0) & " ~ " & data_carier(1)
                clear_variable_array()
            End If
        Catch ex As Exception
            MsgBox(ex.ToString, MsgBoxStyle.Critical, "Warning!!!")
        End Try
    End Sub

    Private Sub browseToko_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
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

    Private Sub gridBarang_CellBeginEdit(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewCellCancelEventArgs) Handles gridBarang.CellBeginEdit
        tempDisc = Val(gridBarang.CurrentRow.Cells("clmDisc").Value)
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
            Dim disc = Val(gridBarang.CurrentRow.Cells("clmDisc").Value)
            Dim stok = Val(gridBarang.CurrentRow.Cells("clmStock").Value)

            If IsNumeric(qty) = False Or qty < 1 Then
                MsgBox("Jumlah harus lebih besar dari 0 dan berupa angka.", MsgBoxStyle.Information, "Validation Error")
                qty = 1
                gridBarang.CurrentRow.Cells("clmQty").Value = 1
                gridBarang.CurrentRow.Cells("clmQty").Selected = True
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
                Dim calcDisc = 100 - ((hargaDisc / harga) * 100)
                gridBarang.CurrentRow.Cells("clmDisc").Value = Math.Round(calcDisc, 2)
            ElseIf disc <> tempDisc Then
                Dim discProses = 100 - Val(disc)
                Dim calcHargaJual = harga * (discProses / 100)
                gridBarang.CurrentRow.Cells("clmHargaDisc").Value = FormatNumber(calcHargaJual, 0)
            End If

            gridBarang.CurrentRow.Cells("clmSubtotal").Value = FormatNumber(Val(hargaDisc) * Val(qty), 0)
            gridBarang.CurrentRow.Cells("clmStatus").Value = check_stock(BarangID, qty, "StatusSO")

            HitungTotal()
        Catch ex As Exception
            MsgBox(ex.ToString, MsgBoxStyle.Critical, "Warning!!!")
        End Try
    End Sub

    Private Sub btnConfirms_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnConfirms.Click
        save(3)
    End Sub
End Class
