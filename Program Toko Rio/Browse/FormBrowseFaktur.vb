Imports System.Data.SqlClient
Public Class FormBrowseFaktur
    Dim status As String
    Dim posisi As Integer = 0
    Dim tab As String
    Dim jumData As Integer = 0
    Dim PK As String
    Dim addQuery = ""
    Dim status_faktur = ""
    Dim kd_payment = ""

    Private Sub msgWarning(ByVal str As String)
        MsgBox(str, MsgBoxStyle.Critical, "Warning")
    End Sub

    Private Sub msgInfo(ByVal str As String)
        MsgBox(str, MsgBoxStyle.Information, "Information")
    End Sub
    Private Sub viewAllData(ByVal cr As String, ByVal opt As String)
        addQuery = " GROUP BY faktur.kdFaktur "

        sql = " select distinct faktur.kdFaktur 'No. Faktur',DATE_FORMAT(TanggalFaktur,'%d %M %Y') Tanggal, " & _
              " NamaLengkap 'Nama User',NamaSales 'Nama Sales', " & _
              " NamaToko 'Nama Toko',Daerah, sum(faktur.Grandtotal) Grandtotal, " & _
              " CASE WHEN Statusfaktur = 0 THEN 'New' WHEN Statusfaktur = 1 THEN 'Confirm' " & _
              " WHEN Statusfaktur = 2 THEN 'Retur Sebagian' " & _
              " WHEN Statusfaktur = 3 THEN 'Retur Semua' End 'Status faktur', " & _
              " MS.KdSales,MT.KdToko, " & _
              " CASE WHEN StatusPayment = 0 THEN 'Belum Lunas' " & _
              " WHEN StatusPayment = 1 THEN 'Lunas' " & _
              " WHEN StatusPayment = 2 THEN 'Bayar Setengah' End 'Pembayaran', " & _
              " StatusPayment " & _
              " from  " & tab & " faktur " & _
              " Join MsSales MS On MS.KdSales = faktur.KdSales " & _
              " Join MsToko MT On MT.KdToko = faktur.KdToko " & _
              " Join msuser mu On mu.userid = faktur.userid " & _
              " Where 1 "
        sql &= QueryLevel(lvlKaryawan, "mu.", "level")

        If kd_payment <> "" And status_faktur = "SalesPayment" Then
            sql &= " And exists( Select 1 from trsalesPayment where kdsalesPayment = '" & kd_payment & "' And kdFaktur = faktur.kdFaktur ) " & _
                   " And StatusPayment <> 1 "
        ElseIf status_faktur = "SalesPayment" Then
            addQuery = " And statusfaktur <> 0 " & _
                       " And StatusPayment <> 1 " & _
                       " GROUP BY faktur.kdFaktur " & _
                       " HAVING SUM(faktur.GrandTotal) - " & _
                       " IFNULL(( SELECT SUM(TotalSalesPayment) " & _
                       " FROM trsalespayment WHERE faktur.kdFaktur = kdFaktur ),0) > 0 "
        ElseIf kd_payment <> "" And status_faktur = "ReturFaktur" Then
            sql &= " And exists( Select 1 from trretur where kdretur = '" & kd_payment & "' And kdFaktur = trfaktur.kdFaktur )"
        ElseIf status_faktur = "ReturFaktur" Then
            sql &= " And statusfaktur <> 0 "
        End If

        If opt <> "" Then
            Dim col As String = ""
            If opt = "No. Faktur" Then
                col = "faktur.kdFaktur"
            ElseIf opt = "Tanggal Faktur" Then
                col = "TanggalFaktur"
            ElseIf opt = "Nama Sales" Then
                col = "NamaSales"
            ElseIf opt = "Nama Toko" Then
                col = "NamaToko"
            ElseIf opt = "Daerah" Then
                col = "Daerah"
            End If

            If col = "TanggalFaktur" Then
                sql &= " And DATE_FORMAT(TanggalFaktur, '%Y/%m/%d') >= '" & dtpFrom.Value.ToString("yyyy/MM/dd") & "' " & _
                       " and DATE_FORMAT(TanggalFaktur, '%Y/%m/%d') <= '" & dtpTo.Value.ToString("yyyy/MM/dd") & "' "
            Else
                sql &= "  And " & col & "  like '%" & cr & "%' "
            End If
        End If
        sql &= addQuery
        sql &= " Order By faktur.no_increment Desc,StatusFaktur asc "

        gridToko.DataSource = execute_datatable(sql)
    End Sub

    Private Sub setGrid()
        With gridToko.ColumnHeadersDefaultCellStyle
            .Alignment = DataGridViewContentAlignment.MiddleCenter
            .Font = New Font(.Font.FontFamily, .Font.Size, _
              .Font.Style Or FontStyle.Bold, GraphicsUnit.Point)
            .ForeColor = Color.Gold

        End With
        gridToko.Columns(0).Width = 120
        gridToko.Columns(1).Width = 100
        gridToko.Columns(2).Width = 100
        gridToko.Columns(3).Width = 100
        gridToko.Columns(4).Width = 100
        gridToko.Columns(5).Width = 100
        gridToko.Columns(6).Width = 100
        gridToko.Columns(8).Visible = False
        gridToko.Columns(9).Visible = False
        gridToko.Columns("StatusPayment").Visible = False

    End Sub

    Private Sub setCmbCari()
        cmbCari.Items.Clear()
        cmbCari.Items.Add("No. Faktur")
        cmbCari.Items.Add("Tanggal Faktur")
        cmbCari.Items.Add("Nama Sales")
        cmbCari.Items.Add("Nama Toko")
        cmbCari.Items.Add("Daerah")
        cmbCari.SelectedIndex = 0
    End Sub

    Private Sub FormMsArea_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        tab = " Trfaktur "
        PK = "  Kdfaktur  "
        kd_payment = data_carier(0)
        status_faktur = data_carier(1)
        data_carier(0) = ""
        data_carier(1) = ""
        viewAllData("", "")
        setGrid()
        setCmbCari()
    End Sub

    Private Sub Button1_Click_1(ByVal sender As System.Object, ByVal e As System.EventArgs)
        txtCari.Text = ""
        viewAllData("", "")
    End Sub

    Function getFaktur()
        Try
            data_carier(0) = gridToko.CurrentRow.Cells("No. Faktur").Value
            data_carier(1) = gridToko.CurrentRow.Cells("Nama Sales").Value
            data_carier(2) = gridToko.CurrentRow.Cells("Nama Toko").Value
            data_carier(3) = gridToko.CurrentRow.Cells("Daerah").Value
            data_carier(4) = gridToko.CurrentRow.Cells("KdSales").Value
            data_carier(5) = gridToko.CurrentRow.Cells("KdToko").Value
            data_carier(6) = gridToko.CurrentRow.Cells("KdToko").Value
        Catch ex As Exception
            MsgBox(ex, MsgBoxStyle.Information)
        End Try
        Return False
    End Function

    Private Sub btnExit_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnExit.Click
        Me.Close()
    End Sub

    Private Sub btnAdd_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnAdd.Click
        getFaktur()
        Me.Close()
    End Sub

    Private Sub gridSales_DoubleClick(ByVal sender As Object, ByVal e As System.EventArgs) Handles gridToko.DoubleClick
        getFaktur()
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

    Private Sub txtCari_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles txtCari.TextChanged
        viewAllData(txtCari.Text, cmbCari.Text)
    End Sub

    Private Sub btnCari_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnCari.Click
        viewAllData(txtCari.Text, cmbCari.Text)
    End Sub

    Private Sub btnReset_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnReset.Click
        txtCari.Text = ""
        viewAllData("", "")
    End Sub
End Class