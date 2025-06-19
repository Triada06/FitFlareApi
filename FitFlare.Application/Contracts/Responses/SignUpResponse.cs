namespace FitFlare.Application.Contracts.Responses;

public class SignUpResponse
{
    public List<string>? Errors { get; set; }
    public bool IsSucceeded { get; set; }
}