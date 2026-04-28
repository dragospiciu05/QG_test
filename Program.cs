using Microsoft.Playwright.NUnit;
using NUnit.Framework;
using Microsoft.Playwright;
using System;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using TestContext = NUnit.Framework.TestContext;

namespace PlayWrightTests
{
    [TestFixture]
    public class Tests : PageTest
    {
        private async Task CreateUserAccount(string name,string password, string email)
        {
            await Page.GotoAsync("https://automationexercise.com");
            await CheckConsent();
            await Expect(Page.Locator("body")).ToBeVisibleAsync();
            await Expect(Page).ToHaveURLAsync("https://automationexercise.com");
            await Page.Locator("a[href='/login']").ClickAsync();
            await Expect(Page.GetByText("New User Signup!")).ToBeVisibleAsync();
            await Page.Locator("[data-qa='signup-name']").FillAsync(name);
            await Page.Locator("[data-qa='signup-email']").FillAsync(email);
            await Page.Locator("[data-qa='signup-button']").ClickAsync();
            await Expect(Page.GetByText("ENTER ACCOUNT INFORMATION")).ToBeVisibleAsync();
            await Page.Locator("#uniform-id_gender1").CheckAsync();
            await Page.Locator("[data-qa='password']").FillAsync(password);
            await Page.Locator("#days").SelectOptionAsync("29");
            await Page.Locator("#months").SelectOptionAsync("5");
            await Page.Locator("#years").SelectOptionAsync("2005");
            await Page.Locator("#newsletter").CheckAsync();
            await Page.Locator("#optin").CheckAsync();
            await Page.Locator("[data-qa='first_name']").FillAsync("Test_fname");
            await Page.Locator("[data-qa='last_name']").FillAsync("Test_lname");
            await Page.Locator("[data-qa='company']").FillAsync("Test_company");
            await Page.Locator("[data-qa='address']").FillAsync("Test_adress");
            await Page.Locator("[data-qa='address2']").FillAsync("Test_adress2");
            await Page.Locator("[data-qa='country']").SelectOptionAsync("United States");
            await Page.Locator("[data-qa='state']").FillAsync("Test_state");
            await Page.Locator("[data-qa='city']").FillAsync("Test_city");
            await Page.Locator("[data-qa='zipcode']").FillAsync("Test_zipcode");
            await Page.Locator("[data-qa='mobile_number']").FillAsync("0711111111");
            await Page.Locator("[data-qa='create-account']").ClickAsync();
            await Expect(Page.GetByText("ACCOUNT CREATED!")).ToBeVisibleAsync();
            await Page.Locator("[data-qa='continue-button']").ClickAsync();
            await Expect(Page.GetByText($"Logged in as {name}")).ToBeVisibleAsync();
            
        }
        private async Task LoginUserAccount(string email,string password,string name)
        {
            await Page.GotoAsync("https://automationexercise.com");
            await Expect(Page.Locator("body")).ToBeVisibleAsync();
            await Expect(Page).ToHaveURLAsync("https://automationexercise.com");
            await Page.Locator("a[href='/login']").ClickAsync();
            await Expect(Page.GetByText("Login to your account")).ToBeVisibleAsync();
            await Page.Locator("[data-qa='login-email']").FillAsync(email);
            await Page.Locator("[data-qa='login-password']").FillAsync(password);
            await Page.Locator("[data-qa='login-button']").ClickAsync();
            await Expect(Page.GetByText($"Logged in as {name}")).ToBeVisibleAsync();
        }
        private async Task CheckConsent()
        {
             if(await Page.GetByRole(AriaRole.Button,new() { Name = "Consent" }).IsVisibleAsync())
            {
                await Page.GetByRole(AriaRole.Button,new() {Name="Consent"}).ClickAsync();
            }
        }
        [SetUp]
        public async Task Setup()
        {
            await Page.RouteAsync("**/*", async route =>
            {
                if (route.Request.Url.Contains("googleads") || route.Request.Url.Contains("doubleclick") || route.Request.Url.Contains("quantserve"))
                {
                    await route.AbortAsync(); //Cancel ADS
                }
                else
                {
                    await route.ContinueAsync();
                }
            });

        }
        [Test]
        public async Task register_user()  // Test Case 1: Register User
        {   
            await CreateUserAccount("username_test","password_test",$"test_{DateTime.Now.Ticks}@test.com");
            await Page.Locator("a[href='/delete_account']").ClickAsync();
            await Expect(Page.GetByText("ACCOUNT DELETED!")).ToBeVisibleAsync();
        }
        [Test]
        public async Task login_correct() //Test Case 2: Login User with correct email and password
        {
            string name = "username_test", password="password_test",email=$"test_{DateTime.Now.Ticks}@test.com";
            await CreateUserAccount(name,password,email);
            await Page.Locator("a[href='/logout']").ClickAsync();
            await LoginUserAccount(email,password,name);
            await Page.Locator("a[href='/delete_account']").ClickAsync();
            await Expect(Page.GetByText("ACCOUNT DELETED!")).ToBeVisibleAsync();


        }
        [Test]
        public async Task login_incorrect() // Test Case 3: Login User with incorrect email and password
        {
            await Page.GotoAsync("https://automationexercise.com");
            await CheckConsent();
            await Expect(Page).ToHaveURLAsync("https://automationexercise.com");
            await Expect(Page.Locator("body")).ToBeVisibleAsync();
            await Page.Locator("a[href='/login']").ClickAsync();
            await  Expect(Page.GetByText("Login to your account")).ToBeVisibleAsync();
            await Page.Locator("[data-qa='login-email']").FillAsync($"test_{DateTime.Now.Ticks}@test.com");
            await Page.Locator("[data-qa='login-password']").FillAsync("password_test_QA123");
            await Page.Locator("[data-qa='login-button']").ClickAsync();
            await Expect(Page.GetByText("Your email or password is incorrect")).ToBeVisibleAsync();
        }
        [Test]
        public async Task register_existing_email() // Test Case 5: Register User with existing email
        {
            string name = "username_test", password="password_test",email=$"test_{DateTime.Now.Ticks}@test.com";
            await CreateUserAccount(name,password,email);
            await Page.Locator("a[href='/logout']").ClickAsync();
            await Page.GotoAsync("https://automationexercise.com/");
            await Page.Locator("a[href='/login']").ClickAsync();
            await Page.Locator("[data-qa='signup-name']").FillAsync(name);
            await Page.Locator("[data-qa='signup-email']").FillAsync(email);
            await Page.Locator("[data-qa='signup-button']").ClickAsync();
            await Expect(Page.GetByText("Email Address already exist!")).ToBeVisibleAsync();
            await LoginUserAccount(email,password,name);
            await Page.Locator("a[href='/delete_account']").ClickAsync();
            await Expect(Page.GetByText("ACCOUNT DELETED!")).ToBeVisibleAsync();



        }
        [Test]
        public async Task remove_products_from_cart() // Test Case 17: Remove Products From Cart
        {
            await Page.GotoAsync("https://automationexercise.com/");
            await CheckConsent();
            await Expect(Page).ToHaveURLAsync("https://automationexercise.com/");
            await Expect(Page.Locator("body")).ToBeVisibleAsync();
            await Page.Locator("a[href='/product_details/1']").ClickAsync();
            await Page.GetByRole(AriaRole.Button,new() { Name = "Add to cart" }).ClickAsync();
            await Page.GetByRole(AriaRole.Link, new() { Name = "View Cart" }).ClickAsync();
            await Expect(Page).ToHaveURLAsync(new Regex("/view_cart"));
            await Page.Locator("a.cart_quantity_delete[data-product-id='1']").ClickAsync();
            await Expect(Page.Locator("[data-product-id='1']")).ToBeHiddenAsync();
        }
        [Test]
        public async Task add_review_on_product() // Test Case 21: Add review on product
        {
            await Page.GotoAsync("https://automationexercise.com/");
            await CheckConsent();
            await Expect(Page).ToHaveURLAsync("https://automationexercise.com/");

            await Page.Locator("a[href='/products']").ClickAsync();
            await Expect(Page.Locator("body")).ToBeVisibleAsync();
            await Expect(Page.GetByText("All Products")).ToBeVisibleAsync();
            await Page.Locator("a[href='/product_details/1']").ClickAsync();
            await Expect(Page.GetByText("Write Your Review")).ToBeVisibleAsync();
            await Page.Locator("#name").FillAsync("test_name");
            await Page.Locator("#email").FillAsync($"test_{DateTime.Now.Ticks}@test.com");
            await Page.Locator("#review").FillAsync("Review test text");
            await Page.Locator("#button-review").ClickAsync();
            await Expect(Page.GetByText("Thank you for your review.")).ToBeVisibleAsync();
        }
        [Test]
        public async Task verify_test_cases_page() // Test Case 7: Verify Test Cases Page
        {
            await Page.GotoAsync("https://automationexercise.com/");
            await CheckConsent();
            await Expect(Page).ToHaveURLAsync("https://automationexercise.com/");
            await Expect(Page.Locator("body")).ToBeVisibleAsync();
            await Page.GetByRole(AriaRole.Button,new() {Name="Test Cases"}).ClickAsync();
            await Expect(Page).ToHaveURLAsync(new Regex("/test_cases"));
        }
        [Test]
        public async Task verify_subscription_in_home_page() // Test Case 10: Verify Subscription in home page
        {
            await Page.GotoAsync("https://automationexercise.com");
            await CheckConsent();
            await Expect(Page).ToHaveURLAsync("https://automationexercise.com/");
            await Expect(Page.Locator("body")).ToBeVisibleAsync();
            await Page.Locator("#footer").ScrollIntoViewIfNeededAsync();
            await Expect(Page.GetByText("SUBSCRIPTION")).ToBeVisibleAsync();
            await Page.Locator("#susbscribe_email").FillAsync($"test_{DateTime.Now.Ticks}@test.com");
            await Page.Locator("#subscribe").ClickAsync();
            await Expect(Page.GetByText("You have been successfully subscribed!")).ToBeVisibleAsync();
        }
        [Test]
        public async Task verify_address_details_in_checkout_page()//Test Case 23: Verify address details in checkout page
        {
            string name="Test_name", email=$"test_{DateTime.Now.Ticks}@test.com", password="testPasswordD123";
            string f_name="Test_fname", l_name="Test_lname",company="Test_company",address1="Test_addres1",address2="Test_addres2";
            string country = "United States", state = "Test_state",city = "Test_city",zipcode="Test_zipcode";
            string mobile_number="0711111111";
            await Page.GotoAsync("https://automationexercise.com");
            await CheckConsent();
            await Expect(Page).ToHaveURLAsync("https://automationexercise.com/");
            await Page.Locator("a[href='/login']").ClickAsync();
            await Expect(Page.GetByText("New User Signup!")).ToBeVisibleAsync();
            await Page.Locator("[data-qa='signup-name']").FillAsync(name);
            await Page.Locator("[data-qa='signup-email']").FillAsync(email);
            await Page.Locator("[data-qa='signup-button']").ClickAsync();
            await Expect(Page.GetByText("ENTER ACCOUNT INFORMATION")).ToBeVisibleAsync();
            await Page.Locator("#uniform-id_gender1").CheckAsync();
            await Page.Locator("[data-qa='password']").FillAsync(password);
            await Page.Locator("#days").SelectOptionAsync("29");
            await Page.Locator("#months").SelectOptionAsync("5");
            await Page.Locator("#years").SelectOptionAsync("2005");
            await Page.Locator("#newsletter").CheckAsync();
            await Page.Locator("#optin").CheckAsync();
            await Page.Locator("[data-qa='first_name']").FillAsync(f_name);
            await Page.Locator("[data-qa='last_name']").FillAsync(l_name);
            await Page.Locator("[data-qa='company']").FillAsync(company);
            await Page.Locator("[data-qa='address']").FillAsync(address1);
            await Page.Locator("[data-qa='address2']").FillAsync(address2);
            await Page.Locator("[data-qa='country']").SelectOptionAsync(country);
            await Page.Locator("[data-qa='state']").FillAsync(state);
            await Page.Locator("[data-qa='city']").FillAsync(city);
            await Page.Locator("[data-qa='zipcode']").FillAsync(zipcode);
            await Page.Locator("[data-qa='mobile_number']").FillAsync(mobile_number);
            await Page.Locator("[data-qa='create-account']").ClickAsync();
            await Expect(Page.GetByText("ACCOUNT CREATED!")).ToBeVisibleAsync();
            await Page.Locator("[data-qa='continue-button']").ClickAsync();
            await Expect(Page.GetByText($"Logged in as {name}")).ToBeVisibleAsync();
            await Page.Locator("a[href='/product_details/1']").ClickAsync();
            await Page.GetByRole(AriaRole.Button,new() { Name = "Add to cart" }).ClickAsync();
            await Expect(Page.GetByText("Added!")).ToBeVisibleAsync();
            await Page.GetByRole(AriaRole.Link, new() { Name = "View Cart" }).ClickAsync();
            await Expect(Page).ToHaveURLAsync(new Regex("/view_cart"));
            await Page.GetByText("Proceed To Checkout").ClickAsync();
            string[] addressType = {"#address_delivery","#address_invoice"};
            foreach(var id in addressType)
            {
                    var address_details = Page.Locator(id);
                    await Expect(address_details.Locator(".address_firstname")).ToContainTextAsync($"{f_name} {l_name}");
                    await Expect(address_details.Locator(".address_address1").Nth(0)).ToContainTextAsync($"{company}");
                    await Expect(address_details.Locator(".address_address1").Nth(1)).ToContainTextAsync($"{address1}");
                    await Expect(address_details.Locator(".address_address1").Nth(2)).ToContainTextAsync($"{address2}");
                    await Expect(address_details.Locator(".address_city")).ToContainTextAsync($"{city} {state} {zipcode}");
                    await Expect(address_details.Locator(".address_country_name")).ToContainTextAsync($"{country}");
                    await Expect(address_details.Locator(".address_phone")).ToContainTextAsync($"{mobile_number}");
            }
            await Page.Locator("a[href='/delete_account']").ClickAsync();
            await Expect(Page.GetByText("ACCOUNT DELETED!")).ToBeVisibleAsync();

        
        }
        [Test]
        public async Task verify_product_quantity_in_cart() // Test Case 13: Verify Product quantity in Cart

