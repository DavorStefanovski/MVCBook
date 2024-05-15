using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MVCBookAesthetic.Data;
using System.IO;
using System.Numerics;
using Microsoft.AspNetCore.Identity;
using MVCBookAesthetic.Areas.Identity.Data;

namespace MVCBookAesthetic.Models
{
    public class SeedData
    {
        public static async Task CreateUserRoles(IServiceProvider serviceProvider)
        {
            var RoleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var UserManager = serviceProvider.GetRequiredService<UserManager<MVCBookAestheticUser>>();
            IdentityResult roleResult;
            //Add Admin Role
            var roleCheck = await RoleManager.RoleExistsAsync("Admin");
            if (!roleCheck) { 
                roleResult = await RoleManager.CreateAsync(new IdentityRole("Admin")); 
            }
            //user role
            roleCheck = await RoleManager.RoleExistsAsync("User");
            if (!roleCheck)
            {
                roleResult = await RoleManager.CreateAsync(new IdentityRole("User"));
            }

            MVCBookAestheticUser user = await UserManager.FindByEmailAsync("admin@mvcbookaesthetic.com");
            if (user == null)
            {
                var User = new MVCBookAestheticUser();
                User.Email = "admin@mvcbookaesthetic.com";
                User.UserName = "admin@mvcbookaesthetic.com";
                string userPWD = "Admin123";
                IdentityResult chkUser = await UserManager.CreateAsync(User, userPWD);
                //Add default User to Role Admin
                if (chkUser.Succeeded) { var result1 = await UserManager.AddToRoleAsync(User, "Admin"); }
            }
        }

        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new MVCBookAestheticContext(serviceProvider.GetRequiredService<DbContextOptions<MVCBookAestheticContext>>()))
            {
                CreateUserRoles(serviceProvider).Wait();
                if (context.Book.Any() || context.Author.Any() || context.Genre.Any() || context.Review.Any() || context.UserBooks.Any())
                {
                    return; // DB has been seeded
                }

                context.Author.AddRange(
                 new Author { /*Id = 1, */FirstName = "Bernhard", LastName = "Schlink", BirthDate = DateTime.Parse("194-3-6"), Nationality="German", Gender="Male" },
                 new Author { /*Id = 2, */FirstName = "Fyodor", LastName = "Dostoevsky", BirthDate = DateTime.Parse("1821-11-11"), Nationality = "Russian", Gender = "Male" },
                 new Author { /*Id = 3, */FirstName = "Agatha", LastName = "Christie", BirthDate = DateTime.Parse("1890-5-30"), Nationality = "British", Gender = "Female" }
                 );
                context.SaveChanges();

                context.Genre.AddRange(
                 new Genre { /*Id = 1, */GenreName = "Classic" },
                 new Genre { /*Id = 1, */GenreName = "Thriller" },
                 new Genre { /*Id = 1, */GenreName = "Historical fiction" },
                 new Genre { /*Id = 1, */GenreName = "Romance" },
                 new Genre { /*Id = 1, */GenreName = "Crime" },
                 new Genre { /*Id = 1, */GenreName = "Psychological fiction" }
                 );
                context.SaveChanges();

                context.Book.AddRange(
                 new Book
                 {
                     //Id = 1,
                     Title = "Crime and punishment",
                     YearPublished = 1866,
                     NumPages = 527,
                     Description = "Crime and Punishment is a novel by the Russian author Fyodor Dostoevsky. It was first published in the literary journal The Russian Messenger in twelve monthly installments during 1866.",
                     Publisher = "The Russian Messenger",
                     FrontPage = "https://upload.wikimedia.org/wikipedia/en/4/4b/Crimeandpunishmentcover.png",
                     DownloadUrl = "https://www.planetebook.com/free-ebooks/crime-and-punishment.pdf",
                     AuthorId = context.Author.Single(d => d.FirstName == "Fyodor" && d.LastName == "Dostoevsky").Id
                 },
                  new Book
                  {
                      //Id = 1,
                      Title = "The reader",
                      YearPublished = 1995,
                      NumPages = 218,
                      Description = "The Reader is a novel by German law professor and judge Bernhard Schlink, published in Germany in 1995 and in the United States in 1997.",
                      Publisher = "Vintage International",
                      FrontPage = "https://m.media-amazon.com/images/I/71mfdLSI+PL._AC_UF1000,1000_QL80_.jpg",
                      DownloadUrl = "https://www.screenwriter.ch/demandit/files/M_BFNO38KLSZ567NYSJ29/dms/File/The_Reader.pdf",
                      AuthorId = context.Author.Single(d => d.FirstName == "Bernhard" && d.LastName == "Schlink").Id
                  },
                    new Book
                    {
                        //Id = 1,
                        Title = "Murder on the Orient Express",
                        YearPublished = 1934,
                        NumPages = 256,
                        Description = "Murder on the Orient Express is a work of detective fiction by English writer Agatha Christie featuring the Belgian detective Hercule Poirot. It was first published in the United Kingdom by the Collins Crime Club on 1 January 1934.",
                        Publisher = "Collins Crime Club",
                        FrontPage = "https://m.media-amazon.com/images/I/71ihbKf67RL._AC_UF1000,1000_QL80_.jpg",
                        DownloadUrl = "http://detective.gumer.info/anto/christie_8_2.pdf",
                        AuthorId = context.Author.Single(d => d.FirstName == "Agatha" && d.LastName == "Christie").Id
                    }
                 );
                context.SaveChanges();

                 context.BookGenre.AddRange
                (
                 new BookGenre { BookId = 1, GenreId = 1 },
                 new BookGenre { BookId = 1, GenreId = 6 },
                 new BookGenre { BookId = 2, GenreId = 3 },
                 new BookGenre { BookId = 2, GenreId = 6 },
                 new BookGenre { BookId = 3, GenreId = 2 },
                 new BookGenre { BookId = 3, GenreId = 5 }

                 );
                context.SaveChanges();

                context.Review.AddRange
                    (
                    new Review
                    {
                        BookId = 1,
                        AppUser = "Angela",
                        Comment = "Dostoyevsky writes a wonderful book that deals with the relationships, punishments, and psychological states of a criminal.",
                        Rating = 10
                    },
                    new Review
                    {
                        BookId = 1,
                        AppUser = "Ana",
                        Comment = "Raskolnikov, a destitute and desperate former student, wanders through the slums of St Petersburg and commits a random murder without remorse or regret.",
                        Rating = 9
                    },
                    new Review
                    {
                        BookId = 2,
                        AppUser = "Dave",
                        Comment = "Hailed for its coiled eroticism and the moral claims it makes upon the reader, this mesmerizing novel is a story of love and secrets, horror and compassion, unfolding against the haunted landscape of postwar Germany.",
                        Rating = 7
                    },
                    new Review
                    {
                        BookId = 3,
                        AppUser = "Maria",
                        Comment = "With the killer still on the train, the world famous detective Hercule Poirot must race against time to solve the case before anyone else is murdered.",
                        Rating = 8
                    }

                    );
                context.SaveChanges();


            }

        }
    }
}
