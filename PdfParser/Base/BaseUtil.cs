using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdeaBridge.Data.Base
{
    public class DbUtil
    {
        public DbUtil(string connectionString)
        {
            this.ConnectionString = connectionString;
        }

        internal string ConnectionString { get; private set; }

        private DatabaseFactory _dbBridge;
        public DatabaseFactory DbBridge
        {
            get
            {
                this._dbBridge = this._dbBridge ?? new DatabaseFactory(this.ConnectionString);
                return this._dbBridge;
            }
        }
        
    }
}
