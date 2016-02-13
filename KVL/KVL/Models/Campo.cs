namespace KVL.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("Campo")]
    public partial class Campo
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Campo()
        {
            PartidaAmistosa = new HashSet<PartidaAmistosa>();
            Campeonato = new HashSet<Campeonato>();
            HorarioDisponivel = new HashSet<HorarioDisponivel>();
            dDataCadastro = DateTime.Now;
        }

        [Key]
        public int IDCampo { get; set; }

        [StringLength(50)]
        [Required(ErrorMessage = "{0} é um campo obrigatório.")]
        [Display(Name = "Campo")]
        //public string sNome { get; set; }
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

        [StringLength(100)]
        [Required(ErrorMessage = "{0} é um campo obrigatório.")]
        [Display(Name = "Endereço")]
        public string sEndereco { get; set; }

        [Range(10, 999.99, ErrorMessage = "O Venda deve estar entre " + "10,00 e 999,99.")]
        [Display(Name = "Valor")]
        [Required(ErrorMessage = "{0} é um campo obrigatório.")]
        [Column(TypeName = "money")]

        public decimal? mValor { get; set; }

        [Range(10, 999.99, ErrorMessage = "O Venda deve estar entre " + "10,00 e 999,99.")]
        [Display(Name = "Valor Mensal")]
        [Required(ErrorMessage = "{0} é um campo obrigatório.")]
        [Column(TypeName = "money")]
        public decimal? mValorMensal { get; set; }

        public DateTime? dDataCadastro { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Campeonato> Campeonato { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PartidaAmistosa> PartidaAmistosa { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<HorarioDisponivel> HorarioDisponivel { get; set; }
    }
}
