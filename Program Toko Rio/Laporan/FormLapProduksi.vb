
Imports System.Data.SqlClient

Public Class FormLapPRoduksi
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
        query = " select DATE_FORMAT(Tanggal,'%d-%m-%Y') `Tgl Produksi`,mp.KdBarang `Part No.`,NamaBarang,Merk,Subkategori,do.KdFising `No Fising`,TypeFising `Type Fising`,JenisFising `Jenis Fising` " & _
        ", do.Qty   "
        query += "  from trproduksi do join Msbarang mp on mp.KDBarang=do.KDBarang " & _
        "  Join MsMerk On MsMerk.kdMerk = mp.kdMerk Join MsSubkategori On MsSubkategori.KdSub = mp.KdSub " & _
        " join msfising mf on mf.kdfising = do.kdfising "
        query += "  where  DATE_FORMAT(Tanggal,'%Y-%m-%d') >='" & tg1 & "' and DATE_FORMAT(Tanggal,'%Y-%m-%d') <='" & tg2 & "'"
        TextBox1.Text = query
        Dim totalJual As Double = 0
        Dim jumlahHasil As Double = 0
        Dim totalJualHarga As Double = 0
        'Try
        Try
            tglMulai = tg1
            tglAkhir = tg2
            dropview("viewCetakLapProduksiUS11010001") ' & kdKaryawan)
            createview(query, "viewCetakLapProduksiUS11010001") ' & kdKaryawan)
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
    Private Sub FormLapPRoduksi_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load


        Button3.Enabled = False
        txtTgl.Value = Convert.ToDateTime("01/" & Today.Month & "/" & Today.Year)
        txtTgl2.Value = Convert.ToDateTime(Today.Date)

    End Sub



    Private Sub Button9_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button9.Click
        initGrid("", "")
    End Sub

    Private Sub Button3_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button3.Click
        flagLaporan = "lap_produksi"
        open_subpage("CRLaporan")
    End Sub

    Private Sub Button2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button2.Click
        initGrid("", "")
    End Sub
End Class