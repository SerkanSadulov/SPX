using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using System.Threading;

namespace SPXTests
{
    public class Tests
    {
        private IWebDriver webDriver;

        [SetUp]
        public void Setup()
        {
            webDriver = new ChromeDriver();
            webDriver.Manage().Window.Maximize();
        }

        [TearDown]
        public void TearDown()
        {
            if (webDriver != null)
            {
                webDriver.Quit();
                webDriver.Dispose();
                webDriver = null;
            }
        }

        private void PerformLogin(string usernameInput, string passwordInput)
        {
            webDriver.Navigate().GoToUrl("https://localhost:7077/Home/LogIn");

            IWebElement username = webDriver.FindElement(By.Name("username"));
            IWebElement password = webDriver.FindElement(By.Name("password"));
            IWebElement loginBtn = webDriver.FindElement(By.XPath("/html/body/div/div/section/div/div/div/div/div/div[2]/div/form/div[4]/button"));

            username.SendKeys(usernameInput);
            password.SendKeys(passwordInput);
            loginBtn.Click();

            Thread.Sleep(1500);
        }

        [Test]
        public void LoginTest()
        {
            PerformLogin("testprofileselenium2", "test123test123");

            var loggedInDataCookie = webDriver.Manage().Cookies.GetCookieNamed("LoggedInData");

            Assert.IsNotNull(loggedInDataCookie, "Login cookie 'LoggedInData' not found.");

            var loggedInData = JObject.Parse(loggedInDataCookie.Value);

            Assert.AreEqual("a34d7fac-9312-4197-acef-e402613e08c8", loggedInData["userId"].ToString());
            Assert.AreEqual("TestProfileSelenium2", loggedInData["username"].ToString());
            Assert.AreEqual("TestProfileSelenium2@SPX.com", loggedInData["email"].ToString());
            Assert.AreEqual("Provider", loggedInData["userType"].ToString());
            Assert.AreEqual("", loggedInData["profilePicture"].ToString());
            Assert.AreEqual("0878925637", loggedInData["phoneNumber"].ToString());
        }

        [Test]
        public void EditProfeTest()
        {
            PerformLogin("TestProfileSelenium2", "test123test123");

            IWebElement editBtn = webDriver.FindElement(By.Id("editButton"));
            editBtn.Click();

            IWebElement passwordFields = webDriver.FindElement(By.Id("password"));
            passwordFields.SendKeys("test123test123");

            IWebElement saveBtn = webDriver.FindElement(By.Id("saveButton"));

            saveBtn.Click();

            string currentURL = webDriver.Url;
            Assert.AreEqual("https://localhost:7077/Home/Profile", currentURL);
        }

        [Test]
        public void LoginLogoutTest()
        {
            PerformLogin("TestProfileSelenium2", "test123test123");

            var loggedInDataCookie = webDriver.Manage().Cookies.GetCookieNamed("LoggedInData");

            Assert.IsNotNull(loggedInDataCookie, "Login cookie 'LoggedInData' not found.");

            var loggedInData = JObject.Parse(loggedInDataCookie.Value);

            Assert.AreEqual("a34d7fac-9312-4197-acef-e402613e08c8", loggedInData["userId"].ToString());
            Assert.AreEqual("TestProfileSelenium2", loggedInData["username"].ToString());
            Assert.AreEqual("TestProfileSelenium2@SPX.com", loggedInData["email"].ToString());
            Assert.AreEqual("Provider", loggedInData["userType"].ToString());
            Assert.AreEqual("", loggedInData["profilePicture"].ToString());
            Assert.AreEqual("0878925637", loggedInData["phoneNumber"].ToString());

            IWebElement logoutBtn = webDriver.FindElement(By.XPath("/html/body/div/div[1]/div/div[2]/div/div/div[1]/div[1]/div[3]/button[4]"));
            logoutBtn.Click();

            Thread.Sleep(2000);

            var loggedInDataCookieLG = webDriver.Manage().Cookies.GetCookieNamed("LoggedInData");

            Assert.IsNull(loggedInDataCookieLG, "Login cookie 'LoggedInData' not found.");

            Thread.Sleep(1500);
        }

