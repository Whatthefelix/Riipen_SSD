using FileHelpers;
using System;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Riipen_SSD.ModelBinders
{
    public class CsvModelBinder<T> : DefaultModelBinder where T : class
    {
        public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            var csv = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
            var file = ((csv.RawValue as HttpPostedFileBase[]) ?? Enumerable.Empty<HttpPostedFileBase>()).FirstOrDefault();

            if (file == null || file.ContentLength < 1)
            {
                return null;
            }

            using (var reader = new StreamReader(file.InputStream))
            {
                try
                {
                    var engine = new FileHelperEngine<T>();
                    return engine.ReadStream(reader);
                }
                catch (Exception c)
                {
                    bindingContext.ModelState.AddModelError(bindingContext.ModelName, c.Message);
                    return null;
                }
            }
        }
    }
}