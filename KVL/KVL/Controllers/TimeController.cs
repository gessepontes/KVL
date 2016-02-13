using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using KVL.Models;
using System.Web;
using System.IO;
using System;
using KVL.Context;
using PagedList;
using System.Collections.Generic;

namespace KVL.Controllers
{

    public class TimeController : Controller
    {
        private ModeloDados db = new ModeloDados();

        // GET: Time
        [Authorize(Roles = "Administrador")]
        public ViewResult Index(string sortOrder, string currentFilter, string searchString, int? page)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.NomeParam = string.IsNullOrEmpty(sortOrder) ? "Nome_desc" : "";
            ViewBag.TimeParm = sortOrder == "Time" ? "Time_desc" : "Time";

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;

            var times = from s in db.Time.Include(t => t.Pessoa)
                        select s;

            if (!String.IsNullOrEmpty(searchString))
            {
                times = times.Where(s => s.sNome.ToUpper().Contains(searchString.ToUpper()));
            }
            switch (sortOrder)
            {
                case "Nome_desc":
                    times = times.OrderByDescending(s => s.Pessoa.sNome);
                    break;
                case "Time_desc":
                    times = times.OrderByDescending(s => s.sNome);
                    break;
                case "Time":
                    times = times.OrderBy(s => s.sNome);
                    break;
                default:
                    times = times.OrderBy(s => s.Pessoa.sNome);
                    break;
            }

            int pageSize = 10;
            int pageNumber = (page ?? 1);
            return View(times.ToPagedList(pageNumber, pageSize));
        }

