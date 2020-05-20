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
                .Length(2)
                .WithMessage("'State' is required.");

            RuleFor(v => v.Zip)
                .NotEmpty()
                .MaximumLength(10);

            RuleFor(v => v.City)
                .NotEmpty()
                .MaximumLength(200);

            RuleFor(v => v.StreetAddress)
                .NotEmpty()
                .MaximumLength(250);
        }
    }
}