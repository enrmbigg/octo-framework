using GDev.Umbraco.Test;
using Moq;
using NUnit.Framework;
using System.Web.Mvc;
using Umbraco.Core.Models;
using Umbraco.Web;
using OctoFramework.Logic.Controllers;
using OctoFramework.Logic.Models.ViewModels;
using System.Globalization;
using Umbraco.Web.Models;
using System;

namespace OctoFramework.Tests
{
    [TestFixture]
    public class BlogPostControllerTests
    {
        private ContextMocker _mocker;
        private IPublishedContent _currentPage;
        private UmbracoHelper _helper;

        [SetUp]
        public void Setup()
        {
            // Our Umbraco Context Mock "Mocker" will give us a fake context
            // Instead of having to write our Umbraco.EnsureContext method
            _mocker = new ContextMocker();
            _currentPage = Mock.Of<IPublishedContent>(); ;
            // Create a fake UmbracoHelper that we can determine the values of
            _helper = new UmbracoHelper(_mocker.UmbracoContextMock, _currentPage);
        }

        [Test]
        public void CanInitializeBlogPostController()
        {
            Assert.DoesNotThrow(() => new BlogPostController(_mocker.UmbracoContextMock, _helper));
        }

        [Test]
        public void HasViewModel()
        {
            // Create an instance of the controller, and pass in our fake context and helper
            var controller = new BlogPostController(_mocker.UmbracoContextMock, _helper);
            var result = (ViewResult)controller.Index();
            Assert.IsNotNull(result.ViewData.Model);
            Assert.IsInstanceOf<BlogPostViewModel>(result.ViewData.Model);
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void UserCanSeeCommentBox(bool enabled)
        {
            // Ensure that getPropertyValue for commentsEnabled returns the expected boolean value
            StubProperty("commentsEnabled", enabled);
            // Create a fake UmbracoHelper that we can determine the values of
            _helper = new UmbracoHelper(this._mocker.UmbracoContextMock, this._currentPage);
            // Create an instance of the controller, and pass in our fake context and helper
            var controller = new BlogPostController(_mocker.UmbracoContextMock, _helper);
            var result = (ViewResult)controller.Index();
            var viewModel = (BlogPostViewModel)result.Model;
            Assert.AreEqual(enabled, viewModel.ShowComments);
        }

        [Test]
        public void HasArticlePublishedDate()
        {
            var currentDateTime = DateTime.Today;
            // Ensure that getPropertyValue for commentsEnabled returns the expected boolean value
            Mock.Get(_currentPage).Setup(c => c.CreateDate).Returns(currentDateTime);
            // Create a fake UmbracoHelper that we can determine the values of
            var helper = new UmbracoHelper(this._mocker.UmbracoContextMock, this._currentPage);
            // Create an instance of the controller, and pass in our fake context and helper
            var controller = new BlogPostController(_mocker.UmbracoContextMock, helper);
            var result = (ViewResult)controller.Index();
            var viewModel = (BlogPostViewModel)result.Model;
            Assert.AreEqual(currentDateTime, viewModel.ArticlePublishedDate);
        }

        [Test]
        public void HasBlogTags()
        {
            var currentDateTime = DateTime.Today;
            // Ensure that getPropertyValue for commentsEnabled returns the expected boolean value
            StubProperty("tag", "test,tags");
            // Create a fake UmbracoHelper that we can determine the values of
            var helper = new UmbracoHelper(this._mocker.UmbracoContextMock, this._currentPage);
            // Create an instance of the controller, and pass in our fake context and helper
            var controller = new BlogPostController(_mocker.UmbracoContextMock, helper);
            var result = (ViewResult)controller.Index();
            var viewModel = (BlogPostViewModel)result.Model;
            var array = new string[2] { "test", "tags" };
            Assert.AreEqual(array, viewModel.BlogTags);
        }

        private void StubProperty<T>(string alias, T value)
        {
            var prop = new Mock<IPublishedProperty>();
            prop.Setup(p => p.Value).Returns(value);
            Mock.Get(_currentPage).Setup(c => c.GetProperty(alias, false)).Returns(prop.Object);
        }
    }
}