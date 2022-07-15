using Refit;

namespace Banking.WebUI.Services
{
    public interface IAccountBackendClient
    {
        [Get("/accounts")]
        Task<List<Account>> GetAccounts();

        [Get("/account/transfer")]
        Task<bool> AccountTransfer(AccountTransfer accountTransfer);
    }
}
