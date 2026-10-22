using System.ComponentModel.DataAnnotations;

namespace POOI_EF_SalgadoAmarillo.Models
{
    public class Consumidor
    {
        [Required(ErrorMessage = "Ingrese el DNI.")]
        [RegularExpression(@"^[0-9]{8}$", ErrorMessage = "El DNI debe tener 8 dígitos numéricos.")]
        [Display(Name = "DNI")]
        public string dnicont { get; set; }

        [Required(ErrorMessage = "Ingrese los nombres.")]
        [StringLength(40, ErrorMessage = "Ingrese como máximo 40 caracteres.")]
        [Display(Name = "Nombres")]
        public string nomcont { get; set; }

        [Required(ErrorMessage = "Ingrese los apellidos.")]
        [StringLength(40, ErrorMessage = "Ingrese como máximo 40 caracteres.")]
        [Display(Name = "Apellidos")]
        public string apecont { get; set; }

        [EmailAddress(ErrorMessage = "Ingrese un correo válido.")]
        [StringLength(250, ErrorMessage = "Ingrese como máximo 250 caracteres.")]
        [Display(Name = "Correo")]
        public string emailcont { get; set; }

        [Required(ErrorMessage = "Seleccione un país.")]
        [StringLength(3)]
        [Display(Name = "País")]
        public string idpais { get; set; }
    }
}
