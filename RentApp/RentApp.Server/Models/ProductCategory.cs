
using System.ComponentModel.DataAnnotations;
namespace RentApp.Server.Models
{
    public enum ProductCategory
    {
        [Display(Name = "Electronice")]
        Electronice,

        [Display(Name = "Electrocasnice")]
        Electrocasnice,

        [Display(Name = "Mobilier")]
        Mobilier,

        [Display(Name = "Vehicule")]
        Vehicule,

        [Display(Name = "Unelte")]
        Unelte,

        [Display(Name = "Imbracaminte")]
        Imbracaminte,

        [Display(Name = "Carti")]
        Carti,

        [Display(Name = "Jucarii")]
        Jucarii,

        [Display(Name ="Audio")]
        Audio,

        [Display(Name = "Video")]
        Video,

        [Display(Name = "Sport")]
        Sport,

        [Display(Name = "Bucatarie")]
        Bucatarie,

        [Display(Name = "Gradina")]
        Gradina,

        [Display(Name = "Constructii")]
        Constructii,

        [Display(Name = "Altele")]
        Altele
    }
}
