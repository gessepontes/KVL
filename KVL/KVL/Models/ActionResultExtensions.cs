using System.Web.Mvc;

namespace KVL.Models
{
    public static class ActionResultExtensions
    {
        /// <summary>
        /// Redireciona para uma ActionResult retornando uma mensagem de confirmação para a view
        /// </summary>
        /// <param name="actionResult"></param>
        /// <param name="mensagem"></param>
        /// <param name="classeAlert">classe do alerta a ser exibido (alert-error, alert-success, alert-info)</param>
        /// <returns></returns>
        public static ActionResult ComMensagem(this ActionResult actionResult, string mensagem, string classeAlert)
        {
            return new TempDataActionResult(actionResult, mensagem, classeAlert);
        }
    }
}