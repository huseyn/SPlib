using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Reflection;
using System.Text;
using System.Linq;
using System.Data;
using SPlib.Core.Helper;

namespace SPlib.Core
{
    public class NonProcedural : IDisposable
    {
        private readonly SqlConnection _connection;
        private SqlCommand _command;
        private SqlTransaction _transaction;

        public NonProcedural(string connectionString)
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

        public List<T> Select<T>(List<int> ids = null) where T : new()
        {
            Type type = typeof(T);

            _command = new SqlCommand()
            {
                Connection = _connection,
                Transaction = _transaction
            };

            IEnumerable<PropertyInfo> properties = type.GetProperties();

            _command.CommandText = StringCommand.SelectSql(type, ids);
            if (ids != null)
                for (int i = 0; i < ids.Count; i++)
                {
                    _command.Parameters.AddWithValue(string.Concat("@Id", i), ids[i]);
                }

            List<T> list = new List<T>();
            try
            {
                if (_connection.State == ConnectionState.Closed)
                    _connection.Open();
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
            catch (Exception ex)
            {

                throw ex;
            }
            finally
            {
                if (_transaction == null)
                {
                    if (_connection.State == ConnectionState.Open)
                        _connection.Close();
                }
            }


            return list;
        }
        public void Insert<T>(T input)
        {
            Type type = typeof(T);

            _command = new SqlCommand()
            {
                Connection = _connection,
                Transaction = _transaction
            };
            IEnumerable<PropertyInfo> properties = type.GetProperties();
            IEnumerable<PropertyInfo> propertiesInsert = EntityModifier.GetInsertProperties(type).ToList();
            foreach (PropertyInfo property in propertiesInsert)
                _command.Parameters.AddWithValue(string.Concat("@", property.Name), property.GetValue(input));

            _command.CommandText = StringCommand.InsertSql(type);
            try
            {
                if (_connection.State == ConnectionState.Closed)
                    _connection.Open();
                int id = (int)_command.ExecuteScalar();
                PropertyInfo property = properties.ToList()[0];
                property.SetValue(input, id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (_transaction == null)
                {
                    if (_connection.State == ConnectionState.Open)
                        _connection.Close();
                }
            }

        }

        public void Update<T>(T input)
        {
            Type type = typeof(T);

            _command = new SqlCommand()
            {
                Connection = _connection,
                Transaction = _transaction
            };
            IEnumerable<PropertyInfo> properties = type.GetProperties();

            foreach (PropertyInfo property in properties)
                _command.Parameters.AddWithValue(string.Concat("@", property.Name), property.GetValue(input));

            _command.CommandText = StringCommand.UpdateSql(type);
            try
            {
                if (_connection.State == ConnectionState.Closed)
                    _connection.Open();
                _command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (_transaction == null)
                {
                    if (_connection.State == ConnectionState.Open)
                        _connection.Close();
                }
            }
        }

        public void Delete<T>(T t)
        {
            Type type = typeof(T);
            _command = new SqlCommand()
            {
                Connection = _connection,
                Transaction = _transaction,
                CommandText = StringCommand.DeleteSql(type)
            };

            try
            {
                int id = EntityModifier.GetIdValue(t);
                _command.Parameters.AddWithValue("@Id", id);
                if (_connection.State == ConnectionState.Closed)
                    _connection.Open();
                _command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            {
                if (_transaction == null)
                {
                    if (_connection.State == ConnectionState.Open)
                        _connection.Close();
                }
            }

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
