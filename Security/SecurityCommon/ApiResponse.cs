namespace Security.WorkflowCommon
{
    public class ApiResponse<T>
    {
        public string Status { get; set; }    // e.g., "Success" or "Error"
        public string Message { get; set; }  // e.g., "Data retrieved successfully"
        public object TotalPages { get; set; } // Dynamic property to support any data type
        public T Data { get; set; }          // Can be a list, array, or any object
    }
}
