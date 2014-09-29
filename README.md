CefTest
=======

CefTest is a Web Browser unit testing framework which uses Chrome as its engine via the [Chromium Embedded Framework](https://code.google.com/p/chromiumembedded/) (CEF) and the .Net bindings [CEFGlue](http://xilium.bitbucket.org/cefglue/)

When the unit tests are run normally, the browser is headless (has no UI) but when you run a unit test with the dubugger attached, the browser is shown so you can more easilly debug your tests.

Currently most helper methods require jQuery to be on the site you are testing.

Here is an example test.

```csharp
[TestMethod]
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
```
