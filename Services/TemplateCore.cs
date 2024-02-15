using static PosMobileApi.Constants.EnumCollections;

namespace PosMobileApi.Services
{
    public interface ITemplateCore
    {
        public string GetTemplate(Template template);
    }

    public class TemplateCore : ITemplateCore
    {
        private readonly string AppleCallBackHtmlTemplate;

        public TemplateCore()
        {
            try
            {
                AppleCallBackHtmlTemplate = File.ReadAllText("applecallback.html");
            }
            catch { }
        }

        public string GetTemplate(Template template)
        {
            return template switch
            {
                Template.AppleCallBack => AppleCallBackHtmlTemplate,
                _ => string.Empty,
            };
        }
    }
}
