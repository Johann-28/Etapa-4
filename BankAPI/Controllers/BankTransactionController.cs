using Microsoft.AspNetCore.Mvc;
using BankAPI.Services;
using BankAPI.Data.BankModels;
using Microsoft.AspNetCore.Authorization;

namespace BankAPI.Controllers;

[Authorize(Policy = "Client")]
[ApiController]
[Route("api/[controller]")]

public class BankTransactionController : ControllerBase
{
    private readonly BankTransactionService _service;

    public BankTransactionController(BankTransactionService service)
    {
        _service = service;
    }

    [HttpGet("getall")]
    public async Task<IEnumerable<BankTransaction>> Get()
    {
        return await _service.GetAll();
    }

}