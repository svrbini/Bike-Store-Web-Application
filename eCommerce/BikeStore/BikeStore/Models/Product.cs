using System.Reflection;
using static System.Net.WebRequestMethods;

namespace BikeStore.Models
{
	public class Product
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public double Price { get; set; }
		public string Category { get; set; }
		public string Brand { get; set; }
		public int Quantity { get; set; }
		public short Year { get; set; }
		public bool InStock
		{
			get 
			{
				if (Quantity > 0) return true;
				else return false;
			}
		}
		public string ImgLink
		{
			get
			{
				switch (Category)
				{
					case "Mountain Bikes":
						return "https://sportmo.b-cdn.net/14796/marlin-6-gen-3-2024.jpg";
                    case "Children Bicycles":
                        return "https://media.trekbikes.com/image/upload/f_auto,fl_progressive:semi,q_auto,w_800,h_600,c_pad/SuperSprocket16Boys_22_35413_A_Primary";
                    case "Comfort Bicycles":
                        return "https://d2j6dbq0eux0bg.cloudfront.net/images/85153076/4023433346.webp";
                    case "Cruisers Bicycles":
                        return "https://www.sanvit.com/media/53/df/02/1701507461/CruiserGoStepThru_22_35209_B_Primary.webp";
                    case "Cyclocross Bicycles":
                        return "https://media.trekbikes.com/image/upload/c_pad,f_auto,w_800,h_600,q_auto,fl_strip_profile/Boone5-24-41655-A-Primary";
                    case "Electric Bikes":
                        return "https://sportmo.b-cdn.net/17327/powerfly-fs-4-gen-3-2024.jpg";
                    case "Road Bikes":
                        return "https://media.trekbikes.com/image/upload/c_pad,f_auto,w_800,h_600,q_auto,fl_strip_profile/DomanePlusAL5_23_36145_A_Primary";
                    default: return "";
				}
			}
		}

        public string GetShortName()
        {
			return Name.Replace(Brand,"").Replace(Year.ToString(),"").Replace("-","").Replace((Year - 1).ToString(), "").Replace("/", "").Trim();
        }

        public Product() { }

		public Product(int id, string name, double price, string category, string brand, int quantity, short year)
		{
			Id = id;
			Name = name;
			Price = price;
			Category = category;
			Brand = brand;
			Quantity = quantity;
			Year = year;
		}
	}
}
