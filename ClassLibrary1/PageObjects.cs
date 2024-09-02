using Newtonsoft.Json;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using SeleniumExtras.PageObjects;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace ClassLibrary1
{
    class PageObjects
    {
        public Actions actions = new Actions(PropertiesCollection.Driver);
        public PageObjects()
        {
            PageFactory.InitElements(PropertiesCollection.Driver, this);

        }

        [FindsBy(How = How.XPath, Using = "*//button[text()='Book this room']")]
        public IWebElement bookThisRoomBtn { get; set; }

        [FindsBy(How = How.XPath, Using = "*//input[@id='name']")]
        public IWebElement nameTxtBox { get; set; }

        [FindsBy(How = How.XPath, Using = "*//button[text()='Next']")]
        public IWebElement nextBtn { get; set; }

        [FindsBy(How = How.XPath, Using = "*//div[@class='rbc-month-view']/child::div[@class='rbc-month-row']")]
        public IList<IWebElement> rowList { get; set; }

        [FindsBy(How = How.XPath, Using = "*//div[@class='rbc-row-content']//div[@class='rbc-date-cell rbc-now rbc-current']/following-sibling::div[@class='rbc-date-cell']//button")]
        public IList<IWebElement> cells { get; set; }

        [FindsBy(How = How.XPath, Using = "*//input[@name='firstname']")]
        public IWebElement firstNameTxt { get; set; }

        [FindsBy(How = How.XPath, Using = "*//input[@name='lastname']")]
        public IWebElement lastNameTxt { get; set; }

        [FindsBy(How = How.XPath, Using = "*//input[@name='email']")]
        public IWebElement emailTxt { get; set; }

        [FindsBy(How = How.XPath, Using = "*//input[@name='phone']")]
        public IWebElement phoneTxt { get; set; }

        [FindsBy(How = How.XPath, Using = "*//button[text()='Book']")]
        public IWebElement bookBtn { get; set; }

        [FindsBy(How = How.XPath, Using = "*//h3[text()='Booking Successful!']")]
        public IWebElement sucessMsg { get; set; }

        [FindsBy(How = How.XPath, Using = "*//button[text()='Close']")]
        public IWebElement closeBtn { get; set; }

        public void ClickOnBookThisRoom()
        {
            actions.MoveToElement(bookThisRoomBtn);
            actions.Perform();
            Log.Debug("Scroll to Book This Room Button");

            bookThisRoomBtn.Click();
            Log.Debug("Clicked on Book This Room Button");
        }
        public void NaviagateToCalender()
        {
            actions.MoveToElement(nameTxtBox);
            actions.Perform();
            Log.Debug("Scroll to Calender");
        }
        public void PerformBookingOperation()
        {
            bool bookingStatus = SelectValidSlotInCalender(true);
            if (!bookingStatus)
            {
                nextBtn.Click();
                Log.Debug("Clicked on next Button");
                Thread.Sleep(4000);
                SelectValidSlotInCalender(false);
            }
        }
        private bool SelectValidSlotInCalender(bool CheckToday)
        {
            int rowCount = 0;
            foreach (var row in rowList)
            {
                rowCount++;
                IList<IWebElement> blocks = row.FindElements(By.XPath("./child::div[@class='rbc-row-bg']/child::div"));
                int blocksCount = 0;
                foreach (var block in blocks)
                {
                    blocksCount++;
                    if (CheckToday)
                    {
                        if (block.GetAttribute("className").Contains("today"))
                        {
                            IList<IWebElement> issue = row.FindElements(By.XPath("./child::div[@class='rbc-row-content']/child::*"));
                            if (issue.Count > 1)
                            {
                                // there is issue , jump to next row
                                CheckToday = false;
                                break;
                            }
                            else
                            {
                                // check for active row count
                                IList<IWebElement> activeBlock = null;
                                if (blocksCount != blocks.Count)
                                {
                                    activeBlock = block.FindElements(By.XPath("./following-sibling::div[@class='rbc-day-bg']")); // error can occur if no sibling present
                                }
                                if (activeBlock == null)
                                    break;
                                else if (activeBlock.Count >= 3)
                                {
                                    // select the slots
                                    Actions action = new Actions(PropertiesCollection.Driver);//                                   
                                    action.ClickAndHold(block).DragAndDrop(cells[0], cells[2]).Build().Perform();
                                    Log.Debug("Selected Two Night(three day) Slot");
                                    return true;
                                }
                                else
                                    break;
                            }
                        }
                    }
                    else
                    {
                        if (block.GetAttribute("className").Contains("off-range-bg"))
                        {
                            continue;
                        }
                        IList<IWebElement> issue = row.FindElements(By.XPath("./child::div[@class='rbc-row-content']/child::*"));
                        if (issue.Count > 1)
                            break;
                        else
                        {
                            // check for active blocks
                            IList<IWebElement> activeBlock = null;
                            if (blocksCount != blocks.Count)
                            {
                                activeBlock = block.FindElements(By.XPath("./following-sibling::div[@class='rbc-day-bg']"));
                            }
                            if (activeBlock == null)
                                break;
                            if (activeBlock.Count >= 3)
                            {
                                // select the slots
                                Actions action = new Actions(PropertiesCollection.Driver);
                                IWebElement currentCell = PropertiesCollection.Driver.FindElement(By.XPath($"((*//div[@class='rbc-month-view']/child::div[@class='rbc-month-row'])[{rowCount}]//div[@class='rbc-row-content']//div[@class='rbc-row ']//child::div)[{blocksCount}]"));
                                IList<IWebElement> cellChild = currentCell.FindElements(By.XPath($"./following-sibling::div/button"));
                                action.ClickAndHold(block).DragAndDrop(currentCell, cellChild[1]).Build().Perform();
                                Log.Debug("Selected Two Night(three day) Slot");
                                return true;
                            }
                            else
                                break;
                        }
                    }
                }
            }
            return false;
        }

        public void EnterCustomerDetails()
        {
            firstNameTxt.SendKeys(PropertiesCollection.Customer.FirstName);
            Log.Debug($"Enter {PropertiesCollection.Customer.FirstName} in FirstName textbox");

            lastNameTxt.SendKeys(PropertiesCollection.Customer.LastName);
            Log.Debug($"Enter {PropertiesCollection.Customer.LastName} in LastName textbox");

            emailTxt.SendKeys(PropertiesCollection.Customer.Email);
            Log.Debug($"Enter {PropertiesCollection.Customer.Email} in Email textbox");

            phoneTxt.SendKeys(PropertiesCollection.Customer.Phone);
            Log.Debug($"Enter {PropertiesCollection.Customer.Phone} in Phone textbox");

            Thread.Sleep(4000);
            bookBtn.Click();
            Log.Debug("Clicked on Book button");
            Thread.Sleep(4000);

        }
        public void VerifySuccessMessage()
        {
            if (!sucessMsg.Displayed)
                Assert.Fail("Booking Successful Message is not Displayed");
            else
            {
                Log.Debug("Booking Successful Message is Displayed");
                closeBtn.Click();
                Log.Debug("Clicked on Close Button");
            }
        }

        public void ReadJsonData()
        {
            Log.Debug("Started Reading Json File");

            string jsonName = "CustomerDetails.json";
            var jsonPath = Path.Combine(Directory.GetParent(System.Reflection.Assembly.GetExecutingAssembly().Location).FullName, $"{jsonName}");

            try
            {
                string jsonString = File.ReadAllText(jsonPath);
                var json = File.ReadAllText(jsonPath);
                PropertiesCollection.Customer = JsonConvert.DeserializeObject<Customer>(json);
            }
            catch (Exception e)
            {
                var msg = $"An error occurred while reading json file : {e.Message}";
                throw new Exception(msg);
            }

            Log.Debug("Finished Reading Json File");
        }
    }
}
