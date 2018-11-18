using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration;
using Microsoft.SharePoint.ApplicationPages;
using Microsoft.SharePoint.Utilities;
using Microsoft.SharePoint.WebControls;
using SPSJobConfigurator;

public class JobDefinition : AdvancedSettingsPage
{
    // Fields
    protected RadioButton AllSiteCollectionsRadioButton;
    protected Literal BeginDayLiteral;
    protected TextBox BeginDHourTextBox;
    protected Literal BeginDLiteral;
    protected TextBox BeginDMinuteTextBox;
    protected TextBox BeginDSecondTextBox;
    protected TextBox BeginMDayTextBox;
    protected TextBox BeginMHourTextBox;
    protected Literal BeginMinuteLiteral;
    protected TextBox BeginMinuteTextBox;
    protected TextBox BeginMMinuteTextBox;
    protected TextBox BeginMSecondTextBox;
    protected Literal BeginSecondLiteral;
    protected TextBox BeginSecondTextBox;
    protected DropDownList BeginWDayDropDownList;
    protected TextBox BeginWHourTextBox;
    protected Literal BeginWLiteral;
    protected TextBox BeginWMinuteTextBox;
    protected TextBox BeginWSecondTextBox;
    protected TextBox BeginYDayTextBox;
    protected TextBox BeginYHourTextBox;
    protected TextBox BeginYMinuteTextBox;
    protected TextBox BeginYMonthTextBox;
    protected TextBox BeginYSecondTextBox;
    protected Button CancelButton;
    protected Literal DailyDescriptionLiteral;
    protected Literal DailyRadioLiteral;
    protected Literal DDayLiteral;
    protected Button DisableButton;
    protected Literal EDBeginLiteral;
    protected Literal EHBeginLiteral;
    protected Literal EMBeginLiteral;
    protected Literal EndDayLiteral;
    protected TextBox EndDHourTextBox;
    protected Literal EndDLiteral;
    protected TextBox EndDMinuteTextBox;
    protected TextBox EndDSecondTextBox;
    protected TextBox EndMDayTextBox;
    protected TextBox EndMHourTextBox;
    protected Literal EndMinuteLiteral;
    protected TextBox EndMinuteTextBox;
    protected TextBox EndMMinuteTextBox;
    protected TextBox EndMSecondTextBox;
    protected Literal EndSecondLiteral;
    protected TextBox EndSecondTextBox;
    protected DropDownList EndWDayDropDownList;
    protected TextBox EndWHourTextBox;
    protected Literal EndWLiteral;
    protected TextBox EndWMinuteTextBox;
    protected TextBox EndWSecondTextBox;
    protected TextBox EndYDayTextBox;
    protected TextBox EndYHourTextBox;
    protected TextBox EndYMinuteTextBox;
    protected TextBox EndYMonthTextBox;
    protected TextBox EndYSecondTextBox;
    protected Literal ESBeginLiteral;
    protected Literal EveryLiteral;
    protected HyperLink HelpHyperLink;
    protected Literal HHourLiteral;
    protected Literal HourDBLiteral;
    protected Literal HourDELiteral;
    protected Literal HourlyDescriptionLiteral;
    protected Literal HourlyRadioLiteral;
    protected Literal HourWBLiteral;
    protected Literal HourWELiteral;
    protected Literal IntervalLiteral;
    protected DropDownList JobDefinitionDropDownList;
    protected InputFormSection JobDefinitionInputSectionTitle;
    protected Literal MDBeginLiteral;
    protected Literal MHBeginLiteral;
    protected Literal MinuteDBLiteral;
    protected Literal MinuteDELiteral;
    protected Literal MinutesDescriptionLiteral;
    protected TextBox MinutesIntervalTextBox;
    protected Literal MinutesLiteral;
    protected Literal MinutesRadioLiteral;
    protected Literal MinuteWBLiteral;
    protected Literal MinuteWELiteral;
    protected Literal MMBeginLiteral;
    protected Literal MMinuteLiteral;
    protected Literal MonthlyDescriptionLiteral;
    protected Literal MonthlyRadioLiteral;
    protected Literal MSBeginLiteral;
    protected Label NextOccurrenceInfoLabel;
    protected Label NextOccurrenceLabel;
    protected TextBox NextOccurrenceTextBox;
    protected Literal OptionalLiteral;
    protected Literal PageDescriptionLiteral;
    protected HtmlInputHidden ScheduleField;
    protected InputFormSection ScheduleInputSectionTitle;
    protected Literal SecondDBLiteral;
    protected Literal SecondDELiteral;
    protected Literal SecondWBLiteral;
    protected Literal SecondWELiteral;
    protected EncodedLiteral SiteCollectionDescription;
    protected InputFormSection SiteCollectionInputSectionTitle;
    protected Literal SiteCollectionLiteral;
    protected Repeater SiteCollectionRepeater;
    protected RadioButton SomeSiteCollectionsRadioButton;
    protected Literal SSecondLiteral;
    protected Button SubmitButton;
    protected Literal TitleAreaLiteral;
    protected Literal TitleLiteral;
    protected InputFormSection WebApplicationInputSectionTitle;
    protected WebApplicationSelector WebAppSelector;
    protected Literal WeeklyDescriptionLiteral;
    protected Literal WeeklyRadioLiteral;
    protected Literal YDBeginLiteral;
    protected Literal YDDayLiteral;
    protected Literal YDEndLiteral;
    protected Literal YearlyDescriptionLiteral;
    protected Literal YearlyRadioLiteral;
    protected Literal YHBeginLiteral;
    protected Literal YHEndLiteral;
    protected Literal YHHourLiteral;
    protected Literal YMBeginLiteral;
    protected Literal YMEndLiteral;
    protected Literal YMiBeginLiteral;
    protected Literal YMiEndLiteral;
    protected Literal YMMinuteLiteral;
    protected Literal YMMonthLiteral;
    protected Literal YSBeginLiteral;
    protected Literal YSecondLiteral;
    protected Literal YSEndLiteral;

