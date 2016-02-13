namespace KVL.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Pessoa")]
    public partial class Pessoa
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Pessoa()
        {
            Jogador = new HashSet<Jogador>();
            Login = new HashSet<Login>();
            Time = new HashSet<Time>();

            dDataCadastro = DateTime.Now;
        }

        [Key]
        public int IDPessoa { get; set; }

        [Required(ErrorMessage = "{0} é um campo obrigatório.")]
        [StringLength(100)]
        [Display(Name = "Nome")]
        // public string sNome { get; set; }
        private string _sNome;
        public string sNome
        {
            get
            {
                if (string.IsNullOrEmpty(_sNome))
                {
                    return _sNome;
                }
                return _sNome.ToUpper();
            }
            set
            {
                _sNome = value;
            }
        }

        [Required(ErrorMessage = "{0} é um campo obrigatório.")]
        [StringLength(50)]
        [Display(Name = "Telefone")]
        public string sTelefone { get; set; }

        [Required(ErrorMessage = "{0} é um campo obrigatório.")]
        [StringLength(50)]
        [Display(Name = "Cpf")]
        public string sCpf { get; set; }

        [StringLength(1)]
        public string cSexo { get; set; }

        [StringLength(50)]
        [Display(Name = "Rg")]
        public string sRg { get; set; }

        [Display(Name = "Foto")]
        public string sFoto { get; set; }

        [Required(ErrorMessage = "{0} é um campo obrigatório.")]
        [DataType(DataType.Date)]
        [Display(Name = "Data de Nasc.")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? dDataNascimento { get; set; }

        [Required(ErrorMessage = "{0} é um campo obrigatório.")]
        [StringLength(100)]
        [EmailAddress(ErrorMessage = "Email inválido.")]
        [Display(Name = "Email")]
        public string sEmail { get; set; }

        public DateTime dDataCadastro { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Jogador> Jogador { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Login> Login { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Time> Time { get; set; }
    }
}
