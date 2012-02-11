Imports System.Data.SqlClient

Public Class FormMsFising
    Dim status As String
    Dim posisi As Integer = 0
    Dim tab As String
    Dim jumData As Integer = 0
    Dim PK As String
    Dim OldQty As Double = 0
    Dim StockAwal As Double = 0
    Dim StockAkhir As Double = 0
    Dim KodeTemp As String = ""
    Dim kdmerk As String
    Dim barangID(2) As String
    Dim SubKategoriID(2) As String

    Private Sub msgWarning(ByVal str As String)
        MsgBox(str, MsgBoxStyle.Critical, "Warning")
    End Sub
    Private Sub msgInfo(ByVal str As String)
        MsgBox(str, MsgBoxStyle.Information, "Information")
    End Sub
    Private Sub emptyField()
        txtID.Text = ""
        txtJenis.Text = ""
        txtType.Text = ""
        txtCari.Text = ""
    End Sub

    Private Sub setData()
        Try
            txtID.Text = DataGridView1.Rows(posisi).Cells(0).Value.ToString
            txtType.Text = DataGridView1.Rows(posisi).Cells(1).Value.ToString
            txtJenis.Text = DataGridView1.Rows(posisi).Cells(2).Value.ToString
            kdFisingView = Trim(txtID.Text)
        Catch ex As Exception
        End Try
    End Sub
    Private Sub generateCode()
        Dim code As String
        Dim angka As Integer
        Dim kode As String

        code = "FS"
        If DataGridView1.Rows.Count > 0 Then
            kode = DataGridView1.Rows(DataGridView1.Rows.Count - 1).Cells(0).Value.ToString()
            angka = CInt(kode.Substring(2, 3))
        Else
            angka = 0
        End If
        angka = angka + 1
        Dim len As Integer = angka.ToString().Length
        For i As Integer = 1 To 3 - len
            code += "0"
        Next
        code = code & (angka)
        txtID.Text = code
    End Sub
    Private Sub viewAllData(ByVal cr As String)
        sql = " select t.KdFising `Kode Fising`,TypeFising `Type Fising`,JenisFising,  " & _
              " t.KdBarang 'No. Part', NamaBarang 'Nama Part',ifnull(( " & _
              " Select StockAkhir " & _
              " from FisingHistory " & _
              " where isActive = 1 " & _
              " And KdFising = t.KdFising " & _
              " order by KdFisingHistory desc limit 1  " & _
              " ),0) `Stock Skrg`,ifnull(( " & _
              " Select StockAwal " & _
              " from FisingHistory " & _
              " where isActive = 1 " & _
              " And KdFising = t.KdFising " & _
              " order by KdFisingHistory desc limit 1  " & _
              " ),0) StockAwal " & _
              " from  " & tab & "  t " & _
              " Left Join MsBarang On MsBarang.KdBarang = t.KdBarang " & _
              " where 1 "
        TextBox1.Text = sql
        ' " WHEN LevelID = 3 THEN 'Superuser' End 'Level User' " & _
        '" CASE WHEN LevelID = 1 THEN 'Staff rio' WHEN LevelID = 2 THEN 'Staff lain' " & _
        sql &= QueryLevel(lvlKaryawan)
        If txtKdFising.Text <> "" Then
            sql = sql & "  And KdFising like '%" & Trim(txtKdFising.Text) & "%'"
        End If

        If cr <> "" Then
            sql = sql & "  And TypeFising like '%" & Trim(cr) & "%'"
        End If
        'MsgBox(sql)
        DataGridView1.DataSource = execute_datatable(sql)

    End Sub
    Private Sub ubahAktif(ByVal b As Boolean)
        txtID.Enabled = b
        txtType.Enabled = b
        txtJenis.Enabled = b
        txtQty.Enabled = b
        btnSave.Enabled = b
        btnCancel.Enabled = b
        btnExit.Enabled = Not b
        btnAdd.Enabled = Not b
        btnHistory.Enabled = Not b
        btnList.Enabled = Not b
        btnUpdate.Enabled = Not b
        btnDelete.Enabled = Not b
        DataGridView1.Enabled = Not b
    End Sub

    Private Sub setGrid()
        With DataGridView1.ColumnHeadersDefaultCellStyle
            .Alignment = DataGridViewContentAlignment.MiddleCenter
            .Font = New Font(.Font.FontFamily, .Font.Size, _
              .Font.Style Or FontStyle.Bold, GraphicsUnit.Point)
            .ForeColor = Color.Gold
        End With
        DataGridView1.Columns(0).Width = 200
        'DataGridView1.Columns(0).Width = 100
        DataGridView1.Columns(1).Width = 120
        DataGridView1.Columns(2).Width = 120
        DataGridView1.Columns(4).Width = 120
    End Sub

    Private Sub FormMsMerk_FormClosed(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosedEventArgs) Handles Me.FormClosed
        'Dim var As String = ""
        'Dim reader As SqlDataReader = Nothing
        'sql = "Select * from msmerk where merk='" & Trim(FormMsBarang.txtMerk.Text) & "'"
        'reader = New SqlCommand(sql, db.Connection).ExecuteReader
        'If reader.Read Then
        '    var = reader(0)
        'End If
        'reader.Close()
        'FormMsBarang.setComboMerk(var)
    End Sub
    Public Sub setComboMerk()
        'Dim reader As SqlDataReader
        'txtMerk.Items.Clear()
        'reader = New SqlCommand("Select * from msjenisfishing order by JenisFishing asc", db.Connection).ExecuteReader
        'Do While reader.Read
        '    txtMerk.Items.Add(reader(1)) '  & "-" & reader(0))
        'Loop
        'reader.Close()
        'If txtMerk.Items.Count > 0 Then txtMerk.SelectedIndex = 0
    End Sub

    Private Sub FormMsFising_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        tab = " MsFising "
        PK = "  KdFising  "
        ubahAktif(False)
        viewAllData("")
        'setComboMerk()
        posisi = 0
        setData()
        setGrid()
        setCmbSubKategori()
    End Sub
    Private Sub DataGridView1_CellClick1(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewCellEventArgs) Handles DataGridView1.CellClick
        If e.RowIndex >= 0 Then
            posisi = e.RowIndex
            setData()

        End If
    End Sub

    Private Sub txtNoPolisi_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txtType.KeyPress
        If AscW(e.KeyChar) = 13 Then
            btnSave_Click(Nothing, Nothing)
        End If
    End Sub

    Private Sub txtMerk_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs)

        Dim reader = execute_reader("Select * from msjenisfishing where JenisFishing ='" & Trim(txtJenis.Text) & "'  order by JenisFishing asc")
        Do While reader.Read
            kdmerk = reader(0)
        Loop
        reader.Close()
    End Sub

    Private Sub Button3_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
        FormMsMerk.Show()
    End Sub

    Private Sub txtCari_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles txtCari.TextChanged
        viewAllData(txtCari.Text)
    End Sub

    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
        txtKdFising.Text = ""
        txtCari.Text = ""
        viewAllData("")
    End Sub

    Function VisibleQty(ByVal status As Boolean)
        lblQty.Visible = status
        txtQty.Visible = status
        lblQtyBintang.Visible = status

        Return False
    End Function

    Private Sub btnAdd_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnAdd.Click
        'generateCode()
        VisibleQty(True)
        ubahAktif(True)
        status = "add"
        emptyField()
        setCmbSubKategori()
        txtID.Focus()

    End Sub


    Private Sub btnUpdate_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnUpdate.Click
        If (DataGridView1.RowCount) Then
            ubahAktif(True)
            status = "update"
            VisibleQty(False)
            txtType.Focus()
        Else
            msgInfo("Data tidak ditemukan.")
        End If
        'cmbNoInvoice.Enabled = False
    End Sub

    Private Sub btnDelete_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnDelete.Click
        If MsgBox("Apakah Anda yakin ingin menghapus fising " & txtType.Text & "?", MsgBoxStyle.Question + MsgBoxStyle.YesNo, "Confirmation") = MsgBoxResult.Yes Then
            execute_update("delete from " & tab & " where  " & PK & " = '" & txtID.Text & "'")
            msgInfo("Data berhasil dihapus")
            posisi = 0
            viewAllData("")
            setData()
        End If
    End Sub

    Private Sub btnSave_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSave.Click
        If txtType.Text = "" Then
            msgInfo("Type Fising  harus diisi")
            txtType.Focus()
        Else
            Dim QtyUpdate_Plus = 0
            Dim QtyUpdate_Min = ""
            Dim QtyUpdate = 0
            Dim OP = ""
            Dim Attribute = ""
            StockAwal = StockAkhir

            If status = "add" Then
                If (txtQty.Text = "" Or txtQty.Text = 0) Then
                    msgInfo("Jumlah harus diisi dan lebih besar dari 0")
                    txtQty.Focus()
                ElseIf txtPartNo.Text = "-" Or Trim(txtPartNo.Text) = "" Then
                    msgInfo("Barang jadi harus dipilih")
                    cmbNamaBarang.Focus()
                Else
                    Dim reader As SqlDataReader = Nothing
                    Try
                        'reader = New SqlCommand("Select * from " & tab & " where TypeFising='" & Trim(txtNoPolisi.Text) & "'  and KdJenisFishing='" & kdmerk & "'", db.Connection).ExecuteReader
                        'If reader.Read Then
                        '    msgInfo("Type fising ini telah ada di dalam database")
                        '    txtNoPolisi.Text = ""
                        '    txtNoPolisi.Focus()
                        '    reader.Close()
                        'Else
                        '    reader.Close()
                        sql = " insert into  " & tab & "  values('" + Trim(txtID.Text) + "', " & _
                              " '" & Trim(txtType.Text) & "','" & Trim(txtJenis.Text) & "', " & _
                              " 0, '" & Trim(lvlKaryawan) & "','" & Trim(txtPartNo.Text) & "')"
                        ' txtCari.Text = sql
                        'MsgBox(sql)
                        If (OldQty < txtQty.Text) Then
                            QtyUpdate_Plus = Val(txtQty.Text) - Val(OldQty)
                            QtyUpdate = QtyUpdate_Plus
                            StockAkhir += Val(QtyUpdate)
                            OP = "Plus"
                            Attribute = "QtyUpdate_Plus"
                        ElseIf (OldQty > txtQty.Text) Then
                            QtyUpdate_Min = Val(OldQty) - Val(txtQty.Text)
                            QtyUpdate = QtyUpdate_Min
                            StockAkhir -= Val(QtyUpdate)
                            OP = "Min"
                            Attribute = "QtyUpdate_Min"
                        End If

                        dbconmanual.Open()
                        Dim trans As MySql.Data.MySqlClient.MySqlTransaction

                        trans = dbconmanual.BeginTransaction(IsolationLevel.ReadCommitted)
                        Try
                            execute_update_manual(sql)
                            If QtyUpdate <> 0 Then
                                StockFising(QtyUpdate, OP, 0, Trim(txtID.Text), Attribute, "", "Form Fising")
                            End If
                            trans.Commit()

                            viewAllData("")
                            ubahAktif(False)
                            msgInfo("Data berhasil disimpan")
                        Catch ex As Exception
                            trans.Rollback()
                            msgWarning(ex.ToString)
                        End Try
                        dbconmanual.Close()
                        'Try
                        '    execute_update(sql)
                        '    viewAllData("")
                        '    ubahAktif(False)
                        '    msgInfo("Data berhasil disimpan")
                        'Catch ex As Exception
                        '    msgWarning("Data tidak valid")
                        '    txtNoPolisi.Text = ""
                        '    txtNoPolisi.Focus()
                        'End Try
                        'End If
                    Catch ex As Exception
                        msgWarning(ex.ToString)
                        txtType.Text = ""
                        txtType.Focus()
                    End Try
                End If
                ' reader.Close()
            ElseIf status = "update" Then
                Try
                    If txtPartNo.Text = "-" Or Trim(txtPartNo.Text) = "" Then
                        msgInfo("Barang jadi harus dipilih")
                        cmbNamaBarang.Focus()
                    Else
                        sql = "  update   " & tab & "  set JenisFising = '" & Trim(txtJenis.Text) & "',  " & _
                              " TypeFising = '" & Trim(txtType.Text) & "', " & _
                              " KdBarang = '" & Trim(txtPartNo.Text) & "' " & _
                              "  where  " & PK & "='" + txtID.Text + "' "
                        execute_update(sql)
                        viewAllData("")
                        ubahAktif(False)
                        msgInfo("Data berhasil disimpan")
                    End If
                Catch ex As Exception
                    msgWarning(" Data tidak valid")
                    txtType.Text = ""
                    txtType.Focus()
                End Try
            End If
        End If
    End Sub

    Private Sub btnCancel_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnCancel.Click
        If status = "add" Then
            posisi = 0
        End If
        ubahAktif(False)
        setData()
    End Sub

    Private Sub btnExit_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnExit.Click
        Me.Close()
    End Sub

   


   

   

    Public Sub setCmbSubKategori()
        Dim reader = execute_reader("Select * from MsSubkategori  where SubKategori <>'' order by SubKategori asc")
        Dim varT As String = ""
        cmbSubKategori.Items.Clear()
        ' cmbSubKategori.Items.Add("- Pilih Sub Kategori -")
        Do While reader.Read
            cmbSubKategori.Items.Add(reader(0) & " ~ " & reader(1))
        Loop
        reader.Close()
        If cmbSubKategori.Items.Count > 0 Then
            cmbSubKategori.SelectedIndex = 0
        End If
        reader.Close()
    End Sub


    Private Sub cmbSubKategori_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmbSubKategori.SelectedIndexChanged
        SubKategoriID = cmbSubKategori.Text.ToString.Split(" ~ ")
        cmbNamaBarang.Items.Clear()
        Dim reader = execute_reader("Select kdbarang,namabarang from msbarang where kdsub='" & SubKategoriID(0) & "' order by namabarang asc")
        Do While reader.Read
            cmbNamaBarang.Items.Add(reader(0) & " ~ " & reader(1))
        Loop
        reader.Close()
        If cmbNamaBarang.Items.Count > 0 Then cmbNamaBarang.SelectedIndex = 0
    End Sub

    Private Sub cmbNamaBarang_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmbNamaBarang.SelectedIndexChanged
        barangID = cmbNamaBarang.Text.ToString.Split(" ~ ")
        txtPartNo.Text = barangID(0)

        'sql = " Select StockAkhir from BarangHistory where isActive = 1 And kdBarang = '" & cmbDari.Text & "' order by KdBarangHistory desc limit 1"
        'Dim reader = execute_reader(sql)
        'If reader.Read Then
        '    lblStock.Text = reader(0)
        'End If
        'reader.Close()
        'reader = Nothing
        'sql = " Select Harga From MsBarangList where KdBarang = '" & cmbDari.Text & "' And StatusBarangList = 0  Order By KdList asc Limit 1"
        'reader = execute_reader(sql)
        'If reader.Read Then
        '    lblHarga.Text = CDbl(reader(0))
        'End If
        'reader.Close()
        'txtHarga.Text = lblHarga.Text
    End Sub

    Private Sub DataGridView1_CellContentClick(ByVal sender As System.Object, ByVal e As System.Windows.Forms.DataGridViewCellEventArgs) Handles DataGridView1.CellContentClick

    End Sub

    Private Sub Button2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button2.Click
        open_page("FormMsFisingKomposisi")
    End Sub

    Private Sub txtQty_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txtQty.KeyPress
        If e.KeyChar <> "." And AscW(e.KeyChar) <> 8 And (Asc(e.KeyChar) < Asc("0") Or Asc(e.KeyChar) > Asc("9")) Then
            e.KeyChar = Nothing
        End If
        'If AscW(e.KeyChar) = 13 And txtKomisi.Text <> "" Then
        '    btnSave_Click(Nothing, Nothing)
        'End If
    End Sub

    Private Sub txtQty_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles txtQty.TextChanged

    End Sub

    Private Sub btnHistory_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnHistory.Click
        Dim selected_cell = DataGridView1.CurrentRow.Cells(0).Value
        open_subpage("FormFisingHistory", selected_cell)
    End Sub

    Private Sub btnList_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnList.Click
        Dim selected_cell = DataGridView1.CurrentRow.Cells("Kode Fising").Value
        open_subpage("FormFisingList", selected_cell)
    End Sub

    Private Sub txtKdFising_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles txtKdFising.TextChanged
        viewAllData(txtCari.Text)
    End Sub
End Class