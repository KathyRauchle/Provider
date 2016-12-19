using Newtonsoft.Json;

namespace MicroServiceUtilites
{
    public class Response
    {
        public int Code { get; set; }
        public string Message { get; set; }

        public Response(int code, string message)
        {
            this.Code = code;
            this.Message = message;
        }

        public Response(ResponseCode code, string message)
        {
            this.Code = (int)code;
            this.Message = message;
        }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }

    public enum ResponseCode: int
    {
        SUCCESS = 20,
        BUSINESS_ENTITY_NOT_EXIST = 50,
        BUSINESS_ENTITY_REQUIRED_FIELD_MISSING = 51,
        BAD_REQUEST = 60,
        UNCAUGHT_EXCEPTION = 99
    };
}