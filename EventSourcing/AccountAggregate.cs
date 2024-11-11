using EventSourcing.Events;
using EventSourcing.Exceptions;
using EventSourcing.Models;
using Microsoft.VisualBasic;

namespace EventSourcing;

public class AccountAggregate
{

  public string? AccountId { get; set; }
  public decimal Balance { get; set; }
  public decimal MaxBalance { get; set; }
  public CurrencyType Currency { get; set; }
  public string? CustomerId { get; set; }
  public AccountStatus Status { get; set; }
  public List<LogMessage>? AccountLog { get; set; }

  private AccountAggregate() { }

  public static AccountAggregate? GenerateAggregate(Event[] events)
  {
    if (events.Length == 0)
    {
      return null;
    }

    var account = new AccountAggregate();
    foreach (var accountEvent in events)
    {
      account.Apply(accountEvent);
    }

    return account;
  }

  private void Apply(Event accountEvent)
  {
    switch (accountEvent)
    {
      case AccountCreatedEvent accountCreated:
        Apply(accountCreated);
        break;
      case DepositEvent deposit:
        Apply(deposit);
        break;
      case WithdrawalEvent withdrawal:
        Apply(withdrawal);
        break;
      case DeactivationEvent deactivation:
        Apply(deactivation);
        break;
      case ActivationEvent activation:
        Apply(activation);
        break;
      case ClosureEvent closure:
        Apply(closure);
        break;
      default:
        throw new EventTypeNotSupportedException("162 ERROR_EVENT_NOT_SUPPORTED");
    }
  }

  private void Apply(AccountCreatedEvent accountCreated)
  {
    AccountId = accountCreated.AccountId;
    Balance = accountCreated.InitialBalance;
    Currency = accountCreated.Currency;
    CustomerId = accountCreated.CustomerId;
    MaxBalance = accountCreated.MaxBalance;
  }

  private void Apply(DepositEvent deposit)
  {
    if (AccountId == null)
    {
      throw new AccountNotCreatedException("128*");
    }

    if (Status != AccountStatus.Enabled)
    {
      if (Status == AccountStatus.Closed)
      {
        throw new InvalidOperationException("502*");
      }
      else
      {
        throw new Exception("344");
      }
    }

    if (MaxBalance < deposit.Amount)
    {
      throw new MaxBalanceExceeded("281*");
    }

    Balance += deposit.Amount;
  }

  private void Apply(WithdrawalEvent withdrawal)
  {

    if (AccountId == null)
    {
      throw new AccountNotCreatedException("128*");
    }


    if (Status != AccountStatus.Enabled)
    {
      if (Status == AccountStatus.Closed)
      {
        throw new InvalidOperationException("502*");
      }
      else
      {
        throw new Exception("344");
      }
    }

    if (withdrawal.Amount > Balance)
    {
      throw new NegativeBalanceException("285*");
    }



    Balance -= withdrawal.Amount;


  }

  private void Apply(DeactivationEvent deactivation)
  {
    Status = AccountStatus.Disabled;

    if (AccountLog == null)
    {
      AccountLog = new List<LogMessage>();
    }

    var log = new LogMessage(
         Type: "DEACTIVATE",
         Message: deactivation.Reason,
         Timestamp: deactivation.Timestamp
     );

    AccountLog.Add(log);
  }

  private void Apply(ActivationEvent activation)
  {

    if (Status != AccountStatus.Enabled)
    {
      Status = AccountStatus.Enabled;
      var log = new LogMessage(
           Type: "ACTIVATE",
           Message: "Account reactivated",
           Timestamp: activation.Timestamp
       );

      AccountLog.Add(log);
    }

  }

  private void Apply(CurrencyChangeEvent currencyChange)
  {
    throw new NotImplementedException();
  }

  private void Apply(ClosureEvent closure)
  {
    if (AccountLog == null)
    {
      AccountLog = new List<LogMessage>();
    }
    if (Status != AccountStatus.Closed)
    {
      Status = AccountStatus.Closed;
    }

    var wholeBalance = Math.Floor(Balance);

    var log = new LogMessage(
           Type: "CLOSURE",
           Message: $"Reason: {closure.Reason}, Closing Balance: '{wholeBalance}'",
           Timestamp: closure.Timestamp
       );

    AccountLog.Add(log);
  }
}
