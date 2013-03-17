using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.IO;
using YAFImporter;

/// <summary>
/// Summary description for ADONETBase
/// </summary>
public class ADONETBase {
    public string FileName { get; set; }
    private SqlConnection _conn = null;

    /// <summary>
    /// Executes an action using an SQL connection, then gracefully closes the connection.
    /// </summary>
    /// <param name="action">The action to invoke.</param>
    /// <summary>
    /// Executes an action using an SQL connection, then gracefully closes the connection.
    /// </summary>
    /// <param name="action">The action to invoke.</param>
    protected virtual void Execute(Action<SqlConnection> action) {
        try {
            if(_conn ==null)
                _conn = GetConnection();
            if (_conn.State != ConnectionState.Open)
                _conn.Open();
            action.Invoke(_conn);
        } finally {
            if (_conn != null && _conn.State == System.Data.ConnectionState.Open) {
                try {
                    _conn.Close();
                } catch {
                }
            }
        }
    }
    
    /// <summary>
    /// Opens an Sql connection to the configured database file.
    /// </summary>
    /// <returns>An Sql connection to the configured database file.</returns>
    protected virtual SqlConnection GetConnection() {
        return GetConnection(FileName);
    }

    /// <summary>
    /// Opens an Sql connection to a file.
    /// </summary>
    /// <param name="databasePath">The database path.</param>
    /// <returns>An Sql connection to the specified file.</returns>
    protected SqlConnection GetConnection(string databasePath) {
        if (_conn == null) {
            _conn = new SqlConnection(string.Format(Constants.SqlConnectionString, databasePath));
        }
        if (_conn.State != ConnectionState.Open)
            _conn.Open();
        return _conn;
    }

    ~ADONETBase()
    {
        Close();
    }

    private void Close() {
        if (_conn != null) {
            _conn.Dispose();
            _conn = null;
        }
    }

    public bool ColumnExists(IDataReader reader, string columnName) {
        for (int i = 0; i < reader.FieldCount; i++) {
            if (reader.GetName(i) == columnName) {
                return true;
            }
        }
        return false;
    }

    public IEnumerable<string> GetColumns(IDataReader reader)
    {
        List<string> cols = new List<string>();
        for (int i = 0; i < reader.FieldCount; i++) {
            cols.Add(reader.GetName(i));
        }
        return cols;
    }

    protected static object SafeNull(object obj, object valueIfNull)
    {
        if (obj == null || obj == DBNull.Value) {
            return valueIfNull;
        } else {
            return obj;
        }
    }
}
