namespace GLMSMVC.Services.Results
{
    public class ServiceResult<T>//(Aseem, 2025)
    {
        public bool IsSuccess { get; set; }
        // Value return from method
        public T? Data { get; set; }
        // If IsSuccess False - First Error
        public string? ErrorMessage { get; set; }
        // If IsSuccess False - List All Errors
        public List<string> Errors { get; set; } = new();

        public static ServiceResult<T> Success(T data)
        {
            return new ServiceResult<T>
            {
                IsSuccess = true,
                Data = data
            };
        }

        public static ServiceResult<T> Failure(string errorMessage)
        {
            return new ServiceResult<T>
            {
                IsSuccess = false,
                ErrorMessage = errorMessage,
                Errors = new List<string> { errorMessage }
            };
        }

        public static ServiceResult<T> Failure(List<string> errors)
        {
            return new ServiceResult<T>
            {
                IsSuccess = false,
                ErrorMessage = errors.FirstOrDefault(),
                Errors = errors
            };
        }
    }
}
/*
Aseem. 2025. The Result Pattern in C#: A Smarter Way to Handle Errors. [online]. Available at: https://medium.com/@aseem2372005/the-result-pattern-in-c-a-smarter-way-to-handle-errors-c6dee28a0ef0 [Accessed 22 April 2026].

 */