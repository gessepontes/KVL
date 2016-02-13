using KVL.Context;
using KVL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace KVL.Controllers
{
    [AllowAnonymous]
    public class HomeController : Controller
    {

        private ModeloDados db = new ModeloDados();

        public ActionResult Index()
        {
            string listaeventos = "";
            var id = Session["id"];
            int idPessoa = Convert.ToInt32(id);

            int iVitoria = 0, iEmpate = 0, iDerrota = 0;

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Ranking(11);

            Time time = db.Time.Where(p => p.IDPessoa == idPessoa).FirstOrDefault();
            if (time == null)
            {
                listaeventos = "{title:\'Sem Time',";
                listaeventos = listaeventos + "start: new Date(" + DateTime.Now.Year + "," + (DateTime.Now.Month - 1) + "," + DateTime.Now.Day + "),";
                listaeventos = listaeventos + "backgroundColor: '#0073b7',borderColor: '#0073b7'},";

                ViewBag.eventos = listaeventos;

                ViewBag.iVitoria = iVitoria;
                ViewBag.iDerrota = iDerrota;
                ViewBag.iEmpate = iEmpate;
                ViewBag.sNome = "";
                return View();
            }


            int iTime = time.IDTime;
            Session["IDTime"] = iTime;

            var ultimosTimesA = db.PartidaAmistosa.Where(p => p.IDTime1 == iTime || p.IDTime2 == iTime);
            var ultimosTimesC = db.PartidaCampeonato.Where(p => p.Inscrito.PreInscrito.IDTime == iTime || p.Inscrito1.PreInscrito.IDTime == iTime);

            List<Partida> listaPartidas = new List<Partida>();

            foreach (var item in ultimosTimesC)
            {
                Partida partidaC = new Partida();

                partidaC.sTime1 = item.Inscrito.PreInscrito.Time.sNome.ToUpper();
                partidaC.sTime2 = item.Inscrito1.PreInscrito.Time.sNome.ToUpper();
                partidaC.IDTime1 = item.Inscrito.PreInscrito.IDTime;
                partidaC.IDTime2 = item.Inscrito1.PreInscrito.IDTime;
                partidaC.iQntGols1 = item.iQntGols1;
                partidaC.iQntGols2 = item.iQntGols2;
                partidaC.dDataPartida = item.dDataPartida;
                partidaC.sHoraPartida = item.sHoraPartida;
                partidaC.sCampo = item.Inscrito.PreInscrito.Campeonato.Campo.sNome;
                partidaC.sCampeonato = item.Inscrito.PreInscrito.Campeonato.sNome;
                partidaC.IDCampeonato = item.Inscrito.PreInscrito.IDCampeonato;

                partidaC.sSimbolo1 = item.Inscrito.PreInscrito.Time.sSimbolo;
                partidaC.sSimbolo2 = item.Inscrito1.PreInscrito.Time.sSimbolo;
                partidaC.IDPartida = item.IDPartidaCampeonato;

                listaPartidas.Add(partidaC);
            }

            foreach (var item1 in ultimosTimesA)
            {
                Partida partidaA = new Partida();

                partidaA.sTime1 = item1.Time.sNome.ToUpper();
                partidaA.sTime2 = item1.Time1.sNome.ToUpper();
                partidaA.IDTime1 = item1.IDTime1;
                partidaA.IDTime2 = item1.IDTime2;
                partidaA.iQntGols1 = item1.iQntGols1;
                partidaA.iQntGols2 = item1.iQntGols2;
                partidaA.dDataPartida = item1.dDataPartida;
                partidaA.sHoraPartida = item1.sHoraPartida;
                partidaA.sCampo = item1.Campo.sNome;
                partidaA.sCampeonato = "";
                partidaA.IDCampeonato = 0;

                partidaA.sSimbolo1 = item1.Time.sSimbolo;
                partidaA.sSimbolo2 = item1.Time1.sSimbolo;

                partidaA.IDPartida = item1.IDPartidaAmistosa;

                listaPartidas.Add(partidaA);
            }            

            List<ListaAdversarios> lista = new List<ListaAdversarios>();

            foreach (var item in listaPartidas)
            {
                ListaAdversarios la = new ListaAdversarios();

                if (iTime != item.IDTime1)
                {
                    la.sNome = item.sTime1.ToUpper();
                    la.sSimbolo = item.sSimbolo1;
                }
                else
                {
                    la.sNome = item.sTime2.ToUpper();
                    la.sSimbolo = item.sSimbolo2;
                }
                la.dDataPartida = item.dDataPartida;

                if (item.IDCampeonato == 0)
                {
                    la.iTipoPartida = 1;
                }
                else
                {
                    la.iTipoPartida = 2;
                }

                
                la.IDPartida = item.IDPartida;

                lista.Add(la);

                if (iTime == item.IDTime1)
                {
                    if (item.iQntGols1 > item.iQntGols2)
                    {
                        iVitoria = iVitoria + 1;
                    }
                    else if (item.iQntGols1 < item.iQntGols2)
                    {
                        iDerrota = iDerrota + 1;
                    }
                    else if (item.iQntGols1 == item.iQntGols2)
                    {
                        iEmpate = iEmpate + 1;
                    }
                }
                else
                {
                    if (item.iQntGols1 < item.iQntGols2)
                    {
                        iVitoria = iVitoria + 1;
                    }
                    else if (item.iQntGols1 > item.iQntGols2)
                    {
                        iDerrota = iDerrota + 1;
                    }
                    else if (item.iQntGols1 == item.iQntGols2)
                    {
                        iEmpate = iEmpate + 1;
                    }
                }
            }

            //var eventos = db.PartidaAmistosa.Where(p => p.IDTime1 == iTime || p.IDTime2 == iTime);
            var eventos = listaPartidas;
            string[] listaHorario;

            foreach (var item in eventos)
            {
                listaHorario = item.sHoraPartida.Split(':');

                if (iTime != item.IDTime1)
                {
                    listaeventos = listaeventos + "{title:\'" + item.sTime1.ToUpper() + "',";
                }
                else
                {
                    listaeventos = listaeventos + "{title:\'" + item.sTime2.ToUpper() + "',";
                }

                listaeventos = listaeventos + "start: new Date(" + item.dDataPartida.Value.Year + "," + (item.dDataPartida.Value.Month - 1) + "," + item.dDataPartida.Value.Day + "," + listaHorario[0] + "," + listaHorario[1].Substring(0, 2) + "),";
                listaeventos = listaeventos + "backgroundColor: '#0073b7',borderColor: '#0073b7'},";


            }

            ViewBag.listaAdversarios = lista.OrderByDescending(p => p.dDataPartida).Take(8).ToList(); ;

            if (listaeventos.Length != 0)
            {
                ViewBag.eventos = listaeventos.Remove(listaeventos.Length - 1);
            }

            ViewBag.iVitoria = iVitoria;
            ViewBag.iDerrota = iDerrota;
            ViewBag.iEmpate = iEmpate;
            ViewBag.sNome = time.sNome;

            return View();
        }

        public void Ranking(int iQnt)
        {

            List<Time> listaTimes = db.Time.ToList();

            int pontuacao;
            List<Ranking> listaRanking = new List<Ranking>();

            foreach (var item in listaTimes)
            {
                pontuacao = 0;

                List<PartidaAmistosa> listaPartidasAmistosas = db.PartidaAmistosa.Where(p => p.IDTime1 == item.IDTime || p.IDTime2 == item.IDTime).ToList();
                List<PartidaCampeonato> listaPartidasCampeonato = db.PartidaCampeonato.Where(p => p.Inscrito.PreInscrito.IDTime == item.IDTime || p.Inscrito1.PreInscrito.IDTime == item.IDTime).ToList();

                foreach (var item2 in listaPartidasAmistosas)
                {
                    if (item.IDTime == item2.IDTime1)
                    {
                        if (item2.iQntGols1 > item2.iQntGols2)
                        {
                            pontuacao = pontuacao + 3;
                        }
                        else if (item2.iQntGols1 == item2.iQntGols2)
                        {
                            pontuacao = pontuacao + 1;
                        }
                    }
                    else
                    {
                        if (item2.iQntGols1 < item2.iQntGols2)
                        {
                            pontuacao = pontuacao + 3;
                        }
                        else if (item2.iQntGols1 == item2.iQntGols2)
                        {
                            pontuacao = pontuacao + 1;
                        }
                    }
                }

                foreach (var item2 in listaPartidasCampeonato)
                {
                    if (item.IDTime == item2.Inscrito.PreInscrito.IDTime)
                    {
                        if (item2.iQntGols1 > item2.iQntGols2)
                        {
                            pontuacao = pontuacao + 3;
                        }
                        else if (item2.iQntGols1 == item2.iQntGols2)
                        {
                            pontuacao = pontuacao + 1;
                        }
                    }
                    else
                    {
                        if (item2.iQntGols1 < item2.iQntGols2)
                        {
                            pontuacao = pontuacao + 3;
                        }
                        else if (item2.iQntGols1 == item2.iQntGols2)
                        {
                            pontuacao = pontuacao + 1;
                        }
                    }
                }


                Ranking ranking = new Ranking();
                ranking.sNome = item.sNome;
                ranking.iPontuacao = pontuacao;
                listaRanking.Add(ranking);

            }
            if (iQnt == 0)
            {
                ViewBag.Ranking = listaRanking.OrderByDescending(p => p.iPontuacao).ToList();
            }
            else
            {
                ViewBag.Ranking = listaRanking.OrderByDescending(p => p.iPontuacao).Take(iQnt).ToList();
            }

        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }


        public ActionResult DadosPartida(int? id, int? iTipo)
        {
            Partida partida = new Partida();

            if (iTipo == 1)
            {
                var partidaAmistosa = db.PartidaAmistosa.Find(id);

                partida.sTime1 = partidaAmistosa.Time.sNome;
                partida.sTime2 = partidaAmistosa.Time1.sNome;
                partida.iQntGols1 = partidaAmistosa.iQntGols1;
                partida.iQntGols2 = partidaAmistosa.iQntGols2;
                partida.sSimbolo1 = partidaAmistosa.Time.sSimbolo;
                partida.sSimbolo2 = partidaAmistosa.Time1.sSimbolo;
                partida.sCampo = partidaAmistosa.Campo.sNome;
                partida.dDataPartida = partidaAmistosa.dDataPartida;
            }
            else
            {
                var partidaCampeonato = db.PartidaCampeonato.Find(id);

                partida.sTime1 = partidaCampeonato.Inscrito.PreInscrito.Time.sNome;
                partida.sTime2 = partidaCampeonato.Inscrito1.PreInscrito.Time.sNome;
                partida.iQntGols1 = partidaCampeonato.iQntGols1;
                partida.iQntGols2 = partidaCampeonato.iQntGols2;
                partida.sSimbolo1 = partidaCampeonato.Inscrito.PreInscrito.Time.sSimbolo;
                partida.sSimbolo2 = partidaCampeonato.Inscrito1.PreInscrito.Time.sSimbolo;
                partida.sCampo = partidaCampeonato.Inscrito.PreInscrito.Campeonato.sNome;
                partida.dDataPartida = partidaCampeonato.dDataPartida;
            }

            return PartialView("_DadosPartida", partida);
        }


        public ActionResult RankingCompleto()
        {
            Ranking(0);
            return PartialView("_RankingCompleto");
        }
    }
}