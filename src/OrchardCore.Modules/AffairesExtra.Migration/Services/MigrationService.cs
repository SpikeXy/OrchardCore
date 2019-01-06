using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using OrchardCore.Environment.Cache;
using YesSql;

namespace AffairesExtra.Migration
{
    public class MigrationService
    {
        private readonly IMemoryCache _memoryCache;
        private readonly ISignal _signal;
        private readonly IServiceProvider _serviceProvider;
        private readonly IStore _store;
        private const string AffairesExtraMigrationCacheKey = "AffairesExtraMigrationService";

        public MigrationService(
            ISignal signal,
            IServiceProvider serviceProvider,
            IMemoryCache memoryCache,
            IStore store,
            ILogger<MigrationService> logger)
        {
            _signal = signal;
            _serviceProvider = serviceProvider;
            _store = store;
            _memoryCache = memoryCache;
            Logger = logger;
        }

        public ILogger Logger { get; set; }
        public IChangeToken ChangeToken => _signal.GetToken(AffairesExtraMigrationCacheKey);

        public async Task<IEnumerable<dynamic>> Migrate()
        {
            var connection = _store.Configuration.ConnectionFactory.CreateConnection();

            try
            {
                using (connection)
                {
                    connection.Open();
                    return await connection.QueryAsync("SELECT * FROM products_import");
                }
            }
            catch (Exception e)
            {
                Logger.LogError("An error occured while executing the SQL query: {0}", e.Message);
                throw;
            }

        }

    }
}
