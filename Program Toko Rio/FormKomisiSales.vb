Imports System.Data.SqlClient

Public Class FormKomisiSales
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
            TextBox1.Text = sql
            execute_update(sql)
        Catch ex As Exception
            MsgBox(1)
        End Try
    End Sub

    Private Sub initGrid()
        tg1 = String.Format("{0:yyyy-MM-dd}", txtTgl.Value)
        tg2 = String.Format("{0:yyyy-MM-dd}", txtTgl2.Value)

        Dim sql As String = ""
        Dim sql2 As String = ""
        Dim queryFrom As String = ""
        'sql = "select NamaSales `Nama Sales` , sum(qty * (harga - (harga*disc/100))) * Komisi /100 `Jumlah Komisi` from trsalesorder so join trsalesorderdetail sod on sod.kdso=so.kdso " & _
        '" join mssales sls on sls.kdsales=so.kdsales  join trfaktur fk on fk.kdso=so.kdso " & _
        '" where statusfaktur <> 0 and (DATE_FORMAT(TanggalSO,'%Y-%m-%d') >='" & tg1 & "' and DATE_FORMAT(TanggalSO,'%Y-%m-%d') <='" & tg2 & "')  group by sls.kdsales  "
        'FORMAT(sum(qty*harga),0)
        'sql = "select NamaSales `Nama Sales` , FORMAT(sum(qty * (harga - (harga*disc/100))) * Komisi /100,0) `Jumlah Komisi` from trsalesorder so " & _
        ' " join mssales sls on sls.kdsales=so.kdsales  join trfaktur fk on fk.kdso=so.kdso  join trfakturdetail fkd on fkd.kdfaktur=fk.kdfaktur " & _
        '" where statusfaktur <> 0 And StatusPayment = 1 and (DATE_FORMAT(TanggalSO,'%Y-%m-%d') >='" & tg1 & "' and DATE_FORMAT(TanggalSO,'%Y-%m-%d') <='" & tg2 & "')  group by sls.kdsales  "

        'commented 14 okt
        'sql = "select so.kdso `No SO`,fk.kdfaktur `No Faktur`,DATE_FORMAT(TanggalFaktur,'%d/%m/%Y') `Tgl Faktur`,NamaToko `Toko`,NamaSales `Nama Sales`," & _
        '" sp.kdbarang `Part No.` ,Qty `Qty Faktur`, " & _
        '" IFNULL( (select ifnull(dr.qty,0) from trretur r join trreturdetail dr on dr.kdretur=r.kdretur  where AfterPaid =1 and r.KdFaktur= fk.KdFaktur and dr.kdbarang=sp.KdBarang and (DATE_FORMAT(TanggalRetur,'%Y-%m-%d') >='" & tg1 & "' and DATE_FORMAT(TanggalRetur,'%Y-%m-%d') <='" & tg1 & "')),0)  `Qty Retur`," & _
        '" FORMAT(Harga,0) Harga,Disc," & _
        '" FORMAT( (qty -IFNULL( (select ifnull(dr.qty,0) from trretur r join trreturdetail dr on dr.kdretur=r.kdretur  where AfterPaid =1 and r.KdFaktur= fk.KdFaktur and dr.kdbarang=sp.KdBarang and (DATE_FORMAT(TanggalRetur,'%Y-%m-%d') >='" & tg1 & "' and DATE_FORMAT(TanggalRetur,'%Y-%m-%d') <='" & tg1 & "')),0) )   * (harga - (harga*disc/100)),0) Total, " & _
        '" fk.KomisiSales `% Komisi` , " & _
        '" FORMAT((qty-IFNULL( (select ifnull(dr.qty,0) from trretur r join trreturdetail dr on dr.kdretur=r.kdretur  where AfterPaid =1 and r.KdFaktur= fk.KdFaktur and dr.kdbarang=sp.KdBarang and (DATE_FORMAT(TanggalRetur,'%Y-%m-%d') >='" & tg1 & "' and DATE_FORMAT(TanggalRetur,'%Y-%m-%d') <='" & tg1 & "')),0)) * (harga - (harga*disc/100)) * fk.KomisiSales /100,0) `Jumlah Komisi`  " & _
        '" from trsalesorder so " & _
        '" join mssales sls on sls.kdsales=so.kdsales  " & _
        '" join trfaktur fk on fk.kdso=so.kdso " & _
        '" join trsalespayment tsp on tsp.KdFaktur = fk.KdFaktur" & _
        '" join  trsalespaymentdetail sp on sp.KdSalesPayment = tsp.KdSalesPayment " & _
        '" join mstoko mt on mt.kdtoko = so.KdToko " & _
        '" where statusfaktur <> 0 And StatusPayment = 1 and (DATE_FORMAT(TanggalSalesPayment,'%Y-%m-%d') >='" & tg1 & "' and DATE_FORMAT(TanggalSalesPayment,'%Y-%m-%d') <='" & tg2 & "')   " & _
        '" and NamaSales = '" & Trim(cmbSales.Text) & "'"


        'comment 10 nov
        'sql = " select so.kdso `No SO`,fk.kdfaktur `No Faktur`,DATE_FORMAT(TanggalFaktur,'%d/%m/%Y') `Tgl Faktur`,NamaToko `Toko`,NamaSales `Nama Sales` " & _
        '", Format(TotalKomisiSales - ifnull((select ifnull(sum(dr.qty *(harga*disc/100)),0) " & _
        '" from trretur r join trreturdetail dr on dr.kdretur=r.kdretur  where AfterPaid =1 and r.KdFaktur= fk.KdFaktur " & _
        '" and (DATE_FORMAT(TanggalRetur,'%Y-%m-%d') >='" & tg1 & "' and DATE_FORMAT(TanggalRetur,'%Y-%m-%d') <='" & tg1 & "')" & _
        '" ),0),0) 'Jumlah Komisi' "
        'sql2 = " select so.kdso `No SO`,fk.kdfaktur `No Faktur`,DATE_FORMAT(TanggalFaktur,'%d/%m/%Y') `Tgl Faktur`,NamaToko `Toko`,NamaSales `Nama Sales` " & _
        '     ",TotalKomisiSales - ifnull((select ifnull(sum(dr.qty *(harga*disc/100)),0) " & _
        '     " from trretur r join trreturdetail dr on dr.kdretur=r.kdretur  where AfterPaid =1 and r.KdFaktur= fk.KdFaktur " & _
        '     " and (DATE_FORMAT(TanggalRetur,'%Y-%m-%d') >='" & tg1 & "' and DATE_FORMAT(TanggalRetur,'%Y-%m-%d') <='" & tg1 & "')" & _
        '     " ),0) 'Jumlah Komisi' "
        'queryFrom = " from trsalesorder so  " & _
        '" join mssales sls on sls.kdsales=so.kdsales    " & _
        '" join trfaktur fk on fk.kdso=so.kdso   " & _
        '" join mstoko mt on mt.kdtoko = so.KdToko   " & _
        '" where statusfaktur <> 0 And StatusPayment = 1 and (DATE_FORMAT(TanggalFaktur,'%Y-%m-%d') >='" & tg1 & "' and DATE_FORMAT(TanggalFaktur,'%Y-%m-%d') <='" & tg2 & "')  " & _
        '" and NamaSales = '" & Trim(cmbSales.Text) & "'"
        '20Nov
        'sql = " select so.kdso `No SO`,fk.kdfaktur `No Faktur`,DATE_FORMAT(TanggalFaktur,'%d/%m/%Y') `Tgl Faktur`,NamaToko `Toko`,NamaSales `Nama Sales` " & _
        '       ", Format(TotalKomisiSales - ifnull((select ifnull(sum(dr.qty *(harga*disc/100)),0) " & _
        '       " from trretur r join trreturdetail dr on dr.kdretur=r.kdretur  where AfterPaid =1  " & _
        '       " and (DATE_FORMAT(TanggalRetur,'%Y-%m-%d') >='" & tg1 & "' and DATE_FORMAT(TanggalRetur,'%Y-%m-%d') <='" & tg1 & "')" & _
        '       " ),0),0) 'Jumlah Komisi' "
        'sql2 = " select so.kdso `No SO`,fk.kdfaktur `No Faktur`,DATE_FORMAT(TanggalFaktur,'%d/%m/%Y') `Tgl Faktur`,NamaToko `Toko`,NamaSales `Nama Sales` " & _
        '     ",TotalKomisiSales - ifnull((select ifnull(sum(dr.qty *(harga*disc/100)),0) " & _
        '     " from trretur r join trreturdetail dr on dr.kdretur=r.kdretur  where AfterPaid =1  " & _
        '     " and (DATE_FORMAT(TanggalRetur,'%Y-%m-%d') >='" & tg1 & "' and DATE_FORMAT(TanggalRetur,'%Y-%m-%d') <='" & tg1 & "')" & _
        '     " ),0) 'Jumlah Komisi' "
        'queryFrom = " from trsalesorder so  " & _
        '" join mssales sls on sls.kdsales=so.kdsales    " & _
        '" join trfaktur fk on fk.kdso=so.kdso   " & _
        '" join mstoko mt on mt.kdtoko = so.KdToko   " & _
        '" join trsalespayment sp on sp.KdFaktur = fk.KdFaktur " & _
        '" where statusfaktur <> 0 And StatusPayment = 1 and (DATE_FORMAT(TanggalSalesPayment,'%Y-%m-%d') >='" & tg1 & "' and DATE_FORMAT(TanggalSalesPayment,'%Y-%m-%d') <='" & tg2 & "')  " & _
        '" and NamaSales = '" & Trim(cmbSales.Text) & "'"


        sql = " select so.kdso `No SO`,fk.kdfaktur `No Faktur`,DATE_FORMAT(TanggalFaktur,'%d/%m/%Y') `Tgl Faktur`,NamaToko `Toko`,NamaSales `Nama Sales`, " & _
              " mb.KdBarang,NamaBarang,Merk,Kategori,SubKategori,fd.Qty," & _
              " Format(fd.HargaDisc,0) `Harga`,Format(fd.KomisiSales,0) `Jumlah Komisi`"
        sql2 = " select so.kdso `No SO`,fk.kdfaktur `No Faktur`,DATE_FORMAT(TanggalFaktur,'%d/%m/%Y') `Tgl Faktur`,NamaToko `Toko`,NamaSales `Nama Sales`, " & _
              " mb.KdBarang,NamaBarang,Merk,Kategori,SubKategori,fd.Qty," & _
              " fd.HargaDisc `Harga`,fd.KomisiSales `Jumlah Komisi`"
        'sql2 = " select so.kdso `No SO`,fk.kdfaktur `No Faktur`,DATE_FORMAT(TanggalFaktur,'%d/%m/%Y') `Tgl Faktur`,NamaToko `Toko`,NamaSales `Nama Sales` " & _
        '     ",TotalKomisiSales - ifnull((select ifnull(sum(dr.qty *(harga*disc/100)),0) " & _
        '     " from trretur r join trreturdetail dr on dr.kdretur=r.kdretur  where AfterPaid =1  " & _
        '     " and (DATE_FORMAT(TanggalRetur,'%Y-%m-%d') >='" & tg1 & "' and DATE_FORMAT(TanggalRetur,'%Y-%m-%d') <='" & tg1 & "')" & _
        '     " ),0) 'Jumlah Komisi' "

        queryFrom = " from trsalesorder so  " & _
        " join mssales sls on sls.kdsales=so.kdsales    " & _
        " join trfaktur fk on fk.kdso=so.kdso " & _
        " join trfakturdetail fd on fd.kdfaktur = fk.kdfaktur " & _
        " join msbarang mb on mb.kdbarang=fd.kdbarang " & _
        " Join MsMerk On MsMerk.kdMerk = MB.kdMerk " & _
        " Join Mskategori mk On Mk.KdKategori = Mb.KdKategori " & _
        " Join MsSubkategori On MsSubkategori.KdSub = MB.KdSub " & _
        " join mstoko mt on mt.kdtoko = so.KdToko " & _
        " join trsalespayment sp on sp.kdfaktur = fk.kdfaktur " & _
        " where statusfaktur <> 0 And StatusPayment = 1 and (DATE_FORMAT(TanggalSalesPayment,'%Y-%m-%d') >='" & tg1 & "' and DATE_FORMAT(TanggalSalesPayment,'%Y-%m-%d') <='" & tg2 & "')  " & _
        " and NamaSales = '" & Trim(cmbSales.Text) & "'"

        sql += queryFrom
        sql2 += queryFrom
        ' TextBox1.Text = sql2
        DataGridView1.DataSource = execute_datatable(sql)
        TextBox1.Text = sql2
        Dim totalValue As Double = 0
        Dim reader = execute_reader(sql)
        Do While reader.Read
            totalValue += reader("Jumlah Komisi")
        Loop
        reader.Close()
        lblTotal.Text = FormatNumber(totalValue, 0)

        Try
            tglMulai = tg1
            tglAkhir = tg2
            dropviewM("viewCetakKomisiSales")
            createviewM(sql2, "viewCetakKomisiSales")
        Catch
            MsgBox("Gagal query", MsgBoxStyle.Critical)
        End Try

    End Sub

    Private Sub FormKomisiSales_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        txtTgl.Value = Convert.ToDateTime("01/" & Today.Month & "/" & Today.Year)
        txtTgl2.Value = Convert.ToDateTime(Today.Date)
        setCmbSales()
        initGrid()
        setGrid()
    End Sub

    Private Sub btnCari_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnCari.Click
        initGrid()
    End Sub

    Public Sub setCmbSales()
        Dim reader = execute_reader("Select * from MsSales  order by NamaSales asc")
        Dim varT As String = ""
        cmbSales.Items.Clear()
        'cmbSales.Items.Add("- Pilih Sales -")
        Do While reader.Read
            cmbSales.Items.Add(reader(1).ToString.ToUpper) ' & "-" & reader(0))
        Loop
        reader.Close()
        If cmbSales.Items.Count > 0 Then
            cmbSales.SelectedIndex = 0
        End If
        reader.Close()
    End Sub

    Private Sub setGrid()
        With DataGridView1.ColumnHeadersDefaultCellStyle
            .Alignment = DataGridViewContentAlignment.MiddleCenter
            .Font = New Font(.Font.FontFamily, .Font.Size, _
              .Font.Style Or FontStyle.Bold, GraphicsUnit.Point)
            .ForeColor = Color.Gold
        End With
        DataGridView1.Columns(0).Width = 120
        DataGridView1.Columns(1).Width = 120
        DataGridView1.Columns(3).Width = 150
        DataGridView1.Columns(4).Width = 150
        DataGridView1.Columns(5).Width = 150

    End Sub

  
    Private Sub Button3_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button3.Click
        flagLaporan = "lap_komisi_sales"
        open_subpage("CRLaporan")
    End Sub

    Private Sub DataGridView1_CellDoubleClick(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewCellEventArgs) Handles DataGridView1.CellDoubleClick
        Dim selected_faktur = DataGridView1.CurrentRow.Cells(1).Value
        open_subpage("FormFakturManagamen", selected_faktur)
    End Sub
End Class