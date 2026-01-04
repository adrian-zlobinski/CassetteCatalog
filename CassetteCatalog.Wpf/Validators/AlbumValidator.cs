using CassetteCatalog.Wpf.ViewModels;
using FluentValidation;

namespace CassetteCatalog.Wpf.Validators
{
    public class AlbumValidator : AbstractValidator<AlbumEditViewModel>
    {
        public AlbumValidator() 
        { 
            RuleFor(x=>x.Artist).NotEmpty().WithMessage("Nazwa artysty jest wymagana")
                .Length(2,100).WithMessage("Nazwa artysty musi mieć od 2 do 100 znaków");

            RuleFor(x => x.Title).NotEmpty().WithMessage("Tytuł albumu jest wymagany");

            RuleFor(x => x.ReleaseYear).GreaterThanOrEqualTo((ushort)1963).WithMessage("Rok wydania musi być większy niż 1963")
                .LessThanOrEqualTo((ushort)DateTime.Now.Year).WithMessage("Rok wydania nie może być z przyszłości");

            RuleFor(x => x.Tracks).Must(x => x.Count > 0).WithMessage("Album musie mieć przynajmniej jeden utwór");

            RuleForEach(x=>x.Tracks).SetValidator(new TrackValidator());
        }
    }
}
