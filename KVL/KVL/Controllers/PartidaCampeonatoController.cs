using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using KVL.Context;
using KVL.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;

namespace KVL.Controllers
{
    public class PartidaCampeonatoController : Controller
    {
        private ModeloDados db = new ModeloDados();

        // GET: PartidaCampeonato
        public ViewResult Index(string sortOrder, string currentFilter, string searchString, int? IDCampeonato, int? page)
        {

            ViewBag.ListaCampeonato = db.Campeonato;
            ViewBag.CurrentSort = sortOrder;
            ViewBag.NomeParam = string.IsNullOrEmpty(sortOrder) ? "Nome_desc" : "";
            ViewBag.DataParm = sortOrder == "Data" ? "Data_desc" : "Data";
            ViewBag.CampeonatoParm = sortOrder == "Campeonato" ? "Campeonato_desc" : "Campeonato";

             ViewBag.CurrentFilter = searchString;
            ViewBag.IDCampeonato = IDCampeonato;

            var partidasCampeonato = from s in db.PartidaCampeonato.Include(p => p.Inscrito).Include(p => p.Inscrito1)
                                     select s;

            if (!string.IsNullOrEmpty(searchString))
            {
                partidasCampeonato = partidasCampeonato.Where(s => s.Inscrito.PreInscrito.Time.sNome.ToUpper().Contains(searchString.ToUpper()) || s.Inscrito1.PreInscrito.Time.sNome.ToUpper().Contains(searchString.ToUpper()));
            }

            if (IDCampeonato != null)
            {
                partidasCampeonato = partidasCampeonato.Where(s => s.Inscrito.PreInscrito.IDCampeonato == IDCampeonato);
            }


            switch (sortOrder)
            {
                case "Nome_desc":
                    partidasCampeonato = partidasCampeonato.OrderByDescending(s => s.Inscrito.PreInscrito.Time.sNome);
                    break;
                case "Data":
                    partidasCampeonato = partidasCampeonato.OrderBy(s => s.dDataPartida);
                    break;
                case "Data_desc":
                    partidasCampeonato = partidasCampeonato.OrderByDescending(s => s.dDataPartida);
                    break;
                case "Campeonato":
                    partidasCampeonato = partidasCampeonato.OrderBy(s => s.Inscrito.PreInscrito.Campeonato.sNome);
                    break;
                case "Campeonato_desc":
                    partidasCampeonato = partidasCampeonato.OrderByDescending(s => s.Inscrito.PreInscrito.Campeonato.sNome);
                    break;
                default:
                    partidasCampeonato = partidasCampeonato.OrderBy(s => s.iRodada);
                    //partidasCampeonato = partidasCampeonato.OrderBy(s => s.IDPartidaCampeonato);
                    break;
            }

            int pageSize = 10;
            int pageNumber = (page ?? 1);
            return View(partidasCampeonato.ToPagedList(pageNumber, pageSize));
        }


        // GET: PartidaCampeonato/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PartidaCampeonato partidaCampeonato = db.PartidaCampeonato.Find(id);
            if (partidaCampeonato == null)
            {
                return HttpNotFound();
            }
            return View(partidaCampeonato);
        }

        // GET: PartidaCampeonato/Create
        public ActionResult Create()
        {
            ViewBag.ListaCampeonato = db.Campeonato;
            return View();
        }

        // GET: PartidaCampeonato/Create
        public ActionResult GerarPartidasCampeonato()
        {

            CampeonatoGrupo campeonatoGrupo = new CampeonatoGrupo();
            IEnumerable<IDGrupo> actionTypes = Enum.GetValues(typeof(IDGrupo))
                                                       .Cast<IDGrupo>();
            ViewBag.ListaGrupos = from action in actionTypes
                                  select new SelectListItem
                                  {
                                      Text = action.ToString(),
                                      Value = ((int)action).ToString()
                                  };


            ViewBag.ListaCampeonato = db.Campeonato;
            return View();
        }

        public int calculaFatorial(int num)
        {

            if (num == 1) return num;
            else return calculaFatorial(num - 1) * num;
        }


