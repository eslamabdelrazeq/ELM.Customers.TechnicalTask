using ELM.Common.DTO;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ELM.Customers.API.Validators
{
    public class CreateCustomerValidator : AbstractValidator<CustomerDTO>
    {
        public CreateCustomerValidator()
        {
            RuleFor(m => m.FirstName).NotEmpty().WithMessage("First name can not be empty");
            RuleFor(x => x.LastName).NotEmpty().WithMessage("Last name can not be empty");
            RuleFor(x => x.Phone).NotEmpty().WithMessage("Phone can not be empty");
            RuleFor(x => x.Email).EmailAddress().WithMessage("Wrong email address"); ;

        }
    }

    public class CreateCustomerListValidator : AbstractValidator<List<CustomerDTO>>
    {
        public CreateCustomerListValidator()
        {
            RuleForEach(x => x).SetValidator(new CreateCustomerValidator());

        }
    }

}