        [Authorize(Roles = "Administrador,Usuario")]
        public ViewResult PartidasTime(string sortOrder, string currentFilter, string searchString, int? IDCampeonato, int? page)
        {

            int IDTime = Convert.ToInt16(Session["IDTime"]);

            ViewBag.ListaCampeonato = db.Campeonato;
            ViewBag.CurrentSort = sortOrder;
            ViewBag.NomeParam = string.IsNullOrEmpty(sortOrder) ? "Nome_desc" : "";
            ViewBag.DataParm = sortOrder == "Data" ? "Data_desc" : "Data";
            ViewBag.CampeonatoParm = sortOrder == "Campeonato" ? "Campeonato_desc" : "Campeonato";

            ViewBag.CurrentFilter = searchString;
            ViewBag.IDCampeonato = IDCampeonato;

            var partidasCampeonato = from s in db.PartidaCampeonato.Include(p => p.Inscrito).Include(p => p.Inscrito1).Where(p => p.Inscrito.PreInscrito.IDTime == IDTime || p.Inscrito1.PreInscrito.IDTime == IDTime)
                                     select s;

            var partidasAmistosas = from s in db.PartidaAmistosa.Include(p => p.Time).Include(p => p.Time1).Where(p => p.IDTime1 == IDTime || p.IDTime2 == IDTime)
                                    select s;

            List<Partida> listaPartidas = new List<Partida>();

            foreach (var item in partidasCampeonato)
            {
                Partida partidaC = new Partida();

                partidaC.sTime1 = item.Inscrito.PreInscrito.Time.sNome.ToUpper();
                partidaC.sTime2 = item.Inscrito1.PreInscrito.Time.sNome.ToUpper();
                partidaC.iQntGols1 = item.iQntGols1;
                partidaC.iQntGols2 = item.iQntGols2;
                partidaC.dDataPartida = item.dDataPartida;
                partidaC.sHoraPartida = item.sHoraPartida;
                partidaC.sCampo = item.Inscrito.PreInscrito.Campeonato.Campo.sNome;
                partidaC.sCampeonato = item.Inscrito.PreInscrito.Campeonato.sNome;
                partidaC.IDCampeonato = item.Inscrito.PreInscrito.IDCampeonato;

                partidaC.sSimbolo1 = item.Inscrito.PreInscrito.Time.sSimbolo;
                partidaC.sSimbolo2 = item.Inscrito1.PreInscrito.Time.sSimbolo;

                partidaC.IDPartida = item.IDPartidaCampeonato;
                partidaC.IDTime1 = item.Inscrito.PreInscrito.IDTime;
                partidaC.IDTime2 = item.Inscrito1.PreInscrito.IDTime;
                partidaC.iTipoPartida = 1;

                listaPartidas.Add(partidaC);
            }

            foreach (var item1 in partidasAmistosas)
            {
                Partida partidaA = new Partida();

                partidaA.sTime1 = item1.Time.sNome.ToUpper();
                partidaA.sTime2 = item1.Time1.sNome.ToUpper();
                partidaA.iQntGols1 = item1.iQntGols1;
                partidaA.iQntGols2 = item1.iQntGols2;
                partidaA.dDataPartida = item1.dDataPartida;
                partidaA.sHoraPartida = item1.sHoraPartida;
                partidaA.sCampo = item1.Campo.sNome;
                partidaA.sCampeonato = "";
                partidaA.IDCampeonato = 0;

                partidaA.sSimbolo1 = item1.Time.sSimbolo;
                partidaA.sSimbolo2 = item1.Time1.sSimbolo;

                partidaA.IDPartida = item1.IDPartidaAmistosa;
                partidaA.IDTime1 = item1.IDTime1;
                partidaA.IDTime2 = item1.IDTime2;
                partidaA.iTipoPartida = 0;

                listaPartidas.Add(partidaA);
            }


            if (!string.IsNullOrEmpty(searchString))
            {
                listaPartidas = listaPartidas.Where(s => s.sTime1.ToUpper().Contains(searchString.ToUpper()) || s.sTime2.ToUpper().Contains(searchString.ToUpper())).ToList();
            }

            if (IDCampeonato != null)
            {
                listaPartidas = listaPartidas.Where(s => s.IDCampeonato == IDCampeonato).ToList();
            }


            switch (sortOrder)
            {
                case "Nome_desc":
                    listaPartidas = listaPartidas.OrderByDescending(s => s.sTime1).ToList();
                    break;
                case "Data":
                    listaPartidas = listaPartidas.OrderBy(s => s.dDataPartida).ToList();
                    break;
                case "Data_desc":
                    listaPartidas = listaPartidas.OrderByDescending(s => s.dDataPartida).ToList();
                    break;
                case "Campeonato":
                    listaPartidas = listaPartidas.OrderBy(s => s.sCampeonato).ToList();
                    break;
                case "Campeonato_desc":
                    listaPartidas = listaPartidas.OrderByDescending(s => s.sCampeonato).ToList();
                    break;
                default:
                    listaPartidas = listaPartidas.OrderBy(s => s.sTime1).ToList();
                    break;
            }

            int pageSize = 10;
            int pageNumber = (page ?? 1);
            return View(listaPartidas.ToPagedList(pageNumber, pageSize));
        }