    // Methods
    private SPSchedule BuildDailySchedule()
    {
        int beginHour = 0;
        if (BeginDHourTextBox.Text.Length > 0)
        {
            int.TryParse(BeginDHourTextBox.Text, out beginHour);
        }
        int beginMinute = 0;
        if (BeginDMinuteTextBox.Text.Length > 0)
        {
            int.TryParse(BeginDMinuteTextBox.Text, out beginMinute);
        }
        int beginSecond = 0;
        if (BeginDSecondTextBox.Text.Length > 0)
        {
            int.TryParse(BeginDSecondTextBox.Text, out beginSecond);
        }
        int endHour = 23;
        if (EndDHourTextBox.Text.Length > 0)
        {
            int.TryParse(EndDHourTextBox.Text, out endHour);
        }
        int endMinute = 59;
        if (EndDMinuteTextBox.Text.Length > 0)
        {
            int.TryParse(EndDMinuteTextBox.Text, out endMinute);
        }
        int endSecond = 59;
        if (EndDSecondTextBox.Text.Length > 0)
        {
            int.TryParse(EndDSecondTextBox.Text, out endSecond);
        }
        var schedule = new SPDailySchedule
                           {
                               BeginHour = beginHour,
                               BeginMinute = beginMinute,
                               BeginSecond = beginSecond,
                               EndHour = endHour,
                               EndMinute = endMinute,
                               EndSecond = endSecond
                           };
        return schedule;
    }

    private SPSchedule BuildHourlySchedule()
    {
        int beginMinute = 0;
        if (BeginMinuteTextBox.Text.Length > 0)
        {
            int.TryParse(BeginMinuteTextBox.Text, out beginMinute);
        }
        int endMinute = 59;
        if (EndMinuteTextBox.Text.Length > 0)
        {
            int.TryParse(EndMinuteTextBox.Text, out endMinute);
        }
        var schedule = new SPHourlySchedule {BeginMinute = beginMinute, EndMinute = endMinute};
        return schedule;
    }

    private SPSchedule BuildMinuteSchedule()
    {
        SPMinuteSchedule schedule = null;
        int interval;
        int.TryParse(MinutesIntervalTextBox.Text, out interval);
        int beginSecond = 0;
        if (BeginSecondTextBox.Text.Length > 0)
        {
            int.TryParse(BeginSecondTextBox.Text, out beginSecond);
        }
        int endSecond = 59;
        if (EndSecondTextBox.Text.Length > 0)
        {
            int.TryParse(EndSecondTextBox.Text, out endSecond);
        }
        if (interval > 0)
        {
            schedule = new SPMinuteSchedule {Interval = interval, BeginSecond = beginSecond, EndSecond = endSecond};
        }
        return schedule;
    }

    private SPSchedule BuildMonthlySchedule()
    {
        int beginDay = 1;
        if (BeginMDayTextBox.Text.Length > 0)
        {
            int.TryParse(BeginMDayTextBox.Text, out beginDay);
        }
        int beginHour = 0;
        if (BeginMHourTextBox.Text.Length > 0)
        {
            int.TryParse(BeginMHourTextBox.Text, out beginHour);
        }
        int beginMinute = 0;
        if (BeginMMinuteTextBox.Text.Length > 0)
        {
            int.TryParse(BeginMMinuteTextBox.Text, out beginMinute);
        }
        int beginSecond = 0;
        if (BeginMSecondTextBox.Text.Length > 0)
        {
            int.TryParse(BeginMSecondTextBox.Text, out beginSecond);
        }
        int endDay = 31;
        if (EndMDayTextBox.Text.Length > 0)
        {
            int.TryParse(EndMDayTextBox.Text, out endDay);
        }
        int endHour = 23;
        if (EndMHourTextBox.Text.Length > 0)
        {
            int.TryParse(EndMHourTextBox.Text, out endHour);
        }
        int endMinute = 59;
        if (EndMMinuteTextBox.Text.Length > 0)
        {
            int.TryParse(EndMMinuteTextBox.Text, out endMinute);
        }
        int endSecond = 59;
        if (EndMSecondTextBox.Text.Length > 0)
        {
            int.TryParse(EndMSecondTextBox.Text, out endSecond);
        }
        var schedule = new SPMonthlySchedule
                           {
                               BeginDay = beginDay,
                               BeginHour = beginHour,
                               BeginMinute = beginMinute,
                               BeginSecond = beginSecond,
                               EndDay = endDay,
                               EndHour = endHour,
                               EndMinute = endMinute,
                               EndSecond = endSecond
                           };
        return schedule;
    }

