using PublishNoSQL.Dto;

namespace PublishNoSQL.RabbitMQ
{
    public interface IMessageBusClient
    {
        void PublishNewPaper(PaperPublishedDto paperPublished);

        void DeletePaper(PaperDeletedDto paperDeleted);

        void UpdatePaper(PaperUpdatedDto paperUpdated);
    }
}
