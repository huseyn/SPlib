using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;
using System.Text;

namespace SPlib.Core
{
    public class Procedural : IDisposable
    {
        private readonly SqlConnection _connection;
        private SqlCommand _command;
        public Procedural(string connectionString)
        {
            _connection = new SqlConnection(connectionString);
        }

        public int Execute<TInput>(string procName, TInput input) where TInput : class
        {
            int result = 0;
            _command = new SqlCommand()
            {
                CommandText = procName,
                Connection = _connection,
                CommandType = CommandType.StoredProcedure
            };
            PropertyInfo[] properties = typeof(TInput).GetProperties();
            foreach (PropertyInfo property in properties)
                _command.Parameters.AddWithValue(string.Concat("@", property.Name), property.GetValue(input));

            try
            {
                if (_connection.State == ConnectionState.Closed)
                    _connection.Open();
                result = _command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {

                throw ex;
            }
            finally
            {
                if (_connection.State == ConnectionState.Open)
                    _connection.Close();
            }

            return result;
        }

        public int ExecuteScalar<TInput>(string procName,TInput input)
        {
            int result = 0;
            _command = new SqlCommand()
            {
                CommandText = procName,
                Connection = _connection,
                CommandType = CommandType.StoredProcedure
            };
            PropertyInfo[] properties = typeof(TInput).GetProperties();
            foreach (PropertyInfo property in properties)
                _command.Parameters.AddWithValue(string.Concat("@", property.Name), property.GetValue(input));

            try
            {
                if (_connection.State == ConnectionState.Closed)
                    _connection.Open();
                result = Convert.ToInt32(_command.ExecuteScalar().ToString());
            }
            catch (Exception ex)
            {

                throw ex;
            }
            finally
            {
                if (_connection.State == ConnectionState.Open)
                    _connection.Close();
            }

            return result;
        }

        public List<TOutput> Select<TInput, TOutput>(string procName, TInput input) where TOutput : new()
        {
            _command.CommandText = procName;
            PropertyInfo[] inputProperties = typeof(TInput).GetProperties();
            foreach (PropertyInfo property in inputProperties)
            {
                object obj = property.GetValue(input);
                _command.Parameters.AddWithValue(string.Concat("@", property.Name), property.GetValue(input));
            }

            List<TOutput> result = new List<TOutput>();
            try
            {
                PropertyInfo[] outputProperties = typeof(TOutput).GetProperties();
                if (_connection.State == ConnectionState.Closed)
                    _connection.Open();

                SqlDataReader reader = _command.ExecuteReader();

                while (reader.Read())
                {
                    TOutput output = new TOutput();
                    foreach (PropertyInfo property in outputProperties)
                        property.SetValue(output, reader[property.Name]);
                    result.Add(output);
                }
                reader.Close();

            }
            catch (Exception ex)
            {
                throw ex; // TODO
            }
            finally
            {
                if (_connection.State == ConnectionState.Open)
                    _connection.Close();
            }

            return result;
        }

        public bool DeleteById(string procName, int id)
        {
            _command = new SqlCommand()
            {
                CommandText = procName,
                Connection = _connection,
                CommandType = CommandType.StoredProcedure
            };
            try
            {
                if (_connection.State == ConnectionState.Closed)
                    _connection.Open();

                _command.Parameters.AddWithValue("@Id", id);


                if (_command.ExecuteNonQuery() > 0) return true;
                return false;
            }
            catch (Exception ex)
            {

                throw ex;
            }
            finally
            {
                if (_connection.State == ConnectionState.Open)
                    _connection.Close();
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
