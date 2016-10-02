using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using KVL.Context;
using KVL.Models;
using PagedList;


namespace KVL.Controllers
{
    public class CartaoController : Controller
    {
        private ModeloDados db = new ModeloDados();

        [Authorize(Roles = "Administrador")]
        public ViewResult Index(string sortOrder, string currentFilter, string searchString, int? IDCampeonato, int? page)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.NomeParam = sortOrder == "Nome" ? "Nome_desc" : "Nome";
            ViewBag.TimeParm = sortOrder == "Time" ? "Time_desc" : "Time";
            ViewBag.DataParm = sortOrder == "Data" ? "Data_desc" : "Data";
            ViewBag.CampeonatoParm = sortOrder == "Campeonato" ? "Campeonato_desc" : "Campeonato";
            ViewBag.ListaCampeonato = db.Campeonato;

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;

            var cartao = db.Cartao.Include(c => c.JogadorSumula).Where(c => c.iTipoCartao != TipoCartao.Nenhum);

            if (!String.IsNullOrEmpty(searchString))
            {
                cartao = cartao.Where(s => s.JogadorSumula.JogadorInscrito.Jogador.Pessoa.sNome.ToUpper().Contains(searchString.ToUpper()));
            }

            if (IDCampeonato != null)
            {
                cartao = cartao.Where(s => s.JogadorSumula.Sumula.PartidaCampeonato.Inscrito.PreInscrito.IDCampeonato == IDCampeonato);
            }


            switch (sortOrder)
            {
                case "Nome_desc":
                    cartao = cartao.OrderByDescending(s => s.JogadorSumula.JogadorInscrito.Jogador.Pessoa.sNome);
                    break;
                case "Nome":
                    cartao = cartao.OrderBy(s => s.JogadorSumula.JogadorInscrito.Jogador.Pessoa.sNome);
                    break;
                case "Time_desc":
                    cartao = cartao.OrderByDescending(s => s.JogadorSumula.JogadorInscrito.Inscrito.PreInscrito.Time.sNome);
                    break;
                case "Time":
                    cartao = cartao.OrderBy(s => s.JogadorSumula.JogadorInscrito.Inscrito.PreInscrito.Time.sNome);
                    break;
                case "Data":
                    cartao = cartao.OrderBy(s => s.JogadorSumula.Sumula.PartidaCampeonato.dDataPartida);
                    break;
                case "Data_desc":
                    cartao = cartao.OrderByDescending(s => s.JogadorSumula.Sumula.PartidaCampeonato.dDataPartida);
                    break;
                case "Campeonato":
                    cartao = cartao.OrderBy(s => s.JogadorSumula.Sumula.PartidaCampeonato.Inscrito.PreInscrito.Campeonato.sNome);
                    break;
                case "Campeonato_desc":
                    cartao = cartao.OrderByDescending(s => s.JogadorSumula.Sumula.PartidaCampeonato.Inscrito.PreInscrito.Campeonato.sNome);
                    break;

                default:
                    cartao = cartao.OrderBy(s => s.JogadorSumula.Sumula.PartidaCampeonato.dDataPartida);
                    break;
            }

            int pageSize = 10;
            int pageNumber = (page ?? 1);
            return View(cartao.ToPagedList(pageNumber, pageSize));
        }


        private int[] QuantidadeRodadas(int iCodCampeonato)
        {
            int quantidade = 0;
            quantidade = db.Inscrito.Where(p => p.PreInscrito.IDCampeonato == iCodCampeonato).Count();

            if (quantidade == 0)
            {
                quantidade = 1;
            }

            int[] lista = new int[quantidade - 1];

            for (int i = 0; i < quantidade - 1; i++)
            {
                lista[i] = i + 1;
            }

            return lista;
        }

        public ActionResult Suspenso()
        {
            ViewBag.ListaCampeonato = db.Campeonato;
            return View();
        }

        public ActionResult SuspensoDetails(int? IDRodada, int ?IDCampeonato)
        {

            ViewBag.CampeonatoNome = db.Campeonato.Find(IDCampeonato).sNome;
            ViewBag.Rodada = IDRodada;

            IDRodada = IDRodada - 1;

            ViewBag.SuspensoDetails = db.Cartao
                .Where(p => p.iTipoCartao == TipoCartao.SegundoAmareloVermelho || p.iTipoCartao == TipoCartao.Vermelho || p.iTipoCartao == TipoCartao.VermelhoAmarelo)
                .Where(s => s.JogadorSumula.Sumula.PartidaCampeonato.iRodada == IDRodada && s.JogadorSumula.Sumula.PartidaCampeonato.Inscrito.PreInscrito.IDCampeonato == IDCampeonato).ToList()
                .OrderBy(p => p.JogadorSumula.JogadorInscrito.Jogador.Pessoa.sNome);

            return View("SuspensoDetails");
        }

        public ActionResult ListaRodada(int? IDCampeonato)
        {

            if (IDCampeonato == null)
            {
                IDCampeonato = 0;
            }

            return Json(QuantidadeRodadas(IDCampeonato??0), JsonRequestBehavior.AllowGet);
        }


        // GET: Cartao/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Cartao cartao = db.Cartao.Find(id);
            if (cartao == null)
            {
                return HttpNotFound();
            }
            return View(cartao);
        }

        // GET: Cartao/Create
        public ActionResult Create()
        {
            ViewBag.iCodJogadorSumula = new SelectList(db.JogadorSumula, "IDJogadorSumula", "IDJogadorSumula");
            return View();
        }

        // POST: Cartao/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "IDCartao,iCodJogadorSumula,iTipoCartao,dDataCadastro")] Cartao cartao)
        {
            if (ModelState.IsValid)
            {
                db.Cartao.Add(cartao);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.iCodJogadorSumula = new SelectList(db.JogadorSumula, "IDJogadorSumula", "IDJogadorSumula", cartao.iCodJogadorSumula);
            return View(cartao);
        }

        // GET: Cartao/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Cartao cartao = db.Cartao.Find(id);
            if (cartao == null)
            {
                return HttpNotFound();
            }
            ViewBag.iCodJogadorSumula = new SelectList(db.JogadorSumula, "IDJogadorSumula", "IDJogadorSumula", cartao.iCodJogadorSumula);
            return View(cartao);
        }

        // POST: Cartao/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "IDCartao,iCodJogadorSumula,iTipoCartao,dDataCadastro")] Cartao cartao)
        {
            if (ModelState.IsValid)
            {
                db.Entry(cartao).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.iCodJogadorSumula = new SelectList(db.JogadorSumula, "IDJogadorSumula", "IDJogadorSumula", cartao.iCodJogadorSumula);
            return View(cartao);
        }

        // GET: Cartao/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Cartao cartao = db.Cartao.Find(id);
            if (cartao == null)
            {
                return HttpNotFound();
            }
            return View(cartao);
        }

        // POST: Cartao/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Cartao cartao = db.Cartao.Find(id);
            db.Cartao.Remove(cartao);
            db.SaveChanges();
            return RedirectToAction("Index");
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
