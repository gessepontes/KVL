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
    public class JogadorController : Controller
    {
        private ModeloDados db = new ModeloDados();

        // GET: Jogador
        //public ActionResult Index()
        //{
        //    var jogador = db.Jogador.Include(j => j.Pessoa).Include(j => j.Posicao).Include(j => j.Time);
        //    return View(jogador.ToList());
        //}

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

            var jogadores = from s in db.Jogador.Include(j => j.Pessoa).Include(j => j.Posicao).Include(j => j.Time).Where(p => p.bAtivo == true)
                            select s;

            if (!string.IsNullOrEmpty(searchString))
            {
                jogadores = jogadores.Where(s => s.Pessoa.sNome.ToUpper().Contains(searchString.ToUpper()));
            }
            switch (sortOrder)
            {
                case "Nome_desc":
                    jogadores = jogadores.OrderByDescending(s => s.Pessoa.sNome);
                    break;
                case "Time_desc":
                    jogadores = jogadores.OrderByDescending(s => s.Time.sNome);
                    break;
                case "Time":
                    jogadores = jogadores.OrderBy(s => s.Time.sNome);
                    break;
                default:
                    jogadores = jogadores.OrderBy(s => s.Pessoa.sNome);
                    break;
            }

            int pageSize = 10;
            int pageNumber = (page ?? 1);
            return View(jogadores.ToPagedList(pageNumber, pageSize));
        }


        // GET: Jogador/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Jogador jogador = db.Jogador.Find(id);
            if (jogador == null)
            {
                return HttpNotFound();
            }
            return View(jogador);
        }

        public ActionResult JogadorTransferencia()
        {
            ViewBag.IDTime = new SelectList(db.Time, "IDTime", "sNome").OrderBy(p=>p.Text);
            ViewBag.IDTimeDestino = new SelectList(db.Time, "IDTime", "sNome").OrderBy(p => p.Text);

            return View();
        }

        [HttpPost]
        public ActionResult JogadorTransferencia(int? IDTime, int? IDJogador, int? IDTimeDestino)
        {

            ViewBag.IDTime = new SelectList(db.Time, "IDTime", "sNome").OrderBy(p => p.Text);
            ViewBag.IDTimeDestino = new SelectList(db.Time, "IDTime", "sNome").OrderBy(p => p.Text);

            if (IDTime == null || IDJogador == null || IDTimeDestino == null)
            {
                return View().ComMensagem("Todos os campos são obrigatórios.", "alert-warning");
            }

            if (IDTime == IDTimeDestino)
            {
                return RedirectToAction("JogadorTransferencia").ComMensagem("O time de destino deve ser diferente do time de destino.", "alert-warning");
            }

            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    Jogador jogador = db.Jogador.Find(IDJogador);
                    db.Entry(jogador).State = EntityState.Modified;
                    db.Entry(jogador).Property("dDataCadastro").IsModified = false;
                    jogador.bAtivo = false;
                    db.SaveChanges();

                    Jogador jogadorNovo = new Jogador();

                    jogadorNovo.IDPessoa = jogador.IDPessoa;
                    jogadorNovo.IDPosicao = jogador.IDPosicao;
                    jogadorNovo.IDTime = IDTimeDestino ?? 0;
                    jogadorNovo.bAtivo = true;
                    db.Jogador.Add(jogadorNovo);
                    db.SaveChanges();

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    ex.Message.ToString();
                    transaction.Rollback();
                    return RedirectToAction("JogadorTransferencia").ComMensagem("Erro ao realizar a operação.", "alert-danger");
                }
            }


            return RedirectToAction("JogadorTransferencia").ComMensagem("Operação realizada com sucesso.", "alert-success");
        }

        public ActionResult ListaJogadores(int? IDTime)
        {

            if (IDTime == null) { IDTime = 0; }

            var cities = new SelectList(db.Jogador.Where(p => p.IDTime == IDTime && p.bAtivo == true).Select(x => new SelectListItem
            {
                Text = x.Pessoa.sNome.ToString(),
                Value = x.IDJogador.ToString()
            }).ToList().OrderBy(p => p.Text), "Value", "Text");
            return Json(cities, JsonRequestBehavior.AllowGet);
        }

        // GET: Jogador/Create
        public ActionResult Create()
        {
            ViewBag.IDPessoa = new SelectList(db.Pessoa, "IDPessoa", "sNome").OrderBy(p => p.Text);
            ViewBag.IDPosicao = new SelectList(db.Posicao, "IDPosicao", "sDescricao").OrderBy(p => p.Text);
            ViewBag.IDTime = new SelectList(db.Time, "IDTime", "sNome").OrderBy(p => p.Text);
            return View();
        }

        // POST: Jogador/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "IDJogador,IDPessoa,IDPosicao,IDTime,dDataCadastro,bAtivo")] Jogador jogador)
        {
            if (ModelState.IsValid)
            {
                jogador.bAtivo = true;
                db.Jogador.Add(jogador);
                db.SaveChanges();
                return RedirectToAction("Index").ComMensagem("Operação realizada com sucesso.", "alert-success");
            }

            ViewBag.IDPessoa = new SelectList(db.Pessoa, "IDPessoa", "sNome", jogador.IDPessoa).OrderBy(p => p.Text);
            ViewBag.IDPosicao = new SelectList(db.Posicao, "IDPosicao", "sDescricao", jogador.IDPosicao).OrderBy(p => p.Text);
            ViewBag.IDTime = new SelectList(db.Time, "IDTime", "sNome", jogador.IDTime).OrderBy(p => p.Text);
            return View(jogador);
        }

        // GET: Jogador/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Jogador jogador = db.Jogador.Find(id);
            if (jogador == null)
            {
                return HttpNotFound();
            }
            ViewBag.IDPessoa = new SelectList(db.Pessoa, "IDPessoa", "sNome", jogador.IDPessoa).OrderBy(p => p.Text);
            ViewBag.IDPosicao = new SelectList(db.Posicao, "IDPosicao", "sDescricao", jogador.IDPosicao).OrderBy(p => p.Text);
            ViewBag.IDTime = new SelectList(db.Time, "IDTime", "sNome", jogador.IDTime).OrderBy(p => p.Text);
            return View(jogador);
        }

        // POST: Jogador/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "IDJogador,IDPessoa,IDPosicao,IDTime")] Jogador jogador)
        {
            if (ModelState.IsValid)
            {
                db.Entry(jogador).State = EntityState.Modified;
                db.Entry(jogador).Property("dDataCadastro").IsModified = false;
                db.Entry(jogador).Property("bAtivo").IsModified = false;
                db.SaveChanges();
                return RedirectToAction("Index").ComMensagem("Operação realizada com sucesso.", "alert-success");
            }
            ViewBag.IDPessoa = new SelectList(db.Pessoa, "IDPessoa", "sNome", jogador.IDPessoa).OrderBy(p => p.Text);
            ViewBag.IDPosicao = new SelectList(db.Posicao, "IDPosicao", "sDescricao", jogador.IDPosicao).OrderBy(p => p.Text);
            ViewBag.IDTime = new SelectList(db.Time, "IDTime", "sNome", jogador.IDTime).OrderBy(p => p.Text);
            return View(jogador);
        }

        // GET: Jogador/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Jogador jogador = db.Jogador.Find(id);
            if (jogador == null)
            {
                return HttpNotFound();
            }
            return View(jogador);
        }

        // POST: Jogador/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                Jogador jogador = db.Jogador.Find(id);
                db.Entry(jogador).State = EntityState.Modified;
                db.Entry(jogador).Property("dDataCadastro").IsModified = false;
                jogador.bAtivo = false;
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