    private SPSchedule BuildWeeklySchedule()
    {
        int beginHour = 0;
        if (BeginWHourTextBox.Text.Length > 0)
        {
            int.TryParse(BeginWHourTextBox.Text, out beginHour);
        }
        int beginMinute = 0;
        if (BeginWMinuteTextBox.Text.Length > 0)
        {
            int.TryParse(BeginWMinuteTextBox.Text, out beginMinute);
        }
        int beginSecond = 0;
        if (BeginWSecondTextBox.Text.Length > 0)
        {
            int.TryParse(BeginWSecondTextBox.Text, out beginSecond);
        }
        int endHour = 23;
        if (EndWHourTextBox.Text.Length > 0)
        {
            int.TryParse(EndWHourTextBox.Text, out endHour);
        }
        int endMinute = 59;
        if (EndWMinuteTextBox.Text.Length > 0)
        {
            int.TryParse(EndWMinuteTextBox.Text, out endMinute);
        }
        int endSecond = 59;
        if (EndWSecondTextBox.Text.Length > 0)
        {
            int.TryParse(EndWSecondTextBox.Text, out endSecond);
        }

        var schedule = new SPWeeklySchedule {BeginDayOfWeek = DayOfWeek.Monday};

        switch (BeginWDayDropDownList.SelectedIndex)
        {
            case 0:
                schedule.BeginDayOfWeek = DayOfWeek.Monday;
                break;

            case 1:
                schedule.BeginDayOfWeek = DayOfWeek.Tuesday;
                break;

            case 2:
                schedule.BeginDayOfWeek = DayOfWeek.Wednesday;
                break;

            case 3:
                schedule.BeginDayOfWeek = DayOfWeek.Thursday;
                break;

            case 4:
                schedule.BeginDayOfWeek = DayOfWeek.Friday;
                break;

            case 5:
                schedule.BeginDayOfWeek = DayOfWeek.Saturday;
                break;

            case 6:
                schedule.BeginDayOfWeek = DayOfWeek.Sunday;
                break;
        }

        switch (EndWDayDropDownList.SelectedIndex)
        {
            case 1:
                schedule.EndDayOfWeek = DayOfWeek.Monday;
                break;

            case 2:
                schedule.EndDayOfWeek = DayOfWeek.Tuesday;
                break;

            case 3:
                schedule.EndDayOfWeek = DayOfWeek.Wednesday;
                break;

            case 4:
                schedule.EndDayOfWeek = DayOfWeek.Thursday;
                break;

            case 5:
                schedule.EndDayOfWeek = DayOfWeek.Friday;
                break;

            case 6:
                schedule.EndDayOfWeek = DayOfWeek.Saturday;
                break;

            case 7:
                schedule.EndDayOfWeek = DayOfWeek.Sunday;
                break;
        }

        schedule.BeginHour = beginHour;
        schedule.BeginMinute = beginMinute;
        schedule.BeginSecond = beginSecond;
        schedule.EndDayOfWeek = DayOfWeek.Monday;
        schedule.EndHour = endHour;
        schedule.EndMinute = endMinute;
        schedule.EndSecond = endSecond;

        return schedule;
    }

    private SPSchedule BuildYearlySchedule()
    {
        int beginMonth = 1;
        if (BeginYMonthTextBox.Text.Length > 0)
        {
            int.TryParse(BeginYMonthTextBox.Text, out beginMonth);
        }
        int beginDay = 1;
        if (BeginYDayTextBox.Text.Length > 0)
        {
            int.TryParse(BeginYDayTextBox.Text, out beginDay);
        }
        int beginHour = 0;
        if (BeginYHourTextBox.Text.Length > 0)
        {
            int.TryParse(BeginYHourTextBox.Text, out beginHour);
        }
        int beginMinute = 0;
        if (BeginYMinuteTextBox.Text.Length > 0)
        {
            int.TryParse(BeginYMinuteTextBox.Text, out beginMinute);
        }
        int beginSecond = 0;
        if (BeginYSecondTextBox.Text.Length > 0)
        {
            int.TryParse(BeginYSecondTextBox.Text, out beginSecond);
        }
        int endMonth = 12;
        if (EndYMonthTextBox.Text.Length > 0)
        {
            int.TryParse(EndYMonthTextBox.Text, out endMonth);
        }
        int endDay = 31;
        if (EndYDayTextBox.Text.Length > 0)
        {
            int.TryParse(EndYDayTextBox.Text, out endDay);
        }
        int endHour = 23;
        if (EndYHourTextBox.Text.Length > 0)
        {
            int.TryParse(EndYHourTextBox.Text, out endHour);
        }
        int endMinute = 59;
        if (EndYMinuteTextBox.Text.Length > 0)
        {
            int.TryParse(EndYMinuteTextBox.Text, out endMinute);
        }
        int endSecond = 59;
        if (EndYSecondTextBox.Text.Length > 0)
        {
            int.TryParse(EndYSecondTextBox.Text, out endSecond);
        }
        var schedule = new SPYearlySchedule
                           {
                               BeginMonth = beginMonth,
                               BeginDay = beginDay,
                               BeginHour = beginHour,
                               BeginMinute = beginMinute,
                               BeginSecond = beginSecond,
                               EndMonth = endMonth,
                               EndDay = endDay,
                               EndHour = endHour,
                               EndMinute = endMinute,
                               EndSecond = endSecond
                           };
        return schedule;
    }

    public void CancelButton_Click(object sender, EventArgs e)
    {
        SPUtility.Redirect(SPContext.Current.Web.Url + "/_admin/operations.aspx", SPRedirectFlags.DoNotEncodeUrl,
                           HttpContext.Current);
    }

    public void DisableButton_Click(object sender, EventArgs e)
    {
        SPJobDefinition jobDefinition = GetJobDefinition();

        if (jobDefinition != null)
        {
            jobDefinition.IsDisabled = DisableButton.Text != "Enable";
            jobDefinition.Update();
            SPUtility.Redirect(SPContext.Current.Web.Url + "/_admin/operations.aspx", SPRedirectFlags.DoNotEncodeUrl,
                               HttpContext.Current);
        }
    }

    private SPJobDefinition GetJobDefinition()
    {
        SPJobDefinition definition = null;
        string[] strArray = JobDefinitionDropDownList.SelectedValue.Split(new[] {'|'});
        var guid = new Guid(strArray[0]);
        try
        {
            definition = WebAppSelector.CurrentItem.JobDefinitions[guid];
        }
        catch
        {
        }
        if (definition == null)
        {
            try
            {
                definition = WebAppSelector.CurrentItem.WebService.JobDefinitions[guid];
            }
            catch
            {
            }
        }
        return definition;
    }

