﻿using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using TerminologyLauncher.Configs;
using TerminologyLauncher.Entities.Account;
using TerminologyLauncher.GUI.Annotations;
using TerminologyLauncher.GUI.ToolkitWindows.PopupWindow;
using TerminologyLauncher.I18n.TranslationObjects.GUITranslations;

namespace TerminologyLauncher.GUI
{


    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public sealed partial class LoginWindow : INotifyPropertyChanged
    {
        private bool IsPerservePasswordValue;


        public delegate void LogingHandler(Object serder, EventArgs e);

        public event LogingHandler Logining;

        public LoginWindowTranslation Translation
        {
            get
            {
                return I18n.TranslationProvider.TranslationProviderInstance
                      .TranslationObject.GuiTranslation.LoginWindowTranslation;
            }
        }

        private String BackgroundImageSourceValue;
        public String BackgroundImageSource
        {
            get { return this.BackgroundImageSourceValue; }
            set { this.BackgroundImageSourceValue = value; }
        }

        public Config Config { get; set; }

        public LoginWindow(Config config)
        {
            this.Config = config;

            if (!String.IsNullOrEmpty(this.Config.GetConfig("loginWindowBackground")) && File.Exists(this.Config.GetConfig("loginWindowBackground")))
            {
                var imageFile = new FileInfo(this.Config.GetConfig("loginWindowBackground"));
                this.BackgroundImageSource = imageFile.FullName;
            }
            else
            {
                this.BackgroundImageSource = @"pack://application:,,,/TerminologyLauncher.GUI;component/Resources/login_bg.jpg";
            }

            this.InitializeComponent();
            this.OnPropertyChanged();
        }

        public void EnableAllInputs(Boolean isEnable)
        {
            this.Dispatcher.InvokeAsync(() =>
            {
                this.UsernameBox.IsEnabled = isEnable;
                this.PasswordBox.IsEnabled = isEnable;
                this.LoginModeComboBox.IsEnabled = isEnable;
                this.IsPerservePasswordCheckBox.IsEnabled = isEnable;
                this.CancleButton.IsEnabled = isEnable;
                this.LoginButton.IsEnabled = isEnable;
            });
        }

        public void CrossThreadClose()
        {
            this.Dispatcher.Invoke(this.Close);
        }

        public Boolean IsPerservePassword
        {
            get { return this.IsPerservePasswordValue; }
            set
            {
                this.IsPerservePasswordValue = value;
                this.OnPropertyChanged();
            }
        }

        public LoginEntity GetLogin()
        {
            LoginEntity login = null;
            this.Dispatcher.Invoke(() =>
            {
                login = new LoginEntity()
                {
                    UserName = this.UsernameBox.Text,
                    Password = this.PasswordBox.Password,
                    LoginType = (LoginType)this.LoginModeComboBox.SelectedIndex,
                    PerserveLogin = this.IsPerservePassword
                };
            });
            if (login == null)
            {
                throw new Exception("Can not get valid login entity");
            }
            return login;

        }

        public void SetLogin(LoginEntity login)
        {
            this.Dispatcher.Invoke(() =>
            {
                this.UsernameBox.Text = login.UserName;
                this.PasswordBox.Password = login.Password;
                this.LoginModeComboBox.SelectedIndex = (int)login.LoginType;
                this.IsPerservePassword = login.PerserveLogin;
            });

        }

        public void LoginResult(LoginResultType result)
        {
            this.Dispatcher.Invoke(() =>
            {
                switch (result)
                {
                    case LoginResultType.Success:
                        {
                            this.Hide();
                            break;
                        }
                    case LoginResultType.IncompleteOfArguments:
                        {
                            new PopupWindow(this, this.Translation.LoginFaultTranslation, this.Translation.LoginFaultInsufficientArgumentsTranslation).ShowDialog();
                            break;
                        }
                    case LoginResultType.WrongPassword:
                        {
                            new PopupWindow(this, this.Translation.LoginFaultTranslation,this.Translation.LoginFaultWrongPasswordTranslation).ShowDialog();
                            break;
                        }
                    case LoginResultType.UserNotExists:
                        {
                            new PopupWindow(this, this.Translation.LoginFaultTranslation, this.Translation.LoginFaultUserNotExistTranslation).ShowDialog();
                            break;
                        }
                    case LoginResultType.NetworkTimedOut:
                        {
                            new PopupWindow(this, this.Translation.LoginFaultTranslation, this.Translation.LoginFaultNetworkTimedOutTranslation).ShowDialog();
                            break;
                        }
                    default:
                        {
                            new PopupWindow(this, this.Translation.LoginFaultTranslation, this.Translation.LoginFaultUnknownErrorTranslation).ShowDialog();
                            break;
                        }
                }
                this.EnableAllInputs(true);
            });

        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = this.PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        private void LoginMode_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var combox = sender as ComboBox;
            var selected = combox.SelectedIndex;
            this.AccountTypeTitle.Text = selected == 1 ? this.Translation.MojongAccountTranslation : this.Translation.OfflineAccountTranslation;

            if (selected == 0)
            {
                this.AccountPasswordTitle.Visibility = Visibility.Hidden;
                this.PasswordBox.Visibility = Visibility.Hidden;

            }
            else
            {
                this.AccountPasswordTitle.Visibility = Visibility.Visible;
                this.PasswordBox.Visibility = Visibility.Visible;
            }

        }

        private void OnLogining(object serder)
        {
            var handler = this.Logining;
            if (handler != null) handler(serder, EventArgs.Empty);
        }

        private void UIElement_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void ToggleButton_OnCheckedChanged(object sender, RoutedEventArgs e)
        {
            var checkbox = (CheckBox)sender;
            var isChecked = (checkbox).IsChecked;
            if (isChecked == null) return;
            this.PasswordBox.Password = String.Empty;
            if ((bool)isChecked)
            {
                this.PasswordBox.IsEnabled = false;
            }
            else
            {
                this.PasswordBox.IsEnabled = true;
            }
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            this.OnLogining(this);
        }

        private void CommandBinding_OnExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            this.OnLogining(this);
        }
    }
}
