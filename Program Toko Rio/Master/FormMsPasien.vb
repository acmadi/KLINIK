Imports System.Data.SqlClient
Public Class FormMsPasien
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
        txtNama.Text = ""
        dtpTglLahir.Text = Now()
        txtTempatLahir.Text = ""
        txtNoIdentitas.Text = ""
        txtAlamat.Text = ""
    End Sub

    Private Sub ubahAktif(ByVal status As Boolean)
        txtKd.Enabled = status
        txtNama.Enabled = status
        dtpTglLahir.Enabled = status
        txtTempatLahir.Enabled = status
        txtNoIdentitas.Enabled = status
        txtAlamat.Enabled = status
        btnSave.Enabled = status
        btnCancel.Enabled = status

        btnExit.Enabled = Not status
        GroupBox1.Enabled = Not status

        btnSave.Enabled = status
        btnCancel.Enabled = status
        btnAdd.Enabled = Not status
        btnUpdate.Enabled = Not status
        btnDelete.Enabled = Not status
        grdPasien.Enabled = Not status
    End Sub

    Private Sub setData()

        If grdPasien.RowCount > 0 Then
            Try
                txtKd.Text = grdPasien.CurrentRow.Cells("Call ID").Value.ToString
                txtNama.Text = grdPasien.CurrentRow.Cells("Nama").Value.ToString
                dtpTglLahir.Text = grdPasien.CurrentRow.Cells("Tanggal Lahir").Value.ToString
                txtTempatLahir.Text = grdPasien.CurrentRow.Cells("Tempat Lahir").Value.ToString
                txtNoIdentitas.Text = grdPasien.CurrentRow.Cells("No. Identitas").Value.ToString
                txtAlamat.Text = grdPasien.CurrentRow.Cells("Alamat").Value.ToString

                LastKode = txtKd.Text
            Catch ex As Exception
                MsgBox(ex, MsgBoxStyle.Information)
            End Try
        End If
    End Sub

    Private Sub setCmbCari()
        cmbCari.Items.Clear()
        cmbCari.Items.Add("Call ID")
        cmbCari.Items.Add("Nama")
        cmbCari.Items.Add("Tanggal Lahir")
        cmbCari.Items.Add("Tempat Lahir")
        cmbCari.Items.Add("No. Identitas")
        cmbCari.Items.Add("Alamat")
        cmbCari.SelectedIndex = 0
    End Sub

    Private Sub viewAllData(ByVal cr As String, ByVal opt As String)
        sql = " SELECT 	KdPasien 'Call ID', NamaPasien Nama, " & _
              " TglLahir 'Tanggal Lahir', TemmpatLahir 'Tempat Lahir', " & _
              " Alamat, NoIdentitas 'No. Identitas' " & _
              " FROM mspasien " & _
              " WHERE IsAktif = 1 "

        If opt <> "" Then
            Dim col As String = ""
            If opt = "Call ID" Then
                col = "KdPasien"
            ElseIf opt = "Nama" Then
                col = "NamaPasien"
            ElseIf opt = "Tanggal Lahir" Then
                col = "TglLahir"
            ElseIf opt = "Tempat Lahir" Then
                col = "TemmpatLahir"
            ElseIf opt = "No. Identitas" Then
                col = "NoIdentitas"
            ElseIf opt = "Alamat" Then
                col = "Alamat"
            End If

            If col = "TglLahir" Then
                sql &= " And DATE_FORMAT(TglLahir, '%Y/%m/%d') >= '" & dtpFrom.Value.ToString("yyyy/MM/dd") & "' " & _
                       " and DATE_FORMAT(TglLahir, '%Y/%m/%d') <= '" & dtpTo.Value.ToString("yyyy/MM/dd") & "' "
            Else
                sql &= " AND " & col & "  like '%" & cr & "%' "
            End If
        End If
        sql &= " ORDER BY NamaPasien "

        grdPasien.DataSource = execute_datatable(sql)
        setGrid()
        setData()
    End Sub

    Private Sub setGrid()
        With grdPasien.ColumnHeadersDefaultCellStyle
            .Alignment = DataGridViewContentAlignment.MiddleCenter
            .Font = New Font(.Font.FontFamily, .Font.Size, _
              .Font.Style Or FontStyle.Bold, GraphicsUnit.Point)
            .ForeColor = Color.Gold

        End With
        grdPasien.Columns("Alamat").Width = 150

    End Sub

    Function visibleDate()
        dtpFrom.Visible = True
        dtpTo.Visible = True
        lblSeperator.Visible = True
        btnCari.Visible = True

        txtCari.Visible = False
        btnReset.Visible = False
        Return False
    End Function

    Function visibleCari()
        dtpFrom.Visible = False
        dtpTo.Visible = False
        lblSeperator.Visible = False
        btnCari.Visible = False

        txtCari.Visible = True
        btnReset.Visible = True
        Return False
    End Function

    Private Sub FormMsSales_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        emptyField()
        ubahAktif(False)
        viewAllData("", "")
        setCmbCari()
    End Sub

    Private Sub generateCode()
        Dim code As String = "PS"
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

        Dim reader = getCode("MsPasien", "KdPasien")

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

    Private Sub txtCari_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txtCari.KeyPress
        If Asc(e.KeyChar) = Asc("'") And AscW(e.KeyChar) <> 13 And AscW(e.KeyChar) <> 8 Then
            e.KeyChar = Nothing
        End If
    End Sub

    Private Sub txtCari_TextChanged1(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtCari.TextChanged
        viewAllData(txtCari.Text, cmbCari.Text)
    End Sub

    Private Sub DataGridView1_CellClick(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewCellEventArgs) Handles grdPasien.CellClick, grdPasien.CellClick
        If e.RowIndex >= 0 And e.RowIndex <= grdPasien.Rows.Count - 1 Then
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
            dtpTglLahir.Focus()
        End If
    End Sub

    Private Sub btnAdd_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnAdd.Click
        emptyField()
        ubahAktif(True)
        generateCode()
        txtKd.Focus()
    End Sub

    Private Sub btnUpdate_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnUpdate.Click
        If grdPasien.RowCount Then
            ubahAktif(True)
            txtNama.Focus()
        Else
            msgInfo("Data tidak ditemukan.")
        End If
    End Sub

    Private Sub btnDelete_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnDelete.Click
        If grdPasien.RowCount Then
            Dim selected_cell = grdPasien.CurrentRow.Cells("Call ID").Value
            If MsgBox("Anda yakin ingin menghapus Call ID " & selected_cell & "?", MsgBoxStyle.Question + MsgBoxStyle.YesNo, "Confirmation") = MsgBoxResult.Yes Then
                execute_update(" UPDATE mspasien SET " & _
                               " IsAktif = 0 " & _
                               " WHERE KdPasien = '" & selected_cell & "' ")
                msgInfo("Data berhasil dihapus")
                viewAllData("", "")
            End If
        Else
            msgInfo("Data tidak ditemukan.")
        End If
    End Sub

    Private Sub btnSave_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSave.Click
        Dim addQuery = ""
        Dim readUsedCode = execute_reader(" SELECT 1 FROM mspasien " & _
                                           " WHERE KdPasien = '" & txtKd.Text & "' " & _
                                           " AND KdPasien <> '" & LastKode & "' ")
        If txtKd.Text = "" Then
            msgInfo("Call ID harus diisi")
            txtKd.Focus()
        ElseIf readUsedCode.read Then
            msgInfo("Call ID sudah terdaftar. Silakan ganti yang lain.")
            txtKd.Focus()
        ElseIf txtNama.Text = "" Then
            msgInfo("Nama harus diisi")
            txtNama.Focus()
        Else
            If LastKode = "" Then
                LastKode = txtKd.Text
            End If

            sql = " INSERT INTO mspasien ( " & _
                  "     KdPasien, NamaPasien, " & _
                  "     TglLahir, TemmpatLahir, " & _
                  "     Alamat, NoIdentitas, " & _
                  "     Created " & _
                  " ) values( " & _
                  "     '" & Trim(LastKode) & "', '" & Trim(txtNama.Text) & "', " & _
                  "     '" & dtpTglLahir.Value.ToString("yyyy/MM/dd") & "', " & _
                  "     '" & Trim(txtTempatLahir.Text) & "', " & _
                  "     '" & Trim(txtAlamat.Text) & "', '" & Trim(txtNoIdentitas.Text) & "', " & _
                  "     now() " & _
                  " )  ON DUPLICATE KEY UPDATE " & _
                  " KdPasien = '" & Trim(txtKd.Text) & "', " & _
                  " NamaPasien = '" & Trim(txtNama.Text) & "', " & _
                  " TglLahir = '" & dtpTglLahir.Value.ToString("yyyy/MM/dd") & "', " & _
                  " TemmpatLahir = '" & Trim(txtTempatLahir.Text) & "', " & _
                  " Alamat = '" & Trim(txtAlamat.Text) & "', " & _
                  " NoIdentitas = '" & Trim(txtNoIdentitas.Text) & "' "
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

    Private Sub txtKd_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txtKd.KeyPress, txtKd.KeyPress
        If Asc(e.KeyChar) = Asc("'") And AscW(e.KeyChar) <> 13 And AscW(e.KeyChar) <> 8 Then
            e.KeyChar = Nothing
        End If
        If AscW(e.KeyChar) = 13 And txtKd.Text <> "" Then
            txtNama.Focus()
        End If
    End Sub

    Private Sub txtAlamat_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txtAlamat.KeyPress
        If Asc(e.KeyChar) = Asc("'") And AscW(e.KeyChar) <> 13 And AscW(e.KeyChar) <> 8 Then
            e.KeyChar = Nothing
        End If
    End Sub

    Private Sub txtNoIdentitas_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txtNoIdentitas.KeyPress
        If Asc(e.KeyChar) = Asc("'") And AscW(e.KeyChar) <> 13 And AscW(e.KeyChar) <> 8 Then
            e.KeyChar = Nothing
        End If
        If AscW(e.KeyChar) = 13 And txtNoIdentitas.Text <> "" Then
            txtAlamat.Focus()
        End If
    End Sub

    Private Sub txtTempatLahir_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txtTempatLahir.KeyPress
        If Asc(e.KeyChar) = Asc("'") And AscW(e.KeyChar) <> 13 And AscW(e.KeyChar) <> 8 Then
            e.KeyChar = Nothing
        End If
        If AscW(e.KeyChar) = 13 And txtTempatLahir.Text <> "" Then
            txtNoIdentitas.Focus()
        End If
    End Sub

    Private Sub cmbCari_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmbCari.SelectedIndexChanged
        If cmbCari.SelectedIndex = 2 Then
            visibleDate()
        Else
            visibleCari()
        End If
        btnReset.PerformClick()
    End Sub

    Private Sub btnCari_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnCari.Click
        viewAllData("", cmbCari.Text)
    End Sub
End Class
