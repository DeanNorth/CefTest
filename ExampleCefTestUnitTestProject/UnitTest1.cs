using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using Xilium.CefGlue.WPF;
using Xilium.CefGlue;
using System.Runtime.InteropServices;
using CefTest;
using CefTest.Helpers;

namespace ExampleAutomatedMSTests
{
    [TestClass]
    public class UnitTest1
    {
        protected static AutoCefBrowser browser;

        [ClassInitialize]
        public static void Initialize(TestContext context)
        {
            // Create a single browser to be used by all tests in this class
            browser = new AutoCefBrowser();
        }

        [TestInitialize]
        public void TestInit()
        {
            // Delete the cookies so we arent still using the session from the previous test
            browser.DeleteCookies();
        }

        [TestMethod]
        // These methods use jQuery, so the page you are testing must have jquery already loaded.
        public void TestMethod1()
        {
            string result = null;
            
            // Load the initial page
            browser.NavigateTo("http://stackoverflow.com/search");
            // Set the value of the search textbox
            browser.SetValue("#bigsearch input", "rick astley");
            // Click the search button
            browser.Click("#bigsearch input[type=submit]");
            // Clicking the search button causes the page to load, so wait for that to happen
            browser.WaitForPageLoad();

            // Now we are on the results page, lets click the first result
            browser.Click(".result-link:first-child a");
            // Clicking a result causes a the page to load, so wait again
            browser.WaitForPageLoad();

            // Get the text from the article we are on
            result = browser.GetText("#question-header h1 a");

            // Did we find the article we were after?
            Assert.AreEqual("Why can't I link to Rick Astley in iOS?", result.Trim());
        }

        [TestMethod]
        // These methods use jQuery, so the page you are testing must have jquery already loaded.
        public void TestMethod2()
        {
            string result = null;

            // Load the initial page
            browser.NavigateTo("http://stackoverflow.com/search");
            // Set the value of the search textbox
            browser.SetValue("#bigsearch input", "How to use CefTest");
            // Click the search button
            browser.Click("#bigsearch input[type=submit]");
            // Clicking the search button causes the page to load, so wait for that to happen
            browser.WaitForPageLoad();

            // Get the text from the article we are on
            result = browser.GetText(".results-header h2");

            // Did we find the article we were after?
            Assert.AreEqual("0 results", result.Trim());
        }
    }
}
