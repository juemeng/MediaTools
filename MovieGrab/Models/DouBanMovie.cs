using System.Collections.Generic;
using System.Linq;

namespace MovieGrab.Models
{
    class DouBanMovie
    {
        public int Count { get; set; }
        public int Start { get; set; }
        public int Total { get; set; }
        public string Title { get; set; }
        public List<Subject> Subjects { get; set; }

        internal class Subject
        {
            public Rating Rating { get; set; }
            public List<string> Genres { get; set; }
            public string Title { get; set; }
            public List<People> Casts { get; set; }
            public int Collect_count { get; set; }
            public string Original_title { get; set; }
            public string Subtype { get; set; }
            public List<People> Directors { get; set; }
            public int Year { get; set; }
            public Picture Images { get; set; }
            public string Alt { get; set; }
            public string Id { get; set; }
        }

        internal class Rating
        {
            public int Max { get; set; }
            public int Min { get; set; }
            public int Stars { get; set; }
            public double Average { get; set; }
        }

        internal class People
        {
            public string Alt { get; set; }
            public string Name { get; set; }
            public string Id { get; set; }
            public Picture Avatars { get; set; }
        }

        internal class Picture
        {
            public string Small { get; set; }
            public string Large { get; set; }
            public string Medium { get; set; }
        }
    }


    class DouBanMovieDetail
    {

    }


    
}