Imports System.Data.SqlClient

Public Class FormLapPenagihan2
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
    Dim sqlTableTemp As String = ""
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
            ' TextBox1.Text = query
            execute_update(sql)

        Catch ex As Exception
            MsgBox("gagal")
        End Try
    End Sub

    Private Sub createTableTemp()
        Try
            Dim sql As String
            sql = "CREATE TABLE z_daftartagih (" & _
            "  `TglKirim` varchar(25) default NULL, " & _
            "  `TglJatuhTempo` varchar(25) default NULL," & _
            "  `NoFaktur` varchar(20) default NULL," & _
            "  `NamaToko` varchar(200) default NULL," & _
            "  `Kota` varchar(100) default NULL," & _
            "  `Area` varchar(200) default NULL," & _
            "  `TglFaktur` varchar(25) default NULL," & _
            "  `TotalFaktur` double default NULL," & _
            "  `TotalRetur` double default NULL," & _
            "  `Debet` double default NULL," & _
            " `BG` double default NULL," & _
            " `Transfer` double default NULL," & _
            " `SisaHutang` double default NULL," & _
            "  `Keterangan` varchar(100) default NULL" & _
            ")"

            execute_update(sql)

        Catch ex As Exception
            execute_update("delete from z_daftartagih")
        End Try
    End Sub
    Private Sub deleteTableTemp()
        Try
            Dim sql As String
            sql = "drop table z_daftartagih"
            execute_update(sql)
        Catch ex As Exception

        End Try
    End Sub
    Private Sub insertTabelTemp(ByVal qq As String)


        Try
            Dim sql As String
            sql = "insert into z_daftartagih " & qq
            execute_update(sql)
        Catch ex As Exception
            MsgBox("gagal")
        End Try
    End Sub
    Private Sub initGrid(ByVal s As String, ByVal s2 As String) 'area,toko
        deleteTableTemp()
        createTableTemp()
        tg1 = String.Format("{0:yyyy-MM-dd}", txtTgl.Value)
        tg2 = String.Format("{0:yyyy-MM-dd}", txtTgl2.Value)
        Dim reader As SqlDataReader = Nothing
        Dim queryFrom As String = ""


        query = " select  date_format(TanggalSJ,'%d-%b-%Y') `Tgl Kirim`,date_format(TanggalJT,'%d-%b-%Y') `Tgl Jatuh Tempo`, " & _
        " ho.KdFaktur `No Faktur`,NamaToko,Daerah `Kota`,NamaArea `Area`," & _
        " date_format(TanggalFaktur,'%d-%b-%Y') `Tgl Faktur` ," & _
        " FORMAT(sum(qty * (harga - (harga*disc/100))),0) `Total Faktur`," & _
        " FORMAT(IFNULL(`Total Retur`,0),0) `Total Retur`," & _
        " FORMAT(IFNULL(Debet,0),0) Debet," & _
        " FORMAT(IFNULL(BG,0),0) BG," & _
        " FORMAT(IFNULL(Transfer,0),0) Transfer, " & _
        " FORMAT(( sum(qty * (harga - (harga*disc/100))) - IFNULL(`Total Retur`,0) - IFNULL(Debet,0) - IFNULL(BG,0)),0) AS `Sisa Hutang`," & _
        "       ' ' Keterangan "
        sql = "select  date_format(TanggalSJ,'%d-%b-%Y') `Tgl Kirim`,date_format(TanggalJT,'%d-%b-%Y') `Tgl Jatuh Tempo`, " & _
        " ho.KdFaktur `No Faktur`,NamaToko,Daerah `Kota`,NamaArea `Area`," & _
        " date_format(TanggalFaktur,'%d-%b-%Y') `Tgl Faktur` ," & _
        " sum(qty * (harga - (harga*disc/100))) `Total Faktur`," & _
        " IFNULL(`Total Retur`,0) `Total Retur`," & _
        " IFNULL(Debet,0) Debet," & _
        " IFNULL(BG,0) BG," & _
        " IFNULL(Transfer,0) Transfer, " & _
        " ( sum(qty * (harga - (harga*disc/100))) - IFNULL(`Total Retur`,0) - IFNULL(Debet,0) - IFNULL(BG,0)) AS `Sisa Hutang`," & _
        "       ' ' Keterangan "
        queryFrom = " from trfaktur ho " & _
        " join trfakturdetail do  on ho.KdFaktur=do.KDFaktur " & _
        " join mstoko c on c.kdtoko = ho.kdtoko  " & _
        " join msarea a on a.kdarea = c.kdarea " & _
        " left join (" & _
        "   select ho.kdfaktur,ho.kdretur,  IFNULL(sum(qty * (harga - (harga*disc/100))),0)  `Total Retur`" & _
        "   from trretur ho " & _
        "   join trreturdetail do  on ho.Kdretur=do.KDRetur " & _
        "   where(StatusRetur <> 0)" & _
        "   group by ho.kdfaktur,ho.kdretur" & _
        ") AS ret on ret.kdfaktur = ho.kdfaktur " & _
        " left join (" & _
        "   select KdFaktur, TotalSalesPayment `Debet` " & _
        "   from(trsalespayment) " & _
        "   where PaidBy='Debet' " & _
        " )as byrdebet on byrdebet.kdfaktur =  ho.kdfaktur " & _
        " left join (" & _
        "   select KdFaktur, TotalSalesPayment `BG`" & _
        "   from(trsalespayment)" & _
        "   where PaidBy='BG'" & _
        ")as byrbg on byrbg.kdfaktur =  ho.kdfaktur " & _
        "left join (" & _
        "   select KdFaktur, TotalSalesPayment `Transfer`" & _
        "   from(trsalespayment)" & _
        "   where PaidBy='Transfer'" & _
        ")as byrtrans on byrtrans.kdfaktur =  ho.kdfaktur " & _
        " where  NamaArea    like  '%" & cmbArea.Text & "%'" & _
        " group by  ho.KdFaktur order by NamaToko ASC "
        '" where StatusFaktur = 1 " & _
        query += queryFrom
        sql += queryFrom
        TextBox1.Text = query
        Dim totalJual As Double = 0
        Dim jumlahHasil As Double = 0
        Dim totalJualHarga As Double = 0
        'Try
        Try
            tglMulai = tg1
            tglAkhir = tg2
            insertTabelTemp(sql)
            '  dropview("viewCetakDaftarTagih" & kdKaryawan)
            '  createview(query, "viewCetakDaftarTagih" & kdKaryawan)
            DataGridView1.DataSource = execute_datatable(query)
            jumlahHasil = DataGridView1.RowCount
            If jumlahHasil = 0 Then
                MsgBox("Tidak ada data ", MsgBoxStyle.Information)
            End If

            Dim totalValue As Double = 0
            Dim reader2 = execute_reader(query)
            Do While reader2.Read
                totalValue += reader2("Sisa Hutang")
            Loop
            reader2.Close()
            lblTotal.Text = FormatNumber(totalValue, 0)
            Button3.Enabled = True

        Catch
            MsgBox("Gagal query", MsgBoxStyle.Critical)
        End Try
    End Sub
    Public Sub setCmbArea()
        Dim reader = execute_reader("Select * from MsArea  where NamaArea <>'' order by NamaArea asc")
        Dim varT As String = ""
        cmbArea.Items.Clear()
        cmbArea.Items.Add("- Pilih Area -")
        Do While reader.Read
            cmbArea.Items.Add(reader(1).ToString.ToUpper) ' & "-" & reader(0))
        Loop
        reader.Close()
        If cmbArea.Items.Count > 0 Then
            cmbArea.SelectedIndex = 0
        End If
        reader.Close()
    End Sub

    Private Sub FormLapPenagihan2_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Button3.Enabled = False
        txtTgl.Value = Convert.ToDateTime("01/" & Today.Month & "/" & Today.Year)
        txtTgl2.Value = Convert.ToDateTime(Today.Date)
        setCmbArea()
    End Sub
    Private Sub Button9_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button9.Click
        initGrid("", "")
    End Sub

    Private Sub Button3_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button3.Click
        'flagLaporan = "lap_pb"

        open_subpage("CRDaftarTagih")
    End Sub

    Private Sub Button2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button2.Click
        If cmbArea.Text = "- Pilih Area -" Then
            MsgBox("Area harus dipilih", MsgBoxStyle.Information)
        Else

            initGrid("", "")
        End If
    End Sub
 
    Private Sub GroupBox1_Enter(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles GroupBox1.Enter

    End Sub
End Class