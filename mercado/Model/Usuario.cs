using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace mercado.Model
{
    public class Usuario
    {
        [Key]
        public int Id { get; set; }

        [MaxLength(100)]
        [Required]
        public string Nome { get; set; } = string.Empty;

        [Required]
        public TipoUsuario Tipo { get; set; }

        [Required]
        [MaxLength(255)]
        public string SenhaHash { get; set; } = string.Empty;

        public bool Ativo { get; set; } = true;

        /// <summary>
        /// Indica que o usuário deve trocar a senha no próximo login.
        /// Usado para o administrador padrão criado pelo seed.
        /// </summary>
        public bool DeveTrocarSenha { get; set; }

        public DateTime DataCriacao { get; set; } = DateTime.Now;
    }

    public enum TipoUsuario
    {
        Administrador = 1,
        Caixa = 2
    }
}