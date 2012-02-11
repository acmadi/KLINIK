
Imports System.Data.SqlClient
Public Class FormMsFisingKomposisi
    Dim status As String
    Dim posisi As Integer = 0
    Dim tab As String
    Dim jumData As Integer = 0
    Dim PK As String
    Dim kdmerk As String
    Dim barangID(2) As String
    Dim SubKategoriID(2) As String


    Private Sub msgWarning(ByVal str As String)
        MsgBox(str, MsgBoxStyle.Critical, "Warning")
    End Sub
    Private Sub msgInfo(ByVal str As String)
        MsgBox(str, MsgBoxStyle.Information, "Information")
    End Sub

    Private Sub viewAllBahanMentah()
        sql = " select kdBahanMentah Kode,NamaBahanMentah Nama,NamaBahanMentahTipe Tipe," & _
              " BahanMentahSubKategori `Sub Kategori` " & _
              " from msbahanmentah a " & _
              " Join MsBahanMentahTipe On MsBahanMentahTipe.KdBahanMentahTipe = a.KdBahanMentahTipe " & _
              " Join MsBahanMentahSubKategori On MsBahanMentahSubKategori.KdBahanMentahSubKategori = a.KdBahanMentahSubKategori " & _
              " where kdBahanMentah not in (select kdbahanmentah from msfising_detail) " ' where kdfising = '" & kdFisingView & "') "
        If Trim(txtTipeBM.Text) <> "" Then
            sql &= " and NamaBahanMentahTipe='" & Trim(txtTipeBM.Text) & "%'"
        End If
        If Trim(txtSubBM.Text) <> "" Then
            sql &= " and BahanMentahSubKategori='" & Trim(txtSubBM.Text) & "%'"
        End If
        DataGridView2.DataSource = execute_datatable(sql)

        If DataGridView2.RowCount Then
            txtBM.Text = DataGridView2.CurrentRow.Cells(0).Value.ToString()
        End If
    End Sub


    Private Sub viewAllBahanMentahFising()
        sql = " select kdBahanMentah Kode,NamaBahanMentah Nama,NamaBahanMentahTipe Tipe," & _
              " BahanMentahSubKategori `Sub Kategori` " & _
              " from msbahanmentah a " & _
              " Join MsBahanMentahTipe On MsBahanMentahTipe.KdBahanMentahTipe = a.KdBahanMentahTipe " & _
              " Join MsBahanMentahSubKategori On MsBahanMentahSubKategori.KdBahanMentahSubKategori = a.KdBahanMentahSubKategori " & _
              " where kdBahanMentah  in (select kdbahanmentah from msfising_detail where kdfising = '" & kdFisingView & "') "

        If Trim(txtTipeBM2.Text) <> "" Then
            sql &= " and NamaBahanMentahTipe='" & Trim(txtTipeBM2.Text) & "%'"
        End If
        If Trim(txtSubBM2.Text) <> "" Then
            sql &= " and BahanMentahSubKategori='" & Trim(txtSubBM2.Text) & "%'"
        End If

        DataGridView3.DataSource = execute_datatable(sql)
        If DataGridView3.RowCount Then
            txtBM2.Text = DataGridView3.CurrentRow.Cells(0).Value.ToString()
        End If
    End Sub
   
 


    Private Sub Button3_Click_1(ByVal sender As System.Object, ByVal e As System.EventArgs)
        If Trim(txtBM2.Text) <> "" Then
            If MsgBox("Anda yakin ingin menghapus kode bahan mentah " & Trim(txtBM2.Text) & "?", MsgBoxStyle.Question + MsgBoxStyle.YesNo, "Confirmation") = MsgBoxResult.Yes Then
                TAB = " msfising_detail "
                sql = "delete from " & tab & " where kdfising= '" & Trim(kdFisingView) & "' and kdbahanmentah = '" & Trim(txtBM2.Text) & "'"
                execute_update(sql)
                msgInfo("Kode bahan mentah " & Trim(txtBM2.Text) & " mentah berhasil dihapus")
                txtBM2.Text = ""
                viewAllBahanMentah()
                viewAllBahanMentahFising()
            End If
        Else
            MsgBox("Pilih Bahan Mentah")
        End If
    End Sub
    Private Sub FormMsFisingKomposisi_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        tab = " MsFising "
        PK = "  KdFising  "
        txtID.Text = kdFisingView
        viewAllBahanMentah()
        viewAllBahanMentahFising()
        setGrid()

    End Sub

    Private Sub txtTipeBM_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs)
        viewAllBahanMentah()
    End Sub

    Private Sub txtSubBM_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs)
        viewAllBahanMentah()
    End Sub


    Private Sub txtTipeBM2_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs)
        viewAllBahanMentahFising()
    End Sub

    Private Sub txtSubBM2_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs)
        viewAllBahanMentahFising()
    End Sub

    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
        open_page("FormMsFising")
    End Sub

   
    Private Sub Button2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button2.Click
        If Trim(txtBM.Text) <> "" Then
            If MsgBox("Anda yakin ingin menambahkan kode bahan mentah " & Trim(txtBM.Text) & "?", MsgBoxStyle.Question + MsgBoxStyle.YesNo, "Confirmation") = MsgBoxResult.Yes Then
                tab = " msfising_detail "
                sql = "insert into " & tab & " values('" & Trim(kdFisingView) & "','" & Trim(txtBM.Text) & "')"
                execute_update(sql)
                msgInfo("Kode bahan mentah " & Trim(txtBM.Text) & " mentah berhasil ditambahkan")
                txtBM.Text = ""
                viewAllBahanMentah()
                viewAllBahanMentahFising()
            End If
        Else
            MsgBox("Pilih Bahan Mentah")
        End If
    End Sub

    Private Sub Button3_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button3.Click
        If Trim(txtBM2.Text) <> "" Then
            If MsgBox("Anda yakin ingin menghapus kode bahan mentah " & Trim(txtBM2.Text) & "?", MsgBoxStyle.Question + MsgBoxStyle.YesNo, "Confirmation") = MsgBoxResult.Yes Then
                tab = " msfising_detail "
                sql = "delete from " & tab & " where kdfising= '" & Trim(txtID.Text) & "' and kdbahanmentah = '" & Trim(txtBM2.Text) & "'"
                execute_update(sql)
                msgInfo("Kode bahan mentah " & Trim(txtBM2.Text) & " mentah berhasil dihapus")
                txtBM2.Text = ""
                viewAllBahanMentah()
                viewAllBahanMentahFising()
            End If
        Else
            MsgBox("Pilih Bahan Mentah")
        End If
    End Sub

    Private Sub DataGridView2_CellClick(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewCellEventArgs) Handles DataGridView2.CellClick
        If e.RowIndex >= 0 Then 'And e.RowIndex <= DataGridView2.Rows.Count - 1 Then
            txtBM.Text = DataGridView2.Rows(e.RowIndex).Cells(0).Value.ToString()
        End If
    End Sub
 
    Private Sub DataGridView3_CellClick(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewCellEventArgs) Handles DataGridView3.CellClick
        If e.RowIndex >= 0 Then 'And e.RowIndex <= DataGridView2.Rows.Count - 1 Then
            txtBM2.Text = DataGridView3.Rows(e.RowIndex).Cells(0).Value.ToString()
        End If
    End Sub

    Private Sub setGrid()
        With DataGridView2.ColumnHeadersDefaultCellStyle
            .Alignment = DataGridViewContentAlignment.MiddleCenter
            .Font = New Font(.Font.FontFamily, .Font.Size, _
              .Font.Style Or FontStyle.Bold, GraphicsUnit.Point)
            .ForeColor = Color.Gold
        End With
        DataGridView2.Columns(0).Width = 120
        DataGridView2.Columns(1).Width = 150
        DataGridView2.Columns(2).Width = 150
        DataGridView2.Columns(3).Width = 150


        With DataGridView3.ColumnHeadersDefaultCellStyle
            .Alignment = DataGridViewContentAlignment.MiddleCenter
            .Font = New Font(.Font.FontFamily, .Font.Size, _
              .Font.Style Or FontStyle.Bold, GraphicsUnit.Point)
            .ForeColor = Color.Gold
        End With
        DataGridView3.Columns(0).Width = 120
        DataGridView3.Columns(1).Width = 150
        DataGridView3.Columns(2).Width = 150
        DataGridView3.Columns(3).Width = 150

    End Sub

End Class