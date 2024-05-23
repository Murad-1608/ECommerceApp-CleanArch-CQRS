﻿using ECommerceApp.Application.Repositories;
using ECommerceApp.Application.RequestParameters;
using ECommerceApp.Application.ViewModels.Products;
using ECommerceApp.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Linq;
using MediatR;
using ECommerceApp.Application.Features.Queries.GetAllProduct;

namespace ECommerceApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductReadRepository _productReadRepository;
        private readonly IProductWriteRepository _productWriteRepository;
        IMediator _mediator;

        public ProductsController(IProductWriteRepository productWriteRepository,
                                  IProductReadRepository productReadRepository,
                                  IMediator mediator)
        {
            _productWriteRepository = productWriteRepository;
            _productReadRepository = productReadRepository;
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(GetAllProductQueryRequest getAllProductQueryRequest)
        {
            var response = await _mediator.Send(getAllProductQueryRequest);

            return Ok(response);

        }

        [HttpGet("{id}")]
        public IActionResult GetById(string id)
        {
            return Ok(_productReadRepository.GetByIdAsync(id, false));
        }
        [HttpPost]
        public async Task<IActionResult> Create(CreateProductViewModel viewModel)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { StatusCode = 400 });

            await _productWriteRepository.AddAsync(new Product()
            {
                Name = viewModel.Name,
                Stock = viewModel.Stock,
                Price = viewModel.Price
            });
            await _productWriteRepository.SaveAsync();
            return StatusCode((int)HttpStatusCode.Created);
        }

        [HttpPut]
        public async Task<IActionResult> Update(UpdateProductViewModel viewModel)
        {
            Product product = await _productReadRepository.GetByIdAsync(viewModel.Id);
            product.Name = viewModel.Name;
            product.Stock = viewModel.Stock;
            product.Price = viewModel.Price;
            await _productWriteRepository.SaveAsync();

            return Ok(new { StatusCode = 200 });
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(string id)
        {
            await _productWriteRepository.RemoveAsync(id);
            await _productWriteRepository.SaveAsync();

            return Ok(new { StatusCode = 200 });
        }

        [HttpPost("[action]")]
        public IActionResult Test(IFormFile file)
        {
            return Ok();
        }
    }
}
