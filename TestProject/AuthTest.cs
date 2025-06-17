using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace TestProject;

public class Tests
{
    private ChromeDriver driver;
    
    [SetUp]
    public void Setup()
    {
        driver = new ChromeDriver();
    }

    [Test]
    public void Test1()
    {
        driver.Navigate().GoToUrl("http://localhost:5110/log-in");
        
        Thread.Sleep(3000);
        
        var login = driver.FindElement(By.Id("email"));
        var pass = driver.FindElement(By.Id("password"));
        
        login.SendKeys("ivan.smirnov@example.com");
        pass.SendKeys("password123");

        driver.FindElement(By.Id("login-button")).Click();
        
        Thread.Sleep(3000);
        
        Assert.IsTrue(driver.Title.Equals("Главная"));
    }
    
    [Test]
    public void Test2()
    {
        driver.Navigate().GoToUrl("http://localhost:5110/log-in");
        
        Thread.Sleep(3000);
        
        driver.FindElement(By.Id("login-button")).Click();
        
        Thread.Sleep(3000);

        var ul = driver.FindElement(By.ClassName("validation-errors"));
        Assert.That(ul, Is.Not.EqualTo(null));
    }

    [TearDown]
    public void TearDown()
    {
        driver.Dispose();
    }
}