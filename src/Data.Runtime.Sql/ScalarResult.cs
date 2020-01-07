using System.Runtime.Serialization;

namespace Data.Runtime.Sql
{
    /// <summary>
    /// Result of Scalar
    /// </summary>
    [DataContract]
    public
#if LATEST_VS
    readonly
#endif
    struct ScalarResult
    {

        public ScalarResult(string message, object value, OperationStatus status)
        {
            Message = message;
            Value = value;
            Status = status;
        }

        [DataMember(Name = "message")]
        public string Message { get; }

        [DataMember(Name = "value")]
        public object Value { get; }

        [IgnoreDataMember]
        public OperationStatus Status { get; }

        public bool IsSuccess => Status == OperationStatus.Success;

        public static ScalarResult Success(object value)
        {
            return new ScalarResult(SqlTableRef.SuccessMessage, value, OperationStatus.Success);
        }

        public static ScalarResult Fail
        {
            get
            {
                return new ScalarResult(SqlTableRef.FailMessage, -1, OperationStatus.Error);
            }
        }


        public static ScalarResult Error(System.Exception ex)
        {
            return new ScalarResult(ex.Message, -1, OperationStatus.Exception);
        }

        public static ScalarResult ErrorMessage(string error)
        {
            return new ScalarResult(error, -1, OperationStatus.Error);
        }

    }
}
