using BLL.DTO;
using BLL.Services;
using DAL.Context;
using DAL.Repository;
using GalaSoft.MvvmLight.Command;
using FilmLibrary.Infrastructure;
using FilmLibrary.Infrastructure;
using FilmLibrary.View;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace FilmLibrary.ViewModels
{
    public class UserProfileViewModel : BaseNotifyPropertyChanged
    {
        public static UserDTO UserDto;
        UserDTO user;
        public delegate void LogOut();
        public static LogOut LogOutEvent;
        public ICommand PasswordCommand
        {
            get => new GalaSoft.MvvmLight.Command.RelayCommand<object>(SaveChanges);
        }
        public ICommand UserExitCommand { get; set; }
        public UserDTO User
        {
            get => user;
            set
            {
                user = value;
                NotifyPropertyChanged();
            }
        }
        BitmapImage exitImage;
        public BitmapImage ExitImage
        {
            get => exitImage;
            set
            {
                exitImage = value;
                NotifyPropertyChanged();
            }
        }
        string passwordText;
        public string PasswordText
        {
            get => passwordText;
            set
            {
                passwordText = value;
                NotifyPropertyChanged();
            }
        }
        string text;
        public string Text
        {
            get => text;
            set
            {
                text = value;
                NotifyPropertyChanged();
            }
        }
        Brush textColor;
        public Brush TextColor
        {
            get => textColor;
            set
            {
                textColor = value;
                NotifyPropertyChanged();
            }
        }
        public UserProfileViewModel()
        {
            ExitImage = new BitmapImage(new Uri(Directory.GetCurrentDirectory() + "\\Images\\UserExit.png"));
            User = UserDto;
            PasswordText = "Confirm the password";
            UserExitCommand = new FilmLibrary.Infrastructure.RelayCommand(x =>
            {
                LoginAndRegistrationViewModel.IsLogin = false;
                UserDto = null;
                LogOutEvent?.Invoke();
                Switcher.Switch(new LoginAndRegistrationView());
            });
        }
        public void SaveChanges(object box)
        {
            PasswordBox pb = ((PasswordBox)box);
            UserService us = new UserService(new UserRepository(App.LibraryContext));
            UserDTO usr = us.GetAll().Where(x => x.Login == User.Login).ToList()[0];
            if (PasswordText == "Confirm the password")
            {
                if (usr.Password == pb.Password.GetHashCode().ToString())
                {
                    PasswordText = "Password confirmed\nYou can change him";
                    pb.Clear();
                    Text = "Correct password";
                    TextColor = Brushes.Green;
                }
                else
                {
                    Text = "Wrong password!";
                    TextColor = Brushes.Red;
                }
            }
            else
            {
                usr.Password = pb.Password.GetHashCode().ToString();
                us.AddOrUpdate(usr);
                Text = "Password changed!";
                TextColor = Brushes.Green;
                PasswordText = "Confirm the password";
                pb.Clear();
            }
        }
    }
}
