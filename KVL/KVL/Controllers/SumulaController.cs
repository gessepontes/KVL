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
using System.Data.Entity.Validation;

namespace KVL.Controllers
{
    public class SumulaController : Controller
    {
        private ModeloDados db = new ModeloDados();

        // GET: Sumula
        //public ActionResult Index()
        //{
        //    var sumula = db.Sumula.Include(s => s.PartidaCampeonato);
        //    return View(sumula.ToList());
        //}


        public ViewResult Index(string sortOrder, int? IDCampeonato, int? page)
        {

            ViewBag.ListaCampeonato = db.Campeonato;
            ViewBag.CurrentSort = sortOrder;
            ViewBag.NomeParam = string.IsNullOrEmpty(sortOrder) ? "Nome_desc" : "";
            ViewBag.DataParm = sortOrder == "Data" ? "Data_desc" : "Data";
            ViewBag.CampeonatoParm = sortOrder == "Campeonato" ? "Campeonato_desc" : "Campeonato";

            ViewBag.IDCampeonato = IDCampeonato;

            var sumula = from s in db.Sumula.Include(s => s.PartidaCampeonato)
                         select s;

            if (IDCampeonato != null)
            {
                sumula = sumula.Where(s => s.PartidaCampeonato.Inscrito.PreInscrito.IDCampeonato == IDCampeonato);
            }


            switch (sortOrder)
            {
                case "Nome_desc":
                    sumula = sumula.OrderByDescending(s => s.PartidaCampeonato.Inscrito.PreInscrito.Time.sNome);
                    break;
                case "Data":
                    sumula = sumula.OrderBy(s => s.PartidaCampeonato.dDataPartida);
                    break;
                case "Data_desc":
                    sumula = sumula.OrderByDescending(s => s.PartidaCampeonato.dDataPartida);
                    break;
                case "Campeonato":
                    sumula = sumula.OrderBy(s => s.PartidaCampeonato.Inscrito.PreInscrito.Campeonato.sNome);
                    break;
                case "Campeonato_desc":
                    sumula = sumula.OrderByDescending(s => s.PartidaCampeonato.Inscrito.PreInscrito.Campeonato.sNome);
                    break;
                default:
                    sumula = sumula.OrderBy(s => s.PartidaCampeonato.Inscrito.PreInscrito.Time.sNome);
                    break;
            }

            int pageSize = 10;
            int pageNumber = (page ?? 1);
            return View(sumula.ToPagedList(pageNumber, pageSize));
        }



        // GET: Sumula/Create
        public ActionResult Create(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Sumula sumula = db.Sumula.Find(id);

            if (sumula == null)
            {
                return HttpNotFound();
            }

            ViewBag.iQntGols1 = sumula.PartidaCampeonato.iQntGols1;
            ViewBag.iQntGols2 = sumula.PartidaCampeonato.iQntGols2;

            jogadoresInscritos(sumula.PartidaCampeonato.IDInscrito1, sumula.PartidaCampeonato.IDInscrito2, id);
            return View(sumula);
        }

        public void jogadoresInscritos(int IDInscrito1, int IDInscrito2, int? id)
        {
            ViewBag.JogadorInscrito = db.JogadorInscrito
                .GroupJoin(db.JogadorSumula.Where(p => p.IDSumula == id)
                , p => p.IDJogadorInscrito
                , c => c.IDJogadorInscrito
                , (p, c) => new { inscritos = p, sumulas = c })
                .SelectMany(s => s.sumulas.DefaultIfEmpty()
                , (s, sumulas) => new JogadorSumulaInscrito
                {
                    iNumCamisa = sumulas.iNumCamisa ?? 0,
                    sNome = s.inscritos.Jogador.Pessoa.sNome.ToUpper(),
                    IDJogadorInscrito = s.inscritos.IDJogadorInscrito,
                    IDTimeInscrito = s.inscritos.IDInscrito,
                }).Where(p => p.IDTimeInscrito == IDInscrito1).ToList();


            ViewBag.JogadorInscrito2 = db.JogadorInscrito
                .GroupJoin(db.JogadorSumula.Where(p => p.IDSumula == id)
                , p => p.IDJogadorInscrito
                , c => c.IDJogadorInscrito
                , (p, c) => new { inscritos = p, sumulas = c })
                .SelectMany(s => s.sumulas.DefaultIfEmpty()
                , (s, sumulas) => new JogadorSumulaInscrito
                {
                    iNumCamisa = sumulas.iNumCamisa ?? 0,
                    sNome = s.inscritos.Jogador.Pessoa.sNome.ToUpper(),
                    IDJogadorInscrito = s.inscritos.IDJogadorInscrito,
                    IDTimeInscrito = s.inscritos.IDInscrito,
                }).Where(p => p.IDTimeInscrito == IDInscrito2).ToList();
        }

