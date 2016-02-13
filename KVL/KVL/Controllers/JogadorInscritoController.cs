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
    public class JogadorInscritoController : Controller
    {
        private ModeloDados db = new ModeloDados();

        // GET: JogadorInscrito
        //public ActionResult Index()
        //{
        //    var jogadorInscrito = db.JogadorInscrito.Include(j => j.Inscrito).Include(j => j.Jogador);
        //    return View(jogadorInscrito.ToList());
        //}


        public ViewResult Index(string sortOrder, int? IDCampeonato, int? page)
        {

            ViewBag.ListaCampeonato = db.Campeonato;
            ViewBag.CurrentSort = sortOrder;
            ViewBag.CampeonatoParm = sortOrder == "Campeonato" ? "Campeonato_desc" : "Campeonato";
            ViewBag.TimeParm=sortOrder == "Time" ? "Time_desc" : "Time";

            ViewBag.IDCampeonato = IDCampeonato;

            var timesInscritos = from s in db.Inscrito
                                     select s;

            if (IDCampeonato != null)
            {
                timesInscritos = timesInscritos.Where(s => s.PreInscrito.IDCampeonato == IDCampeonato);
            }


            switch (sortOrder)
            {
                case "Time":
                    timesInscritos = timesInscritos.OrderBy(s => s.PreInscrito.Time.sNome);
                    break;
                case "Time_desc":
                    timesInscritos = timesInscritos.OrderByDescending(s => s.PreInscrito.Time.sNome);
                    break;
                case "Campeonato_desc":
                    timesInscritos = timesInscritos.OrderByDescending(s => s.PreInscrito.Campeonato.sNome);
                    break;
                default:
                    timesInscritos = timesInscritos.OrderBy(s => s.PreInscrito.Campeonato.sNome);
                    break;
            }

            int pageSize = 10;
            int pageNumber = (page ?? 1);
            return View(timesInscritos.ToPagedList(pageNumber, pageSize));
        }

        // GET: PreInscrito/Details/5
        public ActionResult Visualizar(int? id)
        {
            List<JogadorInscrito> list = new List<JogadorInscrito>();

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var inscrito = db.Inscrito.Find(id);
            var jogador = db.Jogador.Include(p=>p.JogadorInscrito).Where(i => i.IDTime == inscrito.PreInscrito.IDTime && i.bAtivo == true).Select(a => new { a, a.IDTime, a.JogadorInscrito.Count });
            
            

            foreach (var cust in jogador)
            {
                JogadorInscrito jogadorInscrito = new JogadorInscrito();

                jogadorInscrito.IDJogadorInscrito = cust.Count;
                jogadorInscrito.Jogador = cust.a;
                jogadorInscrito.Inscrito = inscrito;

                list.Add(jogadorInscrito);
            }

            //PreInscrito preInscrito = db.PreInscrito.Find(id);
            if (inscrito == null)
            {
                return HttpNotFound();
            }
            return View(list);
        }

        // POST: Inscrito/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "IDInscrito,IDJogador,dDataCadastro")] JogadorInscrito inscrito, int IDCampeonato)
        {
            if (ModelState.IsValid)
            {

                var jogador = db.Jogador.Find(inscrito.IDJogador);
                var inscritos = db.JogadorInscrito.Where(p => p.Jogador.IDPessoa == jogador.IDPessoa && p.Inscrito.PreInscrito.IDCampeonato == IDCampeonato).ToList().Count;

                if (inscritos == 0)
                {
                    db.JogadorInscrito.Add(inscrito);
                    db.SaveChanges();
                    return RedirectToAction("Visualizar/" + inscrito.IDInscrito).ComMensagem("Operação realizada com sucesso.", "alert-success");
                }
                else
                {
                    return RedirectToAction("Visualizar/" + inscrito.IDInscrito).ComMensagem("Este jogador já esta inscrito por outro time.", "alert-warning");
                }

            }

            return View();
        }


        // POST: Inscrito/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int IDJogador, int IDInscrito)
        {
            try
            {
                var inscrito = db.JogadorInscrito.Where(p => p.IDJogador == IDJogador && p.IDInscrito == IDInscrito).FirstOrDefault();

                db.JogadorInscrito.Remove(inscrito);
                db.SaveChanges();
                return RedirectToAction("Visualizar/" + IDInscrito).ComMensagem("Operação realizada com sucesso.", "alert-success");
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
