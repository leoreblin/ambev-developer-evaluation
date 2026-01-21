using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.WebApi.Common;
using Microsoft.AspNetCore.Mvc;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Branches;

[ApiController]
[Route("branches")]
public sealed class BranchesController : BaseController
{
    private readonly IBranchRepository _branchRepository;

    public BranchesController(IBranchRepository branchRepository)
    {
        _branchRepository = branchRepository;
    }

    [HttpGet]
    [ProducesResponseType(typeof(ApiResponseWithData<IEnumerable<Branch>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Get(CancellationToken cancellationToken)
        => Ok(await _branchRepository.GetAllAsync(cancellationToken));
}
