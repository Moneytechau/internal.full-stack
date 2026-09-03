namespace Backend.Models;

public class Greeting
{
    public string Message { get; set; } = string.Empty;

    public DateTime ServerTimeUtc { get; set; }
}
