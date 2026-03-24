using FluentValidation;
using JobSearchApp.Application.Command.CreateJob;
using JobSearchApp.Application.Command.DeleteJob;
using JobSearchApp.Application.Command.UpdateJob;
using JobSearchApp.Application.DTO.Generic;
using JobSearchApp.Application.Query.GetJob;
using JobSearchApp.Application.Query.GetJobById;
using JobSearchApp.Application.Query.SearchCustomer;
using JobSearchApp.Application.Query.SearchJob;
using JobSearchApp.Domain.DTO.Customer;
using JobSearchApp.Domain.DTO.Generic;
using JobSearchApp.Domain.DTO.Job;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace JobSearchApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class JobController : ControllerBase
    {
        private readonly IMediator _mediator;
        public JobController(IMediator mediator) => _mediator = mediator;

        [HttpGet]
        public async Task<IEnumerable<JobData>> Get()
        {
            var query = new GetJobQuery();
            var result = await _mediator.Send(query);
            return result;
        }

        [HttpGet("search")]
        public async Task<ActionResult<PagedResult<JobData>>> SearchJob(
            [FromQuery] JobSearchParams parameters
            )
        {
            try
            {
                var query = new SearchJobQuery(parameters);
                var result = await _mediator.Send(query);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{id}")]
        public async Task<JobData> Get(Guid jobId)
        {
            var query = new GetJobByIdQuery(jobId);
            var result = await _mediator.Send(query);
            return result;
        }

        [HttpPost("create")]
        public async Task<ActionResult<ApiResponse<Guid>>> Post([FromBody] JobData jobData)
        {
            try
            {
                var command = new CreateJobCommand(jobData);
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
                return StatusCode(500, new { message = "An error occurred while creating a job.", error = ex.Message });
            }
        }

        [HttpPut("update/{id}")]
        public async Task<ActionResult<ApiResponse<Guid>>> Put(Guid id,[FromBody] JobData jobData)
        {
            try
            {
                var query = new UpdateJobCommand(id, jobData);
                var result = await _mediator.Send(query);
                return result;
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
                return StatusCode(500, new { message = "An error occurred while updating a job.", error = ex.Message });
            }
        }

        [HttpPut("accept-job/{id}")]
        public async Task<ActionResult<ApiResponse<Guid>>> AcceptJob(Guid id, [FromBody] JobData jobData)
        {
            try
            {
                var query = new UpdateJobCommand(id, jobData);
                var result = await _mediator.Send(query);
                return result;
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
                return StatusCode(500, new { message = "An error occurred while updating a job.", error = ex.Message });
            }
        }

        [HttpDelete("delete/{id}")]
        public async Task<ActionResult<ApiResponse<Guid>>> Delete(Guid id)
        {
            try
            {
                var query = new DeleteJobCommand(id);
                var result = await _mediator.Send(query);
                return result;
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
                return StatusCode(500, new { message = "An error occurred while deleting a job.", error = ex.Message });
            }
        }
    }
}
