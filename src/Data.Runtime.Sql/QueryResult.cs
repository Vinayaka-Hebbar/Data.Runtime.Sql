using System;
using System.Runtime.Serialization;

namespace Data.Runtime.Sql
{
    /// <summary>
    /// Result of Non Query
    /// </summary>
    [DataContract]
    public
#if LATEST_VS
    readonly
#endif
        struct QueryResult
    {
        public QueryResult(string message, int rows, OperationStatus status)
        {
            Message = message;
            AffectedRows = rows;
            Status = status;
        }

        [DataMember(Name = "message")]
        public string Message { get; }

        [DataMember(Name = "affectedRows")]
        public int AffectedRows { get; }

        [IgnoreDataMember]
        public OperationStatus Status { get; }

        public bool IsSuccess => Status == OperationStatus.Success;

        public static QueryResult Success(int rows)
        {
            return new QueryResult(SqlTableRef.SuccessMessage, rows, OperationStatus.Success);
        }
        public static QueryResult Fail
        {
            get
            {
                return new QueryResult(SqlTableRef.FailMessage, -1, OperationStatus.Error);
            }
        }


        public static QueryResult Error(Exception ex)
        {
            return new QueryResult(ex.Message, -1, OperationStatus.Exception);
        }

        public static QueryResult ErrorMessage(string error)
        {
            return new QueryResult(error, -1, OperationStatus.Error);
        }
    }

    public enum OperationStatus
    {
        Success,
        Error,
        Exception
    }
}