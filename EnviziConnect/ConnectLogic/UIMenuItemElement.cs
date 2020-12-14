using System;
using System.Collections.Generic;
using System.Text;

namespace ConnectLogic
{
    public class UIMenuItemElement : ElementEnumeration
    {
        private const string SubMenuXPath = "/html/body/div{0}/div/ul/li/table/tbody/tr/td{2}/div{1}";

        public static readonly UIMenuItemElement PersonalLogoutItem = new UIMenuItemElement(0, "[1];[3]", "Log Out");
        public static readonly UIMenuItemElement OrganizationGroups = new UIMenuItemElement(1, "[2];[2];[1]", "Groups");
        public static readonly UIMenuItemElement OrganizationLocations = new UIMenuItemElement(2, "[2];[3];[1]", "Locations");
        public static readonly UIMenuItemElement OrganizationAccounts = new UIMenuItemElement(3, "[2];[4];[1]", "Accounts");
        public static readonly UIMenuItemElement OrganizationMeters = new UIMenuItemElement(4, "[2];[5];[1]", "Meters");
        public static readonly UIMenuItemElement SelectOrganizationGeneral = new UIMenuItemElement(5, "//*[@id='listitem{0}innerListBoxenvTopSelectorCmd']", "Organization");

        private UIMenuItemElement() { }
        private UIMenuItemElement(int value, string xPath, string displayName) : base(value, xPath, displayName) { }

        public string XPathFormatted
        {
            get
            {
                string[] indexList = base.XPath.Split(';');
                return string.Format(SubMenuXPath, indexList[0], indexList[1], (indexList[2].Contains('[') ? indexList[2] : string.Empty));
            }
        }

        public string XPathFormattedCounter(int index)
        {
            string[] indexList = base.XPath.Split(';');
            return string.Format(indexList[0], index);
        }
    }
}
