using System.Net;
using System.Data;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
 

using (WebClient wc = new WebClient())
{
    
   var json = wc.DownloadString(@"https://cdn.ayensoftware.com/00Coding/01/products.json");
   
    JArray jArray = JArray.Parse(json);
    

    var Sonuc = from product in jArray
            where product["Brand"].ToString() != "Adidas"
            where product["Brand"].ToString() != "adidas"
            group product by product["Color"] into Grup
            select new
            {
                Product = Grup,
                Color = Grup.Key
            };
   
    foreach (var item in Sonuc){

        foreach (var product in item.Product){
            AyenUrunler urun = new AyenUrunler();
            urun.Products=new List<Product>();
            Product currentProduct = new Product();

            currentProduct.Varyantlar = new List<Varyant>(); 
            var SKU = product["SKU"].ToString();
            var Color = product["Color"].ToString();
            var Size = product["Size"].ToString();


    using(var reader = new StreamReader(@"C:\Users\forev\Downloads\stock_prices.csv"))
    {
        List<string> listA = new List<string>();
        List<string> listB = new List<string>();
        List<string> listC = new List<string>();
        while (!reader.EndOfStream)
        {
            var line = reader.ReadLine();
            var values = line.Split(';');

            listA.Add(values[0]);
            listB.Add(values[1]);
            listC.Add(values[2]);
        }
        /* Console.WriteLine(listA[0] + " "+listB[0]+" "+ listC[0]); */
        int index = listA.FindIndex(a => a == SKU);
        if (index != -1) {
            /* Console.WriteLine(index); */
            urun.Products.Add(new Product{ProductCode=product["ProductCode"].ToString(),Name=product["Name"].ToString(),Brand=product["Brand"].ToString(),Stock=product.Count().ToString()});
            
            currentProduct.Varyantlar.Add(new Varyant { SKU = product["SKU"].ToString(), Price = listC[index], Stock = listB[index] });
        }
        else {
            Console.WriteLine("Element not found in the given list.");
        }
        }
           

        Varyant currentVaryant = new Varyant();
        currentVaryant.Ozellikler = new List<Ozellik>(); 
        currentVaryant.Ozellikler.Add(new Ozellik { Name = "Color", Value = Color });
        currentVaryant.Ozellikler.Add(new Ozellik { Name = "Size", Value = Size });

       
        string myjson = JsonSerializer.Serialize(urun.Products[0]);
        string myjson2 = JsonSerializer.Serialize(currentProduct.Varyantlar[0]);
        string myjson3 = JsonSerializer.Serialize(currentVaryant.Ozellikler[0]);
        
        string alljson=myjson+myjson2+myjson3;
        File.AppendAllText(@"C:\Users\forev\OneDrive\Masaüstü\test\TestConsoleApp\AyenUrunler.json", alljson);
        
        }
        
    }
    
}

public class AyenUrunler
{
    public List <Product> Products { get; set; } 
}

public class Product
{
    public string ProductCode { get; set; }
    public string Name { get; set; }

    public string Brand { get; set; }
    
    public string Stock { get; set; }

    public List<Varyant> Varyantlar { get; set; }
    
};

public class Varyant {
    public string SKU { get; set; }
  public string Stock { get; set; }
    public string Price { get; set; }
   public List <Ozellik> Ozellikler { get; set; }
};

public class Ozellik {
    public string Name { get; set; }
    
    public string Value { get; set; }
};
