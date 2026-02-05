namespace BookStore.Data.Migrations
{
    using BookStore.Entities;
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<BookStore.Data.BookStoreDBContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(BookStore.Data.BookStoreDBContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method
            //  to avoid creating duplicate seed data.
            // create authors
            context.AuthorSet.AddOrUpdate(m => m.FirstName, GenerateAuthors());

            //  create genres
            context.GenreSet.AddOrUpdate(g => g.Name, GenerateGenres());

            // create movies
           context.BookSet.AddOrUpdate(m => m.Title, GenerateBooks());

            //// create stocks
            context.StockSet.AddOrUpdate(GenerateStocks());

            // create customers
            context.CustomerSet.AddOrUpdate(GenerateCustomers());

            // create roles
            context.RoleSet.AddOrUpdate(r => r.Name, GenerateRoles());

            // username: chsakell, password: homecinema
            context.UserSet.AddOrUpdate(u => u.Email, new User[]{
                new User()
                {
                    Email="senthilkumar.androsys@gmail.com",
                    Username="Senthilkumar",
                    HashedPassword ="XwAQoiq84p1RUzhAyPfaMDKVgSwnn80NCtsE8dNv3XI=",
                    Salt = "mNKLRbEFCH8y1xIyTXP4qA==",
                    IsLocked = false,
                    DateCreated = DateTime.Now
                }
            });

            // // create user-admin for chsakell
            context.UserRoleSet.AddOrUpdate(new UserRole[] {
                new UserRole() {
                    RoleId = 1, // admin
                    UserId = 1  // chsakell
                }
            });
        }

        private Genre[] GenerateGenres()
        {
            Genre[] genres = new Genre[] {
                new Genre() {ID=1, Name = "Comedy" },
                new Genre() {ID=2, Name = "Sci-fi" },
                new Genre() {ID=3,Name = "Action" },
                new Genre() {ID=4, Name = "Horror" },
                new Genre() {ID=5,Name = "Romance" },
                new Genre() {ID=6,Name = "Comedy" },
                new Genre() {ID=7,Name = "Crime" },
            };

            return genres;
        }
        private Author[] GenerateAuthors()
        {
            Author[] authors = new Author[] {
               new Author() { ID= 1,AuthorId= Guid.NewGuid(), FirstName = "Jane Austen", LastName="Jane",BirthDate = new DateTime(1985, 10, 20).AddMonths(2).AddDays(10),Nationality="USA", Bio="Bio-Description", Email="janeaustin@example.com", Affiliation =""},
               new Author() { ID= 2,AuthorId = Guid.NewGuid(), FirstName = "Charles Dickens", LastName = "Charles",BirthDate = new DateTime(1985, 10, 20).AddMonths(2).AddDays(10),Nationality="USA", Bio = "Bio-Description", Email = "charlesdickens@example.com", Affiliation = "" },
               new Author() { ID= 3,AuthorId = Guid.NewGuid(), FirstName = "Miguel de Cervantes", LastName = "Miguel",BirthDate = new DateTime(1985, 10, 20).AddMonths(2).AddDays(10),Nationality="USA", Bio = "Bio-Description", Email = "miguelde@example.com", Affiliation = "" },
               new Author() {ID= 4, AuthorId = Guid.NewGuid(), FirstName = "Brian Lynch", LastName = "Brian", BirthDate = new DateTime(1985, 10, 20).AddMonths(2).AddDays(10),Nationality="USA",Bio = "Bio-Description", Email = "brianlynch@example.com", Affiliation = "" },
               new Author() { ID= 5,AuthorId = Guid.NewGuid(), FirstName = "Seth MacFarlane", LastName = "Seth", BirthDate = new DateTime(1985, 10, 20).AddMonths(2).AddDays(10),Nationality="USA",Bio = "Bio-Description", Email = "sethmacferlane@example.com", Affiliation = "" },

            };
            return authors;
        }
        private Book[] GenerateBooks()
        {
            Book[] movies = new Book[] {
                new Book()
                {    
                    ID=1,
                    Title = "Pride and Prejudice",
                    Image ="minions.jpg",
                    GenreId = 1,
                    AuthorId=1,
                    IssueDate = new DateTime(2015, 7, 10),
                    Rating = 3,
                    Description = "Minions Stuart, Kevin and Bob are recruited by Scarlet Overkill, a super-villain who, alongside her inventor husband Herb, hatches a plot to take over the world.",
                    TrailURI = "https://www.youtube.com/watch?v=Wfql_DoHRKc"
                },
                new Book()
                {   
                    ID=2,
                    Title = "Northanger Abbey",
                    Image ="ted2.jpg",
                    GenreId = 1,
                    AuthorId=2,
                    IssueDate = new DateTime(2015, 6, 27),
                    Rating = 4,
                    Description = "Newlywed couple Ted and Tami-Lynn want to have a baby, but in order to qualify to be a parent, Ted will have to prove he's a person in a court of law.",
                    TrailURI = "https://www.youtube.com/watch?v=S3AVcCggRnU"
                },
                new Book()
                {
                    ID=3,
                    Title = "David Copperfield",
                    Image ="trainwreck.jpg",
                    GenreId = 2,
                    AuthorId=3,
                    IssueDate = new DateTime(2015, 7, 17),
                    Rating = 5,
                    Description = "Having thought that monogamy was never possible, a commitment-phobic career woman may have to face her fears when she meets a good guy.",
                    TrailURI = "https://www.youtube.com/watch?v=2MxnhBPoIx4"
                },
                new Book()
                {
                    ID=4,
                    Title="Inside Out",
                    Image ="insideout.jpg",
                    GenreId = 1,
                    AuthorId=4,
                    IssueDate = new DateTime(2015, 6, 19),
                    Rating = 4,
                    Description = "After young Riley is uprooted from her Midwest life and moved to San Francisco, her emotions - Joy, Fear, Anger, Disgust and Sadness - conflict on how best to navigate a new city, house, and school.",
                    TrailURI = "https://www.youtube.com/watch?v=seMwpP0yeu4"
                },
                new Book()
                {
                     ID=5,
                    Title = "Don Quixote",
                    Image ="home.jpg",
                    GenreId = 1,
                    AuthorId=5,
                    IssueDate = new DateTime(2015, 5, 27),
                    Rating = 4,
                    Description = "Oh, an alien on the run from his own people, lands on Earth and makes friends with the adventurous Tip, who is on a quest of her own.",
                    TrailURI = "https://www.youtube.com/watch?v=MyqZf8LiWvM"
                },
                new Book()
                {
                    ID=6,
                    Title="Batman v Superman: Dawn of Justice",
                    Image ="batmanvssuperman.jpg",
                    GenreId = 2,
                    AuthorId=2,
                    IssueDate = new DateTime(2015, 3, 25),
                    Rating = 4,
                    Description = "Fearing the actions of a god-like Super Hero left unchecked, Gotham City's own formidable, forceful vigilante takes on Metropolis most revered, modern-day savior, while the world wrestles with what sort of hero it really needs. And with Batman and Superman at war with one another, a new threat quickly arises, putting mankind in greater danger than it's ever known before.",
                    TrailURI = "https://www.youtube.com/watch?v=0WWzgGyAH6Y"
                },
                new Book()
                {
                    ID=7,
                    Title="Ant-Man",
                    Image ="antman.jpg",
                    GenreId = 2,
                    AuthorId=3,
                    IssueDate = new DateTime(2015, 7, 17),
                    Rating = 5,
                    Description = "Armed with a super-suit with the astonishing ability to shrink in scale but increase in strength, cat burglar Scott Lang must embrace his inner hero and help his mentor, Dr. Hank Pym, plan and pull off a heist that will save the world.",
                    TrailURI = "https://www.youtube.com/watch?v=1HpZevFifuo"
                },
                new Book()
                {
                      ID=8,
                    Title="Jurassic World",
                    Image ="jurassicworld.jpg",
                    GenreId = 2,
                    AuthorId=3,
                    IssueDate = new DateTime(2015, 6, 12),
                    Rating = 4,
                    Description = "A new theme park is built on the original site of Jurassic Park. Everything is going well until the park's newest attraction--a genetically modified giant stealth killing machine--escapes containment and goes on a killing spree.",
                    TrailURI = "https://www.youtube.com/watch?v=RFinNxS5KN4"
                },
                new Book()
                {
                    ID=9,
                    Title="Fantastic Four",
                    Image ="fantasticfour.jpg",
                    GenreId = 2,
                    AuthorId=2,
                    IssueDate = new DateTime(2015, 8, 7),
                    Rating = 2,
                    Description = "Four young outsiders teleport to an alternate and dangerous universe which alters their physical form in shocking ways. The four must learn to harness their new abilities and work together to save Earth from a former friend turned enemy.",
                    TrailURI = "https://www.youtube.com/watch?v=AAgnQdiZFsQ"
                },
                new Book()
                {
                    ID=10,
                    Title="Mad Max: Fury Road",
                    Image ="madmax.jpg",
                    GenreId = 2,
                    AuthorId=3,
                    IssueDate = new DateTime(2015, 5, 15),
                    Rating = 3,
                    Description = "In a stark desert landscape where humanity is broken, two rebels just might be able to restore order: Max, a man of action and of few words, and Furiosa, a woman of action who is looking to make it back to her childhood homeland.",
                    TrailURI = "https://www.youtube.com/watch?v=hEJnMQG9ev8"
                },
                new Book()
                {
                    ID=11,
                    Title="True Detective",
                    Image ="truedetective.jpg",
                    GenreId = 6,
                    AuthorId=2,
                    IssueDate = new DateTime(2015, 5, 15),
                    Rating = 4,
                    Description = "An anthology series in which police investigations unearth the personal and professional secrets of those involved, both within and outside the law.",
                    TrailURI = "https://www.youtube.com/watch?v=TXwCoNwBSkQ"
                },
                new Book()
                {
                    ID=12,
                    Title="The Longest Ride",
                    Image ="thelongestride.jpg",
                    GenreId = 5,
                    AuthorId=3,
                    IssueDate = new DateTime(2015, 5, 15),
                    Rating = 5,
                    Description = "After an automobile crash, the lives of a young couple intertwine with a much older man, as he reflects back on a past love.",
                    TrailURI = "https://www.youtube.com/watch?v=FUS_Q7FsfqU"
                },
                new Book()
                {
                    ID=13,
                    Title="The Walking Dead",
                    Image ="thewalkingdead.jpg",
                    GenreId = 4,
                    AuthorId=5,
                    IssueDate = new DateTime(2015, 3, 28),
                    Rating = 5,
                    Description = "Sheriff's Deputy Rick Grimes leads a group of survivors in a world overrun by zombies.",
                    TrailURI = "https://www.youtube.com/watch?v=R1v0uFms68U"
                },
                new Book()
                {
                    ID=14,
                    Title="Southpaw",
                    Image ="southpaw.jpg",
                    GenreId = 3,
                    AuthorId=5,
                    IssueDate = new DateTime(2015, 8, 17),
                    Rating = 4,
                    Description = "Boxer Billy Hope turns to trainer Tick Willis to help him get his life back on track after losing his wife in a tragic accident and his daughter to child protection services.",
                    TrailURI = "https://www.youtube.com/watch?v=Mh2ebPxhoLs"
                },
                new Book()
                {
                     ID=15,
                    Title="Specter",
                    Image ="spectre.jpg",
                    GenreId = 3,
                    AuthorId=4,
                    IssueDate = new DateTime(2015, 11, 5),
                    Rating = 5,
                    Description = "A cryptic message from Bond's past sends him on a trail to uncover a sinister organization. While M battles political forces to keep the secret service alive, Bond peels back the layers of deceit to reveal the terrible truth behind SPECTRE.",
                    TrailURI = "https://www.youtube.com/watch?v=LTDaET-JweU"
                },
            };

            return movies;
        }
        private Stock[] GenerateStocks()
        {
            List<Stock> stocks = new List<Stock>();

            for (int i = 1; i <= 15; i++)
            {
                // Three stocks for each movie
                for (int j = 0; j < 3; j++)
                {
                    Stock stock = new Stock()
                    {
                        BookId = i,
                        UniqueKey = Guid.NewGuid(),
                        IsAvailable = true
                    };
                    stocks.Add(stock);
                }
            }

            return stocks.ToArray();
        }
        private Customer[] GenerateCustomers()
        {
            List<Customer> _customers = new List<Customer>();

            // Create 100 customers
            for (int i = 0; i < 100; i++)
            {
                Customer customer = new Customer()
                {
                    FirstName = MockData.Person.FirstName(),
                    LastName = MockData.Person.Surname(),
                    IdentityCard = Guid.NewGuid().ToString(),
                    UniqueKey = Guid.NewGuid(),
                    Email = MockData.Internet.Email(),
                    DateOfBirth = new DateTime(1985, 10, 20).AddMonths(i).AddDays(10),
                    RegistrationDate = DateTime.Now.AddDays(i),
                    Mobile = "1234567890"
                };

                _customers.Add(customer);
            }

            return _customers.ToArray();
        }
        private Role[] GenerateRoles()
        {
            Role[] _roles = new Role[]{
                new Role()
                {
                    Name="Admin"
                }
            };

            return _roles;
        }
    }
}

