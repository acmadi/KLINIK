Imports System.Data.SqlClient

Public Class FormTrPOFising
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

        'btnAdd.Enabled = Not status
        DataGridView1.Enabled = Not status
    End Sub

    Private Sub setCmbCari()
        cmbCari.Items.Clear()
        cmbCari.Items.Add("No. PO")
        cmbCari.Items.Add("Tanggal PO")
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
        sql = " select PO.No_PO 'No. PO',DATE_FORMAT(Tanggal_PO,'%d %M %Y') Tanggal, Nama 'Nama Supplier' ," & _
              "  " & _
              " CASE WHEN StatusPO = 0 THEN 'New' WHEN StatusPO = 1 THEN 'Confirm' " & _
              " WHEN StatusPO = 2 THEN 'Barang Diterima' End 'Status PO' " & _
              " from  " & tab & " PO " & _
              " Join MsSupplier MS On MS.KdSupplier = PO.KdSupplier " & _
              " Join msuser mu On mu.userid = PO.userid " & _
              " Where 1 " & _
              " And TipePO = 2 "
        sql &= QueryLevel(lvlKaryawan, "mu.", "level")

        If opt <> "" Then
            Dim col As String = ""
            If opt = "No. PO" Then
                col = "No_PO"
            ElseIf opt = "Tanggal PO" Then
                col = "Tanggal_PO"
            ElseIf opt = "Nama Supplier" Then
                col = "Nama"

            End If

            If col = "Tanggal_PO" Then
                sql &= " And DATE_FORMAT(Tanggal_PO, '%Y/%m/%d') >= '" & dtpFrom.Value.ToString("yyyy/MM/dd") & "' " & _
                                   " and DATE_FORMAT(Tanggal_PO, '%Y/%m/%d') <= '" & dtpTo.Value.ToString("yyyy/MM/dd") & "' " & _
                                   "  "

            Else
                sql &= "  And " & col & "  like '%" & cr & "%'   "

            End If
        Else
            ' sql &= "  And No_PO <>'PO00000000'"

        End If
        sql &= " Order By PO.no_increment Desc  "

        DataGridView1.DataSource = execute_datatable(sql)
    End Sub

    Private Sub setGrid()
        With DataGridView1.ColumnHeadersDefaultCellStyle
            .Alignment = DataGridViewContentAlignment.MiddleCenter
            .Font = New Font(.Font.FontFamily, .Font.Size, _
              .Font.Style Or FontStyle.Bold, GraphicsUnit.Point)
            .ForeColor = Color.Gold

        End With
        DataGridView1.Columns(0).Width = 100
        DataGridView1.Columns(1).Width = 100
        DataGridView1.Columns(2).Width = 200
        DataGridView1.Columns(3).Width = 200
        'DataGridView1.Columns(4).Width = 100
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
        is_access = get_access("po")
        tab = " TrheaderPO "
        PK = "  NO_PO"
        ubahAktif(False)
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
            open_subpage("FormTrPOFisingManagement")
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
        Try
            If DataGridView1.RowCount <> 0 Then
                Dim selected_cell = DataGridView1.CurrentRow.Cells(0).Value
                open_subpage("FormTrPOFisingManagement", selected_cell)
                viewAllData("", "")
            Else
                msgInfo("Data pemesanan tidak ditemukan.")
            End If
        Catch ex As Exception
            MsgBox(ex.ToString, MsgBoxStyle.Critical, "Warning!!!")
        End Try
    End Sub

    Private Sub ToolStripButton1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ToolStripButton1.Click
        If DataGridView1.RowCount <> 0 Then
            idPrint = DataGridView1.CurrentRow.Cells(0).Value
            Dim query As String
            query = "select PO.NO_PO,Tanggal_PO,POD.KdBarang,TypeFising,  " & _
              "    POD.Harga,POD.Jumlah,MS.Nama,MS.Alamat,MS.Daerah,MS.NoTelp NoTelp,MS.NoHP NoHP         " & _
              "    ,JenisFising from Trheaderpo PO  " & _
              "    join trdetailpo POD on POD.no_po = PO.no_po " & _
              "    Join Mssupplier MS On MS.KdSupplier = PO.KdSupplier " & _
              "    Join MsFising MB On MB.KdFising = POD.KdBarang where PO.no_po='" & idPrint & "'"
            dropviewM("viewCetakPOFisingus11010001")
            createviewM(query, "viewCetakPOFisingus11010001")
            flagLaporan = "po_fising"
            open_subpage("CRPrintTransaksi")
        Else
            msgInfo("Pilih PO yang mau dicetak")
        End If


    End Sub

    Private Sub btnDelete_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnDelete.Click
        Try
            If (DataGridView1.RowCount) Then
                If DataGridView1.CurrentRow.Cells(3).Value = "New" Then
                    Dim selected_cell = DataGridView1.CurrentRow.Cells(0).Value
                    If MsgBox("Anda yakin ingin menghapus kode po " & selected_cell & "?", MsgBoxStyle.Question + MsgBoxStyle.YesNo, "Confirmation") = MsgBoxResult.Yes Then
                        execute_update("delete from " & tab & " where  " & PK & " = '" & selected_cell & "' And StatusPO = 0 ")
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

    Private Sub DataGridView1_CellContentClick(ByVal sender As System.Object, ByVal e As System.Windows.Forms.DataGridViewCellEventArgs) Handles DataGridView1.CellContentClick

    End Sub
End Class