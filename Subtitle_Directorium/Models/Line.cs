using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Subtitle_Directorium.Models
{
    public class Line
    {
        [Key]
        public int ID { get; set; }
        public int number { get; set; }
        public string timestamp { get; set; }
        public string text { get; set; }
        public int subtitleID { get; set; }
        public Line()
        {

        }
        public Line(int num, string str, string txt, int subID)
        {
            number = num;
            timestamp = str;
            text = txt;
            subtitleID = subID;
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
            foreach(char c in text)
            {
                if (!Char.IsDigit(c))
                    return false;
            }
            return true;
        }
        public long [] getTimes()
        {
            //00:00:02,000 --> 00:00:07,000
            string from = timestamp.Substring(0, 12);
            string to = timestamp.Substring(18, 12);
            string[] timesFrom = from.Split(':');
            long fromH = Convert.ToInt64(timesFrom[0]) * 60 * 60 * 1000;
            long fromM = Convert.ToInt64(timesFrom[1]) * 60 * 1000;
            string []timesFrom2 = timesFrom[2].Split(',');
            long fromS = Convert.ToInt64(timesFrom2[0]) * 1000;
            long fromMS = Convert.ToInt64(timesFrom2[0]);
            long fromTime = fromH + fromM + fromS + fromMS;
            string[] timesTo = to.Split(':');
            long toH = Convert.ToInt64(timesTo[0]) * 60 * 60 * 1000;
            long toM = Convert.ToInt64(timesTo[1]) * 60 * 1000;
            string[] timesTo2 = timesTo[2].Split(',');
            long toS = Convert.ToInt64(timesTo[0]) * 1000;
            long toMS = Convert.ToInt64(timesTo[0]);
            long toTime = toH + toM + toS + toMS;
            long[] vals = new long[2];
            vals[0] = fromTime;
            vals[1] = toTime;
            return vals;

        }
        public void delaySubtitles(int offset)
        {
            long[] times = getTimes();
            long fromTime = times[0] + offset;
            long toTimev = times[1] + offset;
            timestamp = toTime(fromTime) + " --> " + toTime(toTimev);
        }

        public string toTime(long timeInMs)
        {
            long ms = timeInMs % 1000;
            timeInMs /= 1000;
            long s = timeInMs % 60;
            timeInMs /= 60;
            long m = timeInMs % 60;
            timeInMs /= 60;
            long h = timeInMs % 100;
            return h.ToString("00") + ":" + m.ToString("00") + ":" + s.ToString("00") + "," + ms.ToString("000");
        }
        public override string ToString()
        {
            return number.ToString() + "\n" + timestamp + "\n" + text + "\n";
        }
    }
}