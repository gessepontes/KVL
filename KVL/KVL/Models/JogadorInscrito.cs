namespace KVL.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("JogadorInscrito")]
    public partial class JogadorInscrito
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public JogadorInscrito()
        {
            JogadorSumula = new HashSet<JogadorSumula>();
            dDataCadastro = DateTime.Now;
        }

        [Key]
        public int IDJogadorInscrito { get; set; }

        public int IDJogador { get; set; }

        public int IDInscrito { get; set; }

        public DateTime? dDataCadastro { get; set; }

        public virtual Inscrito Inscrito { get; set; }

        public virtual Jogador Jogador { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<JogadorSumula> JogadorSumula { get; set; }
    }
}
