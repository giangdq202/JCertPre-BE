namespace JCertPreApplication.Application.Dtos.Payment;

public class TransactionDto
{
    public string Reference { get; set; } = string.Empty;
    public int Amount { get; set; }
    public string AccountNumber { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string TransactionDateTime { get; set; } = string.Empty;
    public string VirtualAccountName { get; set; } = string.Empty;
    public string VirtualAccountNumber { get; set; } = string.Empty;
    public string CounterAccountBankId { get; set; } = string.Empty;
    public string CounterAccountBankName { get; set; } = string.Empty;
    public string CounterAccountName { get; set; } = string.Empty;
    public string CounterAccountNumber { get; set; } = string.Empty;
}
