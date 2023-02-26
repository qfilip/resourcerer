namespace Resourcerer.Dtos.Data;

public class ResultData<T>
{
    public ResultData(T data)
    {
        Data = data;
        Type = eResultDataType.Some;
    }

    public ResultData()
    {
        Type = eResultDataType.NotFound;
    }

    public ResultData(string[]? errors)
    {
        Type = eResultDataType.Error;
        Errors = errors;
    }

    public T? Data { get; private set; }
    public eResultDataType Type { get; private set; }
    public string[]? Errors { get; set; }
}

