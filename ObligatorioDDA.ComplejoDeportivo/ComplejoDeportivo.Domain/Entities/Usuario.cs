using ComplejoDeportivo.Domain.Enums;
using ComplejoDeportivo.Domain.Excepciones;

namespace ComplejoDeportivo.Domain.Entities
{
    public class Usuario
    {
        public int Id { get; private set; }
        public string Email { get; private set; } = null!;
        public string PasswordHash { get; private set; } = null!;
        public RolUsuario Rol { get; private set; }

        // Opcional: solo se completa si el usuario tiene rol Socio
        public int? SocioId { get; private set; }
        public Socio? Socio { get; private set; }

        private Usuario() { }

        public Usuario(string email, string passwordHash, RolUsuario rol, int? socioId = null)
        {
            if (string.IsNullOrWhiteSpace(email) || !email.Contains('@'))
                throw new ExcepcionDeDominio("El email del usuario no es válido.");
            if (string.IsNullOrWhiteSpace(passwordHash))
                throw new ExcepcionDeDominio("El hash de contraseña es obligatorio.");
            if (rol == RolUsuario.Socio && (!socioId.HasValue || socioId.Value <= 0))
                throw new ExcepcionDeDominio("Un usuario con rol Socio debe estar asociado a un socio válido.");
            if (rol == RolUsuario.Administrador && socioId.HasValue)
                throw new ExcepcionDeDominio("Un usuario Administrador no debe tener un SocioId asociado.");

            Email = email.Trim().ToLowerInvariant();
            PasswordHash = passwordHash;
            Rol = rol;
            SocioId = socioId;
        }

        public void ActualizarPasswordHash(string nuevoPasswordHash)
        {
            if (string.IsNullOrWhiteSpace(nuevoPasswordHash))
                throw new ExcepcionDeDominio("El nuevo hash de contraseña no puede estar vacío.");

            PasswordHash = nuevoPasswordHash;
        }
    }
}