using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;
using System.Text;

namespace SPlib.Core
{
    public class Query : IDisposable
    {
        private readonly SqlConnection _connection;
        private SqlCommand _command;
        private SqlTransaction _transaction;

        public Query(string connectionString)
        {
            _connection = new SqlConnection(connectionString);
        }

        public void BeginTransaction()
        {
            if (_connection.State == ConnectionState.Closed)
                _connection.Open();
            _transaction = _connection.BeginTransaction();
        }
        public void CommitTransaction()
        {
            _transaction.Commit();
            if (_connection.State == ConnectionState.Open)
                _connection.Close();
        }
        public void RollBackTransaction()
        {
            _transaction.Rollback();
            if (_connection.State == ConnectionState.Open)
                _connection.Close();
        }

        public List<T> Select<T>(string sql, object dbParams) where T : new()
        {

            Type type = typeof(T);
            IEnumerable<PropertyInfo> properties = type.GetProperties();

            IEnumerable<PropertyInfo> objProperties = dbParams.GetType().GetProperties();
            _command = new SqlCommand()
            {
                Connection = _connection,
                Transaction = _transaction,
                CommandText = sql
            };

            if (dbParams != null)
            {
                foreach (var property in objProperties)
                {
                    if (property.PropertyType == typeof(string))
                    {
                        SqlParameter sqlParameter = new SqlParameter(string.Concat("@", property.Name),
                            SqlDbType.NVarChar, property.Name.Length, ParameterDirection.Input,
                            true, 0, 0, null, DataRowVersion.Current,
                            property.GetValue(dbParams));
                        _command.Parameters.Add(sqlParameter);
                    }
                    else if (property.PropertyType != typeof(string))
                        _command.Parameters.AddWithValue(string.Concat("@", property.Name), property.GetValue(dbParams));
                }
            }

            List<T> list = new List<T>();
            try
            {
                if (_connection.State == ConnectionState.Closed) _connection.Open();
                SqlDataReader reader = _command.ExecuteReader();

                while (reader.Read())
                {
                    T t = new T();
                    foreach (var property in properties)
                    {
                        property.SetValue(t, reader[property.Name]);
                    }
                    list.Add(t);
                }
            }
            catch (Exception ex) { throw ex; }
            finally
            {
                if (_transaction == null)
                {
                    if (_connection.State == ConnectionState.Open) _connection.Close();
                }
            }
            return list;
        }

        private bool disposed;
        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    //dispose managed resources
                }
            }
            //dispose unmanaged resources
            disposed = true;
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
