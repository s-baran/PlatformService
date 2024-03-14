using PlatformService.Dtos;

namespace PlatformService.AsyncDataServcies
{
    public interface IMessageBusClient
    {
        void PublishNewPlatform(PlatformPublishedDto platformPublishedDto);
    }
}