        // POST: Sumula/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "IDSumula,IDPartidaCampeonato,sObservacao,dDataCadastro")] Sumula sumula)
        {
            if (ModelState.IsValid)
            {
                using (var transaction = db.Database.BeginTransaction())
                {
                    try
                    {


                        PartidaCampeonato partidaCampeonato = new PartidaCampeonato();
                        partidaCampeonato.iQntGols1 = sumula.PartidaCampeonato.iQntGols1;
                        partidaCampeonato.iQntGols2 = sumula.PartidaCampeonato.iQntGols2;

                        db.Entry(partidaCampeonato).State = EntityState.Modified;
                        db.Entry(partidaCampeonato).Property("IDInscrito1").IsModified = false;
                        db.Entry(partidaCampeonato).Property("IDInscrito2").IsModified = false;
                        db.Entry(partidaCampeonato).Property("dDataPartida").IsModified = false;
                        db.Entry(partidaCampeonato).Property("sHoraPartida").IsModified = false;
                        db.Entry(partidaCampeonato).Property("dDataCadastro").IsModified = false;

                        db.SaveChanges();

                        db.Entry(sumula).State = EntityState.Modified;
                        db.Entry(sumula).Property("dDataCadastro").IsModified = false;
                        db.SaveChanges();

                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        ex.Message.ToString();
                        transaction.Rollback();
                    }
                }
            }

            return View(sumula);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditSumula([Bind(Include = "IDSumula,IDPartidaCampeonato,sObservacao,dDataCadastro")] Sumula sumula, int? iQntGols1, int? iQntGols2)
        {
            if (ModelState.IsValid)
            {

                if (iQntGols1 < 0 || iQntGols2 < 0)
                {
                    return RedirectToAction("Create/" + sumula.IDSumula).ComMensagem("Quantidade de gols não pode ser menor que 0.", "alert-warning");
                }

                using (var transaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        PartidaCampeonato partidaCampeonato = db.PartidaCampeonato.Find(sumula.IDPartidaCampeonato);
                        if (partidaCampeonato == null)
                        {
                            return HttpNotFound();
                        }


                        partidaCampeonato.iQntGols1 = iQntGols1;
                        partidaCampeonato.iQntGols2 = iQntGols2;

                        db.Entry(partidaCampeonato).State = EntityState.Modified;
                        db.Entry(partidaCampeonato).Property("dDataPartida").IsModified = false;
                        db.Entry(partidaCampeonato).Property("iRodada").IsModified = false;
                        db.Entry(partidaCampeonato).Property("IDInscrito1").IsModified = false;
                        db.Entry(partidaCampeonato).Property("IDInscrito2").IsModified = false;
                        db.Entry(partidaCampeonato).Property("sHoraPartida").IsModified = false;
                        db.Entry(partidaCampeonato).Property("dDataCadastro").IsModified = false;

                        db.SaveChanges();

                        db.Entry(sumula).State = EntityState.Modified;
                        db.Entry(sumula).Property("dDataCadastro").IsModified = false;
                        db.SaveChanges();

                        transaction.Commit();

                        return RedirectToAction("Index").ComMensagem("Operação realizada com sucesso.", "alert-success");
                    }
                    catch (DbEntityValidationException e)
                    {
                        foreach (var eve in e.EntityValidationErrors)
                        {
                            Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                                eve.Entry.Entity.GetType().Name, eve.Entry.State);
                            foreach (var ve in eve.ValidationErrors)
                            {
                                Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                                    ve.PropertyName, ve.ErrorMessage);
                            }
                        }
                        return RedirectToAction("Create/" + sumula.IDSumula).ComMensagem("Erro ao realizar a operação.", "alert-danger");
                    }
                }
            }

            return View("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateJogador([Bind(Include = "IDJogadorSumula,IDJogadorInscrito,IDSumula,iNumCamisa,dDataCadastro")] JogadorSumula jogadorSumula, int? IDInscrito)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (jogadorSumula.iNumCamisa != 0)
                    {
                        if (jogadorSumula.iNumCamisa < 0)
                        {
                            return RedirectToAction("Create/" + jogadorSumula.IDSumula).ComMensagem("O número da camisa não pode ser menor que 0.", "alert-warning");
                        }
                        else
                        {
                            if (IDInscrito != null)
                            {
                                if (db.JogadorSumula.Where(p => p.iNumCamisa == jogadorSumula.iNumCamisa && p.Sumula.PartidaCampeonato.IDInscrito1 == IDInscrito && p.IDSumula == jogadorSumula.IDSumula).ToList().Count > 0)
                                {
                                    return RedirectToAction("Create/" + jogadorSumula.IDSumula).ComMensagem("Já existe jogador cadastrado com esta numeração.", "alert-warning");
                                }
                            }

                        }

                        using (var transaction = db.Database.BeginTransaction())
                        {
                            try
                            {
                                db.JogadorSumula.Add(jogadorSumula);
                                db.SaveChanges();

                                Cartao cartao = new Cartao();
                                cartao.iCodJogadorSumula = jogadorSumula.IDJogadorSumula;
                                cartao.iTipoCartao = TipoCartao.Nenhum;

                                db.Cartao.Add(cartao);
                                db.SaveChanges();

                                Gol gol = new Gol();

                                gol.iCodJogadorSumula = jogadorSumula.IDJogadorSumula;
                                gol.iQuantidade = 0;
                                db.Gol.Add(gol);
                                db.SaveChanges();

                                transaction.Commit();
                            }
                            catch (Exception ex)
                            {
                                ex.Message.ToString();
                                transaction.Rollback();
                            }
                        }
                    }
                    else
                    {
                        return RedirectToAction("Create/" + jogadorSumula.IDSumula).ComMensagem("Número da camisa é obrigatório.", "alert-warning");
                    }
                }
                catch (DbEntityValidationException dbEx)
                {
                    foreach (var validationErrors in dbEx.EntityValidationErrors)
                    {
                        foreach (var validationError in validationErrors.ValidationErrors)
                        {
                            System.Console.WriteLine("Property: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage);
                        }
                    }
                    return View().ComMensagem("Erro ao realizar a operação.", "alert-danger");
                }

            }