        // GET: PartidaCampeonato/Create
        [HttpPost]
        public ActionResult GerarPartidasCampeonato(int? IDCampeonato, int? IDGrupoP)
        {

            try
            {
                ViewBag.ListaCampeonato = db.Campeonato;

                CampeonatoGrupo campeonatoGrupo = new CampeonatoGrupo();
                IEnumerable<IDGrupo> actionTypes = Enum.GetValues(typeof(IDGrupo))
                                                           .Cast<IDGrupo>();
                ViewBag.ListaGrupos = from action in actionTypes
                                      select new SelectListItem
                                      {
                                          Text = action.ToString(),
                                          Value = ((int)action).ToString()
                                      };

                Random rnd = new Random();
                int qtde_times = 0;
                //int iRodada = 1;
                //int iQntdJogoTimeRodada = 0;
                int iQntdRodadas = 0;
                int cont = 0;
                int turno = 0;
                int[] timesInscritos;
                string[] partidas;


                if (IDCampeonato == null)
                {
                    return View().ComMensagem("Selecione o Campeonato.", "alert-warning");
                }
                else
                {
                    var campeonato = db.Campeonato.Find(IDCampeonato);
                    

                    switch (campeonato.iTipoCampeonato)
                    {
                        case TipoCampeonato.Grupos:

                            if (IDGrupoP == null)
                            {
                                return View().ComMensagem("Selecione o grupo.", "alert-warning");
                            }
                            else
                            {
                                IDGrupo grupo = (IDGrupo)Enum.ToObject(typeof(IDGrupo), IDGrupoP);

                                var inscritosGrupos = db.CampeonatoGrupo.Where(p => p.IDGrupo == grupo && p.Inscrito.PreInscrito.IDCampeonato == IDCampeonato).ToList();

                                int[] timesGrupo = new int[inscritosGrupos.Count];

                                foreach (CampeonatoGrupo e in inscritosGrupos)
                                {
                                    timesGrupo[cont] = e.IDInscrito;
                                    cont++;
                                }

                                timesInscritos = timesGrupo;
                            }
                            break;
                        case TipoCampeonato.MataMata:
                            return View().ComMensagem("Regra do Mata-Mata não foi emplementada.", "alert-warning");
                        default:

                            var inscritos = db.Inscrito.Where(p => p.PreInscrito.Campeonato.IDCampeonato == IDCampeonato).ToList();

                            int[] times = new int[inscritos.Count];

                            foreach (Inscrito e in inscritos)
                            {
                                times[cont] = e.IDInscrito;
                                cont++;
                            }

                            timesInscritos = times;
                            break;
                    }


                    qtde_times = timesInscritos.Count(); // Quantidade de Times

                    if (qtde_times % 2 == 0) { iQntdRodadas = qtde_times - 1; } else { iQntdRodadas = qtde_times; }

                    if (campeonato.bIdaVolta)
                    {
                        turno = 2;
                        partidas = new string[calculaFatorial(qtde_times) / calculaFatorial(qtde_times - 2)];
                    }
                    else
                    {
                        turno = 1;
                        partidas = new string[calculaFatorial(qtde_times) / calculaFatorial(qtde_times - 2) / 2];
                    }

                    cont = 0;

                    for (int t = 0; t < turno; t++)
                    {
                        for (int i = 0; i < qtde_times; i++)
                        { //For para caminhar entre os times
                            for (int j = i; j < qtde_times; j++)
                            { //For para caminha entre os adversários
                                if (timesInscritos[i] != timesInscritos[j])
                                {

                                    using (var transaction = db.Database.BeginTransaction())
                                    {
                                        try
                                        {
                                            PartidaCampeonato partidaCampeonato = new PartidaCampeonato();
                                            partidaCampeonato.IDInscrito1 = timesInscritos[i];
                                            partidaCampeonato.IDInscrito2 = timesInscritos[j];
                                            partidaCampeonato.iQntGols1 = 0;
                                            partidaCampeonato.iQntGols2 = 0;
                                            partidaCampeonato.dDataPartida = DateTime.Now;
                                            partidaCampeonato.dDataCadastro = DateTime.Now;
                                            partidaCampeonato.sHoraPartida = "";
                                            partidaCampeonato.iRodada = 0;

                                            db.PartidaCampeonato.Add(partidaCampeonato);
                                            db.SaveChanges();

                                            Sumula sumula = new Sumula();

                                            sumula.IDPartidaCampeonato = partidaCampeonato.IDPartidaCampeonato;
                                            sumula.sObservacao = "";
                                            sumula.dDataCadastro = DateTime.Now;
                                            db.Sumula.Add(sumula);
                                            db.SaveChanges();

                                            transaction.Commit();
                                        }
                                        catch (Exception ex)
                                        {
                                            ex.Message.ToString();
                                            transaction.Rollback();
                                        }
                                    }
                                    //verifica pra não deixar jogar um time contra ele mesmo
                                    //partidaCampeonato.IDInscrito1 = timesInscritos[i];
                                    //partidaCampeonato.IDInscrito2 = timesInscritos[j];
                                    //partidaCampeonato.iQntGols1 = 0;
                                    //partidaCampeonato.iQntGols2 = 0;
                                    //partidaCampeonato.dDataPartida = DateTime.Now;
                                    //partidaCampeonato.dDataCadastro = DateTime.Now;
                                    //partidaCampeonato.sHoraPartida = "";

                                    //iQntdJogoTimeRodada = db.PartidaCampeonato.Where(p => p.iRodada == iRodada && (p.IDInscrito1 == partidaCampeonato.IDInscrito1 || p.IDInscrito2 == partidaCampeonato.IDInscrito1 || p.IDInscrito1 == partidaCampeonato.IDInscrito2 || p.IDInscrito2 == partidaCampeonato.IDInscrito2)).Count();

                                    //while (iQntdJogoTimeRodada == 0)
                                    //{
                                    //    iRodada = rnd.Next(1, iQntdRodadas);

                                    //    iQntdJogoTimeRodada = db.PartidaCampeonato.Where(p => p.iRodada == iRodada && (p.IDInscrito1 == partidaCampeonato.IDInscrito1 || p.IDInscrito2 == partidaCampeonato.IDInscrito1 || p.IDInscrito1 == partidaCampeonato.IDInscrito2 || p.IDInscrito2 == partidaCampeonato.IDInscrito2)).Count();

                                    //    cont++;

                                    //    if (cont >= qtde_times + iQntdRodadas)
                                    //    {
                                    //        iRodada = qtde_times;
                                    //        break;
                                    //    }
                                    //}

                                    //cont = 0;

                                    //partidaCampeonato.iRodada = iRodada;
                                    //partidaCampeonato.iRodada = 1;

                                    //db.PartidaCampeonato.Add(partidaCampeonato);
                                    //db.SaveChanges();
                                }
                            }
                        }

                    }
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

            return RedirectToAction("Index").ComMensagem("Partidas geradas com sucesso.", "alert-success");
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


        // POST: PartidaCampeonato/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "IDPartidaCampeonato,IDInscrito1,IDInscrito2,iQntGols1,iQntGols2,dDataPartida,sHoraPartida,iRodada,dDataCadastro")] PartidaCampeonato partidaCampeonato)
        {

            if (ModelState.IsValid)
            {
                if (partidaCampeonato.IDInscrito1 == partidaCampeonato.IDInscrito2)
                {
                    ViewBag.ListaCampeonato = db.Campeonato;
                    return View(partidaCampeonato).ComMensagem("Time 1 não pode ser igual a time 2.", "alert-warning");
                }

                using (var transaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        db.PartidaCampeonato.Add(partidaCampeonato);
                        db.SaveChanges();

                        Sumula sumula = new Sumula();

                        sumula.IDPartidaCampeonato = partidaCampeonato.IDPartidaCampeonato;
                        sumula.sObservacao = "";
                        db.Sumula.Add(sumula);
                        db.SaveChanges();

                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        ex.Message.ToString();
                        transaction.Rollback();
                    }
                }

                return RedirectToAction("Index").ComMensagem("Operação realizada com sucesso.", "alert-success");
            }

            ViewBag.ListaCampeonato = db.Campeonato;

            return View(partidaCampeonato);
        }

        // GET: PartidaCampeonato/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PartidaCampeonato partidaCampeonato = db.PartidaCampeonato.Find(id);
            if (partidaCampeonato == null)
            {
                return HttpNotFound();
            }

            ViewBag.IDInscrito1 = new SelectList(db.Inscrito.Select(x => new SelectListItem
            {
                Text = x.PreInscrito.Time.sNome.ToUpper(),
                Value = x.IDInscrito.ToString()
            }), "Value", "Text", partidaCampeonato.IDInscrito1);

            ViewBag.IDInscrito2 = new SelectList(db.Inscrito.Select(x => new SelectListItem
            {
                Text = x.PreInscrito.Time.sNome.ToUpper(),
                Value = x.IDInscrito.ToString()
            }), "Value", "Text", partidaCampeonato.IDInscrito2);

            return View(partidaCampeonato);
        }

        // POST: PartidaCampeonato/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "IDPartidaCampeonato,IDInscrito1,IDInscrito2,iQntGols1,iQntGols2,dDataPartida,sHoraPartida,iRodada,dDataCadastro")] PartidaCampeonato partidaCampeonato)
        {
            if (ModelState.IsValid)
            {
                db.Entry(partidaCampeonato).State = EntityState.Modified;
                db.Entry(partidaCampeonato).Property("dDataCadastro").IsModified = false;
                db.Entry(partidaCampeonato).Property("IDInscrito1").IsModified = false;
                db.Entry(partidaCampeonato).Property("IDInscrito2").IsModified = false;
                db.SaveChanges();
                return RedirectToAction("Index").ComMensagem("Operação realizada com sucesso.", "alert-success");
            }

            ViewBag.IDInscrito1 = new SelectList(db.Inscrito.Select(x => new SelectListItem
            {
                Text = x.PreInscrito.Time.sNome.ToUpper(),
                Value = x.IDInscrito.ToString()
            }).ToList().OrderBy(p => p.Text), "Value", "Text", partidaCampeonato.IDInscrito1);

            ViewBag.IDInscrito2 = new SelectList(db.Inscrito.Select(x => new SelectListItem
            {
                Text = x.PreInscrito.Time.sNome.ToUpper(),
                Value = x.IDInscrito.ToString()
            }).ToList().OrderBy(p => p.Text), "Value", "Text", partidaCampeonato.IDInscrito2);


            return View(partidaCampeonato);
        }

        // GET: PartidaCampeonato/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PartidaCampeonato partidaCampeonato = db.PartidaCampeonato.Find(id);
            if (partidaCampeonato == null)
            {
                return HttpNotFound();
            }
            return View(partidaCampeonato);
        }

        // POST: PartidaCampeonato/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                PartidaCampeonato partidaCampeonato = db.PartidaCampeonato.Find(id);
                db.PartidaCampeonato.Remove(partidaCampeonato);
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
