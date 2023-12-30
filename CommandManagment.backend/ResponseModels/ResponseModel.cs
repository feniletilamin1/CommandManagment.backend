namespace CommandManagment.backend.ResponseModels
{
    public class ResponseModel
    {
        public ResponseModel(string message)
        {
            Message = message;
        }

        public string Message { get; set; }
    }
}
