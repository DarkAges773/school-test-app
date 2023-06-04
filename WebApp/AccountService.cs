using System.Threading;
using System.Threading.Tasks;

namespace WebApp
{
    class AccountService : IAccountService
    {
        private readonly IAccountCache _cache;
        private readonly IAccountDatabase _db;
        private static readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);

        public AccountService(IAccountCache cache, IAccountDatabase db)
        {
            _cache = cache;
            _db = db;
        }

        public Account GetFromCache(long id)
        {
            if (_cache.TryGetValue(id, out var account))
            {
                return account;
            }

            return null;
        }

        public async ValueTask<Account> LoadOrCreateAsync(string id)
        {
            await _semaphore.WaitAsync();
            try
            {
                if (!_cache.TryGetValue(id, out var account))
                {
                    account = await _db.GetOrCreateAccountAsync(id);
                    _cache.AddOrUpdate(account);
                }

                return account;
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public async ValueTask<Account> LoadOrCreateAsync(long id)
        {
            await _semaphore.WaitAsync();
            try
            {
                if (!_cache.TryGetValue(id, out var account))
                {
                    account = await _db.GetOrCreateAccountAsync(id);
                    _cache.AddOrUpdate(account);
                }

                return account;
            }
			finally
			{
				_semaphore.Release();
			}
		}
    }
}