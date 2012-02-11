Imports System.Data.SqlClient

Public Class FormLapPOMentah
    Dim jumData As Integer
    Dim query As String
    Dim queryJumlah As String
    Dim tabel As String
    Dim status As Integer = 0
    Dim tg1 As String
    Dim tg2 As String
    Dim idxB As Integer
    Dim thn As Integer
    Dim idxB2 As Integer
    Dim thn2 As Integer
    Public tglMulai As String
    Public tglAkhir As String

    Private Sub dropview(ByVal viewname As String)
        Try
            Dim sql As String = " drop view  " & viewname
            execute_update(sql)
        Catch ex As Exception
        End Try
    End Sub

    Private Sub createview(ByVal q As String, ByVal viewName As String)
        Try
            Dim sql As String
            sql = " create view " & viewName & " as (" & q & " )"
            '     MsgBox(sql)
            execute_update(sql)
        Catch ex As Exception
        End Try
    End Sub

    Private Sub initGrid(ByVal s As String, ByVal s2 As String) 'area,toko
        tg1 = String.Format("{0:yyyy-MM-dd}", txtTgl.Value)
        tg2 = String.Format("{0:yyyy-MM-dd}", txtTgl2.Value)
        Dim reader As SqlDataReader = Nothing

        'query = " select c.kdcustomer, c.NamaToko,mp.KdBarang,Merk,Warna,Artikel,Jenis,sum(do.Qty) - isnull(tr.total,0) [Qty Total],do.Harga,do.Harga*(sum(do.Qty) - isnull(tr.total,0)) Subtotal  "
        query = " select DATE_FORMAT(Tanggal_PO,'%d-%m-%Y') `Tgl PO`,c.Nama `Supplier`,mp.KdBahanMentah `No. Bahan Mentah`,NamaBahanMentah " & _
        ", jumlah `Qty`  " & _
        ", CASE WHEN StatusPO = 0 THEN 'New' WHEN StatusPO = 1 THEN 'Confirm' " & _
        " WHEN StatusPO = 2 THEN 'Barang Diterima' End 'Status PO' " & _
        " from trheaderpo ho join trdetailpo do  on ho.no_po=do.no_po join Msbahanmentah mp on mp.KdBahanMentah=do.KdBarang " & _
        "  join mssupplier c on c.kdsupplier = ho.kdsupplier  " & _
        "  where  DATE_FORMAT(Tanggal_PO,'%Y-%m-%d') >='" & tg1 & "' and DATE_FORMAT(Tanggal_PO,'%Y-%m-%d') <='" & tg2 & "'" & _
        "  and StatusPO <> 0 And TipePO = 1"
        'query += "  and  NamaToko    like  '%" & s2 & "%'"

        ' query += "  group by `Tgl PO`,c.Nama,do.KdBarang,NamaBarang,Merk,Subkategori,`Qty`,Harga "

        TextBox1.Text = query
        Dim totalJual As Double = 0
        Dim jumlahHasil As Double = 0
        Dim totalJualHarga As Double = 0
        'Try
        Try
            tglMulai = tg1
            tglAkhir = tg2
            dropview("viewCetakLapPOMentahUS11010001") ' & kdKaryawan)
            createview(query, "viewCetakLapPOMentahUS11010001") ' & kdKaryawan)
            DataGridView1.DataSource = execute_datatable(query)
            jumlahHasil = DataGridView1.RowCount
            If jumlahHasil = 0 Then
                MsgBox("Tidak ada data ", MsgBoxStyle.Information)
            End If

            Dim totalValue As Double = 0
            Dim reader2 = execute_reader(query)
            Do While reader2.Read
                totalValue += reader2("Qty")
            Loop
            reader2.Close()
            lblTotal.Text = FormatNumber(totalValue, 0)
            Button3.Enabled = True

        Catch
            MsgBox("Gagal query", MsgBoxStyle.Critical)
        End Try
    End Sub
    Private Sub FormLapPOMentah_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Button3.Enabled = False
        txtTgl.Value = Convert.ToDateTime("01/" & Today.Month & "/" & Today.Year)
        txtTgl2.Value = Convert.ToDateTime(Today.Date)
    End Sub

    Private Sub Button9_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button9.Click
        initGrid("", "")
    End Sub

    Private Sub Button3_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button3.Click
        flagLaporan = "lap_po_mentah"
        'CRLaporan.Show() 
        open_subpage("CRLaporan")
    End Sub

    Private Sub Button2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button2.Click
        initGrid("", "")
    End Sub
End Class