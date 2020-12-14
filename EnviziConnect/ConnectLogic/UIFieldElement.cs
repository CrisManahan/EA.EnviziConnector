using System;
using System.Collections.Generic;
using System.Text;

namespace ConnectLogic
{
    public class UIFieldElement : ElementEnumeration
    {
        public static readonly UIFieldElement ScrollBarVerticalUpButton = new UIFieldElement(0, "jqxScrollBtnDownverticalScrollBarjqxgrid");
        public static readonly UIFieldElement ScrollBarVerticalDownArea = new UIFieldElement(1, "jqxScrollAreaDownverticalScrollBarjqxgrid");
        public static readonly UIFieldElement ScrollBarHorizontalRightButton = new UIFieldElement(2, "jqxScrollBtnDownhorizontalScrollBarjqxgrid");
        public static readonly UIFieldElement UsernameText = new UIFieldElement(3, "LoginControl_txtUsername");
        public static readonly UIFieldElement PasswordText = new UIFieldElement(4, "LoginControl_txtPassword");
        public static readonly UIFieldElement LoginButton = new UIFieldElement(5, "LoginControl_btnLogin");
        public static readonly UIFieldElement SelectOrganizationMenu = new UIFieldElement(6, "dropdownlistContentenvTopSelectorCmd");
        public static readonly UIFieldElement PersonalMenu = new UIFieldElement(7, "HeaderControl_rptPersonalThreads_ctl00_liPersonalName");
        public static readonly UIFieldElement ManageOrganizationMenu = new UIFieldElement(8, "HeaderControl_rptThreads_ctl02_liName");
        public static readonly UIFieldElement MainHomeMenuButton = new UIFieldElement(9, "HeaderControl_rptThreads_ctl00_liName");
        public static readonly UIFieldElement AccountNumberPageLabel = new UIFieldElement(10, "breadbrumb_leaf_node");
#if SANDBOX
        public static readonly UIFieldElement AccountNumberFilterText = new UIFieldElement(11, "row00jqxgrid;/div[4]/input");
        public static readonly UIFieldElement AccountReferenceFilterText = new UIFieldElement(12, "row00jqxgrid;/div[5]/input");
        public static readonly UIFieldElement AccountNumberGridLink = new UIFieldElement(13, "row0jqxgrid;/div[4]");
#else
        public static readonly UIFieldElement AccountNumberFilterText = new UIFieldElement(11, "row00jqxgrid;/div[5]/input");
        public static readonly UIFieldElement AccountReferenceFilterText = new UIFieldElement(12, "row00jqxgrid;/div[6]/input");
        public static readonly UIFieldElement AccountNumberGridLink = new UIFieldElement(13, "row0jqxgrid;/div[5]");
#endif
        public static readonly UIFieldElement BodyContainer = new UIFieldElement(14, "//*[@id='pagewrap']/table/tbody/tr/td[1]/div");
        public static readonly UIFieldElement ActionsButton = new UIFieldElement(15, "dropdownlistContentctl04_SubHeaderControl_subHeaderOptions");
        public static readonly UIFieldElement CaptureDataButton = new UIFieldElement(16, "listitem0innerListBoxctl04_SubHeaderControl_subHeaderOptions");
        public static readonly UIFieldElement CaptureDateStartPeriodText = new UIFieldElement(17, "start_period;/div/input");
        public static readonly UIFieldElement CaptureDataEndPeriodText = new UIFieldElement(18, "end_period;/div/input");
        public static readonly UIFieldElement CaptureDataReferenceText = new UIFieldElement(19, "reference;/div/textarea");
        public static readonly UIFieldElement CaptureDataTotalUnitskWhText = new UIFieldElement(20, "c_1;/div/input");
        public static readonly UIFieldElement CaptureDataPeakUnitskWhText = new UIFieldElement(21, "c_4;/div/input");
        public static readonly UIFieldElement CaptureDataOffPeakUnitskWhText = new UIFieldElement(22, "c_5;/div/input");
        public static readonly UIFieldElement CaptureDataTotalCostText = new UIFieldElement(23, "c_20;/div/input");
        public static readonly UIFieldElement CaptureDataInvoiceFileText = new UIFieldElement(24, "image_content_type_url_attachment_type_id;/input");
        public static readonly UIFieldElement CaptureDataSaveButton = new UIFieldElement(25, "envEditActions;/input[2]");

        private UIFieldElement() { }
        private UIFieldElement(int value, string xPath) : base(value, xPath) { }

        public string XPathFormatted
        {
            get 
            {
                string[] xPaths = base.XPath.Split(';');
                return string.Format("//*[@id='{0}']{1}", xPaths[0], (xPaths.Length > 1 ? xPaths[1] : string.Empty));
            }
        }

        public string XPathFormattedContains
        {
            get
            {
                string[] xPaths = base.XPath.Split(';');
                return string.Format("//*[contains(@id, '{0}')]{1}", xPaths[0], (xPaths.Length > 1 ? xPaths[1] : string.Empty));
            }
        }
    }
}
