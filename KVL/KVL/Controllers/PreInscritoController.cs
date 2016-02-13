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

    public class PreInscritoController : Controller
    {
        private ModeloDados db = new ModeloDados();

        [Authorize(Roles = "Administrador")]
        public ViewResult Index(string sortOrder, string currentFilter, string searchString, int? page)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.NomeParam = string.IsNullOrEmpty(sortOrder) ? "Nome_desc" : "";

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
                default:
                    campeonatos = campeonatos.OrderBy(s => s.sNome);
                    break;
            }

            int pageSize = 10;
            int pageNumber = (page ?? 1);
            return View(campeonatos.ToPagedList(pageNumber, pageSize));
        }

        [Authorize(Roles = "Administrador,Usuario")]
        public ViewResult IndexTime(string sortOrder, string currentFilter, string searchString, int? page)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.NomeParam = string.IsNullOrEmpty(sortOrder) ? "Nome_desc" : "";

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;
            int IDTime = Convert.ToInt16(Session["IDTime"]);

            var preinscrito = from s in db.PreInscrito.Include(t => t.Time).Include(c => c.Campeonato).Where(p => p.IDTime == IDTime)
                              select s;

            if (!string.IsNullOrEmpty(searchString))
            {
                preinscrito = preinscrito.Where(s => s.Campeonato.sNome.ToUpper().Contains(searchString.ToUpper()));
            }
            switch (sortOrder)
            {
                case "Nome_desc":
                    preinscrito = preinscrito.OrderByDescending(s => s.Campeonato.sNome);
                    break;
                default:
                    preinscrito = preinscrito.OrderBy(s => s.Campeonato.sNome);
                    break;
            }

            int pageSize = 10;
            int pageNumber = (page ?? 1);
            return View(preinscrito.ToPagedList(pageNumber, pageSize));
        }

        // GET: PreInscrito/Details/5
        [Authorize(Roles = "Administrador")]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var preInscrito = db.PreInscrito.Include(p => p.Campeonato).Include(p => p.Time).Where(p => p.IDCampeonato == id).OrderBy(p => p.Time.sNome);


            //PreInscrito preInscrito = db.PreInscrito.Find(id);
            if (preInscrito == null)
            {
                return HttpNotFound();
            }
            return View(preInscrito.ToList());
        }

        // GET: PreInscrito/Create
        [Authorize(Roles = "Administrador")]
        public ActionResult Create()
        {
            var dta = DateTime.Now.Date;
            ViewBag.IDCampeonato = new SelectList(db.Campeonato.Where(p => DbFunctions.TruncateTime(p.dDataFim) >= dta && p.bPreInscricao == true), "IDCampeonato", "sNome");
            ViewBag.IDTime = new SelectList(db.Time, "IDTime", "sNome");
            return View();
        }

        // POST: PreInscrito/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Administrador")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "IDPreInscrito,IDTime,IDCampeonato,dDataCadastro")] PreInscrito preInscrito)
        {
            if (ModelState.IsValid)
            {
                if (db.PreInscrito.Where(p => p.IDCampeonato == preInscrito.IDCampeonato && p.IDTime == preInscrito.IDTime).Count() > 0)
                {
                    var dta = DateTime.Now.Date;
                    ViewBag.IDCampeonato = new SelectList(db.Campeonato.Where(p => DbFunctions.TruncateTime(p.dDataFim) >= dta && p.bPreInscricao == true), "IDCampeonato", "sNome", preInscrito.IDCampeonato);
                    ViewBag.IDTime = new SelectList(db.Time, "IDTime", "sNome", preInscrito.IDTime);
                    return View(preInscrito).ComMensagem("Já existe uma pré-inscrição para este time.", "alert-warning");
                }

                db.PreInscrito.Add(preInscrito);
                db.SaveChanges();
                return RedirectToAction("Index").ComMensagem("Operação realizada com sucesso.", "alert-success");
            }

            var dta1 = DateTime.Now.Date;
            ViewBag.IDCampeonato = new SelectList(db.Campeonato.Where(p => DbFunctions.TruncateTime(p.dDataFim) >= dta1 && p.bPreInscricao == true), "IDCampeonato", "sNome", preInscrito.IDCampeonato);
            ViewBag.IDTime = new SelectList(db.Time, "IDTime", "sNome", preInscrito.IDTime);
            return View(preInscrito);
        }


        // GET: PreInscrito/Create
        [Authorize(Roles = "Administrador,Usuario")]
        public ActionResult CreateTime()
        {
            var dta = DateTime.Now.Date;
            int IDTime = Convert.ToInt16(Session["IDTime"]);
            ViewBag.IDCampeonato = new SelectList(db.Campeonato.Where(p => DbFunctions.TruncateTime(p.dDataFim) >= dta && p.bPreInscricao == true), "IDCampeonato", "sNome");
            ViewBag.IDTime = new SelectList(db.Time.Where(p => p.IDTime == IDTime), "IDTime", "sNome");
            return View();
        }

        // POST: PreInscrito/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Administrador,Usuario")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateTime([Bind(Include = "IDPreInscrito,IDTime,IDCampeonato,dDataCadastro")] PreInscrito preInscrito)
        {
            if (ModelState.IsValid)
            {
                if (db.PreInscrito.Where(p => p.IDCampeonato == preInscrito.IDCampeonato && p.IDTime == preInscrito.IDTime).Count() > 0)
                {
                    var dta = DateTime.Now.Date;
                    int IDTime = Convert.ToInt16(Session["IDTime"]);
                    ViewBag.IDCampeonato = new SelectList(db.Campeonato.Where(p => DbFunctions.TruncateTime(p.dDataFim) >= dta && p.bPreInscricao == true), "IDCampeonato", "sNome");
                    ViewBag.IDTime = new SelectList(db.Time.Where(p => p.IDTime == IDTime), "IDTime", "sNome");
                    return View(preInscrito).ComMensagem("Já existe uma pré-inscrição para este time.", "alert-warning");
                }

                db.PreInscrito.Add(preInscrito);
                db.SaveChanges();
                return RedirectToAction("IndexTime").ComMensagem("Operação realizada com sucesso.", "alert-success");
            }

            var dta1 = DateTime.Now.Date;
            int IDTime1 = Convert.ToInt16(Session["IDTime"]);
            ViewBag.IDCampeonato = new SelectList(db.Campeonato.Where(p => DbFunctions.TruncateTime(p.dDataFim) >= dta1 && p.bPreInscricao == true), "IDCampeonato", "sNome");
            ViewBag.IDTime = new SelectList(db.Time.Where(p => p.IDTime == IDTime1), "IDTime", "sNome");
            return View(preInscrito);
        }

        // GET: PreInscrito/Delete/5
        [Authorize(Roles = "Administrador,Usuario")]
        public ActionResult DeleteTime(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PreInscrito preInscrito = db.PreInscrito.Find(id);
            if (preInscrito == null)
            {
                return HttpNotFound();
            }
            return View(preInscrito);
        }

        // POST: PreInscrito/Delete/5
        [Authorize(Roles = "Administrador,Usuario")]
        [HttpPost, ActionName("DeleteTime")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmedTime(int id)
        {
            try
            {
                PreInscrito preInscrito = db.PreInscrito.Find(id);
                db.PreInscrito.Remove(preInscrito);
                db.SaveChanges();
                return RedirectToAction("IndexTime").ComMensagem("Operação realizada com sucesso.", "alert-success");
            }
            catch (Exception)
            {
                return RedirectToAction("IndexTime").ComMensagem("Este registro não pode ser apagado, ele possui dependências.", "alert-danger");
            }


        }

        // GET: PreInscrito/Delete/5
        [Authorize(Roles = "Administrador")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PreInscrito preInscrito = db.PreInscrito.Find(id);
            if (preInscrito == null)
            {
                return HttpNotFound();
            }
            return View(preInscrito);
        }

        // POST: PreInscrito/Delete/5
        [Authorize(Roles = "Administrador")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                PreInscrito preInscrito = db.PreInscrito.Find(id);
                db.PreInscrito.Remove(preInscrito);
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
