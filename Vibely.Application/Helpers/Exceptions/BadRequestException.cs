namespace FitFlare.Application.Helpers.Exceptions;

public class BadRequestException(string message = "Bad Request") : Exception(message);