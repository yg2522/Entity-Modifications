using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityNamespace
{
    public partial interface IEntityName
    {
        Guid ContextId { get; }
        void InitializeContext(int commandTimeout = -1);
        void ExecuteBulkCopyCommand(int bulkCopyTimeout, int batchSize, string destinationTableName, DataTable datatable);
        void ExecuteBulkCopyCommand(ISqlBulkCopyDataReader bulkCopy);
        void ExecuteBulkCopyCommand(ISqlBulkCopyDataRows bulkCopy);
        void ExecuteBulkCopyCommand(ISqlBulkCopyDataTable bulkCopy);
        Task ExecuteBulkCopyCommandAsync(ISqlBulkCopyDataReader bulkCopy);
        Task ExecuteBulkCopyCommandAsync(ISqlBulkCopyDataRows bulkCopy);
        Task ExecuteBulkCopyCommandAsync(ISqlBulkCopyDataTable bulkCopy);
        IDbConnection GetConnection();
        void ExecuteSqlCommand(string p, params object[] o);
        void ExecuteSqlCommand(string p);
        string ConnectionString { get; set; }
    }

    public interface ISqlBulkCopyDataReader
    {
        int BulkCopyTimeout { get; set; }
        int BatchSize { get; set; }
        string DestinationTableName { get; set; }
        IDataReader DataReader { get; set; }
    }
    public interface ISqlBulkCopyDataRows
    {
        int BulkCopyTimeout { get; set; }
        int BatchSize { get; set; }
        string DestinationTableName { get; set; }
        DataRow[] DataRows { get; set; }
    }
    public interface ISqlBulkCopyDataTable
    {
        int BulkCopyTimeout { get; set; }
        int BatchSize { get; set; }
        string DestinationTableName { get; set; }
        DataTable DataTable { get; set; }
        DataRowState? DataRowState { get; set; }
    }
}