        [Test]
        public void ProfilePageElementsTest()
        {
            PerformLogin("TestProfileSelenium2", "test123test123");

            Assert.IsTrue(webDriver.FindElement(By.XPath("/html/body/div/div[1]/div/div[2]/div/div/div[1]/div[1]/div[3]/button[4]")).Displayed, "Logout button not displayed.");
            Assert.IsTrue(webDriver.FindElement(By.XPath("/html/body/div/div[1]/div/div[2]/div/div/div[1]/div[1]/div[1]/div")).Displayed, "Profile picture not displayed");
        }

        [Test]
        public void LoginWithInvalidCredentialsTest()
        {
            PerformLogin("InvalidUser", "InvalidPassword");

            var logs = webDriver.Manage().Logs.GetLog(LogType.Browser);
            bool errorFound = logs.Any(log => log.Message.Contains("User not found!"));
            Assert.IsTrue(errorFound, "Expected 'User not found!' error not found in console logs.");
        }

        [Test]
        public void DarkModeTest()
        {
                PerformLogin("TestProfileSelenium2", "test123test123");

                Thread.Sleep(1500);

                IWebElement darkModeBtn = webDriver.FindElement(By.XPath("/html/body/nav/div/div[2]/ul/li[7]/button"));

                var theme = webDriver.Manage().Cookies.GetCookieNamed("theme");
                string initialTheme = theme?.Value ?? "light"; 

                if (initialTheme == "dark")
                {
                    darkModeBtn.Click();
                    Thread.Sleep(500); 
                    Assert.AreEqual("light", webDriver.Manage().Cookies.GetCookieNamed("theme").Value);
                }
                else if (initialTheme == "light")
                {
                    darkModeBtn.Click();
                    Thread.Sleep(500); 
                    Assert.AreEqual("dark", webDriver.Manage().Cookies.GetCookieNamed("theme").Value);
                } 
        }

        [Test]
        public void NavigationTest() 
        {
            webDriver.Navigate().GoToUrl("https://localhost:7077/Home/LogIn");

            PerformLogin("TestProfileSelenium2", "test123test123");

            Thread.Sleep(1500);

            IWebElement homeLink = webDriver.FindElement(By.XPath("/html/body/nav/div/a"));
            homeLink.Click();
            Thread.Sleep(1000);
            Assert.AreEqual("SPX - Home", webDriver.Title, "Failed to navigate to Home page.");

            IWebElement profileLink = webDriver.FindElement(By.XPath("/html/body/nav/div/div[2]/ul/li[5]/div/div"));
            profileLink.Click();
            Thread.Sleep(1000);
            Assert.AreEqual("SPX - Profile", webDriver.Title, "Failed to navigate to Profile page.");

            IWebElement categoriesLink = webDriver.FindElement(By.XPath("/html/body/nav/div/div[2]/ul/li[2]/button"));
            categoriesLink.Click();
            Thread.Sleep(1000);
            Assert.AreEqual("SPX - Categories", webDriver.Title, "Failed to navigate to Categories page.");

            IWebElement addServiceLink = webDriver.FindElement(By.XPath("/html/body/nav/div/div[2]/ul/li[4]/button"));
            addServiceLink.Click();
            Thread.Sleep(1000);
            Assert.AreEqual("SPX - Add Listing", webDriver.Title, "Failed to navigate to Add Service page.");
        }

