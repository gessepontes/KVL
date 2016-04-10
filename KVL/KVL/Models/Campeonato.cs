namespace KVL.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Campeonato")]
    public partial class Campeonato
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Campeonato()
        {
            PreInscrito = new HashSet<PreInscrito>();
            dDataCadastro = DateTime.Now;
        }

        [Key]
        public int IDCampeonato { get; set; }

        [StringLength(50)]
        [Required(ErrorMessage = "{0} é um campo obrigatório.")]
        [Display(Name = "Nome")]
        public string sNome { get; set; }

        [Required(ErrorMessage = "{0} é um campo obrigatório.")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Data inicio")]
        public DateTime? dDataInicio { get; set; }

        [Required(ErrorMessage = "{0} é um campo obrigatório.")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Data fim")]
        public DateTime? dDataFim { get; set; }

        [Required(ErrorMessage = "{0} é um campo obrigatório.")]
        [Display(Name = "Tipo de campeonato")]
        public TipoCampeonato iTipoCampeonato { get; set; }

        [Required(ErrorMessage = "{0} é um campo obrigatório.")]
        [Display(Name = "Campo")]
        public int iCodCampo { get; set; }

        [Required(ErrorMessage = "{0} é um campo obrigatório.")]
        [Display(Name = "Quantidade de times")]
        public int iQuantidadeTimes { get; set; }

        [Display(Name = "Pré-Inscrição")]
        public bool bPreInscricao { get; set; }

        [Display(Name = "Ida e Volta")]
        public bool bIdaVolta { get; set; }

        public DateTime? dDataCadastro { get; set; }
       
        public virtual Campo Campo { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PreInscrito> PreInscrito { get; set; }
    }

    public enum TipoCampeonato : int
    {
        Grupos = 1,
        [Display(Name = "Mata-Mata")]
        MataMata = 2,
        [Display(Name = "Pontos Corridos")]
        PontosCorridos = 3
    }


    public class Artilharia
    {
        public string sNome { get; set; }
        public int iQuantidade { get; set; }
        public string sTime { get; set; }
    }
}
