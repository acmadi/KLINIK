Imports System.Data.SqlClient
Public Class FormPurchasePaymentManagamen
    Dim status As String
    Dim posisi As Integer = 0
    Dim tab As String
    Dim jumData As Integer = 0
    Dim PK As String = ""
    Dim NamaBarang As String = ""
    Dim Merk As String = ""
    Dim SubCat As String = ""
    Dim kdSupplier As String = ""
    Dim is_access As Integer

    Private Sub msgWarning(ByVal str As String)
        MsgBox(str, MsgBoxStyle.Critical, "Warning")
    End Sub

    Private Sub msgInfo(ByVal str As String)
        MsgBox(str, MsgBoxStyle.Information, "Information")
    End Sub

    Private Sub emptyField()
        dtpPayment.Text = Now()
        cmbPO.SelectedIndex = 0
        txtSupplier.Text = ""
        cmbBayar.SelectedIndex = 0
    End Sub

    Function getPayment(Optional ByVal KdPayment As String = "")
        Dim sql As String = "Select KdPurchasePayment from trPurchasePayment order by no_increment desc "

        If KdPayment <> "" Then
            sql &= "KdPurchasePayment = '" & KdPayment & "'"
        End If
        Dim reader = execute_reader(sql)
        Return reader
    End Function

    Private Sub setData()
        Try
            Dim Supplier = ""
            Dim kdPO = ""
            Dim readerPayment = execute_reader(" select KdPurchasePayment,DATE_FORMAT(TanggalPurchasePayment, '%m/%d/%Y') Tanggal, " & _
                            " No_PO, MS.KdSupplier, MS.Nama, " & _
                            " StatusPurchasePayment, " & _
                            " Note, PaidBy from trPurchasePayment payment " & _
                            " Join MsSupplier MS On MS.KdSupplier = payment.KdSupplier " & _
                            " Where kdPurchasePayment = '" & PK & "' ")

            If readerPayment.Read Then
                txtID.Text = readerPayment.Item(0)
                dtpPayment.Text = readerPayment.Item(1)
                kdPO = readerPayment.Item(2)
                Supplier = readerPayment.Item(3) & " - " & readerPayment.Item(4)
                If readerPayment.Item(5) <> 0 Then
                    btnSave.Enabled = False
                    btnConfirms.Enabled = False
                End If
                txtNote.Text = readerPayment.Item(6)
                cmbBayar.Text = readerPayment.Item(7)
            End If
            readerPayment.Close()
            txtSupplier.Text = Supplier
            cmbPO.Text = kdPO

            Dim reader = execute_reader(" Select MB.KdBarang,NamaBarang,Subkategori, " & _
                                    " Harga, Qty,Disc,StatusBarang,Merk " & _
                                    " from TrPurchasePaymentDetail payment " & _
                                    " Join MsBarang MB On payment.KdBarang = MB.KdBarang " & _
                                    " Join MsMerk On MsMerk.kdMerk = MB.kdMerk " & _
                                    " Join MsSubkategori On MsSubkategori.KdSub = MB.KdSub " & _
                                    " where kdPurchasePayment = '" & PK & "' " & _
                                    " order by NamaBarang asc ")

            gridBarang.Rows.Clear()
            Do While reader.Read
                Dim Subtotal = (Val(reader(3)) * Val(reader(4))) * ((100 - Val(reader(5))) / 100)

                gridBarang.Rows.Add()
                gridBarang.Rows.Item(gridBarang.RowCount - 1).Cells(0).Value = reader(0)
                gridBarang.Rows.Item(gridBarang.RowCount - 1).Cells(1).Value = reader(1)
                gridBarang.Rows.Item(gridBarang.RowCount - 1).Cells(2).Value = reader(7)
                gridBarang.Rows.Item(gridBarang.RowCount - 1).Cells(3).Value = reader(2)
                gridBarang.Rows.Item(gridBarang.RowCount - 1).Cells(4).Value = FormatNumber(reader(3), 0)
                gridBarang.Rows.Item(gridBarang.RowCount - 1).Cells(5).Value = reader(4)
                gridBarang.Rows.Item(gridBarang.RowCount - 1).Cells(6).Value = reader(5)
                gridBarang.Rows.Item(gridBarang.RowCount - 1).Cells(7).Value = Subtotal
                gridBarang.Rows.Item(gridBarang.RowCount - 1).Cells(8).Value = reader(6)
            Loop
            reader.Close()

            HitungTotal()
        Catch ex As Exception
            MsgBox("Error!!!!", MsgBoxStyle.Critical)
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

    Public Sub setCmbPO()
        Dim varT As String = ""
        Dim addQuery = ""
        cmbPO.Items.Clear()
        cmbPO.Items.Add("- Pilih PO -")
        If PK <> "" Then
            addQuery = " And exists( Select 1 from trPurchasePayment where kdPurchasePayment = '" & PK & "' And No_PO = trheaderpb.No_PO )"
            cmbPO.Enabled = False
            BrowsePO.Enabled = False
        Else
            'addQuery = "And statusTerimaBarang in (1,2) And StatusPaid = 0 "
            addQuery = " And statusTerimaBarang <> 0 " & _
                       " And StatusPaid <> 1 " & _
                       " GROUP BY No_PO " & _
                       " HAVING SUM(GrandTotal) - " & _
                       " IFNULL(( SELECT SUM(TotalPurchasePayment) " & _
                       " FROM trpurchasepayment WHERE trheaderpb.No_PO = No_PO ),0) > 0 "
        End If

        Dim reader = execute_reader(" Select Distinct No_PO from trheaderpb " & _
                                    " Join msuser On msuser.UserID = trheaderpb.UserID " & _
                                    " where 1 " & _
                                    " And TipePB = 0 " & _
                                    " And StatusTerimaBarang <> 3 " & _
                                    QueryLevel(lvlKaryawan, "msuser.", "level") & _
                                    addQuery & " Order By Tanggal_TerimaBarang Desc,statusTerimaBarang asc ")

        Do While reader.Read
            cmbPO.Items.Add(reader(0))
        Loop
        reader.Close()
        If cmbPO.Items.Count > 0 Then
            cmbPO.SelectedIndex = 0
        End If
        reader.Close()
    End Sub

    Private Sub FormMsSupplier_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        is_access = get_access("paymentBeli")
        PK = data_carier(0)
        status = data_carier(1)
        clear_variable_array()
        setCmbPO()
        emptyField()
        If PK = "" Then
            generateCode()
        Else
            setData()
            txtID.Text = PK
        End If
        cmbPO.Focus()
    End Sub

    Private Sub generateCode()
        Dim code As String = "FP"
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

        Dim reader = getPayment()

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

    Function SavePaymentDetail(ByVal flag As String)
        Dim sqlDetail = ""
        Dim statusPB = 1

        For i As Integer = 0 To gridBarang.RowCount - 1
            Dim statusDetail = 0
            Dim harga = gridBarang.Rows.Item(i).Cells(4).Value.ToString.Replace(".", "").Replace(",", "")
            Dim Qty = Val(gridBarang.Rows.Item(i).Cells(5).Value)
            Dim KdBarang = gridBarang.Rows.Item(i).Cells(0).Value
            Dim StatusBarang = gridBarang.Rows.Item(i).Cells(8).Value
            Dim Disc = Val(gridBarang.Rows.Item(i).Cells(6).Value)

            sqlDetail = "insert into TrPurchasePaymentDetail(KdPurchasePayment,KdBarang, Harga, " _
                           & " Qty,disc,StatusBarang) values( " _
                           & " '" & Trim(txtID.Text) & "','" & KdBarang & "', " _
                           & " '" & harga & "', '" & Qty & "', " _
                           & " '" & Disc & "', '" & StatusBarang & "')"
            execute_update_manual(sqlDetail)
        Next

        If flag = 1 Then
            Dim sqlPB = " Update trheaderpb set StatusPaid = '" & statusPB & "' " & _
                        " Where No_PO = '" & Trim(cmbPO.Text) & "' "
            execute_update_manual(sqlPB)
            Return True
        End If
        Return True
    End Function

    Function save(ByVal flag As String)
        If cmbPO.SelectedIndex = 0 Then
            msgInfo("Penerimaan Barang harus dipilih")
            cmbPO.Focus()
        ElseIf cmbBayar.SelectedIndex = 0 Then
            msgInfo("Cara Bayar harus dipilih")
            cmbBayar.Focus()
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
                    sql = " insert into  trPurchasePayment ( KdPurchasePayment, " & _
                          " No_PO, TanggalPurchasePayment, KdSupplier, TotalPurchasePayment, " & _
                          " StatusPurchasePayment, Note, UserID, PaidBy " & _
                          " ) values('" + Trim(txtID.Text) + "', " & _
                          " '" & cmbPO.Text & "', " & _
                          " '" & dtpPayment.Value.ToString("yyyy/MM/dd HH:mm:ss") & "', " & _
                          " '" & Trim(kdSupplier) & "'," & _
                          " '" & Trim(Grandtotal) & "','" & flag & "', " & _
                          " '" & txtNote.Text & "','" & kdKaryawan & "','" & cmbBayar.Text & "')"

                    execute_update_manual(sql)
                Else
                    sql = "update   trPurchasePayment  set  TanggalPurchasePayment='" & dtpPayment.Value.ToString("yyyy/MM/dd HH:mm:ss") & "'," & _
                    " No_PO='" & cmbPO.Text & "'," & _
                    " kdSupplier='" & Trim(kdSupplier) & "'," & _
                    " TotalPurchasePayment='" & Trim(Grandtotal) & "', " & _
                    " Note='" & Trim(txtNote.Text) & "', " & _
                    " StatusPurchasePayment='" & flag & "', " & _
                    " UserID='" & kdKaryawan & "', " & _
                    " PaidBy='" & cmbBayar.Text & "' " & _
                    " where  KdPurchasePayment = '" & txtID.Text & "' "
                    execute_update_manual(sql)
                End If

                execute_update_manual("delete from TrPurchasePaymentdetail where  kdPurchasePayment = '" & txtID.Text & "'")
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
                    Dim total = gridBarang.Rows.Item(i).Cells(7).Value.ToString.Replace(".", "").Replace(",", "")
                    Grandtotal = Val(Grandtotal) + Val(total)
                Next
            End If
            lblGrandtotal.Text = FormatNumber(Grandtotal, 0)
        Catch ex As Exception
            MsgBox(ex.ToString, MsgBoxStyle.Critical, "Warning!!!")
        End Try
    End Sub

    Private Sub BrowsePB_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BrowsePO.Click
        Try
            data_carier(0) = PK
            data_carier(1) = "PurchasePayment"
            data_carier(2) = 0
            sub_form = New FormBrowsePB
            sub_form.showDialog(FormMain)
            If data_carier(0) <> "" Then
                cmbPO.Text = data_carier(0)
                txtSupplier.Text = data_carier(1)
                kdSupplier = data_carier(2)
                clear_variable_array()

                Dim reader = execute_reader(" Select MB.KdBarang,NamaBarang,Subkategori, " & _
                                            " Harga,sum(Qty) - ifnull(( Select sum(Qty) " & _
                                            " from trdetailreturbeli returdetail " & _
                                            " Join trheaderreturbeli retur On returdetail.KdRetur = retur.KdRetur " & _
                                            " where KdBarang = pb.KdBarang " & _
                                            " And No_PO = trheaderpb.No_PO " & _
                                            " Group By No_PO,KdBarang ),0) Qty, " & _
                                            " pb.Disc, ifnull(( Select 'Retur' " & _
                                            " from trdetailreturbeli returdetail " & _
                                            " Join trheaderreturbeli retur On returdetail.KdRetur = retur.KdRetur " & _
                                            " where KdBarang = pb.KdBarang " & _
                                            " And No_PO = trheaderpb.No_PO " & _
                                            " Group By No_PO,KdBarang ),'Normal') StatusBarang,Merk " & _
                                            " from trdetailpb pb " & _
                                            " Join trheaderpb On pb.No_PB = trheaderpb.No_PB " & _
                                            " Join MsBarang MB On pb.KdBarang = MB.KdBarang " & _
                                            " Join MsMerk On MsMerk.kdMerk = MB.kdMerk " & _
                                            " Join MsSubkategori On MsSubkategori.KdSub = MB.KdSub " & _
                                            " where No_PO = '" & cmbPO.Text & "' " & _
                                            " And TipePB = 0 " & _
                                            " Group by MB.KdBarang,trheaderpb.No_PO,Harga " & _
                                            " order by NamaBarang asc ")

                gridBarang.Rows.Clear()
                Do While reader.Read
                    Dim Subtotal = (Val(reader(3)) * Val(reader(4))) * ((100 - Val(reader(5))) / 100)

                    gridBarang.Rows.Add()
                    gridBarang.Rows.Item(gridBarang.RowCount - 1).Cells(0).Value = reader(0)
                    gridBarang.Rows.Item(gridBarang.RowCount - 1).Cells(1).Value = reader(1)
                    gridBarang.Rows.Item(gridBarang.RowCount - 1).Cells(2).Value = reader(7)
                    gridBarang.Rows.Item(gridBarang.RowCount - 1).Cells(3).Value = reader(2)
                    gridBarang.Rows.Item(gridBarang.RowCount - 1).Cells(4).Value = FormatNumber(reader(3), 0)
                    gridBarang.Rows.Item(gridBarang.RowCount - 1).Cells(5).Value = reader(4)
                    gridBarang.Rows.Item(gridBarang.RowCount - 1).Cells(6).Value = reader(5)
                    gridBarang.Rows.Item(gridBarang.RowCount - 1).Cells(7).Value = Subtotal
                    gridBarang.Rows.Item(gridBarang.RowCount - 1).Cells(8).Value = reader(6)
                Loop
                reader.Close()

                HitungTotal()
            Else
                txtSupplier.Text = ""
                kdSupplier = ""
                gridBarang.Rows.Clear()
            End If
        Catch ex As Exception
            MsgBox(ex.ToString, MsgBoxStyle.Critical, "Warning!!!")
        End Try
    End Sub

    Private Sub cmbpb_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmbPO.SelectedIndexChanged
        If cmbPO.SelectedIndex <> 0 Then
            Dim reader = execute_reader(" Select MB.KdBarang,NamaBarang,Subkategori, " & _
                                        " Harga,sum(Qty - ifnull(( Select sum(Qty) " & _
                                        " from trdetailreturbeli returdetail " & _
                                        " Join trheaderreturbeli retur On returdetail.KdRetur = retur.KdRetur " & _
                                        " where KdBarang = pb.KdBarang " & _
                                        " And No_PO = trheaderpb.No_PO " & _
                                        " AND Harga = pb.Harga " & _
                                        " Group By No_PO,KdBarang ),0)) Qty, " & _
                                        " ifnull(( Select 'Retur' " & _
                                        " from trdetailreturbeli returdetail " & _
                                        " Join trheaderreturbeli retur On returdetail.KdRetur = retur.KdRetur " & _
                                        " where KdBarang = pb.KdBarang " & _
                                        " And No_PO = trheaderpb.No_PO " & _
                                        " AND Harga = pb.Harga " & _
                                        " Group By No_PO,KdBarang ),'Normal') StatusBarang, " & _
                                        " MS.KdSupplier, MS.Nama, " & _
                                        " Merk,pb.disc " & _
                                        " from trdetailpb pb " & _
                                        " Join trheaderpb On pb.No_PB = trheaderpb.No_PB " & _
                                        " Join MsBarang MB On pb.KdBarang = MB.KdBarang " & _
                                        " Join MsMerk On MsMerk.kdMerk = MB.kdMerk " & _
                                        " Join MsSubkategori On MsSubkategori.KdSub = MB.KdSub " & _
                                        " Join MsSupplier MS On MS.KdSupplier = trheaderpb.KdSupplier " & _
                                        " where trheaderpb.No_PO = '" & cmbPO.Text & "' " & _
                                        " Group by MB.KdBarang,trheaderpb.No_PO,Harga " & _
                                        " order by NamaBarang asc ")

            Dim idxpb = 0
            gridBarang.Rows.Clear()
            Do While reader.Read
                If idxpb = 0 Then
                    txtSupplier.Text = reader(7)
                    kdSupplier = reader(6)
                End If

                Dim Subtotal = (Val(reader(3)) * Val(reader(4))) * ((100 - Val(reader(9))) / 100)
                gridBarang.Rows.Add()
                gridBarang.Rows.Item(gridBarang.RowCount - 1).Cells(0).Value = reader(0)
                gridBarang.Rows.Item(gridBarang.RowCount - 1).Cells(1).Value = reader(1)
                gridBarang.Rows.Item(gridBarang.RowCount - 1).Cells(2).Value = reader(8)
                gridBarang.Rows.Item(gridBarang.RowCount - 1).Cells(3).Value = reader(2)
                gridBarang.Rows.Item(gridBarang.RowCount - 1).Cells(4).Value = FormatNumber(reader(3), 0)
                gridBarang.Rows.Item(gridBarang.RowCount - 1).Cells(5).Value = reader(4)
                gridBarang.Rows.Item(gridBarang.RowCount - 1).Cells(6).Value = reader(9)
                gridBarang.Rows.Item(gridBarang.RowCount - 1).Cells(7).Value = Subtotal
                gridBarang.Rows.Item(gridBarang.RowCount - 1).Cells(8).Value = reader(5)
                idxpb += 1
            Loop
            reader.Close()

            HitungTotal()
        Else
            txtSupplier.Text = ""
            kdSupplier = ""
            gridBarang.Rows.Clear()
        End If
    End Sub

    Private Sub btnConfirms_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnConfirms.Click
        save(1)
    End Sub
End Class
