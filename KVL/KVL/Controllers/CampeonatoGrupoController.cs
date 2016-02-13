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
    public class CampeonatoGrupoController : Controller
    {
        private ModeloDados db = new ModeloDados();

        // GET: CampeonatoGrupo
        //public ActionResult Index()
        //{
        //    var campeonatoGrupo = db.CampeonatoGrupo.Include(c => c.Inscrito);
        //    return View(campeonatoGrupo.ToList());
        //}


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

            var campeonatos = from s in db.CampeonatoGrupo.Include(c => c.Inscrito)
                              select s;

            if (!string.IsNullOrEmpty(searchString))
            {
                campeonatos = campeonatos.Where(s => s.Inscrito.PreInscrito.Campeonato.sNome.ToUpper().Contains(searchString.ToUpper()));
            }
            switch (sortOrder)
            {
                case "Nome_desc":
                    campeonatos = campeonatos.OrderByDescending(s => s.Inscrito.PreInscrito.Campeonato.sNome);
                    break;
                default:
                    campeonatos = campeonatos.OrderBy(s => s.Inscrito.PreInscrito.Campeonato.sNome);
                    break;
            }

            int pageSize = 10;
            int pageNumber = (page ?? 1);
            return View(campeonatos.ToPagedList(pageNumber, pageSize));
        }

        // GET: CampeonatoGrupo/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CampeonatoGrupo campeonatoGrupo = db.CampeonatoGrupo.Find(id);
            if (campeonatoGrupo == null)
            {
                return HttpNotFound();
            }
            return View(campeonatoGrupo);
        }

        // GET: CampeonatoGrupo/Create
        public ActionResult Create()
        {
            ViewBag.ListaCampeonato = db.Campeonato.Where(p => p.iTipoCampeonato == TipoCampeonato.Grupos);
            return View();
        }

        public ActionResult ListaTimes(int? IDCampeonato)
        {

            if (IDCampeonato == null) { IDCampeonato = 0; }

            var cities = new SelectList(db.Inscrito.Where(p => p.PreInscrito.Campeonato.IDCampeonato == IDCampeonato).Select(x => new SelectListItem
            {
                Text = x.PreInscrito.Time.sNome.ToUpper(),
                Value = x.IDInscrito.ToString()
            }).ToList().OrderBy(p => p.Text), "Value", "Text");
            return Json(cities, JsonRequestBehavior.AllowGet);
        }

        // POST: CampeonatoGrupo/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "IDCampeonatoGrupo,IDGrupo,IDInscrito,dDataCadastro")] CampeonatoGrupo campeonatoGrupo)
        {
            if (ModelState.IsValid)
            {
                if (campeonatoGrupo.IDInscrito == 0 || campeonatoGrupo.IDGrupo == 0)
                {
                    ViewBag.ListaCampeonato = db.Campeonato.Where(p => p.iTipoCampeonato == TipoCampeonato.Grupos);
                    return RedirectToAction("Create").ComMensagem("Time inscrito ou grupo é obrigatorio.", "alert-warning");
                }

                if (db.CampeonatoGrupo.Where(p => p.IDInscrito == campeonatoGrupo.IDInscrito).Count() > 0)
                {
                    ViewBag.ListaCampeonato = db.Campeonato.Where(p => p.iTipoCampeonato == TipoCampeonato.Grupos);
                    return View(campeonatoGrupo).ComMensagem("Já existe uma inscrição para este time.", "alert-warning");
                }

                db.CampeonatoGrupo.Add(campeonatoGrupo);
                db.SaveChanges();
                return RedirectToAction("Index").ComMensagem("Operação realizada com sucesso.", "alert-success");
            }

            ViewBag.ListaCampeonato = db.Campeonato.Where(p => p.iTipoCampeonato == TipoCampeonato.Grupos);

            return View(campeonatoGrupo);
        }

        // GET: CampeonatoGrupo/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CampeonatoGrupo campeonatoGrupo = db.CampeonatoGrupo.Find(id);
            if (campeonatoGrupo == null)
            {
                return HttpNotFound();
            }
            ViewBag.IDInscrito = new SelectList(db.Inscrito.Select(x => new SelectListItem
            {
                Text = x.PreInscrito.Time.sNome.ToUpper(),
                Value = x.IDInscrito.ToString()
            }), "Value", "Text",campeonatoGrupo.IDInscrito);

            return View(campeonatoGrupo);
        }

        // POST: CampeonatoGrupo/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "IDCampeonatoGrupo,IDGrupo,IDInscrito,dDataCadastro")] CampeonatoGrupo campeonatoGrupo)
        {
            if (ModelState.IsValid)
            {
                db.Entry(campeonatoGrupo).State = EntityState.Modified;
                db.Entry(campeonatoGrupo).Property("dDataCadastro").IsModified = false;
                db.Entry(campeonatoGrupo).Property("IDInscrito").IsModified = false;
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.IDInscrito = new SelectList(db.Inscrito.Select(x => new SelectListItem
            {
                Text = x.PreInscrito.Time.sNome.ToUpper(),
                Value = x.IDInscrito.ToString()
            }), "Value", "Text");

            return View(campeonatoGrupo);
        }

        // GET: CampeonatoGrupo/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CampeonatoGrupo campeonatoGrupo = db.CampeonatoGrupo.Find(id);
            if (campeonatoGrupo == null)
            {
                return HttpNotFound();
            }
            return View(campeonatoGrupo);
        }

        // POST: CampeonatoGrupo/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                CampeonatoGrupo campeonatoGrupo = db.CampeonatoGrupo.Find(id);
                db.CampeonatoGrupo.Remove(campeonatoGrupo);
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
