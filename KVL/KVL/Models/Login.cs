namespace KVL.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Web.Mvc;

    [Table("Login")]
    public partial class Login
    {

        public Login()
        {
            dDataCadastro = DateTime.Now;
            bAtivo = true;
            bConfimacao = false;
            sSenha = "";
        }

        [Key]
        public int IDLogin { get; set; }

        [Required(ErrorMessage = "{0} é um campo obrigatório.")]
        [Display(Name = "Pessoa")]
        public int IDPessoa { get; set; }

        [Required(ErrorMessage = "{0} é um campo obrigatório.")]
        [Display(Name = "Perfil")]
        public Perfil IDPerfil { get; set; }

        [Required(ErrorMessage = "{0} é um campo obrigatório.")]
        [Display(Name = "Login")]
        [StringLength(50)]
        public string sLogin { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Senha")]
        [StringLength(50)]
        public string sSenha { get; set; }

        [Display(Name = "Confimação")]
        public bool bConfimacao { get; set; }

        [Display(Name = "Ativo")]
        public bool bAtivo { get; set; }

        public DateTime dDataCadastro { get; set; }

        public string sSecurityStamp { get; set; }

        public virtual Pessoa Pessoa { get; set; }
    }

    public enum Perfil : int
    {
        Administrador = 1,
        Usuario = 2,
        Arbitro = 3
    }

    public class RegistroViewModel
    {
        [Required(ErrorMessage = "{0} é um campo obrigatório.")]
        [Display(Name = "Pessoa")]
        public int IDLogin { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "O {0} deve ser, pelo menos, {2} caracteres.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Senha")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirmar Senha")]
        [System.ComponentModel.DataAnnotations.Compare("Password", ErrorMessage = "A senha e a confirmação da senha não coincidem.")]
        public string ConfirmPassword { get; set; }
    }

    public class AcessoViewModel
    {
        [Required]
        [Display(Name = "Login")]
        public string Login { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Senha")]
        public string Password { get; set; }

        [Display(Name = "Mantenha-me conectado?")]
        public bool RememberMe { get; set; }

        [HiddenInput]
        public string ReturnUrl { get; set; }
    }

    public class ResetPasswordViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Senha")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirmar senha")]
        [System.ComponentModel.DataAnnotations.Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        public string Token { get; set; }
    }

    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }

}
