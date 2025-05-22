using System.ComponentModel.DataAnnotations;

namespace RentApp.Server.Utils
{
    public class AllowedExtensionsAttribute : ValidationAttribute
    {
        private readonly string[] _extensions;

        public AllowedExtensionsAttribute(string[] extensions)
        {
            _extensions = extensions;
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var file = value as IFormFile;
            if (file != null)
            {
                var extension = Path.GetExtension(file.FileName)?.ToLower();
                if (!_extensions.Contains(extension))
                {
                    return new ValidationResult($"Fisierul trebuie sa aiba una dintre extensiile: {string.Join(", ", _extensions)}");
                }
            }

            return ValidationResult.Success;
        }
    }

}
