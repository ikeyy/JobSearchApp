using JobSearchApp.Application.Command.AcceptJobOffer;
using JobSearchApp.Application.Command.CreateJobOffer;
using JobSearchApp.Application.Command.DeleteJobOffer;
using JobSearchApp.Application.Command.UpdateJobOffer;
using JobSearchApp.Application.DTO.Generic;
using JobSearchApp.Application.Query.GetJobOffer;
using JobSearchApp.Application.Query.SearchJobOffer;
using JobSearchApp.Domain.DTO.Generic;
using JobSearchApp.Domain.DTO.JobOffer;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace JobSearchApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class JobOfferController : ControllerBase
    {
        private readonly IMediator _mediator;
        public JobOfferController(IMediator mediator) => _mediator = mediator;

        [HttpGet]
        public async Task<ActionResult<IEnumerable<JobOfferData>>> Get()
        {
            try
            {
                var command = new GetJobOfferQuery();
                var result = await _mediator.Send(command);
                return Ok(result);
            } catch (ArgumentException ex)
            {
                return BadRequest($"Invalid request: {ex.Message}");
            } catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while processing the request: {ex.Message}");
            }
        }

        [HttpGet("search")]
        public async Task<ActionResult<PagedResult<JobOfferData>>> SearchJobOffer(
            [FromQuery] string? filter,
            [FromQuery] int pageNumber,
            [FromQuery] int pageSize
            )
        {
            try
            {
                var query = new SearchJobOfferQuery(filter, pageNumber, pageSize);
                var result = await _mediator.Send(query);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("create")]
        public async Task<ActionResult<JobOfferData>> Post([FromBody] JobOfferData jobOfferData)
        {
            try
            {
                var command = new CreateJobOfferCommand(jobOfferData);
                var result = await _mediator.Send(command);
                return result;
            }
            catch (FluentValidation.ValidationException ex)
            {
                var errors = ex.Errors
                    .Select(e => new { Property = e.PropertyName, Error = e.ErrorMessage })
                    .ToList();

                return BadRequest(new { Message = "Validation failed", Errors = errors });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while creating a job offer.", error = ex.Message });
            }
        }

        [HttpPut("update")]
        public async Task<ActionResult<ApiResponse<Guid>>> Put([FromBody] JobOfferData jobOfferData)
        {
            try
            {
                var command = new UpdateJobOfferCommand(jobOfferData);
                var result = await _mediator.Send(command);
                return result;
            }
            catch (FluentValidation.ValidationException ex)
            {
                var errors = ex.Errors
                    .Select(e => new { Property = e.PropertyName, Error = e.ErrorMessage })
                    .ToList();

                return BadRequest(new { Message = "Validation failed", Errors = errors });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while updating a job offer.", error = ex.Message });
            }
        }

        [HttpPut("status")]
        public async Task<ActionResult<ApiResponse<Guid>>> UpdateJobOfferStatus([FromBody] SetJobOfferStatus statusJobOffer)
        {
            try
            {
                var command = new UpdateJobOfferStatusCommand(statusJobOffer);
                var result = await _mediator.Send(command);
                return result;
            }
            catch (FluentValidation.ValidationException ex)
            {
                var errors = ex.Errors
                    .Select(e => new { Property = e.PropertyName, Error = e.ErrorMessage })
                    .ToList();

                return BadRequest(new { Message = "Validation failed", Errors = errors });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while updating a job offer.", error = ex.Message });
            }
        }

        [HttpDelete("delete/{id}")]
        public async Task<ActionResult<ApiResponse<Guid>>> Delete(Guid id)
        {
            try
            {
                var command = new DeleteJobOfferCommand(id);
                var result = await _mediator.Send(command);
                return result;
            }
            catch (FluentValidation.ValidationException ex)
            {
                var errors = ex.Errors
                    .Select(e => new { Property = e.PropertyName, Error = e.ErrorMessage })
                    .ToList();

                return BadRequest(new { Message = "Validation failed", Errors = errors });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while deleting a job offer.", error = ex.Message });
            }
        }
    }
}
