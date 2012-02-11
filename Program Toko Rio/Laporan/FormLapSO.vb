Imports System.Data.SqlClient

Public Class FormLapSO
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
        Dim queryFrom As String = ""
        Dim query2 As String = ""
        query = " select DATE_FORMAT(TanggalSO,'%d-%m-%Y') `Tgl SO`,NamaToko,mp.KdBarang `Part No.`,NamaBarang,Merk,Subkategori " & _
        ", Qty,FORMAT(Harga,0) Harga,Disc, format( sum( qty*(harga-(harga*disc/100))),0)  `Total`  "
        query2 = " select DATE_FORMAT(TanggalSO,'%d-%m-%Y') `Tgl SO`,NamaToko,mp.KdBarang `Part No.`,NamaBarang,Merk,Subkategori " & _
                ", Qty,FORMAT(Harga,0) Harga,Disc, sum( qty*(harga-(harga*disc/100)))  `Total`  "
        queryFrom += "  from    " & _
        "  trsalesorder hr join mstoko c on c.kdtoko = hr.kdtoko join trsalesorderdetail dr on dr.KdSO=hr.KdSO join Msbarang mp on mp.KDBarang=dr.KDBarang  " & _
        "  Join MsMerk On MsMerk.kdMerk = mp.kdMerk Join MsSubkategori On MsSubkategori.KdSub = mp.KdSub "
        queryFrom += "  where  DATE_FORMAT(TanggalSO,'%Y-%m-%d') >='" & tg1 & "' and DATE_FORMAT(TanggalSO,'%Y-%m-%d') <='" & tg2 & "'"
        queryFrom += "  and StatusSO <> 0 "
        queryFrom += "  group by `Tgl SO`,NamaToko,dr.KdBarang,NamaBarang,Merk,Subkategori,Qty,Harga,Disc,harga*disc/100 "

        query += queryFrom
        query2 += queryFrom

        TextBox1.Text = query
        Dim totalJual As Double = 0
        Dim jumlahHasil As Double = 0
        Dim totalJualHarga As Double = 0
        'Try
        Try
            tglMulai = tg1
            tglAkhir = tg2
            dropview("viewCetakLapSOUS11010001") ' & kdKaryawan)
            createview(query2, "viewCetakLapSOUS11010001") ' & kdKaryawan)
            DataGridView1.DataSource = execute_datatable(query)
            jumlahHasil = DataGridView1.RowCount
            If jumlahHasil = 0 Then
                MsgBox("Tidak ada data ", MsgBoxStyle.Information)
            End If

            Dim totalValue As Double = 0
            Dim reader2 = execute_reader(query)
            Do While reader2.Read
                totalValue += reader2("Total")
            Loop
            reader2.Close()
            lblTotal.Text = FormatNumber(totalValue, 0)
            Button3.Enabled = True

        Catch
            MsgBox("Gagal query", MsgBoxStyle.Critical)
        End Try
    End Sub
    Private Sub FormLapSO_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Button3.Enabled = False
        txtTgl.Value = Convert.ToDateTime("01/" & Today.Month & "/" & Today.Year)
        txtTgl2.Value = Convert.ToDateTime(Today.Date)
    End Sub

    Private Sub Button9_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button9.Click
        initGrid("", "")
    End Sub

    Private Sub Button3_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button3.Click
        flagLaporan = "lap_so"
        open_subpage("CRLaporan")
    End Sub

    Private Sub Button2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button2.Click
        initGrid("", "")
    End Sub
End Class