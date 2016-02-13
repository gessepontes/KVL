namespace KVL.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("Time")]
    public partial class Time
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Time()
        {
            HorarioDisponivel = new HashSet<HorarioDisponivel>();
            Jogador = new HashSet<Jogador>();
            PartidaAmistosa = new HashSet<PartidaAmistosa>();
            PartidaAmistosa1 = new HashSet<PartidaAmistosa>();
            PreInscrito = new HashSet<PreInscrito>();
            dDataCadastro = DateTime.Now;
        }

        [Key]
        public int IDTime { get; set; }

        [Display(Name = "Responsável")]
        public int IDPessoa { get; set; }

        [Required(ErrorMessage = "{0} é um campo obrigatório.")]
        [StringLength(50)]
        [Display(Name = "Time")]
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
        [Display(Name = "Símbolo")]
        public string sSimbolo { get; set; }

        [Required(ErrorMessage = "{0} é um campo obrigatório.")]
        [Column(TypeName = "text")]
        [Display(Name = "Observações")]
        public string sObservacao { get; set; }

        public DateTime? dDataCadastro { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<HorarioDisponivel> HorarioDisponivel { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Jogador> Jogador { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PartidaAmistosa> PartidaAmistosa { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PartidaAmistosa> PartidaAmistosa1 { get; set; }

        public virtual Pessoa Pessoa { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PreInscrito> PreInscrito { get; set; }
    }
}