    protected void JobDefinitionDropDownList_SelectedIndexChanged(object sender, EventArgs e)
    {
        AllSiteCollectionsRadioButton.Checked = true;
        SomeSiteCollectionsRadioButton.Checked = false;
        if (JobDefinitionDropDownList.SelectedIndex > 0)
        {
            SPJobDefinition jobDefinition = GetJobDefinition();
            if (jobDefinition != null)
            {
                if (jobDefinition.Schedule != null)
                {
                    DateTime time = jobDefinition.Schedule.NextOccurrence(DateTime.Now);
                    NextOccurrenceLabel.Text = time.ToLongDateString() + " " + time.ToShortTimeString();
                    NextOccurrenceInfoLabel.Visible = true;
                    NextOccurrenceLabel.Visible = true;
                    DisableButton.Visible = true;
                    if (jobDefinition.IsDisabled)
                    {
                        DisableButton.Text = "Enable";
                    }
                    else
                    {
                        DisableButton.Text = "Disable";
                    }
                    PopulateSchedule(jobDefinition.Schedule);
                    SPSiteCollection sites = WebAppSelector.CurrentItem.Sites;
                    foreach (SPSite site in sites)
                    {
                        var child =
                            WebAppSelector.CurrentItem.GetChild<JobSettings>(jobDefinition.Name);

                        if ((child != null) && child.SiteCollectionsEnabled.Contains(site.ID))
                        {
                            SomeSiteCollectionsRadioButton.Checked = true;
                            foreach (RepeaterItem item in SiteCollectionRepeater.Items)
                            {
                                var box = item.FindControl("SiteCollectionCheckBox") as CheckBox;
                                box.Checked = true;
                                break;
                            }
                        }
                    }
                }
                else
                {
                    DisableButton.Visible = false;
                    NextOccurrenceInfoLabel.Visible = false;
                    NextOccurrenceLabel.Visible = false;
                }
            }
        }
    }

