using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using KVL.Context;
using KVL.Models;
using PagedList;
using System;


namespace KVL.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class CampeonatoController : Controller
    {
        private ModeloDados db = new ModeloDados();

        // GET: Campeonato
        //public ActionResult Index()
        //{
        //    var campeonato = db.Campeonato.Include(c => c.Campo).Include(c => c.TipoCampeonato);
        //    return View(campeonato.ToList());
        //}

        public ViewResult Index(string sortOrder, string currentFilter, string searchString, int? page)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.NomeParam = string.IsNullOrEmpty(sortOrder) ? "Nome_desc" : "";
            ViewBag.DtInicioParam = sortOrder == "Dt_asc" ? "Dt_desc" : "Dt_asc";

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;

            var campeonatos = from s in db.Campeonato.Include(c => c.Campo)
                              select s;

            if (!string.IsNullOrEmpty(searchString))
            {
                campeonatos = campeonatos.Where(s => s.sNome.ToUpper().Contains(searchString.ToUpper()));
            }
            switch (sortOrder)
            {
                case "Nome_desc":
                    campeonatos = campeonatos.OrderByDescending(s => s.sNome);
                    break;
                case "Dt_desc":
                    campeonatos = campeonatos.OrderByDescending(s => s.dDataInicio);
                    break;
                case "Dt_asc":
                    campeonatos = campeonatos.OrderBy(s => s.dDataInicio);
                    break;
                default:
                    campeonatos = campeonatos.OrderBy(s => s.sNome);
                    break;
            }

            int pageSize = 10;
            int pageNumber = (page ?? 1);
            return View(campeonatos.ToPagedList(pageNumber, pageSize));
        }


        // GET: Campeonato/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Campeonato campeonato = db.Campeonato.Find(id);
            if (campeonato == null)
            {
                return HttpNotFound();
            }
            return View(campeonato);
        }

        // GET: Campeonato/Create
        public ActionResult Create()
        {
            ViewBag.iCodCampo = new SelectList(db.Campo, "IDCampo", "sNome");
            return View();
        }

        // POST: Campeonato/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "IDCampeonato,sNome,dDataInicio,dDataFim,iTipoCampeonato,iCodCampo,iQuantidadeTimes,bPreInscricao,bIdaVolta,dDataCadastro")] Campeonato campeonato)
        {
            if (ModelState.IsValid)
            {
                db.Campeonato.Add(campeonato);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.iCodCampo = new SelectList(db.Campo, "IDCampo", "sNome", campeonato.iCodCampo);
            return View(campeonato).ComMensagem("Operação realizada com sucesso.", "alert-success");
        }

        // GET: Campeonato/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Campeonato campeonato = db.Campeonato.Find(id);
            if (campeonato == null)
            {
                return HttpNotFound();
            }
            ViewBag.iCodCampo = new SelectList(db.Campo, "IDCampo", "sNome", campeonato.iCodCampo);
            return View(campeonato);
        }

        // POST: Campeonato/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "IDCampeonato,sNome,dDataInicio,dDataFim,iTipoCampeonato,iCodCampo,iQuantidadeTimes,bPreInscricao,bIdaVolta")] Campeonato campeonato)
        {
            if (ModelState.IsValid)
            {
                db.Entry(campeonato).State = EntityState.Modified;
                db.Entry(campeonato).Property("dDataCadastro").IsModified = false;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.iCodCampo = new SelectList(db.Campo, "IDCampo", "sNome", campeonato.iCodCampo);
            return View(campeonato).ComMensagem("Operação realizada com sucesso.", "alert-success");
        }

        // GET: Campeonato/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Campeonato campeonato = db.Campeonato.Find(id);
            if (campeonato == null)
            {
                return HttpNotFound();
            }
            return View(campeonato);
        }

        // POST: Campeonato/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                Campeonato campeonato = db.Campeonato.Find(id);
                db.Campeonato.Remove(campeonato);
                db.SaveChanges();
                return RedirectToAction("Index").ComMensagem("Operação realizada com sucesso.", "alert-success");
            }
            catch (Exception)
            {
                return RedirectToAction("Index").ComMensagem("Este registro não pode ser apagado, ele possui dependências.", "alert-danger");
            }
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
