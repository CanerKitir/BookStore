using System.Text.Json.Serialization;

public class BaseEntity
{
    public int Id { get; set; }
    [JsonIgnore]
    public DateTime UpdateTime { get; set; }
    [JsonIgnore]
    public DateTime CreatedTime { get; set; }

}