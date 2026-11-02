using System.Text;

namespace MauiApp1
{
    public partial class MainPage : ContentPage
    {
        private const string DefaultText = "";
        private const int DefaultLanguageIndex = -1;
        private const bool DefaultRadioChecked = false;
        private const bool DefaultSqlChecked = false;
        private const bool DefaultDockerChecked = false;
        private const bool DefaultGitChecked = false;
        private const bool DefaultRestChecked = false;
        private const bool DefaultCommercialExperience = false;
        private const double DefaultExperienceMonths = 0;
        private const double DefaultHoursPerWeek = 20;
        private const bool DefaultShowResultEnabled = false;
        private const bool DefaultResultVisible = false;
        private static readonly DateTime DefaultBirthDate = new(2000, 1, 1);

        private const string NotSpecified = "не указано";
        private const string NotSelected = "не выбран";

        public MainPage()
        {
            InitializeComponent();
        }

        private void OnRequiredFieldTextChanged(object? sender, TextChangedEventArgs e)
        {
            UpdateShowResultButtonState();
        }

        private void UpdateShowResultButtonState()
        {
            ShowResultButton.IsEnabled = !string.IsNullOrWhiteSpace(FullNameEntry.Text)
                                         && !string.IsNullOrWhiteSpace(EmailEntry.Text);
        }

        private void OnCommercialSwitchToggled(object? sender, ToggledEventArgs e)
        {
            ExperienceBlock.IsVisible = e.Value;
        }

        private void OnHoursSliderValueChanged(object? sender, ValueChangedEventArgs e)
        {
            var rounded = Math.Round(e.NewValue);
            if (rounded != e.NewValue)
            {
                HoursSlider.Value = rounded;
            }
        }

        private async void OnShowResultClicked(object? sender, EventArgs e)
        {
            var errors = ValidateRequiredFields();
            if (errors.Count > 0)
            {
                ResultLabel.IsVisible = false;
                ResultLabel.Text = DefaultText;
                await DisplayAlertAsync("Ошибка заполнения", string.Join(Environment.NewLine, errors), "OK");
                return;
            }

            ResultLabel.Text = BuildSummary();
            ResultLabel.IsVisible = true;
        }

        private List<string> ValidateRequiredFields()
        {
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(FullNameEntry.Text))
            {
                errors.Add("• Укажите ФИО.");
            }

            var email = EmailEntry.Text?.Trim() ?? string.Empty;
            if (email.Length == 0)
            {
                errors.Add("• Укажите email.");
            }
            else if (!email.Contains('@'))
            {
                errors.Add("• Email должен содержать символ @.");
            }

            return errors;
        }

        private string BuildSummary()
        {
            var skills = new List<string>();
            if (SqlCheckBox.IsChecked) skills.Add("SQL");
            if (DockerCheckBox.IsChecked) skills.Add("Docker");
            if (GitCheckBox.IsChecked) skills.Add("Git");
            if (RestCheckBox.IsChecked) skills.Add("REST API");

            var employment = new[] { FullTimeRadio, PartTimeRadio, RemoteRadio }
                .FirstOrDefault(r => r.IsChecked)?.Value?.ToString() ?? NotSelected;

            var language = LanguagePicker.SelectedItem as string ?? NotSelected;

            var commercial = CommercialSwitch.IsToggled
                ? $"да, {ExperienceStepper.Value:F0} мес."
                : "нет";

            var birthDate = BirthDatePicker.Date is DateTime date
                ? date.ToString("dd.MM.yyyy")
                : NotSpecified;

            var summary = new StringBuilder();
            summary.AppendLine("Результат анкеты:");
            summary.AppendLine($"ФИО: {FullNameEntry.Text.Trim()}");
            summary.AppendLine($"Email: {EmailEntry.Text.Trim()}");
            summary.AppendLine($"Телефон: {ValueOrDefault(PhoneEntry.Text)}");
            summary.AppendLine($"О себе: {ValueOrDefault(AboutEditor.Text)}");
            summary.AppendLine($"Основной язык: {language}");
            summary.AppendLine($"Формат занятости: {employment}");
            summary.AppendLine($"Навыки: {(skills.Count > 0 ? string.Join(", ", skills) : "не отмечены")}");
            summary.AppendLine($"Коммерческий опыт: {commercial}");
            summary.AppendLine($"Дата рождения: {birthDate}");
            summary.Append($"Часов в неделю: {HoursSlider.Value:F0}");
            return summary.ToString();
        }

        private static string ValueOrDefault(string? value)
        {
            return string.IsNullOrWhiteSpace(value) ? NotSpecified : value.Trim();
        }

        private void OnClearClicked(object? sender, EventArgs e)
        {
            FullNameEntry.Text = DefaultText;
            EmailEntry.Text = DefaultText;
            PhoneEntry.Text = DefaultText;
            AboutEditor.Text = DefaultText;

            LanguagePicker.SelectedIndex = DefaultLanguageIndex;

            FullTimeRadio.IsChecked = DefaultRadioChecked;
            PartTimeRadio.IsChecked = DefaultRadioChecked;
            RemoteRadio.IsChecked = DefaultRadioChecked;

            SqlCheckBox.IsChecked = DefaultSqlChecked;
            DockerCheckBox.IsChecked = DefaultDockerChecked;
            GitCheckBox.IsChecked = DefaultGitChecked;
            RestCheckBox.IsChecked = DefaultRestChecked;

            CommercialSwitch.IsToggled = DefaultCommercialExperience;
            ExperienceBlock.IsVisible = DefaultCommercialExperience;
            ExperienceStepper.Value = DefaultExperienceMonths;

            BirthDatePicker.Date = DefaultBirthDate;
            HoursSlider.Value = DefaultHoursPerWeek;

            ResultLabel.Text = DefaultText;
            ResultLabel.IsVisible = DefaultResultVisible;
            ShowResultButton.IsEnabled = DefaultShowResultEnabled;
        }
    }
}
