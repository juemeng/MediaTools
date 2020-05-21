using AngleSharp;
using AngleSharp.Css.Dom;
using AngleSharp.Dom;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace SubTitleShooter
{
    class Program
    {
        static readonly List<SubTitleSite> sites = new List<SubTitleSite> 
        { 
            new SubTitleSite
            {
                SearchUrl = "https://assrt.net/sub/?searchword=",
                DownloadUrl = "https://assrt.net"
            }

        };


        static async System.Threading.Tasks.Task Main(string[] args)
        {
            var httpClient = new HttpClient();
            var currentFolder = AppDomain.CurrentDomain.BaseDirectory;

            Console.Write("input movie name to search: ");
            var movie = Console.ReadLine();
            var encodedMovieName = HttpUtility.UrlEncode(movie);
            foreach (var site in sites)
            {
                try
                {
                    var url = site.SearchUrl + encodedMovieName;
                    var html = await httpClient.GetStringAsync(url);

                    var context = BrowsingContext.New(Configuration.Default);
                    var document = await context.OpenAsync(req => req.Content(html));

                    var subTitleList =
                        document.All.Where(x => x.LocalName == "div" && x.ClassList.Contains("subitem") && x.Id != "top-banner" && x.Id != "bottom-banner").ToList();
                    var data = subTitleList.Select(x =>
                    {
                        var titleElement = x.QuerySelector("a.introtitle");
                        var downloadElement = x.Children.SingleOrDefault(y => y.LocalName == "a" && y.Attributes["title"].Value == "下载本字幕");
                        var href = downloadElement.Attributes["onclick"].Value;
                        var start = href.IndexOf("'") + 1;
                        var end = href.LastIndexOf("'");
                        var downloadLink = site.DownloadUrl + href.Substring(start, end - start);
                        return new
                        {
                            Title = titleElement.Attributes["title"].Value,
                            DownloadLink = downloadLink
                        };
                    }).ToList();

                    for (int i = 0; i < data.Count; i++)
                    {
                        Console.WriteLine($"{i+1} - {data[i].Title}");
                    }

                    Console.Write("input the number to download subtitle: ");
                    var selectedIndex = int.Parse(Console.ReadLine());

                    var selectedItem = data[selectedIndex - 1];
                    var fileName = HttpUtility.UrlDecode(selectedItem.DownloadLink.Substring(selectedItem.DownloadLink.LastIndexOf("/")+1));
                    var downloadLink = selectedItem.DownloadLink;
                    var filePath = Path.Combine(currentFolder, fileName);

                    var downloadResponse = await httpClient.GetAsync(downloadLink);

                    if (downloadResponse.StatusCode == HttpStatusCode.Redirect)
                    {
                        downloadLink = downloadResponse.Headers.Location.AbsoluteUri;
                        var bytes = await httpClient.GetByteArrayAsync(downloadLink);
                        await SaveSubtitle(filePath,bytes);
                    }
                    else
                    {
                        var bytes = await downloadResponse.Content.ReadAsByteArrayAsync();
                        await SaveSubtitle(filePath, bytes);
                    }
                    httpClient.Dispose();
                    break;
                }catch(Exception e)
                {
                    continue;
                }
            }

            Console.WriteLine("Hello World!");
        }

        static async Task SaveSubtitle(string filePath, byte[] bytes)
        {
            var file = new FileStream(filePath, FileMode.Create, FileAccess.Write);
            await file.WriteAsync(bytes, 0, bytes.Length);
            file.Close();
        }
    }

    class SubTitleSite
    {
        public string SearchUrl { get; set; }
        public string DownloadUrl { get; set; }
    }
}
