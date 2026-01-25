using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using server.Repository;

namespace server.Controllers;
[ApiController]
[Route("api/report")]
public class ReportController:ControllerBase
{
    private readonly ReportRepository _reportRepository;
    public ReportController(ReportRepository reportRepository)
    {
        _reportRepository = reportRepository;
    }
    [Authorize(Roles = "ADMIN,OWNER")]
    [HttpGet]
    public async Task<IActionResult> GetReport()
    {
        return Ok(await _reportRepository.GetReport());
    }
}