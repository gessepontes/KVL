using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using KVL.Models;
using System.Web;
using System.IO;
using System;
using PagedList;
using KVL.Context;
using System.Text.RegularExpressions;

namespace KVL.Controllers
{
    public class PessoaController : Controller
    {
        private ModeloDados db = new ModeloDados();

        // GET: Pessoa
        [Authorize(Roles = "Administrador")]
        public ViewResult Index(string sortOrder, string currentFilter, string searchString, int? page)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.NomeParam = string.IsNullOrEmpty(sortOrder) ? "Nome_desc" : "";
            ViewBag.EmailParm = sortOrder == "Email" ? "Email_desc" : "Email";

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;

            var pessoas = from s in db.Pessoa
                          select s;

            if (!String.IsNullOrEmpty(searchString))
            {
                pessoas = pessoas.Where(s => s.sNome.ToUpper().Contains(searchString.ToUpper()));
            }
            switch (sortOrder)
            {
                case "Nome_desc":
                    pessoas = pessoas.OrderByDescending(s => s.sNome);
                    break;
                case "Email_desc":
                    pessoas = pessoas.OrderByDescending(s => s.sEmail);
                    break;
                case "Email":
                    pessoas = pessoas.OrderBy(s => s.sEmail);
                    break;
                default:
                    pessoas = pessoas.OrderBy(s => s.sNome);
                    break;
            }


            int pageSize = 10;
            int pageNumber = (page ?? 1);
            return View(pessoas.ToPagedList(pageNumber, pageSize));
        }

