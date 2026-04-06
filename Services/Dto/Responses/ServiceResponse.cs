using System.Text.Json.Serialization;

namespace Services.Dto.Responses;

public class ServiceResponse
{
    public int Status { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Message { get; set; }
}

public class ServiceResponse<T> : ServiceResponse
{
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public T? Data { get; set; }
}
