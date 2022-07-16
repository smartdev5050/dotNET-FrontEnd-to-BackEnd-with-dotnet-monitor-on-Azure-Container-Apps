using Refit;

namespace Banking.WebUI.Services
{
    public interface IAccountBackendClient
    {
        [Get("/accounts")]
        Task<List<Account>> GetAccounts();

        [Post("/account/transfer")]
        Task<HttpResponseMessage> AccountTransfer([Body] AccountTransfer accountTransfer);
    }
}
