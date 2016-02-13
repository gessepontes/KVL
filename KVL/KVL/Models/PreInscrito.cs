namespace KVL.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("PreInscrito")]
    public partial class PreInscrito
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public PreInscrito()
        {
            Inscrito = new HashSet<Inscrito>();
            dDataCadastro = DateTime.Now;
        }

        [Key]
        public int IDPreInscrito { get; set; }

        [Required(ErrorMessage = "{0} é um campo obrigatório.")]
        [Display(Name = "Time")]
        public int IDTime { get; set; }

        [Required(ErrorMessage = "{0} é um campo obrigatório.")]
        [Display(Name = "Campeonato")]
        public int IDCampeonato { get; set; }

        public DateTime? dDataCadastro { get; set; }

        public virtual Campeonato Campeonato { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Inscrito> Inscrito { get; set; }

        public virtual Time Time { get; set; }
    }
}
