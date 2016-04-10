namespace KVL.Models
{
    using Context;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("Jogador")]
    public partial class Jogador
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Jogador()
        {
            GolAmistoso = new HashSet<GolAmistoso>();
            JogadorInscrito = new HashSet<JogadorInscrito>();
            dDataCadastro = DateTime.Now;
        }

        [Key]
        public int IDJogador { get; set; }

        [Display(Name = "Pessoa")]
        public int IDPessoa { get; set; }

        [Display(Name = "Posição")]
        public int IDPosicao { get; set; }

        [Display(Name = "Time")]
        public int IDTime { get; set; }

        public DateTime? dDataCadastro { get; set; }

        public bool? bAtivo { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<GolAmistoso> GolAmistoso { get; set; }

        public virtual Pessoa Pessoa { get; set; }

        public virtual Posicao Posicao { get; set; }

        public virtual Time Time { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<JogadorInscrito> JogadorInscrito { get; set; }
    }

}
