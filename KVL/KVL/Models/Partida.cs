using System;
using System.ComponentModel.DataAnnotations;

namespace KVL.Models
{

    public class Partida
    {
        public string sTime1 { get; set; }
        public string sTime2 { get; set; }
        public string sCampeonato { get; set; }
        public string sCampo { get; set; }
        public string sSimbolo1 { get; set; }
        public string sSimbolo2 { get; set; }

        [DataType(DataType.Date), DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? dDataPartida { get; set; }
        public int? iQntGols1 { get; set; }
        public int? iQntGols2 { get; set; }
        public int? IDCampeonato { get; set; }
        public string sHoraPartida { get; set; }
        public int iTipoPartida { get; set; }
        public int IDTime1 { get; set; }
        public int IDTime2 { get; set; }
        public int IDPartida { get; set; }

    }

    public class Ranking
    {
        public string sNome { get; set; }
        public int iPontuacao { get; set; }
    }


    ////Classe para listar os ultimos adversarios no index home
    public class ListaAdversarios
    {
        public string sNome { get; set; }
        public string sSimbolo { get; set; }
        public string sHoraPartida { get; set; }
        public int iTipoPartida { get; set; }
        public int IDPartida { get; set; }
        public DateTime? dDataPartida { get; set; }
    }
}
