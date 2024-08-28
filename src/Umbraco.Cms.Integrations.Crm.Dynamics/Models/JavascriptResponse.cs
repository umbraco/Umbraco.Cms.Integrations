using System.Text;

namespace Umbraco.Cms.Integrations.Crm.Dynamics.Models
{
    public class JavascriptResponse
    {
        protected StringBuilder ScriptBuilder { get; set; }

        public bool Success { get; set; }

        public string Error { get; set; }

        public bool Failure => !Success;

        protected JavascriptResponse(bool success, string data, string error)
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
                ? "window.opener.postMessage({ type: 'dynamics:oauth:success', url: location.href, code: '" + data + "' }, '*');"
                : "window.opener.postMessage({ type: 'dynamics:oauth:error', url: location.href, response: '" + error + "' }, '*');");
            ScriptBuilder.AppendLine("window.close();");
            ScriptBuilder.AppendLine("</script>");
        }

        public override string ToString() => ScriptBuilder.ToString();

        public static string Ok(string data) => new JavascriptResponse(true, data, string.Empty).ToString();

        public static string Fail(string error) => new JavascriptResponse(false, string.Empty, error).ToString();
    }
}