            return RedirectToAction("Create/" + jogadorSumula.IDSumula).ComMensagem("Operação realizada com sucesso.", "alert-success");
        }

        // POST: JogadorSumula/Delete/5
        [HttpPost, ActionName("DeleteJogador")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmedJogador(int IDJogadorInscrito, int IDSumula)
        {
            foreach (var cartao in db.Cartao.Where(p => p.JogadorSumula.IDJogadorInscrito == IDJogadorInscrito && p.JogadorSumula.IDSumula == IDSumula))
                db.Cartao.Remove(cartao);

            foreach (var gol in db.Gol.Where(p => p.JogadorSumula.IDJogadorInscrito == IDJogadorInscrito && p.JogadorSumula.IDSumula == IDSumula))
                db.Gol.Remove(gol);

            JogadorSumula jogadorSumula = db.JogadorSumula.Where(p => p.IDSumula == IDSumula && p.IDJogadorInscrito == IDJogadorInscrito).FirstOrDefault();
            db.JogadorSumula.Remove(jogadorSumula);
            db.SaveChanges();

            ViewBag.JogadorSumula = db.JogadorSumula.Where(p => p.IDSumula == IDSumula).ToList();
            return RedirectToAction("Create/" + IDSumula).ComMensagem("Operação realizada com sucesso.", "alert-success");

        }

        public ActionResult JogadorSumula(int? id, int? IDSumula)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var jogadorSumulaGolCartao = db.JogadorSumula
                .Join(db.Cartao
                      , c => c.IDJogadorSumula
                      , cm => cm.iCodJogadorSumula
                      , (c, cm) => new { c, cm })
                .Join(db.Gol
                      , ccm => ccm.cm.iCodJogadorSumula
                      , t => t.iCodJogadorSumula
                      , (ccm, t) => new JogadorSumulaGolCartao
                      {
                          IDJogadorInscrito = ccm.c.IDJogadorInscrito,
                          IDSumula = ccm.c.IDSumula,
                          sNome = ccm.c.JogadorInscrito.Jogador.Pessoa.sNome,
                          iCartao = ccm.cm.iTipoCartao,
                          IDJogadorSumula = ccm.c.IDJogadorSumula,
                          iGol = t.iQuantidade ?? 0
                      }).Where(p => p.IDJogadorInscrito == id && p.IDSumula == IDSumula).FirstOrDefault();

            ViewBag.QuantidadeGols = new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };

            if (jogadorSumulaGolCartao == null)
            {
                return HttpNotFound();
            }

            return PartialView("_jogadorSumula", jogadorSumulaGolCartao);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult JogadorSumula(JogadorSumulaGolCartao jogadorSumulaGolCartao)
        {
            if (jogadorSumulaGolCartao == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            JogadorSumula jogadorsumula = db.JogadorSumula.Find(jogadorSumulaGolCartao.IDJogadorSumula);

            if (jogadorsumula == null)
            {
                return HttpNotFound();
            }

            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    // TipoCartao tipoCartao = (TipoCartao)Enum.ToObject(typeof(TipoCartao), iCartao);

                    Cartao cartao = db.Cartao.Where(p => p.iCodJogadorSumula == jogadorsumula.IDJogadorSumula).FirstOrDefault();

                    db.Entry(cartao).State = EntityState.Modified;
                    db.Entry(cartao).Property("dDataCadastro").IsModified = false;
                    cartao.iTipoCartao = jogadorSumulaGolCartao.iCartao;
                    db.SaveChanges();

                    Gol gol = db.Gol.Where(p => p.iCodJogadorSumula == jogadorsumula.IDJogadorSumula).FirstOrDefault();
                    db.Entry(gol).State = EntityState.Modified;
                    db.Entry(gol).Property("dDataCadastro").IsModified = false;
                    gol.iQuantidade = jogadorSumulaGolCartao.iGol;
                    db.SaveChanges();

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    ex.Message.ToString();
                    transaction.Rollback();
                }
            }

            return RedirectToAction("Create/" + jogadorSumulaGolCartao.IDSumula).ComMensagem("Operação realizada com sucesso.", "alert-success");
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
