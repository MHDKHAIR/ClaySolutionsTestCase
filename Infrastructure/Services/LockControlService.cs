using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Interfaces.Services;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services
{
    /// <summary>
    /// IOT Service interface
    /// </summary>
    public class LockControlService : ILockControlService
    {
        private readonly ILogger<LockControlService> logger;
        private readonly IDateTimeService dateTimeService;

        public LockControlService(ILogger<LockControlService> logger, IDateTimeService dateTimeService)
        {
            this.logger = logger;
            this.dateTimeService = dateTimeService;
        }
        public async Task<bool> CloseLock(string key)
        {
            logger.LogInformation($"Door closed with key:{key} ; At {dateTimeService.Now}");
            return await Task.FromResult(true);
        }

        public async Task<bool> OpenLock(string key, int timeOutToClose = 10)
        {
            logger.LogInformation($"Door opend with key:{key} ; At {dateTimeService.Now}");
            await Task.Delay(timeOutToClose);
            dateTimeService.Now.AddSeconds(timeOutToClose);
            await CloseLock(key);
            return await Task.FromResult(true);
        }
    }
}
