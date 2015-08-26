using System.Web.Mvc;

namespace TheosWeb
{
    public class JsonRequestBehaviorAttribute : ActionFilterAttribute
    {
        private JsonRequestBehavior Behavior { get; set; }

        public JsonRequestBehaviorAttribute()
        {
            Behavior = JsonRequestBehavior.AllowGet;
        }

        public override void OnResultExecuting(ResultExecutingContext filterContext)
        {
            var result = filterContext.Result as JsonResult;

            if (result != null)
            {
                result.JsonRequestBehavior = Behavior;
            }
        }
    }
}