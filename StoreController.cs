using Assignment3_API.Models;
using Assignment3_API.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Runtime.InteropServices;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;

namespace Assignment3_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StoreController : ControllerBase
    {
        private readonly IRepository _repository;
        public StoreController(IRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        [Route("ProductListing")]
        public async Task<ActionResult> ProductListing()
        {
            try
            {
                var results = await _repository.GetProductsAsync();

                var products = results.Select(p => new
                {
                    productId = p.ProductId,       // camelCase
                    name = p.Name,
                    price = p.Price,
                    description = p.Description,
                    brandName = p.Brand.Name,      // camelCase
                    productTypeName = p.ProductType.Name, // camelCase
                    image = p.Image
                });

                return Ok(products);
            }
            catch (Exception)
            {
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpPost, DisableRequestSizeLimit]
        [Route("AddProduct")]
        public async Task<IActionResult> AddProduct([FromForm] IFormCollection formData)
        {
            try
            {
                var formCollection = await Request.ReadFormAsync();

                var file = formCollection.Files.First();

                if (file.Length > 0)
                {

                    using (var ms = new MemoryStream())
                    {
                        file.CopyTo(ms);
                        var fileBytes = ms.ToArray();
                        string base64 = Convert.ToBase64String(fileBytes);

                        string price = formData["price"];
                        decimal num = decimal.Parse(price.Replace(".", ","));

                        var product = new Product
                        {
                            Price = num
                            ,
                            Name = formData["name"]
                            ,
                            Description = formData["description"]
                            ,
                            BrandId = Convert.ToInt32(formData["brand"])
                            ,
                            ProductTypeId = Convert.ToInt32(formData["producttype"])
                            ,
                            Image = base64
                            ,
                            DateCreated = DateTime.Now
                        };


                        _repository.Add(product);
                        await _repository.SaveChangesAsync();
                    }

                    return Ok();
                }
                else
                {
                    return BadRequest();
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex}");
            }
        }


        // Modify the Brands endpoint
        [HttpGet]
        [Route("Brands")]
        public async Task<ActionResult> Brands()
        {
            try
            {
                var results = await _repository.GetBrandsAsync();
                var response = results.Select(b => new {
                    brandId = b.BrandId,
                    name = b.Name
                });
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Error: {ex.Message}");
            }
        }

        // Modify the ProductTypes endpoint
        [HttpGet]
        [Route("ProductTypes")]
        public async Task<ActionResult> ProductTypes()
        {
            try
            {
                var results = await _repository.GetProductTypesAsync();
                var response = results.Select(pt => new {
                    productTypeId = pt.ProductTypeId,
                    name = pt.Name
                });
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Error: {ex.Message}");
            }
        }
        [HttpGet("ProductCountByBrand")]
        public async Task<ActionResult> GetProductCountByBrand()
        {
            try
            {
                var products = await _repository.GetProductsAsync();
                var results = products
                    .GroupBy(p => p.Brand.Name)
                    .Select(g => new
                    {
                        Brand = g.Key,
                        Count = g.Count()
                    })
                    .ToList();

                return Ok(results);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("ProductCountByProductType")]
        public async Task<ActionResult> GetProductCountByProductType()
        {
            try
            {
                var products = await _repository.GetProductsAsync();
                var results = products
                    .GroupBy(p => p.ProductType.Name)
                    .Select(g => new
                    {
                        ProductType = g.Key,
                        Count = g.Count()
                    })
                    .ToList();

                return Ok(results);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("TopExpensiveProducts")]
        public async Task<ActionResult> GetTopExpensiveProducts()
        {
            try
            {
                var products = await _repository.GetProductsAsync();
                var results = products
                    .OrderByDescending(p => p.Price)
                    .Take(10)
                    .Select(p => new
                    {
                        p.Name,
                        p.Price,
                        Brand = p.Brand.Name,
                        ProductType = p.ProductType.Name,
                        p.Description
                    })
                    .ToList();

                return Ok(results);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }

}

