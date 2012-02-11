Imports System.Data.SqlClient

Public Class FormTrTransferStock
    Dim query As String = ""
    Dim PK As String = ""
    Dim tabelH As String = ""
    Dim tabelD As String = ""
    Dim tab As String
    Dim SubKategoriID(2) As String
    Dim barangID(2) As String
    Dim SubKategoriID2(2) As String
    Dim barangID2(2) As String
    Dim is_access As Integer

    Private Sub msgWarning(ByVal str As String)
        MsgBox(str, MsgBoxStyle.Critical, "Warning")
    End Sub
    Private Sub msgInfo(ByVal str As String)
        MsgBox(str, MsgBoxStyle.Information, "Information")
    End Sub

    Public Sub setComboDari()
        'cmbDari.Items.Clear()
        'Dim reader = execute_reader("Select kdbarang from msbarang order by kdbarang asc")
        'Do While reader.Read
        '    cmbDari.Items.Add(reader(0))
        'Loop
        'reader.Close()
        'If cmbDari.Items.Count > 0 Then cmbDari.SelectedIndex = 0
    End Sub

    'Public Sub setComboKe()
    '    cmbKe.Items.Clear()
    '    Dim reader = execute_reader("Select kdbarang from msbarang order by kdbarang asc")
    '    Do While reader.Read
    '        cmbKe.Items.Add(reader(0))
    '    Loop
    '    reader.Close()
    '    If cmbKe.Items.Count > 0 Then cmbKe.SelectedIndex = 0
    'End Sub

    Private Sub setCode() 'As String
        Dim code As String = "TF"
        Dim angka As Integer
        Dim kode As String = ""
        Dim temp As String
        Dim bulan As Integer = CInt(Today.Month.ToString)
        code += Today.Year.ToString.Substring(2, 2)
        If bulan < 10 Then
            code += "0" + bulan.ToString
        Else
            code += bulan.ToString
        End If

        'generate code
        query = " select KdTransfer from  TrTransferStok order by KdTransfer  desc"
        Dim reader = execute_reader(query)
        If reader.Read() Then
            kode = reader(0)
        Else
            reader.Close()
            query = " select KdTransfer from  TrTransferStok_T  order by KdTransfer  desc limit 1"
            Dim reader2 = execute_reader(query)
            If reader2.Read() Then
                kode = reader2(0)
            Else
                kode = ""
            End If
            reader2.Close()
        End If
        reader.Close()
        reader = Nothing

        If kode <> "" Then
            temp = kode.Substring(0, 6)
            If temp = code Then
                angka = CInt(kode.Substring(6, 4))
            Else
                angka = 0
            End If

        Else
            angka = 0
        End If


        angka = angka + 1
        Dim len As Integer = angka.ToString().Length
        For i As Integer = 1 To 4 - len
            code += "0"
        Next
        code = code & (angka)   
        txtID.Text = code
        'Return code
    End Sub
    'Public Sub setCmbSubKategori()
    '    Dim reader = execute_reader("Select * from MsSubkategori  where SubKategori <>'' order by SubKategori asc")
    '    Dim varT As String = ""
    '    cmbSubKategori.Items.Clear()
    '    ' cmbSubKategori.Items.Add("- Pilih Sub Kategori -")
    '    Do While reader.Read
    '        cmbSubKategori.Items.Add(reader(0) & " ~ " & reader(1))
    '    Loop
    '    reader.Close()
    '    If cmbSubKategori.Items.Count > 0 Then
    '        cmbSubKategori.SelectedIndex = 0
    '    End If
    '    reader.Close()
    'End Sub

    'Public Sub setCmbSubKategori2()
    '    Dim reader = execute_reader("Select * from MsSubkategori  where SubKategori <>'' order by SubKategori asc")
    '    Dim varT As String = ""
    '    cmbSubKategori2.Items.Clear()
    '    'cmbSubKategori2.Items.Add("- Pilih Sub Kategori -")
    '    Do While reader.Read
    '        cmbSubKategori2.Items.Add(reader(0) & " ~ " & reader(1))
    '    Loop
    '    reader.Close()
    '    If cmbSubKategori2.Items.Count > 0 Then
    '        cmbSubKategori2.SelectedIndex = 0
    '    End If
    '    reader.Close()
    'End Sub
    Private Sub add()
        'setCmbSubKategori()
        'setCmbSubKategori2()
        ' setComboDari()

        tab = "TrTransferStok"
        txtQty.Text = ""
        setCode()
        fillbarang()
    End Sub

    Function fillbarang()
        cmbNamaBarang.Items.Clear()
        cmbNamaBarang2.Items.Clear()
        Dim reader = execute_reader("Select kdbarang,namabarang from msbarang order by namabarang asc")
        Do While reader.Read
            cmbNamaBarang.Items.Add(reader(0) & " ~ " & reader(1))
            cmbNamaBarang2.Items.Add(reader(0) & " ~ " & reader(1))
        Loop
        reader.Close()
        If cmbNamaBarang.Items.Count > 0 Then cmbNamaBarang.SelectedIndex = 0
        If cmbNamaBarang2.Items.Count > 0 Then cmbNamaBarang2.SelectedIndex = 0
        Return True
    End Function

    Private Sub FormTrTransferStock_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        is_access = get_access("trans")
        If is_access = 2 Then
            btnAdd.Enabled = True
            btnTransfer.Enabled = True
        End If
        add()
    End Sub

    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnTransfer.Click
        If cmbNamaBarang.Text = "" Then
            msgInfo("Nama barang harus dipilih")
            cmbNamaBarang.Focus()
        ElseIf cmbDari.Text = "" Then
            msgInfo("Part No asal harus dipilih")
            cmbDari.Focus()
        ElseIf cmbNamaBarang2.Text = "" Then
            msgInfo("Nama barang harus dipilih")
            cmbNamaBarang2.Focus()
        ElseIf cmbKe.Text = "" Then
            msgInfo("Part No tujuan harus dipilih")
            cmbKe.Focus()
        ElseIf cmbDari.Text = cmbKe.Text Then
            msgInfo("Part No tidak boleh sama")
        ElseIf txtQty.Text = "" Then
            msgInfo("Quantity harus diisi")
            txtQty.Focus()
        ElseIf Val(txtQty.Text) > Val(lblStock.Text) Then
            msgInfo("Jumlah yang ditransfer tidak boleh melebihi stok yang ada")
            txtQty.Text = ""
            txtQty.Focus()
        ElseIf txtHarga.Text = "" Then
            msgInfo("Harga harus diisi")
            txtQty.Focus()
        Else
            dbconmanual.Open()
            Dim trans As MySql.Data.MySqlClient.MySqlTransaction

            trans = dbconmanual.BeginTransaction(IsolationLevel.ReadCommitted)
            Try
                If MsgBox("Anda yakin ?", MsgBoxStyle.Question + MsgBoxStyle.YesNo, "Confirmation") = MsgBoxResult.Yes Then
                    'insert historybarang
                    Dim Qty = Trim(txtQty.Text)
                    Dim OP = "Min"
                    Dim Attribute = "QtyUpdate_Min"
                    Dim KdBarang = cmbDari.Text
                    StockBarang(txtQty.Text, OP, 0, KdBarang, Attribute, Trim(txtID.Text), "Form Transfer Stok")
                    'BarangHistory(Val(Qty), OP, Trim(Attribute), KdBarang, 0, 0, Trim(txtID.Text), "Form Transfer Stok")

                    Qty = Trim(txtQty.Text)
                    OP = "Plus"
                    Attribute = "QtyUpdate_Plus"
                    KdBarang = cmbKe.Text
                    StockBarang(txtQty.Text, OP, Trim(txtHarga.Text), KdBarang, Attribute, Trim(txtID.Text), "Form Transfer Stok")
                    'BarangHistory(Val(Qty), OP, Trim(Attribute), KdBarang, 0, 0, Trim(txtID.Text), "Form Transfer Stok")

                    sql = "insert into  " & tab & " values('" & txtID.Text & "',now(),'" & Trim(cmbDari.Text) & "','" & Trim(cmbKe.Text) & "'," & Trim(txtQty.Text) & ")"
                    ' TextBox1.Text = sql
                    execute_update_manual(sql)
                    msgInfo("Transfer stock berhasil")

                    sql = "delete from  " & tab & "_T  where kdtransfer= '" & txtID.Text & "'"
                    execute_update_manual(sql)

                    trans.Commit()
                    add()
                End If
            Catch ex As Exception
                trans.Rollback()
                MsgBox(ex, MsgBoxStyle.Information)
            End Try
            dbconmanual.Close()
        End If
    End Sub

    Private Sub txtQty_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txtQty.KeyPress
        If AscW(e.KeyChar) = 13 Then
            Button1_Click(Nothing, Nothing)
        End If
        If AscW(e.KeyChar) <> 8 And (Asc(e.KeyChar) < Asc("0") Or Asc(e.KeyChar) > Asc("9")) Then
            e.KeyChar = Nothing
        End If
    End Sub
 
    

    Private Sub btnAdd_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnAdd.Click
        add()
    End Sub
 

    Private Sub cmbSubKategori_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs)
        'SubKategoriID = cmbSubKategori.Text.ToString.Split(" ~ ")
        'cmbNamaBarang.Items.Clear()
        'Dim reader = execute_reader("Select kdbarang,namabarang from msbarang where kdsub='" & SubKategoriID(0) & "' order by namabarang asc")
        'Do While reader.Read
        '    cmbNamaBarang.Items.Add(reader(0) & " ~ " & reader(1))
        'Loop
        'reader.Close()
        'If cmbNamaBarang.Items.Count > 0 Then cmbNamaBarang.SelectedIndex = 0
    End Sub

    Private Sub cmbNamaBarang_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmbNamaBarang.SelectedIndexChanged
        barangID = cmbNamaBarang.Text.ToString.Split(" ~ ")
        cmbDari.Text = barangID(0)

        sql = " Select StockAkhir from BarangHistory where isActive = 1 And kdBarang = '" & cmbDari.Text & "' order by KdBarangHistory desc limit 1"
        Dim reader = execute_reader(sql)
        If reader.Read Then
            lblStock.Text = reader(0)
        End If
        reader.Close()
        reader = Nothing
        sql = " Select Harga, SubKategori, MsBarang.KdSub From MsBarangList " & _
              " JOIN MsBarang ON MsBarang.KdBarang = MsBarangList.KdBarang " & _
              " JOIN mssubkategori ON MsBarang.KdSub = mssubkategori.KdSub " & _
              " where MsBarangList.KdBarang = '" & cmbDari.Text & "' " & _
              " And StatusBarangList = 0 " & _
              " Order By KdList asc Limit 1 "
        reader = execute_reader(sql)
        If reader.Read Then
            lblHarga.Text = CDbl(reader(0))
        End If
        reader.Close()
        txtHarga.Text = lblHarga.Text
    End Sub

  

    Private Sub cmbSubKategori2_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs)
        'SubKategoriID2 = cmbSubKategori2.Text.ToString.Split(" ~ ")
        'cmbNamaBarang2.Items.Clear()
        'Dim reader = execute_reader("Select kdbarang,namabarang from msbarang where kdsub='" & SubKategoriID2(0) & "' order by namabarang asc")
        'Do While reader.Read
        '    cmbNamaBarang2.Items.Add(reader(0) & " ~ " & reader(1))
        'Loop
        'reader.Close()
        'If cmbNamaBarang2.Items.Count > 0 Then cmbNamaBarang2.SelectedIndex = 0
    End Sub

    Private Sub cmbNamaBarang2_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmbNamaBarang2.SelectedIndexChanged
        barangID2 = cmbNamaBarang2.Text.ToString.Split(" ~ ")
        cmbKe.Text = barangID2(0)

        sql = " Select StockAkhir from BarangHistory where isActive = 1 And kdBarang = '" & cmbKe.Text & "' order by KdBarangHistory desc limit 1"
        Dim reader = execute_reader(sql)
        If reader.Read Then
            lblStock2.Text = reader(0)
        End If
        reader.Close()

        reader = Nothing
        sql = " Select Harga, SubKategori, MsBarang.KdSub From MsBarangList " & _
              " JOIN MsBarang ON MsBarang.KdBarang = MsBarangList.KdBarang " & _
              " JOIN mssubkategori ON MsBarang.KdSub = mssubkategori.KdSub " & _
              " where MsBarangList.KdBarang = '" & cmbKe.Text & "' " & _
              " And StatusBarangList = 0 " & _
              " Order By KdList asc Limit 1 "
        reader = execute_reader(sql)
        If reader.Read Then
            lblHarga2.Text = CDbl(reader(0))
        End If
        reader.Close()
    End Sub

    Private Sub txtQty_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles txtQty.TextChanged

    End Sub

    Private Sub txtHarga_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txtHarga.KeyPress
        If AscW(e.KeyChar) = 13 Then
            Button1_Click(Nothing, Nothing)
        End If
        If AscW(e.KeyChar) <> 8 And (Asc(e.KeyChar) < Asc("0") Or Asc(e.KeyChar) > Asc("9")) Then
            e.KeyChar = Nothing
        End If
    End Sub
 
    Private Sub browseBarang_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles browseBarang.Click
        Try
            sub_form = New FormBrowseBarang
            sub_form.showDialog(FormMain)
            If data_carier(0) <> "" Then
                cmbNamaBarang.Text = data_carier(0) & " ~ " & data_carier(1)
            End If
        Catch ex As Exception
            MsgBox(ex.ToString, MsgBoxStyle.Critical, "Warning!!!")
        End Try
    End Sub

    Private Sub BorwserBarang2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BorwserBarang2.Click
        Try
            sub_form = New FormBrowseBarang
            sub_form.showDialog(FormMain)
            If data_carier(0) <> "" Then
                cmbNamaBarang2.Text = data_carier(0) & " ~ " & data_carier(1)
            End If
        Catch ex As Exception
            MsgBox(ex.ToString, MsgBoxStyle.Critical, "Warning!!!")
        End Try
    End Sub
End Class