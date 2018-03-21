using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdeaBridge.Data.Base
{
    public class DatabaseFactory : ProviderBase
    {
        #region Class Variables
        private string _connectionString;
        private IDbConnection _transConnection;
        private IDbTransaction _transaction;
        private bool _inTransaction = false;
        private string _defaultLocation = "SYSTEM";
        private long _defaultUserId = 1;
        private string _defaultUserName = "admin";
        #endregion

        #region Constructors

        public DatabaseFactory(string connstring)
            : base()
        {
            this.ConnectionString = connstring;
        }

        public DatabaseFactory(string connstring, ProviderType provider)
            : base(provider)
        {
            this.ConnectionString = connstring;
        }

        #endregion

        #region Properties

        public string ConnectionString
        {
            get { return this._connectionString; }
            set { this._connectionString = value; }
        }

        public string DefaultLocation
        {
            get { return this._defaultLocation; }
            set { this._defaultLocation = value; }
        }

        public string DefaultUserName
        {
            get { return this._defaultUserName; }
            set { this._defaultUserName = value; }
        }

        public long DefaultUserId
        {
            get { return this._defaultUserId; }
            set { this._defaultUserId = value; }
        }

        #endregion

        #region Methods

        #region Connection Methods

        public IDbConnection GetConnection()
        {
            return this.GetConnection(false);
        }


        public IDbConnection GetConnection(bool openConnection)
        {
            try
            {
                if (this._inTransaction)
                    return this._transConnection;
                else
                {
                    IDbConnection con = base.CreateConnection(this.ConnectionString);
                    if (openConnection) con.Open();
                    return con;
                }
            }
            catch (Exception ex) { throw new ApplicationException(ex.Message, ex.InnerException); }
        }


        public void CompleteConnection(IDbConnection connection)
        {
            try
            {
                if (!this._inTransaction)
                    connection.Close();
            }
            catch (Exception ex) { throw new ApplicationException(ex.Message, ex.InnerException); }
        }


        #endregion

        #region Transaction Methods

        public IDbTransaction BeginTransaction()
        {
            try
            {
                if (this._inTransaction)
                    throw new ApplicationException("Already in a transaction");

                this._inTransaction = true;
                this._transConnection = base.CreateConnection(this.ConnectionString);
                this._transConnection.Open();
                this._transaction = this._transConnection.BeginTransaction();
                return this._transaction;
            }
            catch (Exception ex) { throw new ApplicationException(ex.Message, ex.InnerException); }
        }


        public void Commit()
        {
            try
            {
                if (!this._inTransaction)
                    throw new ApplicationException("No transaction in progress");

                this._transaction.Commit();
                this._inTransaction = false;
                this._transConnection.Close();
            }
            catch (Exception ex) { throw new ApplicationException(ex.Message, ex.InnerException); }
        }


        public void Rollback()
        {
            try
            {
                if (!this._inTransaction)
                    throw new ApplicationException("No transaction in progress");

                this._transaction.Rollback();
                this._inTransaction = false;
                this._transConnection.Close();
            }
            catch (Exception ex) { throw new ApplicationException(ex.Message, ex.InnerException); }
        }


        #endregion

        #region Command Methods

        public IDbCommand GetCommand(string cmdText)
        {
            return this.GetCommand(cmdText, false);
        }


        public IDbCommand GetCommand(string cmdText, bool openConnection)
        {
            try
            {
                IDbCommand cmd = base.CreateCommand(cmdText, this.GetConnection(openConnection));
                if (this._inTransaction)
                    cmd.Transaction = this._transaction;

                return cmd;
            }
            catch (Exception ex) { throw new ApplicationException(ex.Message, ex.InnerException); }
        }


        #endregion

        #region Adapter Methods

        public IDbDataAdapter GetDataAdapter(string cmdText)
        {
            return this.GetDataAdapter(cmdText, false);
        }

        public IDbDataAdapter GetDataAdapter(string cmdText, bool openConnection)
        {
            try
            {
                IDbCommand cmd = base.CreateCommand(cmdText, this.GetConnection(openConnection));
                if (this._inTransaction)
                    cmd.Transaction = this._transaction;
                return base.CreateDataAdapter(cmd);
            }
            catch (Exception ex) { throw new ApplicationException(ex.Message, ex.InnerException); }
        }

        public IDbDataAdapter GetDataAdapter(IDbCommand cmd)
        {
            try
            {
                cmd.Connection = this.GetConnection();
                if (this._inTransaction)
                    cmd.Transaction = this._transaction;
                return base.CreateDataAdapter(cmd);
            }
            catch (Exception ex) { throw new ApplicationException(ex.Message, ex.InnerException); }
        }

        #endregion

        #region Reader Methods

        public IDataReader GetReader(string cmdText)
        {
            try
            {
                IDbCommand command = this.GetCommand(cmdText, true);

                if (this._inTransaction)
                    return command.ExecuteReader();
                else
                    return command.ExecuteReader(System.Data.CommandBehavior.CloseConnection);
            }
            catch (Exception ex) { throw new ApplicationException(ex.Message, ex.InnerException); }
        }


        #endregion

        #region Execute Methods

        public object ExecuteScalar(IDbCommand cmd)
        {
            try
            {
                cmd.Connection = this.GetConnection(true);
                if (this._inTransaction && cmd.Transaction == null)
                    cmd.Transaction = this._transaction;

                object scalar = cmd.ExecuteScalar();
                this.CompleteConnection(cmd.Connection);
                return scalar;
            }
            catch (Exception ex) { throw new ApplicationException(ex.Message, ex.InnerException); }
        }


        public object ExecuteScalar(string cmdText)
        {
            try
            {
                return this.ExecuteScalar(this.GetCommand(cmdText, true));
            }
            catch (Exception ex) { throw new ApplicationException(ex.Message, ex.InnerException); }
        }

        public DataRow ExecuteRecord(IDbCommand cmd)
        {
            try
            {
                cmd.Connection = this.GetConnection(true);
                if (this._inTransaction && cmd.Transaction == null)
                    cmd.Transaction = this._transaction;

                var row = this.ExecuteTable(cmd);
                this.CompleteConnection(cmd.Connection);
                return row.Rows[0];
            }
            catch (Exception ex) { throw new ApplicationException(ex.Message, ex.InnerException); }
        }

        //public object ExecuteScalar(string cmdText)
        //{
        //    try
        //    {
        //        return this.ExecuteRecord(this.GetCommand(cmdText, true));
        //    }
        //    catch (Exception ex) { throw; }
        //}

        public void ExecuteNonQuery(string cmdText)
        {
            try
            {
                this.ExecuteNonQuery(this.GetCommand(cmdText, true));
            }
            catch (Exception ex) { throw new ApplicationException(ex.Message, ex.InnerException); }
        }

        public void ExecuteNonQuery(IDbCommand cmd)
        {
            try
            {
                if (cmd.Connection.State == ConnectionState.Closed)
                    cmd.Connection.Open();
                cmd.ExecuteNonQuery();
                this.CompleteConnection(cmd.Connection);
            }
            catch (Exception ex) { throw new ApplicationException(ex.Message, ex.InnerException); }
        }

        public List<object> ExecuteArray(string cmdText)
        {
            try
            {
                List<object> arrObjects = new List<object>();
                IDataReader rdr = this.GetReader(cmdText);
                while (rdr.Read())
                    arrObjects.Add(rdr.GetValue(0));

                rdr.Close();
                return arrObjects;
            }
            catch (Exception ex) { throw new ApplicationException(ex.Message, ex.InnerException); }
        }


        public DataTable ExecuteTable(string cmdText)
        {
            return this.FillTable(cmdText);
        }


        public DataTable ExecuteTable(string cmdText, string tableName)
        {
            return this.FillTable(cmdText, tableName);
        }


        public DataTable ExecuteTable(string cmdText, DataTable dt)
        {
            return this.FillTable(cmdText, dt);
        }


        public DataTable ExecuteTable(IDbCommand cmd)
        {
            return this.FillTable(cmd);
        }


        #endregion

        #region Fill Methods

        public DataTable FillTable(IDbCommand cmd)
        {
            return this.FillTable(cmd, new DataTable());
        }

        public DataTable FillTable(IDbCommand cmd, string tableName)
        {
            return this.FillTable(cmd, new DataTable(tableName));
        }

        public DataTable FillTable(IDbCommand cmd, DataTable dt)
        {
            try
            {
                cmd.Connection = this.GetConnection();
                switch (this.Provider)
                {
                    case ProviderType.OLEDB:
                        OleDbDataAdapter oOledbAdp = ((OleDbDataAdapter)this.GetDataAdapter(cmd));
                        oOledbAdp.Fill(dt);
                        break;
                    case ProviderType.SQLCLIENT:
                        SqlDataAdapter oSqlAdp = (SqlDataAdapter)this.GetDataAdapter(cmd);
                        oSqlAdp.Fill(dt);
                        break;
                   
                }
                this.CompleteConnection(cmd.Connection);
                return dt;
            }
            catch (Exception ex) { throw new ApplicationException(ex.Message, ex.InnerException); }
        }

        public DataSet FillDataset(IDbCommand cmd)
        {
            try
            {
                DataSet ds = new DataSet();
                cmd.Connection = this.GetConnection();
                switch (this.Provider)
                {
                    case ProviderType.OLEDB:
                        OleDbDataAdapter oOledbAdp = ((OleDbDataAdapter)this.GetDataAdapter(cmd));
                        oOledbAdp.Fill(ds);
                        break;
                    case ProviderType.SQLCLIENT:
                        SqlDataAdapter oSqlAdp = (SqlDataAdapter)this.GetDataAdapter(cmd);
                        oSqlAdp.Fill(ds);
                        break;
                   
                }
                this.CompleteConnection(cmd.Connection);
                return ds;
            }
            catch (Exception ex) { throw new ApplicationException(ex.Message, ex.InnerException); }
        }

        public DataSet FillDataset(IDbCommand cmd, DataSet ds)
        {
            try
            {
                cmd.Connection = this.GetConnection();
                switch (this.Provider)
                {
                    case ProviderType.OLEDB:
                        OleDbDataAdapter oOledbAdp = ((OleDbDataAdapter)this.GetDataAdapter(cmd));
                        oOledbAdp.Fill(ds);
                        break;
                    case ProviderType.SQLCLIENT:
                        SqlDataAdapter oSqlAdp = (SqlDataAdapter)this.GetDataAdapter(cmd);
                        oSqlAdp.Fill(ds);
                        break;
                  
                }
                this.CompleteConnection(cmd.Connection);
                return ds;
            }
            catch (Exception ex) { throw new ApplicationException(ex.Message, ex.InnerException); }
        }

        public DataTable FillTable(string cmdText)
        {
            return this.FillTable(this.GetCommand(cmdText));
        }

        public DataTable FillTable(string cmdText, string tableName)
        {
            return this.FillTable(this.GetCommand(cmdText), tableName);
        }

        public DataTable FillTable(string cmdText, DataTable dt)
        {
            return this.FillTable(this.GetCommand(cmdText), dt);
        }

        public void FillTable(DataSet ds, string tableName)
        {
            string cmdtext = "Select * From " + tableName;
            this.FillTable(cmdtext, ds.Tables[tableName]);
        }

        public void FillTable(DataSet ds)
        {
            string cmdtext = "Select * From " + ds.Tables[0].TableName;
            this.FillTable(cmdtext, ds.Tables[0]);
        }

        #endregion

        #region Update Methods

        public int Update(IDbDataAdapter da, DataTable dt)
        {
            try
            {
                int count = 0;
                switch (this.Provider)
                {
                    case ProviderType.OLEDB:
                        OleDbDataAdapter oOledbAdp = (OleDbDataAdapter)da;
                        count = oOledbAdp.Update(dt);
                        break;
                    case ProviderType.SQLCLIENT:
                        SqlDataAdapter oSqlAdp = (SqlDataAdapter)da;
                        count = oSqlAdp.Update(dt);
                        break;
                   
                }
                return count;
            }
            catch (Exception ex) { throw new ApplicationException(ex.Message, ex.InnerException); }
        }

        public int Update(IDbDataAdapter da, DataRow[] dr)
        {
            try
            {
                int count = 0;
                switch (this.Provider)
                {
                    case ProviderType.OLEDB:
                        OleDbDataAdapter oOledbAdp = (OleDbDataAdapter)da;
                        count = oOledbAdp.Update(dr);
                        break;
                    case ProviderType.SQLCLIENT:
                        SqlDataAdapter oSqlAdp = (SqlDataAdapter)da;
                        count = oSqlAdp.Update(dr);
                        break;
                 
                }
                return count;
            }
            catch (Exception ex) { throw new ApplicationException(ex.Message, ex.InnerException); }
        }

        public int UpdateTable(string selectCommandText, DataTable dt)
        {
            try
            {
                int count = 0;
                IDbDataAdapter da = this.GetDataAdapter(selectCommandText);
                switch (this.Provider)
                {
                    case ProviderType.OLEDB:
                        OleDbDataAdapter oOledbAdp = (OleDbDataAdapter)da;
                        OleDbCommandBuilder oledbcb = new OleDbCommandBuilder(oOledbAdp);
                        count = oOledbAdp.Update(dt);
                        break;
                    case ProviderType.SQLCLIENT:
                        SqlDataAdapter oSqlAdp = (SqlDataAdapter)da;
                        SqlCommandBuilder sqlcb = new SqlCommandBuilder(oSqlAdp);
                        count = oSqlAdp.Update(dt);
                        break;
                  
                }
                dt.AcceptChanges();
                return count;
            }
            catch (Exception ex) { throw new ApplicationException(ex.Message, ex.InnerException); }
        }

        public int UpdateTable(DataTable dt)
        {
            return this.UpdateTable(this.GetSelectCommandText(dt), dt);
        }

        public int UpdateInsertedRow(DataTable dt)
        {
            DataView dv = new DataView(dt, "", "", DataViewRowState.Added);
            return this.UpdateTable(this.GetSelectCommandText(dt), dv.ToTable());
        }

        public int UpdateInsertedRow(string selectCommandText, DataTable dt)
        {
            DataView dv = new DataView(dt, "", "", DataViewRowState.Added);
            return this.UpdateTable(selectCommandText, dv.ToTable());
        }

        public int UpdateDeletedRow(DataTable dt)
        {
            DataView dv = new DataView(dt, "", "", DataViewRowState.Deleted);
            return this.UpdateTable(this.GetSelectCommandText(dt), dv.ToTable());
        }

        public int UpdateDeletedRow(string selectCommandText, DataTable dt)
        {
            DataView dv = new DataView(dt, "", "", DataViewRowState.Deleted);
            return this.UpdateTable(selectCommandText, dv.ToTable());
        }

        public int UpdateModifiedRow(DataTable dt)
        {
            DataView dv = new DataView(dt, "", "", DataViewRowState.ModifiedOriginal);
            return this.UpdateTable(this.GetSelectCommandText(dt), dv.ToTable());
        }

        public int UpdateModifiedRow(string selectCommandText, DataTable dt)
        {
            DataView dv = new DataView(dt, "", "", DataViewRowState.ModifiedOriginal);
            return this.UpdateTable(selectCommandText, dv.ToTable());
        }

        public string GetSelectCommandText(DataTable dt)
        {
            string selectcommand = "";
            if (dt != null)
            {
                foreach (DataColumn dc in dt.Columns)
                    selectcommand += selectcommand == "" ? "Select " + dc.ColumnName : ", " + dc.ColumnName;
                selectcommand += " " + dt.TableName;
            }
            return selectcommand;
        }

        #endregion

        #endregion
    }
}
