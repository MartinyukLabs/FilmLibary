using DAL.Context;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using PexelsDotNetSDK.Api;
using System.Data.Entity.Migrations;
using System.Net;
using System.Drawing;

namespace FilmLibrary.Models
{
    public class IMDbService
    {
        List<Actor> Actors { get; set; } = new List<Actor>();
        List<Genre> Genres { get; set; } = new List<Genre>();
        List<Producer> Producers { get; set; } = new List<Producer>();
        Film Film { get; set; } = new Film();
        LibraryContext context { get; set; }
        HttpClient client = new HttpClient();

        string apikey = "k_mr2gkm66";
        string path = "filmlist.txt";
        string photoApiKey = "563492ad6f91700001000001043a650f1cee45ed9079a5b40c98aad0";
        readonly string startUrlTitle = "https://imdb8.p.rapidapi.com/title/";
        readonly string startUrlActor = "https://imdb8.p.rapidapi.com/actors/";
        public IMDbService()
        {
            RequestOptions.MostPopular = apikey;
        }
        public async Task Fill(LibraryContext context, int max = 100)
        {
            this.context = context;
            if (!File.Exists(path))
                await CreateFilmList();
            List<string> items = File.ReadAllLines(path).ToList();
            int count = max < items.Count ? max : items.Count;
            for (int i = 0; i < count; i++)
            {
                RequestOptions.Title = $"{apikey}/{items[i]}";
                using (HttpResponseMessage response = await client.GetAsync(new Uri(RequestOptions.Title)))
                {
                    JObject film = (JObject)JsonConvert.DeserializeObject(await response.Content.ReadAsStringAsync());
                    if (film["id"].ToString().Count() == 0)
                        if (!ChangeApiKey())
                            return;
                    string filmid = items[i];
                    if (!context.Films.Any(x => x.ImdbId == filmid))
                    {
                        Film.ImdbId = filmid;
                        SetFilm(film).Wait();
                        if (Film.Title == null || Film.Plot == null || Film.Poster == null)
                        {
                            Clear();
                            continue;
                        }

                        context.Films.Add(Film);
                        context.SaveChanges();
                        Film = context.Films.FirstOrDefault(x => x.ImdbId == Film.ImdbId);
                        Film.Actors = Actors;
                        Film.Genres = Genres;
                        Film.Producers = Producers;
                        context.Films.AddOrUpdate(Film);
                        context.SaveChanges();
                        Clear();
                    }
                }
            }
        }
        bool ChangeApiKey()
        {
            //Get main project folder
            var dir = Directory.GetParent(Assembly.GetExecutingAssembly().Location).Parent;
            dir = dir.Parent;
            dir = dir.Parent;

            string keysPath = dir.FullName + "\\keys.txt";
            if (File.Exists(keysPath))
            {
                string[] keys = File.ReadAllLines(keysPath);
                for (int i = 0; i < keys.Length; i++)
                    if (keys[i] == apikey && i + 1 < keys.Length)
                    {
                        apikey = keys[i + 1];
                        return true;
                    }
                    else if (keys[i] != apikey && i + 1 >= keys.Length)
                        return false;
            }
            return false;
        }
        async Task SetFilm(JObject film)
        {
            try
            {
                Film.Title = film["title"].ToString();
                Film.DateOfPublishing = Convert.ToDateTime(film["releaseDate"]);
                Film.Poster = film["image"].ToString();
                Film.Plot = film["plot"].ToString();
                Film.Trailer = film["trailer"]["linkEmbed"].ToString();
                if (Film.Title == null || Film.Plot == null || Film.Poster == null)
                    return;
                SetGenres(film["genreList"].ToString()).Wait();
                //SetCrew(film).Wait();
                await SetProducers(film["id"].ToString());
                await SetActors(film["id"].ToString());
            }
            catch (HttpRequestException ex)
            {
                if (!File.Exists("errors.txt"))
                    File.Create("errors.txt").Close();
                File.AppendAllText("errors.txt", $"bad request - {ex.Message}");
            }
            catch { }
        }
        async Task SetCrew(JObject film)
        {
            //var pexelsClient = new PexelsClient(photoApiKey);
            //int count = context.Producers.Count();
            //foreach (JObject director in (JArray)JsonConvert.DeserializeObject(film["directorList"].ToString()))
            //    if (context.Producers.AsEnumerable().Any(x => x.ImdbId == director["id"].ToString()))
            //        Producers.Add(context.Producers.FirstOrDefault(x => x.ImdbId == director["id"].ToString()));
            //    else
            //    {
            //        var task = GetBio(director["id"].ToString());
            //        task.Wait();
            //        var photo = await pexelsClient.SearchPhotosAsync(director["name"].ToString());
            //        Producers.Add(new Producer()
            //        {
            //            ImdbId = director["id"].ToString(),
            //            Name = director["name"].ToString(),
            //            SmallInfo = task.Result
            //        });
            //        if (photo.photos.Count > 0 && photo.photos[0].source.medium.Length > 0)
            //            Producers.Last().Image = photo.photos[0].source.medium;
            //    }
            //foreach (JObject writer in (JArray)JsonConvert.DeserializeObject(film["writerList"].ToString()))
            //    if (context.Producers.AsEnumerable().Any(x => x.ImdbId == writer["id"].ToString()))
            //        Producers.Add(context.Producers.FirstOrDefault(x => x.ImdbId == writer["id"].ToString()));
            //    else
            //    {
            //        var task = GetBio(writer["id"].ToString());
            //        task.Wait();
            //        var photo = await pexelsClient.SearchPhotosAsync(writer["name"].ToString());
            //        Producers.Add(new Producer()
            //        {
            //            ImdbId = writer["id"].ToString(),
            //            Name = writer["name"].ToString(),
            //            SmallInfo = task.Result
            //        });
            //        if (photo.photos.Count > 0 && photo.photos[0].source.medium.Length > 0)
            //            Producers.Last().Image = photo.photos[0].source.medium;
            //    }
            //int i = 0;
            //foreach (JObject actor in (JArray)JsonConvert.DeserializeObject(film["actorList"].ToString()))
            //{
            //    if (i > 10)
            //        break;
            //    if (context.Actors.AsEnumerable().Any(x => x.ImdbId == actor["id"].ToString()))
            //        Actors.Add(context.Actors.FirstOrDefault(x => x.ImdbId == actor["id"].ToString()));
            //    else
            //    {
            //        var task = GetBio(actor["id"].ToString());
            //        task.Wait();
            //        Actors.Add(new Actor()
            //        {
            //            ImdbId = actor["id"].ToString(),
            //            Name = actor["name"].ToString(),
            //            SmallInfo = task.Result
            //        });
            //        if (actor["image"].ToString().Length > 0)
            //            Actors.Last().Image = actor["image"].ToString();
            //    }
            //    i++;
            //}
        }
        async Task SetGenres(string genresList)
        {
            JArray genres = (JArray)JsonConvert.DeserializeObject(genresList);
            foreach (JObject genre in genres)
            {
                string name = genre["value"].ToString();
                if (!context.Genres.Any(x => x.Name.ToLower().Equals(name.ToLower())))
                    Genres.Add(new Genre() { Name = name });
                else
                    Genres.Add(context.Genres.FirstOrDefault(x => x.Name.ToLower().Equals(name.ToLower())));
            }
        }
        public async Task CreateFilmList()
        {
            using (HttpResponseMessage response = await client.GetAsync(new Uri(RequestOptions.MostPopular)))
            {
                JObject obj = (JObject)JsonConvert.DeserializeObject(await response.Content.ReadAsStringAsync());
                JArray items = (JArray)obj["items"];
                File.Create(path).Close();
                foreach (var item in items)
                    File.AppendAllText(path, $"{item["id"]}\n");
            }
        }
        void Clear()
        {
            this.Genres = new List<Genre>();
            this.Actors = new List<Actor>();
            this.Producers = new List<Producer>();
            Film = new Film();
        }
        async Task SetProducers(string title)
        {
            string uri = startUrlTitle + "get-top-crew?tconst=" + title;
            //List<Producer> producers = new List<Producer>();
            JObject obj = (JObject)JsonConvert.DeserializeObject(SendRequest(uri).Result);
            List<string> ids = new List<string>();
            int count = 0;
            if (obj["writers"]?.Count() != null)
                count = (int)obj["writers"]?.Count();
            if (count > 2)
                count = 2;
            for (int i = 0; i < count; i++)
            {
                Producers.Add(new Producer()
                {
                    Name = obj["writers"][i]["name"]?.ToString(),
                    ImdbId = obj["writers"]?[i]?["id"].ToString().Split(new char[] { '/' })[2]
                });
                if (obj["writers"]?[i]?["image"]?["url"]?.ToString() != null)
                    Producers[Producers.Count - 1].Image = obj["writers"][i]["image"]?["url"]?.ToString();
                else
                    Producers[Producers.Count - 1].Image = "https://www.icrewz.com/wp-content/files/producers-chair1-233x244.jpg";
                ids.Add(obj["writers"]?[i]?["id"].ToString().Split(new char[] { '/' })[2]);
            }
            for (int i = 0; i < ids.Count; i++)
            {
                uri = startUrlActor + "get-bio?nconst=" + Producers[i].ImdbId;
                obj = (JObject)JsonConvert.DeserializeObject(SendRequest(uri).Result);
                Producers[i].SmallInfo = obj["miniBios"]?[0]?["text"]?.ToString();
            }
        }
        async Task SetActors(string title)
        {
            string uri = startUrlTitle + "get-top-cast?tconst=" + title;
            //List<Actor> actors = new List<Actor>();
            JArray array = (JArray)JsonConvert.DeserializeObject(SendRequest(uri).Result);
            List<string> ids = new List<string>();
            foreach (var val in array)
            {
                ids.Add(val.Value<string>());
            }
            int count = ids.Count;
            if (count > 7)
                count = 7;
            for (int i = 0; i < count; i++)
            {
                uri = startUrlActor + "get-bio?nconst=" + ids[i].Split(new char[] { '/' })[2];
                JObject obj = (JObject)JsonConvert.DeserializeObject(SendRequest(uri).Result);
                Actors.Add(new Actor()
                {
                    Name = obj["name"]?.ToString(),
                    SmallInfo = obj["miniBios"]?[0]?["text"]?.ToString(),
                    ImdbId = ids[i].Split(new char[] { '/' })[2]
                });
                if (obj["image"]?["url"]?.ToString() != null)
                    Actors[Actors.Count - 1].Image = obj["image"]?["url"]?.ToString();
                else
                    Actors[Actors.Count - 1].Image = "https://e7.pngegg.com/pngimages/906/448/png-clipart-silhouette-person-person-with-helmut-animals-logo.png";
            }
        }
        async Task<string> SendRequest(string uri)
        {
            string str;
            var client = new HttpClient();
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(uri),
                Headers =
                    {
                        { "x-rapidapi-key", "7467395e30msh911943b787d67b4p1adf6djsncee5f2744cae" },
                        { "x-rapidapi-host", "imdb8.p.rapidapi.com" },
                    },
            };
            using (var response = await client.SendAsync(request))
            {
                response.EnsureSuccessStatusCode();
                var body = await response.Content.ReadAsStringAsync();
                str = body;
            }
            if (str == "{\"message\":\"The security token included in the request is expired\"}")
                str = SendRequest(uri).Result;
            return str;
        }
    }
}
