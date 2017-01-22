﻿using OpenQA.Selenium;

namespace Atata.KendoUI
{
    [ControlDefinition("span", ContainingClass = "k-dropdown", ComponentTypeName = "drop-down list")]
    [IdXPathForLabel("@aria-owns='{0}_listbox'")]
    public class KendoDropDownList<T, TOwner> : EditableField<T, TOwner>
        where TOwner : PageObject<TOwner>
    {
        [FindByAttribute("data-role", "dropdownlist", Visibility = Visibility.Any)]
        private Control<TOwner> DataControl { get; set; }

        [FindByClass("k-select")]
        [Name("Drop-Down")]
        [Wait(0.5)]
        protected virtual ClickableDelegate<TOwner> DropDownButton { get; set; }

        protected string ValueXPath
        {
            get { return Metadata.Get<ValueXPathAttribute>(AttributeLevels.DeclaredAndComponent)?.XPath; }
        }

        protected string ItemValueXPath
        {
            get { return Metadata.Get<ItemValueXPathAttribute>(AttributeLevels.DeclaredAndComponent)?.XPath; }
        }

        protected override T GetValue()
        {
            string value = Scope.Get(By.XPath(".//span[@class='k-input']{0}").FormatWith(ValueXPath)).Text.Trim();
            return ConvertStringToValue(value);
        }

        protected override void SetValue(T value)
        {
            string valueAsString = ConvertValueToString(value);

            DropDownButton.Click();

            GetDropDownOption(valueAsString).
                Click();
        }

        protected virtual IWebElement GetDropDownList()
        {
            return Driver.
                Get(By.CssSelector(".k-animation-container .k-list-container.k-popup").DropDownList());
        }

        protected virtual IWebElement GetDropDownOption(string value, SearchOptions searchOptions = null)
        {
            return GetDropDownList().
               Get(By.XPath(".//li{0}[normalize-space(.)='{1}']").FormatWith(ItemValueXPath, value).DropDownOption(value).With(searchOptions));
        }

        protected override bool GetIsReadOnly()
        {
            return DataControl.Attributes.GetValue("readonly") != null;
        }

        protected override bool GetIsEnabled()
        {
            return DataControl.IsEnabled;
        }
    }
}
