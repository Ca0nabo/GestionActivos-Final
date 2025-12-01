using System.ComponentModel.DataAnnotations;

namespace GestionActivos.Models
{
    public class ValidadorCedulaAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null || string.IsNullOrEmpty(value.ToString()))
            {
                return ValidationResult.Success; // Dejamos pasar si es nulo (eso lo valida el [Required])
            }

            string cedula = value.ToString().Replace("-", "").Trim();

            if (cedula.Length != 11 || !long.TryParse(cedula, out _))
            {
                return new ValidationResult("La cédula debe tener 11 dígitos numéricos.");
            }

            if (!ValidaCedula(cedula))
            {
                return new ValidationResult("Esta cédula NO es válida (Matemáticamente incorrecta).");
            }

            return ValidationResult.Success;
        }

        // El famoso Algoritmo de Luhn (Módulo 10) usado en RD
        private bool ValidaCedula(string pCedula)
        {
            int vnTotal = 0;
            string vcCedula = pCedula.Replace("-", "");
            int pLongCed = vcCedula.Trim().Length;
            int[] digitoMult = new int[11] { 1, 2, 1, 2, 1, 2, 1, 2, 1, 2, 1 };

            if (pLongCed < 11 || pLongCed > 11)
                return false;

            for (int vDig = 1; vDig <= pLongCed; vDig++)
            {
                int vCalculo = Int32.Parse(vcCedula.Substring(vDig - 1, 1)) * digitoMult[vDig - 1];
                if (vCalculo < 10)
                    vnTotal += vCalculo;
                else
                    vnTotal += Int32.Parse(vCalculo.ToString().Substring(0, 1)) + Int32.Parse(vCalculo.ToString().Substring(1, 1));
            }

            return (vnTotal % 10 == 0);
        }
    }
}