using System.Web.Mvc;
using OctoFramework.Logic.Cache;
using OctoFramework.Logic.Models.PetaPocos;
using OctoFramework.Logic.Models.ViewModels;
using Umbraco.Core;
using Umbraco.Core.Logging;
using Umbraco.Core.Models;
using Umbraco.Core.Persistence;
using Umbraco.Web;
using Umbraco.Web.Models;

namespace OctoFramework.Logic.Controllers
{
    public class BlogPostController : DefaultController
    {
        public BlogPostController(UmbracoContext context, UmbracoHelper helper) : base(context, helper)
        {
        }

        [UmbracoOutputCache]
        public ActionResult Index()
        {
            var model = Umbraco.AssignedContentItem;
            var tags = model.GetPropertyValue<string>("tag");
            var blogPostModel = new BlogPostViewModel(model)
            {
                ShowComments = model.GetPropertyValue<bool>("commentsEnabled"),
                BlogTags = tags != null ? tags.Split(',') : new string[1]
            };
            return View("BlogPost", blogPostModel);
        }

        public ActionResult RecordView(IPublishedContent model)
        {
            var logger = LoggerResolver.Current.Logger;
            var dbContext = ApplicationContext.DatabaseContext;
            var helper = new DatabaseSchemaHelper(dbContext.Database, logger, dbContext.SqlSyntax);
            //Check if the DB table does NOT exist
            if (helper.TableExist("PageViews"))
            {
                var sql = new Sql().Where<PageView>(x => x.Url == model.Url, dbContext.SqlSyntax);
                var record = dbContext.Database.FirstOrDefault<PageView>(sql);
                if (record == null)
                {
                    record = new PageView
                    {
                        Url = model.Url,
                        PublishDate = model.CreateDate,
                        Count = 0
                    };
                }
                else
                {
                    record.Count = record.Count++;
                }

                dbContext.Database.Insert(record);
            }
            return new EmptyResult();
        }
    }
}