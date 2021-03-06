﻿using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace Atata.KendoUI.Tests
{
    [TestFixture]
    public abstract class UITestFixture
    {
        public const string BaseUrl = "http://localhost:56828/";

        [SetUp]
        public void SetUp()
        {
            AtataContext.Configure()
                .UseChrome()
                    .WithArguments(GetChromeArguments())
                .UseBaseUrl(BaseUrl)
                .UseCulture("en-US")
                .UseAllNUnitFeatures()
                .AddNLogLogging()
                .AddScreenshotFileSaving()
                .Build();

            OnSetUp();
        }

        private static IEnumerable<string> GetChromeArguments()
        {
            yield return "start-maximized";
            yield return "disable-extensions";

            bool headless = TestContext.Parameters.Get("headless", false);

            if (headless)
                yield return "headless";
        }

        protected virtual void OnSetUp()
        {
        }

        [TearDown]
        public void TearDown()
        {
            AtataContext.Current.CleanUp();
        }

        protected static SnippetPage GoToSnippetPage(KendoLibrary library)
        {
            string componentName = RetrieveComponentNameFromTestName();

            return GoToSnippetPage(library, componentName);
        }

        protected static SnippetPage GoToSnippetPage(KendoLibrary library, string componentName)
        {
            string url = ResolveSnippetPageUrl(library, componentName);
            return GoToSnippetPage(url);
        }

        protected static SnippetPage GoToSnippetPage(string url)
        {
            var page = Go.To<SnippetPage>(url: url);

            if (url.Contains(".stackblitz.io"))
                page.WaitAndClickRunButton();

            return page;
        }

        protected static SnippetPage GoToAngularDemoPage(string componentName)
        {
            string url = $"https://www.telerik.com/kendo-angular-ui/components/{componentName.ToLowerInvariant()}";
            return GoToSnippetPage(url);
        }

        private static string RetrieveComponentNameFromTestName()
        {
            string componentName = TestContext.CurrentContext.Test.MethodName;
            string[] prefixOptionsToRemove =
            {
                "VueKendo",
                "NgKendo",
                "ReactKendo",
                "Kendo"
            };

            string prefixToRemove = prefixOptionsToRemove.FirstOrDefault(prefix => componentName.StartsWith(prefix));

            return prefixToRemove != null
                ? componentName.Remove(0, prefixToRemove.Length)
                : componentName;
        }

        private static string ResolveSnippetPageUrl(KendoLibrary library, string componentName)
        {
            return library switch
            {
                KendoLibrary.JQuery => componentName.ToLowerInvariant(),
                KendoLibrary.AspNetMvc => $"https://demos.telerik.com/aspnet-mvc/{componentName.ToLowerInvariant()}",
                KendoLibrary.AspNetCore => $"https://demos.telerik.com/aspnet-core/{componentName.ToLowerInvariant()}",
                _ => $"https://atata-kendoui-{library.ToString().ToLowerInvariant()}-{componentName.ToLowerInvariant()}.stackblitz.io",
            };
        }
    }
}
