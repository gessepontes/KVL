namespace KVL.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("Inscrito")]
    public partial class Inscrito
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Inscrito()
        {
            JogadorInscrito = new HashSet<JogadorInscrito>();
            PartidaCampeonato = new HashSet<PartidaCampeonato>();
            PartidaCampeonato1 = new HashSet<PartidaCampeonato>();
            CampeonatoGrupo = new HashSet<CampeonatoGrupo>();
            dDataCadastro = DateTime.Now;
            bAtivo = true;
        }

        [Key]
        public int IDInscrito { get; set; }

        public int IDPreInscrito { get; set; }

        public bool? bAtivo { get; set; }

        public DateTime? dDataCadastro { get; set; }

        public virtual PreInscrito PreInscrito { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<JogadorInscrito> JogadorInscrito { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PartidaCampeonato> PartidaCampeonato { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PartidaCampeonato> PartidaCampeonato1 { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CampeonatoGrupo> CampeonatoGrupo { get; set; }
    }
}