        [Test]
        public void SendMessageTest()
        {
            webDriver.Navigate().GoToUrl("https://localhost:7077/Home/LogIn");

            PerformLogin("TestProfileSelenium2", "test123test123");

            Thread.Sleep(1500);

            IWebElement profileMessageBtn = webDriver.FindElement(By.Id("messagesTabSideBar"));
            profileMessageBtn.Click();
            Thread.Sleep(1500);

            By chatButtonLocator = By.XPath("/html/body/div/div[1]/div/div[2]/div/div/div[2]/table/tbody/tr[1]/td[2]/button");

            if (webDriver.FindElements(chatButtonLocator).Count > 0)
            {
                IWebElement chatButton = webDriver.FindElement(chatButtonLocator);
                chatButton.Click();
                Thread.Sleep(1500);

                IWebElement chatinput = webDriver.FindElement(By.Id("messageInput"));
                IWebElement chatSendButton = webDriver.FindElement(By.Id("sendButton"));

                string testString = "ChatTest2";
                chatinput.SendKeys(testString);
                Thread.Sleep(500);

                chatSendButton.Click();

                Thread.Sleep(2000);

                IWebElement messagBubble = webDriver.FindElement(By.XPath("//*[text()='ChatTest2']"));

                Assert.AreEqual(messagBubble.Text, testString, "The messages dont match");

            }
            else
            {
                Assert.Fail("Chat button not found.");
            }

        }

        [Test]
        public void AddToFavoritesTest()
        {
            webDriver.Navigate().GoToUrl("https://localhost:7077/Home/LogIn");

            PerformLogin("TestProfileSelenium2", "test123test123");

            Thread.Sleep(1500);

            IWebElement categoriesBtn = webDriver.FindElement(By.XPath("/html/body/nav/div/div[2]/ul/li[2]/button"));
            categoriesBtn.Click();
            Thread.Sleep(2500);

            IWebElement categoryBtn = webDriver.FindElement(By.XPath("//*[@id=\"categories-container\"]/div[5]/div"));
            categoryBtn.Click();
            Thread.Sleep(2000);

            IWebElement productCard = webDriver.FindElement(By.XPath("/html/body/div/div[2]/div[2]/div[1]/div"));
            productCard.Click();
            Thread.Sleep(1500);

            IWebElement favoritesButton = webDriver.FindElement(By.Id("favoriteButton"));
            favoritesButton.Click();

        }

        [Test]
        public void AddServiceTest()
        {
            webDriver.Navigate().GoToUrl("https://localhost:7077/Home/LogIn");

            PerformLogin("TestProfileSelenium2", "test123test123");

            Thread.Sleep(1500);

            IWebElement addServiceBtn = webDriver.FindElement(By.XPath("/html/body/nav/div/div[2]/ul/li[4]/button"));
            addServiceBtn.Click();
            Thread.Sleep(1500);

            IWebElement serviceName = webDriver.FindElement(By.Id("serviceName"));
            IWebElement categorySelect = webDriver.FindElement(By.Id("serviceCategory"));
            SelectElement selectCategory = new SelectElement(categorySelect);
            IWebElement price = webDriver.FindElement(By.Id("servicePrice"));
            IWebElement contactPhone = webDriver.FindElement(By.Id("contactPhone"));
            IWebElement location = webDriver.FindElement(By.Id("serviceLocation"));
            IWebElement description = webDriver.FindElement(By.XPath("/html/body/div[1]/div[1]/div/form/div/div[1]/div[3]/div[2]/div[1]/p"));
            IWebElement addService = webDriver.FindElement(By.Id("addService"));

            Thread.Sleep(1000);

            serviceName.SendKeys("TestService");
            selectCategory.SelectByIndex(1);
            price.SendKeys("25");
            contactPhone.SendKeys("0888888888");
            location.SendKeys("Ruse");
            description.SendKeys("DescriptionTest");

            Thread.Sleep(1500);

            addService.Click();

            Thread.Sleep(1500);

            Assert.AreEqual(webDriver.Url, "https://localhost:7077/Home/Profile");
        }

