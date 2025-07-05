namespace Core.Entities;

public class Message
{
    public string Action { get; set; } = string.Empty;
    public object? Data { get; set; } = null;
}