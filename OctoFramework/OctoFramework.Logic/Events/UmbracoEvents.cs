using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Configuration;
using System.Web.Http;
using DevTrends.MvcDonutCaching;
using OctoFramework.Logic.ContentFinder;
using OctoFramework.Logic.Controllers;
using OctoFramework.Logic.Models.PetaPocos;
using OctoFramework.Logic.UrlProvider;
using OctoFramework.Logic.Utilities;
using Examine;
using Umbraco.Core;
using Umbraco.Core.Events;
using Umbraco.Core.Logging;
using Umbraco.Core.Models;
using Umbraco.Core.Persistence;
using Umbraco.Core.Publishing;
using Umbraco.Core.Services;
using Umbraco.Web.Mvc;
using Umbraco.Web.Routing;
using Umbraco.Core.Strings;

namespace OctoFramework.Logic.Events
{
    public class UmbracoEvents : ApplicationEventHandler
    {
        private static ServiceContext Services
        {
            get { return ApplicationContext.Current.Services; }
        }

        private OutputCacheManager cacheManager = new OutputCacheManager();

        protected override void ApplicationStarting(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
        {
            //Events
            ContentService.Created += Content_New;
            ContentService.Saving += ContentService_Saving;
            ContentService.Saved += ContentService_Saved;
            ContentService.Published += Content_Published;
            ContentService.UnPublished += Content_Unpublished;
            ContentService.Moved += Content_Moved;
            ContentService.Trashed += Content_Trashed;
            ContentService.Deleted += Content_Deleted;
            MediaService.Saved += Media_Saved;
            MediaService.Saving += Media_Saving;
            //With the url providers we can change node urls.
            UrlProviderResolver.Current.InsertTypeBefore<DefaultUrlProvider, BlogPostsUrlProvider>();
            UrlSegmentProviderResolver.Current.InsertTypeBefore<DefaultUrlSegmentProvider, SeoMetaDataUrlSegmentProvider>();
            //With the content finder we can match nodes to urls.
            ContentFinderResolver.Current.InsertTypeBefore<ContentFinderByNotFoundHandlers, BlogPostsContentFinder>();
            //Add a web api handler. Here we can change the values from each web api call.
            //GlobalConfiguration.Configuration.MessageHandlers.Add(new WebApiHandler());
            //By registering this here we can make sure that if route hijacking doesn't find a controller it will use this controller.
            //That way each page will always be routed through one of our controllers.
            DefaultRenderMvcControllerResolver.Current.SetDefaultControllerType(typeof(DefaultController));
        }

        protected override void ApplicationStarted(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
        {
            ExamineManager.Instance.IndexProviderCollection["ExternalIndexer"].GatheringNodeData += OnGatheringNodeData;
            PreRenderViewActionFilterAttribute.ActionExecuted += PreRenderViewActionFilterAttribute_ActionExecuted;
            var logger = LoggerResolver.Current.Logger;
            var dbContext = ApplicationContext.Current.DatabaseContext;
            var helper = new DatabaseSchemaHelper(dbContext.Database, logger, dbContext.SqlSyntax);

            if (!helper.TableExist("PageViews"))
            {
                helper.CreateTable<PageView>(false);
            }
        }

        #region Events

        private void Content_New(IContentService sender, NewEventArgs<IContent> e)
        {
            //Set current date.
            if (e.Entity.HasProperty("currentDate"))
            {
                e.Entity.SetValue("currentDate", DateTime.Today);
            }
            if (e.Entity.HasProperty("createTime"))
            {
                e.Entity.SetValue("createTime", DateTime.Today);
            }
        }

        protected void ContentService_Saving(IContentService sender, SaveEventArgs<IContent> e)
        {
            foreach (var entity in e.SavedEntities)
            {
                //Set node name if the properties are empty. We can't do this in the Content_New event because we don't have the node name yet.
                if (entity.HasProperty("menuTitle") && string.IsNullOrWhiteSpace(entity.GetValue<string>("menuTitle")))
                {
                    entity.SetValue("menuTitle", entity.Name);
                }
                if (entity.HasProperty("pageTitle") && string.IsNullOrWhiteSpace(entity.GetValue<string>("pageTitle")))
                {
                    entity.SetValue("pageTitle", entity.Name);
                }
            }
        }

        private void ContentService_Saved(IContentService sender, SaveEventArgs<IContent> e)
        {
        }

        private void Content_Published(IPublishingStrategy sender, PublishEventArgs<IContent> e)
        {
            ClearCache();
        }

        private void Content_Deleted(IContentService sender, DeleteEventArgs<IContent> e)
        {
            ClearCache();
        }

        private void Content_Trashed(IContentService sender, MoveEventArgs<IContent> e)
        {
            ClearCache();
        }

        private void Content_Moved(IContentService sender, MoveEventArgs<IContent> e)
        {
            ClearCache();
        }

        private void Content_Unpublished(IPublishingStrategy sender, PublishEventArgs<IContent> e)
        {
            ClearCache();
        }

        private void Media_Saved(IMediaService sender, SaveEventArgs<IMedia> e)
        {
        }

        private void Media_Saving(IMediaService sender, SaveEventArgs<IMedia> e)
        {
            //Set up a limit to how much media can be uploaded
            var uploadLimit = int.Parse(WebConfigurationManager.AppSettings["UploadLimit"]);
            uploadLimit = uploadLimit > 0 ? uploadLimit : 1024;
            var dir = new DirectoryInfo($@"{AppDomain.CurrentDomain.BaseDirectory}media");
            if (dir.Exists)
            {
                var foldersize = DirSize(dir);
                //In MB's
                foldersize = foldersize / 1000000;
                //if more than set upload limit
                if (foldersize > uploadLimit)
                {
                    e.CancelOperation(new EventMessage("Oops", $"You have gone over you alloted size limit of {foldersize - uploadLimit}, please remove some unused media to free some space and try again", EventMessageType.Warning));
                }
            }
        }

        private void OnGatheringNodeData(object sender, IndexingNodeDataEventArgs e)
        {
            // Create searchable path
            if (e.Fields.ContainsKey("path"))
            {
                e.Fields["searchPath"] = e.Fields["path"].Replace(',', ' ');
            }

            // Lowercase all the fields for case insensitive searching
            var keys = e.Fields.Keys.ToList();
            foreach (var key in keys)
            {
                e.Fields[key] = HttpUtility.HtmlDecode(e.Fields[key].ToLower(CultureInfo.InvariantCulture));
            }

            // Extract the filename from media items
            if (e.Fields.ContainsKey("umbracoFile"))
            {
                e.Fields["umbracoFileName"] = Path.GetFileName(e.Fields["umbracoFile"]);
            }

            // Stuff all the fields into a single field for easier searching
            var combinedFields = new StringBuilder();
            foreach (var keyValuePair in e.Fields)
            {
                combinedFields.AppendLine(keyValuePair.Value);
            }
            e.Fields.Add("contents", combinedFields.ToString());
        }

        protected void PreRenderViewActionFilterAttribute_ActionExecuted(object sender, ActionExecutedEventArgs e)
        {
            //In this event it's possible to modify the model that will go the view.
            //If we use a package that has it's own route hijacking (for example Articulate) we can still give it our own master model here.
        }

        #endregion Events

        #region Other

        private void ClearCache()
        {
            try
            {
                //Clear all output cache so everything is refreshed.
                cacheManager.RemoveItems();
                HttpContext.Current.Cache.Remove("CachedBlogPostNodes");
            }
            catch (Exception ex)
            {
                LogHelper.Error<UmbracoEvents>(string.Format("Exception: {0} - StackTrace: {1}", ex.Message, ex.StackTrace), ex);
            }
        }

        public long DirSize(DirectoryInfo d)
        {
            long size = 0;
            // Add file sizes.
            FileInfo[] fis = d.GetFiles();
            foreach (FileInfo fi in fis)
            {
                size += fi.Length;
            }
            // Add subdirectory sizes.
            DirectoryInfo[] dis = d.GetDirectories();
            foreach (DirectoryInfo di in dis)
            {
                size += DirSize(di);
            }
            return size;
        }

        #endregion Other
    }
}