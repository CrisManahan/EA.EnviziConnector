using System;
using System.Linq;
using System.Threading;
using System.Collections.Generic;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;

namespace ConnectLogic
{
    public class WebDriver
    {
        private const string GridColumnXPath = "//*[@id='row{0}jqxgrid']/div[{1}]";

        RemoteWebDriver _driver = null;

        public WebDriver(RemoteWebDriver driver = null)
        {
            if (driver != null)
                _driver = driver;
            else
                _driver = new ChromeDriver();
        }

        public string Url
        {
            set
            {
                if (_driver != null && !string.IsNullOrEmpty(value))
                {
                    _driver.Url = value;
                    _driver.Manage().Window.Maximize();
                }
            }
        }

        /// <summary>
        /// Get text from corresponding field
        /// </summary>
        /// <param name="field">Web element field</param>
        public string GetText(UIFieldElement field)
        {
            string result = string.Empty;
            try
            {
                var element = _driver.FindElement(By.XPath(field.XPathFormatted));
                result = element.Text;
            }
            catch { }

            return result;
        }

        /// <summary>
        /// Enter text into corresponding field
        /// </summary>
        /// <param name="field">Web element field</param>
        /// <param name="elemValue">Text to be entered</param>
        public void EnterText(UIFieldElement field, string elemValue)
        {
            try
            {
                var element = _driver.FindElement(By.XPath(field.XPathFormatted));
                element.SendKeys(elemValue);

                // allow some time for the page to update
                Thread.Sleep(1000);
            }
            catch { }
        }

        /// <summary>
        /// Enter text into corresponding field inside the capture data form
        /// </summary>
        /// <param name="field">Web element field</param>
        /// <param name="elemValue">Text to be entered</param>
        public void EnterCaptureData(UIFieldElement field, string elemValue)
        {
            try
            {
                var element = _driver.FindElement(By.XPath(field.XPathFormattedContains));

                //check if element value is for a date entry
                DateTime dtTest;
                if (DateTime.TryParse(elemValue, out dtTest))
                {
                    element.SendKeys(Keys.Control + "a");
                    TextCopy.ClipboardService.SetText(elemValue);
                    element.SendKeys(Keys.Control + "v");
                }
                else
                    element.SendKeys(elemValue);

            }
            catch{}
        }

        /// <summary>
        /// Click corresponding object
        /// </summary>
        /// <param name="field">Web element field</param>
        /// <param name="milliSecondsWaitTime">Wait time (milliseconds) after clicking</param>
        public bool ClickObject(UIFieldElement field, int milliSecondsWaitTime = 1000)
        {
            bool clickSuccess = false;
            try
            {
                var element = _driver.FindElement(By.XPath(field.XPathFormatted));
                element.Click();
                clickSuccess = true;

                // allow some time for a page to update
                // TODO: WebDriverWait does not work yet; use Sleep for now
                //WebDriverWait wait = new WebDriverWait(_driver, TimeSpan.FromMilliseconds(milliSecondsWaitTime));
                //wait.Until(ExpectedConditions.VisibilityOfAllElementsLocatedBy(By.XPath(UIFieldElement.BodyContainer.XPath)));
                Thread.Sleep(milliSecondsWaitTime);
            }
            catch { }
            return clickSuccess;
        }

        /// <summary>
        /// Click corresponding object
        /// </summary>
        /// <param name="field">Web element field</param>
        public void ClickVariableIDObject(UIFieldElement field)
        {
            try
            {
                var element = _driver.FindElement(By.XPath(field.XPathFormattedContains));
                element.Click();

                // allow some time for a page to update
                Thread.Sleep(1000);
            }
            catch (Exception ex)
            {
                string test = ex.Message;
            }
        }

        /// <summary>
        /// Click an item that belongs to a menu
        /// </summary>
        /// <param name="menuItem">Web element menu item</param>
        /// <param name="menuText">If set, click the specific menu item having the given text</param>
        /// <param name="milliSecondsWaitTime">Wait time (milliseconds) after clicking</param>
        public void ClickMenuItem(UIMenuItemElement menuItem, string menuText = null, int milliSecondsWaitTime = 2000)
        {
            HandleClickMenuItem(menuItem, menuText, milliSecondsWaitTime);
        }

