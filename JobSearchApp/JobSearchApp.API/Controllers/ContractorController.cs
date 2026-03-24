using FluentValidation;
using JobSearchApp.Application.Command.CreateContractor;
using JobSearchApp.Application.DTO.Generic;
using JobSearchApp.Application.Query.SearchContractor;
using JobSearchApp.Domain.DTO.Contractor;
using JobSearchApp.Domain.DTO.Generic;
using MediatR;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace JobSearchApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContractorController : ControllerBase
    {
        private readonly IMediator _mediator;
        public ContractorController(IMediator mediator) => _mediator = mediator;

        // GET: api/<ContractorController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<ContractorController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        [HttpGet("search")]
        public async Task<ActionResult<PagedResult<ContractorData>>> SearchContractor(
            [FromQuery] string? filter,
            [FromQuery] int pageNumber,
            [FromQuery] int pageSize
            )
        {
            try
            {
                var query = new SearchContractorQuery(filter, pageNumber, pageSize);
                var result = await _mediator.Send(query);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("create")]
        public async Task<ActionResult<ApiResponse<Guid>>> Post([FromBody] ContractorData contractorData)
        {
            try
            {
                var command = new CreateContractorCommand(contractorData);
                var result = await _mediator.Send(command);
                return Ok(result);
            }
            catch (ValidationException ex)
            {
                var errors = ex.Errors
                    .Select(e => new { Property = e.PropertyName, Error = e.ErrorMessage })
                    .ToList();

                return BadRequest(new { Message = "Validation failed", Errors = errors });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while creating a customer." });
            }
        }

        // PUT api/<ContractorController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<ContractorController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }

        
    }
}
