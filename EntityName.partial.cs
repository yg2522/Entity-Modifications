namespace EntityNamespace
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Entity.Infrastructure;
    using System.Linq;
    using System.Data.Entity.Validation;
    using System.Threading.Tasks;

    /// <summary>
    /// Wiring for 
    /// EntityName
    /// </summary>
    /// <seealso cref="System.Data.Entity.DbContext" />
    /// <seealso cref="EntityNamespace.IEntityName" />
    public partial class EntityName
    {
        public EntityName(string connString)
            : base(connString)
        {
            this.ContextId = Guid.NewGuid();
#if DIAGNOSTICS
            logSql();
#endif
        }

        public bool AutoDetectChangedEnabled
        {
            get
            {
                return true;
            }
        }

        public string ConnectionString
        {
            get
            {
                return this.Database.Connection.ConnectionString;
            }
            set
            {
                this.Database.Connection.ConnectionString = value;
            }
        }

        public IDbConnection GetConnection()
        {
            return this.Database.Connection;
        }

        public void ExecuteSqlCommand(string p, params object[] os)
        {
            this.Database.ExecuteSqlCommand(p, os);
        }

        public void ExecuteSqlCommand(string p)
        {
            this.Database.ExecuteSqlCommand(p);
        }

        public Guid ContextId { get; set; }

        public void InitializeContext(int commandTimeout = -1)
        {
#if DIAGNOSTICS
            System.Diagnostics.Debug.WriteLine(string.Format("Initializing Context: {0}", this.ContextId));
#endif
            if (commandTimeout >= 0)
                ((IObjectContextAdapter)this).ObjectContext.CommandTimeout = commandTimeout;
            setTransactionIsolation();
        }

        private void setTransactionIsolation()
        {
            if (System.Transactions.Transaction.Current == null)
            {
                //open the connection
                if (this.Database.Connection.State != System.Data.ConnectionState.Open) this.Database.Connection.Open();

                //issue an isolation level command to prevent locks to prevent readers from blocking writers
                using (var cmd = new System.Data.SqlClient.SqlCommand("set transaction isolation level read uncommitted", (System.Data.SqlClient.SqlConnection)this.Database.Connection))
                {
                    cmd.ExecuteNonQuery();
                }
            }
        }

#if DIAGNOSTICS
        private void logSql()
        {
            this.Database.Log = s =>
            {
                if (string.IsNullOrWhiteSpace(s) || s.StartsWith("--")) return;
    
                System.Diagnostics.StackTrace stack = new System.Diagnostics.StackTrace();
    
                System.Reflection.MethodBase method = null;
                for (int i = 0; i < stack.GetFrames().Length; i++)
                {
                    string tempFullName = stack.GetFrame(i)?.GetMethod()?.ReflectedType?.FullName;
                    if (tempFullName != null && (tempFullName.StartsWith(this.GetType().Assembly.GetName().Name) && !tempFullName.StartsWith(this.GetType().FullName)))
                    {
                        method = stack.GetFrame(i).GetMethod();
                        break;
                    }
                }
                if (null != method)
                {
                    System.Diagnostics.Trace.WriteLine(string.Format("Entity SQL Source: {0}.{1}", method.ReflectedType.FullName, method.Name));
                }
                System.Diagnostics.Debug.WriteLine(string.Concat(s, Environment.NewLine));
            };
        }
#endif
        internal static EntityName Create()
        {
            return Create(-1);
        }

        internal static EntityName Create(string connectionString)
        {
            return Create(connectionString, -1);
        }

        internal static EntityName Create(int commandTimeout)
        {
            var context = new EntityName();
            if (commandTimeout >= 0) ((IObjectContextAdapter)context).ObjectContext.CommandTimeout = commandTimeout;
            return context;
        }

        internal static EntityName Create(string connectionString, int commandTimeout)
        {
            var context = new EntityName(connectionString);
            if (commandTimeout >= 0) ((IObjectContextAdapter)context).ObjectContext.CommandTimeout = commandTimeout;
            return context;
        }

        public void ExecuteBulkCopyCommand(ISqlBulkCopyDataReader bulkCopy)
        {
            using (System.Data.SqlClient.SqlBulkCopy sqlBulkCopy = new System.Data.SqlClient.SqlBulkCopy(this.ConnectionString))
            {
                sqlBulkCopy.BatchSize = bulkCopy.BatchSize;
                sqlBulkCopy.BulkCopyTimeout = bulkCopy.BulkCopyTimeout;
                sqlBulkCopy.DestinationTableName = bulkCopy.DestinationTableName;
                sqlBulkCopy.WriteToServer(bulkCopy.DataReader);
            }
        }
        public void ExecuteBulkCopyCommand(ISqlBulkCopyDataRows bulkCopy)
        {
            using (System.Data.SqlClient.SqlBulkCopy sqlBulkCopy = new System.Data.SqlClient.SqlBulkCopy(this.ConnectionString))
            {
                sqlBulkCopy.BatchSize = bulkCopy.BatchSize;
                sqlBulkCopy.BulkCopyTimeout = bulkCopy.BulkCopyTimeout;
                sqlBulkCopy.DestinationTableName = bulkCopy.DestinationTableName;
                sqlBulkCopy.WriteToServer(bulkCopy.DataRows);
            }
        }
        public void ExecuteBulkCopyCommand(ISqlBulkCopyDataTable bulkCopy)
        {
            using (System.Data.SqlClient.SqlBulkCopy sqlBulkCopy = new System.Data.SqlClient.SqlBulkCopy(this.ConnectionString))
            {
                sqlBulkCopy.BatchSize = bulkCopy.BatchSize;
                sqlBulkCopy.BulkCopyTimeout = bulkCopy.BulkCopyTimeout;
                sqlBulkCopy.DestinationTableName = bulkCopy.DestinationTableName;
                if(bulkCopy.DataRowState == null)
                    sqlBulkCopy.WriteToServer(bulkCopy.DataTable);
                else
                    sqlBulkCopy.WriteToServer(bulkCopy.DataTable, bulkCopy.DataRowState.Value);
            }
        }

        public async Task ExecuteBulkCopyCommandAsync(ISqlBulkCopyDataReader bulkCopy)
        {
            using (System.Data.SqlClient.SqlBulkCopy sqlBulkCopy = new System.Data.SqlClient.SqlBulkCopy(this.ConnectionString))
            {
                sqlBulkCopy.BatchSize = bulkCopy.BatchSize;
                sqlBulkCopy.BulkCopyTimeout = bulkCopy.BulkCopyTimeout;
                sqlBulkCopy.DestinationTableName = bulkCopy.DestinationTableName;
                await sqlBulkCopy.WriteToServerAsync(bulkCopy.DataReader);
            }
        }
        public async Task ExecuteBulkCopyCommandAsync(ISqlBulkCopyDataRows bulkCopy)
        {
            using (System.Data.SqlClient.SqlBulkCopy sqlBulkCopy = new System.Data.SqlClient.SqlBulkCopy(this.ConnectionString))
            {
                sqlBulkCopy.BatchSize = bulkCopy.BatchSize;
                sqlBulkCopy.BulkCopyTimeout = bulkCopy.BulkCopyTimeout;
                sqlBulkCopy.DestinationTableName = bulkCopy.DestinationTableName;
                await sqlBulkCopy.WriteToServerAsync(bulkCopy.DataRows);
            }
        }
        public async Task ExecuteBulkCopyCommandAsync(ISqlBulkCopyDataTable bulkCopy)
        {
            using (System.Data.SqlClient.SqlBulkCopy sqlBulkCopy = new System.Data.SqlClient.SqlBulkCopy(this.ConnectionString))
            {
                sqlBulkCopy.BatchSize = bulkCopy.BatchSize;
                sqlBulkCopy.BulkCopyTimeout = bulkCopy.BulkCopyTimeout;
                sqlBulkCopy.DestinationTableName = bulkCopy.DestinationTableName;
                if (bulkCopy.DataRowState == null)
                    await sqlBulkCopy.WriteToServerAsync(bulkCopy.DataTable);
                else
                    await sqlBulkCopy.WriteToServerAsync(bulkCopy.DataTable, bulkCopy.DataRowState.Value);
            }
        }

        protected override DbEntityValidationResult ValidateEntity(DbEntityEntry entityEntry, IDictionary<object, object> items)
        {
            if (entityEntry.State == System.Data.Entity.EntityState.Modified) //only check individual modified fields
            {
                var errorList = new List<DbValidationError>();
                foreach (var propertyName in entityEntry.CurrentValues.PropertyNames)
                {
                    if (entityEntry.Property(propertyName).IsModified && entityEntry.Property(propertyName).GetValidationErrors().Any())
                    {
                        foreach(var validationError in entityEntry.Property(propertyName).GetValidationErrors())
                        {
                            errorList.Add(new DbValidationError(validationError.PropertyName, validationError.ErrorMessage));
                        }
                    }
                        
                }
                return new DbEntityValidationResult(entityEntry, errorList);
            }
            return base.ValidateEntity(entityEntry, items);
        }

        public void ExecuteBulkCopyCommand(int bulkCopyTimeout, int batchSize, string destinationTableName, DataTable datatable)
        {
            using (System.Data.SqlClient.SqlBulkCopy sqlBulkCopy = new System.Data.SqlClient.SqlBulkCopy(this.ConnectionString))
            {
                sqlBulkCopy.BatchSize = batchSize;
                sqlBulkCopy.BulkCopyTimeout = bulkCopyTimeout;
                sqlBulkCopy.DestinationTableName = destinationTableName;
                sqlBulkCopy.WriteToServer(datatable);
            }
        }
    }
}
