using Filmiverse.Models;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System;

namespace Filmiverse
{
    public class MoviesController : Controller
    {
        private FilmiverseDBContext db = new FilmiverseDBContext();

        // GET: Movies
        public ActionResult Index()
        {
            var movieList = new List<MovieViewModel>();
            var movies = db.Movies.ToList();
            //get details of actors present in each movie and build the MovieViewModel object
            foreach (var movie in movies)
            {
                var movieActors = db.MovieActors.Where(ma => ma.MovieId == movie.Id).Select(ma => ma.ActorId).ToList();
                var actorList = db.Actors.Where(a => movieActors.Contains(a.Id)).Select(a => a.Name).ToList();

                MovieViewModel mv = new Models.MovieViewModel
                {
                    Id = movie.Id,
                    Title = movie.Title,
                    Producer = movie.Producer,
                    Actors = actorList,
                    Poster = movie.Poster,
                    Plot = movie.Plot,
                    RunningTime = movie.RunningTime,
                    YearOfRelease = movie.YearOfRelease
                };
                movieList.Add(mv);
            }

            return View(movieList);
        }

        // GET: Movies/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var movie = db.Movies.Find(id);
            if (movie == null)
            {
                return HttpNotFound();
            }

            //get details of actors present in this movie and build it's MovieViewModel object
            var movieActors = db.MovieActors.Where(ma => ma.MovieId == movie.Id).Select(ma => ma.ActorId).ToList();
            var actorList = db.Actors.Where(a => movieActors.Contains(a.Id)).Select(a => a.Name).ToList();

            var mv = new MovieViewModel
            {
                Id = movie.Id,
                Title = movie.Title,
                Producer = movie.Producer,
                Actors = actorList,
                Poster = movie.Poster,
                Plot = movie.Plot,
                RunningTime = movie.RunningTime,
                YearOfRelease = movie.YearOfRelease
            };

            return View(mv);
        }

        // GET: Movies/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Movies/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(MovieViewModel movie)
        {
            //check if the Producer details are present
            if (!db.Producers.Any(p => p.Name.ToLower() == movie.Producer.ToLower()))
            {
                ModelState.AddModelError("Producer", movie.Producer + " doesn't exist. Add Producer");
            }

            //check if the Actors details are present
            if (movie.Actors != null && movie.Actors.Count > 0)
                for (int i = 0; i < movie.Actors.Count; i++)
                {
                    var name = movie.Actors[i].ToLower();
                    if (!db.Actors.Any(a => a.Name.ToLower() == name))
                    {
                        ModelState.AddModelError("Actors[" + i + "]", movie.Actors[i] + " doesn't exist. Add Actor");
                    }
                }

            if (ModelState.IsValid)
            {
                //check if any image file has been chosen by the user to add the movie poster
                if (Request.Files.Count > 0)
                {
                    var img = Request.Files[0];
                    if (img != null && img.ContentLength > 0)
                    {
                        byte[] buffer = new byte[img.ContentLength];
                        img.InputStream.Read(buffer, 0, buffer.Length);
                        movie.Poster = buffer;
                    }
                }

                var mv = new Movie
                {
                    Title = movie.Title,
                    Producer = movie.Producer,
                    Poster = movie.Poster,
                    Plot = movie.Plot,
                    RunningTime = movie.RunningTime,
                    YearOfRelease = movie.YearOfRelease
                };

                mv = db.Movies.Add(mv);

                //get ids of actors present in this movie and then add MovieActor objects for all actors
                var actorIdList = db.Actors.Where(a => movie.Actors.Contains(a.Name))?.Select(a => a.Id).Distinct().ToList();

                foreach (int actorId in actorIdList)
                {
                    MovieActor ma = new MovieActor
                    {
                        MovieId = mv.Id,
                        ActorId = actorId
                    };
                    db.MovieActors.Add(ma);
                }

                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(movie);
        }

        // GET: Movies/Edit/5
        /// <summary>
        /// Edit movie's information
        /// </summary>
        /// <param name="id">Movie id</param>
        /// <param name="type">decides if a row should be added or deleted</param>
        /// <param name="actors">existing list of actors in ui</param>
        /// <returns></returns>
        public ActionResult Edit(int? id, string type = "", string actors = "")
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Movie movie = db.Movies.Find(id);
            if (movie == null)
            {
                return HttpNotFound();
            }

            //get details of actors present in this movie and build it's MovieViewModel object
            var movieActors = db.MovieActors.Where(ma => ma.MovieId == movie.Id).Select(ma => ma.ActorId).ToList();
            var actorList = db.Actors.Where(a => movieActors.Contains(a.Id)).Select(a => a.Name).ToList();

            if (type == "add")//called on adding a row for actors
            {
                if (!string.IsNullOrEmpty(actors))
                {
                    var actorArray = actors.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                    if (actorArray != null)
                        actorList = actorArray.ToList();
                }

                actorList.Add("");
            }
            else if (type == "delete")//called on deleting a row for actors
            {
                if (actorList.Count > 1)
                {
                    if (!string.IsNullOrEmpty(actors))
                    {
                        var actorArray = actors.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                        if (actorArray != null)
                            actorList = actorArray.ToList();
                    }

                    actorList.RemoveAt(actorList.Count - 1);
                }
            }

            var mv = new MovieViewModel
            {
                Id = movie.Id,
                Title = movie.Title,
                Producer = movie.Producer,
                Actors = actorList,
                Poster = movie.Poster,
                Plot = movie.Plot,
                RunningTime = movie.RunningTime,
                YearOfRelease = movie.YearOfRelease
            };

            return View(mv);
        }

