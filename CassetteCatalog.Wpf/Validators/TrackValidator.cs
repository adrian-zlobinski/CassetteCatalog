using CassetteCatalog.Wpf.ViewModels;
using FluentValidation;

namespace CassetteCatalog.Wpf.Validators
{
    public class TrackValidator : AbstractValidator<TrackViewModel>
    {
        public TrackValidator()
        {
            RuleFor(x => x.Title).NotEmpty().WithMessage("Tytuł utworu jest wymagany")
                .Length(1, 100).WithMessage("Tytuł utworu musi mieć od 1 do 100 znaków");
            RuleFor(x => x.Duration).GreaterThan(TimeSpan.Zero).WithMessage("Czas trwania utworu musi być większy niż 0");
            RuleFor(x=>x.Side).IsInEnum().WithMessage("Strona kasety musi być poprawna");
        }
    }
}
