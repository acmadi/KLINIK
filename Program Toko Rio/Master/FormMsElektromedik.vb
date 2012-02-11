Imports System.Data.SqlClient
Public Class FormMsElektromedik
    Dim LastKode = ""

    Private Sub msgWarning(ByVal str As String)
        MsgBox(str, MsgBoxStyle.Critical, "Warning")
    End Sub

    Private Sub msgInfo(ByVal str As String)
        MsgBox(str, MsgBoxStyle.Information, "Information")
    End Sub

    Private Sub emptyField()
        txtCari.Text = ""
        txtNama.Text = ""
        txtTarif.Text = ""
        txtKd.Text = ""
        LastKode = ""
    End Sub

    Private Sub ubahAktif(ByVal status As Boolean)
        txtKd.Enabled = status
        txtNama.Enabled = status
        txtTarif.Enabled = status
        btnSave.Enabled = status
        btnCancel.Enabled = status

        btnExit.Enabled = Not status
        GroupBox1.Enabled = Not status

        btnSave.Enabled = status
        btnCancel.Enabled = status
        btnAdd.Enabled = Not status
        btnUpdate.Enabled = Not status
        btnDelete.Enabled = Not status
        grdElektromedik.Enabled = Not status
    End Sub

    Private Sub setData()

        If grdElektromedik.RowCount > 0 Then
            Try
                txtKd.Text = grdElektromedik.CurrentRow.Cells("Call ID").Value.ToString
                txtNama.Text = grdElektromedik.CurrentRow.Cells("Nama").Value.ToString
                txtTarif.Text = Val(grdElektromedik.CurrentRow.Cells("Tarif").Value.ToString)

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
        cmbCari.Items.Add("Tarif")
        cmbCari.SelectedIndex = 0
    End Sub

    Private Sub viewAllData(ByVal cr As String, ByVal opt As String)
        sql = " SELECT KdElektromedik 'Call ID', NamaElektromedik 'Nama',  " & _
              " TarifPemeriksaan 'Tarif' " & _
              " FROM mselektromedik " & _
              " WHERE IsAktif = 1 "

        If opt <> "" Then
            Dim col As String = ""
            If opt = "Call ID" Then
                col = "KdElektromedik"
            ElseIf opt = "Nama" Then
                col = "NamaElektromedik"
            ElseIf opt = "Tarif" Then
                col = "TarifPemeriksaan"
            End If
            sql &= " AND " & col & "  like '%" & cr & "%' "
        End If
        sql &= " ORDER BY NamaElektromedik "

        grdElektromedik.DataSource = execute_datatable(sql)
        setGrid()
        setData()
    End Sub

    Private Sub setGrid()
        With grdElektromedik.ColumnHeadersDefaultCellStyle
            .Alignment = DataGridViewContentAlignment.MiddleCenter
            .Font = New Font(.Font.FontFamily, .Font.Size, _
              .Font.Style Or FontStyle.Bold, GraphicsUnit.Point)
            .ForeColor = Color.Gold

        End With

    End Sub

    Private Sub FormMsSales_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        emptyField()
        ubahAktif(False)
        viewAllData("", "")
        setCmbCari()
    End Sub

    Private Sub generateCode()
        Dim code As String = "EM"
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

        Dim reader = getCode("MsElektromedik", "KdElektromedik")

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

    Private Sub DataGridView1_CellClick(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewCellEventArgs) Handles grdElektromedik.CellClick, grdElektromedik.CellClick
        If e.RowIndex >= 0 And e.RowIndex <= grdElektromedik.Rows.Count - 1 Then
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
            txtTarif.Focus()
        End If
    End Sub

    Private Sub btnAdd_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnAdd.Click
        emptyField()
        ubahAktif(True)
        generateCode()
        txtKd.Focus()
    End Sub

    Private Sub btnUpdate_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnUpdate.Click
        If grdElektromedik.RowCount Then
            ubahAktif(True)
            txtNama.Focus()
        Else
            msgInfo("Data tidak ditemukan.")
        End If
    End Sub

    Private Sub btnDelete_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnDelete.Click
        If grdElektromedik.RowCount Then
            Dim selected_cell = grdElektromedik.CurrentRow.Cells("Call ID").Value
            If MsgBox("Anda yakin ingin menghapus Call ID " & selected_cell & "?", MsgBoxStyle.Question + MsgBoxStyle.YesNo, "Confirmation") = MsgBoxResult.Yes Then
                execute_update(" UPDATE mselektromedik SET " & _
                               " IsAktif = 0 " & _
                               " WHERE KdElektromedik = '" & selected_cell & "' ")
                msgInfo("Data berhasil dihapus")
                viewAllData("", "")
            End If
        Else
            msgInfo("Data tidak ditemukan.")
        End If
    End Sub

    Private Sub btnSave_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSave.Click
        Dim addQuery = ""
        Dim readUsedCode = execute_reader(" SELECT 1 FROM mselektromedik " & _
                                           " WHERE KdElektromedik = '" & txtKd.Text & "' " & _
                                           " AND KdElektromedik <> '" & LastKode & "' ")
        If txtKd.Text = "" Then
            msgInfo("Call ID harus diisi")
            txtKd.Focus()
        ElseIf readUsedCode.read Then
            msgInfo("Call ID sudah terdaftar. Silakan ganti yang lain.")
            txtKd.Focus()
        ElseIf txtNama.Text = "" Then
            msgInfo("Nama harus diisi")
            txtNama.Focus()
        ElseIf Not IsNumeric(txtTarif.Text) Then
            msgInfo("Tarif harus berupa angka")
            txtTarif.Focus()
        Else
            If LastKode = "" Then
                LastKode = txtKd.Text
            End If

            sql = " INSERT INTO mselektromedik ( " & _
                  "     KdElektromedik, NamaElektromedik, " & _
                  "     TarifPemeriksaan, " & _
                  "     Created " & _
                  " ) values( " & _
                  "     '" & Trim(LastKode) & "', '" & Trim(txtNama.Text) & "', " & _
                  "     '" & Val(txtTarif.Text) & "', " & _
                  "     now() " & _
                  " )  ON DUPLICATE KEY UPDATE " & _
                  " KdElektromedik = '" & Trim(txtKd.Text) & "', " & _
                  " NamaElektromedik = '" & Trim(txtNama.Text) & "', " & _
                  " TarifPemeriksaan = '" & Val(txtTarif.Text) & "' "
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

    Private Sub cmbCari_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmbCari.SelectedIndexChanged
        btnReset.PerformClick()
    End Sub

    Private Sub txtTarif_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txtTarif.KeyPress
        If Asc(e.KeyChar) <> Asc("-") And AscW(e.KeyChar) <> 13 And AscW(e.KeyChar) <> 8 And (Asc(e.KeyChar) < Asc("0") Or Asc(e.KeyChar) > Asc("9")) Then
            e.KeyChar = Nothing
        End If
        If AscW(e.KeyChar) = 13 And txtTarif.Text <> "" Then
            btnSave.PerformClick()
        End If
    End Sub
End Class
