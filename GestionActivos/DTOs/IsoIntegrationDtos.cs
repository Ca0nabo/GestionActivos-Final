namespace GestionActivos.DTOs
{
    // Para el Login
    public class LoginRequest
    {
        public string username { get; set; }
        public string password { get; set; }
    }

    public class LoginResponse
    {
        public bool isOk { get; set; }
        public LoginData data { get; set; }
    }

    public class LoginData
    {
        public string token { get; set; }
    }

    // Para leer las Cuentas disponibles (NUEVO)
    public class CuentaDto
    {
        public int id { get; set; }
        public string description { get; set; }
        public string type { get; set; } // "ACTIVO", "PASIVO", etc.
    }

    // Para enviar el Asiento
    public class AsientoRequest
    {
        public string description { get; set; }
        public object accountId { get; set; } // ID de la cuenta (el servidor lo pide)
        public string movementType { get; set; } // "DB" o "CR"
        public decimal amount { get; set; }
        public string entryDate { get; set; }
    }
}