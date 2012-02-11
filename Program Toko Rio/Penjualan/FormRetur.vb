Imports System.Data.SqlClient
Public Class FormRetur
    Dim status As String
    Dim posisi As Integer = 0
    Dim tab As String
    Dim jumData As Integer = 0
    Dim PK As String
    Dim is_access As Integer

    Private Sub msgWarning(ByVal str As String)
        MsgBox(str, MsgBoxStyle.Critical, "Warning")
    End Sub

    Private Sub msgInfo(ByVal str As String)
        MsgBox(str, MsgBoxStyle.Information, "Information")
    End Sub

    Private Sub ubahAktif(ByVal status As Boolean)
        btnExit.Enabled = Not status
        GroupBox1.Enabled = Not status

        btnAdd.Enabled = Not status
        btnUpdate.Enabled = Not status
        btnDelete.Enabled = Not status
        DataGridView1.Enabled = Not status
        GridPendingRetur.Enabled = Not status
    End Sub

    Private Sub setCmbCari()
        cmbCari.Items.Clear()
        cmbCari.Items.Add("No. Retur")
        cmbCari.Items.Add("Tanggal Retur")
        cmbCari.Items.Add("No. Faktur")
        cmbCari.Items.Add("Nama Sales")
        cmbCari.Items.Add("Nama Toko")
        cmbCari.SelectedIndex = 0
    End Sub

    Private Sub viewAllData(ByVal cr As String, ByVal opt As String)
        If is_access = 2 Or is_access = 4 Then
            btnAdd.Enabled = True
        ElseIf is_access = 5 Then
            btnAdd.Enabled = True
            btnDelete.Enabled = True
        End If
        sql = " select KdRetur 'No. Retur',DATE_FORMAT(TanggalRetur,'%d %M %Y') Tanggal, " & _
              " retur.kdFaktur 'No. Faktur', NamaLengkap 'Nama User', NamaSales 'Nama Sales', " & _
              " NamaToko 'Nama Toko', mt.Daerah, format(retur.Grandtotal,0) Grandtotal,retur.Status, " & _
              " CASE WHEN StatusRetur = 0 THEN 'New' WHEN StatusRetur = 1 THEN 'Confirm' End 'Status Retur' " & _
              " from  " & tab & " retur " & _
              " Join mssales ms On ms.kdsales = retur.kdsales " & _
              " Join mstoko mt On mt.kdtoko = retur.kdtoko " & _
              " Join msuser mu On mu.userid = retur.userid " & _
              " Where Status = 'Oke' "
        sql &= QueryLevel(lvlKaryawan, "mu.", "level")

        Dim sqlReturPending = " select KdRetur 'No. Retur',DATE_FORMAT(TanggalRetur,'%d %M %Y') Tanggal, " & _
              " retur.kdFaktur 'No. Faktur', NamaLengkap 'Nama User', NamaSales 'Nama Sales', " & _
              " NamaToko 'Nama Toko', mt.Daerah, format(retur.Grandtotal,0) Grandtotal,retur.Status, " & _
              " CASE WHEN StatusRetur = 0 THEN 'New' WHEN StatusRetur = 1 THEN 'Confirm' End 'Status Retur' " & _
              " from  " & tab & " retur " & _
              " Join mssales ms On ms.kdsales = retur.kdsales " & _
              " Join mstoko mt On mt.kdtoko = retur.kdtoko " & _
              " Join msuser mu On mu.userid = retur.userid " & _
              " Where Status = 'Pending' "
        sqlReturPending &= QueryLevel(lvlKaryawan, "mu.", "level")

        If opt <> "" Then
            Dim col As String = ""
            If opt = "No. Retur" Then
                col = "kdretur"
            ElseIf opt = "Tanggal Retur" Then
                col = "tanggalRetur"
            ElseIf opt = "No. Faktur" Then
                col = "retur.kdFaktur"
            ElseIf opt = "Nama Sales" Then
                col = "NamaSales"
            ElseIf opt = "Nama Toko" Then
                col = "NamaToko"
            End If

            If col = "tanggalRetur" Then
                sql &= " And DATE_FORMAT(tanggalretur, '%Y/%m/%d') >= '" & dtpFrom.Value.ToString("yyyy/MM/dd") & "' " & _
                       " and DATE_FORMAT(tanggalretur, '%Y/%m/%d') <= '" & dtpTo.Value.ToString("yyyy/MM/dd") & "' "

                sqlReturPending &= " And DATE_FORMAT(tanggalretur, '%Y/%m/%d') >= '" & dtpFrom.Value.ToString("yyyy/MM/dd") & "' " & _
                                   " and DATE_FORMAT(tanggalretur, '%Y/%m/%d') <= '" & dtpTo.Value.ToString("yyyy/MM/dd") & "' "
            Else
                sql &= "  And " & col & "  like '%" & cr & "%' "
                sqlReturPending &= "  And " & col & "  like '%" & cr & "%' "
            End If
        End If
        sql &= " Order By StatusRetur asc,retur.no_increment Desc "
        sqlReturPending &= " Order By StatusRetur asc,tanggalretur Desc "

        DataGridView1.DataSource = execute_datatable(sql)
        GridPendingRetur.DataSource = execute_datatable(sqlReturPending)
    End Sub

    Private Sub setGrid()
        With DataGridView1.ColumnHeadersDefaultCellStyle
            .Alignment = DataGridViewContentAlignment.MiddleCenter
            .Font = New Font(.Font.FontFamily, .Font.Size, _
              .Font.Style Or FontStyle.Bold, GraphicsUnit.Point)
            .ForeColor = Color.Gold

        End With
        DataGridView1.Columns(0).Width = 120
        DataGridView1.Columns(1).Width = 100
        DataGridView1.Columns(2).Width = 120
        DataGridView1.Columns(3).Width = 100
        DataGridView1.Columns(4).Width = 100
        DataGridView1.Columns(5).Width = 100
        DataGridView1.Columns(6).Width = 100

        With GridPendingRetur.ColumnHeadersDefaultCellStyle
            .Alignment = DataGridViewContentAlignment.MiddleCenter
            .Font = New Font(.Font.FontFamily, .Font.Size, _
              .Font.Style Or FontStyle.Bold, GraphicsUnit.Point)
            .ForeColor = Color.Gold

        End With
        GridPendingRetur.Columns(0).Width = 100
        GridPendingRetur.Columns(1).Width = 100
        GridPendingRetur.Columns(2).Width = 100
        GridPendingRetur.Columns(3).Width = 100
        GridPendingRetur.Columns(4).Width = 100
        GridPendingRetur.Columns(5).Width = 100
        GridPendingRetur.Columns(6).Width = 100
    End Sub

    Function visibleDate()
        dtpFrom.Visible = True
        dtpTo.Visible = True
        lblSeperator.Visible = True
        btnCari.Visible = True

        txtCari.Visible = False
        btnReset.Visible = False
        Return False
    End Function

    Function visibleCari()
        dtpFrom.Visible = False
        dtpTo.Visible = False
        lblSeperator.Visible = False
        btnCari.Visible = False

        txtCari.Visible = True
        btnReset.Visible = True
        Return False
    End Function

    Private Sub FormMsArea_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        is_access = get_access("returJual")
        tab = " trretur "
        PK = "  kdretur  "
        'ubahAktif(False)
        viewAllData("", "")
        posisi = 0
        setGrid()
        setCmbCari()
        visibleCari()
    End Sub

    Private Sub txtCari_TextChanged1(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtCari.TextChanged
        viewAllData(txtCari.Text, cmbCari.Text)
    End Sub

    Private Sub DataGridView1_CellClick(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewCellEventArgs) Handles DataGridView1.CellClick
        If e.RowIndex >= 0 And e.RowIndex <= DataGridView1.Rows.Count - 1 Then
            posisi = e.RowIndex
        End If
    End Sub

    Private Sub Button1_Click_1(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnReset.Click
        txtCari.Text = ""
        viewAllData("", "")
    End Sub

    Private Sub btnAdd_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnAdd.Click
        Try
            open_subpage("FormReturManagamen")
            viewAllData("", "")
        Catch ex As Exception
            MsgBox(ex.ToString, MsgBoxStyle.Critical, "Warning!!!")
        End Try
    End Sub

    Private Sub btnUpdate_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnUpdate.Click
        Try
            If Retur_tab.SelectedIndex = 0 Then
                If DataGridView1.RowCount <> 0 Then
                    Dim selected_cell = DataGridView1.CurrentRow.Cells(0).Value
                    open_subpage("FormReturManagamen", selected_cell)
                Else
                    msgInfo("Data retur tidak ditemukan.")
                End If
            ElseIf Retur_tab.SelectedIndex = 1 Then
                If GridPendingRetur.RowCount <> 0 Then
                    Dim selected_cell = GridPendingRetur.CurrentRow.Cells(0).Value
                    open_subpage("FormReturManagamen", selected_cell)
                Else
                    msgInfo("Data retur tidak ditemukan.")
                End If
            End If
        Catch ex As Exception
            MsgBox(ex.ToString, MsgBoxStyle.Critical, "Warning!!!")
        End Try
        viewAllData("", "")
    End Sub

    'Private Sub btnDelete_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnDelete.Click
    '    If (DataGridView1.RowCount) Then
    '        Dim selected_cell = DataGridView1.CurrentRow.Cells(0).Value
    '        If MsgBox("Anda yakin ingin menghapus kode area " & selected_cell & "?", MsgBoxStyle.Question + MsgBoxStyle.YesNo, "Confirmation") = MsgBoxResult.Yes Then
    '            Dim cmd As New SqlCommand("delete from " & tab & " where  " & PK & " = '" & selected_cell & "'", db.Connection)
    '            cmd.ExecuteNonQuery()
    '            msgInfo("Data berhasil dihapus")
    '            posisi = 0
    '            viewAllData("", "")
    '        End If
    '    Else
    '        msgInfo("Data tidak ditemukan.")
    '    End If
    'End Sub

    Private Sub btnExit_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnExit.Click
        Me.Close()
    End Sub

    Private Sub cmbCari_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmbCari.SelectedIndexChanged
        If cmbCari.SelectedIndex = 1 Then
            visibleDate()
        Else
            visibleCari()
        End If
        txtCari.Text = ""
        viewAllData("", "")
        txtCari.Focus()
    End Sub

    Private Sub DataGridView1_DoubleClick(ByVal sender As Object, ByVal e As System.EventArgs) Handles DataGridView1.DoubleClick
        Try
            Dim selected_cell = DataGridView1.CurrentRow.Cells(0).Value
            open_subpage("FormReturManagamen", selected_cell)
            viewAllData("", "")
        Catch ex As Exception
            MsgBox(ex.ToString, MsgBoxStyle.Critical, "Warning!!!")
        End Try
    End Sub

    Private Sub btnCari_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnCari.Click
        viewAllData("", cmbCari.Text)
    End Sub

    Private Sub GridPendingRetur_DoubleClick(ByVal sender As Object, ByVal e As System.EventArgs) Handles GridPendingRetur.DoubleClick
        Try
            Dim selected_cell = GridPendingRetur.CurrentRow.Cells(0).Value
            open_subpage("FormReturManagamen", selected_cell)
            viewAllData("", "")
        Catch ex As Exception
            MsgBox(ex.ToString, MsgBoxStyle.Critical, "Warning!!!")
        End Try
    End Sub

    Private Sub sales_order_main_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles sales_order_main.Click

    End Sub

    Private Sub ToolStripButton1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ToolStripButton1.Click
        If DataGridView1.RowCount <> 0 Then
            idPrint = DataGridView1.CurrentRow.Cells(0).Value
            Dim query As String
            query = "select retur.KdRetur ,TanggalRetur,retur.kdFaktur , NamaSales ,  " & _
                    " NamaToko, retur.Grandtotal, retur.Status, mp.KDBarang, NamaBarang,HargaDisc `harga`, qty, disc " & _
                    "  from  trretur  retur " & _
                    " join trreturdetail rd on rd.kdretur = retur.kdretur " & _
                    " join msbarang mp on mp.kdbarang = rd.kdbarang " & _
                    " Join mssales ms On ms.kdsales = retur.kdsales   " & _
                    " Join mstoko mt On mt.kdtoko = retur.kdtoko   " & _
                    " Join msuser mu On mu.userid = retur.userid  " & _
                    "  where   retur.kdretur='" & idPrint & "' "
            dropviewM("viewCetakTrReturJualus11010001")
            createviewM(query, "viewCetakTrReturJualus11010001")
            flagLaporan = "retur_jual"
            open_subpage("CRPrintTransaksi")
        Else
            msgInfo("Pilih No retur yang mau dicetak")
        End If
    End Sub

    Private Sub btnDelete_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnDelete.Click
        Try
            If Retur_tab.SelectedIndex = 0 Then
                If (DataGridView1.RowCount) Then
                    If DataGridView1.CurrentRow.Cells("Status Retur").Value = "New" Then
                        Dim selected_cell = DataGridView1.CurrentRow.Cells(0).Value
                        If MsgBox("Anda yakin ingin menghapus kode retur " & selected_cell & "?", MsgBoxStyle.Question + MsgBoxStyle.YesNo, "Confirmation") = MsgBoxResult.Yes Then
                            execute_update("delete from " & tab & " where  " & PK & " = '" & selected_cell & "' And StatusRetur = 0 ")
                            msgInfo("Data berhasil dihapus")
                            viewAllData("", "")
                        End If
                    Else
                        msgInfo("Data tidak dihapus. Hanya data yang memiliki status New yang dapat di hapus.")
                    End If
                Else
                    msgInfo("Data tidak ditemukan.")
                End If
            ElseIf Retur_tab.SelectedIndex = 1 Then
                If (GridPendingRetur.RowCount) Then
                    If GridPendingRetur.CurrentRow.Cells("Status Retur").Value = "New" Then
                        Dim selected_cell = GridPendingRetur.CurrentRow.Cells(0).Value
                        If MsgBox("Anda yakin ingin menghapus kode retur " & selected_cell & "?", MsgBoxStyle.Question + MsgBoxStyle.YesNo, "Confirmation") = MsgBoxResult.Yes Then
                            execute_update("delete from " & tab & " where  " & PK & " = '" & selected_cell & "' And StatusRetur = 0 ")
                            msgInfo("Data berhasil dihapus")
                            viewAllData("", "")
                        End If
                    Else
                        msgInfo("Data tidak dihapus. Hanya data yang memiliki status New yang dapat di hapus.")
                    End If
                Else
                    msgInfo("Data tidak ditemukan.")
                End If
            End If
        Catch ex As Exception
            msgInfo("Data tidak dapat dihapus.")
        End Try
    End Sub

    Private Sub dtpFrom_ValueChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles dtpFrom.ValueChanged

    End Sub

    Private Sub dtpTo_ValueChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles dtpTo.ValueChanged

    End Sub

    Private Sub lblSeperator_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles lblSeperator.Click

    End Sub
End Class
