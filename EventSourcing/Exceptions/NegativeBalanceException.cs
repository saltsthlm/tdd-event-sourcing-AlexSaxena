namespace EventSourcing.Exceptions;

public class NegativeBalanceException(string message) : InvalidOperationException(message);

