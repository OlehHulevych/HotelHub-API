using server.DTO;

namespace server.IRepositories;

public interface IReportRepository
{
    public Task<ResultDto> GetReport();
}