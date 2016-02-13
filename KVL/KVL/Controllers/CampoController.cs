using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using KVL.Models;
using KVL.Context;
using PagedList;
using System;

namespace KVL.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class CampoController : Controller
    {
        private ModeloDados db = new ModeloDados();

        // GET: Campo
        //public ActionResult Index()
        //{
        //    return View(db.Campo.ToList());
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

            var times = from s in db.Campo.ToList()
                        select s;

            if (!string.IsNullOrEmpty(searchString))
            {
                times = times.Where(s => s.sNome.ToUpper().Contains(searchString.ToUpper()));
            }
            switch (sortOrder)
            {
                case "Nome_desc":
                    times = times.OrderByDescending(s => s.sNome);
                    break;
                default:
                    times = times.OrderBy(s => s.sNome);
                    break;
            }

            int pageSize = 10;
            int pageNumber = (page ?? 1);
            return View(times.ToPagedList(pageNumber, pageSize));
        }


        // GET: Campo/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Campo campo = db.Campo.Find(id);
            if (campo == null)
            {
                return HttpNotFound();
            }
            return View(campo);
        }

        // GET: Campo/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Campo/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "IDCampo,sNome,sEndereco,mValor,mValorMensal,dDataCadastro")] Campo campo)
        {
            if (ModelState.IsValid)
            {
                db.Campo.Add(campo);
                db.SaveChanges();
                return RedirectToAction("Index").ComMensagem("Operação realizada com sucesso.", "alert-success");
            }

            return View(campo);
        }

        // GET: Campo/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Campo campo = db.Campo.Find(id);
            if (campo == null)
            {
                return HttpNotFound();
            }
            return View(campo);
        }

        // POST: Campo/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "IDCampo,sNome,sEndereco,mValor,mValorMensal,dDataCadastro")] Campo campo)
        {
            if (ModelState.IsValid)
            {
                db.Entry(campo).State = EntityState.Modified;
                db.Entry(campo).Property("dDataCadastro").IsModified = false;
                db.SaveChanges();
                return RedirectToAction("Index").ComMensagem("Operação realizada com sucesso.", "alert-success");
            }
            return View(campo);
        }

        // GET: Campo/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Campo campo = db.Campo.Find(id);
            if (campo == null)
            {
                return HttpNotFound();
            }
            return View(campo);
        }

        // POST: Campo/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                Campo campo = db.Campo.Find(id);
                db.Campo.Remove(campo);
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
