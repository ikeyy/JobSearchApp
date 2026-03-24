using JobSearchApp.Domain.DTO.Generic;
using JobSearchApp.Domain.Entities;
using JobSearchApp.Domain.Interfaces.Service;
using MediatR;

namespace JobSearchApp.Application.Command.CreateCustomer
{
    public class CreateCustomerCommandHandler : IRequestHandler<CreateCustomerCommand, ApiResponse<Guid>>
    {
        private readonly ICustomerService _customerService;

        public CreateCustomerCommandHandler(ICustomerService customerService)
        {
            _customerService = customerService;
        }


        public async Task<ApiResponse<Guid>> Handle(CreateCustomerCommand request, CancellationToken cancellationToken)
        {
            var customer = new Customer
            {
                Id = Guid.NewGuid(),
                FirstName = request.CustomerData.FirstName,
                LastName = request.CustomerData.LastName,
                CreatedAt = DateTime.UtcNow
            };

            var customerId = await _customerService.CreateCustomerAsync(customer, cancellationToken);

            return ApiResponse<Guid>.SuccessResponse(customerId, "Customer has been added");
           
        }
    }
}
