Imports System.Data.SqlClient
Public Class FormBahanMentahList
    Dim tab As String
    Dim PK As String

    Private Sub msgWarning(ByVal str As String)
        MsgBox(str, MsgBoxStyle.Critical, "Warning")
    End Sub

    Private Sub msgInfo(ByVal str As String)
        MsgBox(str, MsgBoxStyle.Information, "Information")
    End Sub

    Private Sub setData(ByVal sql As String)

        Dim reader = execute_reader(sql)

        While reader.read
            txtID.Text = reader("Kode").ToString
            txtNama.Text = reader("NamaBahanMentah").ToString
            txtTipe.Text = reader("Tipe").ToString
            txtSubKategori.Text = reader("Sub Kategori").ToString
            txtQty.Text = reader("Stock").ToString
            txtRusak.Text = reader("StockRusak").ToString
            txtTotalAset.Text = FormatNumber(reader("GrandTotalAset").ToString, 0)
        End While
        reader.close()

    End Sub

    Private Sub viewAllData(ByVal cr As String, ByVal opt As String)
        sql = " select kdBahanMentah 'Kode', NamaBahanMentah, " & _
              " NamaBahanMentahTipe Tipe, BahanMentahSubKategori 'Sub Kategori', " & _
              " ifnull(( Select StockAkhir from BahanMentahHistory where isActive = 1 And KdBahanMentah = " & tab & ".KdBahanMentah order by KdHistory desc limit 1 ),0) Stock, " & _
              " ifnull(( Select sum(Qty) from msBahanMentahlist where StatusBahanMentahList = 1 And KdBahanMentah = " & tab & ".KdBahanMentah ),0) StockRusak, " & _
              " ifnull(( select sum(QtyPurchase_Plus) from BahanMentahhistory where IfNull(QtyPurchase_Plus,0) <> 0 And KdBahanMentah = " & tab & ".KdBahanMentah ),0) 'Total Pembelian', " & _
              " ifnull(( Select sum(Harga*Qty) from msBahanMentahlist where KdBahanMentah = " & tab & ".KdBahanMentah ),0) GrandTotalAset " & _
              " from  " & tab & _
              " Join msbahanmentahtipe bmt On bmt.KdBahanMentahTipe = " & tab & ".KdBahanMentahTipe" & _
              " Join msbahanmentahsubkategori bmsk On bmsk.KdBahanMentahSubKategori = " & tab & ".KdBahanMentahSubKategori" & _
              " Where MsBahanMentah.KdBahanMentah = '" & PK & "' "

        Dim sql_stock = " select sum(Qty) 'Stock', " & _
                        " Harga 'Harga Modal', sum(Harga*Qty) 'Subtotal Asset' " & _
                        " from msBahanMentahlist " & _
                        " Where KdBahanMentah = '" & PK & "' " & _
                        " Group By Harga "

        If opt <> "" Then
            Dim col As String = ""
            If opt = "Stock" Then
                col = "Qty"
            ElseIf opt = "Harga Modal" Then
                col = "Harga"
            ElseIf opt = "Subtotal Asset" Then
                col = "Harga*Qty"
            End If

            sql_stock &= "  And " & col & "  like '%" & cr & "%' "
        End If

        gridStock.DataSource = execute_datatable(sql_stock)
        setData(sql)
    End Sub

    Private Sub setCmbCari()
        cmbCari.Items.Clear()
        cmbCari.Items.Add("Stock")
        cmbCari.Items.Add("Harga Modal")
        cmbCari.Items.Add("Subtotal Asset")
        cmbCari.SelectedIndex = 0
    End Sub

    Private Sub FormMsSupplier_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        tab = " MsBahanMentah "
        PK = data_carier(0)
        clear_variable_array()
        setCmbCari()
        viewAllData("", "")
    End Sub

    Private Sub btnReset_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnReset.Click
        txtCari.Text = ""
        viewAllData("", "")
    End Sub

    Private Sub txtCari_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtCari.TextChanged
        viewAllData(txtCari.Text, cmbCari.Text)
    End Sub
End Class
