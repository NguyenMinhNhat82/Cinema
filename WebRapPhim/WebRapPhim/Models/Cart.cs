using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Web;
using System.Web.Management;

namespace WebRapPhim.Models
{

    public class cartItem { 
        public LoaiVe loaiVe { get; set; }
        public Film Phim { get; set; }
        public SuatChieu SuatChieu { get; set; }
        public Ghe ghe { get; set; }

        public double getGia() {
            return double.Parse((SuatChieu.Gia + this.SuatChieu.Gia * loaiVe.PhanTramCongThem/100).ToString());
        }
    }
    public class Cart
    {
        AppXemPhimEntities1 db = new AppXemPhimEntities1();
        List<cartItem> items = new List<cartItem>();
        public IEnumerable<cartItem> Items { get {  return items; } }
        public void Add(LoaiVe l,Film a, SuatChieu s, Ghe g) {

            var item = items.FirstOrDefault(x => x.ghe.ID == g.ID);
            if (item == null) {
                items.Add(new cartItem
                {
                    loaiVe = l,
                    Phim = a,
                    SuatChieu = s,
                    ghe = g
                });
            }

        }

        public double getTongGia() {
            double tonggia = 0;
            foreach (cartItem i in items) {
                tonggia += i.getGia();
            }
            return tonggia;
        }

        public double discount(double dis) { 
            double tong = this.getTongGia();
            tong = tong - tong*dis/100;

            return tong;
        }


        public String getListStringCartOfGhe() {
            String s = "";
            for (int i = 0; i < items.Count; i++)
            {
                if (i == 0)
                    s += items[i].ghe.ID;
                else
                    s += " ," + items[i].ghe.ID;
            }
            return s;
        }

        public  SuatChieu GetChieu()
        {
            return items[0].SuatChieu;
        }
        
        public void UpdateGhe(int index, String IDGhe) {

            Ghe gheUpdate = db.Ghe.First(x => x.ID == IDGhe);
            Items.ToList()[index].ghe = gheUpdate;
        }

        public List<LoaiVe> getListLoaiVe() {

            List<LoaiVe> l = new List<LoaiVe>();
            foreach (cartItem i in items) {
                if (!l.Contains(i.loaiVe))
                {
                    l.Add(i.loaiVe);
                }
            }

            return l;
        }

        public int getSoLuongOfLoaiVe(LoaiVe l) {
            int count = 0;
            foreach (cartItem i in items) {
                if (i.loaiVe.ID == l.ID) {
                    count++;
                }
            }
            return count;
        }

        public double getGiaOfLoaiVe(LoaiVe loaive)
        {
            double gia = 0;
            foreach (cartItem i in items)
            {
                if (i.loaiVe.ID == loaive.ID) 
                {
                    gia += i.getGia();
                }
            }
            return gia;
        }
    }
}