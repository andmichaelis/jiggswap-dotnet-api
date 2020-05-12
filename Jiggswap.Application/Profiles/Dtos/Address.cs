using System;
using System.Collections.Generic;
using System.Text;
using FluentValidation;

namespace Jiggswap.Application.Profiles.Dtos
{
    public interface IAddress
    {
        public string StreetAddress { get; set; }

        public string City { get; set; }

        public string State { get; set; }

        public string Zip { get; set; }
    }

    public class Address : IAddress
    {
        public string StreetAddress { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
    }

    public class AddressValidator : AbstractValidator<IAddress>
    {
        public AddressValidator()
        {
            RuleFor(v => v.State)
                .NotEmpty()
                .Length(2);

            RuleFor(v => v.Zip)
                .MaximumLength(10);
        }
    }
}
