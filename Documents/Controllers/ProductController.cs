using Dapper;
using Documents.Cache;
using Documents.Data;
using Documents.DocumentCommon;
using Documents.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace Documents.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    [SwaggerControllerOrder(8)]
    public class ProductController : ControllerBase
    {
        DBConnection conn = new DBConnection();
        private readonly DbContextClass _dbContext;
        private readonly ICacheService _cacheService;
        //https://www.c-sharpcorner.com/article/implementation-of-the-redis-cache-in-the-net-core-api/

        /*
            CREATE TABLE Products(
            ProductId int,
            ProductName varchar(50),
            ProductDescription varchar(100),
            Stock int
            )
         */
        public ProductController(DbContextClass dbContext, ICacheService cacheService)
        {
            _dbContext = dbContext;
            _cacheService = cacheService;
        }

        [HttpGet("products")]
        public IEnumerable<Product> Get()
        {
            var cacheData = _cacheService.GetData<IEnumerable<Product>>("product");
            if (cacheData != null)
            {
                return cacheData;
            }

            var expirationTime = DateTimeOffset.Now.AddMinutes(5.0);
            cacheData = _dbContext.Products.ToList();
            _cacheService.SetData<IEnumerable<Product>>("product", cacheData, expirationTime);
            return cacheData;
        }
       // [HttpGet("products2")]
        //public IEnumerable<Product> Get2()
        //{
        //    var cacheData = _cacheService.GetData<IEnumerable<Product>>("product");
        //    if (cacheData != null)
        //    {
        //        return cacheData;
        //    }
        //    lock (_lock)
        //    {
        //        var expirationTime = DateTimeOffset.Now.AddMinutes(5.0);
        //        cacheData = _dbContext.Products.ToList();
        //        _cacheService.SetData<IEnumerable<Product>>("product", cacheData, expirationTime);
        //    }
        //    return cacheData;
        //}
        [HttpGet("product")]
        public Product Get(int id)
        {
            Product filteredData;
            var cacheData = _cacheService.GetData<IEnumerable<Product>>("product");
            if (cacheData != null)
            {
                filteredData = cacheData.Where(x => x.ProductId == id).FirstOrDefault();
                return filteredData;
            }
            // Use raw SQL with Dapper
            using (var connection = new SqlConnection(conn.SAConnStrReader()))
            {
                var sql = "SELECT * FROM Products WHERE ProductId = @id";
                filteredData = connection.QueryFirstOrDefault<Product>(sql, new { id });
            }
           // filteredData = _dbContext.Products.Where(x => x.ProductId == id).FirstOrDefault();
            return filteredData;
        }

        [HttpPost("addproduct")]
        public async Task<Product> Post(Product value)
        {
            var obj = await _dbContext.Products.AddAsync(value);
            _cacheService.RemoveData("product");
            _dbContext.SaveChanges();
            return obj.Entity;
        }

        [HttpPut("updateproduct")]
        public void Put(Product product)
        {
            _dbContext.Products.Update(product);
            _cacheService.RemoveData("product");
            _dbContext.SaveChanges();
        }

        [HttpDelete("deleteproduct")]
        public void Delete(int Id)
        {
            var filteredData = _dbContext.Products.Where(x => x.ProductId == Id).FirstOrDefault();

            _dbContext.Remove(filteredData);
            _cacheService.RemoveData("product");
            _dbContext.SaveChanges();
        }
    }
}