    protected void LocalizeStrings()
    {
        TitleLiteral.Text = SPUtility.GetLocalizedString("$Resources:TitleLiteral", "U2UJobDef",
                                                         SPContext.Current.Web.Language);
        TitleAreaLiteral.Text = SPUtility.GetLocalizedString("$Resources:TitleAreaLiteral", "U2UJobDef",
                                                             SPContext.Current.Web.Language);
        PageDescriptionLiteral.Text = SPUtility.GetLocalizedString("$Resources:PageDescriptionLiteral", "U2UJobDef",
                                                                   SPContext.Current.Web.Language);
        WebApplicationInputSectionTitle.Title = (
                                                    SPUtility.GetLocalizedString(
                                                        "$Resources:WebApplicationInputSectionTitle", "U2UJobDef",
                                                        SPContext.Current.Web.Language));
        WebApplicationInputSectionTitle.Description = (
                                                          SPUtility.GetLocalizedString(
                                                              "$Resources:WebApplicationInputSectionDescription",
                                                              "U2UJobDef",
                                                              SPContext.Current.Web.Language));
        JobDefinitionInputSectionTitle.Title = (
                                                   SPUtility.GetLocalizedString(
                                                       "$Resources:JobDefinitionInputSectionTitle", "U2UJobDef",
                                                       SPContext.Current.Web.Language));
        JobDefinitionInputSectionTitle.Description = (
                                                         SPUtility.GetLocalizedString(
                                                             "$Resources:JobDefinitionInputSectionTitle", "U2UJobDef",
                                                             SPContext.Current.Web.Language));
        SiteCollectionInputSectionTitle.Title = (
                                                    SPUtility.GetLocalizedString(
                                                        "$Resources:SiteCollectionInputSectionTitle", "U2UJobDef",
                                                        SPContext.Current.Web.Language));
        SiteCollectionLiteral.Text = SPUtility.GetLocalizedString("$Resources:SiteCollectionLiteral", "U2UJobDef",
                                                                  SPContext.Current.Web.Language);
        HelpHyperLink.Text = SPUtility.GetLocalizedString("$Resources:HelpHyperlink", "U2UJobDef",
                                                          SPContext.Current.Web.Language);
        SiteCollectionDescription.Text = SPUtility.GetLocalizedString(
            "$Resources:SiteCollectionInputSectionDescription", "U2UJobDef",
            SPContext.Current.Web.Language);
        AllSiteCollectionsRadioButton.Text = SPUtility.GetLocalizedString("$Resources:AllSiteCollectionsRadioButton",
                                                                          "U2UJobDef",
                                                                          SPContext.Current.Web.Language);
        SomeSiteCollectionsRadioButton.Text = SPUtility.GetLocalizedString("$Resources:SomeSiteCollectionsRadioButton",
                                                                           "U2UJobDef",
                                                                           SPContext.Current.Web.
                                                                               Language);
        ScheduleInputSectionTitle.Title = (SPUtility.GetLocalizedString("$Resources:ScheduleInputSectionTitle",
                                                                        "U2UJobDef",
                                                                        SPContext.Current.Web.Language));
        ScheduleInputSectionTitle.Description = (
                                                    SPUtility.GetLocalizedString(
                                                        "$Resources:ScheduleInputSectionDescription", "U2UJobDef",
                                                        SPContext.Current.Web.Language));
        NextOccurrenceInfoLabel.Text = SPUtility.GetLocalizedString("$Resources:NextOccurrenceInfoLabel", "U2UJobDef",
                                                                    SPContext.Current.Web.Language);
        MinutesRadioLiteral.Text = SPUtility.GetLocalizedString("$Resources:MinutesRadioLiteral", "U2UJobDef",
                                                                SPContext.Current.Web.Language);
        MinutesDescriptionLiteral.Text = SPUtility.GetLocalizedString("$Resources:MinutesDescriptionLiteral",
                                                                      "U2UJobDef",
                                                                      SPContext.Current.Web.Language);
        IntervalLiteral.Text = SPUtility.GetLocalizedString("$Resources:IntervalLiteral", "U2UJobDef",
                                                            SPContext.Current.Web.Language);
        EveryLiteral.Text = SPUtility.GetLocalizedString("$Resources:EveryLiteral", "U2UJobDef",
                                                         SPContext.Current.Web.Language);
        MinutesLiteral.Text = SPUtility.GetLocalizedString("$Resources:MinutesLiteral", "U2UJobDef",
                                                           SPContext.Current.Web.Language);
        OptionalLiteral.Text = SPUtility.GetLocalizedString("$Resources:OptionalLiteral", "U2UJobDef",
                                                            SPContext.Current.Web.Language);
        BeginSecondLiteral.Text = SPUtility.GetLocalizedString("$Resources:BeginSecondLiteral", "U2UJobDef",
                                                               SPContext.Current.Web.Language);
        EndSecondLiteral.Text = SPUtility.GetLocalizedString("$Resources:EndSecondLiteral", "U2UJobDef",
                                                             SPContext.Current.Web.Language);
        HourlyRadioLiteral.Text = SPUtility.GetLocalizedString("$Resources:HourlyRadioLiteral", "U2UJobDef",
                                                               SPContext.Current.Web.Language);
        HourlyDescriptionLiteral.Text = SPUtility.GetLocalizedString("$Resources:HourlyDescriptionLiteral", "U2UJobDef",
                                                                     SPContext.Current.Web.Language);
        BeginMinuteLiteral.Text = SPUtility.GetLocalizedString("$Resources:BeginMinuteLiteral", "U2UJobDef",
                                                               SPContext.Current.Web.Language);
        EndMinuteLiteral.Text = SPUtility.GetLocalizedString("$Resources:EndMinuteLiteral", "U2UJobDef",
                                                             SPContext.Current.Web.Language);
        DailyRadioLiteral.Text = SPUtility.GetLocalizedString("$Resources:DailyRadioLiteral", "U2UJobDef",
                                                              SPContext.Current.Web.Language);
        DailyDescriptionLiteral.Text = SPUtility.GetLocalizedString("$Resources:DailyDescriptionLiteral", "U2UJobDef",
                                                                    SPContext.Current.Web.Language);
        BeginDLiteral.Text = SPUtility.GetLocalizedString("$Resources:BeginLiteral", "U2UJobDef",
                                                          SPContext.Current.Web.Language);
        HourDBLiteral.Text = SPUtility.GetLocalizedString("$Resources:HourLiteral", "U2UJobDef",
                                                          SPContext.Current.Web.Language);
        MinuteDBLiteral.Text = SPUtility.GetLocalizedString("$Resources:MinuteLiteral", "U2UJobDef",
                                                            SPContext.Current.Web.Language);
        SecondDBLiteral.Text = SPUtility.GetLocalizedString("$Resources:SecondLiteral", "U2UJobDef",
                                                            SPContext.Current.Web.Language);
        EndDLiteral.Text = SPUtility.GetLocalizedString("$Resources:SecondLiteral", "U2UJobDef",
                                                        SPContext.Current.Web.Language);
        HourDELiteral.Text = SPUtility.GetLocalizedString("$Resources:HourLiteral", "U2UJobDef",
                                                          SPContext.Current.Web.Language);
        MinuteDELiteral.Text = SPUtility.GetLocalizedString("$Resources:MinuteLiteral", "U2UJobDef",
                                                            SPContext.Current.Web.Language);
        SecondDELiteral.Text = SPUtility.GetLocalizedString("$Resources:SecondLiteral", "U2UJobDef",
                                                            SPContext.Current.Web.Language);
        WeeklyRadioLiteral.Text = SPUtility.GetLocalizedString("$Resources:WeeklyRadioLiteral", "U2UJobDef",
                                                               SPContext.Current.Web.Language);
        WeeklyDescriptionLiteral.Text = SPUtility.GetLocalizedString("$Resources:WeeklyDescriptionLiteral", "U2UJobDef",
                                                                     SPContext.Current.Web.Language);
        BeginDayLiteral.Text = SPUtility.GetLocalizedString("$Resources:BeginDayLiteral", "U2UJobDef",
                                                            SPContext.Current.Web.Language);
        EndDayLiteral.Text = SPUtility.GetLocalizedString("$Resources:EndDayLiteral", "U2UJobDef",
                                                          SPContext.Current.Web.Language);
        BeginWLiteral.Text = SPUtility.GetLocalizedString("$Resources:BeginLiteral", "U2UJobDef",
                                                          SPContext.Current.Web.Language);
        HourWBLiteral.Text = SPUtility.GetLocalizedString("$Resources:HourLiteral", "U2UJobDef",
                                                          SPContext.Current.Web.Language);
        MinuteWBLiteral.Text = SPUtility.GetLocalizedString("$Resources:MinuteLiteral", "U2UJobDef",
                                                            SPContext.Current.Web.Language);
        SecondWBLiteral.Text = SPUtility.GetLocalizedString("$Resources:SecondLiteral", "U2UJobDef",
                                                            SPContext.Current.Web.Language);
        EndWLiteral.Text = SPUtility.GetLocalizedString("$Resources:EndLiteral", "U2UJobDef",
                                                        SPContext.Current.Web.Language);
        HourWELiteral.Text = SPUtility.GetLocalizedString("$Resources:HourLiteral", "U2UJobDef",
                                                          SPContext.Current.Web.Language);
        MinuteWELiteral.Text = SPUtility.GetLocalizedString("$Resources:MinuteLiteral", "U2UJobDef",
                                                            SPContext.Current.Web.Language);
        SecondWELiteral.Text = SPUtility.GetLocalizedString("$Resources:SecondLiteral", "U2UJobDef",
                                                            SPContext.Current.Web.Language);
        MonthlyRadioLiteral.Text = SPUtility.GetLocalizedString("$Resources:MonthlyRadioLiteral", "U2UJobDef",
                                                                SPContext.Current.Web.Language);
        MonthlyDescriptionLiteral.Text = SPUtility.GetLocalizedString("$Resources:MonthlyDescriptionLiteral",
                                                                      "U2UJobDef",
                                                                      SPContext.Current.Web.Language);
        DDayLiteral.Text = SPUtility.GetLocalizedString("$Resources:DDayLiteral", "U2UJobDef",
                                                        SPContext.Current.Web.Language);
        MDBeginLiteral.Text = SPUtility.GetLocalizedString("$Resources:BeginLiteral", "U2UJobDef",
                                                           SPContext.Current.Web.Language);
        EDBeginLiteral.Text = SPUtility.GetLocalizedString("$Resources:EndLiteral", "U2UJobDef",
                                                           SPContext.Current.Web.Language);
        HHourLiteral.Text = SPUtility.GetLocalizedString("$Resources:HHourLiteral", "U2UJobDef",
                                                         SPContext.Current.Web.Language);
        MHBeginLiteral.Text = SPUtility.GetLocalizedString("$Resources:BeginLiteral", "U2UJobDef",
                                                           SPContext.Current.Web.Language);
        EHBeginLiteral.Text = SPUtility.GetLocalizedString("$Resources:EndLiteral", "U2UJobDef",
                                                           SPContext.Current.Web.Language);
        MMinuteLiteral.Text = SPUtility.GetLocalizedString("$Resources:MMinuteLiteral", "U2UJobDef",
                                                           SPContext.Current.Web.Language);
        MMBeginLiteral.Text = SPUtility.GetLocalizedString("$Resources:BeginLiteral", "U2UJobDef",
                                                           SPContext.Current.Web.Language);
        EMBeginLiteral.Text = SPUtility.GetLocalizedString("$Resources:EndLiteral", "U2UJobDef",
                                                           SPContext.Current.Web.Language);
        SSecondLiteral.Text = SPUtility.GetLocalizedString("$Resources:SSecondLiteral", "U2UJobDef",
                                                           SPContext.Current.Web.Language);
        MSBeginLiteral.Text = SPUtility.GetLocalizedString("$Resources:BeginLiteral", "U2UJobDef",
                                                           SPContext.Current.Web.Language);
        ESBeginLiteral.Text = SPUtility.GetLocalizedString("$Resources:EndLiteral", "U2UJobDef",
                                                           SPContext.Current.Web.Language);
        YearlyRadioLiteral.Text = SPUtility.GetLocalizedString("$Resources:YearlyRadioLiteral", "U2UJobDef",
                                                               SPContext.Current.Web.Language);
        YearlyDescriptionLiteral.Text = SPUtility.GetLocalizedString("$Resources:YearlyDescriptionLiteral", "U2UJobDef",
                                                                     SPContext.Current.Web.Language);
        YMMonthLiteral.Text = SPUtility.GetLocalizedString("$Resources:MMonthLiteral", "U2UJobDef",
                                                           SPContext.Current.Web.Language);
        YMBeginLiteral.Text = SPUtility.GetLocalizedString("$Resources:BeginLiteral", "U2UJobDef",
                                                           SPContext.Current.Web.Language);
        YMEndLiteral.Text = SPUtility.GetLocalizedString("$Resources:EndLiteral", "U2UJobDef",
                                                         SPContext.Current.Web.Language);
        YDDayLiteral.Text = SPUtility.GetLocalizedString("$Resources:DDayLiteral", "U2UJobDef",
                                                         SPContext.Current.Web.Language);
        YDBeginLiteral.Text = SPUtility.GetLocalizedString("$Resources:BeginLiteral", "U2UJobDef",
                                                           SPContext.Current.Web.Language);
        YDEndLiteral.Text = SPUtility.GetLocalizedString("$Resources:EndLiteral", "U2UJobDef",
                                                         SPContext.Current.Web.Language);
        YHHourLiteral.Text = SPUtility.GetLocalizedString("$Resources:HHourLiteral", "U2UJobDef",
                                                          SPContext.Current.Web.Language);
        YHBeginLiteral.Text = SPUtility.GetLocalizedString("$Resources:BeginLiteral", "U2UJobDef",
                                                           SPContext.Current.Web.Language);
        YHEndLiteral.Text = SPUtility.GetLocalizedString("$Resources:EndLiteral", "U2UJobDef",
                                                         SPContext.Current.Web.Language);
        YMMinuteLiteral.Text = SPUtility.GetLocalizedString("$Resources:MMinuteLiteral", "U2UJobDef",
                                                            SPContext.Current.Web.Language);
        YMiBeginLiteral.Text = SPUtility.GetLocalizedString("$Resources:BeginLiteral", "U2UJobDef",
                                                            SPContext.Current.Web.Language);
        YMiEndLiteral.Text = SPUtility.GetLocalizedString("$Resources:EndLiteral", "U2UJobDef",
                                                          SPContext.Current.Web.Language);
        YSecondLiteral.Text = SPUtility.GetLocalizedString("$Resources:SSecondLiteral", "U2UJobDef",
                                                           SPContext.Current.Web.Language);
        YSBeginLiteral.Text = SPUtility.GetLocalizedString("$Resources:BeginLiteral", "U2UJobDef",
                                                           SPContext.Current.Web.Language);
        YSBeginLiteral.Text = SPUtility.GetLocalizedString("$Resources:EndLiteral", "U2UJobDef",
                                                           SPContext.Current.Web.Language);
        DisableButton.Text = SPUtility.GetLocalizedString("$Resources:DisableButton", "U2UJobDef",
                                                          SPContext.Current.Web.Language);
        SubmitButton.Text = SPUtility.GetLocalizedString("$Resources:SubmitButton", "U2UJobDef",
                                                         SPContext.Current.Web.Language);
        CancelButton.Text = SPUtility.GetLocalizedString("$Resources:CancelButton", "U2UJobDef",
                                                         SPContext.Current.Web.Language);
    }

