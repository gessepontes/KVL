namespace KVL.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("PartidaCampeonato")]
    public partial class PartidaCampeonato
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public PartidaCampeonato()
        {
            Sumula = new HashSet<Sumula>();
            dDataCadastro = DateTime.Now;
        }

        [Key]
        public int IDPartidaCampeonato { get; set; }

        [Required(ErrorMessage = "{0} é um campo obrigatório.")]
        [Display(Name = "Time 1")]
        public int IDInscrito1 { get; set; }

        [Required(ErrorMessage = "{0} é um campo obrigatório.")]
        [Display(Name = "Time 2")]
        public int IDInscrito2 { get; set; }

        [Required(ErrorMessage = "{0} é um campo obrigatório.")]
        [Display(Name = "Gol time 1")]
        public int? iQntGols1 { get; set; }

        [Required(ErrorMessage = "{0} é um campo obrigatório.")]
        [Display(Name = "Gol time 2")]
        public int? iQntGols2 { get; set; }

        [Required(ErrorMessage = "{0} é um campo obrigatório.")]
        [Display(Name = "Rodada")]
        public int? iRodada { get; set; }

        [Required(ErrorMessage = "{0} é um campo obrigatório.")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Data da Partida")]
        public DateTime? dDataPartida { get; set; }

        [StringLength(10)]
        [Display(Name = "Hora da Partida")]
        public string sHoraPartida { get; set; }

        public DateTime? dDataCadastro { get; set; }      

        public virtual Inscrito Inscrito { get; set; }

        public virtual Inscrito Inscrito1 { get; set; }
       
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Sumula> Sumula { get; set; }
    }
}
