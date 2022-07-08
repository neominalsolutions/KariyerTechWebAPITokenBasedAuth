using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebAPISample.DTO;

namespace WebAPISample.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        public List<ProductDto> Products
        {
            get
            {
                return 
                    new List<ProductDto>
                    {

                        new ProductDto
                        {
                            Name = "Test1",
                            Price = 10,
                            Stock = 15
                        },
                        new ProductDto
                        {
                            Name = "Test2",
                            Price = 20,
                            Stock = 35
                        }
                    };
                
            }
        }

        // AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme
       
        [HttpGet]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> Get()
        {
            // commonly we get query ok result

            return Ok(Products);
        }

        [HttpGet("{Id}")]
        public async Task<IActionResult> Get(string Id)
        {
            // commonly we get query ok result

            return Ok(Products);
        }


        [HttpPost]
        public async Task<IActionResult> Post(ProductCreateDto model)
        {
            model.Id = Guid.NewGuid().ToString();
            // commonly we get query ok result

            return Created($"/api/product/{model.Id}",model);
        }
    }
}
