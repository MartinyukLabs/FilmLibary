﻿using AutoMapper;
using BLL.DTO;
using DAL.Context;
using FilmLibrary.Infrastructure;
using FilmLibrary.Infrastructure;
using FilmLibrary.Models;
using FilmLibrary.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FilmLibrary.ViewModels
{
    public class HumanDetailViewModel : BaseNotifyPropertyChanged
    {
        DataOfHuman human;
        public DataOfHuman Human
        {
            get => human;
            set
            {
                human = value;
                NotifyPropertyChanged();
            }
        }
        Film selectedFilm;
        public Film SelectedFilm
        {
            get => selectedFilm;
            set
            {
                selectedFilm = value;
                NotifyPropertyChanged();
                OpenFilm();
            }
        }

        private void OpenFilm()
        {
            FilmDetailView view = new FilmDetailView();
            FilmDetailViewModel vm = (FilmDetailViewModel)view.DataContext;
            MapperConfiguration config = new MapperConfiguration(x => x.CreateMap<Film, FilmDTO>());
            IMapper mapper = new Mapper(config);
            vm.SetFilm(mapper.Map < Film, FilmDTO>(SelectedFilm));
            Switcher.Switch(view);
        }

        public void SetHuman(DataOfHuman human)
        {
            Human = human;
        }
    }
}
