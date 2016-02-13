using System.Web.Mvc;

namespace KVL.Models
{
    public class TempDataActionResult : ActionResult
    {
        private readonly ActionResult _actionResult;
        private readonly string _mensagem;
        private readonly string _classeAlert;

        public TempDataActionResult(ActionResult actionResult, string mensagem, string classeAlert)
        {
            _actionResult = actionResult;
            _mensagem = mensagem;
            _classeAlert = classeAlert;
        }

        public override void ExecuteResult(ControllerContext context)
        {
            context.Controller.TempData["Mensagem"] = _mensagem;
            context.Controller.TempData["ClasseAlert"] = _classeAlert;
            _actionResult.ExecuteResult(context);
        }
    }
}