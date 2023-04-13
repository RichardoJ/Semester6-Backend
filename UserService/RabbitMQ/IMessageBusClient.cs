using UserService.Dto;

namespace UserService.RabbitMQ
{
    public interface IMessageBusClient
    {
        void deleteAuthor(UserDeleteDto userDeleteDto);
    }
}
