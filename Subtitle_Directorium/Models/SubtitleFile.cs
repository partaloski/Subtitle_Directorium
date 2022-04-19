using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;

namespace Subtitle_Directorium.Models
{
    public class SubtitleFile
    {
        [Key]
        public int ID { get; set; }
        public Line [] lines { get; set; }
        [Required]
        [Display(Name ="Language of subtitle")]
        public string language { get; set; }
        [Display(Name = "The raw content of the subtitle")]
        public string raw { get; set; }
        public int movieId { get; set; }
        public string movieName { get; set; }
        [Display(Name = ".srt file of the subtitle")]
        public HttpPostedFileBase SrtFile { get; set; }
        public void updateRaw(List<Line> lines)
        {
            string raw = "";
            foreach(Line line in lines)
            {
                raw += line.number + "\n" + line.timestamp + "\n" + line.text+"\n\n";
            }
            if(raw.Length != 0)
            {
                this.raw = raw;
            }
        }
        public void addLines(int id, string srt)
        {
            List<Line> lines = new List<Line>();
            string[] all = srt.Split(new string[] { "\r\n" }, StringSplitOptions.None);
            for (int i = 0; i < all.Length-2; i++) {
                if (all[i].Length != 0 && onlyNumbers(all[i].Trim()))
                {
                    int number = Convert.ToInt32(all[i].Trim());
                    string timestamp = all[i + 1];
                    string txt = "";
                    for(int j = i + 2; j < all.Length; j++)
                    {
                        if(all[j].Trim().Length == 0)
                        {
                            i = j;
                            break;
                        }
                        txt += all[j] + "\n";
                    }
                    lines.Add(new Line(number, timestamp, txt, id));
                }
            }
            this.lines = lines.ToArray();
        }
        public string readFile()
        {
            string raw = System.IO.File.ReadAllText(@"C:\Users\parta\Temp-Files-SRTs\temp.srt");
            this.raw = raw;
            return raw;
        }
        public SubtitleFile(MovieModel movieModel)
        {
            movieId = movieModel.ID;
            movieName = movieModel.Name;
        }
        public SubtitleFile()
        {
        }
        public SubtitleFile(string language, HttpPostedFileBase srtFile)
        {
            this.language = language;
            List<Line> lines = new List<Line>();
            StreamReader reader = new StreamReader(srtFile.InputStream);
            string number = "";
            while (true)
            {
                number = reader.ReadLine();
                if (number == null)
                    break;
                if (onlyNumbers(number))
                {
                    string timestamp = reader.ReadLine();
                    string txt = "";
                    string line = reader.ReadLine();
                    while(line != "")
                    {
                        txt += line + '\n';
                        line = reader.ReadLine();
                    }
                    int num = Convert.ToInt32(number);
                    lines.Add(new Line(num, timestamp, txt, this.ID));
                }
            }
            this.lines = lines.ToArray();
        }
        public SubtitleFile(string language)
        {
            this.language = language;
        }
        private static string[] getSplit(string line)
        {
            List<String> split = new List<string>();
            int lastSplit = 0;
            for (int i = 0; i < line.Length - 1; i++)
            {
                if (line[i].Equals('\\') && line[i + 1] == 'n')
                {
                    split.Add(line.Substring(lastSplit, (i - lastSplit)));
                    lastSplit = i + 2;
                }
                else if (i == line.Length - 2)
                    split.Add(line.Substring(lastSplit));
            }
            return split.ToArray();
        }
        private bool onlyNumbers(string text)
        {
            foreach (char c in text)
            {
                if (!Char.IsDigit(c))
                    return false;
            }
            return true;
        }
        public void delaySubtitles(int offset)
        {
            foreach(Line l in lines)
            {
                l.delaySubtitles(offset);
            }
        }
        public override string ToString()
        {
            return language + " with " + lines.Count() + " of lines.";
        }
    }
}