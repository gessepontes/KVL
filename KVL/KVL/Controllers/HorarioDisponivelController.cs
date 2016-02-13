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
    public class HorarioDisponivelController : Controller
    {
        private ModeloDados db = new ModeloDados();

        // GET: HorarioDisponivel
        //public ActionResult Index()
        //{
        //    var horarioDisponivel = db.HorarioDisponivel.Include(h => h.Campo).Include(h => h.Time);
        //    return View(horarioDisponivel.ToList());
        //}

        public ViewResult Index(string sortOrder, string currentFilter, string searchString, int? page)
        {

            ViewBag.ListaCampeonato = db.Campeonato;
            ViewBag.CurrentSort = sortOrder;
            ViewBag.NomeParam = string.IsNullOrEmpty(sortOrder) ? "Nome_desc" : "";
            ViewBag.TimeParm = sortOrder == "Time" ? "Time_desc" : "Time";
            ViewBag.DiaParm = sortOrder == "Dia" ? "Dia_desc" : "Dia";


            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;

            var horario = from s in db.HorarioDisponivel.Include(h => h.Campo).Include(h => h.Time)
                          select s;


            if (!string.IsNullOrEmpty(searchString))
            {
                horario = horario.Where(s => s.Campo.sNome.ToUpper().Contains(searchString.ToUpper()));
            }


            switch (sortOrder)
            {
                case "Nome_desc":
                    horario = horario.OrderByDescending(s => s.Campo.sNome);
                    break;
                case "Time":
                    horario = horario.OrderBy(s => s.Time.sNome);
                    break;
                case "Dia":
                    horario = horario.OrderBy(s => s.iDiaSemana);
                    break;
                case "Dia_desc":
                    horario = horario.OrderByDescending(s => s.iDiaSemana);
                    break;
                case "Time_desc":
                    horario = horario.OrderByDescending(s => s.Time.sNome);
                    break;
                default:
                    horario = horario.OrderBy(s => s.Campo.sNome);
                    break;
            }

            int pageSize = 10;
            int pageNumber = (page ?? 1);
            return View(horario.ToPagedList(pageNumber, pageSize));
        }

        // GET: HorarioDisponivel/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            HorarioDisponivel horarioDisponivel = db.HorarioDisponivel.Find(id);
            if (horarioDisponivel == null)
            {
                return HttpNotFound();
            }
            return View(horarioDisponivel);
        }

        // GET: HorarioDisponivel/Create
        [Authorize(Roles = "Administrador")]
        public ActionResult Create()
        {
            ViewBag.iCodCampo = new SelectList(db.Campo, "IDCampo", "sNome");
            ViewBag.iCodTime = new SelectList(db.Time, "IDTime", "sNome");
            return View();
        }


        // GET: HorarioDisponivel/Create
        [Authorize(Roles = "Administrador,Usuario")]
        public ActionResult CreateTime()
        {
            int IDTime = Convert.ToInt16(Session["IDTime"]);
            ViewBag.iCodCampo = new SelectList(db.Campo, "IDCampo", "sNome");
            ViewBag.iCodTime = new SelectList(db.Time.Where(p => p.IDTime == IDTime), "IDTime", "sNome");
            return View("CreateTime");
        }

        [Authorize(Roles = "Administrador,Usuario")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateTime([Bind(Include = "IDHorario,sHora,iCodTime,iCodCampo,iDiaSemana,dDataCadastro")] HorarioDisponivel horarioDisponivel)
        {
            if (ModelState.IsValid && horarioDisponivel.iDiaSemana != 0)
            {
                db.HorarioDisponivel.Add(horarioDisponivel);
                db.SaveChanges();
                return RedirectToAction("PerfilTime","Time").ComMensagem("Operação realizada com sucesso.", "alert-success"); 
            }

            int IDTime = Convert.ToInt16(Session["IDTime"]);
            ViewBag.iCodCampo = new SelectList(db.Campo, "IDCampo", "sNome", horarioDisponivel.iCodCampo);
            ViewBag.iCodTime = new SelectList(db.Time.Where(p => p.IDTime == IDTime), "IDTime", "sNome");
            return View(horarioDisponivel).ComMensagem("Todos os campos são obrigatorios.", "alert-warning");
        }


        // POST: HorarioDisponivel/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Administrador")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "IDHorario,sHora,iCodTime,iCodCampo,iDiaSemana,dDataCadastro")] HorarioDisponivel horarioDisponivel)
        {
            if (ModelState.IsValid)
            {
                db.HorarioDisponivel.Add(horarioDisponivel);
                db.SaveChanges();
                return RedirectToAction("Index").ComMensagem("Operação realizada com sucesso.", "alert-success");
            }

            ViewBag.iCodCampo = new SelectList(db.Campo, "IDCampo", "sNome", horarioDisponivel.iCodCampo);
            ViewBag.iCodTime = new SelectList(db.Time, "IDTime", "sNome", horarioDisponivel.iCodTime);
            return View(horarioDisponivel);
        }

        // GET: HorarioDisponivel/Edit/5
        [Authorize(Roles = "Administrador")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            HorarioDisponivel horarioDisponivel = db.HorarioDisponivel.Find(id);
            if (horarioDisponivel == null)
            {
                return HttpNotFound();
            }
            ViewBag.iCodCampo = new SelectList(db.Campo, "IDCampo", "sNome", horarioDisponivel.iCodCampo);
            ViewBag.iCodTime = new SelectList(db.Time, "IDTime", "sNome", horarioDisponivel.iCodTime);
            return View(horarioDisponivel);
        }

        // POST: HorarioDisponivel/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Administrador")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "IDHorario,sHora,iCodTime,iCodCampo,iDiaSemana,dDataCadastro")] HorarioDisponivel horarioDisponivel)
        {
            if (ModelState.IsValid)
            {
                db.Entry(horarioDisponivel).State = EntityState.Modified;
                db.Entry(horarioDisponivel).Property("dDataCadastro").IsModified = false;
                db.SaveChanges();
                return RedirectToAction("Index").ComMensagem("Operação realizada com sucesso.", "alert-success");
            }
            ViewBag.iCodCampo = new SelectList(db.Campo, "IDCampo", "sNome", horarioDisponivel.iCodCampo);
            ViewBag.iCodTime = new SelectList(db.Time, "IDTime", "sNome", horarioDisponivel.iCodTime);
            return View(horarioDisponivel);
        }

        // GET: HorarioDisponivel/Delete/5
        [Authorize(Roles = "Administrador")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            HorarioDisponivel horarioDisponivel = db.HorarioDisponivel.Find(id);
            if (horarioDisponivel == null)
            {
                return HttpNotFound();
            }
            return View(horarioDisponivel);
        }

        // POST: HorarioDisponivel/Delete/5
        [Authorize(Roles = "Administrador")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            HorarioDisponivel horarioDisponivel = db.HorarioDisponivel.Find(id);
            db.HorarioDisponivel.Remove(horarioDisponivel);
            db.SaveChanges();
            return RedirectToAction("Index").ComMensagem("Operação realizada com sucesso.", "alert-success"); 
        }


        // GET: HorarioDisponivel/Delete/5
        [Authorize(Roles = "Administrador,Usuario")]
        public ActionResult DeleteTime(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            HorarioDisponivel horarioDisponivel = db.HorarioDisponivel.Find(id);
            if (horarioDisponivel == null)
            {
                return HttpNotFound();
            }
            return View(horarioDisponivel);
        }

        // POST: HorarioDisponivel/Delete/5
        [Authorize(Roles = "Administrador,Usuario")]
        [HttpPost, ActionName("DeleteTime")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmedTime(int id)
        {
            HorarioDisponivel horarioDisponivel = db.HorarioDisponivel.Find(id);
            db.HorarioDisponivel.Remove(horarioDisponivel);
            db.SaveChanges();
            return RedirectToAction("PerfilTime", "Time").ComMensagem("Operação realizada com sucesso.", "alert-success");
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
