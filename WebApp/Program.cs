using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace WebApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
                WebHost.CreateDefaultBuilder(args)
                       .UseStartup<Startup>();

        /*
         * TODO 6: Fix issue
         * Users complains that sometimes, when they call AccountController.UpdateAccount followed by
         * AccountController.GetByInternalId they get account with counter equals 0, like if UpdateCounter was never
         * called.
         * It looks like as if there were two accounts, one being updated by UpdateAccount method and another does not.
         * Find out the problem and fix it.
         */

        // it took so much more time than everything else...
        /*
         * So, basically the problem is that when the LoadOrCreateAsync method of AccountService
         * gets called multiple times at the same time asyncronously, it is likely that the cache wouldnt contain any accounts
         * and so both of the threads will fetch new account copy from the database, assign it to the cache and return.
         * 
         * This behavior is known as "race condition". Race condition occurs when two or more threads can access
         * shared data and try to change it at the same time. To prevent this behavior we can either implement locks
         * for the AccountCache (but that wouldnt prevent us from having multiple copies of one account fetched from db)
         * or we can "lock" LoadOrCreate async method of AccountService, using semaphore (normal locks wont cut it, 
         * because there is another asynchronous call inside this method).
         */

    }
}