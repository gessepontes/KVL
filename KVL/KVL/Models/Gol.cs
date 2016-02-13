namespace KVL.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("Gol")]
    public partial class Gol
    {
        public Gol()
        {
            dDataCadastro = DateTime.Now;
        }


        [Key]
        public int IDGol { get; set; }

        public int iCodJogadorSumula { get; set; }

        public int? iQuantidade { get; set; }

        public DateTime? dDataCadastro { get; set; }
        
        public virtual JogadorSumula JogadorSumula { get; set; }
    }
}
