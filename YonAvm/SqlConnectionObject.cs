﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YonAvm
{
    class SqlConnectionObject
    {
        public DataTable GetDataTableConnectionSql(string q, SqlConnection conn)
        {
            SqlDataAdapter adapter = new SqlDataAdapter(q, conn);
            DataTable dataTable = new DataTable();
            adapter.Fill(dataTable);
            return dataTable;
        }

        public string GetValueConnectionSql(string q, SqlConnection conn)
        {
            SqlDataAdapter adapter = new SqlDataAdapter(q, conn);
            DataTable dataTable = new DataTable();
            adapter.Fill(dataTable);
            if (dataTable.Rows.Count > 0)
            {
                return dataTable.Rows[0][0].ToString();
            }
            else
            {
                return "";
            }
        }
        public DataTable GetDataTableConnectionString(string q, string con)
        {
            using (SqlConnection connection = new SqlConnection(con))
            {
                SqlCommand command = new SqlCommand(q, connection);
                SqlDataAdapter adapter = new SqlDataAdapter(command);
                DataTable dataTable = new DataTable();

                connection.Open();
                adapter.Fill(dataTable);
                connection.Close();

                return dataTable;
            }
        }
        public int insertData(string query, SqlConnection sql)
        {
            SqlCommand cmd = new SqlCommand(query, sql);
            if (sql.State == ConnectionState.Closed)
            {
                sql.Open();
            }
            return cmd.ExecuteNonQuery();
        }

    }
}
