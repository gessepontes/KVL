namespace KVL.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("Pagamento")]
    public partial class Pagamento
    {
        public Pagamento()
        {
            dDataCadastro = DateTime.Now;
        }

        [Key]
        public int IDPagamento { get; set; }

        [Required(ErrorMessage = "{0} é um campo obrigatório.")]
        [Display(Name = "Login")]
        public int IDLogin { get; set; }

        [Required(ErrorMessage = "{0} é um campo obrigatório.")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Data inicio")]
        public DateTime dDataInicio { get; set; }

        [Required(ErrorMessage = "{0} é um campo obrigatório.")]
        [Display(Name = "Quantidade de mês pago")]
        public int iQuantidadeMes { get; set; }

        [Range(10, 999.99, ErrorMessage = "O Venda deve estar entre " + "10,00 e 999,99.")]
        [Display(Name = "Valor Pago")]
        [Required(ErrorMessage = "{0} é um campo obrigatório.")]
        [Column(TypeName = "money")]
        public decimal mValorPago { get; set; }

        [Range(10, 999.99, ErrorMessage = "O Venda deve estar entre " + "10,00 e 999,99.")]
        [Display(Name = "Valor Real")]
        [Required(ErrorMessage = "{0} é um campo obrigatório.")]
        [Column(TypeName = "money")]
        public decimal mValorReal { get; set; }

        public DateTime dDataCadastro { get; set; }

        public virtual Login Login { get; set; }
    }
    
}
