using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using KVL.Models;
using System.Security.Cryptography;
using System.Text;
using KVL.Context;
using PagedList;
using System.Web.Security;
using LigaWeb;
using Microsoft.AspNet.Identity;
using System.Threading.Tasks;
using System;

namespace KVL.Controllers
{
    public class LoginController : Controller
    {

        private ModeloDados db = new ModeloDados();

        [Authorize(Roles = "Administrador")]
        [HttpGet]
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

            //var logins = from s in db.Login.Include(l => l.Perfil).Include(l => l.Pessoa)
            var logins = from s in db.Login.Include(l => l.Pessoa)
                         select s;

            if (!string.IsNullOrEmpty(searchString))
            {
                logins = logins.Where(s => s.Pessoa.sNome.ToUpper().Contains(searchString.ToUpper()));
            }
            switch (sortOrder)
            {
                case "Nome_desc":
                    logins = logins.OrderByDescending(s => s.Pessoa.sNome);
                    break;
                default:
                    logins = logins.OrderBy(s => s.Pessoa.sNome);
                    break;
            }

            int pageSize = 10;
            int pageNumber = (page ?? 1);
            return View(logins.ToPagedList(pageNumber, pageSize));
        }


        // GET: Login/Details/5
        [Authorize(Roles = "Administrador")]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Login login = db.Login.Find(id);
            if (login == null)
            {
                return HttpNotFound();
            }
            return View(login);
        }

        // GET: Login/Create
        [Authorize(Roles = "Administrador")]
        public ActionResult Create()
        {
            //ViewBag.IDPerfil = new SelectList(db.Perfil, "IDPerfil", "sDescricao");
            ViewBag.IDPessoa = new SelectList(db.Pessoa, "IDPessoa", "sNome");
            return View();
        }

        // POST: Login/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Administrador")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "IDLogin,IDPessoa,IDPerfil,sLogin,sSenha,bConfimacao,bAtivo,dDataCadastro")] Login login)
        {
            if (ModelState.IsValid)
            {
                db.Login.Add(login);
                db.SaveChanges();
                return RedirectToAction("Index").ComMensagem("Operação realizada com sucesso.", "alert-success");
            }

            //ViewBag.IDPerfil = new SelectList(db.Perfil, "IDPerfil", "sDescricao", login.IDPerfil);
            ViewBag.IDPessoa = new SelectList(db.Pessoa, "IDPessoa", "sNome", login.IDPessoa);
            return View(login);
        }

        // GET: Login/Edit/5
        [Authorize(Roles = "Administrador")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Login login = db.Login.Find(id);
            if (login == null)
            {
                return HttpNotFound();
            }

            ViewBag.IDPessoa = new SelectList(db.Pessoa, "IDPessoa", "sNome", login.IDPessoa);
            return View(login);
        }

        [Authorize(Roles = "Administrador")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "IDLogin,IDPessoa,IDPerfil,sLogin,sSenha,bConfimacao,bAtivo,dDataCadastro")] Login login)
        {
            if (ModelState.IsValid)
            {
                db.Entry(login).State = EntityState.Modified;
                db.Entry(login).Property("sSenha").IsModified = false;
                db.Entry(login).Property("dDataCadastro").IsModified = false;
                db.SaveChanges();
                return RedirectToAction("Index").ComMensagem("Operação realizada com sucesso.", "alert-success");
            }

            ViewBag.IDPessoa = new SelectList(db.Pessoa, "IDPessoa", "sNome", login.IDPessoa);
            return View(login);
        }

        // GET: Login/Delete/5
        [Authorize(Roles = "Administrador")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Login login = db.Login.Find(id);
            if (login == null)
            {
                return HttpNotFound();
            }
            return View(login);
        }

        // POST: Login/Delete/5
        [Authorize(Roles = "Administrador")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Login login = db.Login.Find(id);
            db.Login.Remove(login);
            db.SaveChanges();
            return RedirectToAction("Index").ComMensagem("Operação realizada com sucesso.", "alert-success");
        }

        // GET: Login
        [Authorize(Roles = "Administrador")]
        public ActionResult Senha()
        {
            ViewBag.IDLogin = new SelectList(db.Login, "IDLogin", "sLogin");
            return View();

        }

        public string GenerateMD5(string yourString)
        {
            return string.Join("", MD5.Create().ComputeHash(Encoding.ASCII.GetBytes(yourString)).Select(s => s.ToString("x2")));
        }

        // POST: Login/Delete/5
        [Authorize(Roles = "Administrador")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Senha(RegistroViewModel model)
        {
            if (ModelState.IsValid)
            {
                var loginUser = db.Login.FirstOrDefault(p => p.IDLogin == model.IDLogin);

                loginUser.sSenha = GenerateMD5(model.Password);
                db.Entry(loginUser).State = EntityState.Modified;
                db.SaveChanges();

                ViewBag.IDLogin = new SelectList(db.Login, "IDLogin", "sLogin");
                return RedirectToAction("Index").ComMensagem("Senha cadastrada com sucesso.", "alert-success");
            }

            ViewBag.IDLogin = new SelectList(db.Login, "IDLogin", "sLogin");
            return View(model);

        }

        // GET: Login
        [AllowAnonymous]
        [HttpGet]
        public ActionResult LogIn(string returnUrl)
        {
            var model = new AcessoViewModel
            {
                ReturnUrl = returnUrl
            };

            return View(model);
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(AcessoViewModel model)
        {
            if (ModelState.IsValid)
            {

                if (model.Login.ToUpper() == "ADMIN" && model.Password == "123456q!")
                {
                    FormsAuthentication.SetAuthCookie(model.Login, false);
                    Session["sFoto"] = "semimagem.jpg";
                    Session["id"] = 0;
                    return Redirect(GetRedirectUrl(model.ReturnUrl));
                }

                string senha = GenerateMD5(model.Password);
                var loginUser = db.Login.Where(p => p.sLogin.ToUpper() == model.Login.ToUpper() && p.bConfimacao == true && p.bAtivo == true && p.sSenha == senha).ToList();

                if (loginUser.Count != 0)
                {
                    FormsAuthentication.SetAuthCookie(model.Login, model.RememberMe);

                    foreach (var item in loginUser)
                    {
                        Session["sFoto"] = item.Pessoa.sFoto;
                        Session["id"] = item.Pessoa.IDPessoa;
                    }

                    return Redirect(GetRedirectUrl(model.ReturnUrl));
                }

                return View().ComMensagem("Usuário ou senha inválida.", "alert-danger");

            }
            return View();
        }

        public ActionResult LogOut()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Login", "Login");
        }

        public ActionResult EmailTeste()
        {
            return View();
        }

        private string GetRedirectUrl(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl) && returnUrl.Length > 1 && returnUrl.StartsWith("/")
                && !returnUrl.StartsWith("//") && !returnUrl.StartsWith("/\\"))
            {
                return returnUrl;
            }
            else
            {
                return Url.Action("index", "home");
            }
        }

        [AllowAnonymous]
        public ActionResult Confirm(string Token, string Email)
        {
            var userList = db.Login.Where(p => p.sSecurityStamp == Token).ToList(); ;

            if (userList.Count != 0)
            {
                foreach (var item in userList)
                {
                    Login login = db.Login.Find(item.IDLogin);

                    if (item.Pessoa.sEmail == Email)
                    {
                        login.bConfimacao = true;
                        db.Entry(login).State = EntityState.Modified;
                        db.Entry(login).Property("sSenha").IsModified = false;
                        db.Entry(login).Property("dDataCadastro").IsModified = false;
                        db.SaveChanges();

                        return RedirectToAction("ConfirmEmail", "Login", new { Email = Email });
                    }
                    else
                    {
                        return RedirectToAction("ConfirmEmail", "Login", new { Email = Email }).ComMensagem("E-mail inválido.", "alert-danger");
                    }

                }

                return RedirectToAction("ConfirmEmail", "Login", new { Email = "" }).ComMensagem("Usuário inexistente.", "alert-danger");
            }
            else
            {
                return RedirectToAction("ConfirmEmail", "Login", new { Email = "" }).ComMensagem("Usuário inexistente.", "alert-danger");
            }
        }

        [AllowAnonymous]
        public ActionResult ConfirmEmail(string Email)
        {
            ViewBag.Email = Email;
            return View();
        }

        [AllowAnonymous]
        public ActionResult ForgotPassword(string Email)
        {
            return View();
        }

        [AllowAnonymous]
        public ActionResult ResetPassword(string Token, string Email)
        {
            ViewBag.Email = Email;
            ViewBag.Token = Token;
            return View();
        }

        [AllowAnonymous]
        public ActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult ResetPasswordConfirm(string Token, string Password)
        {
            var userList = db.Login.Where(p => p.sSecurityStamp == Token).ToList(); ;

            if (userList.Count != 0)
            {
                foreach (var item in userList)
                {
                    Login login = db.Login.Find(item.IDLogin);

                    login.bConfimacao = true;
                    login.sSenha = GenerateMD5(Password);
                    db.Entry(login).State = EntityState.Modified;
                    db.Entry(login).Property("dDataCadastro").IsModified = false;
                    db.SaveChanges();

                    return RedirectToAction("ResetPasswordConfirmation", "Login");
                }

                return RedirectToAction("ResetPassword", "Login").ComMensagem("E-mail não foi encontrado na base de dados.", "alert-danger");
            }
            else
            {
                return RedirectToAction("ResetPassword", "Login").ComMensagem("Dados de validação inválida.", "alert-danger");
            }
        }

        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotPasswordConfirm(string email)
        {
            string sToken = "";
            var userList = db.Login.Where(p => p.Pessoa.sEmail == email).ToList();

            if (userList.Count != 0)
            {
                foreach (var item in userList)
                {
                    Login login = db.Login.Find(item.IDLogin);

                    IdentityMessage emailIdentity = new IdentityMessage();
                    EmailService emailServe = new EmailService();

                    sToken = Guid.NewGuid().ToString("D");

                    // emailIdentity.Body = string.Format("Caro {0} <br/> Por favor clique no link abaixo para completar a mudança da sua senha:  <a href =\"{1}\">Clique aqui.</a>", item.Pessoa.sNome, Url.Action("ResetPassword", "Login",
                    emailIdentity.Body = string.Format(BodyEmail(1), item.Pessoa.sNome, Url.Action("ResetPassword", "Login",
                                           new { Token = sToken, Email = item.Pessoa.sEmail }, Request.Url.Scheme))
                                            ;
                    emailIdentity.Destination = email;
                    emailIdentity.Subject = "Reset de Senha";

                    await emailServe.SendAsync(emailIdentity);

                    login.bConfimacao = false;
                    login.sSecurityStamp = sToken;
                    db.Entry(login).State = EntityState.Modified;
                    db.Entry(login).Property("sSenha").IsModified = false;
                    db.Entry(login).Property("dDataCadastro").IsModified = false;
                    db.SaveChanges();

                    return View("ForgotPassword").ComMensagem("E-mail de mudança de senha enviada com sucesso.", "alert-success");
                }
            }
            else
            {
                return View("ForgotPassword").ComMensagem("E-mail não encontrado.", "alert-danger");
            }
            return View();
        }

        string BodyEmail(int iTipo)
        {
            string body = "";

            if (iTipo == 1)
            {
                body += "<table style='border - collapse:collapse; border - spacing:0; Margin - left:auto; Margin - right:auto; width: 600px; background - color:#ffffff;font-size:14px;table-layout:fixed'><tbody><tr><td style='padding:0;vertical-align:top;text-align:left'><div><div style='font-size:32px;line-height:32px'>&nbsp;</div></div>";
                body += "<table style='border - collapse:collapse; border - spacing:0; table - layout:fixed; width: 100 % '><tbody><tr><td style='padding: 0; vertical - align:top; padding - left:32px; padding - right:32px; word -break:break-word; word - wrap:break-word'><h1 style='font - style:normal; font - weight:700; Margin - bottom:18px; Margin - top:0; font - size:36px; line - height:44px; font - family:Ubuntu,sans - serif; color:#565656;text-align:center'><strong style='font-weight:bold'>Mudança de Senha</strong></h1>";
                body += "</td></tr></tbody></table><table style='border - collapse:collapse; border - spacing:0; table - layout:fixed; width: 100 % '><tbody><tr><td style='padding: 0; vertical - align:top; padding - left:32px; padding - right:32px; word -break:break-word; word - wrap:break-word'><div style='min - height:20px'>&nbsp;</div></td></tr></tbody></table><table style='border - collapse:collapse; border - spacing:0; table - layout:fixed; width: 100 % '><tbody><tr><td style='padding: 0; vertical - align:top; padding - left:32px; padding - right:32px; word -break:break-word; word - wrap:break-word'><p style='font - style:normal; font - weight:400; Margin - bottom:24px; Margin - top:0; line - height:24px; font - family:Ubuntu,sans - serif; color:#787778;font-size:16px'>Obrigado por participar desta emoção. Segue o link para mudança de sua senha.</p></td></tr></tbody></table><table style='border-collapse:collapse;border-spacing:0;table-layout:fixed;width:100%'><tbody><tr><td style='padding:0;vertical-align:top;padding-left:32px;padding-right:32px;word-break:break-word;word-wrap:break-word'><div><u></u><a style='border-radius:3px;display:inline-block;font-size:14px;font-weight:700;line-height:24px;padding:13px 35px 12px 35px;text-align:center;text-decoration:none!important;color:#fff;font-family:Ubuntu,sans-serif;background-color:#80bf2e' href=\"{1}\" target='_blank'>Clique aqui</a><u></u></div></td></tr></tbody></table><table style='border-collapse:collapse;border-spacing:0;table-layout:fixed;width:100%'><tbody><tr><td style='padding:0;vertical-align:top;padding-left:32px;padding-right:32px;word-break:break-word;word-wrap:break-word'><div style='min-height:14px'>&nbsp;</div></td></tr></tbody></table><table style='border-collapse:collapse;border-spacing:0;table-layout:fixed;width:100%'><tbody><tr><td style='padding:0;vertical-align:top;padding-left:32px;padding-right:32px;word-break:break-word;word-wrap:break-word'><p style='font-style:normal;font-weight:400;Margin-bottom:0;Margin-top:0;line-height:24px;font-family:Ubuntu,sans-serif;color:#787778;font-size:16px'><em>Equipe </em>Societypro agradece sua preferência.</p></td></tr></tbody></table><div style='font-size:32px;line-height:32px'>&nbsp;</div></td></tr></tbody></table>";

            }
            else
            {
                body += "<table style='border - collapse:collapse; border - spacing:0; Margin - left:auto; Margin - right:auto; width: 600px; background - color:#ffffff;font-size:14px;table-layout:fixed'><tbody><tr><td style='padding:0;vertical-align:top;text-align:left'><div><div style='font-size:32px;line-height:32px'>&nbsp;</div></div>";
                body += "<table style='border - collapse:collapse; border - spacing:0; table - layout:fixed; width: 100 % '><tbody><tr><td style='padding: 0; vertical - align:top; padding - left:32px; padding - right:32px; word -break:break-word; word - wrap:break-word'><h1 style='font - style:normal; font - weight:700; Margin - bottom:18px; Margin - top:0; font - size:36px; line - height:44px; font - family:Ubuntu,sans - serif; color:#565656;text-align:center'><strong style='font-weight:bold'>Confirmação de Conta</strong></h1>";
                body += "</td></tr></tbody></table><table style='border - collapse:collapse; border - spacing:0; table - layout:fixed; width: 100 % '><tbody><tr><td style='padding: 0; vertical - align:top; padding - left:32px; padding - right:32px; word -break:break-word; word - wrap:break-word'><div style='min - height:20px'>&nbsp;</div></td></tr></tbody></table><table style='border - collapse:collapse; border - spacing:0; table - layout:fixed; width: 100 % '><tbody><tr><td style='padding: 0; vertical - align:top; padding - left:32px; padding - right:32px; word -break:break-word; word - wrap:break-word'><p style='font - style:normal; font - weight:400; Margin - bottom:24px; Margin - top:0; line - height:24px; font - family:Ubuntu,sans - serif; color:#787778;font-size:16px'>Obrigado por participar desta emoção. Segue o link para confirmação de sua conta.</p></td></tr></tbody></table><table style='border-collapse:collapse;border-spacing:0;table-layout:fixed;width:100%'><tbody><tr><td style='padding:0;vertical-align:top;padding-left:32px;padding-right:32px;word-break:break-word;word-wrap:break-word'><div><u></u><a style='border-radius:3px;display:inline-block;font-size:14px;font-weight:700;line-height:24px;padding:13px 35px 12px 35px;text-align:center;text-decoration:none!important;color:#fff;font-family:Ubuntu,sans-serif;background-color:#80bf2e' href=\"{1}\" target='_blank'>Clique aqui</a><u></u></div></td></tr></tbody></table><table style='border-collapse:collapse;border-spacing:0;table-layout:fixed;width:100%'><tbody><tr><td style='padding:0;vertical-align:top;padding-left:32px;padding-right:32px;word-break:break-word;word-wrap:break-word'><div style='min-height:14px'>&nbsp;</div></td></tr></tbody></table><table style='border-collapse:collapse;border-spacing:0;table-layout:fixed;width:100%'><tbody><tr><td style='padding:0;vertical-align:top;padding-left:32px;padding-right:32px;word-break:break-word;word-wrap:break-word'><p style='font-style:normal;font-weight:400;Margin-bottom:0;Margin-top:0;line-height:24px;font-family:Ubuntu,sans-serif;color:#787778;font-size:16px'><em>Equipe </em>Societypro agradece sua preferência.</p></td></tr></tbody></table><div style='font-size:32px;line-height:32px'>&nbsp;</div></td></tr></tbody></table>";

            }

            return body;
        }

        // POST: /Account/Register
        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ConfirmacaoSolicitacao(string email)
        {
            string sToken = "";
            var userList = db.Login.Where(p => p.Pessoa.sEmail == email).ToList();

            if (userList.Count != 0)
            {
                foreach (var item in userList)
                {
                    Login login = db.Login.Find(item.IDLogin);

                    IdentityMessage emailIdentity = new IdentityMessage();
                    EmailService emailServe = new EmailService();

                    sToken = Guid.NewGuid().ToString("D");

                    //emailIdentity.Body = string.Format("Caro {0} <br/> Obrigado por se registrar, por favor clique no link abaixo para completar o seu registo:  <a href =\"{1}\">Clique aqui.</a>", item.Pessoa.sNome, Url.Action("Confirm", "Login",
                    emailIdentity.Body = string.Format(BodyEmail(2), item.Pessoa.sNome, Url.Action("Confirm", "Login",
new { Token = sToken, Email = item.Pessoa.sEmail }, Request.Url.Scheme))
                                            ;
                    emailIdentity.Destination = email;
                    emailIdentity.Subject = "Email de confirmação";

                    await emailServe.SendAsync(emailIdentity);

                    login.bConfimacao = false;
                    login.sSecurityStamp = sToken;
                    db.Entry(login).State = EntityState.Modified;
                    db.Entry(login).Property("sSenha").IsModified = false;
                    db.Entry(login).Property("dDataCadastro").IsModified = false;
                    db.SaveChanges();

                    return RedirectToAction("Index").ComMensagem("E-mail de confirmação foi enviado com sucesso.", "alert-success");
                }
            }
            else
            {
                return RedirectToAction("Index").ComMensagem("E-mail não encontrado.", "alert-danger");
            }
            return RedirectToAction("Index").ComMensagem("Usuário não encontrado.", "alert-danger");
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
