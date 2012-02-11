Imports System.Data.SqlClient
Public Class FormBrowseFising
    Dim status As String
    Dim posisi As Integer = 0
    Dim tab As String
    Dim jumData As Integer = 0
    Dim PK As String

    Private Sub msgWarning(ByVal str As String)
        MsgBox(str, MsgBoxStyle.Critical, "Warning")
    End Sub

    Private Sub msgInfo(ByVal str As String)
        MsgBox(str, MsgBoxStyle.Information, "Information")
    End Sub
    Private Sub viewAllData(Optional ByVal cr As String = "", Optional ByVal cr2 As String = "", Optional ByVal opt As String = "", Optional ByVal opt2 As String = "")
        sql = " select kdFising Kode,TypeFising 'Tipe Fising', " & _
              " JenisFising 'Jenis Fising', " & _
              " IfNull((Select sum(Qty) - IfNull(( Select sum(Qty) from TrSalesOrderDetail " & _
              " Join TrSalesOrder On TrSalesOrderDetail.kdso = TrSalesOrder.kdso " & _
              " Where KdBarang = MsFisingList.KdFising " & _
              " And StatusSo = 3 " & _
              " Group By KdBarang ),0) As Stock From MsFisingList " & _
              " Where KdFising = " & tab & ".KdFising And StatusFisingList = 0 " & _
              " Group By KdFising ),0) Stock " & _
              " from  " & tab & _
              " Where 1 "
        sql &= QueryLevel(lvlKaryawan)

        Dim col As String = ""
        Dim col2 As String = ""

        If opt = "No Fising" Then
            col = "kdFising"
        ElseIf opt = "Tipe Fising" Then
            col = "TypeFising"
        ElseIf opt = "Jenis Fising" Then
            col = "JenisFising"
        End If
        If opt2 = "No Fising" Then
            col2 = "kdFising"
        ElseIf opt2 = "Tipe Fising" Then
            col2 = "TypeFising"
        ElseIf opt2 = "Jenis Fising" Then
            col2 = "JenisFising"
        End If

        If opt <> "" Then
            sql &= "  And " & col & "  like '%" & cr & "%'  "
        End If
        If opt2 <> "" Then
            sql &= "  And " & col2 & "  like '%" & cr2 & "%' "
        End If
        gridBarang.DataSource = execute_datatable(sql)
    End Sub

    Private Sub setGrid()
        With gridBarang.ColumnHeadersDefaultCellStyle
            .Alignment = DataGridViewContentAlignment.MiddleCenter
            .Font = New Font(.Font.FontFamily, .Font.Size, _
              .Font.Style Or FontStyle.Bold, GraphicsUnit.Point)
            .ForeColor = Color.Gold

        End With
        gridBarang.Columns(0).Width = 100
        gridBarang.Columns(1).Width = 100
        gridBarang.Columns(2).Width = 100
        gridBarang.Columns(3).Width = 100
    End Sub

    Private Sub setCmbCari()
        cmbCari.Items.Clear()
        cmbCari.Items.Add("No Fising")
        cmbCari.Items.Add("Tipe Fising")
        cmbCari.Items.Add("Jenis Fising")
        cmbCari.SelectedIndex = 0

        cmbCari2.Items.Clear()
        cmbCari2.Items.Add("No Fising")
        cmbCari2.Items.Add("Tipe Fising")
        cmbCari2.Items.Add("Jenis Fising")
        cmbCari2.SelectedIndex = 1
    End Sub

    Private Sub FormMsArea_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        tab = " MsFising "
        PK = "  KdFising  "
        data_carier(0) = ""
        viewAllData("", "")
        setGrid()
        setCmbCari()
    End Sub

    Private Sub txtCari_TextChanged1(ByVal sender As Object, ByVal e As System.EventArgs)
        viewAllData(txtCari.Text, cmbCari.Text)
    End Sub

    Private Sub Button1_Click_1(ByVal sender As System.Object, ByVal e As System.EventArgs)
        txtCari.Text = ""
        viewAllData("", "")
    End Sub

    Function getBarang()
        Try
            If gridBarang.RowCount > 0 Then
                data_carier(0) = gridBarang.CurrentRow.Cells("Kode").Value
                data_carier(1) = gridBarang.CurrentRow.Cells("Tipe Fising").Value
                data_carier(2) = gridBarang.CurrentRow.Cells("Jenis Fising").Value
                data_carier(3) = gridBarang.CurrentRow.Cells("Stock").Value
            End If
        Catch ex As Exception
            MsgBox(ex, MsgBoxStyle.Information)
        End Try
        Return False
    End Function

    Private Sub btnExit_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnExit.Click
        Me.Close()
    End Sub

    Private Sub btnAdd_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnAdd.Click
        getBarang()
        Me.Close()
    End Sub

    Private Sub gridBarang_DoubleClick(ByVal sender As Object, ByVal e As System.EventArgs) Handles gridBarang.DoubleClick
        getBarang()
        Me.Close()
    End Sub

    Private Sub txtCari_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles txtCari.TextChanged
        viewAllData(txtCari.Text, txtCari2.Text, cmbCari.Text, cmbCari2.Text)
    End Sub

    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
        txtCari.Text = ""
        txtCari2.Text = ""
        viewAllData("", "")
    End Sub

    Private Sub cmbCari_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmbCari.SelectedIndexChanged
        txtCari.Text = ""
        viewAllData("", "")
        txtCari.Focus()
    End Sub

    Private Sub txtCari2_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles txtCari2.TextChanged
        viewAllData(txtCari.Text, txtCari2.Text, cmbCari.Text, cmbCari2.Text)
    End Sub
End Class
