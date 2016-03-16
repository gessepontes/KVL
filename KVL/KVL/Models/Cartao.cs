namespace KVL.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("Cartao")]
    public partial class Cartao
    {
        public Cartao()
        {
            dDataCadastro = DateTime.Now;
        }

        [Key]
        public int IDCartao { get; set; }

        public int iCodJogadorSumula { get; set; }

        public TipoCartao iTipoCartao { get; set; }

        public DateTime? dDataCadastro { get; set; }

        public virtual JogadorSumula JogadorSumula { get; set; }
    }

    public enum TipoCartao : int
    {
        [Display(Name = "Nenhum")]
        Nenhum = 0,
        [Display(Name = "Amarelo")]
        Amarelo = 1,
        [Display(Name = "Vermelho")]
        Vermelho = 2,
        [Display(Name = "Segundo amarelo seguido de vermelho")]
        SegundoAmareloVermelho = 3,
        [Display(Name = "Vermelho depois de um amarelo")]
        VermelhoAmarelo = 4
    }

}