    protected override void OnLoad(EventArgs e)
    {
        LocalizeStrings();
        if (!base.IsPostBack)
        {
            AllSiteCollectionsRadioButton.Attributes["onclick"] = "ShowSiteCollections('false');";
            SomeSiteCollectionsRadioButton.Attributes["onclick"] = "ShowSiteCollections('true');";
            BeginWDayDropDownList.Items.Clear();
            BeginWDayDropDownList.Items.Add(new ListItem("Monday", DayOfWeek.Monday.ToString()));
            BeginWDayDropDownList.Items.Add(new ListItem("Tuesday", DayOfWeek.Tuesday.ToString()));
            BeginWDayDropDownList.Items.Add(new ListItem("Wednesday", DayOfWeek.Wednesday.ToString()));
            BeginWDayDropDownList.Items.Add(new ListItem("Thursday", DayOfWeek.Thursday.ToString()));
            BeginWDayDropDownList.Items.Add(new ListItem("Friday", DayOfWeek.Friday.ToString()));
            BeginWDayDropDownList.Items.Add(new ListItem("Saturday", DayOfWeek.Saturday.ToString()));
            BeginWDayDropDownList.Items.Add(new ListItem("Sunday", DayOfWeek.Sunday.ToString()));
            EndWDayDropDownList.Items.Clear();
            EndWDayDropDownList.Items.Add("");
            EndWDayDropDownList.Items.Add(new ListItem("Monday", DayOfWeek.Monday.ToString()));
            EndWDayDropDownList.Items.Add(new ListItem("Tuesday", DayOfWeek.Tuesday.ToString()));
            EndWDayDropDownList.Items.Add(new ListItem("Wednesday", DayOfWeek.Wednesday.ToString()));
            EndWDayDropDownList.Items.Add(new ListItem("Thursday", DayOfWeek.Thursday.ToString()));
            EndWDayDropDownList.Items.Add(new ListItem("Friday", DayOfWeek.Friday.ToString()));
            EndWDayDropDownList.Items.Add(new ListItem("Saturday", DayOfWeek.Saturday.ToString()));
            EndWDayDropDownList.Items.Add(new ListItem("Sunday", DayOfWeek.Sunday.ToString()));
            NextOccurrenceInfoLabel.Visible = false;
            NextOccurrenceLabel.Visible = false;
            DisableButton.Visible = false;
        }
    }

