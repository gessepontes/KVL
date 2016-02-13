using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using KVL.Context;
using KVL.Models;
using PagedList;

namespace KVL.Controllers
{
    public class PartidaAmistosaController : Controller
    {
        private ModeloDados db = new ModeloDados();

        // GET: PartidaAmistosa

        public ViewResult Index(string sortOrder, string currentFilter, string searchString, int? page)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.NomeParam = string.IsNullOrEmpty(sortOrder) ? "Nome_desc" : "";
            ViewBag.DataParm = sortOrder == "Data" ? "Data_desc" : "Data";

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;

            var partidaAmistosa = from s in db.PartidaAmistosa.Include(p => p.Time).Include(p => p.Time1)
                                  select s;

            if (!string.IsNullOrEmpty(searchString))
            {
                partidaAmistosa = partidaAmistosa.Where(s => s.Time.sNome.ToUpper().Contains(searchString.ToUpper()) || s.Time1.sNome.ToUpper().Contains(searchString.ToUpper()));
            }
            switch (sortOrder)
            {
                case "Nome_desc":
                    partidaAmistosa = partidaAmistosa.OrderByDescending(s => s.Time.sNome);
                    break;
                case "Data":
                    partidaAmistosa = partidaAmistosa.OrderBy(s => s.dDataPartida);
                    break;
                case "Data_desc":
                    partidaAmistosa = partidaAmistosa.OrderByDescending(s => s.dDataPartida);
                    break;
                default:
                    partidaAmistosa = partidaAmistosa.OrderBy(s => s.Time.sNome);
                    break;
            }

            int pageSize = 10;
            int pageNumber = (page ?? 1);
            return View(partidaAmistosa.ToPagedList(pageNumber, pageSize));
        }


        // GET: PartidaAmistosa/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PartidaAmistosa partidaAmistosa = db.PartidaAmistosa.Find(id);
            if (partidaAmistosa == null)
            {
                return HttpNotFound();
            }
            return View(partidaAmistosa);
        }

        // GET: PartidaAmistosa/Create
        public ActionResult Create()
        {
            PartidaAmistosa partidaAmistosa = new PartidaAmistosa();

            ViewBag.iCodCampo = new SelectList(db.Campo.OrderBy(s => s.sNome), "IDCampo", "sNome");
            ViewBag.IDTime1 = new SelectList(db.Time.OrderBy(s => s.sNome), "IDTime", "sNome");
            ViewBag.IDTime2 = new SelectList(db.Time.OrderBy(s => s.sNome), "IDTime", "sNome");
            return View(partidaAmistosa);
        }

        // POST: PartidaAmistosa/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "IDPartidaAmistosa,IDTime1,IDTime2,iQntGols1,iQntGols2,iCodCampo,dDataPartida,sHoraPartida,dDataCadastro")] PartidaAmistosa partidaAmistosa)
        {
            ViewBag.iCodCampo = new SelectList(db.Campo.OrderBy(s => s.sNome), "IDCampo", "sNome", partidaAmistosa.iCodCampo);
            ViewBag.IDTime1 = new SelectList(db.Time.OrderBy(s => s.sNome), "IDTime", "sNome", partidaAmistosa.IDTime1);
            ViewBag.IDTime2 = new SelectList(db.Time.OrderBy(s => s.sNome), "IDTime", "sNome", partidaAmistosa.IDTime2);

            if (ModelState.IsValid)
            {
                if (partidaAmistosa.IDTime1 == partidaAmistosa.IDTime2)
                {
                    return View(partidaAmistosa).ComMensagem("Time 1 não pode ser igual ao Time 2.", "alert-warning");
                }

                db.PartidaAmistosa.Add(partidaAmistosa);
                db.SaveChanges();
                return RedirectToAction("Index").ComMensagem("Operação realizada com sucesso.", "alert-success");
            }

            return View(partidaAmistosa);
        }

        // GET: PartidaAmistosa/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PartidaAmistosa partidaAmistosa = db.PartidaAmistosa.Find(id);
            if (partidaAmistosa == null)
            {
                return HttpNotFound();
            }

            ViewBag.iCodCampo = new SelectList(db.Campo.OrderBy(s => s.sNome), "IDCampo", "sNome",partidaAmistosa.iCodCampo);
            ViewBag.IDTime1 = new SelectList(db.Time.OrderBy(s => s.sNome), "IDTime", "sNome", partidaAmistosa.IDTime1);
            ViewBag.IDTime2 = new SelectList(db.Time.OrderBy(s => s.sNome), "IDTime", "sNome", partidaAmistosa.IDTime2);
            return View(partidaAmistosa);
        }

        // POST: PartidaAmistosa/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "IDPartidaAmistosa,IDTime1,IDTime2,iQntGols1,iQntGols2,iCodCampo,dDataPartida,sHoraPartida,dDataCadastro")] PartidaAmistosa partidaAmistosa)
        {
            ViewBag.iCodCampo = new SelectList(db.Campo.OrderBy(s => s.sNome), "IDCampo", "sNome", partidaAmistosa.iCodCampo);
            ViewBag.IDTime1 = new SelectList(db.Time.OrderBy(s => s.sNome), "IDTime", "sNome", partidaAmistosa.IDTime1);
            ViewBag.IDTime2 = new SelectList(db.Time.OrderBy(s => s.sNome), "IDTime", "sNome", partidaAmistosa.IDTime2);

            if (ModelState.IsValid)
            {
                if (partidaAmistosa.IDTime1 == partidaAmistosa.IDTime2)
                {
                    return View(partidaAmistosa).ComMensagem("Time 1 não pode ser igual ao Time 2.", "alert-warning");
                }

                db.Entry(partidaAmistosa).State = EntityState.Modified;
                db.Entry(partidaAmistosa).Property("dDataCadastro").IsModified = false;
                db.SaveChanges();
                return RedirectToAction("Index").ComMensagem("Operação realizada com sucesso.", "alert-success");
            }

            return View(partidaAmistosa);
        }

        // GET: PartidaAmistosa/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PartidaAmistosa partidaAmistosa = db.PartidaAmistosa.Find(id);
            if (partidaAmistosa == null)
            {
                return HttpNotFound();
            }
            return View(partidaAmistosa);
        }

        // POST: PartidaAmistosa/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            PartidaAmistosa partidaAmistosa = db.PartidaAmistosa.Find(id);
            db.PartidaAmistosa.Remove(partidaAmistosa);
            db.SaveChanges();
            return RedirectToAction("Index").ComMensagem("Operação realizada com sucesso.", "alert-success");
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
