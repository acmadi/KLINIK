Imports MySql.Data.MySqlClient

Public Class FormTrPOBahanMentahManagement
    Dim status As String
    Dim tab As String
    Dim PK As String = ""
    Dim tabelH As String
    Dim tabelD As String
    Dim NamaBahanMentah As String = ""
    Dim Tipe As String = ""
    Dim SubCat As String = ""
    Dim is_access As Integer

    Private Sub msgWarning(ByVal str As String)
        MsgBox(str, MsgBoxStyle.Critical, "Warning")
    End Sub

    Private Sub msgInfo(ByVal str As String)
        MsgBox(str, MsgBoxStyle.Information, "Information")
    End Sub

    'FlagStatus : StatusSO,Stock
    Function check_stock(ByVal KdBahanMentah As String, ByVal addQty As String, ByVal FlagStatus As String)
        Dim readerStock = execute_reader(" Select sum(Qty) - IfNull(( Select sum(Qty) from TrSalesOrderDetail " & _
                                         " Join TrSalesOrder On TrSalesOrderDetail.kdso = TrSalesOrder.kdso " & _
                                         " Where KdBarang = MsBahanMentahList.KdBahanMentah " & _
                                         " And StatusSo = 3 " & _
                                         " And TrSalesOrder.KdSO <> '" & txtID.Text & "' " & _
                                         " Group By KdBarang ),0) As Stock From MsBahanMentahList " & _
                                         " Where KdBahanMentah = '" & KdBahanMentah & "' " & _
                                         " And StatusBahanMentahList = 0 " & _
                                         " Group By KdBahanMentah ")
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
        End If
        readerStock.close()
        If FlagStatus = "Stock" Then
            Return 0
        End If
        Return "Ready"
    End Function

    Private Sub emptyField()
        dtpPO.Text = Now()
        gridBahanMentah.Rows.Clear()
        cmbSupplier.SelectedIndex = 0
        txtAlamat.Text = ""
        txtDaerah.Text = ""
        emptyBahanMentah()
    End Sub

    Private Sub emptyBahanMentah()
        cmbBahanMentah.SelectedIndex = 0
        txtQty.Text = ""
    End Sub

    Function getSO(Optional ByVal KdSO As String = "")
        Dim sql As String = "Select * from SalesOrder "

        If KdSO <> "" Then
            sql &= "KdSO = '" & KdSO & "'"
        End If
        Dim reader = execute_reader(sql)
        Return reader
    End Function

    Private Sub setData()
        Try
            Dim supplier = ""
            Dim readerPO = execute_reader(" select No_PO 'No PO',DATE_FORMAT(Tanggal_PO, '%m/%d/%Y') 'Tanggal', " & _
                            " MS.KdSupplier, MS.nama, " & _
                            " Alamat, MS.Daerah, StatusPO from trheaderpo PO " & _
                            " Join MsSupplier MS On MS.KdSupplier = PO.KdSupplier " & _
                            " Where No_PO = '" & PK & "' ")

            If readerPO.Read() Then
                txtID.Text = readerPO.Item("No PO")
                dtpPO.Text = readerPO.Item("Tanggal")
                supplier = readerPO.Item("KdSupplier") & " ~ " & readerPO.Item("nama")
                If readerPO.Item("StatusPO") <> 0 Then
                    btnSave.Enabled = False
                    btnConfirms.Enabled = False
                End If
            End If
            readerPO.Close()

            cmbSupplier.Text = supplier

            Dim reader = execute_reader(" Select PO.KdBarang,NamaBahanMentah, " & _
                                        " Harga,jumlah,NamaBahanMentahTipe,BahanMentahSubKategori " & _
                                        " from trdetailpo PO " & _
                                        " Join MsBahanMentah MB On PO.KdBarang = MB.KdBahanMentah " & _
                                        " Join msbahanmentahtipe bmt On bmt.KdBahanMentahTipe = MB.KdBahanMentahTipe" & _
                                        " Join msbahanmentahsubkategori bms On bms.KdBahanMentahSubKategori = MB.KdBahanMentahSubKategori" & _
                                        " where No_PO = '" & PK & "' " & _
                                        " order by NamaBahanMentah asc ")

            Do While reader.Read
                gridBahanMentah.Rows.Add()
                gridBahanMentah.Rows.Item(gridBahanMentah.RowCount - 1).Cells("clmPartNo").Value = reader("KdBarang")
                gridBahanMentah.Rows.Item(gridBahanMentah.RowCount - 1).Cells("clmNamaBarang").Value = reader("NamaBahanMentah")
                gridBahanMentah.Rows.Item(gridBahanMentah.RowCount - 1).Cells("clmTipe").Value = reader("NamaBahanMentahTipe")
                gridBahanMentah.Rows.Item(gridBahanMentah.RowCount - 1).Cells("clmSubCat").Value = reader("BahanMentahSubKategori")
                gridBahanMentah.Rows.Item(gridBahanMentah.RowCount - 1).Cells("clmQty").Value = reader("jumlah")
            Loop
            reader.Close()
        Catch ex As Exception
            MsgBox(ex, MsgBoxStyle.Critical)
        End Try
    End Sub

    Private Sub setGrid()
        With gridBahanMentah.ColumnHeadersDefaultCellStyle
            .Alignment = DataGridViewContentAlignment.MiddleCenter
            .Font = New Font(.Font.FontFamily, .Font.Size, _
              .Font.Style Or FontStyle.Bold, GraphicsUnit.Point)
            .ForeColor = Color.Gold

        End With
        gridBahanMentah.ReadOnly = False
        gridBahanMentah.Columns(0).Width = 100
        gridBahanMentah.Columns(1).Width = 200
        gridBahanMentah.Columns(2).Width = 220
        gridBahanMentah.Columns(2).ReadOnly = False
        gridBahanMentah.Columns(3).Width = 115
        gridBahanMentah.Columns(3).ReadOnly = False
        gridBahanMentah.Columns(4).Width = 115
        gridBahanMentah.Columns(4).ReadOnly = False

    End Sub


    Public Sub setCmbSupplier()
        Dim varT As String = ""
        cmbSupplier.Items.Clear()
        cmbSupplier.Items.Add("- Pilih Supplier -")
        sql = " Select * from MsSupplier" & _
              " where Nama <>'' order by Nama asc "
        Dim readerSupplier = execute_reader(sql)
        Do While readerSupplier.Read
            cmbSupplier.Items.Add(readerSupplier(0) & " ~ " & readerSupplier(1))
        Loop
        readerSupplier.Close()
        If cmbSupplier.Items.Count > 0 Then
            cmbSupplier.SelectedIndex = 0
        End If
        readerSupplier.Close()
    End Sub

    Public Sub setCmbBahanMentah()
        Dim varT As String = ""
        cmbBahanMentah.Items.Clear()
        cmbBahanMentah.Items.Add("- Pilih Bahan Mentah -")
        Dim reader = execute_reader("Select KdBahanMentah,NamaBahanMentah from msbahanmentah where 1 " & _
                                    QueryLevel(lvlKaryawan) & _
                                    " And NamaBahanMentah <>'' order by NamaBahanMentah asc")
        Do While reader.Read
            cmbBahanMentah.Items.Add(reader(0) & " ~ " & reader(1))
        Loop
        reader.Close()
        If cmbBahanMentah.Items.Count > 0 Then
            cmbBahanMentah.SelectedIndex = 0
        End If
        reader.Close()
    End Sub
    Private Sub deleteTabelTemp()
        Dim query As String
        query = " delete from trheaderpo_t where   no_po='" & txtID.Text & "'  "
        Try
            execute_update(query)
        Catch ex As Exception
            msgWarning("Error")
        End Try
    End Sub

    Private Sub FormTrPOManagement_FormClosed(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosedEventArgs) Handles Me.FormClosed
        deleteTabelTemp()
    End Sub


    Private Sub FormTrPOManagement_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        is_access = get_access("po")
        tabelH = " TrHeaderPO "
        tabelD = " TrDetailPO "
        PK = data_carier(0)
        status = data_carier(1)
        clear_variable_array()

        setCmbSupplier()
        setCmbBahanMentah()
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
        cmbSupplier.Focus()

        'insert to header pb temp
        Dim tabel As String = " TrHeaderPO_T "
        sql = "insert into " & tabel & " values ('" & txtID.Text & "')"
        Try
            execute_update(sql)
        Catch ex As Exception
        End Try
    End Sub


    Private Sub generateCode()
        Dim code As String = "PO"
        Dim angka As Integer
        Dim kode As String = ""
        Dim temp As String
        Dim bulan As Integer = CInt(Today.Month.ToString)
        code += Today.Year.ToString.Substring(2, 2)
        If bulan < 10 Then
            code += "0" + bulan.ToString
        Else
            code += bulan.ToString
        End If

        'generate code
        sql = " select no_po from  TrHeaderPO order by no_increment desc limit 1"
        Dim reader = execute_reader(sql)
        If reader.Read() Then
            kode = reader(0)
        Else
            reader.Close()
            sql = " select no_po from  TrHeaderPO_T  order by no_increment desc limit 1"
            Dim reader2 = execute_reader(sql)
            If reader2.Read() Then
                kode = reader2(0)
            Else
                kode = ""
            End If
            reader2.Close()
        End If
        reader.Close()
        reader = Nothing

        If kode <> "" Then
            temp = kode.Substring(0, 6)
            If temp = code Then
                angka = CInt(kode.Substring(6, 4))
            Else
                angka = 0
            End If

        Else
            angka = 0
        End If
        angka = angka + 1
        Dim len As Integer = angka.ToString().Length
        For i As Integer = 1 To 4 - len
            code += "0"
        Next
        code = code & (angka)
        txtID.Text = Trim(code)
    End Sub

    Function SavePODetail(ByVal flag As String)
        Dim sqlHistory = ""
        Dim sqlDetail = ""
        Dim statusPO = flag

        For i As Integer = 0 To gridBahanMentah.RowCount - 1
            Dim statusDetail = 0

            Dim Qty = Val(gridBahanMentah.Rows.Item(i).Cells("clmQty").Value)
            Dim OP = "Min"
            Dim Attribute = "QtyPO_Min"
            Dim KdBahanMentah = gridBahanMentah.Rows.Item(i).Cells("clmPartNo").Value

            sqlDetail = "insert into trDetailPO values( " _
                           & " '" & Trim(txtID.Text) & "','" & KdBahanMentah & "', " & Qty & ", 0 )"
            execute_update_manual(sqlDetail)
        Next
        Return statusPO
    End Function

    Function save(ByVal flag As String)
        If cmbSupplier.SelectedIndex = 0 Then
            msgInfo("Supplier harus dipilih")
            cmbSupplier.Focus()
        ElseIf gridBahanMentah.RowCount = 0 Then
            msgInfo("Tambah BahanMentah terlebih dahulu")
            cmbBahanMentah.Focus()
        Else
            Dim supplierID = cmbSupplier.Text.ToString.Split("~")

            dbconmanual.Open()
            Dim trans As MySql.Data.MySqlClient.MySqlTransaction

            trans = dbconmanual.BeginTransaction(IsolationLevel.ReadCommitted)

            Try

                If status = "add" Then
                    sql = " insert into  " & tabelH & " ( No_PO, userid, Tanggal_PO, " & _
                          " KdSupplier, StatusPO, TipePO " & _
                          " ) values('" + Trim(txtID.Text) + "','" & kdKaryawan & "','" + dtpPO.Value.ToString("yyyy/MM/dd HH:mm:ss") + "', " & _
                          " '" & supplierID(0) & "','" & flag & "', '1') "
                    execute_update_manual(sql)
                ElseIf status = "update" Then
                    sql = " update trheaderpo  set  Tanggal_PO='" & dtpPO.Value.ToString("yyyy/MM/dd HH:mm:ss") & "'," & _
                          " KdSupplier='" & Trim(supplierID(0)) & "'," & _
                          " StatusPO='" & flag & "'," & _
                          " UserID='" & Trim(kdKaryawan) & "' " & _
                          " where  no_po = '" + txtID.Text + "' "
                    execute_update_manual(sql)
                End If

                execute_update_manual("delete from trdetailpo where  No_PO = '" & txtID.Text & "'")
                Dim statusSO = SavePODetail(flag)
                deleteTabelTemp()
                trans.Commit()
                msgInfo("Data berhasil disimpan. Silakan cetak PO")
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

    Private Sub btnAdd_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnAdd.Click
        Try
            If cmbBahanMentah.SelectedIndex = 0 Then
                msgInfo("Bahan Mentah harus dipilih")
                cmbBahanMentah.Focus()
            ElseIf txtQty.Text = "" Then
                msgInfo("Jumlah harus diisi")
                txtQty.Focus()
            ElseIf txtQty.Text = "0" Then
                msgInfo("Jumlah harus lebih dari 0")
                txtQty.Focus()
            Else
                Dim BahanMentahID = cmbBahanMentah.Text.ToString.Split("~")
                Dim StatusBahanMentah = ""


                For i As Integer = 0 To (gridBahanMentah.RowCount - 1)
                    If gridBahanMentah.Rows(i).Cells("clmPartNo").Value.ToString = Trim(BahanMentahID(0)) Then
                        Dim addQty = Val(txtQty.Text) + gridBahanMentah.Rows.Item(i).Cells("clmQty").Value

                        gridBahanMentah.Rows.Item(i).Cells("clmTipe").Value = Trim(Tipe)
                        gridBahanMentah.Rows.Item(i).Cells("clmSubCat").Value = Trim(SubCat)
                        gridBahanMentah.Rows.Item(i).Cells("clmQty").Value = addQty

                        emptyBahanMentah()
                        Exit Sub
                    End If
                Next
                gridBahanMentah.Rows.Add()
                gridBahanMentah.Rows.Item(gridBahanMentah.RowCount - 1).Cells("clmPartNo").Value = Trim(BahanMentahID(0))
                gridBahanMentah.Rows.Item(gridBahanMentah.RowCount - 1).Cells("clmNamaBarang").Value = Trim(NamaBahanMentah)
                gridBahanMentah.Rows.Item(gridBahanMentah.RowCount - 1).Cells("clmTipe").Value = Trim(Tipe)
                gridBahanMentah.Rows.Item(gridBahanMentah.RowCount - 1).Cells("clmSubCat").Value = Trim(SubCat)
                gridBahanMentah.Rows.Item(gridBahanMentah.RowCount - 1).Cells("clmQty").Value = Val(txtQty.Text)
                emptyBahanMentah()

                gridBahanMentah.Focus()
                'End If
            End If
        Catch ex As Exception
            MsgBox(ex.ToString, MsgBoxStyle.Critical, "Warning!!!")
        End Try
    End Sub



    Private Sub cmbSupplier_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmbSupplier.SelectedIndexChanged
        If cmbSupplier.SelectedIndex <> 0 Then
            Dim supplierID = cmbSupplier.Text.ToString.Split("~")

            sql = " Select Alamat,Daerah from MsSupplier " & _
                  " where KdSupplier = '" & Trim(supplierID(0)) & "' "
            Dim reader = execute_reader(sql)

            If reader.Read Then
                txtAlamat.Text = reader(0)
                txtDaerah.Text = reader(1)
            End If
            reader.Close()
        Else
            txtAlamat.Text = ""
            txtDaerah.Text = ""
        End If
    End Sub

    Private Sub cmbBarang_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmbBahanMentah.SelectedIndexChanged
        If cmbBahanMentah.SelectedIndex <> 0 Then
            Dim BahanMentahID = cmbBahanMentah.Text.ToString.Split("~")

            sql = " Select IFNULL(StockAkhir, 0) StockAkhir, IfNull((Select Harga From MsBahanMentahList " & _
                  " where KdBahanMentah = BahanMentahHistory.KdBahanMentah And StatusBahanMentahList = 0 " & _
                  " Order By KdBahanMentahList asc Limit 1 ),0) As HargaAwal, " & _
                  " NamaBahanMentah, BahanMentahSubKategori,NamaBahanMentahTipe " & _
                  " from MsBahanMentah " & _
                  " LEFT JOIN BahanMentahHistory ON MsBahanMentah.KdBahanMentah = BahanMentahHistory.KdBahanMentah " & _
                  " LEFT Join msbahanmentahtipe bmt On bmt.KdBahanMentahTipe = MsBahanMentah.KdBahanMentahTipe" & _
                  " LEFT Join msbahanmentahsubkategori bms On bms.KdBahanMentahSubKategori = MsBahanMentah.KdBahanMentahSubKategori" & _
                  " Where IFNULL(isActive, 1) = 1 " & _
                  " And MsBahanMentah.kdBahanMentah = '" & BahanMentahID(0) & "' " & _
                  " order by KdHistory desc limit 1"
            Dim reader = execute_reader(sql)

            If reader.Read Then
                NamaBahanMentah = reader("NamaBahanMentah")
                Tipe = reader("NamaBahanMentahTipe")
                SubCat = reader("BahanMentahSubKategori")
            End If
            reader.Close()
            lblStock.Text = check_stock(BahanMentahID(0), 0, "Stock")
        Else
            lblStock.Text = 0
        End If
    End Sub

    Private Sub txtQty_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txtQty.KeyPress
        If Asc(e.KeyChar) <> Asc("-") And AscW(e.KeyChar) <> 13 And AscW(e.KeyChar) <> 8 And (Asc(e.KeyChar) < Asc("0") Or Asc(e.KeyChar) > Asc("9")) Then
            e.KeyChar = Nothing
        End If
        If AscW(e.KeyChar) = 13 And txtQty.Text <> "" Then
            btnAdd_Click(Nothing, Nothing)
        End If
    End Sub

    Private Sub browseSupplier_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles browseSupplier.Click
        Try
            sub_form = New FormBrowseSupplier
            sub_form.showDialog(FormMain)
            If data_carier(0) <> "" Then
                cmbSupplier.Text = data_carier(0) & " ~ " & data_carier(1)
                clear_variable_array()
            End If
        Catch ex As Exception
            MsgBox(ex, MsgBoxStyle.Critical, "Warning!!!")
        End Try
    End Sub

    Private Sub Button2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles browseBahanMentah.Click
        Try
            sub_form = New FormBrowseBahanMentah
            sub_form.showDialog(FormMain)
            If data_carier(0) <> "" Then
                cmbBahanMentah.Text = data_carier(0) & " ~ " & data_carier(1)
                clear_variable_array()
            End If
        Catch ex As Exception
            MsgBox(ex.ToString, MsgBoxStyle.Critical, "Warning!!!")
        End Try
    End Sub

    Private Sub btnRemove_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnRemove.Click
        Try
            If gridBahanMentah.RowCount > 0 Then
                gridBahanMentah.Rows.RemoveAt(gridBahanMentah.CurrentRow.Index)
            Else
                msgInfo("Belum ada barang")
            End If
        
        Catch ex As Exception
            MsgBox(ex, MsgBoxStyle.Critical, "Warning!!!")
        End Try
    End Sub

    Private Sub gridBarang_CellEndEdit(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewCellEventArgs) Handles gridBahanMentah.CellEndEdit
        Try
            Dim qty = Val(gridBahanMentah.CurrentRow.Cells("clmQty").Value)

            If IsNumeric(qty) = False Or qty < 1 Then
                MsgBox("Jumlah harus lebih besar dari 0 dan berupa angka.", MsgBoxStyle.Information, "Validation Error")
                qty = 1
                gridBahanMentah.CurrentRow.Cells("clmQty").Value = 1
                gridBahanMentah.CurrentRow.Cells("clmQty").Selected = True
            End If
        Catch ex As Exception
            MsgBox(ex, MsgBoxStyle.Critical, "Warning!!!")
        End Try
    End Sub

    Private Sub gridBarang_CellContentClick(ByVal sender As System.Object, ByVal e As System.Windows.Forms.DataGridViewCellEventArgs) Handles gridBahanMentah.CellContentClick

    End Sub

    Private Sub btnConfirms_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnConfirms.Click
        save(1)
    End Sub
End Class