using System.Net.Http;
using System.Text;
using System.Web.Http;
using OctoFramework.Logic.Models.Custom;
using Umbraco.Web.WebApi;
using Umbraco.Core.Models;
using Umbraco.Core;
using Umbraco.Core.Persistence;
using OctoFramework.Logic.Models.PetaPocos;
using System;
using System.Linq;
using Umbraco.Core.Logging;

namespace OctoFramework.Logic.Controllers.WebAPI
{
    public class RedirectUrlManagementController : UmbracoAuthorizedApiController
    {
        //add paging
        [HttpGet]
        public RedirectUrlSearchResult SearchRedirectUrls(string searchTerm, int page = 0, int pageSize = 20)
        {
            var searchResult = new RedirectUrlSearchResult();
            var redirectUrlService = Services.RedirectUrlService;

            var redirects = redirectUrlService.GetAllRedirectUrls(page, pageSize, out var resultCount);
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                redirects = redirects.Where(x => x.Url.Contains(searchTerm));
            }

            searchResult.SearchResults = redirects;
            searchResult.TotalCount = resultCount;
            searchResult.CurrentPage = page;
            searchResult.PageCount = ((int)resultCount + pageSize - 1) / pageSize;
            searchResult.HasSearchResults = resultCount > 0;
            searchResult.HasExactMatch = (resultCount == 1);

            return searchResult;
        }

        [HttpGet]
        public HttpResponseMessage GetPublishedUrl(int id)
        {
            var publishedUrl = "#";
            if (id > 0)
            {
                publishedUrl = Umbraco.Url(id);
            }
            return new HttpResponseMessage()
            {
                Content = new StringContent(publishedUrl, Encoding.UTF8, "text/html")
            };
        }

        [HttpPost]
        public HttpResponseMessage DeleteRedirectUrl(RedirectUrl id)
        {
            try
            {
                var test = id as RedirectUrl;
                var redirectUrlService = Services.RedirectUrlService;
                redirectUrlService.Delete(id.ContentKey);
                return new HttpResponseMessage(System.Net.HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                LogHelper.Error<RedirectUrlManagementController>("Failed to delete Redirect", ex);
                return new HttpResponseMessage(System.Net.HttpStatusCode.BadRequest);
            }
        }

        [HttpPost]
        public HttpResponseMessage InsertRedirectUrl(int contentId, string url, string key)
        {
            try
            {
                var redirectUrlService = Services.RedirectUrlService;
                redirectUrlService.Register(url, new Guid(key));

                return new HttpResponseMessage(System.Net.HttpStatusCode.OK);
            }
            catch
            {
                return new HttpResponseMessage(System.Net.HttpStatusCode.InternalServerError);
            }
        }
    }
}