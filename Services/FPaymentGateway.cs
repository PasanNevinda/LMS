namespace LMS.Services
{
    public class GatewayResponse
    {
        public bool Success { get; set; }
        public string TransactionId { get; set; }
        public string Message { get; set; }
    }

    public interface IPaymentGateway
    {
        Task<GatewayResponse> ChargeAsync(decimal amount, string currency, string description, string paymentMethodToken);
        Task<GatewayResponse> VerifyAsync(string transactionId);
    }

    // Fake gateway for development/testing
    public class FakePaymentGateway : IPaymentGateway
    {
        public Task<GatewayResponse> ChargeAsync(decimal amount, string currency, string description, string paymentMethodToken)
        {
            // Simulate success
            return Task.FromResult(new GatewayResponse
            {
                Success = true,
                TransactionId = Guid.NewGuid().ToString(),
                Message = "Simulated success"
            });
        }

        public Task<GatewayResponse> VerifyAsync(string transactionId)
        {
            return Task.FromResult(new GatewayResponse
            {
                Success = true,
                TransactionId = transactionId,
                Message = "Simulated verified"
            });
        }
    }

}