        {
            await Page.GotoAsync("https://automationexercise.com");
            await CheckConsent();
            await Expect(Page).ToHaveURLAsync("https://automationexercise.com");
            await Expect(Page.Locator("body")).ToBeVisibleAsync();
            await Page.Locator("a[href='/product_details/1']").ClickAsync();
            await Page.Locator("#quantity").FillAsync("4");
            await Page.GetByRole(AriaRole.Button,new() {Name="Add to cart"}).ClickAsync();
            await Page.GetByRole(AriaRole.Link, new() { Name = "View Cart" }).ClickAsync(new LocatorClickOptions {Force = true});
            await Expect(Page.Locator(".cart_quantity")).ToContainTextAsync("4");
        }
        [Test]
        public async Task test_payment_field_input_validation() //  Additional Testing Scenarios T3
        {
            await Page.GotoAsync("https://automationexercise.com");
            await CheckConsent();
            await Expect(Page).ToHaveURLAsync("https://automationexercise.com");;
            await Expect(Page.Locator("body")).ToBeVisibleAsync();
            await CreateUserAccount("name_test","password_test1D",$"test_{DateTime.Now.Ticks}@test.com");
            await Page.Locator("a[href='/products']").ClickAsync();
            await Page.Locator("a[href='/product_details/1']").ClickAsync();
            await Page.GetByRole(AriaRole.Button,new() {Name="Add to cart"}).ClickAsync();
            await Page.GetByRole(AriaRole.Link,new() {Name="View Cart"}).ClickAsync();
            await Page.GetByText("Proceed To Checkout").ClickAsync();
            await Page.Locator("a[href='/payment']").ClickAsync();
            await Page.Locator("[data-qa='name-on-card']").FillAsync("Test_Name");
            await Page.Locator("[data-qa='card-number']").FillAsync("test_number");
            await Page.Locator("[data-qa='cvc']").FillAsync("test_cvc");
            await Page.Locator("[data-qa='expiry-month']").FillAsync("test_month");
            await Page.Locator("[data-qa='expiry-year']").FillAsync("test_year");
            await Page.Locator("[data-qa='pay-button']").ClickAsync();
            await Expect(Page.GetByText("Congratulations! Your order has been confirmed!")).ToBeVisibleAsync();
            // BUG: The system should prevent payment with alphabetic characters, but it proceeds to confirmation.
            await Page.Locator("a[href='/delete_account']").ClickAsync();
            await Expect(Page.GetByText("ACCOUNT DELETED!")).ToBeVisibleAsync();
        }
        [Test]
        public async Task partial_keyword_search() // Additional Testing Scenarios T4
        {
            await Page.GotoAsync("https://automationexercise.com");
            await CheckConsent();
            await Expect(Page).ToHaveURLAsync("https://automationexercise.com");
            await Page.Locator("a[href='/products']").ClickAsync();
            await Page.Locator("#search_product").FillAsync("Blue");
            await Page.Locator("#submit_search").ClickAsync();
            await Expect(Page.Locator(".features_items")).ToContainTextAsync("Blue");
        }

    }
}