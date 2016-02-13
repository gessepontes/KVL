using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using KVL.Context;
using KVL.Models;
using PagedList;
using System.Collections.Generic;
using System;

namespace KVL.Controllers
{
    public class InscritoController : Controller
    {
        private ModeloDados db = new ModeloDados();

        // GET: Inscrito
        //public ActionResult Index()
        //{
        //    var inscrito = db.Inscrito.Include(i => i.PreInscrito);
        //    return View(inscrito.ToList());
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

        // GET: PreInscrito/Details/5
        public ActionResult Visualizar(int? id)
        {
            List<Inscrito> list = new List<Inscrito>();

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var inscrito = db.PreInscrito.Select(a => new { a, a.IDCampeonato, a.Inscrito.Count }).Where(i => i.IDCampeonato == id);

            foreach (var cust in inscrito)
            {
                Inscrito timeInscrito = new Inscrito();

                timeInscrito.PreInscrito = cust.a;
                timeInscrito.IDInscrito = cust.Count;

                list.Add(timeInscrito);
            }

            //PreInscrito preInscrito = db.PreInscrito.Find(id);
            if (inscrito == null)
            {
                return HttpNotFound();
            }
            return View(list);
        }


        // GET: Inscrito/Create
        public ActionResult Create()
        {
            ViewBag.IDPreInscrito = new SelectList(db.PreInscrito, "IDPreInscrito", "IDPreInscrito");
            return View();
        }

        // POST: Inscrito/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "IDInscrito,IDPreInscrito,bAtivo,dDataCadastro")] Inscrito inscrito, int IDCampeonato)
        {
            if (ModelState.IsValid)
            {

                Campeonato campeonato = db.Campeonato.Find(IDCampeonato);
                var inscritos = db.Inscrito.Where(p => p.PreInscrito.IDCampeonato == IDCampeonato);

                if (campeonato.iQuantidadeTimes > inscritos.Count())
                {
                    db.Inscrito.Add(inscrito);
                    db.SaveChanges();
                    return RedirectToAction("Visualizar/" + IDCampeonato).ComMensagem("Operação realizada com sucesso.", "alert-success");
                }
                else
                {
                    return RedirectToAction("Visualizar/" + IDCampeonato).ComMensagem("Quantidade maxima de times inscritos já foi atendida.", "alert-warning");
                }

            }

            return View(inscrito);
        }


        // POST: Inscrito/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int IDCampeonato, int IDPreInscrito)
        {
            try
            {
                var inscrito = db.Inscrito.Where(p => p.IDPreInscrito == IDPreInscrito).FirstOrDefault();
                db.Inscrito.Remove(inscrito);
                db.SaveChanges();
                return RedirectToAction("Visualizar/" + IDCampeonato).ComMensagem("Operação realizada com sucesso.", "alert-success");
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
