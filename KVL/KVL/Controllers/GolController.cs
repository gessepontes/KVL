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
    public class GolController : Controller
    {
        private ModeloDados db = new ModeloDados();

        // GET: Gol
        //public ActionResult Index()
        //{
        //    var gol = db.Gol.Include(g => g.JogadorSumula);
        //    return View(gol.ToList());
        //}

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

            var gols = from s in db.Gol.Include(g=> g.JogadorSumula).Where(s=>s.iQuantidade != 0)
                        select s;

            if (!String.IsNullOrEmpty(searchString))
            {
                gols = gols.Where(s => s.JogadorSumula.JogadorInscrito.Jogador.Pessoa.sNome.ToUpper().Contains(searchString.ToUpper()));
            }

            if (IDCampeonato != null)
            {
                gols = gols.Where(s => s.JogadorSumula.Sumula.PartidaCampeonato.Inscrito.PreInscrito.IDCampeonato == IDCampeonato);
            }


            switch (sortOrder)
            {
                case "Nome_desc":
                    gols = gols.OrderByDescending(s => s.JogadorSumula.JogadorInscrito.Jogador.Pessoa.sNome);
                    break;
                case "Nome":
                    gols = gols.OrderBy(s => s.JogadorSumula.JogadorInscrito.Jogador.Pessoa.sNome);
                    break;
                case "Time_desc":
                    gols = gols.OrderByDescending(s => s.JogadorSumula.JogadorInscrito.Inscrito.PreInscrito.Time.sNome);
                    break;
                case "Time":
                    gols = gols.OrderBy(s => s.JogadorSumula.JogadorInscrito.Inscrito.PreInscrito.Time.sNome);
                    break;
                case "Data":
                    gols = gols.OrderBy(s => s.JogadorSumula.Sumula.PartidaCampeonato.dDataPartida);
                    break;
                case "Data_desc":
                    gols = gols.OrderByDescending(s => s.JogadorSumula.Sumula.PartidaCampeonato.dDataPartida);
                    break;
                case "Campeonato":
                    gols = gols.OrderBy(s => s.JogadorSumula.Sumula.PartidaCampeonato.Inscrito.PreInscrito.Campeonato.sNome);
                    break;
                case "Campeonato_desc":
                    gols = gols.OrderByDescending(s => s.JogadorSumula.Sumula.PartidaCampeonato.Inscrito.PreInscrito.Campeonato.sNome);
                    break;

                default:
                    gols = gols.OrderBy(s => s.JogadorSumula.Sumula.PartidaCampeonato.dDataPartida);
                    break;
            }

            int pageSize = 10;
            int pageNumber = (page ?? 1);
            return View(gols.ToPagedList(pageNumber, pageSize));
        }

        [Authorize(Roles = "Administrador")]
        public ViewResult Artilharia(string sortOrder, string currentFilter, string searchString, int? IDCampeonato, int? page)
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

            //var gols = from s in db.Gol.Include(g => g.JogadorSumula).Where(s => s.iQuantidade != 0)
            //           select s;

            var gols = db.Gol.GroupBy(g => g.JogadorSumula.IDJogadorInscrito).ToList();
                       

            //if (!String.IsNullOrEmpty(searchString))
            //{
            //    gols = gols.Where(s => s.JogadorSumula.JogadorInscrito.Jogador.Pessoa.sNome.ToUpper().Contains(searchString.ToUpper()));
            //}

            //if (IDCampeonato != null)
            //{
            //    gols = gols.Where(s => s.JogadorSumula.Sumula.PartidaCampeonato.Inscrito.PreInscrito.IDCampeonato == IDCampeonato);
            //}
            

            int pageSize = 10;
            int pageNumber = (page ?? 1);
            return View(gols.ToPagedList(pageNumber, pageSize));
        }



        // GET: Gol/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Gol gol = db.Gol.Find(id);
            if (gol == null)
            {
                return HttpNotFound();
            }
            return View(gol);
        }

        // GET: Gol/Create
        public ActionResult Create()
        {
            ViewBag.iCodJogadorSumula = new SelectList(db.JogadorSumula, "IDJogadorSumula", "IDJogadorSumula");
            return View();
        }

        // POST: Gol/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "IDGol,iCodJogadorSumula,iQuantidade,dDataCadastro")] Gol gol)
        {
            if (ModelState.IsValid)
            {
                db.Gol.Add(gol);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.iCodJogadorSumula = new SelectList(db.JogadorSumula, "IDJogadorSumula", "IDJogadorSumula", gol.iCodJogadorSumula);
            return View(gol);
        }

        // GET: Gol/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Gol gol = db.Gol.Find(id);
            if (gol == null)
            {
                return HttpNotFound();
            }
            ViewBag.iCodJogadorSumula = new SelectList(db.JogadorSumula, "IDJogadorSumula", "IDJogadorSumula", gol.iCodJogadorSumula);
            return View(gol);
        }

        // POST: Gol/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "IDGol,iCodJogadorSumula,iQuantidade,dDataCadastro")] Gol gol)
        {
            if (ModelState.IsValid)
            {
                db.Entry(gol).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.iCodJogadorSumula = new SelectList(db.JogadorSumula, "IDJogadorSumula", "IDJogadorSumula", gol.iCodJogadorSumula);
            return View(gol);
        }

        // GET: Gol/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Gol gol = db.Gol.Find(id);
            if (gol == null)
            {
                return HttpNotFound();
            }
            return View(gol);
        }

        // POST: Gol/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Gol gol = db.Gol.Find(id);
            db.Gol.Remove(gol);
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
