using System;
using System.Runtime.Serialization;

namespace Data.Runtime.Sql
{
    /// <summary>
    /// Result of Non Query
    /// </summary>
    [DataContract]
    public struct QueryResult
    {
        const string SuccessMessage = "Success";
        const string FailMessage = "Success";

        [DataMember(Name = "message")]
        public string StatusMessage { get; }

        [DataMember(Name = "affectedRows")]
        public int AffectedRows { get; }

        [IgnoreDataMember]
        public QueryStatus Status { get; }

        public bool IsSuccess => Status == QueryStatus.Success;

        public static QueryResult Success(int rows)
        {
            return new QueryResult(SuccessMessage, rows, QueryStatus.Success);
        }
        public static QueryResult Fail
        {
            get
            {
                return new QueryResult(FailMessage, -1, QueryStatus.Error);
            }
        }


        public QueryResult(string statusMessage, int rows, QueryStatus status)
        {
            StatusMessage = statusMessage;
            AffectedRows = rows;
            Status = status;
        }

        public static QueryResult Error(Exception ex)
        {
            return new QueryResult(ex.Message, -1, QueryStatus.Exception);
        }

        public static QueryResult ErrorMessage(string error)
        {
            return new QueryResult(error, -1, QueryStatus.Error);
        }
    }

    public enum QueryStatus
    {
        Success,
        Error,
        Exception
    }
}