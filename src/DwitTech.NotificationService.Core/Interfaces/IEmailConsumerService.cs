using Confluent.Kafka;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DwitTech.NotificationService.Core.Interfaces
{
    public interface IEmailConsumerService
    {
        Task ConsumeEmailMessage(CancellationToken cancellationToken);
    }
}
