namespace KVL.Models
{
    using Models;
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("GolAmistoso")]
    public partial class GolAmistoso
    {
        public GolAmistoso()
        {
            dDataCadastro = DateTime.Now;
        }

        [Key]
        public int IDGol { get; set; }

        public int IDJogador { get; set; }

        public int IDPartida { get; set; }

        public int IDTime { get; set; }

        public int iQuantidade { get; set; }

        public DateTime? dDataCadastro { get; set; }

        public virtual Jogador Jogador { get; set; }

        public virtual Time Time { get; set; }

        public virtual PartidaAmistosa PartidaAmistosa { get; set; }
    }
}
