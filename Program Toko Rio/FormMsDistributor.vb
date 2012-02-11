Public Class FormMsDistributor

    Private Sub main_tool_strip_ItemClicked(ByVal sender As System.Object, ByVal e As System.Windows.Forms.ToolStripItemClickedEventArgs) Handles main_tool_strip.ItemClicked

    End Sub
    Private Sub FormMsDistributor_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        sql = " select * " & _
              " from msdistributor " & _
              " Where 1 "
        DataGridView1.DataSource = execute_datatable(sql)
    End Sub
End Class