using JobSearchApp.Application.Command.CreateCustomer;
using JobSearchApp.Application.Query.GetCustomer;
using JobSearchApp.Application.Query.SearchCustomer;
using JobSearchApp.Domain.DTO.Customer;
using JobSearchApp.Domain.DTO.Generic;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using FluentValidation;
using JobSearchApp.Application.DTO.Generic;

namespace JobSearchApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly IMediator _mediator;
        public CustomerController(IMediator mediator) => _mediator = mediator;

        [HttpGet]
        public async Task<IEnumerable<CustomerData>> Get()
        {
            var query = new GetCustomerQuery();
            var result = await _mediator.Send(query);
            return result;
        }

        [HttpGet("search")]
        public async Task<ActionResult<PagedResult<CustomerData>>> SearchCustomer(
            [FromQuery] string? filter,
            [FromQuery] int pageNumber,
            [FromQuery] int pageSize
            )
        {
            try
            {
                var query = new SearchCustomerQuery(filter, pageNumber,pageSize);
                var result = await _mediator.Send(query);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("create")]
        public async Task<ActionResult<ApiResponse<Guid>>> Post([FromBody] CustomerData customerData)
        {
            try
            {
                var command = new CreateCustomerCommand(customerData);
                var result = await _mediator.Send(command);
                return Ok(result);
            }catch (ValidationException ex)
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

        [HttpPut("{id}")]
        public void Put(Guid id, [FromBody] CustomerData data)
        {

        }

        [HttpDelete("{id}")]
        public void Delete(Guid id)
        {

        }
    }
}