        [Test]
        public void DeleteServiceTest()
        {
            webDriver.Navigate().GoToUrl("https://localhost:7077/Home/LogIn");

            PerformLogin("TestProfileSelenium2", "test123test123");

            Thread.Sleep(1500);

            IWebElement listingsBtn = webDriver.FindElement(By.XPath("/html/body/div/div[1]/div/div[1]/div/ul/li[5]/a"));
            listingsBtn.Click();

            Thread.Sleep(1500);

            IWebElement listing = webDriver.FindElement(By.XPath("/html/body/div/div[1]/div/div[2]/div/div/div[5]/div/table/tbody/tr"));
            listing.Click();

            Thread.Sleep(1500);

            IWebElement deleteListing = webDriver.FindElement(By.XPath("/html/body/div/div[2]/div/div[2]/form/div[3]/button[3]"));
            deleteListing.Click();

            Thread.Sleep(1500);

            IWebElement deleteConfirm = webDriver.FindElement(By.Id("confirmDeleteButton"));
        }
        [Test]
        public void SearchServiceTest()
        {
            webDriver.Navigate().GoToUrl("https://localhost:7077/Home/LogIn");

            PerformLogin("TestProfileSelenium2", "test123test123");

            Thread.Sleep(1500);

            IWebElement searchBar = webDriver.FindElement(By.XPath("/html/body/nav/div/div[1]/div/input"));
            searchBar.SendKeys("Ц");
            Thread.Sleep(300);
            searchBar.SendKeys("в");
            Thread.Sleep(300);
            searchBar.SendKeys("е");
            Thread.Sleep(300);
            searchBar.SendKeys("т");
            Thread.Sleep(300);
            searchBar.SendKeys("а");

            Thread.Sleep(1000);

            IWebElement listing = webDriver.FindElement(By.XPath("/html/body/nav/div/div[1]/div/ul/li[1]"));
            Thread.Sleep(1000);
            listing.Click();

            Assert.AreEqual(webDriver.Url, "https://localhost:7077/Services/Service?serviceID=28bf04c8-85b2-4739-a375-40e64f34f8e5");
        }

        [Test]
        public void MessageSellerTest()
        {
            webDriver.Navigate().GoToUrl("https://localhost:7077/Home/LogIn");

            PerformLogin("TestProfileSelenium2", "test123test123");

            Thread.Sleep(1500);

            IWebElement searchBar = webDriver.FindElement(By.XPath("/html/body/nav/div/div[1]/div/input"));
            searchBar.SendKeys("Ц");
            Thread.Sleep(300);
            searchBar.SendKeys("в");
            Thread.Sleep(300);
            searchBar.SendKeys("е");
            Thread.Sleep(300);
            searchBar.SendKeys("т");
            Thread.Sleep(300);
            searchBar.SendKeys("а");

            Thread.Sleep(1000);

            IWebElement listing = webDriver.FindElement(By.XPath("/html/body/nav/div/div[1]/div/ul/li[1]"));
            Thread.Sleep(1000);
            listing.Click();

            Thread.Sleep(1000);
            IWebElement messageBtn = webDriver.FindElement(By.Id("messageButton"));
            messageBtn.Click();
            Thread.Sleep(1000);

            IWebElement messageInput = webDriver.FindElement(By.Id("messageInput"));
            messageInput.SendKeys("test");

            IWebElement sendBtn = webDriver.FindElement(By.Id("sendButton"));
            sendBtn.Click();
            Thread.Sleep(1000);
        }

