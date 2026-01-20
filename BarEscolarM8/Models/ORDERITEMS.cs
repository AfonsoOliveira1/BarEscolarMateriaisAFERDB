namespace APiConsumer.Models
{
    public class ORDERITEMS
    {
        public int id { get; set; }
        public int? orderid { get; set; }
        public int? productid { get; set; } // pode referir-se a PRODUCTS.Id ou MENUDAY.Id dependendo de ismenu
        public bool? ismenu { get; set; }
        public int? quantity { get; set; }
        public string createdat { get; set; } // ou DateTime
        public decimal? unitprice { get; set; }
        public decimal? subtotal { get; set; }
        public int? menuid { get; set; }

        public ORDERS order { get; set; }
        public MENUDAY product { get; set; } // quando ismenu == true
        public PRODUCTS productnavigation { get; set; } // quando ismenu == false
    }
}