        /// <summary>
        /// Click an item that belongs to a menu
        /// </summary>
        /// <param name="menuType">Web element menu type</param>
        public void ClickMenuItem(string menuType)
        {
            foreach (UIMenuItemElement menuItem in Enum.GetValues(typeof(UIMenuItemElement)))
            {
                string itemType = menuItem.DisplayName;
                if (menuType.Equals(itemType, StringComparison.OrdinalIgnoreCase))
                {
                    HandleClickMenuItem(menuItem);
                    break;
                }
            }
        }

        /// <summary>
        /// Fetch grid row values corresponding to a specific menu item
        /// </summary>
        /// <param name="menuItem">Web element menu item</param>
        public List<List<string>> GetGridRows(UIMenuItemElement menuItem)
        {
#if SANDBOX
            const int CELL_GROUP_OR_ACCOUNT_INDEX = 3;
#else
            const int CELL_GROUP_OR_ACCOUNT_INDEX = 4;
#endif
            const int CELL_LOCATION_OR_METER_INDEX = 4;
            const int ROW_GROUP_OR_LOCATION_MAX_INDEX = 11;
            const int ROW_ACCOUNT_OR_METER_MAX_INDEX = 9;

            const int CELL_COMPARE_GROUP_OR_LOCATION_INDEX = 0;
            const int CELL_COMPARE_ACCOUNT_OR_METER_INDEX = 1;

            int baseCellIndex = -1;
            int maxGridIndex = -1;
            int compareCellIndex = -1;
            if (menuItem == UIMenuItemElement.OrganizationGroups)
            {
                baseCellIndex = CELL_GROUP_OR_ACCOUNT_INDEX;
                maxGridIndex = ROW_GROUP_OR_LOCATION_MAX_INDEX;
                compareCellIndex = CELL_COMPARE_GROUP_OR_LOCATION_INDEX;
            }
            else if (menuItem == UIMenuItemElement.OrganizationLocations)
            {
                baseCellIndex = CELL_LOCATION_OR_METER_INDEX;
                maxGridIndex = ROW_GROUP_OR_LOCATION_MAX_INDEX;
                compareCellIndex = CELL_COMPARE_GROUP_OR_LOCATION_INDEX;
            }
            else if (menuItem == UIMenuItemElement.OrganizationAccounts)
            {
                baseCellIndex = CELL_GROUP_OR_ACCOUNT_INDEX;
                maxGridIndex = ROW_ACCOUNT_OR_METER_MAX_INDEX;
                compareCellIndex = CELL_COMPARE_ACCOUNT_OR_METER_INDEX;
            }
            else if (menuItem == UIMenuItemElement.OrganizationMeters)
            {
                baseCellIndex = CELL_LOCATION_OR_METER_INDEX;
                maxGridIndex = ROW_ACCOUNT_OR_METER_MAX_INDEX;
                compareCellIndex = CELL_COMPARE_ACCOUNT_OR_METER_INDEX;
            }

            List<List<string>> gridRows = new List<List<string>>();
            if (baseCellIndex == -1)
                return gridRows;

            // Get the total row count as shown in the corresponding web element
            int totalRowCount = 0;
            try
            {
                string rowCountText = _driver.FindElement(By.XPath("//*[@id='grid_countElements']")).Text;
                if (!string.IsNullOrEmpty(rowCountText))
                {
                    string[] tempSplit = rowCountText.Split(' ');
                    totalRowCount = Convert.ToInt32(tempSplit[0]);
                }
            }
            catch { }
            
            int duplicateCount = 0;
            int rowCount = 0;
            while (gridRows.Count < totalRowCount)
            {
                int cellCount = baseCellIndex;
                int rowIndex = rowCount;
                List<string> cells = new List<string>();
                while (true)
                {
                    // Scroll first
                    if (rowIndex >= maxGridIndex)
                    {
                        int rowIndexNew = (rowIndex % maxGridIndex);
                        if (rowIndexNew == 0)
                        {
                            ++rowIndexNew;
                            string previousText = _driver.FindElement(By.XPath(string.Format(GridColumnXPath, (maxGridIndex - 1), baseCellIndex))).Text;
                            string targetText = string.Empty;
                            while (!targetText.Equals(previousText))
                            {
                                ClickObject(UIFieldElement.ScrollBarVerticalUpButton, 0);
                                targetText = _driver.FindElement(By.XPath(string.Format(GridColumnXPath, rowIndexNew, baseCellIndex))).Text;

                                if (!_driver.FindElement(By.XPath(UIFieldElement.ScrollBarVerticalDownArea.XPathFormatted)).Displayed)
                                    break;
                            }
                        }

                        rowIndex = rowIndexNew;
                    }

                    // If corresponding grid cell is found, add to list of cell values
                    bool cellPresent = false;
                    try
                    {
                        var cell = _driver.FindElement(By.XPath(string.Format(GridColumnXPath, rowIndex, cellCount)));
                        cells.Add(cell.Text);
                        ++cellCount;
                        cellPresent = true;
                    }
                    catch { }

                    if (!cellPresent)
                        break;
                }

                if (cells.Count > 0 && cells.All(x => string.IsNullOrEmpty(x)))
                    break;

                // Add to list of grid rows until reaching the end of the grid that contains values
                if (cells.Count > 0 && !gridRows.Exists(x => x[compareCellIndex].Equals(cells[compareCellIndex])) && !string.IsNullOrEmpty(cells[compareCellIndex]))
                    gridRows.Add(cells);
                else if (!_driver.FindElement(By.XPath(UIFieldElement.ScrollBarVerticalDownArea.XPathFormatted)).Displayed)
                {
                    ++duplicateCount;
                    if (duplicateCount >= maxGridIndex)
                        break;
                }

                ++rowCount;
            }

            return gridRows;
        }

