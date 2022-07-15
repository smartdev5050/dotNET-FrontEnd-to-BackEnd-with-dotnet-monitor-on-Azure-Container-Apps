using Refit;

namespace Banking.WebUI.Services
{
    public class AccountBackendClient : IAccountBackendClient
    {
        IHttpClientFactory _httpClientFactory;

        public AccountBackendClient(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public Task<bool> AccountTransfer(AccountTransfer accountTransfer)
        {
            var client = _httpClientFactory.CreateClient("Accounts");

            return RestService.For<IAccountBackendClient>(client).AccountTransfer(accountTransfer);
        }

        public Task<List<Account>> GetAccounts()
        {
            var client = _httpClientFactory.CreateClient("Accounts");

            return RestService.For<IAccountBackendClient>(client).GetAccounts();
        }
    }
}
