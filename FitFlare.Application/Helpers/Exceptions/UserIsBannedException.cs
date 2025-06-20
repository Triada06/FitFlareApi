namespace FitFlare.Application.Helpers.Exceptions;

public class UserIsBannedException(string message = "User is banned") : Exception(message);
    