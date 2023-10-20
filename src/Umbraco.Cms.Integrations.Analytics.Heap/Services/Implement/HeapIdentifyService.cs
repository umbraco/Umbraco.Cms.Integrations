using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using Umbraco.Cms.Core.Security;
using Umbraco.Cms.Integrations.Analytics.Heap.Models;
using Umbraco.Extensions;

namespace Umbraco.Cms.Integrations.Analytics.Heap.Services.Implement;

public class HeapIdentifyService : IHeapIdentifyService
{
    private readonly IOptionsSnapshot<CookieAuthenticationOptions> _cookieOptionsSnapshot;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IMemberManager _memberManager;
    private readonly IBackOfficeUserManager _backOfficeUserManager;

    public HeapIdentifyService(
        IOptionsSnapshot<CookieAuthenticationOptions> cookieOptionsSnapshot, 
        IHttpContextAccessor httpContextAccessor,
        IMemberManager memberManager,
        IBackOfficeUserManager backOfficeUserManager)
    {
        _cookieOptionsSnapshot = cookieOptionsSnapshot;
        _httpContextAccessor = httpContextAccessor;
        _memberManager = memberManager;
        _backOfficeUserManager = backOfficeUserManager;
    }

    public async Task<User?> Identify()
    {
        var currentMemberIdentityUser = await GetCurrentMemberIdentityUser();
        if (currentMemberIdentityUser is not null)
        {
            return new User(
                currentMemberIdentityUser.Key.ToString(),
                currentMemberIdentityUser.Email,
                currentMemberIdentityUser.Name ?? string.Empty,
                string.Join(',', currentMemberIdentityUser.Roles.Select(p => p.RoleId).ToArray()));
        }

        var backOfficeIdentity = GetBackOfficeIdentity();
        if (backOfficeIdentity is not null && backOfficeIdentity.IsAuthenticated)
        {
            var currentBackOfficeUser = await _backOfficeUserManager.GetUserAsync(new ClaimsPrincipal(backOfficeIdentity));

            return currentBackOfficeUser is not null 
                ? new User(
                    currentBackOfficeUser.Key.ToString(),
                    currentBackOfficeUser.Email,
                    currentBackOfficeUser.Name ?? string.Empty,
                    string.Join(',', backOfficeIdentity.GetRoles()))
                : null;
        }

        return null;
    }

    private async Task<MemberIdentityUser?> GetCurrentMemberIdentityUser() => _memberManager.IsLoggedIn()
            ? await _memberManager.GetCurrentMemberAsync()
            : null;

    private ClaimsIdentity? GetBackOfficeIdentity()
    {
        var httpContext = _httpContextAccessor.HttpContext;

        if (httpContext == null)
            return null;

        var cookieOptions = _cookieOptionsSnapshot.Get(Core.Constants.Security.BackOfficeAuthenticationType);
        var cookieManager = new ChunkingCookieManager();
        var backOfficeCookie = cookieManager.GetRequestCookie(httpContext, cookieOptions.Cookie.Name!);

        if (string.IsNullOrEmpty(backOfficeCookie))
            return new ClaimsIdentity();

        var unprotected = cookieOptions.TicketDataFormat.Unprotect(backOfficeCookie!);
        var backOfficeIdentity = unprotected!.Principal.GetUmbracoIdentity();

        return backOfficeIdentity;
    }
}
