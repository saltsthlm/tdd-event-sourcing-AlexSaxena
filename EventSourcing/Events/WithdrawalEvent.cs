using System.Text.Json.Serialization;
using EventSourcing.Models;

namespace EventSourcing.Events;

public record WithdrawalEvent : Event
{
  [JsonPropertyName("amount")]
  public required decimal Amount { get; init; }

  [JsonPropertyName("transactionId")]
  public required string TransactionId { get; init; }

  [JsonPropertyName("currency")]
  public required CurrencyType Currency { get; init; }
}
