namespace KVL.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("HorarioDisponivel")]
    public partial class HorarioDisponivel
    {

        public HorarioDisponivel() {
            dDataCadastro = DateTime.Now;
        }

        [Key]
        public int IDHorario { get; set; }

        [StringLength(10)]
        [Required(ErrorMessage = "{0} é um campo obrigatório.")]
        [Display(Name = "Horário")]
        public string sHora { get; set; }

        [Required(ErrorMessage = "{0} é um campo obrigatório.")]
        [Display(Name = "Time")]
        public int iCodTime { get; set; }

        [Required(ErrorMessage = "{0} é um campo obrigatório.")]
        [Display(Name = "Campo")]
        public int iCodCampo { get; set; }

        [Required(ErrorMessage = "{0} é um campo obrigatório.")]
        [Display(Name = "Dia da Semana")]
        public IDDiaSemana iDiaSemana { get; set; }

        public DateTime? dDataCadastro { get; set; }

        public virtual Campo Campo { get; set; }

        public virtual Time Time { get; set; }
    }

    public enum IDDiaSemana : int
    {

        [Display(Name = "Segunda-Feira")]
        Segunda = 1,
        [Display(Name = "Terça-Feira")]
        Terca = 2,
        [Display(Name = "Quarta-Feira")]
        Quarta = 3,
        [Display(Name = "Quinta-Feira")]
        Quinta = 4,
        [Display(Name = "Sexta-Feira")]
        Sexta = 5,
        [Display(Name = "Sábado")]
        Sabado = 6,
        Domingo = 7,
    }

}
