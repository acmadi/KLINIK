Imports System.Data.SqlClient
Public Class FormReturManagamen
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
    Dim isPaid As String = 0
    Dim statusReturFaktur = 0
    Dim is_access As Integer
    Dim tempDisc = 0
    Dim tempHargaDisc = 0
    Dim tempHarga = 0

    Private Sub msgWarning(ByVal str As String)
        MsgBox(str, MsgBoxStyle.Critical, "Warning")
    End Sub

    Private Sub msgInfo(ByVal str As String)
        MsgBox(str, MsgBoxStyle.Information, "Information")
    End Sub

    Private Sub emptyField()
        dtpRetur.Text = Now()
        cmbFaktur.SelectedIndex = 0
        txtSales.Text = ""
        txtToko.Text = ""
        lblDaerah.Text = ""
        lblProvinsi.Text = ""
        cmbStatusRetur.SelectedIndex = 0
    End Sub

    Function getRetur(Optional ByVal KdRetur As String = "")
        Dim sql As String = "Select KdRetur from trRetur order by no_increment desc "

        If KdRetur <> "" Then
            sql &= "kdRetur = '" & KdRetur & "'"
        End If
        Dim reader = execute_reader(sql)
        Return reader
    End Function

    Private Sub setData()
        Try
            Dim Sales = ""
            Dim Toko = ""
            Dim kdFaktur = ""
            Dim readerRetur = execute_reader(" select kdretur,DATE_FORMAT(Tanggalretur, '%m/%d/%Y') Tanggal, " & _
                            " retur.kdFaktur, MS.KdSales, NamaSales, " & _
                            " MT.KdToko, NamaToko, MT.Daerah, NamaArea,StatusRetur, " & _
                            " Status, Note from trretur retur " & _
                            " Join MsSales MS On MS.KdSales = retur.KdSales " & _
                            " Join MsToko MT On MT.KdToko = retur.KdToko " & _
                            " Join MsArea MP On MP.KdArea = MT.KdArea" & _
                            " Where kdretur = '" & PK & "' ")
            If readerRetur.Read Then
                txtID.Text = readerRetur.Item("kdretur")
                dtpRetur.Text = readerRetur.Item("Tanggal")
                lblDaerah.Text = readerRetur.Item("Daerah")
                'lblProvinsi.Text = readerRetur.Item(8)
                kdFaktur = readerRetur.Item("kdFaktur")
                Sales = readerRetur.Item("KdSales") & " - " & readerRetur.Item("NamaSales")
                Toko = readerRetur.Item("KdToko") & " - " & readerRetur.Item("NamaToko")
                If readerRetur.Item("StatusRetur") <> 0 Then
                    btnSave.Enabled = False
                    btnConfirms.Enabled = False
                End If
                statusReturFaktur = readerRetur.Item("StatusRetur")
                cmbStatusRetur.Text = readerRetur.Item("Status")
                txtNote.Text = readerRetur.Item("Note")
            End If
            readerRetur.Close()
            txtSales.Text = Sales
            txtToko.Text = Toko
            cmbFaktur.Text = kdFaktur

            Dim reader = execute_reader(" Select MB.KdBarang,NamaBarang,Subkategori, " & _
                                        " HargaDisc, Harga, Qty,Disc, " & _
                                        " IfNull(( select sum(Qty) from TrFakturDetail " & _
                                        " Join TrFaktur on TrFaktur.KdFaktur = TrFakturDetail.KdFaktur " & _
                                        " Where TrFaktur.kdFaktur = trretur.kdFaktur " & _
                                        " And KdBarang = retur.KdBarang " & _
                                        " And statusfaktur <> 0 " & _
                                        " Group By  TrFaktur.kdFaktur,KdBarang ),0) - ifNull(( " & _
                                        " Select sum(Qty) from TrReturDetail " & _
                                        " Join TrRetur on TrRetur.KdRetur = TrReturDetail.KdRetur " & _
                                        " Where TrRetur.KdRetur <> retur.KdRetur " & _
                                        " And KdBarang = retur.KdBarang " & _
                                        " And kdFaktur = trretur.kdFaktur " & _
                                        " Group by TrRetur.kdFaktur,KdBarang ),0) as QtyFaktur, " & _
                                        " StatusBarang,Merk,IfNull(HargaAwal,0) HargaAwal " & _
                                        " from TrReturDetail retur " & _
                                        " Join trretur On retur.KdRetur = trretur.KdRetur " & _
                                        " Join MsBarang MB On retur.KdBarang = MB.KdBarang " & _
                                        " Join MsMerk On MsMerk.kdMerk = MB.kdMerk " & _
                                        " Join MsSubkategori On MsSubkategori.KdSub = MB.KdSub " & _
                                        " Left Join baranghistory BH On BH.RefNumber = trretur.kdfaktur" & _
                                        " And BH.KdBarang = retur.KdBarang" & _
                                        " where trretur.kdretur = '" & PK & "' " & _
                                        " order by NamaBarang asc ")

            gridBarang.Rows.Clear()
            Do While reader.Read
                Dim Subtotal = Val(reader("HargaDisc")) * Val(reader("Qty"))

                gridBarang.Rows.Add()
                gridBarang.Rows.Item(gridBarang.RowCount - 1).Cells("clmPartNo").Value = reader("KdBarang")
                gridBarang.Rows.Item(gridBarang.RowCount - 1).Cells("clmNamaBarang").Value = reader("NamaBarang")
                gridBarang.Rows.Item(gridBarang.RowCount - 1).Cells("clmMerk").Value = reader("Merk")
                gridBarang.Rows.Item(gridBarang.RowCount - 1).Cells("clmSubKategori").Value = reader("Subkategori")
                gridBarang.Rows.Item(gridBarang.RowCount - 1).Cells("clmQtyFaktur").Value = reader("QtyFaktur")
                gridBarang.Rows.Item(gridBarang.RowCount - 1).Cells("clmHarga").Value = FormatNumber(reader("Harga"), 0)
                gridBarang.Rows.Item(gridBarang.RowCount - 1).Cells("clmHargaDisc").Value = FormatNumber(reader("HargaDisc"), 0)
                gridBarang.Rows.Item(gridBarang.RowCount - 1).Cells("clmDisc").Value = reader("Disc")
                gridBarang.Rows.Item(gridBarang.RowCount - 1).Cells("clmQty").Value = reader("Qty")
                gridBarang.Rows.Item(gridBarang.RowCount - 1).Cells("clmSubtotal").Value = Subtotal
                gridBarang.Rows.Item(gridBarang.RowCount - 1).Cells("clmStatus").Value = reader("StatusBarang")
                gridBarang.Rows.Item(gridBarang.RowCount - 1).Cells("clmHargaAwal").Value = reader("HargaAwal")
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

    Public Sub setCmbFaktur()
        Dim varT As String = ""
        Dim addQuery = ""
        cmbFaktur.Items.Clear()
        cmbFaktur.Items.Add("- Pilih Faktur -")
        If PK <> "" Then
            addQuery = " And exists( Select 1 from trretur where kdretur = '" & PK & "' And kdFaktur = trfaktur.kdFaktur )"
            cmbFaktur.Enabled = False
            BrowseFaktur.Enabled = False
        Else
            addQuery = " And statusfaktur <> 0 "
        End If

        Dim reader = execute_reader(" Select kdFaktur from trfaktur " & _
                                    " Join msuser On msuser.UserID = trfaktur.UserID " & _
                                    " where 1 " & _
                                    QueryLevel(lvlKaryawan, "msuser.", "level") & _
                                    addQuery & " Group By kdFaktur Order By TanggalFaktur Desc,StatusFaktur asc ")
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
        is_access = get_access("returJual")
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
        Dim code As String = "RF"
        Dim angka As Integer
        Dim kode As String
        Dim temp As String
        Dim tempCompare As String = ""

        Dim bulan As Integer = CInt(Today.Month.ToString)
        Dim currentTime As System.DateTime = System.DateTime.Now
        Dim FormatDate As String = Format(currentTime, ".dd.MM.yy")
        tempCompare &= FormatDate

        Dim reader = getRetur()

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

    Function SaveReturDetail(ByVal flag As String)
        Dim sqlHistory = ""
        Dim sqlDetail = ""
        Dim statusFaktur = 3
        Dim StatusBarangList = 0

        For i As Integer = 0 To gridBarang.RowCount - 1
            Dim statusDetail = 0
            Dim harga = gridBarang.Rows.Item(i).Cells("clmHarga").Value.ToString.Replace(".", "").Replace(",", "")
            Dim hargaDisc = gridBarang.Rows.Item(i).Cells("clmHargaDisc").Value.ToString.Replace(".", "").Replace(",", "")
            Dim Qty = Val(gridBarang.Rows.Item(i).Cells("clmQty").Value)
            Dim disc = Val(gridBarang.Rows.Item(i).Cells("clmDisc").Value)
            Dim OP = "Plus"
            Dim Attribute = "QtyRetur_Plus"
            Dim KdBarang = gridBarang.Rows.Item(i).Cells("clmPartNo").Value
            Dim Stok = gridBarang.Rows.Item(i).Cells("clmQtyFaktur").Value
            Dim statusBarang = gridBarang.Rows.Item(i).Cells("clmStatus").Value
            Dim HargaAwal = gridBarang.Rows.Item(i).Cells("clmHargaAwal").Value

            If statusBarang = "Rusak" Then
                StatusBarangList = 1
            End If

            If Qty <> Stok Then
                statusFaktur = 2
            End If

            If Qty <> 0 Then
                If flag = 1 Then
                    StockBarang(Qty, OP, HargaAwal, KdBarang, Attribute, Trim(txtID.Text), "Form Retur Pembelian Barang", StatusBarangList)
                End If
                sqlDetail = "insert into TrReturDetail(KdRetur, KdBarang, Harga, " & _
                            " Qty, Disc, StatusBarang, HargaDisc) values( " & _
                            " '" & Trim(txtID.Text) & "', '" & KdBarang & "', " & _
                            " '" & harga & "', '" & Qty & "', " & _
                            " '" & disc & "', '" & statusBarang & "', " & _
                            " '" & hargaDisc & "' ) "
                execute_update_manual(sqlDetail)
            End If
        Next

        If flag = 1 Then
            Dim sqlFaktur = " Update TrFaktur set StatusFaktur = '" & statusFaktur & "' " & _
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
        ElseIf cmbStatusRetur.SelectedIndex = 0 Then
            msgInfo("Status retur harus dipilih")
            cmbStatusRetur.Focus()
        Else
            Dim Grandtotal = ""
            Dim checkQty = 0

            For i As Integer = 0 To (gridBarang.Rows.Count - 1)
                If gridBarang.Rows.Item(i).Cells("clmQty").Value <> 0 Then
                    checkQty += 1
                    If gridBarang.Rows.Item(i).Cells("clmStatus").Value = "- Klik disini -" Then
                        msgInfo("Klik status barang.")
                        gridBarang.Rows.Item(i).Cells("clmStatus").Selected = True
                        Return True
                        Exit Function
                    End If
                Else
                    checkQty += 0
                End If
            Next

            If checkQty = 0 Then
                msgInfo("Salah satu jumlah harus diisi lebih dari 0.")
                Return True
                Exit Function
            End If

            If (lblGrandtotal.Text <> "") Then
                Grandtotal = lblGrandtotal.Text.ToString.Replace(".", "").Replace(",", "")
            End If

            dbconmanual.Open()
            Dim trans As MySql.Data.MySqlClient.MySqlTransaction

            trans = dbconmanual.BeginTransaction(IsolationLevel.ReadCommitted)

            If isPaid > 0 Then
                isPaid = 1
            End If

            Try
                If PK = "" Then
                    sql = " insert into  trretur ( KdRetur, kdFaktur, TanggalRetur, " & _
                          " KdSales, KdToko, GrandTotal, StatusRetur, Note, Status, " & _
                          " UserID, AfterPaid) " & _
                          " values('" + Trim(txtID.Text) + "', " & _
                          " '" & cmbFaktur.Text & "', " & _
                          " '" & dtpRetur.Value.ToString("yyyy/MM/dd HH:mm:ss") & "', " & _
                          " '" & Trim(kdsales) & "','" & Trim(kdtoko) & "'," & _
                          " '" & Trim(Grandtotal) & "','" & flag & "', " & _
                          " '" & txtNote.Text & "','" & cmbStatusRetur.Text & "', " & _
                          " '" & kdKaryawan & "','" & isPaid & "' )"

                    execute_update_manual(sql)
                Else
                    sql = "update   trretur  set  TanggalRetur='" & dtpRetur.Value.ToString("yyyy/MM/dd HH:mm:ss") & "'," & _
                    " kdFaktur = '" & cmbFaktur.Text & "'," & _
                    " KdSales = '" & Trim(kdsales) & "'," & _
                    " KdToko = '" & Trim(kdtoko) & "'," & _
                    " GrandTotal = '" & Trim(Grandtotal) & "', " & _
                    " Status = '" & Trim(cmbStatusRetur.Text) & "', " & _
                    " Note = '" & Trim(txtNote.Text) & "', " & _
                    " UserID = '" & kdKaryawan & "', " & _
                    " AfterPaid = '" & isPaid & "', " & _
                    " StatusRetur = '" & flag & "' " & _
                    " where  KdRetur = '" + txtID.Text + "' "
                    execute_update_manual(sql)
                End If

                execute_update_manual("delete from Trreturdetail where  kdretur = '" & txtID.Text & "'")
                SaveReturDetail(flag)

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

    Private Sub gridBarang_CellBeginEdit(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewCellCancelEventArgs) Handles gridBarang.CellBeginEdit
        tempDisc = Val(gridBarang.CurrentRow.Cells("clmDisc").Value)
        tempDisc = Math.Round(tempDisc, 2)
        tempHargaDisc = Val(gridBarang.CurrentRow.Cells("clmHargaDisc").Value.Replace(".", "").Replace(",", ""))
        tempHarga = Val(gridBarang.CurrentRow.Cells("clmHarga").Value.Replace(".", "").Replace(",", ""))
    End Sub

    Private Sub gridBarang_CellClick(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewCellEventArgs) Handles gridBarang.CellClick
        If gridBarang.CurrentRow.Cells("clmStatus").Selected = True Then
            sub_form = New FormBrowseStatusRetur
            sub_form.showDialog(FormMain)
            If data_carier(0) <> "" Then
                gridBarang.CurrentRow.Cells("clmStatus").Value = data_carier(0)
                clear_variable_array()
            End If
        Else
            tempDisc = gridBarang.CurrentRow.Cells("clmDisc").Value * 1
            tempDisc = Math.Round(tempDisc, 2)
            tempHargaDisc = Val(gridBarang.CurrentRow.Cells("clmHargaDisc").Value.Replace(".", "").Replace(",", ""))
            tempHarga = Val(gridBarang.CurrentRow.Cells("clmHarga").Value.Replace(".", "").Replace(",", ""))
        End If
    End Sub

    Private Sub gridBarang_CellEndEdit(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewCellEventArgs) Handles gridBarang.CellEndEdit
        Try
            Dim BarangID = gridBarang.CurrentRow.Cells("clmPartNo").Value
            Dim harga = Val(gridBarang.CurrentRow.Cells("clmHarga").Value.Replace(".", "").Replace(",", ""))
            Dim hargaDisc = Val(gridBarang.CurrentRow.Cells("clmHargaDisc").Value.Replace(".", "").Replace(",", ""))
            Dim qty = Val(gridBarang.CurrentRow.Cells("clmQty").Value)
            Dim disc = Val(gridBarang.CurrentRow.Cells("clmDisc").Value)
            Dim stok = Val(gridBarang.CurrentRow.Cells("clmQtyFaktur").Value)

            If IsNumeric(qty) = False Then
                MsgBox("Jumlah harus berupa angka.", MsgBoxStyle.Information, "Validation Error")
                qty = stok
                gridBarang.CurrentRow.Cells("clmQty").Value = stok
                gridBarang.CurrentRow.Cells("clmQty").Selected = True
            ElseIf qty > stok Then
                MsgBox("Jumlah tidak mencukupi persedian. Persedian saat ini adalah " & stok, MsgBoxStyle.Information, "Validation Error")
                qty = stok
                gridBarang.CurrentRow.Cells("clmQty").Value = stok
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
                Dim TempHargaDisc = FormatNumber(hargaDisc, 0)
                gridBarang.CurrentRow.Cells("clmHarga").Value = TempHarga
                gridBarang.CurrentRow.Cells("clmHargaDisc").Value = TempHargaDisc
            End If

            If hargaDisc <> tempHargaDisc Or harga <> tempHarga Then
                Dim calcDisc = 100 - (hargaDisc / harga * 100)
                gridBarang.CurrentRow.Cells("clmDisc").Value = Math.Round(calcDisc, 2)
            ElseIf disc <> tempDisc Then
                Dim discProses = 100 - Math.Round(disc * 1, 2)
                Dim calcHarga = harga * (discProses / 100)
                gridBarang.CurrentRow.Cells("clmHargaDisc").Value = FormatNumber(calcHarga, 0)
            End If

            gridBarang.CurrentRow.Cells("clmSubtotal").Value = FormatNumber(Val(hargaDisc) * Val(qty), 0)

            HitungTotal()
        Catch ex As Exception
            MsgBox(ex.ToString, MsgBoxStyle.Critical, "Warning!!!")
        End Try
    End Sub

    Private Sub BrowseFaktur_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BrowseFaktur.Click
        Try
            data_carier(0) = PK
            data_carier(1) = "ReturFaktur"
            sub_form = New FormBrowseFaktur
            sub_form.showDialog(FormMain)
            If data_carier(0) <> "" Then
                cmbFaktur.Text = data_carier(0)
                clear_variable_array()
                'txtSales.Text = data_carier(1)
                'txtToko.Text = data_carier(2)
                'lblDaerah.Text = data_carier(3)
                'kdsales = data_carier(4)
                'kdtoko = data_carier(5)
                'isPaid = data_carier(6)
                'clear_variable_array()

                'generateCode()
                'txtID.Text &= "/" & kdsales

                'Dim reader = execute_reader(" Select MB.KdBarang,NamaBarang,Subkategori, " & _
                '                            " HargaDisc, Harga,sum(Qty) - ifnull(( Select sum(Qty) " & _
                '                            " from TrReturDetail " & _
                '                            " Join TrRetur On TrReturDetail.KdRetur = TrRetur.KdRetur " & _
                '                            " where KdBarang = faktur.KdBarang " & _
                '                            " And kdFaktur = Trfaktur.kdFaktur " & _
                '                            " Group By kdFaktur,KdBarang ),0) Qty, " & _
                '                            " Disc, faktur.StatusBarang,Merk,IfNull(HargaAwal,0) HargaAwal " & _
                '                            " from TrfakturDetail faktur " & _
                '                            " Join Trfaktur On Trfaktur.KdFaktur = faktur.KdFaktur " & _
                '                            " Join MsBarang MB On faktur.KdBarang = MB.KdBarang " & _
                '                            " Join MsMerk On MsMerk.kdMerk = MB.kdMerk " & _
                '                            " Join MsSubkategori On MsSubkategori.KdSub = MB.KdSub " & _
                '                            " Left Join baranghistory BH On BH.RefNumber = trfaktur.kdfaktur" & _
                '                            " And BH.KdBarang = faktur.KdBarang" & _
                '                            " where Trfaktur.kdFaktur = '" & cmbFaktur.Text & "' " & _
                '                            " Group by Trfaktur.kdFaktur,MB.KdBarang " & _
                '                            " order by NamaBarang asc ")

                'gridBarang.Rows.Clear()
                'Do While reader.Read
                '    gridBarang.Rows.Add()
                '    gridBarang.Rows.Item(gridBarang.RowCount - 1).Cells("clmPartNo").Value = reader("KdBarang")
                '    gridBarang.Rows.Item(gridBarang.RowCount - 1).Cells("clmNamaBarang").Value = reader("NamaBarang")
                '    gridBarang.Rows.Item(gridBarang.RowCount - 1).Cells("clmMerk").Value = reader("Merk")
                '    gridBarang.Rows.Item(gridBarang.RowCount - 1).Cells("clmSubKategori").Value = reader("Subkategori")
                '    gridBarang.Rows.Item(gridBarang.RowCount - 1).Cells("clmQtyFaktur").Value = reader("Qty")
                '    gridBarang.Rows.Item(gridBarang.RowCount - 1).Cells("clmHarga").Value = FormatNumber(reader("Harga"), 0)
                '    gridBarang.Rows.Item(gridBarang.RowCount - 1).Cells("clmHargaDisc").Value = FormatNumber(reader("HargaDisc"), 0)
                '    gridBarang.Rows.Item(gridBarang.RowCount - 1).Cells("clmDisc").Value = reader("Disc")
                '    gridBarang.Rows.Item(gridBarang.RowCount - 1).Cells("clmQty").Value = 0
                '    gridBarang.Rows.Item(gridBarang.RowCount - 1).Cells("clmSubtotal").Value = 0
                '    gridBarang.Rows.Item(gridBarang.RowCount - 1).Cells("clmStatus").Value = "- Klik disini -"
                '    gridBarang.Rows.Item(gridBarang.RowCount - 1).Cells("clmHargaAwal").Value = reader("HargaAwal")
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
            Dim reader = execute_reader(" Select MB.KdBarang,NamaBarang,Subkategori, " & _
                                        " HargaDisc,Harga,sum(Qty) - ifnull(( Select sum(Qty) " & _
                                        " from TrReturDetail " & _
                                        " Join TrRetur On TrReturDetail.KdRetur = TrRetur.KdRetur " & _
                                        " where KdBarang = faktur.KdBarang " & _
                                        " And kdFaktur = trfaktur.kdFaktur " & _
                                        " AND Harga = faktur.Harga " & _
                                        " Group By kdFaktur,KdBarang ),0) Qty, " & _
                                        " Disc, faktur.StatusBarang, " & _
                                        " MS.KdSales, NamaSales, " & _
                                        " MT.KdToko, NamaToko, MT.Daerah, " & _
                                        " NamaArea,Merk,StatusPayment,IfNull(HargaAwal,0) HargaAwal " & _
                                        " from TrFakturDetail faktur " & _
                                        " Join trfaktur On faktur.kdfaktur = trfaktur.kdfaktur " & _
                                        " Join MsBarang MB On faktur.KdBarang = MB.KdBarang " & _
                                        " Join MsMerk On MsMerk.kdMerk = MB.kdMerk " & _
                                        " Join MsSubkategori On MsSubkategori.KdSub = MB.KdSub " & _
                                        " Join MsSales MS On MS.KdSales = trfaktur.KdSales " & _
                                        " Join MsToko MT On MT.KdToko = trfaktur.KdToko " & _
                                        " Join MsArea MP On MP.KdArea = MT.KdArea" & _
                                        " Left Join baranghistory BH On BH.RefNumber = trfaktur.kdfaktur" & _
                                        " And BH.KdBarang = faktur.KdBarang" & _
                                        " where trfaktur.kdFaktur = '" & cmbFaktur.Text & "' " & _
                                        " Group by Trfaktur.kdFaktur, MB.KdBarang " & _
                                        " order by NamaBarang asc ")

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
                    isPaid = reader("StatusPayment")
                End If

                gridBarang.Rows.Add()
                gridBarang.Rows.Item(gridBarang.RowCount - 1).Cells("clmPartNo").Value = reader("KdBarang")
                gridBarang.Rows.Item(gridBarang.RowCount - 1).Cells("clmNamaBarang").Value = reader("NamaBarang")
                gridBarang.Rows.Item(gridBarang.RowCount - 1).Cells("clmMerk").Value = reader("Merk")
                gridBarang.Rows.Item(gridBarang.RowCount - 1).Cells("clmSubKategori").Value = reader("Subkategori")
                gridBarang.Rows.Item(gridBarang.RowCount - 1).Cells("clmQtyFaktur").Value = reader("Qty")
                gridBarang.Rows.Item(gridBarang.RowCount - 1).Cells("clmHarga").Value = FormatNumber(reader("Harga"), 0)
                gridBarang.Rows.Item(gridBarang.RowCount - 1).Cells("clmHargaDisc").Value = FormatNumber(reader("HargaDisc"), 0)
                gridBarang.Rows.Item(gridBarang.RowCount - 1).Cells("clmDisc").Value = reader("Disc")
                gridBarang.Rows.Item(gridBarang.RowCount - 1).Cells("clmQty").Value = 0
                gridBarang.Rows.Item(gridBarang.RowCount - 1).Cells("clmSubtotal").Value = 0
                gridBarang.Rows.Item(gridBarang.RowCount - 1).Cells("clmStatus").Value = "- Klik disini -"
                gridBarang.Rows.Item(gridBarang.RowCount - 1).Cells("clmHargaAwal").Value = reader("HargaAwal")
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

    Private Sub cmbStatusRetur_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmbStatusRetur.SelectedIndexChanged
        If cmbStatusRetur.SelectedIndex = 2 Then
            btnConfirms.Enabled = False
        ElseIf statusReturFaktur = 0 Then
            btnConfirms.Enabled = True
        End If
    End Sub
End Class
