namespace TourCrm.Application.Common.Results;

public class ServiceResult<T>
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public T? Data { get; set; }

    public static ServiceResult<T> Ok(T data, string message = "Успешно") =>
        new() { Success = true, Message = message, Data = data };

    public static ServiceResult<T> Ok(string message = "Успешно") =>
        new() { Success = true, Message = message };

    public static ServiceResult<T> Fail(string message) =>
        new() { Success = false, Message = message, Data = default };
}