        // GET: Time/Details/5
        [Authorize(Roles = "Administrador")]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Time time = db.Time.Find(id);
            if (time == null)
            {
                return HttpNotFound();
            }
            return View(time);
        }

        // GET: Time/Create
        [Authorize(Roles = "Administrador")]
        public ActionResult Create()
        {
            ViewBag.IDPessoa = new SelectList(db.Pessoa, "IDPessoa", "sNome");
            return View();
        }

        // POST: Time/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Administrador")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "IDTime,IDPessoa,sNome,sSimbolo,sObservacao,dDataCadastro")] Time time, HttpPostedFileBase upload)
        {
            if (ModelState.IsValid)
            {
                db.Time.Add(time);
                db.SaveChanges();

                time.sSimbolo = SalvarArquivo(upload, time.IDTime);

                db.Entry(time).State = EntityState.Modified;
                db.SaveChanges();

                return RedirectToAction("Index").ComMensagem("Operação realizada com sucesso.", "alert-success");
            }

            ViewBag.IDPessoa = new SelectList(db.Pessoa, "IDPessoa", "sNome", time.IDPessoa);
            return View(time);
        }

        // GET: Time/Edit/5
        [Authorize(Roles = "Administrador")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Time time = db.Time.Find(id);
            if (time == null)
            {
                return HttpNotFound();
            }
            ViewBag.IDPessoa = new SelectList(db.Pessoa, "IDPessoa", "sNome", time.IDPessoa);
            return View(time);
        }

        // POST: Time/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Administrador")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "IDTime,IDPessoa,sNome,sSimbolo,sObservacao")] Time time, HttpPostedFileBase upload)
        {
            if (ModelState.IsValid)
            {
                db.Entry(time).State = EntityState.Modified;
                db.Entry(time).Property("dDataCadastro").IsModified = false;

                if (upload == null)
                {
                    db.Entry(time).Property("sSimbolo").IsModified = false;
                }
                else
                {
                    time.sSimbolo = SalvarArquivo(upload, time.IDTime);
                }

                db.SaveChanges();
                return RedirectToAction("Index").ComMensagem("Operação realizada com sucesso.", "alert-success");
            }
            ViewBag.IDPessoa = new SelectList(db.Pessoa, "IDPessoa", "sNome", time.IDPessoa);
            return View(time);
        }

        // GET: Time/Delete/5
        [Authorize(Roles = "Administrador")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Time time = db.Time.Find(id);
            if (time == null)
            {
                return HttpNotFound();
            }
            return View(time);
        }

        // POST: Time/Delete/5
        [Authorize(Roles = "Administrador")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                Time time = db.Time.Find(id);
                db.Time.Remove(time);
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

        protected string SalvarArquivo(HttpPostedFileBase upload, int id)
        {
            string sRetorno = "";
            string sFilename = "";

            if (upload != null)
            {
                if (upload.ContentLength > (2 * 1024 * 1024))
                {
                    ModelState.AddModelError("CustomError", "Tamanho maximo 2 mb.");
                    sRetorno = "semimagem.gif";
                }

                if (!(upload.ContentType == "image/jpeg" || upload.ContentType == "image/gif" || upload.ContentType == "image/png"))
                {
                    ModelState.AddModelError("CustomError", "Tipo válido jpg/gif.");
                    sRetorno = "semimagem.gif";
                }

                if (sRetorno != "semimagem.gif")
                {
                    var fileType = Path.GetFileName(upload.ContentType);
                    sFilename = Convert.ToString(id) + "." + fileType;

                    var path = Path.Combine(Server.MapPath("~/imagens/simbolo/"), sFilename);

                    upload.SaveAs(path);
                    sRetorno = sFilename;
                }
            }
            else
            {
                sRetorno = "semimagem.gif";
            }

            return sRetorno;
        }

        [Authorize(Roles = "Administrador,Usuario")]
        public ActionResult PerfilTime()
        {
            int IDTime = Convert.ToInt16(Session["IDTime"]);

            Time time = db.Time.Find(IDTime);
            ViewBag.Jogadores = db.Jogador.Where(p => p.IDTime == time.IDTime && p.bAtivo == true).OrderBy(p => p.Pessoa.sNome);
            ViewBag.HorarioDisponivel = db.HorarioDisponivel.Where(p => p.iCodTime == time.IDTime).OrderBy(p => p.iDiaSemana);

            return View(time);
        }

        [Authorize(Roles = "Administrador,Usuario")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PessoaTime([Bind(Include = "IDTime,sObservacao")] Time time, HttpPostedFileBase upload)
        {

            Time timeOld = db.Time.Find(time.IDTime);

            if (timeOld != null)
            {
                db.Entry(timeOld).State = EntityState.Modified;

                if (upload == null)
                {
                    db.Entry(timeOld).Property("sSimbolo").IsModified = false;
                }
                else
                {
                    timeOld.sSimbolo = SalvarArquivo(upload, timeOld.IDTime);
                }

                timeOld.sObservacao = time.sObservacao;

                db.Entry(timeOld).Property("dDataCadastro").IsModified = false;
                db.SaveChanges();
                return RedirectToAction("PerfilTime", "Time").ComMensagem("Operação realizada com sucesso.", "alert-success");
            }
            return RedirectToAction("PerfilTime", "Time").ComMensagem("Erro ao realizar a operação.", "alert-danger");
        }




    }
}