        // POST: Movies/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(MovieViewModel movie)
        {
            //check if the Producer details are present
            if (!db.Producers.Any(p => p.Name.ToLower() == movie.Producer.ToLower()))
            {
                ModelState.AddModelError("Producer", movie.Producer + " doesn't exist. Add Producer");
            }

            //check if the Actors details are present
            if (movie.Actors != null && movie.Actors.Count > 0)
            {
                for (int i = 0; i < movie.Actors.Count; i++)
                {
                    var name = movie.Actors[i].ToLower();
                    if (!db.Actors.Any(a => a.Name.ToLower() == name))
                    {
                        ModelState.AddModelError("Actors[" + i + "]", movie.Actors[i] + " actor doesn't exist. Add Actor");
                    }
                }
            }
            else
            {
                ModelState.AddModelError("Actors", "No actor present. Add Actor");
            }

            if (ModelState.IsValid)
            {
                //check if any image file has been chosen by the user to update the movie poster
                if (Request.Files.Count > 0)
                {
                    var img = Request.Files[0];
                    if (img != null && img.ContentLength > 0)
                    {
                        byte[] buffer = new byte[img.ContentLength];
                        img.InputStream.Read(buffer, 0, buffer.Length);
                        movie.Poster = buffer;
                    }
                }

                //update the corresponding movie db object from the viewmodel
                var mv = db.Movies.Find(movie.Id);
                mv.Plot = movie.Plot;
                mv.Title = movie.Title;
                mv.RunningTime = movie.RunningTime;
                mv.YearOfRelease = movie.YearOfRelease;
                mv.Producer = movie.Producer;
                mv.Poster = movie.Poster;

                db.Entry(mv).State = EntityState.Modified;

                //get ids of actors present in this movie and then add MovieActor objects for all actors
                var actorIdList = db.Actors.Where(a => movie.Actors.Contains(a.Name))?.Select(a => a.Id).Distinct().ToList();

                foreach (int actorId in actorIdList)
                {
                    //if any entry exists for this actor againt this movie then ignore
                    if (!db.MovieActors.Any(e => e.ActorId == actorId && e.MovieId == mv.Id))
                    {
                        MovieActor ma = new MovieActor
                        {
                            MovieId = movie.Id,
                            ActorId = actorId
                        };
                        db.MovieActors.Add(ma);
                    }
                }

                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(movie);
        }

        // GET: Movies/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Movie movie = db.Movies.Find(id);
            if (movie == null)
            {
                return HttpNotFound();
            }

            //get details of actors present in this movie and build it's MovieViewModel object
            var movieActors = db.MovieActors.Where(ma => ma.MovieId == movie.Id).Select(ma => ma.ActorId)?.ToList();
            var actorList = db.Actors.Where(a => movieActors.Contains(a.Id)).Select(a => a.Name)?.ToList();

            var mv = new MovieViewModel
            {
                Id = movie.Id,
                Title = movie.Title,
                Producer = movie.Producer,
                Actors = actorList ?? new List<string>(),
                Poster = movie.Poster,
                Plot = movie.Plot,
                RunningTime = movie.RunningTime,
                YearOfRelease = movie.YearOfRelease
            };

            return View(mv);
        }

        // POST: Movies/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Movie movie = db.Movies.Find(id);
            if (movie != null)
                db.Movies.Remove(movie);
            //also delete coressponding entries from MovieActors
            var movieActors = db.MovieActors.Where(ma => ma.MovieId == id)?.ToList();

            if (movieActors != null && movieActors.Count > 0)
                db.MovieActors.RemoveRange(movieActors);

            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
