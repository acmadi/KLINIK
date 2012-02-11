Imports System.Data.SqlClient
Public Class FormBrowseSO
    Dim status As String
    Dim posisi As Integer = 0
    Dim tab As String
    Dim jumData As Integer = 0
    Dim PK As String
    Dim kd_so = ""

    Private Sub msgWarning(ByVal str As String)
        MsgBox(str, MsgBoxStyle.Critical, "Warning")
    End Sub

    Private Sub msgInfo(ByVal str As String)
        MsgBox(str, MsgBoxStyle.Information, "Information")
    End Sub
    Private Sub viewAllData(ByVal cr As String, ByVal opt As String) 
        sql = " select DISTINCT SO.KdSO 'No. SO',DATE_FORMAT(TanggalSO,'%d %M %Y') Tanggal, " & _
              " NamaLengkap 'Nama User', NamaSales 'Nama Sales', " & _
              " NamaToko 'Nama Toko',Daerah, Grandtotal, " & _
              " CASE WHEN StatusSO = 0 THEN 'New' WHEN StatusSO = 1 THEN 'New' " & _
              " WHEN StatusSO = 2 THEN 'Faktur' WHEN StatusSO = 3 THEN 'Confirm' " & _
              " WHEN StatusSO = 4 THEN 'Faktur - Pending' End 'Status SO', " & _
              " MS.KdSales,MT.KdToko " & _
              " from  " & tab & " SO " & _
              " Join trsalesorderdetail sodetail On sodetail.kdSO = so.kdSO " & _
              " Join MsSales MS On MS.KdSales = SO.KdSales " & _
              " Join MsToko MT On MT.KdToko = SO.KdToko " & _
              " Join msuser mu On mu.userid = SO.userid " & _
              " where 1 "
        sql &= QueryLevel(lvlKaryawan, "mu.", "level")

        If kd_so <> "" Then
            sql &= " And exists( Select 1 from trfaktur where kdfaktur = '" & kd_so & "' And kdSO = so.kdso )"
        Else
            sql &= " And StatusSO = 3 And Not Exists( Select 1 from trfakturdetail " & _
                   " Join trfaktur On trfaktur.KdFaktur = trfakturdetail.KdFaktur " & _
                   " where trfaktur.kdSO = sodetail.kdso " & _
                   " And kdBarang = sodetail.KdBarang " & _
                   " AND qty - sodetail.qty = 0 ) " & _
                   " And StatusBarang = 0 "
        End If

        If opt <> "" Then
            Dim col As String = ""
            If opt = "No. SO" Then
                col = "SO.KdSO"
            ElseIf opt = "Tanggal SO" Then
                col = "TanggalSO"
            ElseIf opt = "Nama Sales" Then
                col = "NamaSales"
            ElseIf opt = "Nama Toko" Then
                col = "NamaToko"
            End If

            If col = "TanggalSO" Then
                sql &= " And DATE_FORMAT(TanggalSO, '%Y/%m/%d') >= '" & dtpFrom.Value.ToString("yyyy/MM/dd") & "' " & _
                                   " and DATE_FORMAT(TanggalSO, '%Y/%m/%d') <= '" & dtpTo.Value.ToString("yyyy/MM/dd") & "' "
            Else
                sql &= "  And " & col & "  like '%" & cr & "%' "
            End If
        End If
        sql &= " Order By SO.no_increment Desc,StatusSO asc "

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
        gridToko.Columns(7).Width = 100
        gridToko.Columns(8).Visible = False
        gridToko.Columns(9).Visible = False

    End Sub

    Private Sub setCmbCari()
        cmbCari.Items.Clear()
        cmbCari.Items.Add("No. SO")
        cmbCari.Items.Add("Tanggal SO")
        cmbCari.Items.Add("Nama Sales")
        cmbCari.Items.Add("Nama Toko")
        cmbCari.SelectedIndex = 0
    End Sub

    Private Sub FormMsArea_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        tab = " TrSalesOrder "
        PK = "  SO.KdSO  "
        kd_so = data_carier(0)
        data_carier(0) = ""
        viewAllData("", "")
        setGrid()
        setCmbCari()
    End Sub

    Private Sub Button1_Click_1(ByVal sender As System.Object, ByVal e As System.EventArgs)
        txtCari.Text = ""
        viewAllData("", "")
    End Sub

    Function getSO()
        Try
            data_carier(0) = gridToko.CurrentRow.Cells(0).Value
            data_carier(1) = gridToko.CurrentRow.Cells(3).Value
            data_carier(2) = gridToko.CurrentRow.Cells(4).Value
            data_carier(3) = gridToko.CurrentRow.Cells(5).Value
            data_carier(4) = gridToko.CurrentRow.Cells(8).Value
            data_carier(5) = gridToko.CurrentRow.Cells(9).Value
        Catch ex As Exception
            MsgBox(ex, MsgBoxStyle.Information)
        End Try
        Return False
    End Function

    Private Sub btnExit_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnExit.Click
        Me.Close()
    End Sub

    Private Sub btnAdd_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnAdd.Click
        getSO()
        Me.Close()
    End Sub

    Private Sub gridSales_DoubleClick(ByVal sender As Object, ByVal e As System.EventArgs) Handles gridToko.DoubleClick
        getSO()
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
End Class