    private void PopulateDailySchedule(SPSchedule currentSchedule)
    {
        var schedule = (SPDailySchedule) currentSchedule;
        BeginDHourTextBox.Text = schedule.BeginHour.ToString();
        BeginDMinuteTextBox.Text = schedule.BeginMinute.ToString();
        BeginDSecondTextBox.Text = schedule.BeginSecond.ToString();
        EndDHourTextBox.Text = schedule.EndHour.ToString();
        EndDMinuteTextBox.Text = schedule.EndMinute.ToString();
        EndDSecondTextBox.Text = schedule.EndSecond.ToString();
    }

    private void PopulateHourlySchedule(SPSchedule currentSchedule)
    {
        var schedule = (SPHourlySchedule) currentSchedule;
        BeginMinuteTextBox.Text = schedule.BeginMinute.ToString();
        EndMinuteTextBox.Text = schedule.EndMinute.ToString();
    }

    private void PopulateMinuteSchedule(SPSchedule currentSchedule)
    {
        var schedule = (SPMinuteSchedule) currentSchedule;
        BeginSecondTextBox.Text = schedule.BeginSecond.ToString();
        EndSecondTextBox.Text = schedule.EndSecond.ToString();
        MinutesIntervalTextBox.Text = schedule.Interval.ToString();
    }

    private void PopulateMonthlySchedule(SPSchedule currentSchedule)
    {
        var schedule = (SPMonthlySchedule) currentSchedule;
        BeginMDayTextBox.Text = schedule.BeginDay.ToString();
        BeginMHourTextBox.Text = schedule.BeginHour.ToString();
        BeginMMinuteTextBox.Text = schedule.BeginMinute.ToString();
        BeginMSecondTextBox.Text = schedule.BeginSecond.ToString();
        EndMDayTextBox.Text = schedule.EndDay.ToString();
        EndMHourTextBox.Text = schedule.EndHour.ToString();
        EndMMinuteTextBox.Text = schedule.EndMinute.ToString();
        EndMSecondTextBox.Text = schedule.EndSecond.ToString();
    }

    private void PopulateSchedule(SPSchedule schedule)
    {
        if (schedule is SPMinuteSchedule)
        {
            PopulateMinuteSchedule(schedule);
            ScheduleField.Value = "Minutes";
        }
        else if (schedule is SPHourlySchedule)
        {
            PopulateHourlySchedule(schedule);
            ScheduleField.Value = "Hourly";
        }
        else if (schedule is SPDailySchedule)
        {
            PopulateDailySchedule(schedule);
            ScheduleField.Value = "Daily";
        }
        else if (schedule is SPWeeklySchedule)
        {
            PopulateWeeklySchedule(schedule);
            ScheduleField.Value = "Weekly";
        }
        else if (schedule is SPMonthlySchedule)
        {
            PopulateMonthlySchedule(schedule);
            ScheduleField.Value = "Monthly";
        }
        else if (schedule is SPYearlySchedule)
        {
            PopulateYearlySchedule(schedule);
            ScheduleField.Value = "Yearly";
        }
    }

    private void PopulateWeeklySchedule(SPSchedule currentSchedule)
    {
        var schedule = (SPWeeklySchedule) currentSchedule;
        BeginWDayDropDownList.SelectedValue = schedule.BeginDayOfWeek.ToString();
        BeginWHourTextBox.Text = schedule.BeginHour.ToString();
        BeginWMinuteTextBox.Text = schedule.BeginMinute.ToString();
        BeginWSecondTextBox.Text = schedule.BeginSecond.ToString();
        EndWDayDropDownList.SelectedValue = schedule.EndDayOfWeek.ToString();
        EndWHourTextBox.Text = schedule.EndHour.ToString();
        EndWMinuteTextBox.Text = schedule.EndMinute.ToString();
        EndWSecondTextBox.Text = schedule.EndSecond.ToString();
    }

