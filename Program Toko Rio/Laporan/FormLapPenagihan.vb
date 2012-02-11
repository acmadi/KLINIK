Imports System.Data.SqlClient

Public Class FormLapPenagihan
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
        Dim queryReport As String = ""
        Dim queryForm As String = ""
        query = ""
        '17des
        '" left join(" & _
        '    " select ho.kdfaktur,ho.kdretur,  IFNULL(sum(qty * (harga - (harga*disc/100))),0)  `Total Retur` " & _
        '    " from trretur ho join trreturdetail do  on ho.Kdretur=do.KDRetur  " & _
        '    " where StatusRetur <> 0 " & _
        '    " group by ho.kdfaktur,ho.kdretur " & _
        '    ")as ret on ret.kdfaktur = ho.kdfaktur" & _
        If RadioButton1.Checked = False And RadioButton2.Checked = True Then 'by nama toko
            queryForm = " select ho.KdFaktur `No Faktur`,TanggalFaktur `Tgl Faktur`,FORMAT(sum(qty * (harga - (harga*disc/100))) -   ifnull(0,0),0) `Grand Total`  "
            queryReport = " select ho.KdFaktur `No Faktur`,TanggalFaktur `Tgl Faktur`,sum(qty * (harga - (harga*disc/100)))- ifnull(0,0) `Grand Total`  "
            query += "  from trfaktur ho join trfakturdetail do  on ho.KdFaktur=do.KDFaktur join Msbarang mp on mp.KDBarang=do.KDBarang" & _
            "  join mstoko c on c.kdtoko = ho.kdtoko  " & _
            "  where  DATE_FORMAT(TanggalFaktur,'%Y-%m-%d') >='" & tg1 & "' and DATE_FORMAT(TanggalFaktur,'%Y-%m-%d') <='" & tg2 & "'" & _
            "  AND StatusFaktur <> 1 and  NamaToko    like  '%" & s2 & "%'" & _
            "  group by ho.KdFaktur, TanggalFaktur"
        ElseIf RadioButton2.Checked = False And RadioButton1.Checked = True Then 'by area
            '17des
            ' " left join(" & _
            '" select ho.kdfaktur,ho.kdretur,  IFNULL(sum(qty * (harga - (harga*disc/100))),0)  `Total Retur` " & _
            '" from trretur ho join trreturdetail do  on ho.Kdretur=do.KDRetur  " & _
            '" where StatusRetur <> 0 " & _
            '" group by ho.kdfaktur,ho.kdretur " & _
            '")as ret on ret.kdfaktur = ho.kdfaktur " & _
            queryForm = " select NamaToko,ho.KdFaktur `No Faktur`,TanggalFaktur `Tgl Faktur`,FORMAT(sum(qty * (harga - (harga*disc/100)))- ifnull(0,0),0) `Grand Total` "
            queryReport = " select NamaToko,ho.KdFaktur `No Faktur`,TanggalFaktur `Tgl Faktur`,sum(qty * (harga - (harga*disc/100))) - ifnull(0,0) `Grand Total` "
            query += "  from trfaktur ho join trfakturdetail do  on ho.KdFaktur=do.KDFaktur join Msbarang mp on mp.KDBarang=do.KDBarang" & _
            "  join mstoko c on c.kdtoko = ho.kdtoko  join msarea a on a.kdarea = c.kdarea  " & _
            "  where  DATE_FORMAT(TanggalFaktur,'%Y-%m-%d') >='" & tg1 & "' and DATE_FORMAT(TanggalFaktur ,'%Y-%m-%d')<='" & tg2 & "'" & _
            "  AND StatusFaktur <> 1 and  NamaArea    like  '%" & s & "%'" & _
            "  group by NamaToko,ho.KdFaktur, TanggalFaktur"

        End If
        queryReport += query
        queryForm += query

        TextBox1.Text = queryReport
        Dim totalJual As Double = 0
        Dim jumlahHasil As Double = 0
        Dim totalJualHarga As Double = 0
        'Try
        Try
            tglMulai = tg1
            tglAkhir = tg2

            If RadioButton1.Checked = True Then
                areaReport = cmbArea.Text
                jenisReport = "area"
                dropviewM("viewCetakLapPenagihanUS11010001") ' & kdKaryawan)
                createviewM(queryReport, "viewCetakLapPenagihanUS11010001") ' & kdKaryawan)
            ElseIf RadioButton2.Checked = True Then
                areaReport = cmbCari2.Text
                jenisReport = "toko"
                dropviewM("viewCetakLapPenagihanTokoUS11010001") ' & kdKaryawan)
                createviewM(queryReport, "viewCetakLapPenagihanTokoUS11010001") ' & kdKaryawan)
            End If
            
            DataGridView1.DataSource = execute_datatable(queryForm)
            jumlahHasil = DataGridView1.RowCount
            If jumlahHasil = 0 Then
                MsgBox("Tidak ada data ", MsgBoxStyle.Information)
            End If

            Dim totalValue As Double = 0
            Dim reader2 = execute_reader(queryForm)
            Do While reader2.Read
                totalValue += reader2("Grand Total")
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

    Private Sub setCmbCari2()
        cmbCari2.Items.Clear()
        cmbCari2.Items.Clear()
        cmbCari2.Items.Add("- Pilih Toko -")
        Dim reader = execute_reader("Select * from mstoko order by namaToko   asc")
        Do While reader.Read
            cmbCari2.Items.Add(reader(1).ToString.ToUpper)
        Loop
        reader.Close()
        If cmbCari2.Items.Count > 0 Then cmbCari2.SelectedIndex = 1
    End Sub
    Private Sub FormLapPenagihan_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
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
    End Sub


    Private Sub Button9_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button9.Click
        initGrid("", "")
        cmbArea.SelectedIndex = 0
        cmbCari2.SelectedIndex = 0
    End Sub

    Private Sub Button3_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button3.Click
        flagLaporan = "lap_penagihan"
        open_subpage("CRLaporan")
        'CRLaporan.Show()
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

    Private Sub GroupBox1_Enter(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles GroupBox1.Enter

    End Sub
End Class