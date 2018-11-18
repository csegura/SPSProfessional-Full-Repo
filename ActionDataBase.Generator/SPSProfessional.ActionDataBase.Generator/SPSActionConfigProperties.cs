using System.ComponentModel;

namespace SPSProfessional.ActionDataBase.Generator
{
    internal sealed class ControlsTypeConverter : StringConverter
    {
        private readonly string[] controls = {"TextBox", 
                                             "Memo", 
                                             "Lookup", 
                                             "Date", 
                                             "DateTime", 
                                             "ListBox", 
                                             "DropDownList"};

        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return true;
        }

        public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
        {
            return true;
        }

        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            return new StandardValuesCollection(controls);
        }
    }

    internal sealed class SqlTypesConverter : StringConverter
    {
        private readonly string[] types = {
                                              "BigInt",
                                              "Binary",
                                              "Image",
                                              "TimeStamp",
                                              "Bit",
                                              "Char",
                                              "NChar",
                                              "NText",
                                              "NVarChar",
                                              "SysName",
                                              "Text",
                                              "VarChar",
                                              "Datetime",
                                              "SmallDateTime",
                                              "Decimal",
                                              "Money",
                                              "Numeric",
                                              "SmallMoney",
                                              "Float",
                                              "Int",
                                              "Real",
                                              "SmallInt",
                                              "TinyInt"
                                          };

        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return true;
        }

        public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
        {
            return true;
        }

        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            return new StandardValuesCollection(types);
        }
    }

    internal sealed class DefaultsTypeConverter : StringConverter
    {
        private readonly string[] controls = {   "", 
                                                 "DateFormat()",
                                                 "DateNow()", 
                                                 "DateTimeNow()",
                                                 "MonthNumber()",
                                                 "YearNumber()",
                                                 "DayNumber()",
                                                 "Guid()", 
                                                 "QueryString()",
                                                 "QueryStringNull()",
                                                 "UserLogin()", 
                                                 "UserName()",
                                                 "UserId()",
                                                 "UserEmail()",
                                                 "WebName()",
                                                 "WebTitle()",
                                                 "WebUrl()"
                                                 };

        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return true;
        }

        public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
        {
            return true;
        }

        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            return new StandardValuesCollection(controls);
        }
    }

    internal sealed class TypeValidatorTypeConverter : StringConverter
    {
        private readonly string[] controls = { "RegEx", 
                                               "Compare", 
                                               "Range"};

        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return true;
        }

        public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
        {
            return true;
        }

        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            return new StandardValuesCollection(controls);
        }
    }

    internal sealed class DataTypeValidatorTypeConverter : StringConverter
    {
        private readonly string[] controls = { "", 
                                               "Currency", 
                                               "Date",
                                               "Double",
                                               "Integer",
                                               "String"};

        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return true;
        }

        public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
        {
            return true;
        }

        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            return new StandardValuesCollection(controls);
        }
    }

    internal sealed class ValidatorCompareOperatorTypeConverter : StringConverter
    {
        private readonly string[] controls = {
                                                 "",
                                                 "DataTypeCheck",
                                                 "Equal",
                                                 "GreaterThan",
                                                 "GreaterThanEqual",
                                                 "LessThan",
                                                 "LessThanEqual",
                                                 "NotEqual"
                                             };


        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return true;
        }

        public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
        {
            return true;
        }

        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            return new StandardValuesCollection(controls);
        }
    }

    internal sealed class ToolBarsTypeConverter : StringConverter
    {
        private readonly string[] controls = { "New", "Edit", "View" };

        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return true;
        }

        public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
        {
            return true;
        }

        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            return new StandardValuesCollection(controls);
        }
    }

    internal sealed class ToolBarsActionsTypeConverter : StringConverter
    {
        private readonly string[] controls = { "", "Edit", "Delete", "New", "Update", "Back" };

        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return true;
        }

        public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
        {
            return true;
        }

        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            return new StandardValuesCollection(controls);
        }
    }

    internal sealed class EnableDisableTypeConverter : StringConverter
    {
        private readonly string[] controls = { "enabled", "disabled" };

        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return true;
        }

        public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
        {
            return true;
        }

        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            return new StandardValuesCollection(controls);
        }
    }

    internal sealed class LookupControlTypeConverter : StringConverter
    {
        private readonly string[] controls = { "DropDownList", "PickerDataBase" };

        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return true;
        }

        public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
        {
            return true;
        }

        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            return new StandardValuesCollection(controls);
        }
    }

    internal sealed class RichTextTypeConverter : StringConverter
    {
        private readonly string[] controls = {"No", 
                                              "Simple", 
                                              "Full"};

        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return true;
        }

        public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
        {
            return true;
        }

        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            return new StandardValuesCollection(controls);
        }
    }
}