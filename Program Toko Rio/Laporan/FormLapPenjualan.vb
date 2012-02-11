Imports System.Data.SqlClient

Public Class FormLapPenjualan
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
        Dim namaview As String = ""
        If RadioButton1.Checked = False And RadioButton2.Checked = True Then ' s2 <> "- Pilih Toko -" And s = "- Pilih Area -" Then 'per toko
            jenisReport = "toko"
            areaReport = s2
            namaview = "viewCetakLapPenjualanToko"
            'query = " select c.kdcustomer, c.NamaToko,mp.KdBarang,Merk,Warna,Artikel,Jenis,sum(do.Qty) - isnull(tr.total,0) [Qty Total],do.Harga,do.Harga*(sum(do.Qty) - isnull(tr.total,0)) Subtotal  "
            query = " select ho.KdFaktur `No Faktur`,DATE_FORMAT(TanggalFaktur,'%d-%m-%Y') `Tgl Faktur`,FORMAT(sum(qty*harga),0) `Grand Total`  " & _
            " ,CASE WHEN StatusFaktur = 0 THEN 'New' WHEN StatusFaktur = 1 THEN 'Confirm' " & _
            " WHEN StatusFaktur = 2 THEN 'Retur Sebagian' " & _
            " WHEN StatusFaktur = 3 THEN 'Retur Semua' End 'Status Faktur', " & _
            " CASE WHEN StatusPayment = 0 THEN 'Belum Lunas' " & _
            " WHEN StatusPayment = 1 THEN 'Lunas' " & _
            " WHEN StatusPayment = 2 THEN 'Bayar Setengah' End 'Pembayaran'" & _
            "  from trfaktur ho join trfakturdetail do  on ho.KdFaktur=do.KDFaktur join Msbarang mp on mp.KDBarang=do.KDBarang" & _
            "  join mstoko c on c.kdtoko = ho.kdtoko  " & _
            "  where  DATE_FORMAT(TanggalFaktur,'%Y-%m-%d') >='" & tg1 & "' and DATE_FORMAT(TanggalFaktur,'%Y-%m-%d') <='" & tg2 & "'" & _
            "  and StatusFaktur <> 0 and  NamaToko    like  '%" & s2 & "%'" & _
            "  group by ho.KdFaktur, TanggalFaktur"
            '            query += " left join total_retur tr on tr.kdbarang = mp.kdbarang "
          
        ElseIf RadioButton2.Checked = False And RadioButton1.Checked = True Then 'per wilayah
            jenisReport = "area"
            areaReport = s
            namaview = "viewCetakLapPenjualanArea"
            query = " select NamaToko,ho.KdFaktur `No Faktur`,DATE_FORMAT(TanggalFaktur,'%d-%m-%Y') `Tgl Faktur`,FORMAT(sum(qty*harga),0) `Grand Total` " & _
            " ,CASE WHEN StatusFaktur = 0 THEN 'New' WHEN StatusFaktur = 1 THEN 'Confirm' " & _
            " WHEN StatusFaktur = 2 THEN 'Retur Sebagian' " & _
            " WHEN StatusFaktur = 3 THEN 'Retur Semua' End 'Status Faktur', " & _
            " CASE WHEN StatusPayment = 0 THEN 'Belum Lunas' " & _
            " WHEN StatusPayment = 1 THEN 'Lunas' " & _
            " WHEN StatusPayment = 2 THEN 'Bayar Setengah' End 'Pembayaran'" & _
            "  from trfaktur ho join trfakturdetail do  on ho.KdFaktur=do.KDFaktur join Msbarang mp on mp.KDBarang=do.KDBarang" & _
            "  join mstoko c on c.kdtoko = ho.kdtoko  join msarea a on a.kdarea = c.kdarea  " & _
            "  where  DATE_FORMAT(TanggalFaktur,'%Y-%m-%d') >='" & tg1 & "' and DATE_FORMAT(TanggalFaktur ,'%Y-%m-%d')<='" & tg2 & "'" & _
            "  AND StatusFaktur <> 0 and  NamaArea    like  '%" & s & "%'" & _
            "  group by NamaToko,ho.KdFaktur, TanggalFaktur"
            '            query += " left join total_retur tr on tr.kdbarang = mp.kdbarang "


            'query = "insert into total_faktur(kdcustomer,NamaToko,KdBarang, merk,warna,artikel,jenis,[Qty Total],Harga,Total) " & _
            '        " select c.Kdcustomer, c.NamaToko,mp.Kdbarang,Merk,Warna,Artikel,Jenis,sum(do.Qty),do.Harga,do.Harga*sum(do.Qty)   " & _
            '        " from trheaderfaktur ho join trdetailfaktur do  on ho.noFaktur=do.noFaktur  " & _
            '        " join Msbarang mp on mp.KDBarang=do.KDBarang  " & _
            '        " join msmerk m on m.kdmerk = mp.kdmerk    " & _
            '        " join mscustomer c on c.kdcustomer = ho.kdcustomer  " & _
            '        " where  ho.Tanggal>='" & tg1 & "'  and ho.Tanggal <='" & tg2 & "'  "


            'query += " group by c.kdcustomer, c.NamaToko,mp.kdbarang,Merk,Warna,Artikel,Jenis,do.Harga "
            'Dim cmd2 As New SqlCommand(query, MainMenu.db.Connection)
            'cmd2.ExecuteNonQuery()

            'Dim jumAr As Integer = 0
            'query = "select * from total_retur"
            'reader = New SqlCommand(query, MainMenu.db.Connection).ExecuteReader
            'Do While reader.Read
            '    jumAr = jumAr + 1
            'Loop
            'reader.Close()
            'Dim kdCustomer(jumAr) As String
            'Dim kdBarang(jumAr) As String
            'Dim qtyRetur(jumAr) As String
            'Dim idAr As Integer = 0
            'reader = New SqlCommand(query, MainMenu.db.Connection).ExecuteReader
            'Do While reader.Read
            '    kdCustomer(idAr) = reader(1)
            '    kdBarang(idAr) = reader(2)
            '    qtyRetur(idAr) = reader(3)
            '    idAr = idAr + 1
            'Loop
            'reader.Close()
            'Dim x As Integer = 0
            'For x = 0 To idAr - 1
            '    query = "update total_faktur set  [qty total] = [qty total] - " & qtyRetur(x) & "  where kdcustomer='" & kdCustomer(x) & "' and kdbarang='" & kdBarang(x) & "'"

            '    Dim cmd1 As New SqlCommand(query, MainMenu.db.Connection)
            '    cmd1.ExecuteNonQuery()
            'Next
            'query = " select kdcustomer, NamaToko,KdBarang,Merk,Warna,Artikel,Jenis, [Qty Total],Harga,[Qty Total]*Harga [Subtotal]  from total_faktur "

        End If
        TextBox1.Text = query
        Dim totalJual As Double = 0
        Dim jumlahHasil As Double = 0
        Dim totalJualHarga As Double = 0
        'Try
        Try
            tglMulai = tg1
            tglAkhir = tg2
            dropview(namaview & "US11010001")
            createview(query, namaview & "US11010001")
            DataGridView1.DataSource = execute_datatable(query)
            jumlahHasil = DataGridView1.RowCount
            If jumlahHasil = 0 Then
                MsgBox("Tidak ada data ", MsgBoxStyle.Information)
            End If

            Dim totalValue As Double = 0
            Dim reader2 = execute_reader(query)
            Do While reader2.Read
                totalValue += reader2("Grand Total")
            Loop
            reader2.Close()
            lblTotal.Text = FormatNumber(totalValue, 0)
            Button3.Enabled = True

        Catch ex As Exception
            MsgBox(ex.ToString, MsgBoxStyle.Critical)
        End Try
    End Sub
    Public Sub setCmbArea()
        Dim reader = execute_reader("Select * from MsArea  where NamaArea <>'' order by NamaArea asc")
        Dim varT As String = ""
        cmbArea.Items.Clear()
        cmbArea.Items.Add("- Pilih Area -")
        Do While reader.Read
            cmbArea.Items.Add(reader(1)) ' & "-" & reader(0))
        Loop
        reader.Close()
        If cmbArea.Items.Count > 0 Then
            cmbArea.SelectedIndex = 0
        End If
        reader.Close()
    End Sub

    Private Sub setCmbCari2()
        cmbCari2.Items.Clear()
        cmbCari2.Items.Clear()
        cmbCari2.Items.Add("- Pilih Toko -")
        Dim reader = execute_reader("Select * from mstoko order by namaToko   asc")
        Do While reader.Read
            cmbCari2.Items.Add(reader(1))
        Loop
        reader.Close()
        If cmbCari2.Items.Count > 0 Then cmbCari2.SelectedIndex = 1
    End Sub

    Private Sub FormLapPenjualan_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        status = 0
        Button3.Enabled = False

        txtTgl.Value = Convert.ToDateTime("01/" & Today.Month & "/" & Today.Year)
        txtTgl2.Value = Convert.ToDateTime(Today.Date)
 
        setCmbArea()
        setCmbCari2()
        status = 2

        RadioButton1.Checked = True
        Label2.Visible = True
        cmbArea.Visible = True
        Label3.Visible = False
        cmbCari2.Visible = False
        ' initGrid("", cmbCari2.Text)
    End Sub

    Private Sub Button9_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button9.Click
        initGrid("", "")
        cmbArea.SelectedIndex = 0
        cmbCari2.SelectedIndex = 0
    End Sub

    Private Sub Button3_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button3.Click
        flagLaporan = "lap_jual"
        open_subpage("CRLaporan")
    End Sub

    Private Sub Button2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button2.Click
        view()
    End Sub

    Private Sub view()
        Dim key As String
        key = cmbArea.Text

        Dim key2 As String
        key2 = cmbCari2.Text

        If RadioButton1.Checked = True And cmbArea.Text = "- Pilih Area -" Then
            MsgBox("Area harus dipilih", MsgBoxStyle.Information)
        ElseIf RadioButton2.Checked = True And cmbArea.Text = "- Pilih Toko -" Then
            MsgBox("Toko harus dipilih", MsgBoxStyle.Information)
        Else
            initGrid(key, key2)
        End If
        'If cmbCari.Text = "Article" Then
        '    key = "Artikel"
        'ElseIf cmbCari.Text = "Kode Barang" Then
        '    key = "mp.KdBarang"
        'End If
        'If cmbCari2.Text = "Kode Customer" Then
        '    key2 = "ho.KdCustomer"
        'ElseIf cmbCari2.Text = "Nama Toko" Then
        '    key2 = "c.NamaToko"
        'End If
        ' key2 = cmbCari2.Text

    End Sub

    Private Sub RadioButton1_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles RadioButton1.CheckedChanged
        Label2.Visible = True
        cmbArea.Visible = True
        Label3.Visible = False
        cmbCari2.Visible = False
    End Sub

    Private Sub RadioButton2_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles RadioButton2.CheckedChanged
        Label2.Visible = False
        cmbArea.Visible = False
        Label3.Visible = True
        cmbCari2.Visible = True
    End Sub
End Class