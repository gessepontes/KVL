namespace KVL.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Sumula")]
    public partial class Sumula
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Sumula()
        {
            JogadorSumula = new HashSet<JogadorSumula>();
            dDataCadastro = DateTime.Now;
        }

        [Key]
        public int IDSumula { get; set; }

        public int IDPartidaCampeonato { get; set; }

        [Column(TypeName = "text")]
        public string sObservacao { get; set; }

        public DateTime? dDataCadastro { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<JogadorSumula> JogadorSumula { get; set; }

        public virtual PartidaCampeonato PartidaCampeonato { get; set; }
    }
}