        // GET: Pessoa/Details/5
        [Authorize(Roles = "Administrador")]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Pessoa pessoa = db.Pessoa.Find(id);
            if (pessoa == null)
            {
                return HttpNotFound();
            }
            return View(pessoa);
        }

        // GET: Pessoa/Create
        [Authorize(Roles = "Administrador")]
        public ActionResult Create()
        {
            return View();
        }

        // POST: Pessoa/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public ActionResult Create([Bind(Include = "IDPessoa,sNome,sTelefone,sCpf,sRg,sFoto,dDataNascimento,sEmail,dDataCadastro")] Pessoa pessoa, HttpPostedFileBase upload)
        {
            if (ModelState.IsValid)
            {
                if (!isCPFCNPJ(pessoa.sCpf, true))
                {
                    return View(pessoa).ComMensagem("Cpf inválido.", "alert-warning");
                }

                db.Pessoa.Add(pessoa);
                db.SaveChanges();

                pessoa.sFoto = SalvarArquivo(upload, pessoa.IDPessoa);

                db.Entry(pessoa).State = EntityState.Modified;
                db.Entry(pessoa).Property("dDataCadastro").IsModified = false;

                db.SaveChanges();

                return RedirectToAction("Index").ComMensagem("Operação realizada com sucesso.", "alert-success");
            }

            return View(pessoa);
        }

        // GET: Pessoa/Edit/5
        [Authorize(Roles = "Administrador")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Pessoa pessoa = db.Pessoa.Find(id);
            if (pessoa == null)
            {
                return HttpNotFound();
            }
            return View(pessoa);
        }

        // POST: Pessoa/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Administrador")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "IDPessoa,sNome,sTelefone,sCpf,sRg,sFoto,dDataNascimento,sEmail,dDataCadastro")] Pessoa pessoa, HttpPostedFileBase upload)
        {
            if (ModelState.IsValid)
            {
                if (!isCPFCNPJ(pessoa.sCpf, true))
                {
                    return View(pessoa).ComMensagem("Cpf inválido.", "alert-warning");
                }

                db.Entry(pessoa).State = EntityState.Modified;

                if (upload == null)
                {
                    db.Entry(pessoa).Property("sFoto").IsModified = false;
                }
                else
                {
                    pessoa.sFoto = SalvarArquivo(upload, pessoa.IDPessoa);
                }

                db.Entry(pessoa).Property("dDataCadastro").IsModified = false;
                db.SaveChanges();
                return RedirectToAction("Index").ComMensagem("Operação realizada com sucesso.", "alert-success");
            }
            return View(pessoa);
        }


        [Authorize(Roles = "Administrador,Usuario")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PessoaTime([Bind(Include = "IDPessoa,sTelefone")] Pessoa pessoa, HttpPostedFileBase upload)
        {

            Pessoa pessoaOld = db.Pessoa.Find(pessoa.IDPessoa);

            if (pessoaOld != null)
            {
                db.Entry(pessoaOld).State = EntityState.Modified;

                if (upload == null)
                {
                    db.Entry(pessoaOld).Property("sFoto").IsModified = false;
                }
                else
                {
                    pessoaOld.sFoto = SalvarArquivo(upload, pessoaOld.IDPessoa);
                    Session["sFoto"] = pessoaOld.sFoto;
                }

                pessoaOld.sTelefone = pessoa.sTelefone;

                db.Entry(pessoaOld).Property("dDataCadastro").IsModified = false;
                db.SaveChanges();
                return RedirectToAction("PerfilTime", "Time").ComMensagem("Operação realizada com sucesso.", "alert-success");
            }
            return RedirectToAction("PerfilTime", "Time").ComMensagem("Erro ao realizar a operação.", "alert-danger");
        }


        // GET: Pessoa/Delete/5
        [Authorize(Roles = "Administrador")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Pessoa pessoa = db.Pessoa.Find(id);
            if (pessoa == null)
            {
                return HttpNotFound();
            }
            return View(pessoa);
        }

        // POST: Pessoa/Delete/5
        [Authorize(Roles = "Administrador")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                Pessoa pessoa = db.Pessoa.Find(id);
                db.Pessoa.Remove(pessoa);
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

        protected String SalvarArquivo(HttpPostedFileBase upload, int id)
        {
            string sRetorno = "";
            string sFilename = "";

            if (upload != null)
            {
                if (upload.ContentLength > (2 * 1024 * 1024))
                {
                    ModelState.AddModelError("CustomError", "Tamanho maximo 2 mb.");
                    sRetorno = "semimagem.jpg";
                }

                if (!(upload.ContentType == "image/jpeg" || upload.ContentType == "image/gif" || upload.ContentType == "image/png"))
                {
                    ModelState.AddModelError("CustomError", "Tipo válido jpg/gif.");
                    sRetorno = "semimagem.jpg";
                }

                if (sRetorno != "semimagem.jpg")
                {
                    var fileType = Path.GetFileName(upload.ContentType);
                    sFilename = Convert.ToString(id) + "." + fileType;

                    var path = Path.Combine(Server.MapPath("~/imagens/foto/"), sFilename);

                    upload.SaveAs(path);
                    sRetorno = sFilename;
                }
            }
            else
            {
                sRetorno = "semimagem.jpg";
            }

            return sRetorno;
        }


        // o metodo isCPFCNPJ recebe dois parâmetros:
        // uma string contendo o cpf ou cnpj a ser validado
        // e um valor do tipo boolean, indicando se o método irá
        // considerar como válido um cpf ou cnpj em branco.
        // o retorno do método também é do tipo boolean:
        // (true = cpf ou cnpj válido; false = cpf ou cnpj inválido)
        public static bool isCPFCNPJ(string cpfcnpj, bool vazio)
        {
            if (string.IsNullOrEmpty(cpfcnpj))
                return vazio;
            else
            {
                int[] d = new int[14];
                int[] v = new int[2];
                int j, i, soma;
                string Sequencia, SoNumero;

                SoNumero = Regex.Replace(cpfcnpj, "[^0-9]", string.Empty);

                //verificando se todos os numeros são iguais
                if (new string(SoNumero[0], SoNumero.Length) == SoNumero) return false;

                // se a quantidade de dígitos numérios for igual a 11
                // iremos verificar como CPF
                if (SoNumero.Length == 11)
                {
                    for (i = 0; i <= 10; i++) d[i] = Convert.ToInt32(SoNumero.Substring(i, 1));
                    for (i = 0; i <= 1; i++)
                    {
                        soma = 0;
                        for (j = 0; j <= 8 + i; j++) soma += d[j] * (10 + i - j);

                        v[i] = (soma * 10) % 11;
                        if (v[i] == 10) v[i] = 0;
                    }
                    return (v[0] == d[9] & v[1] == d[10]);
                }
                // se a quantidade de dígitos numérios for igual a 14
                // iremos verificar como CNPJ
                else if (SoNumero.Length == 14)
                {
                    Sequencia = "6543298765432";
                    for (i = 0; i <= 13; i++) d[i] = Convert.ToInt32(SoNumero.Substring(i, 1));
                    for (i = 0; i <= 1; i++)
                    {
                        soma = 0;
                        for (j = 0; j <= 11 + i; j++)
                            soma += d[j] * Convert.ToInt32(Sequencia.Substring(j + 1 - i, 1));

                        v[i] = (soma * 10) % 11;
                        if (v[i] == 10) v[i] = 0;
                    }
                    return (v[0] == d[12] & v[1] == d[13]);
                }
                // CPF ou CNPJ inválido se
                // a quantidade de dígitos numérios for diferente de 11 e 14
                else return false;
            }
        }
    }
}
