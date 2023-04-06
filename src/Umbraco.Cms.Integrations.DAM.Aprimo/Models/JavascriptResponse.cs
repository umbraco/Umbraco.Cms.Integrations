
using System.Text;

namespace Umbraco.Cms.Integrations.DAM.Aprimo.Models
{
    public class JavascriptResponse
    {
        protected StringBuilder ScriptBuilder { get; set; }

        public bool Success { get; set; }

        public string Error { get; set; }

        public bool Failure => !Success;

        protected JavascriptResponse(bool success, string error)
        {
            if (success && !string.IsNullOrEmpty(error))
            {
                throw new ArgumentException("A succesful Response cannot have an error message.", error);
            }

            if (!success && string.IsNullOrEmpty(error))
            {
                throw new ArgumentException("A failure Response must have an error message.", error);
            }

            Success = success;

            Error = error;

            ScriptBuilder = new StringBuilder();
            ScriptBuilder.AppendLine("<script>");
            ScriptBuilder.AppendLine(success
                ? "window.opener.postMessage({ type: 'aprimo:oauth:success', url: location.href, response: '' }, '*');"
                : "window.opener.postMessage({ type: 'aprimo:oauth:error', url: location.href, response: '" + error + "' }, '*');");
            ScriptBuilder.AppendLine("window.close();");
            ScriptBuilder.AppendLine("</script>");
        }

        public override string ToString() => ScriptBuilder.ToString();

        public static string Ok() => new JavascriptResponse(true, string.Empty).ToString();

        public static string Fail(string error) => new JavascriptResponse(false, error).ToString();
    }
}
