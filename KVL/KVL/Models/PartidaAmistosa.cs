namespace KVL.Models
{
    using Context;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("PartidaAmistosa")]
    public partial class PartidaAmistosa
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public PartidaAmistosa()
        {
            dDataCadastro = DateTime.Now;
            sHoraPartida = DateTime.Now.ToShortTimeString();
            GolAmistoso = new HashSet<GolAmistoso>();
        }

        [Key]
        public int IDPartidaAmistosa { get; set; }

        [Required(ErrorMessage = "{0} é um campo obrigatório.")]
        [Display(Name = "Time 1")]
        public int IDTime1 { get; set; }

        [Required(ErrorMessage = "{0} é um campo obrigatório.")]
        [Display(Name = "Time 2")]
        public int IDTime2 { get; set; }

        [Required(ErrorMessage = "{0} é um campo obrigatório.")]
        [Display(Name = "Gol 1")]
        public int? iQntGols1 { get; set; }

        [Required(ErrorMessage = "{0} é um campo obrigatório.")]
        [Display(Name = "Gol 2")]
        public int? iQntGols2 { get; set; }

        [Required(ErrorMessage = "{0} é um campo obrigatório.")]
        [Display(Name = "Campo")]
        public int? iCodCampo { get; set; }

        [Required(ErrorMessage = "{0} é um campo obrigatório.")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Data da Partida")]
        public DateTime? dDataPartida { get; set; }

        [Display(Name = "Hora da Partida")]
        public string sHoraPartida { get; set; }

        public DateTime? dDataCadastro { get; set; }

        public virtual Campo Campo { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<GolAmistoso> GolAmistoso { get; set; }

        public virtual Time Time { get; set; }

        public virtual Time Time1 { get; set; }

    }
}
