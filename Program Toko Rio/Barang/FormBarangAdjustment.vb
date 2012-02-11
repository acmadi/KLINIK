Imports System.Data.SqlClient
Public Class FormBarangAdjustment
    Dim status As String
    Dim posisi As Integer = 0
    Dim tab As String
    Dim jumData As Integer = 0
    Dim Stock As Integer = 0
    Dim PK As String = ""
    Dim is_access As Integer

    Private Sub msgWarning(ByVal str As String)
        MsgBox(str, MsgBoxStyle.Critical, "Warning")
    End Sub

    Private Sub msgInfo(ByVal str As String)
        MsgBox(str, MsgBoxStyle.Information, "Information")
    End Sub

    Function visibleHarga(ByVal status As Boolean)
        lblHarga.Visible = status
        txtHarga.Visible = status
        lblIDR.Visible = status
        Return True
    End Function

    Private Sub emptyField()
        txtHarga.Text = ""
        cmbTipeBarang.SelectedIndex = 0
        cmbPenyesuaian.SelectedIndex = 0
        txtQty.Text = ""
        txtHargaList.Text = 0
    End Sub

    Private Sub ubahAktif(ByVal status As Boolean)
        txtID.Enabled = False
        txtNote.Enabled = status
        cmbTipeBarang.Enabled = status

        btnSave.Enabled = status
        btnCancel.Enabled = status

        btnExit.Enabled = Not status
        GroupBox1.Enabled = Not status

        btnSave.Enabled = status
        btnCancel.Enabled = status
        'btnAdd.Enabled = Not status
        DataGridView1.Enabled = Not status
    End Sub

    Private Sub setData()

        If jumData > 0 Then
            Try
                txtID.Text = DataGridView1.CurrentRow.Cells("Kode").Value.ToString
                cmbBarang.Text = DataGridView1.CurrentRow.Cells("No. Part").Value.ToString & " ~ " & DataGridView1.CurrentRow.Cells("Nama Barang").Value.ToString
                txtQty.Text = DataGridView1.CurrentRow.Cells("Jumlah").Value.ToString
                cmbPenyesuaian.Text = DataGridView1.CurrentRow.Cells("Penyesuaian").Value.ToString
                txtHarga.Text = DataGridView1.CurrentRow.Cells("Harga Modal").Value.ToString
                txtHargaList.Text = DataGridView1.CurrentRow.Cells("Harga List").Value.ToString
            Catch ex As Exception
            End Try
        End If
    End Sub

    Private Sub setCmbCari()
        cmbCari.Items.Clear()
        cmbCari.Items.Add("Tanggal")
        cmbCari.Items.Add("Penyesuaian")
        cmbCari.Items.Add("Harga Modal")
        cmbCari.Items.Add("Harga List")
        cmbCari.Items.Add("Jumlah")
        cmbCari.Items.Add("Note")
        cmbCari.SelectedIndex = 0
    End Sub

    Private Sub viewAllData(ByVal cr As String, ByVal opt As String)
        If is_access = 2 Or is_access = 4 Then
            btnAdd.Enabled = True
        ElseIf is_access = 5 Then
            btnAdd.Enabled = True
        End If
        sql = " select KdAdj Kode, DATE_FORMAT(TanggalAdj,'%d %M %Y') Tanggal,Type Penyesuaian, Adj.KdBarang 'No. Part', " & _
              " ifnull(ifnull(NamaBarang,NamaBahanMentah),TypeFising) 'Nama Barang', Harga 'Harga Modal', " & _
              " Adj.HargaList 'Harga List', adj.Qty Jumlah,Adj.Note " & _
              " from  " & tab & " Adj " & _
              " Left Join MsBarang On MsBarang.KdBarang = Adj.KdBarang " & _
              " Left Join MsBahanMentah On MsBahanMentah.KdBahanMentah = Adj.KdBarang " & _
              " Left Join msfising On msfising.KdFising = Adj.KdBarang where 1 "
        sql &= QueryLevel(lvlKaryawan, "Adj.")
        If opt <> "" Then
            Dim col As String = ""
            If opt = "Tanggal" Then
                col = "TanggalAdj"
            ElseIf opt = "Penyesuaian" Then
                col = "Type"
            ElseIf opt = "Harga Modal" Then
                col = "Harga"
            ElseIf opt = "Harga List" Then
                col = "Adj.HargaList"
            ElseIf opt = "Jumlah" Then
                col = "Qty"
            ElseIf opt = "Note" Then
                col = "Note"
            End If
            sql &= "  And " & col & "  like '%" & cr & "%'  and KdAdj <>'AJ00000000'"
        Else
            sql &= "  And KdAdj <>'AJ00000000'"
        End If

        DataGridView1.DataSource = execute_datatable(sql)
        jumData = DataGridView1.RowCount
        If jumData > 0 Then
            posisi = jumData
            setData()
        End If
        txtQty.Enabled = False
        cmbPenyesuaian.Enabled = False
        visibleHarga(False)
    End Sub

    Public Sub setCmbBarangJadi()
        Dim reader = execute_reader(" Select KdBarang,NamaBarang from MsBarang where 1 " & _
                                    QueryLevel(lvlKaryawan) & _
                                    " And NamaBarang <>'' order by NamaBarang asc")
        Dim varT As String = ""
        cmbBarang.Items.Clear()
        cmbBarang.Items.Add("- Pilih Barang -")
        Do While reader.Read
            cmbBarang.Items.Add(reader(0) & " ~ " & reader(1))
        Loop
        reader.Close()
        If cmbBarang.Items.Count > 0 Then
            cmbBarang.SelectedIndex = 0
        End If
        reader.Close()
    End Sub

    Public Sub setCmbBahanMentah()
        Dim reader = execute_reader(" Select KdBahanMentah,NamaBahanMentah from MsBahanMentah where 1 " & _
                                    QueryLevel(lvlKaryawan) & _
                                    " And NamaBahanMentah <>'' order by NamaBahanMentah asc")
        Dim varT As String = ""
        cmbBarang.Items.Clear()
        cmbBarang.Items.Add("- Pilih Barang -")
        Do While reader.Read
            cmbBarang.Items.Add(reader(0) & " ~ " & reader(1))
        Loop
        reader.Close()
        If cmbBarang.Items.Count > 0 Then
            cmbBarang.SelectedIndex = 0
        End If
        reader.Close()
    End Sub

    Public Sub setCmbFising()
        Dim reader = execute_reader(" Select KdFising,TypeFising from MsFising where 1 " & _
                                    QueryLevel(lvlKaryawan) & _
                                    " And TypeFising <>'' order by TypeFising asc")
        Dim varT As String = ""
        cmbBarang.Items.Clear()
        cmbBarang.Items.Add("- Pilih Fising -")
        Do While reader.Read
            cmbBarang.Items.Add(reader(0) & " ~ " & reader(1))
        Loop
        reader.Close()
        If cmbBarang.Items.Count > 0 Then
            cmbBarang.SelectedIndex = 0
        End If
        reader.Close()
    End Sub

    Private Sub setCmbTipeBarang()
        cmbTipeBarang.Items.Clear()
        cmbTipeBarang.Items.Add("- Pilih -")
        cmbTipeBarang.Items.Add("Barang Jadi")
        cmbTipeBarang.Items.Add("Barang Mentah")
        cmbTipeBarang.Items.Add("Barang Fising")
        cmbTipeBarang.SelectedIndex = 0
    End Sub

    Private Sub FormMsSupplier_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        is_access = get_access("adj")
        tab = " TrAdjusment "
        PK = "  KdAdj "
        setCmbTipeBarang()
        emptyField()
        generateCode()
        cmbTipeBarang.Focus()
        ubahAktif(False)
        viewAllData("", "")
        posisi = 0
        setData()
        setGrid()
        setCmbCari()
    End Sub

    Private Sub setGrid()
        With DataGridView1.ColumnHeadersDefaultCellStyle
            .Alignment = DataGridViewContentAlignment.MiddleCenter
            .Font = New Font(.Font.FontFamily, .Font.Size, _
              .Font.Style Or FontStyle.Bold, GraphicsUnit.Point)
            .ForeColor = Color.Gold

        End With
        DataGridView1.Columns(0).Width = 100
        DataGridView1.Columns(1).Width = 100
        DataGridView1.Columns(2).Width = 100
        DataGridView1.Columns(3).Visible = False
        DataGridView1.Columns(4).Width = 200
        DataGridView1.Columns(5).Width = 100
        DataGridView1.Columns(6).Width = 100
    End Sub

    Function getADJ(Optional ByVal KdADJ As String = "")
        Dim sql As String = "Select * from TrAdjusment Order By KdAdj desc"
        Dim reader = execute_reader(sql)
        Return reader
    End Function

    Private Sub generateCode()
        Dim code As String = "AJ"
        Dim angka As Integer
        Dim kode As String
        Dim temp As String
        'code += Today.Year.ToString.Substring(2, 2)
        Dim bulan As Integer = CInt(Today.Month.ToString)
        Dim currentTime As System.DateTime = System.DateTime.Now
        Dim FormatDate As String = Format(currentTime, "yyMMdd")

        'If bulan < 10 Then
        '    code += "0" + bulan.ToString
        'Else
        '    code += bulan.ToString
        'End If
        code += FormatDate

        Dim reader = getADJ()

        If reader.read Then
            kode = Trim(reader(0).ToString())
            temp = kode.Substring(0, 8)
            If temp = code Then
                angka = CInt(kode.Substring(8, 4))
            Else
                angka = 0
            End If
            reader.Close()
        Else
            angka = 0
            reader.Close()
        End If
        angka = angka + 1
        Dim len As Integer = angka.ToString().Length
        For i As Integer = 1 To 4 - len
            code += "0"
        Next
        code = code & (angka)
        txtID.Text = Trim(code)
    End Sub

    Private Sub btnSave_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSave.Click
        If cmbTipeBarang.SelectedIndex = 0 Then
            msgInfo("Tipebarang harus dipilih")
            cmbTipeBarang.Focus()
        ElseIf cmbBarang.SelectedIndex = 0 Then
            msgInfo("Barang harus dipilih")
            cmbBarang.Focus()
        ElseIf cmbPenyesuaian.SelectedIndex = 0 Then
            msgInfo("Penyesuaian harus dipilih")
            cmbPenyesuaian.Focus()
        ElseIf txtQty.Text = "" Or txtQty.Text = "0" Then
            msgInfo("Jumlah harus diisi dan lebih besar dari 0")
            txtQty.Focus()
        ElseIf cmbPenyesuaian.SelectedIndex = 2 And (txtHarga.Text = "" Or txtHarga.Text = "0") Then
            msgInfo("Harga Modal harus diisi dan lebih besar dari 0")
            txtHarga.Focus()
        Else
            Dim barangID = cmbBarang.Text.ToString.Split("~")
            If txtHarga.Text = "" Then
                txtHarga.Text = 0
            End If
            sql = " insert into  TrAdjusment ( KdAdj, TanggalAdj, KdBarang, TYPE, Harga, " & _
                  " Qty, Note, LevelID, HargaList ) values( " & _
                  " '" + Trim(txtID.Text) + "',now(), " & _
                  " '" & Trim(barangID(0)) & "','" & Trim(cmbPenyesuaian.Text) & "'," & _
                  " '" & Val(Trim(txtHarga.Text)) & "','" & txtQty.Text & "', " & _
                  " '" & txtNote.Text & "','" & lvlKaryawan & "','" & Val(txtHargaList.Text) & "')"

            Dim OP = ""
            Dim Attribute = ""
            If cmbPenyesuaian.Text = "-" Then
                OP = "Min"
                Attribute = "QtyAdj_Min"
            ElseIf cmbPenyesuaian.Text = "+" Then
                OP = "Plus"
                Attribute = "QtyAdj_Plus"
            End If

            dbconmanual.Open()
            Dim trans As MySql.Data.MySqlClient.MySqlTransaction

            trans = dbconmanual.BeginTransaction(IsolationLevel.ReadCommitted)
            Try
                If cmbTipeBarang.SelectedIndex = 1 Then
                    StockBarang(txtQty.Text, OP, txtHarga.Text, Trim(barangID(0)), Attribute, Trim(txtID.Text), "Form Adjusment")
                ElseIf cmbTipeBarang.SelectedIndex = 2 Then
                    StockBahanMentah(txtQty.Text, OP, txtHarga.Text, Trim(barangID(0)), Attribute, Trim(txtID.Text), "Form Adjusment")
                ElseIf cmbTipeBarang.SelectedIndex = 3 Then
                    StockFising(txtQty.Text, OP, txtHarga.Text, Trim(barangID(0)), Attribute, Trim(txtID.Text), "Form Adjusment")
                End If
                execute_update(sql)
                trans.Commit()
                msgInfo("Data berhasil disimpan")
                viewAllData("", "")
                ubahAktif(False)
            Catch ex As Exception
                trans.Rollback()
                msgWarning("Data tidak valid pada Adjusment")
            End Try
            dbconmanual.Close()
        End If
    End Sub

    Private Sub btnExit_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnExit.Click
        Me.Close()
    End Sub

    Private Sub txtHarga_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txtHarga.KeyPress
        If Asc(e.KeyChar) <> Asc("-") And AscW(e.KeyChar) <> 13 And AscW(e.KeyChar) <> 8 And (Asc(e.KeyChar) < Asc("0") Or Asc(e.KeyChar) > Asc("9")) Then
            e.KeyChar = Nothing
        End If
        If AscW(e.KeyChar) = 13 And txtHarga.Text <> "" Then
            txtNote.Focus()
        End If
    End Sub

    Private Sub txtQty_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txtQty.KeyPress
        If Asc(e.KeyChar) <> Asc("-") And AscW(e.KeyChar) <> 13 And AscW(e.KeyChar) <> 8 And (Asc(e.KeyChar) < Asc("0") Or Asc(e.KeyChar) > Asc("9")) Then
            e.KeyChar = Nothing
        End If
        If AscW(e.KeyChar) = 13 And txtQty.Text <> "" Then
            txtHarga.Focus()
        End If
    End Sub


    Private Sub Button2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles browseBarang.Click
        Try
            If cmbTipeBarang.SelectedIndex = 1 Then
                sub_form = New FormBrowseBarang
            ElseIf cmbTipeBarang.SelectedIndex = 2 Then
                sub_form = New FormBrowseBahanMentah
            ElseIf cmbTipeBarang.SelectedIndex = 3 Then
                sub_form = New FormBrowseFising
            End If
            sub_form.showDialog(FormMain)
            If data_carier(0) <> "" Then
                cmbBarang.Text = data_carier(0) & " ~ " & data_carier(1)
                clear_variable_array()
            End If
        Catch ex As Exception
            MsgBox(ex, MsgBoxStyle.Critical, "Warning!!!")
        End Try
    End Sub

    Private Sub txtCari_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles txtCari.TextChanged
        viewAllData(txtCari.Text, cmbCari.Text)
    End Sub

    Private Sub DataGridView1_CellClick(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewCellEventArgs) Handles DataGridView1.CellClick
        If e.RowIndex >= 0 And e.RowIndex <= DataGridView1.Rows.Count - 1 Then
            posisi = e.RowIndex
            setData()
        End If
    End Sub

    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
        txtCari.Text = ""
        viewAllData("", "")
    End Sub

    Private Sub btnAdd_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnAdd.Click
        btnAdd.Enabled = False
        generateCode()
        ubahAktif(True)
        status = "add"
        emptyField()
        cmbTipeBarang.Focus()
    End Sub

    Private Sub btnCancel_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnCancel.Click
        If status = "add" Then
            posisi = 0
        End If
        ubahAktif(False)
        setData()
        viewAllData("", "")
    End Sub

    Private Sub cmbBarang_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles cmbBarang.SelectedIndexChanged
        Dim barangID = cmbBarang.Text.ToString.Split("~")
        If cmbTipeBarang.SelectedIndex = 1 Then
            sql = " SELECT IFNULL(bh.StockAkhir,0), HargaList  " & _
                  " FROM MsBarang mb " & _
                  " LEFT JOIN BarangHistory bh ON mb.KdBarang = bh.KdBarang " & _
                  " WHERE IFNULL(bh.isActive,1) = 1 " & _
                  " And mb.kdBarang = '" & barangID(0) & "' " & _
                  " ORDER BY IFNULL(bh.KdBarangHistory,1) DESC " & _
                  " LIMIT 1 "
        ElseIf cmbTipeBarang.SelectedIndex = 2 Then
            sql = " Select StockAkhir, '0' HargaList " & _
                  " from BahanMentahHistory " & _
                  " where isActive = 1 " & _
                  " And kdBahanMentah = '" & barangID(0) & "' " & _
                  " order by KdHistory desc " & _
                  " limit 1 "
        ElseIf cmbTipeBarang.SelectedIndex = 3 Then
            sql = " Select StockAkhir, '0' HargaList " & _
                  " from fisinghistory " & _
                  " where isActive = 1 " & _
                  " And KdFising = '" & barangID(0) & "' " & _
                  " order by KdFisingHistory desc " & _
                  " limit 1 "
        End If
        Dim reader = execute_reader(sql)

        If reader.Read Then
            Stock = reader(0)
            txtHargaList.Text = reader(1)
            cmbPenyesuaian.Enabled = True
        Else
            Stock = 0
            txtHargaList.Text = 0
            cmbPenyesuaian.Enabled = True
        End If
        reader.Close()
        txtQty.Focus()
    End Sub

    Private Sub cmbPenyesuaian_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles cmbPenyesuaian.SelectedIndexChanged
        If cmbBarang.SelectedIndex <> 0 Then
            txtQty.Enabled = True
            If cmbPenyesuaian.SelectedIndex = 2 Then
                visibleHarga(True)
            Else
                visibleHarga(False)
            End If
        Else
            txtQty.Enabled = False
            visibleHarga(False)
        End If
        txtQty.Focus()
    End Sub

    Private Sub txtNote_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txtNote.KeyPress
        If Asc(e.KeyChar) = Asc("'") And AscW(e.KeyChar) <> 13 And AscW(e.KeyChar) <> 8 Then
            e.KeyChar = Nothing
        End If
    End Sub

    Private Sub txtQty_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles txtQty.TextChanged
        If cmbPenyesuaian.SelectedIndex = 1 And Val(txtQty.Text) > Val(Stock) Then
            msgInfo("Persediaan tidak mencukupi tidak mencukupi. Persediaan saat ini adalah " & Stock)
            txtQty.Text = Stock
        End If
    End Sub

    Private Sub cmbTipeBarang_SelectedIndexChanged_1(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmbTipeBarang.SelectedIndexChanged
        If cmbTipeBarang.SelectedIndex = 0 Then
            'cmbBarang.SelectedIndex = 0
            cmbBarang.Enabled = False
        ElseIf cmbTipeBarang.SelectedIndex = 1 Then
            setCmbBarangJadi()
            cmbBarang.Enabled = True
            browseBarang.Enabled = True
        ElseIf cmbTipeBarang.SelectedIndex = 2 Then
            setCmbBahanMentah()
            cmbBarang.Enabled = True
            browseBarang.Enabled = True
        ElseIf cmbTipeBarang.SelectedIndex = 3 Then
            setCmbFising()
            cmbBarang.Enabled = True
            browseBarang.Enabled = True
        End If
    End Sub
End Class