        [Test]
        public void OtherServiceByTheSameProviderTest()
        {
            webDriver.Navigate().GoToUrl("https://localhost:7077/Home/LogIn");

            PerformLogin("TestProfileSelenium2", "test123test123");

            Thread.Sleep(1500);

            IWebElement searchBar = webDriver.FindElement(By.XPath("/html/body/nav/div/div[1]/div/input"));
            string searchText = "Цвета";
            foreach (char c in searchText)
            {
                searchBar.SendKeys(c.ToString());
                Thread.Sleep(300);
            }

            Thread.Sleep(1000);

            IWebElement listing = webDriver.FindElement(By.XPath("/html/body/nav/div/div[1]/div/ul/li[1]"));
            Thread.Sleep(1000);
            listing.Click();

            Thread.Sleep(1000);
            IWebElement service = webDriver.FindElement(By.XPath("/html/body/div/section/div/div/div/div[2]/div/div/div[1]/a/div/div/h5"));
            string serviceName = service.Text;
            IWebElement otherServiceBtn = webDriver.FindElement(By.XPath("/html/body/div/section/div/div/div/div[2]/div/div/div[1]"));

            var jsScrollExecute = (IJavaScriptExecutor)webDriver;
            jsScrollExecute.ExecuteScript("arguments[0].scrollIntoView({ behavior: 'smooth', block: 'center' });", otherServiceBtn);

            Thread.Sleep(2000);
            otherServiceBtn.Click();

            Thread.Sleep(2000);

            IWebElement serviceNameAfterClick = webDriver.FindElement(By.Id("serviceTitle"));
            string serviceNameAfter = serviceNameAfterClick.Text;
            Thread.Sleep(1000);

            Assert.AreEqual(serviceName, serviceNameAfter);
        }

        [Test]
        public void HomePageCategorryTests()
        {
            webDriver.Navigate().GoToUrl("https://localhost:7077/Home/LogIn");

            PerformLogin("TestProfileSelenium2", "test123test123");

            Thread.Sleep(1500);

            IWebElement homeBtn = webDriver.FindElement(By.XPath("/html/body/nav/div/a"));
            Thread.Sleep(1000);
            homeBtn.Click();   

            IWebElement categoryName = webDriver.FindElement(By.XPath("/html/body/div/div/section[2]/div/div[1]/div/div/h3"));
            string categoryNameBefore = categoryName.Text;  
            IWebElement categoryBtn = webDriver.FindElement(By.XPath("/html/body/div/div/section[2]/div/div[1]/div/div/a"));
            Thread.Sleep(1000);



            var jsScrollExecute = (IJavaScriptExecutor)webDriver;
            jsScrollExecute.ExecuteScript("arguments[0].scrollIntoView({ behavior: 'smooth', block: 'center' });", categoryBtn);
            Thread.Sleep(1000);

            categoryBtn.Click();
            Thread.Sleep(1500);

            IWebElement categoryNameAfterClick = webDriver.FindElement(By.XPath("/html/body/div/div[2]/h1"));

            string categoryNameAfter = categoryNameAfterClick.Text;

            Assert.AreEqual(categoryNameAfter, categoryNameBefore);   
        }

        [Test]
        public void AccessToProductWithoutLoginTest()
        {
            webDriver.Navigate().GoToUrl("https://localhost:7077/Home/Home");

            Thread.Sleep(1500);

            IWebElement service = webDriver.FindElement(By.XPath("/html/body/div/div/div[2]/div/div/div"));
            Thread.Sleep(1000);

            var jsScrollExecute = (IJavaScriptExecutor)webDriver;
            jsScrollExecute.ExecuteScript("arguments[0].scrollIntoView({ behavior: 'smooth', block: 'center' });", service);
            Thread.Sleep(1000);

            service.Click();
            Thread.Sleep(2500);

            Assert.AreEqual(webDriver.Url, "https://localhost:7077/Home/LogIn");
        }

        [Test]
        public void ViewCategoryListingsWithoutProfile()
        {
            webDriver.Navigate().GoToUrl("https://localhost:7077/Categories/Categories");

            Thread.Sleep(1500);

            IWebElement category = webDriver.FindElement(By.XPath("/html/body/div/div[2]/div[2]/div[3]/div"));
            Thread.Sleep(1000);

            category.Click();
            Thread.Sleep(1500);

            var services = webDriver.FindElements(By.XPath("//div[contains(@class, 'service-card') and contains(@class, 'bg-dark')]"));


            Assert.IsTrue(services.Count > 0, "No services were found.");
        }

