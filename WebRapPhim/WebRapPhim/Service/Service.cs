using Microsoft.Ajax.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using WebRapPhim.Models;

namespace WebRapPhim.Service
{
    public static class Service
    {
        public static AppXemPhimEntities1 db = new AppXemPhimEntities1();
        public static int getDateSubtract(DateTime start, DateTime end) {

            TimeSpan span = end.Subtract(start);
            return span.Days;
        }

        public static String getGhe(int row, int col,int idPhong)
        {
            char rowGhe = (char)(row+64);
            String id = idPhong.ToString() +rowGhe+"0"+col.ToString();
            Ghe g = db.Ghe.Where(x => x.ID == id).ToList().FirstOrDefault();

            if (g == null)
                return "";
            return g.ID;
            

        }

        public static int getRowPhong(int idPhong) {
            List<Ghe> g = db.Ghe.Where(x => x.PhongID == idPhong).ToList();
            List<String> row = new List<string>();
            foreach(Ghe ghe in g)
            {
                if(!row.Contains(ghe.ID.Substring(1,1)))
                    row.Add(ghe.ID.Substring(1, 1));
            }
            return row.Count;
        }
        public static int getColPhong(int idPhong, int row) {
            String rowGhe = ((char)(row + 64)).ToString();
            List<Ghe> g = db.Ghe.Where(x => x.PhongID == idPhong && x.ID.Substring(1,1) == rowGhe).ToList();
            List<String> col = new List<string>();
            
            
            foreach (Ghe ghe in g)
            {

                
                if (!col.Contains(ghe.ID.Substring(2, 2)))
                    col.Add(ghe.ID.Substring(1, 1));
            }
            return col.Count;
        }
        public static Customer GetCurrentCus(int id) {
            return db.Customer.First(x => x.ID == id);
        }
    }
}