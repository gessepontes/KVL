using System.Linq;
using System.Web.Mvc;
using KVL.Context;
using KVL.Models;
using System.Collections.Generic;

namespace KVL.Controllers
{
    public class GolAmistosoController : Controller
    {
        private ModeloDados db = new ModeloDados();

        // GET: GolAmistoso
        [Authorize(Roles = "Administrador,Usuario")]
        public ActionResult Index(int IDTime, int IDPartida, int iTipoPartida)
        {

            List<GolAmistoso> jogadorGol = new List<GolAmistoso>();

            if (iTipoPartida == 0)
            {
                PartidaAmistosa partidaAmistosa = db.PartidaAmistosa.Find(IDPartida);
                ViewBag.Partida = "Partida Amistosa - " + partidaAmistosa.Time.sNome.ToUpper() + " " + partidaAmistosa.iQntGols1 + " X " + partidaAmistosa.iQntGols2 + " " + partidaAmistosa.Time1.sNome.ToUpper();

                jogadorGol = (from j in db.Jogador.Where(p => p.IDTime == IDTime && p.bAtivo == true)
                              join g in db.GolAmistoso.Where(p => p.IDPartida == IDPartida)
                              on j.IDJogador equals g.IDJogador into jg
                              from g in jg.DefaultIfEmpty()
                              select new
                              {
                                  IDJogador = j.IDJogador,
                                  Jogador = j,
                                  IDGol = (int?)g.IDGol ?? 0,
                                  iQuantidade = (int?)g.iQuantidade ?? 0
                              }).OrderBy(p => p.Jogador.Pessoa.sNome).ToList()
                                       .Select(x => new GolAmistoso()
                                       {
                                           IDGol = x.IDGol,
                                           IDJogador = x.IDJogador,
                                           iQuantidade = x.iQuantidade,
                                           Jogador = x.Jogador
                                       }).ToList();

            }
            else
            {
                PartidaCampeonato partidaCampeonato = db.PartidaCampeonato.Find(IDPartida);
                ViewBag.Partida = "Campeonato - " + partidaCampeonato.Inscrito.PreInscrito.Campeonato.sNome + " : " + partidaCampeonato.Inscrito.PreInscrito.Time.sNome.ToUpper() + " " + partidaCampeonato.iQntGols1 + " X " + partidaCampeonato.iQntGols2 + " " + partidaCampeonato.Inscrito1.PreInscrito.Time.sNome.ToUpper();

                jogadorGol = (from j in db.Jogador.Where(p => p.IDTime == IDTime && p.bAtivo == true)
                              join g in db.Gol.Where(p => p.JogadorSumula.Sumula.IDPartidaCampeonato == IDPartida)
                              on j.IDJogador equals g.JogadorSumula.JogadorInscrito.IDJogador into jg
                              from g in jg.DefaultIfEmpty()
                              select new
                              {
                                  IDJogador = j.IDJogador,
                                  Jogador = j,
                                  IDGol = (int?)g.IDGol ?? 0,
                                  iQuantidade = (int?)g.iQuantidade ?? 0
                              }).OrderBy(p => p.Jogador.Pessoa.sNome).ToList()
                                       .Select(x => new GolAmistoso()
                                       {
                                           IDGol = x.IDGol,
                                           IDJogador = x.IDJogador,
                                           iQuantidade = x.iQuantidade,
                                           Jogador = x.Jogador
                                       }).ToList();
            }

            ViewBag.IDPartida = IDPartida;
            ViewBag.iTipoPartida = iTipoPartida;
            ViewBag.QuantidadeGols = new int[] { 0, 1, 2, 3, 5, 6, 7, 8, 9 };
            return View(jogadorGol);
        }


        // POST: GolAmistoso/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "IDGol,IDJogador,IDPartida,iQuantidade,dDataCadastro")] GolAmistoso golAmistoso)
        {
            int iQntGolsPartida, iQntGols = 0;

            Jogador jogador = db.Jogador.Where(p => p.IDJogador == golAmistoso.IDJogador).FirstOrDefault();
            PartidaAmistosa partida = db.PartidaAmistosa.Find(golAmistoso.IDPartida);

            if (partida.IDTime1 == jogador.IDTime)
            {
                iQntGolsPartida = partida.iQntGols1 ?? 0;
            }
            else
            {
                iQntGolsPartida = partida.iQntGols2 ?? 0;
            }

            var QntGols = db.GolAmistoso.Where(p => p.IDPartida == golAmistoso.IDPartida && p.IDTime == golAmistoso.IDTime).ToList();

            foreach (var item in QntGols)
            {
                iQntGols = iQntGols + item.iQuantidade ;
            }

            if (golAmistoso.iQuantidade + iQntGols <= iQntGolsPartida)
            {
                golAmistoso.IDTime = jogador.IDTime;
                db.GolAmistoso.Add(golAmistoso);
                db.SaveChanges();
                return RedirectToAction("Index", "GolAmistoso", new { IDTime = jogador.IDTime, IDPartida = golAmistoso.IDPartida, iTipoPartida = 0 }).ComMensagem("Operação realizada com sucesso.", "alert-success");
            }
            else {
                return RedirectToAction("Index", "GolAmistoso", new { IDTime = jogador.IDTime, IDPartida = golAmistoso.IDPartida, iTipoPartida = 0 }).ComMensagem("A quantidade de gols inseridos superam a quantidade de gols da partida.", "alert-warning");
            }
        }

        // POST: GolAmistoso/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int IDGol)
        {
            GolAmistoso golAmistoso = db.GolAmistoso.Find(IDGol);
            db.GolAmistoso.Remove(golAmistoso);
            db.SaveChanges();

            Jogador jogador = db.Jogador.Where(p => p.IDJogador == golAmistoso.IDJogador).FirstOrDefault();
            return RedirectToAction("Index", "GolAmistoso", new { IDTime = jogador.IDTime, IDPartida = golAmistoso.IDPartida, iTipoPartida = 0 }).ComMensagem("Operação realizada com sucesso.", "alert-success");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