        [Test]
        public void EditListingTest()
        {
            webDriver.Navigate().GoToUrl("https://localhost:7077/Home/LogIn");

            PerformLogin("TestProfileSelenium2", "test123test123");

            Thread.Sleep(1500);

            IWebElement listingBtn = webDriver.FindElement(By.XPath("/html/body/div/div[1]/div/div[1]/div/ul/li[5]/a"));
            Thread.Sleep(1000);


            listingBtn.Click();
            Thread.Sleep(1500);

            IWebElement listing = webDriver.FindElement(By.XPath("/html/body/div/div[1]/div/div[2]/div/div/div[5]/div/table/tbody/tr"));
            listing.Click();

            Thread.Sleep(1000);

            IWebElement lisitngName = webDriver.FindElement(By.Id("serviceName"));
            IWebElement lisitngPrice = webDriver.FindElement(By.Id("servicePrice"));
            IWebElement lisitngDescrp = webDriver.FindElement(By.XPath("/html/body/div/div[2]/div/div[2]/form/div[1]/div[2]/div[2]/div[1]/p"));
            IWebElement saveBtn = webDriver.FindElement(By.XPath("/html/body/div/div[2]/div/div[2]/form/div[3]/button[1]"));

            lisitngName.Clear();
            lisitngDescrp.Clear();
            lisitngPrice.Clear();
            lisitngName.SendKeys("TestServiceEdit");
            lisitngPrice.SendKeys("20");
            lisitngDescrp.SendKeys("TestServiceEditDesc");

            Thread.Sleep(1500);
            saveBtn.Click();
            Thread.Sleep(1500);
        }

        [Test]
        public void OrderProductTest()
        {
            webDriver.Navigate().GoToUrl("https://localhost:7077/Home/LogIn");

            PerformLogin("TestProfileSelenium", "test123test123");

            Thread.Sleep(1500);

            IWebElement searchBar = webDriver.FindElement(By.XPath("/html/body/nav/div/div[1]/div/input"));

            string searchText = "testS";
            foreach (char c in searchText)
            {
                searchBar.SendKeys(c.ToString());
                Thread.Sleep(300); 
            }

            Thread.Sleep(1000);

            IWebElement listing = webDriver.FindElement(By.XPath("/html/body/nav/div/div[1]/div/ul/li[1]"));
            Thread.Sleep(1000);
            listing.Click();

            Thread.Sleep(1500);

            IWebElement requestProduct = webDriver.FindElement(By.Id("requestButton"));
            Thread.Sleep(1000);
            requestProduct.Click();
            Thread.Sleep(1000);
        }

        [Test]
        public void ConfirmOrderAsDeliveredTest()
        {
            webDriver.Navigate().GoToUrl("https://localhost:7077/Home/LogIn");

            PerformLogin("TestProfileSelenium2", "test123test123");

            Thread.Sleep(1500);

            IWebElement oerderListSelect = webDriver.FindElement(By.XPath("/html/body/div/div[1]/div/div[2]/div/div/div[1]/div[2]/div/table/tbody/tr/td[5]/select"));
            SelectElement select = new SelectElement(oerderListSelect);
            Thread.Sleep(1500);

            select.SelectByIndex(4);

        }

        [Test]
        public void RateOrderExperienceTest()
        {
            webDriver.Navigate().GoToUrl("https://localhost:7077/Home/LogIn");

            PerformLogin("TestProfileSelenium", "test123test123");

            Thread.Sleep(1500);

            IWebElement historyBtn = webDriver.FindElement(By.XPath("/html/body/div/div[1]/div/div[1]/div/ul/li[4]/a"));
            Thread.Sleep(1000);
            historyBtn.Click();
            Thread.Sleep(1000);

            IWebElement rateBtn = webDriver.FindElement(By.XPath("/html/body/div/div[1]/div/div[2]/div/div/div[4]/div/table/tbody/tr/td[6]/button"));
            rateBtn.Click();
            Thread.Sleep(1000);

            IWebElement stars = webDriver.FindElement(By.XPath("/html/body/div[1]/div[11]/div/div/div[2]/div[1]/span[5]"));
            IWebElement comment = webDriver.FindElement(By.XPath("/html/body/div[1]/div[11]/div/div/div[2]/div[2]/textarea"));
            IWebElement rate = webDriver.FindElement(By.XPath("/html/body/div[1]/div[11]/div/div/div[3]/button[2]"));
            stars.Click();
            comment.SendKeys("rated");
            rate.Click();
            Thread.Sleep(1000);
        }

