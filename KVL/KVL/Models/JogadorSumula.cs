namespace KVL.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("JogadorSumula")]
    public partial class JogadorSumula
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public JogadorSumula()
        {
            Cartao = new HashSet<Cartao>();
            Gol = new HashSet<Gol>();
            dDataCadastro = DateTime.Now;
        }

        [Key]
        public int IDJogadorSumula { get; set; }

        public int IDJogadorInscrito { get; set; }

        public int IDSumula { get; set; }

        public int? iNumCamisa { get; set; }

        public DateTime? dDataCadastro { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Cartao> Cartao { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Gol> Gol { get; set; }

        public virtual JogadorInscrito JogadorInscrito { get; set; }

        public virtual Sumula Sumula { get; set; }
    }

    public class JogadorSumulaInscrito {
        public int iNumCamisa { get; set; }
        public int IDJogadorInscrito { get; set; }
        public int IDTimeInscrito { get; set; }
        public int IDSumula { get; set; }
        public string sNome { get; set; }
    }

    public class JogadorSumulaGolCartao
    {
        public int IDJogadorInscrito { get; set; }
        public int IDSumula { get; set; }
        public int IDJogadorSumula { get; set; }
        public TipoCartao iCartao { get; set; }
        public int iGol { get; set; }
        public string sNome { get; set; }
    }

}
