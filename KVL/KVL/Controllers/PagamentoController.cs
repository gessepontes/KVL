using KVL.Context;
using KVL.Models;
using PagedList;
using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace KVL.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class PagamentoController : Controller
    {
        // GET: Pagamento
        //public ActionResult Index()
        //{
        //    return View();
        //}

        private ModeloDados db = new ModeloDados();


        public ViewResult Index(string sortOrder, string currentFilter, string searchString, int? page)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.LoginParam = string.IsNullOrEmpty(sortOrder) ? "Login_desc" : "";

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;

            var pagamentos = from s in db.Pagamento.ToList()
                             select s;

            if (!string.IsNullOrEmpty(searchString))
            {
                pagamentos = pagamentos.Where(s => s.Login.sLogin.ToUpper().Contains(searchString.ToUpper()));
            }
            switch (sortOrder)
            {
                case "Login_desc":
                    pagamentos = pagamentos.OrderByDescending(s => s.Login.sLogin);
                    break;
                default:
                    pagamentos = pagamentos.OrderBy(s => s.Login.sLogin);
                    break;
            }

            int pageSize = 10;
            int pageNumber = (page ?? 1);
            return View(pagamentos.ToPagedList(pageNumber, pageSize));
        }


        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Pagamento pagamentos = db.Pagamento.Find(id);
            if (pagamentos == null)
            {
                return HttpNotFound();
            }
            return View(pagamentos);
        }

        public ActionResult Create()
        {
            ViewBag.IDLogin = new SelectList(db.Login.OrderBy(m => m.sLogin), "IDLogin", "sLogin");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "IDPagamento,IDLogin,dDataInicio,iQuantidadeMes,mValorPago,mValorReal,dDataCadastro")] Pagamento pagamentos)
        {
            if (ModelState.IsValid)
            {
                db.Pagamento.Add(pagamentos);
                db.SaveChanges();
                return RedirectToAction("Index").ComMensagem("Operação realizada com sucesso.", "alert-success");
            }

            ViewBag.IDLogin = new SelectList(db.Login.OrderBy(m => m.sLogin), "IDLogin", "sLogin");

            return View(pagamentos);
        }

        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Pagamento pagamentos = db.Pagamento.Find(id);
            if (pagamentos == null)
            {
                return HttpNotFound();
            }

            ViewBag.IDLogin = new SelectList(db.Login.OrderBy(m => m.sLogin), "IDLogin", "sLogin", pagamentos.IDLogin);         

            return View(pagamentos);
        }        

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "IDPagamento,IDLogin,dDataInicio,iQuantidadeMes,mValorPago,mValorReal,dDataCadastro")] Pagamento pagamentos)
        {

            if (ModelState.IsValid)
            {
                db.Entry(pagamentos).State = EntityState.Modified;
                db.Entry(pagamentos).Property("dDataCadastro").IsModified = false;
                db.SaveChanges();
                return RedirectToAction("Index").ComMensagem("Operação realizada com sucesso.", "alert-success");
            }

            ViewBag.IDLogin = new SelectList(db.Login.OrderBy(m => m.sLogin), "IDLogin", "sLogin", pagamentos.IDLogin);

            return View(pagamentos);
        }

        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Pagamento pagamentos = db.Pagamento.Find(id);
            if (pagamentos == null)
            {
                return HttpNotFound();
            }
            return View(pagamentos);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                Pagamento pagamentos = db.Pagamento.Find(id);
                db.Pagamento.Remove(pagamentos);
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