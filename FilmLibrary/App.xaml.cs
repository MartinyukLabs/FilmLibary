using DAL.Context;
using FilmLibrary.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace FilmLibrary
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static LibraryContext LibraryContext { get; set; }
        public App()
        {
            LibraryContext = new LibraryContext();
            if(LibraryContext.Films.Count() == 0)
            {
                IMDbService service = new IMDbService();
                service.Fill(LibraryContext, 30).Wait();
            }
        }
    }
}