        [Test]
        public void DeleteFavoriteTest()
        {
            webDriver.Navigate().GoToUrl("https://localhost:7077/Home/LogIn");

            PerformLogin("TestProfileSelenium2", "test123test123");

            Thread.Sleep(1500);

            IWebElement favoritesBtn = webDriver.FindElement(By.XPath("/html/body/div/div[1]/div/div[1]/div/ul/li[3]/a"));
            Thread.Sleep(1000);
            favoritesBtn.Click();
            Thread.Sleep(1000);

            IWebElement deleteBtn = webDriver.FindElement(By.XPath("/html/body/div/div[1]/div/div[2]/div/div/div[3]/div/table/tbody/tr[1]/td[3]/button"));
            deleteBtn.Click();

            Thread.Sleep(1000);
        }

        [Test]
        public void VisiteFavoriteTest()
        {
            webDriver.Navigate().GoToUrl("https://localhost:7077/Home/LogIn");

            PerformLogin("TestProfileSelenium2", "test123test123");

            Thread.Sleep(1500);

            IWebElement favoritesBtn = webDriver.FindElement(By.XPath("/html/body/div/div[1]/div/div[1]/div/ul/li[3]/a"));
            Thread.Sleep(1000);
            favoritesBtn.Click();
            Thread.Sleep(1000);

            IWebElement visitBtn = webDriver.FindElement(By.XPath("/html/body/div/div[1]/div/div[2]/div/div/div[3]/div/table/tbody/tr/td[3]/a"));
            visitBtn.Click();

            Thread.Sleep(1000);
        }

        [Test]
        public void AlertsTest()
        {
            webDriver.Navigate().GoToUrl("https://localhost:7077/Home/LogIn");

            PerformLogin("TestProfileSelenium2", "test123test123");

            Thread.Sleep(1000);
            webDriver.Navigate().GoToUrl("https://localhost:7077/Categories/Categories");
            Thread.Sleep(1000);
            IWebElement bellAlertBtn = webDriver.FindElement(By.Id("notificationBell"));
            bellAlertBtn.Click();

            Thread.Sleep(1000);
            IWebElement message = webDriver.FindElement(By.XPath("/html/body/nav/div/div[2]/ul/li[6]/ul/li[2]/a"));

            Thread.Sleep(1000);
            message.Click();
            Thread.Sleep(1500);

            Assert.AreEqual(webDriver.Url, "https://localhost:7077/Home/Profile#messagesTab");
        }

        [Test]
        public void CategorySearchTest()
        {
            webDriver.Navigate().GoToUrl("https://localhost:7077/Home/LogIn");

            PerformLogin("TestProfileSelenium2", "test123test123");

            Thread.Sleep(1000);
            webDriver.Navigate().GoToUrl("https://localhost:7077/Categories/Categories");
            Thread.Sleep(1000);
            IWebElement bellAlertBtn = webDriver.FindElement(By.Id("notificationBell"));
            bellAlertBtn.Click();

            Thread.Sleep(1000);
            IWebElement searchBar = webDriver.FindElement(By.Id("search-input"));

            Thread.Sleep(1000);
            searchBar.SendKeys("Clothes");
            Thread.Sleep(1500);

            IWebElement category = webDriver.FindElement(By.XPath("/html/body/div/div[2]/div[2]/div/div"));
            category.Click();
            Thread.Sleep(1000);

            Assert.AreEqual(webDriver.Url, "https://localhost:7077/Services/ServicesByCategory?categoryID=03028ec4-b13a-46e3-b2ea-1423cccb5052&categoryName=Clothes");
        }
    }
}
