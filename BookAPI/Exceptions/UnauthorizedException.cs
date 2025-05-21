namespace BookAPI.Exceptions;

public class UnauthorizedException(string message) : Exception(message);