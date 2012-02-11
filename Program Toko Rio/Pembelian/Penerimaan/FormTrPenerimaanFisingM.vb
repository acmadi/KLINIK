Public Class FormTrPenerimaanFisingM
    Dim status As String
    Dim posisi As Integer = 0
    Dim tab As String
    Dim jumData As Integer = 0
    Dim PK As String = ""
    Dim tabelH As String
    Dim tabelD As String
    Dim TipeFising As String = ""
    Dim JenisFising As String = ""
    Dim kdsales As String = ""
    Dim kdtoko As String = ""
    Dim query As String = ""
    Dim is_access As Integer

    Private Sub msgWarning(ByVal str As String)
        MsgBox(str, MsgBoxStyle.Critical, "Warning")
    End Sub

    Private Sub msgInfo(ByVal str As String)
        MsgBox(str, MsgBoxStyle.Information, "Information")
    End Sub

    Function getRealQty(ByVal kdFising As String)
        Dim sqlRealQty = " SELECT SUM(jumlah) - IFNULL( ( SELECT SUM(Qty) " & _
                        " FROM trdetailpb PB " & _
                        " JOIN trheaderpb ON  trheaderpb.No_PB = PB.No_PB " & _
                        " JOIN MsSupplier MS ON MS.KdSupplier = trheaderpb.KdSupplier " & _
                        " WHERE No_PO = trheaderpo.No_PO " & _
                        " AND StatusTerimaBarang > 0  " & _
                        " AND kdbarang = trdetailpo.kdbarang " & _
                        " GROUP BY kdbarang,No_PO " & _
                        " ) , 0 ) qty,kdbarang " & _
                        " FROM(trdetailpo) " & _
                        " JOIN trheaderpo ON  trheaderpo.No_PO = trdetailpo.No_PO " & _
                        " WHERE trdetailpo.No_PO = '" & cmbPO.Text & "' " & _
                        " AND StatusPO > 0 " & _
                        " AND kdbarang = '" & kdFising & "' " & _
                        " AND TipePO = 2 " & _
                        " GROUP BY kdbarang,trheaderpo.No_PO "
        Dim readerRealQty = execute_reader(sqlRealQty)
        If readerRealQty.read Then
            Return readerRealQty("qty")
        End If
        readerRealQty.close()
        Return 0
    End Function

    Private Sub setData()
        Try
            Dim Supplier = ""
            Dim kdPO = ""
            Dim readerPB = execute_reader(" select No_PB,DATE_FORMAT(Tanggal_TerimaBarang, '%d %M %Y') Tanggal, " & _
                            " PO.No_PO,Tanggal_PO, MS.KdSupplier, MS.Nama, " & _
                            " StatusTerimaBarang " & _
                            " from trheaderpb PB " & _
                            " Join trheaderpo PO On  PO.No_PO = PB.No_PO " & _
                            " Join MsSupplier MS On MS.KdSupplier = PB.KdSupplier " & _
                            " Where No_PB = '" & PK & "' ")
            If readerPB.Read Then
                txtID.Text = readerPB.Item(0)
                dtpPB.Text = readerPB.Item(1)
                kdPO = readerPB.Item(2)
                txtTglPO.Text = readerPB.Item(3).ToString
                txtKdSupplier.Text = readerPB.Item(4).ToString
                txtSupplier.Text = readerPB.Item(5)
                If readerPB.Item(6) > 0 Then
                    btnSave.Enabled = False
                    btnConfirms.Enabled = False
                End If
            End If
            readerPB.Close()

            cmbPO.Text = kdPO

            Dim reader = execute_reader(" Select MB.KdFising,TypeFising,JenisFising, " & _
                                    " Harga,PB.Qty, " & _
                                    " Disc,QtyPenerimaan " & _
                                    " from trdetailpb PB " & _
                                    " JOIN  trheaderpb ON trheaderpb.No_PB = PB.No_PB " & _
                                    " Join MsFising MB On PB.KdBarang = MB.KdFising " & _
                                    " where PB.No_PB = '" & PK & "' " & _
                                    " order by TypeFising asc ")
            gridFising.Rows.Clear()
            Do While reader.Read
                Dim Subtotal = (Val(reader(3)) * Val(reader(4))) * ((100 - Val(reader(6))) / 100)

                gridFising.Rows.Add()
                gridFising.Rows.Item(gridFising.RowCount - 1).Cells(0).Value = reader(0)
                gridFising.Rows.Item(gridFising.RowCount - 1).Cells(1).Value = reader(1)
                gridFising.Rows.Item(gridFising.RowCount - 1).Cells(2).Value = reader(2)
                gridFising.Rows.Item(gridFising.RowCount - 1).Cells(3).Value = FormatNumber(reader(3), 0)
                gridFising.Rows.Item(gridFising.RowCount - 1).Cells(4).Value = reader(4)
                gridFising.Rows.Item(gridFising.RowCount - 1).Cells(5).Value = reader(5)
                gridFising.Rows.Item(gridFising.RowCount - 1).Cells(6).Value = FormatNumber(Subtotal, 0)
                gridFising.Rows.Item(gridFising.RowCount - 1).Cells(7).Value = reader(6)
            Loop
            reader.Close()

            HitungTotal()
        Catch ex As Exception
            MsgBox(ex, MsgBoxStyle.Critical)
        End Try
    End Sub

    Private Sub emptyField()
        dtpPB.Text = Now()
        cmbPO.SelectedIndex = 0
        txtTglPO.Text = ""
        txtKdSupplier.Text = ""
        txtSupplier.Text = ""
        gridFising.Rows.Clear()
    End Sub

    Private Sub deleteTabelTemp()
        query = " delete from trheaderpb_t where   no_pb='" & txtID.Text & "'  "
        Try
            execute_update(query)
        Catch ex As Exception
            msgWarning("Error")
        End Try
    End Sub

    Private Sub FormTrPenerimaanBarangM_FormClosed(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosedEventArgs) Handles Me.FormClosed
        'delete data sampah
        deleteTabelTemp()
    End Sub
    Private Sub setCmbPO()
        Dim addQuery = ""
        cmbPO.Items.Clear()
        cmbPO.Items.Add("- Pilih PO -")

        If PK <> "" Then
            addQuery = " And exists( Select 1 from trheaderpb where No_PB = '" & PK & "' And No_PO = TrHeaderPO.No_PO )"
            cmbPO.Enabled = False
            BrowsePO.Enabled = False
        Else
            addQuery &= " And StatusPO <> 0 AND no_po IN ( " & _
                        " SELECT no_po FROM trdetailpo " & _
                        " WHERE 1 " & _
                        " AND NOT EXISTS( SELECT 1 FROM trdetailpb  " & _
                        " JOIN trheaderpb ON trheaderpb.No_PB = trdetailpb.No_PB  " & _
                        " WHERE(trheaderpb.No_PO = TrHeaderPO.No_PO ) " & _
                        " AND kdBarang = trdetailpo.KdBarang  " & _
                        " GROUP BY trheaderpb.No_PO  " & _
                        " HAVING SUM(qty) - trdetailpo.Jumlah = 0 )) "
        End If

        sql = " Select DISTINCT no_po from TrHeaderPO " & _
              " Join msuser On msuser.UserID = TrHeaderPO.UserID " & _
              " where 1 " & _
              " And TipePO = 2 " & _
              addQuery & _
              QueryLevel(lvlKaryawan, "msuser.", "level")

        Dim reader = execute_reader(sql)
        Do While reader.Read
            cmbPO.Items.Add(reader.GetString(0))
        Loop
        reader.Close()
        If cmbPO.Items.Count > 0 Then
            cmbPO.SelectedIndex = 0
        End If
    End Sub
    Private Sub FormTrPenerimaanBarangM_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        is_access = get_access("pb")
        tabelH = " TrHeaderPB "
        tabelD = " TrDetailPB "
        PK = " No_PB"

        PK = data_carier(0)
        status = data_carier(1)
        clear_variable_array()
        setCmbPO()

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

        'insert to header pb temp
        Dim tabel As String = " TrHeaderPO_T "
        sql = "insert into " & tabel & " values ('" & txtID.Text & "')"
        Try
            execute_update(sql)
        Catch ex As Exception
        End Try
    End Sub


    Private Sub generateCode()
        Dim code As String = "PB"
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
        sql = " select no_PB from  TrHeaderPB order by no_increment desc limit 1"
        Dim reader = execute_reader(sql)
        If reader.Read() Then
            kode = reader(0)
        Else
            reader.Close()
            sql = " select no_PB from  TrHeaderPB_T  order by no_increment desc limit 1"
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


    Private Sub cmbPO_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmbPO.SelectedIndexChanged
        If cmbPO.SelectedIndex <> 0 Then
            sql = " select hpo.kdsupplier,nama,DATE_FORMAT(tanggal_po,'%d %M %Y') from  TrHeaderPO hpo join mssupplier s on s.kdsupplier=hpo.kdsupplier where no_PO='" & cmbPO.Text & "'"
            Dim reader = execute_reader(sql)
            If reader.Read() Then
                txtKdSupplier.Text = reader(0)
                txtSupplier.Text = reader(1)
                txtTglPO.Text = reader(2).ToString
            End If
            reader.Close()

            reader = Nothing
            sql = " select dpo.kdbarang,TypeFising,JenisFising, " & _
                  " dpo.harga,dpo.jumlah - ifnull(( Select sum(Qty) " & _
                  " from trdetailpb " & _
                  " Join trheaderpb On trdetailpb.No_PB = trheaderpb.No_PB " & _
                  " where KdBarang = dpo.KdBarang " & _
                  " And No_PO = dpo.No_PO " & _
                  " Group By No_PO,KdBarang ),0) jumlah," & _
                  " dpo.jumlah*dpo.harga  `total` " & _
                  " from  TrdetailPO dpo " & _
                  " join msFising b on b.kdFising=dpo.kdbarang " & _
                  " where no_PO='" & cmbPO.Text & "' "
            reader = execute_reader(sql)
            Dim harga As Double = 0
            gridFising.Rows.Clear()
            Do While reader.Read
                gridFising.Rows.Add()
                gridFising.Rows.Item(gridFising.RowCount - 1).Cells(0).Value = Trim(reader(0))
                gridFising.Rows.Item(gridFising.RowCount - 1).Cells(1).Value = Trim(reader(1))
                gridFising.Rows.Item(gridFising.RowCount - 1).Cells(2).Value = Trim(reader(2))
                gridFising.Rows.Item(gridFising.RowCount - 1).Cells(3).Value = FormatNumber(reader(3), 0)
                gridFising.Rows.Item(gridFising.RowCount - 1).Cells(4).Value = Val(reader(4))
                gridFising.Rows.Item(gridFising.RowCount - 1).Cells(5).Value = 0
                gridFising.Rows.Item(gridFising.RowCount - 1).Cells(6).Value = FormatNumber(reader(5), 0)
                gridFising.Rows.Item(gridFising.RowCount - 1).Cells(7).Value = reader(4)
            Loop
            reader.Close()

            HitungTotal()
        Else
            emptyField()
        End If
    End Sub


    Private Sub gridBarang_CellEndEdit(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewCellEventArgs) Handles gridFising.CellEndEdit
        Try
            If gridFising.CurrentRow.Cells(3).Value <> "" Then
                gridFising.CurrentRow.Cells(3).Value = gridFising.CurrentRow.Cells(3).Value.Replace(".", "").Replace(",", "")
            End If
            Dim harga = Val(gridFising.CurrentRow.Cells(3).Value)
            Dim qty = Val(gridFising.CurrentRow.Cells(4).Value)
            Dim Disc = Val(gridFising.CurrentRow.Cells(5).Value)
            Dim RealPenerimaan = Val(gridFising.CurrentRow.Cells(7).Value)

            If qty <= 0 Or qty > RealPenerimaan Then
                MsgBox("Jumlah harus berupa angka dan lebih kecil dari " & RealPenerimaan & ".", MsgBoxStyle.Information, "Validation Error")
                qty = 1
                gridFising.CurrentRow.Cells(4).Value = 1
                gridFising.CurrentRow.Cells(4).Selected = True
            ElseIf Disc < 0 Or IsNumeric(gridFising.CurrentRow.Cells(5).Value) = False Then
                MsgBox("Diskon harus berupa angka.", MsgBoxStyle.Information, "Validation Error")
                Disc = 0
                gridFising.CurrentRow.Cells(5).Value = 0
                gridFising.CurrentRow.Cells(5).Selected = True
            ElseIf harga <= 0 Then
                MsgBox("Harga harus berupa angka dan tidak sama dengan 0.", MsgBoxStyle.Information, "Validation Error")
                harga = 1
                gridFising.CurrentRow.Cells(3).Value = 1
                gridFising.CurrentRow.Cells(3).Selected = True
            Else
                Dim TempHarga = FormatNumber(harga, 0)
                gridFising.CurrentRow.Cells(3).Value = TempHarga
            End If
            gridFising.CurrentRow.Cells(6).Value = FormatNumber((Val(harga) * Val(qty)) * ((100 - Val(Disc)) / 100), 0)
            HitungTotal()
        Catch ex As Exception
            MsgBox(ex, MsgBoxStyle.Critical, "Warning!!!")
        End Try
    End Sub

    Private Sub HitungTotal()
        Try
            Dim Grandtotal = 0
            If gridFising.Rows.Count <> 0 Then
                For i As Integer = 0 To (gridFising.Rows.Count - 1)
                    Dim total = gridFising.Rows.Item(i).Cells(6).Value.ToString.Replace(".", "").Replace(",", "")
                    Grandtotal = Val(Grandtotal) + Val(total)
                Next
            End If
            lblGrandtotal.Text = FormatNumber(Grandtotal, 0)
        Catch ex As Exception
            MsgBox(ex.ToString, MsgBoxStyle.Critical, "Warning!!!")
        End Try
    End Sub

    Function SavePBDetail(ByVal flag As String)
        Dim sqlHistory = ""
        Dim sqlDetail = ""
        Dim StatusTerimaFising = ""
        Dim statusPenerimaan = 0

        For i As Integer = 0 To gridFising.RowCount - 1
            Dim statusDetail = 0

            Dim harga = gridFising.Rows.Item(i).Cells(3).Value.ToString.Replace(".", "").Replace(",", "")
            Dim Qty = Val(gridFising.Rows.Item(i).Cells(4).Value)
            Dim disc = Val(gridFising.Rows.Item(i).Cells(5).Value)
            Dim OP = "Plus"
            Dim Attribute = "QtyPurchase_Plus"
            Dim KdFising = gridFising.Rows.Item(i).Cells(0).Value
            Dim realQty = getRealQty(KdFising)
            Dim HargaDisc = (Val(harga)) * ((100 - Val(disc)) / 100)

            If flag = 1 Then
                StockFising(Qty, OP, HargaDisc, KdFising, Attribute, Trim(txtID.Text), "Form Penerimaan Fising")
            End If

            If Qty = realQty Then
                statusPenerimaan = 2
            End If

            sqlDetail = "insert into trDetailPB ( No_PB, KdBarang, " & _
                        " Qty, Harga, Disc, QtyPenerimaan ) values( " _
                           & " '" & Trim(txtID.Text) & "','" & KdFising & "', " & Qty & ", " _
                           & " '" & harga & "','" & disc & "','" & realQty & "')"
            execute_update_manual(sqlDetail)
        Next

        If flag = 1 And statusPenerimaan > 0 Then
            Dim sqlPO = " Update trheaderpo set StatusPO = '" & statusPenerimaan & "' " & _
                        " Where No_PO = '" & Trim(cmbPO.Text) & "' "
            execute_update_manual(sqlPO)
        End If
        Return True
    End Function

    Function save(ByVal flag As String)
        If cmbPO.SelectedIndex = 0 Then
            msgInfo("No PO harus dipilih")
            cmbPO.Focus()
        Else
            Dim Grandtotal = ""

            If (lblGrandtotal.Text <> "") Then
                Grandtotal = lblGrandtotal.Text.ToString.Replace(".", "").Replace(",", "")
            End If

            dbconmanual.Open()
            Dim trans As MySql.Data.MySqlClient.MySqlTransaction

            trans = dbconmanual.BeginTransaction(IsolationLevel.ReadCommitted)

            Try

                If PK = "" Then
                    sql = " insert into  " & tabelH & " ( No_PB, " & _
                          "	No_PO, userid, KdSupplier, Tanggal_TerimaBarang, " & _
                          " StatusTerimaBarang, GrandTotal, TipePB ) " & _
                          " values('" + Trim(txtID.Text) + "','" & Trim(cmbPO.Text) & "', " & _
                          " '" & kdKaryawan & "','" & Trim(txtKdSupplier.Text) & "', " & _
                          " '" + dtpPB.Value.ToString("yyyy/MM/dd HH:mm:ss") + "', " & _
                          " '" & Trim(flag) & "','" & Trim(Grandtotal) & "', '2') "
                    execute_update_manual(sql)
                Else
                    sql = " update " & tabelH & " set Tanggal_TerimaBarang='" & dtpPB.Value.ToString("yyyy/MM/dd HH:mm:ss") & "'," & _
                          " No_PO='" & cmbPO.Text & "'," & _
                          " userid='" & Trim(kdKaryawan) & "'," & _
                          " KdSupplier='" & Trim(txtKdSupplier.Text) & "'," & _
                          " GrandTotal='" & Trim(Grandtotal) & "', " & _
                          " StatusTerimaBarang='" & Trim(flag) & "' " & _
                          " where  No_PB = '" + txtID.Text + "' "
                    execute_update_manual(sql)
                End If

                execute_update_manual("delete from trdetailpb where  No_PB = '" & txtID.Text & "'")
                SavePBDetail(flag)

                trans.Commit()
                deleteTabelTemp()
                msgInfo("Data berhasil disimpan. Silakan cetak Penerimaan Fising")
                Me.Close()
            Catch ex As Exception
                trans.Rollback()
                MsgBox(ex, MsgBoxStyle.Critical)
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

    Private Sub BrowseSO_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BrowsePO.Click
        Try
            data_carier(0) = 2
            sub_form = New FormBrowsePO
            sub_form.showDialog(FormMain)
            If data_carier(0) <> "" Then
                txtTglPO.Text = data_carier(0)
                txtKdSupplier.Text = data_carier(1)
                txtSupplier.Text = data_carier(2)
                cmbPO.Text = data_carier(3)
                clear_variable_array()

                'sql = " select dpo.kdbarang,TypeFising,JenisFising, " & _
                '      " dpo.harga,dpo.jumlah,dpo.jumlah*dpo.harga 'total' " & _
                '      " from  TrdetailPO dpo " & _
                '      " join msFising b on b.kdFising=dpo.kdbarang " & _
                '      " where no_PO='" & cmbPO.Text & "' "

                'Dim reader = execute_reader(sql)
                'Dim harga As Double = 0
                'gridFising.Rows.Clear()
                'Do While reader.Read
                '    gridFising.Rows.Add()
                '    gridFising.Rows.Item(gridFising.RowCount - 1).Cells(0).Value = Trim(reader(0))
                '    gridFising.Rows.Item(gridFising.RowCount - 1).Cells(1).Value = Trim(reader(1))
                '    gridFising.Rows.Item(gridFising.RowCount - 1).Cells(2).Value = Trim(reader(2))
                '    gridFising.Rows.Item(gridFising.RowCount - 1).Cells(3).Value = FormatNumber(reader(3), 0)
                '    gridFising.Rows.Item(gridFising.RowCount - 1).Cells(4).Value = Val(reader(4))
                '    gridFising.Rows.Item(gridFising.RowCount - 1).Cells(5).Value = 0
                '    gridFising.Rows.Item(gridFising.RowCount - 1).Cells(6).Value = FormatNumber(reader(5), 0)
                '    gridFising.Rows.Item(gridFising.RowCount - 1).Cells(7).Value = reader(4)
                'Loop
                'reader.Close()

                'HitungTotal()
            End If
        Catch ex As Exception
            MsgBox(ex.ToString, MsgBoxStyle.Critical, "Warning!!!")
        End Try
    End Sub

    Private Sub btnConfirms_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnConfirms.Click
        save(1)
    End Sub
End Class