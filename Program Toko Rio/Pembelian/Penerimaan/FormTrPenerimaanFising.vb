Imports System.Data.SqlClient

Public Class FormTrPenerimaanFising
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
        DataGridView1.Enabled = Not status
    End Sub

    Private Sub setCmbCari()
        cmbCari.Items.Clear()
        cmbCari.Items.Add("No. Penerimaan")
        cmbCari.Items.Add("Tanggal Penerimaan")
        cmbCari.Items.Add("No. PO")
        cmbCari.Items.Add("Nama Supplier")
        cmbCari.SelectedIndex = 0
    End Sub

    Private Sub viewAllData(ByVal cr As String, ByVal opt As String)
        If is_access = 2 Or is_access = 4 Then
            btnAdd.Enabled = True
        ElseIf is_access = 5 Then
            btnAdd.Enabled = True
            btnDelete.Enabled = True
        End If

        sql = " select PB.No_PB 'No. Penerimaan',DATE_FORMAT(Tanggal_TerimaBarang,'%d %M %Y') Tanggal, " & _
              " No_PO 'No. PO', MS.Nama 'Nama Supplier' ," & _
              " format(PB.GrandTotal,0) `Grand Total`, " & _
              " CASE WHEN StatusTerimaBarang = 0 THEN 'New' WHEN StatusTerimaBarang = 1 THEN 'Confirm' " & _
              " WHEN StatusTerimaBarang = 2 THEN 'Retur Sebagian' " & _
              " WHEN StatusTerimaBarang = 3 THEN 'Retur Semua' " & _
              " End 'Status Penerimaan', " & _
              " CASE WHEN StatusPaid = 0 THEN 'Belum Lunas' WHEN StatusPaid = 1 THEN 'Lunas' " & _
              " End 'Status Paid' " & _
              " from  " & tab & " PB " & _
              " Join MsSupplier MS On MS.KdSupplier = PB.KdSupplier " & _
              " Join msuser mu On mu.userid = PB.userid " & _
              " Where 1 " & _
              " And TipePB = 2 "

        If opt <> "" Then
            Dim col As String = ""
            If opt = "No. Penerimaan" Then
                col = "No_PB"
            ElseIf opt = "No. PO" Then
                col = "PB.No_PO"
            ElseIf opt = "Tanggal Penerimaan" Then
                col = "Tanggal_TerimaBarang"
            ElseIf opt = "Nama Supplier" Then
                col = "MS.Nama"

            End If

            If col = "Tanggal_TerimaBarang" Then
                sql &= " And DATE_FORMAT(Tanggal_TerimaBarang, '%Y/%m/%d') >= '" & dtpFrom.Value.ToString("yyyy/MM/dd") & "' " & _
                       " and DATE_FORMAT(Tanggal_TerimaBarang, '%Y/%m/%d') <= '" & dtpTo.Value.ToString("yyyy/MM/dd") & "' "
            Else
                sql &= "  And " & col & "  like '%" & cr & "%'   "
            End If
        Else
            sql &= "  And No_PB <>'PB00000000'"

        End If
        sql &= " Order By StatusTerimaBarang asc,PB.No_PO Asc,PB.no_increment Desc  "

        DataGridView1.DataSource = execute_datatable(sql)
    End Sub

    Private Sub setGrid()
        With DataGridView1.ColumnHeadersDefaultCellStyle
            .Alignment = DataGridViewContentAlignment.MiddleCenter
            .Font = New Font(.Font.FontFamily, .Font.Size, _
              .Font.Style Or FontStyle.Bold, GraphicsUnit.Point)
            .ForeColor = Color.Gold

        End With
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
    Private Sub FormTrPO_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        is_access = get_access("pb")
        tab = " trheaderpb "
        PK = "  NO_PB"
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

    Private Sub Button1_Click_1(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnReset.Click
        txtCari.Text = ""
        viewAllData("", "")
    End Sub

    Private Sub btnAdd_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnAdd.Click
        Try
            open_subpage("FormTrPenerimaanFisingM")
            viewAllData("", "")
        Catch ex As Exception
            MsgBox(ex.ToString, MsgBoxStyle.Critical, "Warning!!!")
        End Try
    End Sub


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



    Private Sub btnCari_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnCari.Click
        viewAllData("", cmbCari.Text)
    End Sub


    Private Sub btnUpdate_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnUpdate.Click
        If DataGridView1.RowCount <> 0 Then
            Dim selected_cell = DataGridView1.CurrentRow.Cells(0).Value
            open_subpage("FormTrPenerimaanFisingM", selected_cell)
            viewAllData("", "")
        Else
            msgInfo("Data faktur tidak ditemukan.")
        End If
    End Sub

    Private Sub main_tool_strip_ItemClicked(ByVal sender As System.Object, ByVal e As System.Windows.Forms.ToolStripItemClickedEventArgs) Handles main_tool_strip.ItemClicked

    End Sub

    Private Sub ToolStripButton1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ToolStripButton1.Click
        If DataGridView1.RowCount <> 0 Then
            idPrint = DataGridView1.CurrentRow.Cells(0).Value
            Dim query As String
            query = "select pb.No_PB,Tanggal_TerimaBarang,PO.NO_PO,Tanggal_PO,MB.KdFising,TypeFising,  " & _
           " POD.Harga,POD.Qty,MS.Nama,MS.Alamat,MS.Daerah,MS.NoTelp NoTelp,MS.NoHP NoHP,JenisFising   " & _
           "  from Trheaderpo PO " & _
           "  join trheaderpb pb on Pb.no_po = PO.no_po" & _
           "  join trdetailpb POD on Pb.no_pb = POD.no_pb" & _
           "  Join Mssupplier MS On MS.KdSupplier = PO.KdSupplier " & _
           "  Join MsFising MB On MB.KdFising = POD.Kdbarang where PB.no_pb='" & idPrint & "'"
            dropviewM("viewCetakPBFisingus11010001") ' & kdKaryawan)
            createviewM(query, "viewCetakPBFisingus11010001") ' & kdKaryawan)
            flagLaporan = "pb_fising"
            open_subpage("CRPrintTransaksi")
        Else
            msgInfo("Pilih No Penerimaan yang mau dicetak")
        End If
    End Sub

    Private Sub btnDelete_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnDelete.Click
        Try
            If (DataGridView1.RowCount) Then
                If DataGridView1.CurrentRow.Cells(5).Value = "New" Then
                    Dim selected_cell = DataGridView1.CurrentRow.Cells(0).Value
                    If MsgBox("Anda yakin ingin menghapus kode penerimaan Fising " & selected_cell & "?", MsgBoxStyle.Question + MsgBoxStyle.YesNo, "Confirmation") = MsgBoxResult.Yes Then
                        execute_update("delete from " & tab & " where  " & PK & " = '" & selected_cell & "' And StatusTerimaBarang = 0 ")
                        msgInfo("Data berhasil dihapus")
                        viewAllData("", "")
                    End If
                Else
                    msgInfo("Data tidak dihapus. Hanya data yang memiliki status New yang dapat di hapus.")
                End If
            Else
                msgInfo("Data tidak ditemukan.")
            End If
        Catch ex As Exception
            msgInfo("Data tidak dapat dihapus.")
        End Try
    End Sub
End Class