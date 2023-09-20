Imports MySql.Data.MySqlClient
Module config
    Dim server As String = "localhost"
    Dim user As String = "root"
    Dim db As String = "db_budget"
    Dim pass As String = ""
    Dim strconection As String = "server=" & server & " ;user id=" & user & ";password=" & pass & ";database=" & db & ";sslMode=none"
    Dim cmd As MySqlCommand
    Dim da As MySqlDataAdapter
    Public dt As DataTable
    Public sql As String
    Private Function mysqlcon() As MySqlConnection
        Return New MySqlConnection(strconection)
    End Function
    Public con As MySqlConnection = mysqlcon()
    Public Sub executeQuery(sql As String)
        Try
            con.Open()
            cmd = New MySqlCommand
            With cmd
                .Connection = con
                .CommandText = sql
                .ExecuteNonQuery()
            End With
        Catch ex As Exception
            MsgBox(ex.Message)
        Finally
            con.Close()
        End Try
    End Sub
    Public Sub loadResultList(sql As String, dtg As DataGridView)
        Try
            cmd = New MySqlCommand
            With cmd
                .Connection = con
                .CommandText = sql
            End With
            da = New MySqlDataAdapter
            da.SelectCommand = cmd
            dt = New DataTable
            da.Fill(dt)
            dtg.DataSource = dt
            dtg.Columns(0).Visible = False
        Catch ex As Exception
            MsgBox(ex.Message)
        Finally
            con.Close()
            da.Dispose()
        End Try
    End Sub
    Public Function loadSingleResult(sql As String)
        Dim maxrow = 0
        Try
            cmd = New MySqlCommand
            With cmd
                .Connection = con
                .CommandText = sql
            End With
            da = New MySqlDataAdapter
            da.SelectCommand = cmd
            dt = New DataTable
            da.Fill(dt)
            maxrow = dt.Rows.Count
        Catch ex As Exception
            MsgBox(ex.Message)
        Finally
            con.Close()
            da.Dispose()
        End Try
        Return maxrow
    End Function
End Module
