namespace ShareMyCarBackend.Response
{
    public class ErrorResponse : IResponse
    {
        public int ErrorCode { get; set; }
        public object Message { get; set; }
    }
}
