Imports System.Data.SqlClient
Public Class FormMsFarmasi
    Dim LastKode = ""

    Private Sub msgWarning(ByVal str As String)
        MsgBox(str, MsgBoxStyle.Critical, "Warning")
    End Sub

    Private Sub msgInfo(ByVal str As String)
        MsgBox(str, MsgBoxStyle.Information, "Information")
    End Sub

    Private Sub emptyField()
        txtCari.Text = ""
        txtKd.Text = ""
        LastKode = ""
        cmbSupplier.SelectedIndex = 0
        cmbDistributor.SelectedIndex = 0
        txtNama.Text = ""
        txtAlamat.Text = ""
        txtNoHP.Text = ""
        txtNoFax.Text = ""
        txtEmail.Text = ""
    End Sub

    Private Sub ubahAktif(ByVal status As Boolean)
        txtKd.Enabled = status
        cmbSupplier.Enabled = status
        BrowseSupplier.Enabled = status
        cmbDistributor.Enabled = status
        BrowseDistributor.Enabled = status
        txtNama.Enabled = status
        txtAlamat.Enabled = status
        txtNoHP.Enabled = status
        txtNoFax.Enabled = status
        txtEmail.Enabled = status
        btnSave.Enabled = status
        btnCancel.Enabled = status

        btnExit.Enabled = Not status
        GroupBox1.Enabled = Not status

        btnSave.Enabled = status
        btnCancel.Enabled = status
        btnAdd.Enabled = Not status
        btnUpdate.Enabled = Not status
        btnDelete.Enabled = Not status
        grdFarmasi.Enabled = Not status
    End Sub

    Private Sub setData()

        If grdFarmasi.RowCount > 0 Then
            Try
                Dim KdSupplier = grdFarmasi.CurrentRow.Cells("KdProdusen").Value.ToString
                Dim NamaSupplier = grdFarmasi.CurrentRow.Cells("Produsen").Value.ToString
                Dim KdDistributor = grdFarmasi.CurrentRow.Cells("KdDistributor").Value.ToString
                Dim NamaDistributor = grdFarmasi.CurrentRow.Cells("Distributor").Value.ToString
                txtKd.Text = grdFarmasi.CurrentRow.Cells("Call ID").Value.ToString
                cmbSupplier.Text = KdSupplier & " ~ " & NamaSupplier
                cmbDistributor.Text = KdDistributor & " ~ " & NamaDistributor
                txtNama.Text = grdFarmasi.CurrentRow.Cells("Nama AM").Value.ToString
                txtAlamat.Text = grdFarmasi.CurrentRow.Cells("Alamat").Value.ToString
                txtNoHP.Text = grdFarmasi.CurrentRow.Cells("No. HP").Value.ToString
                txtNoFax.Text = grdFarmasi.CurrentRow.Cells("No. Fax").Value.ToString
                txtEmail.Text = grdFarmasi.CurrentRow.Cells("Email").Value.ToString

                LastKode = txtKd.Text
            Catch ex As Exception
                MsgBox(ex, MsgBoxStyle.Information)
            End Try
        End If
    End Sub

    Private Sub setCmbCari()
        cmbCari.Items.Clear()
        cmbCari.Items.Add("Call ID")
        cmbCari.Items.Add("Produsen")
        cmbCari.Items.Add("Distributor")
        cmbCari.Items.Add("Nama AM")
        cmbCari.Items.Add("Alamat")
        cmbCari.Items.Add("No. HP")
        cmbCari.Items.Add("No. Fax")
        cmbCari.Items.Add("Email")
        cmbCari.SelectedIndex = 0
    End Sub

    Private Sub viewAllData(ByVal cr As String, ByVal opt As String)
        sql = " SELECT 	KdFarmasi 'Call ID', mf.KdProdusen, mf.KdDistributor, " & _
              " mf.NamaAM 'Nama AM', IFNULL(ms.Nama, '') Produsen, " & _
              " IFNULL(md.Nama, '') Distributor, mf.Alamat, mf.NoHP 'No. HP', " & _
              " mf.Email, mf.NoFax 'No. Fax' " & _
              " FROM msfarmasi mf " & _
              " LEFT JOIN MsSupplier ms ON ms.KdSupplier = mf.KdProdusen " & _
              " LEFT JOIN MsDistributor md ON md.KdDistributor = mf.KdDistributor " & _
              " WHERE mf.IsAktif = 1 "

        If opt <> "" Then
            Dim col As String = ""
            If opt = "Call ID" Then
                col = "mf.KdFarmasi"
            ElseIf opt = "Produsen" Then
                col = "ms.Nama"
            ElseIf opt = "Distributor" Then
                col = "md.Nama"
            ElseIf opt = "Nama" Then
                col = "mf.NamaAM"
            ElseIf opt = "Alamat" Then
                col = "mf.Alamat"
            ElseIf opt = "Nomor HP" Then
                col = "mf.NoHP"
            ElseIf opt = "No. Fax" Then
                col = "mf.NoFax"
            ElseIf opt = "Email" Then
                col = "mf.Email"
            End If
            sql &= " AND " & col & "  like '%" & cr & "%' "
        End If

        sql &= " ORDER BY mf.NamaAM "
        grdFarmasi.DataSource = execute_datatable(sql)
        setGrid()
        setData()
    End Sub

    Private Sub setGrid()
        With grdFarmasi.ColumnHeadersDefaultCellStyle
            .Alignment = DataGridViewContentAlignment.MiddleCenter
            .Font = New Font(.Font.FontFamily, .Font.Size, _
              .Font.Style Or FontStyle.Bold, GraphicsUnit.Point)
            .ForeColor = Color.Gold

        End With
        grdFarmasi.Columns("Alamat").Width = 150
        grdFarmasi.Columns("KdProdusen").Visible = False
        grdFarmasi.Columns("KdDistributor").Visible = False

    End Sub

    Public Sub setCmbSupplier()
        Dim varT As String = ""
        cmbSupplier.Items.Clear()
        cmbSupplier.Items.Add("- Pilih -")
        sql = " SELECT KdSupplier, Nama " & _
              " FROM MsSupplier " & _
              " WHERE IsAktif = 1 " & _
              " ORDER BY Nama "
        Dim readerSupplier = execute_reader(sql)
        Do While readerSupplier.Read
            cmbSupplier.Items.Add(readerSupplier("KdSupplier") & " ~ " & readerSupplier("Nama"))
        Loop
        readerSupplier.Close()
        If cmbSupplier.Items.Count > 0 Then
            cmbSupplier.SelectedIndex = 0
        End If
        readerSupplier.Close()
    End Sub

    Public Sub setCmbDistributor()
        Dim varT As String = ""
        cmbDistributor.Items.Clear()
        cmbDistributor.Items.Add("- Pilih -")
        sql = " SELECT KdDistributor, Nama " & _
              " FROM msdistributor " & _
              " WHERE IsAktif = 1 " & _
              " ORDER BY Nama "
        Dim readerDistributor = execute_reader(sql)
        Do While readerDistributor.Read
            cmbDistributor.Items.Add(readerDistributor("KdDistributor") & " ~ " & readerDistributor("Nama"))
        Loop
        readerDistributor.Close()
        If cmbDistributor.Items.Count > 0 Then
            cmbDistributor.SelectedIndex = 0
        End If
        readerDistributor.Close()
    End Sub

    Private Sub FormMsSales_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        setCmbSupplier()
        setCmbDistributor()
        emptyField()
        ubahAktif(False)
        viewAllData("", "")
        setCmbCari()
    End Sub

    Private Sub generateCode()
        Dim code As String = "FR"
        Dim angka As Integer
        Dim kode As String
        Dim temp As String
        code += Today.Year.ToString.Substring(2, 2)
        Dim bulan As Integer = CInt(Today.Month.ToString)

        If bulan < 10 Then
            code += "0" + bulan.ToString
        Else
            code += bulan.ToString
        End If

        Dim reader = getCode("MsFarmasi", "KdFarmasi")

        If reader.read Then
            kode = Trim(reader(0).ToString())
            temp = kode.Substring(0, 6)
            If temp = code Then
                angka = CInt(kode.Substring(6, 4))
            Else
                angka = 0
            End If
        Else
            angka = 0
        End If
        reader.close()

        angka = angka + 1
        Dim len As Integer = angka.ToString().Length
        For i As Integer = 1 To 4 - len
            code += "0"
        Next
        code = code & (angka)
        txtKd.Text = Trim(code)
    End Sub

    Private Sub txtCari_TextChanged1(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtCari.TextChanged
        viewAllData(txtCari.Text, cmbCari.Text)
    End Sub

    Private Sub DataGridView1_CellClick(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewCellEventArgs) Handles grdFarmasi.CellClick, grdFarmasi.CellClick
        If e.RowIndex >= 0 And e.RowIndex <= grdFarmasi.Rows.Count - 1 Then
            setData()
        End If
    End Sub

    Private Sub Button1_Click_1(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnReset.Click
        txtCari.Text = ""
        viewAllData("", "")
        txtCari.Focus()
    End Sub

    Private Sub txtNama_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txtNama.KeyPress
        If Asc(e.KeyChar) = Asc("'") And AscW(e.KeyChar) <> 13 And AscW(e.KeyChar) <> 8 Then
            e.KeyChar = Nothing
        End If
        If AscW(e.KeyChar) = 13 And txtNama.Text <> "" Then
            txtAlamat.Focus()
        End If
    End Sub

    Private Sub btnAdd_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnAdd.Click
        emptyField()
        ubahAktif(True)
        generateCode()
        txtKd.Focus()
    End Sub

    Private Sub btnUpdate_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnUpdate.Click
        If grdFarmasi.RowCount Then
            ubahAktif(True)
            txtNama.Focus()
        Else
            msgInfo("Data tidak ditemukan.")
        End If
    End Sub

    Private Sub btnDelete_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnDelete.Click
        If grdFarmasi.RowCount Then
            Dim selected_cell = grdFarmasi.CurrentRow.Cells("Call ID").Value
            If MsgBox("Anda yakin ingin menghapus Call ID " & selected_cell & "?", MsgBoxStyle.Question + MsgBoxStyle.YesNo, "Confirmation") = MsgBoxResult.Yes Then
                execute_update(" UPDATE msfarmasi SET " & _
                               " IsAktif = 0 " & _
                               " WHERE KdFarmasi = '" & selected_cell & "' ")
                msgInfo("Data berhasil dihapus")
                viewAllData("", "")
            End If
        Else
            msgInfo("Data tidak ditemukan.")
        End If
    End Sub

    Private Sub btnSave_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSave.Click
        Dim addQuery = ""
        Dim readUsedCode = execute_reader(" SELECT 1 FROM msfarmasi " & _
                                           " WHERE KdFarmasi = '" & txtKd.Text & "' " & _
                                           " AND KdFarmasi <> '" & LastKode & "' ")
        If txtKd.Text = "" Then
            msgInfo("Call ID harus diisi")
            txtKd.Focus()
        ElseIf readUsedCode.read Then
            msgInfo("Call ID sudah terdaftar. Silakan ganti yang lain.")
            txtKd.Focus()
        ElseIf txtNama.Text = "" Then
            msgInfo("Nama harus diisi")
            txtNama.Focus()
        ElseIf Trim(txtEmail.Text) <> "" And Not validate_email(txtEmail.Text, False) Then
            msgInfo("Format email salah")
            txtEmail.Focus()
        Else
            If LastKode = "" Then
                LastKode = txtKd.Text
            End If
            Dim KdProdusen = cmbSupplier.Text.ToString.Split("~")
            Dim KdDistributor = cmbDistributor.Text.ToString.Split("~")

            If cmbSupplier.SelectedIndex = 0 Then
                KdProdusen(0) = ""
            End If

            If cmbDistributor.SelectedIndex = 0 Then
                KdDistributor(0) = ""
            End If

            sql = " INSERT INTO msfarmasi ( " & _
                  "     KdFarmasi, KdProdusen, " & _
                  "     KdDistributor, NamaAM, " & _
                  "     Alamat, NoHP, " & _
                  "     Email, NoFax, " & _
                  "     Created " & _
                  " ) values( " & _
                  "     '" & Trim(LastKode) & "', '" & Trim(KdProdusen(0)) & "', " & _
                  "     '" & Trim(KdDistributor(0)) & "', '" & Trim(txtNama.Text) & "', " & _
                  "     '" & Trim(txtAlamat.Text) & "', '" & Trim(txtNoHP.Text) & "', " & _
                  "     '" & Trim(txtEmail.Text) & "', '" & Trim(txtNoFax.Text) & "', " & _
                  "     now() " & _
                  " )  ON DUPLICATE KEY UPDATE " & _
                  " KdFarmasi = '" & Trim(txtKd.Text) & "', " & _
                  " KdProdusen = '" & Trim(KdProdusen(0)) & "', " & _
                  " KdDistributor = '" & Trim(KdDistributor(0)) & "', " & _
                  " NamaAM = '" & Trim(txtNama.Text) & "', " & _
                  " Alamat = '" & Trim(txtAlamat.Text) & "', " & _
                  " NoHP = '" & Trim(txtNoHP.Text) & "', " & _
                  " Email = '" & Trim(txtEmail.Text) & "', " & _
                  " NoFax = '" & Trim(txtNoFax.Text) & "' "
            Try
                execute_update(sql)
                viewAllData("", "")
                ubahAktif(False)
                msgInfo("Data berhasil disimpan")
            Catch ex As Exception
                MsgBox(ex, MsgBoxStyle.Information)
            End Try
            readUsedCode.close()
        End If
    End Sub

    Private Sub btnCancel_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnCancel.Click
        ubahAktif(False)
        setData()
    End Sub

    Private Sub btnExit_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnExit.Click
        Me.Close()
    End Sub

    Private Sub txtAlamat_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txtAlamat.KeyPress
        If Asc(e.KeyChar) = Asc("'") And AscW(e.KeyChar) <> 13 And AscW(e.KeyChar) <> 8 Then
            e.KeyChar = Nothing
        End If
    End Sub

    Private Sub txtKd_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txtKd.KeyPress, txtKd.KeyPress
        If Asc(e.KeyChar) = Asc("'") And AscW(e.KeyChar) <> 13 And AscW(e.KeyChar) <> 8 Then
            e.KeyChar = Nothing
        End If
        If AscW(e.KeyChar) = 13 And txtKd.Text <> "" Then
            cmbSupplier.Focus()
        End If
    End Sub

    Private Sub txtNoHP_KeyPress1(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txtNoHP.KeyPress
        If Asc(e.KeyChar) <> Asc("-") And AscW(e.KeyChar) <> 13 And AscW(e.KeyChar) <> 8 And (Asc(e.KeyChar) < Asc("0") Or Asc(e.KeyChar) > Asc("9")) Then
            e.KeyChar = Nothing
        End If
        If AscW(e.KeyChar) = 13 And txtNoHP.Text <> "" Then
            txtNoFax.Focus()
        End If
    End Sub

    Private Sub cmbCari_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmbCari.SelectedIndexChanged
        btnReset.PerformClick()
    End Sub

    Private Sub txtCari_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txtCari.KeyPress
        If Asc(e.KeyChar) = Asc("'") And AscW(e.KeyChar) <> 13 And AscW(e.KeyChar) <> 8 Then
            e.KeyChar = Nothing
        End If
    End Sub

    Private Sub txtEmail_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txtEmail.KeyPress

    End Sub

    Private Sub txtNoFax_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txtNoFax.KeyPress
        If Asc(e.KeyChar) <> Asc("-") And AscW(e.KeyChar) <> 13 And AscW(e.KeyChar) <> 8 And (Asc(e.KeyChar) < Asc("0") Or Asc(e.KeyChar) > Asc("9")) Then
            e.KeyChar = Nothing
        End If
        If AscW(e.KeyChar) = 13 And txtNoHP.Text <> "" Then
            txtEmail.Focus()
        End If
    End Sub

    Private Sub BrowseSupplier_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BrowseSupplier.Click
        Try
            sub_form = New FormBrowseSupplier
            sub_form.showDialog(FormMain)
            If data_carier(0) <> "" Then
                cmbSupplier.Text = data_carier(0) & " ~ " & data_carier(1)
                clear_variable_array()
            End If
        Catch ex As Exception
            MsgBox(ex, MsgBoxStyle.Critical, "Warning!!!")
        End Try
    End Sub

    Private Sub BrowseDistributor_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BrowseDistributor.Click
        Try
            sub_form = New FormBrowseDistributor
            sub_form.showDialog(FormMain)
            If data_carier(0) <> "" Then
                cmbDistributor.Text = data_carier(0) & " ~ " & data_carier(1)
                clear_variable_array()
            End If
        Catch ex As Exception
            MsgBox(ex, MsgBoxStyle.Critical, "Warning!!!")
        End Try
    End Sub
End Class
