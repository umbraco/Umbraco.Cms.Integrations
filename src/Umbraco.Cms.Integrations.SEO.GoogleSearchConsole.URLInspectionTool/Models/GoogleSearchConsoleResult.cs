using Umbraco.Cms.Integrations.SEO.GoogleSearchConsole.URLInspectionTool.Models.Dtos;

namespace Umbraco.Cms.Integrations.SEO.GoogleSearchConsole.URLInspectionTool.Models;

public record GoogleSearchConsoleResult(bool Success, string? ErrorMessage = null, TokenDto? TokenDto = null);
