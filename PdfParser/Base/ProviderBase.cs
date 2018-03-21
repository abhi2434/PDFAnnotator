using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace IdeaBridge.Data.Base
{
    public enum ProviderType : int
    {
        SQLCLIENT, OLEDB
    };

    public class ProviderBase
    {
        #region Static Variables

        private static Type[] _connectionTypes = { typeof(SqlConnection), typeof(OleDbConnection) };
        private static Type[] _commandTypes = { typeof(SqlCommand), typeof(OleDbCommand) };
        private static Type[] _dataAdapterTypes = { typeof(SqlDataAdapter), typeof(OleDbDataAdapter) };
        private static Type[] _dataParameterTypes = { typeof(SqlParameter), typeof(OleDbParameter) };
        private static Type[] _dataReader = { typeof(SqlDataReader), typeof(OleDbDataReader) };

        #endregion

        #region Class Variables

        private ProviderType _provider;

        #endregion

        #region Constructor

        public ProviderBase()
        {
            this.Provider = ProviderType.SQLCLIENT;
        }

        public ProviderBase(ProviderType provider)
        {
            this.Provider = provider;
        }

        #endregion

        #region Properties

        public ProviderType Provider
        {
            get { return this._provider; }
            set { this._provider = value; }
        }

        private int GetProvider
        {
            get { return (int)this._provider; }
        }

        #endregion

        #region Methods

        #region CreateConnection

        protected IDbConnection CreateConnection()
        {
            IDbConnection conn = null;

            try
            {
                conn = (IDbConnection)Activator.CreateInstance(_connectionTypes[this.GetProvider]);
            }
            catch (TargetInvocationException e)
            {
                throw new ApplicationException(e.InnerException.Message, e.InnerException);
            }

            return conn;
        }

        protected IDbConnection CreateConnection(string connectionString)
        {
            IDbConnection conn = this.CreateConnection();
            if (conn != null)
                conn.ConnectionString = connectionString;
            return conn;
        }

        #endregion

        #region CreateCommand

        public IDbCommand CreateCommand()
        {
            IDbCommand cmd = null;
            try
            {
                cmd = (IDbCommand)Activator.CreateInstance(_commandTypes[this.GetProvider]);
            }
            catch (TargetInvocationException e)
            {
                throw new SystemException(e.InnerException.Message, e.InnerException);
            }
            return cmd;
        }

        public IDbCommand CreateCommand(string cmdText)
        {
            IDbCommand cmd = this.CreateCommand();
            if (cmd != null)
                cmd.CommandText = cmdText;

            return cmd;
        }

        public IDbCommand CreateCommand(string cmdText, IDbConnection connection)
        {
            IDbCommand cmd = this.CreateCommand(cmdText);
            if (cmd != null)
                cmd.Connection = connection;
            return cmd;
        }

        public IDbCommand CreateCommand(string cmdText, string connectionString)
        {
            return this.CreateCommand(cmdText, this.CreateConnection(connectionString));
        }

        public IDbCommand CreateCommand(string cmdText, IDbConnection connection, IDbTransaction transaction)
        {
            IDbCommand cmd = this.CreateCommand(cmdText, connection);
            if (cmd != null)
                cmd.Transaction = transaction;
            return cmd;
        }

        #endregion

        #region CreateDataAdapter

        public IDbDataAdapter CreateDataAdapter()
        {
            IDbDataAdapter da = null;

            try
            {
                da = (IDbDataAdapter)Activator.CreateInstance(_dataAdapterTypes[this.GetProvider]);
            }
            catch (TargetInvocationException e)
            {
                throw new ApplicationException(e.InnerException.Message, e.InnerException);
            }
            return da;
        }

        protected IDbDataAdapter CreateDataAdapter(IDbCommand selectCommand)
        {
            IDbDataAdapter da = this.CreateDataAdapter();
            if (da != null)
                da.SelectCommand = selectCommand;
            return da;
        }

        protected IDbDataAdapter CreateDataAdapter(string cmdText, IDbConnection connection)
        {
            return this.CreateDataAdapter(this.CreateCommand(cmdText, connection));
        }

        protected IDbDataAdapter CreateDataAdapter(string cmdText, string connectionString)
        {
            return this.CreateDataAdapter(this.CreateCommand(cmdText, connectionString));
        }

        #endregion

        #region CreateDataParameter

        public IDbDataParameter CreateDataParameter()
        {
            IDbDataParameter param = null;

            try
            {
                param = (IDbDataParameter)Activator.CreateInstance(_dataParameterTypes[this.GetProvider]);
            }
            catch (TargetInvocationException e)
            {
                throw new ApplicationException(e.InnerException.Message, e.InnerException);
            }
            return param;
        }

        public IDbDataParameter CreateDataParameter(string parameterName)
        {
            IDbDataParameter param = this.CreateDataParameter();
            if (param != null)
                param.ParameterName = parameterName;

            return param;
        }

        public IDbDataParameter CreateDataParameter(string parameterName, object val)
        {
            IDbDataParameter param = this.CreateDataParameter(parameterName);
            if (param != null)
                param.Value = val;

            return param;
        }

        public IDbDataParameter CreateDataParameter(string parameterName, DbType dataType)
        {
            IDbDataParameter param = this.CreateDataParameter(parameterName);
            if (param != null)
            {
                param.ParameterName = parameterName;
                param.DbType = dataType;
            }
            return param;
        }

        public IDbDataParameter CreateDataParameter(string parameterName, DbType dataType, int size)
        {
            IDbDataParameter param = this.CreateDataParameter();
            if (param != null)
            {
                param.ParameterName = parameterName;
                param.DbType = dataType;
                param.Size = size;
            }
            return param;
        }

        public IDbDataParameter CreateDataParameter(string parameterName, DbType dataType, int size, string sourceColumn)
        {
            IDbDataParameter param = this.CreateDataParameter();
            if (param != null)
            {
                param.ParameterName = parameterName;
                param.DbType = dataType;
                param.Size = size;
                param.SourceColumn = sourceColumn;
            }
            return param;
        }

        #endregion

        #endregion
    }
}
