Imports System.Data.SqlClient
Public Class FormBrowsePB
    Dim tab As String
    Dim PK As String
    Dim tipe_pb = ""
    Dim kd_pb = ""
    Dim status_pb = ""

    Private Sub msgWarning(ByVal str As String)
        MsgBox(str, MsgBoxStyle.Critical, "Warning")
    End Sub

    Private Sub msgInfo(ByVal str As String)
        MsgBox(str, MsgBoxStyle.Information, "Information")
    End Sub

    Private Sub viewAllData(ByVal cr As String, ByVal opt As String)
        Dim addQuery = " GROUP BY pb.No_PO "
        sql = " select distinct pb.No_PO 'No. PO',DATE_FORMAT(Tanggal_PO,'%d %M %Y') Tanggal, " & _
              " NamaLengkap 'Nama User',MS.Nama 'Supplier', " & _
              " sum(pb.Grandtotal) Grandtotal, " & _
              " MS.KdSupplier,StatusPaid " & _
              " from  " & tab & " pb " & _
              " Join trheaderpo po On po.No_PO = pb.No_PO " & _
              " Join MsSupplier MS On MS.KdSupplier = pb.KdSupplier " & _
              " Join msuser mu On mu.userid = pb.userid " & _
              " Where TipePB = '" & tipe_pb & "' "
        sql &= QueryLevel(lvlKaryawan, "mu.", "level")
        If kd_pb <> "" And status_pb = "PurchasePayment" Then
            sql &= " And exists( Select 1 from trpurchasepayment where KdPurchasePayment = '" & kd_pb & "' And No_PO = pb.No_PO ) " & _
                   " And StatusPaid <> 1 "
        ElseIf status_pb = "PurchasePayment" Then
            addQuery = " And StatusTerimaBarang not in (0,3) " & _
                       " And StatusPaid <> 1 " & _
                       " GROUP BY pb.No_PO " & _
                       " HAVING SUM(pb.GrandTotal) - " & _
                       " IFNULL(( SELECT SUM(TotalPurchasePayment) " & _
                       " FROM trpurchasepayment WHERE pb.No_PO = No_PO ),0) > 0 "
        ElseIf kd_pb <> "" And status_pb = "ReturPB" Then
            sql &= " And exists( Select 1 from trheaderreturbeli where kdretur = '" & kd_pb & "' And No_PO = pb.No_PO )"
        ElseIf status_pb = "ReturPB" Then
            sql &= " And StatusTerimaBarang <> 0 "
        End If

        If opt <> "" Then
            Dim col As String = ""
            If opt = "No. PO" Then
                col = "po.No_PO"
            ElseIf opt = "Tanggal PO" Then
                col = "Tanggal_PO"
            ElseIf opt = "Nama Supplier" Then
                col = "MS.Nama"
            End If

            If col = "Tanggal_PO" Then
                sql &= " And DATE_FORMAT(Tanggal_PO, '%Y/%m/%d') >= '" & dtpFrom.Value.ToString("yyyy/MM/dd") & "' " & _
                       " and DATE_FORMAT(Tanggal_PO, '%Y/%m/%d') <= '" & dtpTo.Value.ToString("yyyy/MM/dd") & "' "
            Else
                sql &= "  And " & col & "  like '%" & cr & "%' "
            End If
        End If
        sql &= addQuery
        sql &= " Order By pb.no_increment Desc "
        gridPenerimaanBarang.DataSource = execute_datatable(sql)
    End Sub

    Private Sub setGrid()
        With gridPenerimaanBarang.ColumnHeadersDefaultCellStyle
            .Alignment = DataGridViewContentAlignment.MiddleCenter
            .Font = New Font(.Font.FontFamily, .Font.Size, _
              .Font.Style Or FontStyle.Bold, GraphicsUnit.Point)
            .ForeColor = Color.Gold

        End With
        gridPenerimaanBarang.Columns(0).Width = 100
        gridPenerimaanBarang.Columns(1).Width = 100
        gridPenerimaanBarang.Columns(2).Width = 100
        gridPenerimaanBarang.Columns(3).Width = 100
        gridPenerimaanBarang.Columns(4).Width = 100
        gridPenerimaanBarang.Columns(5).Visible = False
        gridPenerimaanBarang.Columns(6).Visible = False

    End Sub

    Private Sub setCmbCari()
        cmbCari.Items.Clear()
        cmbCari.Items.Add("No. PO")
        cmbCari.Items.Add("Tanggal PO")
        cmbCari.Items.Add("Nama Supplier")
        cmbCari.SelectedIndex = 0
    End Sub

    Private Sub FormMsArea_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        tab = " trheaderpb "
        PK = "  No_PB  "
        kd_pb = data_carier(0)
        status_pb = data_carier(1)
        tipe_pb = data_carier(2)
        data_carier(0) = ""
        data_carier(1) = ""
        data_carier(2) = ""
        viewAllData("", "")
        setGrid()
        setCmbCari()
    End Sub

    Private Sub Button1_Click_1(ByVal sender As System.Object, ByVal e As System.EventArgs)
        txtCari.Text = ""
        viewAllData("", "")
    End Sub

    Function getPB()
        Try
            data_carier(0) = gridPenerimaanBarang.CurrentRow.Cells(0).Value
            data_carier(1) = gridPenerimaanBarang.CurrentRow.Cells(3).Value
            data_carier(2) = gridPenerimaanBarang.CurrentRow.Cells(5).Value
            data_carier(3) = gridPenerimaanBarang.CurrentRow.Cells(6).Value
        Catch ex As Exception
            MsgBox(ex, MsgBoxStyle.Information)
        End Try
        Return False
    End Function

    Private Sub btnExit_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnExit.Click
        Me.Close()
    End Sub

    Private Sub btnAdd_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnAdd.Click
        getpb()
        Me.Close()
    End Sub

    Private Sub gridSales_DoubleClick(ByVal sender As Object, ByVal e As System.EventArgs) Handles gridPenerimaanBarang.DoubleClick
        getpb()
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

    Private Sub main_tool_strip_ItemClicked(ByVal sender As System.Object, ByVal e As System.Windows.Forms.ToolStripItemClickedEventArgs) Handles main_tool_strip.ItemClicked

    End Sub

    Private Sub btnCari_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnCari.Click
        viewAllData(txtCari.Text, cmbCari.Text)
    End Sub

    Private Sub txtCari_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles txtCari.TextChanged
        viewAllData(txtCari.Text, cmbCari.Text)
    End Sub

    Private Sub btnReset_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnReset.Click
        txtCari.Text = ""
        viewAllData("", "")
    End Sub

    Private Sub GroupBox1_Enter(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles GroupBox1.Enter

    End Sub
End Class