    private void PopulateYearlySchedule(SPSchedule currentSchedule)
    {
        var schedule = (SPYearlySchedule) currentSchedule;
        BeginYMonthTextBox.Text = schedule.BeginMonth.ToString();
        BeginYDayTextBox.Text = schedule.BeginDay.ToString();
        BeginYHourTextBox.Text = schedule.BeginHour.ToString();
        BeginYMinuteTextBox.Text = schedule.BeginMinute.ToString();
        BeginYSecondTextBox.Text = schedule.BeginSecond.ToString();
        EndYMonthTextBox.Text = schedule.EndMonth.ToString();
        EndYDayTextBox.Text = schedule.EndDay.ToString();
        EndYHourTextBox.Text = schedule.EndHour.ToString();
        EndYMinuteTextBox.Text = schedule.EndMinute.ToString();
        EndYSecondTextBox.Text = schedule.EndSecond.ToString();
    }

    protected void SiteCollectionRepeater_OnItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        if ((e.Item.ItemType == ListItemType.Item) || (e.Item.ItemType == ListItemType.AlternatingItem))
        {
            AllSiteCollectionsRadioButton.Checked = true;
            var dataItem = e.Item.DataItem as SPSite;
            var box = e.Item.FindControl("SiteCollectionCheckBox") as CheckBox;
            box.Text = dataItem.Url;
            if (JobDefinitionDropDownList.SelectedIndex > 0)
            {
                string[] strArray = JobDefinitionDropDownList.SelectedValue.Split(new[] {'|'});
                if (strArray.Length == 2)
                {
                    var child =
                        WebAppSelector.CurrentItem.GetChild<JobSettings>(strArray[1]);
                    if ((child != null) && child.SiteCollectionsEnabled.Contains(dataItem.ID))
                    {
                        SomeSiteCollectionsRadioButton.Checked = true;
                        box.Checked = true;
                    }
                }
            }
            var label = e.Item.FindControl("SiteCollectionIDLabel") as Label;
            label.Text = dataItem.ID.ToString();
        }
    }

    public void SubmitButton_Click(object sender, EventArgs e)
    {
        SPJobDefinition jobDefinition = GetJobDefinition();
        if (jobDefinition != null)
        {
            string str = ScheduleField.Value;
            SPSchedule schedule = null;
            string str2 = str;
            if (str2 != null)
            {
                if (!(str2 == "Minutes"))
                {
                    if (str2 == "Hourly")
                    {
                        schedule = BuildHourlySchedule();
                    }
                    else if (str2 == "Daily")
                    {
                        schedule = BuildDailySchedule();
                    }
                    else if (str2 == "Weekly")
                    {
                        schedule = BuildWeeklySchedule();
                    }
                    else if (str2 == "Monthly")
                    {
                        schedule = BuildMonthlySchedule();
                    }
                    else if (str2 == "Yearly")
                    {
                        schedule = BuildYearlySchedule();
                    }
                }
                else
                {
                    schedule = BuildMinuteSchedule();
                }
            }
            bool flag = true;
            if ((jobDefinition.Schedule != null) && jobDefinition.Schedule.Equals(schedule))
            {
                flag = false;
            }
            if (flag)
            {
                jobDefinition.Schedule = schedule;
                if (jobDefinition.IsDisabled)
                {
                    jobDefinition.IsDisabled = false;
                }
                jobDefinition.Update();
            }
            var child =
                WebAppSelector.CurrentItem.GetChild<JobSettings>(jobDefinition.Name);
            if (child == null)
            {
                child = new JobSettings(jobDefinition.Name, WebAppSelector.CurrentItem,
                                        Guid.NewGuid());
            }
            else
            {
                child.SiteCollectionsEnabled.Clear();
            }
            if (SomeSiteCollectionsRadioButton.Checked)
            {
                foreach (RepeaterItem item in SiteCollectionRepeater.Items)
                {
                    var box = item.FindControl("SiteCollectionCheckBox") as CheckBox;
                    if ((box != null) && box.Checked)
                    {
                        var label = item.FindControl("SiteCollectionIDLabel") as Label;
                        if (label != null)
                        {
                            var guid = new Guid(label.Text);
                            child.SiteCollectionsEnabled.Add(guid);
                        }
                    }
                }
            }
            child.Update();
        }
        SPUtility.Redirect(SPContext.Current.Web.Url + "/_admin/operations.aspx", SPRedirectFlags.DoNotEncodeUrl,
                           HttpContext.Current);
    }

    protected void WebAppSelector_OnContextChange(object sender, EventArgs e)
    {
        JobDefinitionDropDownList.Items.Clear();
        JobDefinitionDropDownList.Items.Add("...select a job definition...");
        var dictionary = new SortedDictionary<string, string>();
        foreach (SPJobDefinition definition in WebAppSelector.CurrentItem.WebService.JobDefinitions)
        {
            if (!string.IsNullOrEmpty(definition.Title))
            {
                JobDefinitionDropDownList.Items.Add(new ListItem(definition.Title,
                                                                 definition.Id + "|" +
                                                                 definition.Name));
            }
            else
            {
                JobDefinitionDropDownList.Items.Add(new ListItem(definition.DisplayName,
                                                                 definition.Id + "|" +
                                                                 definition.Name));
            }
        }
        foreach (SPJobDefinition definition in WebAppSelector.CurrentItem.JobDefinitions)
        {
            if (!string.IsNullOrEmpty(definition.Title))
            {
                JobDefinitionDropDownList.Items.Add(new ListItem(definition.Title,
                                                                 definition.Id + "|" +
                                                                 definition.Name));
            }
            else
            {
                JobDefinitionDropDownList.Items.Add(new ListItem(definition.DisplayName,
                                                                 definition.Id + "|" +
                                                                 definition.Name));
            }
        }
        if ((WebAppSelector.CurrentItem.Sites != null) &&
            (WebAppSelector.CurrentItem.Sites.Count > 0))
        {
            SiteCollectionRepeater.DataSource = WebAppSelector.CurrentItem.Sites;
            SiteCollectionRepeater.DataBind();
        }
        AllSiteCollectionsRadioButton.Checked = true;
    }
}