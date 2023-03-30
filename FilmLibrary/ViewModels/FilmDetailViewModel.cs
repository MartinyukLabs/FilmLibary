using AutoMapper;
using BLL.DTO;
using BLL.Services;
using DAL.Context;
using DAL.Repository;
using FilmLibrary.Infrastructure;
using FilmLibrary.Infrastructure;
using FilmLibrary.Models;
using FilmLibrary.View;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace FilmLibrary.ViewModels
{
    public class FilmDetailViewModel : BaseNotifyPropertyChanged
    {
        public ICommand AddToFavoriteCommand { get; set; }
        public ICommand RemoveFromFavoriteCommand { get; set; }
        FilmDTO selectedFilm;
        public FilmDTO SelectedFilm
        {
            get => selectedFilm;
            set
            {
                selectedFilm = value;
                NotifyPropertyChanged();
            }
        }
        Actor selectedActor;
        public Actor SelectedActor
        {
            get => selectedActor;
            set
            {
                selectedActor = value;
                NotifyPropertyChanged();
                OpenActor();
            }
        }
        Visibility addVisiblity;
        public Visibility AddVisiblity
        {
            get => addVisiblity;
            set
            {
                addVisiblity = value;
                NotifyPropertyChanged();
            }
        }
        Visibility removeVisibility;
        public Visibility RemoveVisibility
        {
            get => removeVisibility;
            set
            {
                removeVisibility = value;
                NotifyPropertyChanged();
            }
        }
        Producer selectedProducer;
        public Producer SelectedProducer
        {
            get => selectedProducer;
            set
            {
                selectedProducer = value;
                NotifyPropertyChanged();
                OpenProducer();
            }
        }
        public FilmDetailViewModel()
        {
            RemoveVisibility = Visibility.Collapsed;
            if (!LoginAndRegistrationViewModel.IsLogin)
            {
                AddVisiblity = Visibility.Collapsed;
                RemoveVisibility = Visibility.Collapsed;
            }
            else
                AddVisiblity = Visibility.Visible;
            AddToFavoriteCommand = new RelayCommand(x =>
            {
                if (LoginAndRegistrationViewModel.IsLogin)
                {
                    MapperConfiguration config = new MapperConfiguration(y => y.CreateMap<FilmDTO, Film>());
                    IMapper mapper = new Mapper(config);
                    UserProfileViewModel.UserDto.FavoriteFilms.Add(mapper.Map<FilmDTO, Film>(SelectedFilm));
                    var user = App.LibraryContext.Users.FirstOrDefault(us => us.Id == UserProfileViewModel.UserDto.Id);
                    user.FavoriteFilms.Add(App.LibraryContext.Films.FirstOrDefault(f => f.Id == SelectedFilm.Id));
                    App.LibraryContext.Users.AddOrUpdate(user);
                    App.LibraryContext.SaveChanges();
                    RemoveVisibility = Visibility.Visible;
                    AddVisiblity = Visibility.Collapsed;
                }
            });
            RemoveFromFavoriteCommand = new RelayCommand(x =>
            {
                if(LoginAndRegistrationViewModel.IsLogin)
                {
                    var user = App.LibraryContext.Users.Include(u => u.FavoriteFilms).FirstOrDefault(us => us.Id == UserProfileViewModel.UserDto.Id);
                    user.FavoriteFilms.Remove(App.LibraryContext.Films.FirstOrDefault(f => f.Id == SelectedFilm.Id));
                    App.LibraryContext.Users.AddOrUpdate(user);
                    App.LibraryContext.SaveChanges();
                    UserProfileViewModel.UserDto = new UserService(new UserRepository(App.LibraryContext)).Get(UserProfileViewModel.UserDto.Id);
                    RemoveVisibility = Visibility.Collapsed;
                    AddVisiblity = Visibility.Visible;
                }
            });
        }
        public void SetFilm(FilmDTO film)
        {
            SelectedFilm = film;
        }
        void OpenActor()
        {
            DataOfHuman data = new DataOfHuman()
            {
                Id = SelectedActor.Id,
                Name = SelectedActor.Name,
                Image = SelectedActor.Image,
                SmallInfo = SelectedActor.SmallInfo,
                Films = SelectedActor.Films
            };
            HumanDetailView view = new HumanDetailView();
            HumanDetailViewModel vm = (HumanDetailViewModel)view.DataContext;
            vm.SetHuman(data);
            Switcher.Switch(view);
        }
        void OpenProducer()
        {
            DataOfHuman data = new DataOfHuman()
            {
                Id = SelectedProducer.Id,
                Name = selectedProducer.Name,
                Image = SelectedProducer.Image,
                SmallInfo = SelectedProducer.SmallInfo,
                Films = selectedProducer.Films
            };
            HumanDetailView view = new HumanDetailView();
            HumanDetailViewModel vm = (HumanDetailViewModel)view.DataContext;
            vm.SetHuman(data);
            Switcher.Switch(view);
        }
        public void CheckFilm()
        {
            if (UserProfileViewModel.UserDto.FavoriteFilms.Where(x => x.Id == SelectedFilm.Id).Count() > 0)
            {
                RemoveVisibility = Visibility.Visible;
                AddVisiblity = Visibility.Collapsed;
            }
        }
    }
}
