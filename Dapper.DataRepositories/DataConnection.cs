using System;
using System.Data;

namespace Dapper.DataRepositories
{
    public class DataConnection : IDisposable
    {
        #region Properties

        /// <summary>
        /// 
        /// </summary>
        private IDbConnection _connection;

        /// <summary>
        /// 
        /// </summary>
        protected IDbConnection Connection
        {
            get
            {
                if (_connection.State != ConnectionState.Open && _connection.State != ConnectionState.Connecting)
                    _connection.Open();

                return _connection;
            }
        }

        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connection"></param>
        public DataConnection(IDbConnection connection)
        {
            _connection = connection;
        }

        /// <summary>
        /// Close the connection if this is open
        /// </summary>
        public void Dispose()
        {
            if (_connection != null && _connection.State != ConnectionState.Closed)
                _connection.Close();
        }
    }
}
