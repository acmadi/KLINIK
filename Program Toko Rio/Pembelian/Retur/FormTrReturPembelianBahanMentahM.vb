Imports System.Data.SqlClient

Public Class FormTrReturPembelianBahanMentahM
    Dim status As String
    Dim posisi As Integer = 0
    Dim tab As String
    Dim jumData As Integer = 0
    Dim PK As String = ""
    Dim NamaBahanMentah As String = ""
    Dim Tipe As String = ""
    Dim SubCat As String = ""
    Dim kdSupplier As String = ""
    Dim statusReturPB = 0
    Dim isPaid As String = 0
    Dim is_access As Integer

    Private Sub msgWarning(ByVal str As String)
        MsgBox(str, MsgBoxStyle.Critical, "Warning")
    End Sub

    Private Sub msgInfo(ByVal str As String)
        MsgBox(str, MsgBoxStyle.Information, "Information")
    End Sub

    Private Sub emptyField()
        dtpRetur.Text = Now()
        cmbPO.SelectedIndex = 0
        txtSupplier.Text = ""
        cmbStatusRetur.SelectedIndex = 0
    End Sub

    Function getRetur(Optional ByVal KdRetur As String = "")
        Dim sql As String = "Select kdRetur from trheaderReturbeli order by no_increment desc "

        If KdRetur <> "" Then
            sql &= "kdRetur = '" & KdRetur & "'"
        End If
        Dim reader = execute_reader(sql)
        Return reader
    End Function

    Private Sub setData()
        Try
            Dim kdpo = ""
            Dim readerRetur = execute_reader(" select kdretur,DATE_FORMAT(Tanggalretur, '%m/%d/%Y') Tanggal, " & _
                            " No_PO, retur.KdSupplier,Nama `Supplier`, " & _
                            " StatusRetur, " & _
                            " Status, Note from trheaderreturbeli retur " & _
                           " Join MsSupplier MT On MT.KdSupplier = retur.KdSupplier " & _
                           " Where kdretur = '" & PK & "' ")
            If readerRetur.Read Then
                txtID.Text = readerRetur.Item(0)
                dtpRetur.Text = readerRetur.Item(1)
                kdpo = readerRetur.Item(2)
                txtSupplier.Text = readerRetur("Supplier")
                kdSupplier = readerRetur("KdSupplier")
                If readerRetur.Item(5) <> 0 Then
                    btnSave.Enabled = False
                    btnConfirms.Enabled = False
                End If
                statusReturPB = readerRetur.Item(5)
                cmbStatusRetur.Text = readerRetur("Status")
                txtNote.Text = readerRetur("Note")
            End If
            cmbPO.Text = kdpo
            Dim reader = execute_reader(" Select MB.KdBahanMentah,NamaBahanMentah,BahanMentahSubKategori, " & _
                                    " Harga, Qty,Disc, " & _
                                    " IfNull(( select sum(Qty) from TrDetailPB dp " & _
                                    " Join TrheaderPB pb on pb.No_pb = dp.No_pb " & _
                                    " Where pb.No_PO = '" & cmbPO.Text & "' " & _
                                    " And KdBarang = dr.KdBarang " & _
                                    " And statusterimabarang <> 0 " & _
                                    " And harga = dr.harga " & _
                                    " And TipePB = 1 " & _
                                    " Group By pb.No_PO,KdBarang,harga ),0) - ifNull(( " & _
                                    " Select sum(Qty) from TrdetailReturbeli dr " & _
                                    " Join TrheaderReturbeli hr on hr.KdRetur = dr.KdRetur " & _
                                    " Where hr.KdRetur <> dr.KdRetur " & _
                                    " And KdBarang = dr.KdBarang " & _
                                    " And No_PO = '" & cmbPO.Text & "' ),0) as QtyFaktur,StatusBarang,Namabahanmentahtipe " & _
                                    " from TrdetailReturbeli dr " & _
                                    " Join MsBahanMentah MB On dr.KdBarang = MB.KdBahanMentah " & _
                                    " Join MsBahanMentahTipe On MsBahanMentahTipe.kdBahanMentahTipe = MB.kdBahanMentahTipe " & _
                                    " Join MsBahanMentahSubKategori On MsBahanMentahSubKategori.KdBahanMentahSubKategori = MB.KdBahanMentahSubKategori " & _
                                    " where kdretur = '" & PK & "' " & _
                                    " order by NamaBahanMentah asc ")

            gridBahanMentah.Rows.Clear()
            Do While reader.Read
                Dim Subtotal = (Val(reader(3)) * Val(reader(4))) * ((100 - Val(reader(5))) / 100)

                gridBahanMentah.Rows.Add()
                gridBahanMentah.Rows.Item(gridBahanMentah.RowCount - 1).Cells(0).Value = reader(0)
                gridBahanMentah.Rows.Item(gridBahanMentah.RowCount - 1).Cells(1).Value = reader(1)
                gridBahanMentah.Rows.Item(gridBahanMentah.RowCount - 1).Cells(2).Value = reader(8)
                gridBahanMentah.Rows.Item(gridBahanMentah.RowCount - 1).Cells(3).Value = reader(2)
                gridBahanMentah.Rows.Item(gridBahanMentah.RowCount - 1).Cells(4).Value = FormatNumber(reader(3), 0)
                gridBahanMentah.Rows.Item(gridBahanMentah.RowCount - 1).Cells(5).Value = reader(6)
                gridBahanMentah.Rows.Item(gridBahanMentah.RowCount - 1).Cells(6).Value = reader(5)
                gridBahanMentah.Rows.Item(gridBahanMentah.RowCount - 1).Cells(7).Value = reader(4)
                gridBahanMentah.Rows.Item(gridBahanMentah.RowCount - 1).Cells(8).Value = Subtotal
                gridBahanMentah.Rows.Item(gridBahanMentah.RowCount - 1).Cells(9).Value = reader(7)
            Loop
            reader.Close()

            HitungTotal()
        Catch ex As Exception
            MsgBox(ex, MsgBoxStyle.Information)
        End Try
    End Sub

    Private Sub setGrid()
        With gridBahanMentah.ColumnHeadersDefaultCellStyle
            .Alignment = DataGridViewContentAlignment.MiddleCenter
            .Font = New Font(.Font.FontFamily, .Font.Size, _
              .Font.Style Or FontStyle.Bold, GraphicsUnit.Point)
            .ForeColor = Color.Gold

        End With
    End Sub

    Public Sub setCmbFaktur()
        Dim varT As String = ""
        Dim addQuery = ""
        cmbPO.Items.Clear()
        cmbPO.Items.Add("- Pilih PO -")
        If PK <> "" Then
            addQuery = " And exists( Select 1 from trheaderreturbeli hr where kdretur = '" & PK & "' And NO_PO = hr.No_PO ) "
            cmbPO.Enabled = False
            BrowsePO.Enabled = False
        Else
            addQuery = " And statusTerimaBarang <> 0 "
        End If

        Dim reader = execute_reader("Select Distinct No_PO from trheaderpb hp " & _
                                    " Join msuser On msuser.UserID = hp.UserID " & _
                                    " where 1 " & _
                                    " And TipePB = 1 " & _
                                    QueryLevel(lvlKaryawan, "msuser.", "level") & _
                                    addQuery & "  Order By Tanggal_terimaBarang Desc,StatusTerimaBarang asc ")
        Do While reader.Read
            cmbPO.Items.Add(reader(0))
        Loop
        reader.Close()
        If cmbPO.Items.Count > 0 Then
            cmbPO.SelectedIndex = 0
        End If
        reader.Close()
    End Sub

    Private Sub FormTrReturPembelianM_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        is_access = get_access("returBeli")
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
        cmbPO.Focus()
    End Sub

    Private Sub generateCode()
        Dim code As String = "RB"
        Dim angka As Integer
        Dim kode As String
        Dim temp As String
        'code += Today.Year.ToString.Substring(2, 2)
        Dim bulan As Integer = CInt(Today.Month.ToString)
        Dim currentTime As System.DateTime = System.DateTime.Now
        Dim FormatDate As String = Format(currentTime, "yyMMdd")

        'If bulan < 10 Then
        '    code += "0" + bulan.ToString
        'Else
        '    code += bulan.ToString
        'End If
        code += FormatDate

        Dim reader = getRetur()

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
        code = code & (angka)
        txtID.Text = Trim(code)
    End Sub

    Function SaveReturDetail(ByVal flag As String)
        Dim sqlHistory = ""
        Dim sqlDetail = ""
        Dim statusFaktur = 3
        Dim StatusBahanMentahList = 0

        For i As Integer = 0 To gridBahanMentah.RowCount - 1
            Dim statusDetail = 0
            Dim harga = gridBahanMentah.Rows.Item(i).Cells(4).Value.ToString.Replace(".", "").Replace(",", "")
            Dim Qty = Val(gridBahanMentah.Rows.Item(i).Cells(7).Value)
            Dim disc = Val(gridBahanMentah.Rows.Item(i).Cells(6).Value)
            Dim OP = "Min"
            Dim Attribute = "QtyRetur_Min"
            Dim KdBahanMentah = gridBahanMentah.Rows.Item(i).Cells(0).Value
            Dim Stok = gridBahanMentah.Rows.Item(i).Cells(5).Value
            Dim statusBahanMentah = gridBahanMentah.Rows.Item(i).Cells(9).Value

            If statusBahanMentah = "Rusak" Then
                StatusBahanMentahList = 1
            End If

            If Qty <> Stok Then
                statusFaktur = 2
            End If

            If Qty <> 0 Then
                If flag = 1 Then
                    StockBahanMentah(Qty, OP, harga, KdBahanMentah, Attribute, Trim(txtID.Text), "Form Retur", StatusBahanMentahList)

                    Dim sqlRetur = " Update trheaderreturbeli set StatusRetur = '1' " & _
                        " Where kdretur = '" & Trim(txtID.Text) & "' "
                    execute_update_manual(sqlRetur)
                End If
                sqlDetail = "insert into trdetailreturbeli(KdRetur,KdBarang, Harga, " _
                               & " Qty,Disc,StatusBarang) values( " _
                               & " '" & Trim(txtID.Text) & "','" & KdBahanMentah & "', " _
                               & " '" & harga & "', '" & Qty & "', '" & disc & "'," _
                               & " '" & statusBahanMentah & "')"
                execute_update_manual(sqlDetail)
            End If
        Next

        If flag = 1 Then
            Dim sqlFaktur = " Update trheaderpb set StatusTerimaBarang = '" & statusFaktur & "' " & _
                        " Where NO_PO = '" & Trim(cmbPO.Text) & "' "
            execute_update_manual(sqlFaktur)
            Return True
        End If
        Return True
    End Function

    Function save(ByVal flag As String)
        If cmbPO.SelectedIndex = 0 Then
            msgInfo("No Penerimaan Bahan Mentah harus dipilih")
            cmbPO.Focus()
        ElseIf cmbStatusRetur.SelectedIndex = 0 Then
            msgInfo("Status Penerimaan Bahan Mentah  harus dipilih")
            cmbStatusRetur.Focus()
        Else
            Dim Grandtotal = ""
            Dim checkQty = 0

            For i As Integer = 0 To (gridBahanMentah.Rows.Count - 1)
                If gridBahanMentah.Rows.Item(i).Cells(7).Value <> 0 Then
                    checkQty += 1
                    If gridBahanMentah.Rows.Item(i).Cells(9).Value = "- Klik disini -" Then
                        msgInfo("Klik status Bahan Mentah.")
                        gridBahanMentah.Rows.Item(i).Cells(9).Selected = True
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

            If isPaid > 0 Then
                isPaid = 1
            End If

            dbconmanual.Open()
            Dim trans As MySql.Data.MySqlClient.MySqlTransaction

            trans = dbconmanual.BeginTransaction(IsolationLevel.ReadCommitted)

            Try
                If PK = "" Then
                    sql = " insert into  trheaderreturbeli ( KdRetur, No_PO, TanggalRetur, " & _
                          " KdSupplier, GrandTotal, StatusRetur, Note,STATUS, " & _
                          " UserID, StatusTerimaBarang, TipeReturBeli, AfterPaid " & _
                          " ) values('" + Trim(txtID.Text) + "', " & _
                          " '" & cmbPO.Text & "', " & _
                          " '" & dtpRetur.Value.ToString("yyyy/MM/dd HH:mm:ss") & "', " & _
                          " '" & Trim(kdSupplier) & "'," & _
                          " '" & Trim(Grandtotal) & "','" & flag & "', " & _
                          " '" & txtNote.Text & "','" & cmbStatusRetur.Text & "', " & _
                          " '" & kdKaryawan & "',0,1,'" & isPaid & "')"
                    execute_update_manual(sql)
                Else
                    sql = "update   trheaderreturbeli  set  TanggalRetur='" & dtpRetur.Value.ToString("yyyy/MM/dd HH:mm:ss") & "'," & _
                    " No_PO='" & cmbPO.Text & "'," & _
                    " KdSupplier='" & Trim(kdSupplier) & "'," & _
                    " GrandTotal='" & Trim(Grandtotal) & "', " & _
                    " Status='" & Trim(cmbStatusRetur.Text) & "', " & _
                    " Note='" & Trim(txtNote.Text) & "', " & _
                    " UserID='" & kdKaryawan & "', " & _
                    " AfterPaid='" & isPaid & "' " & _
                    " where  KdRetur = '" + txtID.Text + "' "
                    execute_update_manual(sql)
                End If

                execute_update_manual("delete from Trdetailreturbeli where  kdretur = '" & txtID.Text & "'")
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
            If gridBahanMentah.Rows.Count <> 0 Then
                For i As Integer = 0 To (gridBahanMentah.Rows.Count - 1)
                    Dim total = gridBahanMentah.Rows.Item(i).Cells(8).Value.ToString.Replace(".", "").Replace(",", "")
                    Grandtotal = Val(Grandtotal) + Val(total)
                Next
            End If
            lblGrandtotal.Text = FormatNumber(Grandtotal, 0)
        Catch ex As Exception
            MsgBox(ex.ToString, MsgBoxStyle.Critical, "Warning!!!")
        End Try
    End Sub

    Private Sub gridBarang_CellClick(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewCellEventArgs) Handles gridBahanMentah.CellClick
        If gridBahanMentah.CurrentRow.Cells(9).Selected = True Then
            sub_form = New FormBrowseStatusRetur
            sub_form.showDialog(FormMain)
            If data_carier(0) <> "" Then
                gridBahanMentah.CurrentRow.Cells(9).Value = data_carier(0)
                clear_variable_array()
            End If
        End If
    End Sub

    Private Sub gridBarang_CellEndEdit(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewCellEventArgs) Handles gridBahanMentah.CellEndEdit
        Try
            Dim BahanMentahID = gridBahanMentah.CurrentRow.Cells(0).Value
            Dim harga = Val(gridBahanMentah.CurrentRow.Cells(4).Value.Replace(".", "").Replace(",", ""))
            Dim qty = Val(gridBahanMentah.CurrentRow.Cells(7).Value)
            Dim disc = Val(gridBahanMentah.CurrentRow.Cells(6).Value)
            Dim stok = Val(gridBahanMentah.CurrentRow.Cells(5).Value)
            Dim qtyStock = general_check_stock(BahanMentahID, qty, "Stock")

            If IsNumeric(qty) = False Then
                MsgBox("Jumlah retur harus berupa angka.", MsgBoxStyle.Information, "Validation Error")
                qty = stok
                gridBahanMentah.CurrentRow.Cells(7).Value = stok
                gridBahanMentah.CurrentRow.Cells(7).Selected = True
            ElseIf qty > stok Then
                MsgBox("Jumlah retur melebihi Bahan Mentah yang diterima", MsgBoxStyle.Information, "Validation Error")
                qty = stok
                gridBahanMentah.CurrentRow.Cells(7).Value = stok
                gridBahanMentah.CurrentRow.Cells(7).Selected = True
            ElseIf qty > qtyStock Then
                MsgBox("Jumlah retur melebihi stock barang", MsgBoxStyle.Information, "Validation Error")
                qty = qtyStock
                gridBahanMentah.CurrentRow.Cells(7).Value = qtyStock
                gridBahanMentah.CurrentRow.Cells(7).Selected = True
            ElseIf IsNumeric(disc) = False Then
                MsgBox("Diskon harus berupa angka.", MsgBoxStyle.Information, "Validation Error")
                disc = 0
                gridBahanMentah.CurrentRow.Cells(6).Value = 0
                gridBahanMentah.CurrentRow.Cells(6).Selected = True
            Else
                Dim TempHarga = FormatNumber(harga, 0)
                gridBahanMentah.CurrentRow.Cells(4).Value = TempHarga
            End If
            ' MsgBox(FormatNumber((Val(harga) * Val(qty)), 0))
            gridBahanMentah.CurrentRow.Cells(8).Value = FormatNumber((Val(harga) * Val(qty)) * ((100 - Val(disc)) / 100), 0)

            HitungTotal()
        Catch ex As Exception
            MsgBox(ex.ToString, MsgBoxStyle.Critical, "Warning!!!")
        End Try
    End Sub

    Private Sub BrowseFaktur_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BrowsePO.Click
        Try
            data_carier(0) = PK
            data_carier(1) = "ReturPB"
            data_carier(2) = 1
            sub_form = New FormBrowsePB
            sub_form.showDialog(FormMain)
            If data_carier(0) <> "" Then
                cmbPO.Text = data_carier(0)
                kdSupplier = data_carier(2)
                txtSupplier.Text = data_carier(1)
                isPaid = data_carier(3)
                clear_variable_array()

                Dim reader = execute_reader(" Select MB.KdFising,TypeFising,JenisFising, " & _
                                            " Harga,sum(pb.Qty) - ifnull(( Select sum(Qty) " & _
                                            " from trdetailreturbeli " & _
                                            " Join trheaderreturbeli On trdetailreturbeli.KdRetur = trheaderreturbeli.KdRetur " & _
                                            " where KdBarang = pb.KdBarang " & _
                                            " And No_PO = hpb.No_PO " & _
                                            " Group By No_PO,KdBarang ),0) Qty, " & _
                                            " Disc " & _
                                            " from trdetailpb pb " & _
                                            " Join trheaderpb hpb On pb.No_PB = hpb.No_PB " & _
                                            " Join MsFising MB On pb.KdBarang = MB.KdFising " & _
                                            " where No_PO = '" & cmbPO.Text & "' " & _
                                            " Group by hpb.No_PO,MB.KdFising " & _
                                            " order by TypeFising asc ")

                gridBahanMentah.Rows.Clear()
                Do While reader.Read
                    gridBahanMentah.Rows.Add()
                    gridBahanMentah.Rows.Item(gridBahanMentah.RowCount - 1).Cells(0).Value = reader(0)
                    gridBahanMentah.Rows.Item(gridBahanMentah.RowCount - 1).Cells(1).Value = reader(1)
                    gridBahanMentah.Rows.Item(gridBahanMentah.RowCount - 1).Cells(2).Value = reader(2)
                    gridBahanMentah.Rows.Item(gridBahanMentah.RowCount - 1).Cells(3).Value = FormatNumber(reader(3), 0)
                    gridBahanMentah.Rows.Item(gridBahanMentah.RowCount - 1).Cells(4).Value = reader(4)
                    gridBahanMentah.Rows.Item(gridBahanMentah.RowCount - 1).Cells(5).Value = reader(5)
                    gridBahanMentah.Rows.Item(gridBahanMentah.RowCount - 1).Cells(6).Value = 0
                    gridBahanMentah.Rows.Item(gridBahanMentah.RowCount - 1).Cells(7).Value = 0
                    gridBahanMentah.Rows.Item(gridBahanMentah.RowCount - 1).Cells(8).Value = "- Klik disini -"
                Loop
                reader.Close()

                HitungTotal()
            End If
        Catch ex As Exception
            MsgBox(ex.ToString, MsgBoxStyle.Critical, "Warning!!!")
        End Try
    End Sub

    Private Sub cmbfaktur_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmbPO.SelectedIndexChanged
        If cmbPO.SelectedIndex <> 0 Then
            Dim reader = execute_reader(" Select MB.KdBahanMentah,NamaBahanMentah,BahanMentahSubKategori, " & _
                                        " Harga,sum(Qty) - ifnull(( Select sum(Qty) " & _
                                        " from TrdetailReturbeli dr " & _
                                        " Join TrHeaderReturBeli hr On hr.KdRetur = dr.KdRetur " & _
                                        " where KdBarang = dp.KdBarang " & _
                                        " And No_PO = pb.No_PO " & _
                                        " AND Harga = dp.Harga " & _
                                        " Group By No_PO,KdBarang ),0) Qty, " & _
                                        " Disc,MT.KdSupplier, Nama, NamaBahanMentahTipe,StatusPaid " & _
                                        " from Trdetailpb dp " & _
                                        " Join trheaderpb pb On dp.no_pb = pb.no_pb " & _
                                        " Join MsBahanMentah MB On dp.KdBarang = MB.KdBahanMentah " & _
                                        " Join MsBahanMentahTipe On MsBahanMentahTipe.kdBahanMentahTipe = MB.kdBahanMentahTipe " & _
                                        " Join MsBahanMentahSubKategori On MsBahanMentahSubKategori.KdBahanMentahSubKategori = MB.KdBahanMentahSubKategori " & _
                                        " Join Mssupplier MT On MT.Kdsupplier = pb.KdSupplier " & _
                                        " where pb.No_PO = '" & cmbPO.Text & "' " & _
                                        " Group by pb.No_PO,KdBarang,harga " & _
                                        " order by NamaBahanMentah asc ")

            Dim idxfaktur = 0
            gridBahanMentah.Rows.Clear()
            Do While reader.Read
                If idxfaktur = 0 Then
                    txtSupplier.Text = reader("Nama")
                    kdSupplier = reader("KdSupplier")
                    isPaid = reader("StatusPaid")
                End If

                gridBahanMentah.Rows.Add()
                gridBahanMentah.Rows.Item(gridBahanMentah.RowCount - 1).Cells(0).Value = reader(0)
                gridBahanMentah.Rows.Item(gridBahanMentah.RowCount - 1).Cells(1).Value = reader(1)
                gridBahanMentah.Rows.Item(gridBahanMentah.RowCount - 1).Cells(2).Value = reader("NamaBahanMentahTipe")
                gridBahanMentah.Rows.Item(gridBahanMentah.RowCount - 1).Cells(3).Value = reader(2)
                gridBahanMentah.Rows.Item(gridBahanMentah.RowCount - 1).Cells(4).Value = FormatNumber(reader(3), 0) 'harga
                gridBahanMentah.Rows.Item(gridBahanMentah.RowCount - 1).Cells(5).Value = reader(4) 'qty
                gridBahanMentah.Rows.Item(gridBahanMentah.RowCount - 1).Cells(6).Value = reader("Disc")
                gridBahanMentah.Rows.Item(gridBahanMentah.RowCount - 1).Cells(7).Value = 0
                gridBahanMentah.Rows.Item(gridBahanMentah.RowCount - 1).Cells(8).Value = 0
                gridBahanMentah.Rows.Item(gridBahanMentah.RowCount - 1).Cells(9).Value = "- Klik disini -"
                idxfaktur += 1
            Loop
            reader.Close()

            HitungTotal()
        Else
            txtSupplier.Text = ""
            kdSupplier = ""
            gridBahanMentah.Rows.Clear()
        End If
    End Sub

    Private Sub btnConfirms_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnConfirms.Click
        save(1)
    End Sub

    Private Sub cmbStatusRetur_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmbStatusRetur.SelectedIndexChanged
        If cmbStatusRetur.SelectedIndex = 2 Then
            btnConfirms.Enabled = False
        ElseIf statusReturPB = 0 Then
            btnConfirms.Enabled = True
        End If
    End Sub

    Private Sub gridBarang_CellContentClick(ByVal sender As System.Object, ByVal e As System.Windows.Forms.DataGridViewCellEventArgs) Handles gridBahanMentah.CellContentClick

    End Sub
End Class