        /// <summary>
        /// Closes the instance of the web driver
        /// </summary>
        public void Close()
        {
            try
            {
                _driver.Close();
            }
            catch { }
        }

        /// <summary>
        /// Click an item that belongs to a menu
        /// </summary>
        /// <param name="menuItem">Web element menu item</param>
        /// <param name="searchMenuText">If set, click the specific menu item having the given text</param>
        /// <param name="milliSecondsWaitTime">Wait time (milliseconds) after clicking</param>
        private void HandleClickMenuItem(UIMenuItemElement menuItem, string searchMenuText = null, int milliSecondsWaitTime = 2000)
        {
            // Handles clicking of parent menu to open submenu 
            if (menuItem == UIMenuItemElement.PersonalLogoutItem)
                ClickObject(UIFieldElement.PersonalMenu);
            else if (menuItem == UIMenuItemElement.SelectOrganizationGeneral)
                ClickObject(UIFieldElement.SelectOrganizationMenu);
            else if (menuItem == UIMenuItemElement.OrganizationAccounts || menuItem == UIMenuItemElement.OrganizationGroups ||
                     menuItem == UIMenuItemElement.OrganizationLocations || menuItem == UIMenuItemElement.OrganizationMeters)
                ClickObject(UIFieldElement.ManageOrganizationMenu);

            // Find corresponding submenu element by using the corresponding xpath
            Thread.Sleep(500);
            if (string.IsNullOrEmpty(searchMenuText))
            {
                try
                {
                    var element = _driver.FindElement(By.XPath(menuItem.XPathFormatted));
                    element.Click();
                }
                catch {}
            }
            else
            {
                int menuCounter = 0;
                while (true)
                {
                    try
                    {
                        var element = _driver.FindElement(By.XPath(menuItem.XPathFormattedCounter(menuCounter)));
                        if (element.Text.Contains(searchMenuText, StringComparison.OrdinalIgnoreCase))
                        {
                            element.Click();
                            break;
                        }
                    }
                    catch
                    {
                        break;
                    }

                    ++menuCounter;
                }
            }

            // TODO: WebDriverWait does not work yet; use Sleep for now
            //WebDriverWait wait = new WebDriverWait(_driver, TimeSpan.FromMilliseconds(milliSecondsWaitTime));
            //wait.Until(ExpectedConditions.VisibilityOfAllElementsLocatedBy(By.XPath(UIFieldElement.BodyContainer.XPath)));
            Thread.Sleep(milliSecondsWaitTime);
        }
    }
}
